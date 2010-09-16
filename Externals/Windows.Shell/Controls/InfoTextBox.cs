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
    public sealed class InfoTextBox : TextBox
    {
        #region Constructors

        public InfoTextBox()
        {
            if (Resources.Count != 0)
                return;
            var resourceDictionary = new ResourceDictionary {Source = new Uri("/Windows.Shell;component/Resources/Dictionary.xaml", UriKind.Relative)};
            Resources.MergedDictionaries.Add(resourceDictionary);
        }

        #endregion

        #region Properties

        // Using a DependencyProperty as the backing store for Label.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LabelProperty = DependencyProperty.Register("Label", typeof (string), typeof (InfoTextBox), new UIPropertyMetadata("", LabelPropertyChanged));

        // Using a DependencyProperty as the backing store for LabelStyle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LabelStyleProperty = DependencyProperty.Register("LabelStyle", typeof (Style), typeof (InfoTextBox), new UIPropertyMetadata(null));

        private static readonly DependencyPropertyKey HasTextPropertyKey = DependencyProperty.RegisterReadOnly("HasText", typeof (bool), typeof (InfoTextBox), new PropertyMetadata(false));

        public static readonly DependencyProperty HasTextProperty = HasTextPropertyKey.DependencyProperty;

        public string Label { get { return (string) GetValue(LabelProperty); } set { SetValue(LabelProperty, value); } }
        public Style LabelStyle { get { return (Style) GetValue(LabelStyleProperty); } set { SetValue(LabelStyleProperty, value); } }
        public bool HasText { get { return (bool) GetValue(HasTextProperty); } private set { SetValue(HasTextPropertyKey, value); } }

        #endregion

        #region Callbacks

        private static void LabelPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var infoTextBox = d as InfoTextBox;

            if (infoTextBox != null)
                infoTextBox.UpdateAdorner(infoTextBox);
            var isVisiblePropertyDescriptor = DependencyPropertyDescriptor.FromProperty(IsVisibleProperty, typeof (InfoTextBox));
            isVisiblePropertyDescriptor.AddValueChanged(d, IsVisibleChanged);
        }

        #endregion

        private AdornerLabel myAdornerLabel;
        private AdornerLayer myAdornerLayer;

        private new static void IsVisibleChanged(object sender, EventArgs e)
        {
            var infoTextBox = sender as InfoTextBox;
            if (infoTextBox == null)
                return;

            infoTextBox.UpdateAdorner(infoTextBox, !infoTextBox.IsVisible);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            myAdornerLayer = AdornerLayer.GetAdornerLayer(this);
            myAdornerLabel = new AdornerLabel(this, Label, LabelStyle);
            UpdateAdorner(this);

            var focusProp = DependencyPropertyDescriptor.FromProperty(IsFocusedProperty, typeof (FrameworkElement));
            if (focusProp != null)
                focusProp.AddValueChanged(this, delegate { UpdateAdorner(this); });

            var containsTextProp = DependencyPropertyDescriptor.FromProperty(HasTextProperty, typeof (InfoTextBox));
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

        private void UpdateAdorner(FrameworkElement elem, bool hide = false)
        {
            if (elem == null || myAdornerLayer == null)
                return;

            myAdornerLabel = new AdornerLabel(this, Label, LabelStyle);
            myAdornerLayer.RemoveAdorners<AdornerLabel>(elem);

            if (!((InfoTextBox) elem).HasText && !elem.IsFocused && !hide)
                myAdornerLayer.Add(myAdornerLabel);
        }
    }

    // Adorners must subclass the abstract base class Adorner.
    public class AdornerLabel : Adorner
    {
        private readonly TextBlock textBlock;

        // Be sure to call the base class constructor.
        public AdornerLabel(UIElement adornedElement, string label, Style labelStyle) : base(adornedElement)
        {
            textBlock = new TextBlock {Style = labelStyle, Text = label};
        }

        protected override int VisualChildrenCount { get { return 1; } }

        //make sure that the layout system knows of the element
        protected override Size MeasureOverride(Size constraint)
        {
            textBlock.Measure(constraint);
            return constraint;
        }

        //make sure that the layout system knows of the element
        protected override Size ArrangeOverride(Size finalSize)
        {
            textBlock.Arrange(new Rect(finalSize));
            return finalSize;
        }

        //return the visual that we want to display
        protected override Visual GetVisualChild(int index)
        {
            return textBlock;
        }

        //return the count of the visuals
    }

    public static class AdornerExtensions
    {
        public static void RemoveAdorners<T>(this AdornerLayer adr, UIElement elem)
        {
            var adorners = adr.GetAdorners(elem);

            if (adorners == null)
                return;

            for (var i = adorners.Length - 1; i >= 0; i--)
            {
                if (adorners[i] is T)
                    adr.Remove(adorners[i]);
            }
        }

        public static bool Contains<T>(this AdornerLayer adr, UIElement elem)
        {
            if (adr == null)
                return false;
            var adorners = adr.GetAdorners(elem);

            if (adorners == null)
                return false;

            for (var i = adorners.Length - 1; i >= 0; i--)
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
                var adorners = adr.GetAdorners(elem);

                if (adorners == null)
                    return;

                foreach (var toRemove in adorners)
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