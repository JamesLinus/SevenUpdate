#region GNU Public License Version 3

// Copyright 2007-2010 Robert Baker, Seven Software.
// This file is part of Seven Update.
//   
//      Seven Update is free software: you can redistribute it and/or modify
//      it under the terms of the GNU General Public License as published by
//      the Free Software Foundation, either version 3 of the License, or
//      (at your option) any later version.
//  
//      Seven Update is distributed in the hope that it will be useful,
//      but WITHOUT ANY WARRANTY; without even the implied warranty of
//      MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//      GNU General Public License for more details.
//   
//      You should have received a copy of the GNU General Public License
//      along with Seven Update.  If not, see <http://www.gnu.org/licenses/>.

#endregion

#region

using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

#endregion

namespace Microsoft.Windows.Controls
{
    public class InfoTextBox : TextBox
    {
        #region Properties

        // Using a DependencyProperty as the backing store for Label.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LabelProperty = DependencyProperty.Register("Label", typeof (string), typeof (InfoTextBox), new UIPropertyMetadata("Label"));

        // Using a DependencyProperty as the backing store for LabelStyle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LabelStyleProperty = DependencyProperty.Register("LabelStyle", typeof (Style), typeof (InfoTextBox), new UIPropertyMetadata(null));

        private static readonly DependencyPropertyKey HasTextPropertyKey = DependencyProperty.RegisterReadOnly("HasText", typeof (bool), typeof (InfoTextBox),
                                                                                                               new PropertyMetadata(false));

        public static readonly DependencyProperty HasTextProperty = HasTextPropertyKey.DependencyProperty;
        public string Label { get { return (string) GetValue(LabelProperty); } set { SetValue(LabelProperty, value); } }
        public Style LabelStyle { get { return (Style) GetValue(LabelStyleProperty); } set { SetValue(LabelStyleProperty, value); } }
        public bool HasText { get { return (bool) GetValue(HasTextProperty); } private set { SetValue(HasTextPropertyKey, value); } }

        #endregion

        private AdornerLabel myAdornerLabel;
        private AdornerLayer myAdornerLayer;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            myAdornerLayer = AdornerLayer.GetAdornerLayer(this);
            myAdornerLabel = new AdornerLabel(this, Label, LabelStyle);
            UpdateAdorner(this);

            DependencyPropertyDescriptor focusProp = DependencyPropertyDescriptor.FromProperty(IsFocusedProperty, typeof (FrameworkElement));
            if (focusProp != null)
                focusProp.AddValueChanged(this, delegate { UpdateAdorner(this); });

            DependencyPropertyDescriptor containsTextProp = DependencyPropertyDescriptor.FromProperty(HasTextProperty, typeof (InfoTextBox));
            if (containsTextProp != null)
                containsTextProp.AddValueChanged(this, delegate { UpdateAdorner(this); });
        }

        protected override void OnTextChanged(TextChangedEventArgs e)
        {
            HasText = Text != "";

            base.OnTextChanged(e);
        }

        protected override void OnDragEnter(DragEventArgs e)
        {
            myAdornerLayer.RemoveAdorners<AdornerLabel>(this);

            base.OnDragEnter(e);
        }

        protected override void OnDragLeave(DragEventArgs e)
        {
            UpdateAdorner(this);

            base.OnDragLeave(e);
        }

        private void UpdateAdorner(FrameworkElement elem)
        {
            if (elem == null || myAdornerLayer == null)
                return;
            if (((InfoTextBox) elem).HasText || elem.IsFocused)
            {
                // Hide the Shadowed Label
                ToolTip = Label;
                myAdornerLayer.RemoveAdorners<AdornerLabel>(elem);
            }
            else
            {
                // Show the Shadowed Label
                ToolTip = null;
                if (!myAdornerLayer.Contains<AdornerLabel>(elem))
                    myAdornerLayer.Add(myAdornerLabel);
            }
        }
    }

    // Adorners must subclass the abstract base class Adorner.
    public class AdornerLabel : Adorner
    {
        private TextBlock _textBlock;

        // Be sure to call the base class constructor.
        public AdornerLabel(UIElement adornedElement, string label, Style labelStyle) : base(adornedElement)
        {
            _textBlock = new TextBlock();
            _textBlock.Style = labelStyle;
            _textBlock.Text = label;
        }

        protected override int VisualChildrenCount { get { return 1; } }

        //make sure that the layout system knows of the element
        protected override Size MeasureOverride(Size constraint)
        {
            _textBlock.Measure(constraint);
            return constraint;
        }

        //make sure that the layout system knows of the element
        protected override Size ArrangeOverride(Size finalSize)
        {
            _textBlock.Arrange(new Rect(finalSize));
            return finalSize;
        }

        //return the visual that we want to display
        protected override Visual GetVisualChild(int index)
        {
            return _textBlock;
        }

        //return the count of the visuals
    }

    public static class AdornerExtensions
    {
        public static void RemoveAdorners<T>(this AdornerLayer adr, UIElement elem)
        {
            Adorner[] adorners = adr.GetAdorners(elem);

            if (adorners == null)
                return;

            for (int i = adorners.Length - 1; i >= 0; i--)
            {
                if (adorners[i] is T)
                    adr.Remove(adorners[i]);
            }
        }

        public static bool Contains<T>(this AdornerLayer adr, UIElement elem)
        {
            if (adr == null)
                return false;
            Adorner[] adorners = adr.GetAdorners(elem);

            if (adorners == null)
                return false;

            for (int i = adorners.Length - 1; i >= 0; i--)
            {
                if (adorners[i] is T)
                    return true;
            }
            return false;
        }

        public static void RemoveAll(this AdornerLayer adr, UIElement elem)
        {
            try
            {
                Adorner[] adorners = adr.GetAdorners(elem);

                if (adorners == null)
                    return;

                foreach (Adorner toRemove in adorners)
                    adr.Remove(toRemove);
            }
            catch
            {
            }
        }

        public static void RemoveAllRecursive(this AdornerLayer adr, UIElement element)
        {
            try
            {
                Action<UIElement> recurse = null;
                recurse = (delegate(UIElement elem)
                               {
                                   adr.RemoveAll(elem);
                                   if (elem is Panel)
                                   {
                                       foreach (UIElement e in ((Panel) elem).Children)
                                           recurse(e);
                                   }
                                   else if (elem is Decorator)
                                       recurse(((Decorator) elem).Child);
                                   else if (elem is ContentControl)
                                   {
                                       if (((ContentControl) elem).Content is UIElement)
                                           recurse(((ContentControl) elem).Content as UIElement);
                                   }
                               });

                recurse(element);
            }
            catch
            {
            }
        }
    }
}