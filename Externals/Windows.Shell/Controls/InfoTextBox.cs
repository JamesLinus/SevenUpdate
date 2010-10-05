// Copyright 2007-2010 Robert Baker, Seven Software.
// This file is part of Seven Update.
// Seven Update is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
// Seven Update is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.
// You should have received a copy of the GNU General Public License along with Seven Update.  If not, see <http://www.gnu.org/licenses/>.

namespace Microsoft.Windows.Controls
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Media;

    /// <summary>
    /// The adorner extensions
    /// </summary>
    public static class AdornerExtensions
    {
        #region Public Methods

        /// <summary>
        /// Determines whether the adorner layer contains an element
        /// </summary>
        /// <typeparam name="T">
        /// The type of element
        /// </typeparam>
        /// <param name="adr">
        /// The adorner.
        /// </param>
        /// <param name="elem">
        /// The element
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the adorner layer contains the element otherwise, <see langword="false"/>.
        /// </returns>
        public static bool Contains<T>(this AdornerLayer adr, UIElement elem)
        {
            if (adr == null)
            {
                return false;
            }

            var adorners = adr.GetAdorners(elem);

            if (adorners == null)
            {
                return false;
            }

            for (var i = adorners.Length - 1; i >= 0; i--)
            {
                if (adorners[i] is T)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Removes the adorners
        /// </summary>
        /// <typeparam name="T">
        /// The type of element
        /// </typeparam>
        /// <param name="adr">
        /// The adorner
        /// </param>
        /// <param name="elem">
        /// The element
        /// </param>
        public static void RemoveAdorners<T>(this AdornerLayer adr, UIElement elem)
        {
            var adorners = adr.GetAdorners(elem);

            if (adorners == null)
            {
                return;
            }

            for (var i = adorners.Length - 1; i >= 0; i--)
            {
                if (adorners[i] is T)
                {
                    adr.Remove(adorners[i]);
                }
            }
        }

        /// <summary>
        /// Removes all.
        /// </summary>
        /// <param name="adr">
        /// The adorner layer
        /// </param>
        /// <param name="elem">
        /// The element
        /// </param>
        public static void RemoveAll(this AdornerLayer adr, UIElement elem)
        {
            try
            {
                var adorners = adr.GetAdorners(elem);

                if (adorners == null)
                {
                    return;
                }

                foreach (var toRemove in adorners)
                {
                    adr.Remove(toRemove);
                }
            }
            catch
            {
            }
        }

        /// <summary>
        /// Removes all recursive.
        /// </summary>
        /// <param name="adr">
        /// The adorner layer
        /// </param>
        /// <param name="element">
        /// The element.
        /// </param>
        public static void RemoveAllRecursive(this AdornerLayer adr, UIElement element)
        {
            try
            {
                Action<UIElement> recurse = null;
                recurse = delegate(UIElement elem)
                    {
                        adr.RemoveAll(elem);
                        if (elem is Panel)
                        {
                            foreach (UIElement e in ((Panel)elem).Children)
                            {
                                recurse(e);
                            }
                        }
                        else if (elem is Decorator)
                        {
                            recurse(((Decorator)elem).Child);
                        }
                        else if (elem is ContentControl)
                        {
                            if (((ContentControl)elem).Content is UIElement)
                            {
                                recurse(((ContentControl)elem).Content as UIElement);
                            }
                        }
                    };

                recurse(element);
            }
            catch
            {
            }
        }

        #endregion
    }

    /// <summary>
    /// A <see cref="TextBox"/> that included help text
    /// </summary>
    [SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Control")]
    public sealed class InfoTextBox : TextBox
    {
        #region Constants and Fields

        /// <summary>
        ///   Indicates if the <see cref = "InfoTextBox" /> has text
        /// </summary>
        public static readonly DependencyProperty HasTextProperty = HasTextPropertyKey.DependencyProperty;

        /// <summary>
        ///   The text to display when there is no text in the <see cref = "InfoTextBox" />
        /// </summary>
        public static readonly DependencyProperty NoteProperty = DependencyProperty.Register(
            "Note", typeof(string), typeof(InfoTextBox), new UIPropertyMetadata(string.Empty, LabelPropertyChanged));

        /// <summary>
        ///   The style of the Note
        /// </summary>
        public static readonly DependencyProperty NoteStyleProperty = DependencyProperty.Register(
            "NoteStyle", typeof(Style), typeof(InfoTextBox), new UIPropertyMetadata(null));

        /// <summary>
        ///   Indicates if the <see cref = "InfoTextBox" /> has text
        /// </summary>
        private static readonly DependencyPropertyKey HasTextPropertyKey = DependencyProperty.RegisterReadOnly(
            "HasText", typeof(bool), typeof(InfoTextBox), new PropertyMetadata(false));

        /// <summary>
        ///   The adorner label
        /// </summary>
        private AdornerLabel myAdornerLabel;

        /// <summary>
        ///   The adorner layer
        /// </summary>
        private AdornerLayer myAdornerLayer;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the <see cref = "InfoTextBox" /> class.
        /// </summary>
        public InfoTextBox()
        {
            if (this.Resources.Count != 0)
            {
                return;
            }

            var resourceDictionary = new ResourceDictionary { Source = new Uri("/Windows.Shell;component/Resources/Dictionary.xaml", UriKind.Relative) };
            this.Resources.MergedDictionaries.Add(resourceDictionary);
        }

        #endregion

        #region Properties

        /// <summary>
        ///   Gets a value indicating whether this instance has text.
        /// </summary>
        /// <value>
        ///   <see langword = "true" /> if this instance has text; otherwise, <see langword = "false" />.
        /// </value>
        public bool HasText
        {
            get
            {
                return (bool)this.GetValue(HasTextProperty);
            }

            private set
            {
                this.SetValue(HasTextPropertyKey, value);
            }
        }

        /// <summary>
        ///   Gets or sets the note to display
        /// </summary>
        /// <value>The note to display</value>
        public string Note
        {
            get
            {
                return (string)this.GetValue(NoteProperty);
            }

            set
            {
                this.SetValue(NoteProperty, value);
            }
        }

        /// <summary>
        ///   Gets or sets the note style.
        /// </summary>
        /// <value>The note style.</value>
        public Style NoteStyle
        {
            get
            {
                return (Style)this.GetValue(NoteStyleProperty);
            }

            set
            {
                this.SetValue(NoteStyleProperty, value);
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Is called when a control template is applied.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.myAdornerLayer = AdornerLayer.GetAdornerLayer(this);
            this.myAdornerLabel = new AdornerLabel(this, this.Note, this.NoteStyle);
            this.UpdateAdorner(this);

            var focusProp = DependencyPropertyDescriptor.FromProperty(IsFocusedProperty, typeof(FrameworkElement));
            if (focusProp != null)
            {
                focusProp.AddValueChanged(this, delegate { this.UpdateAdorner(this); });
            }

            var containsTextProp = DependencyPropertyDescriptor.FromProperty(HasTextProperty, typeof(InfoTextBox));
            if (containsTextProp != null)
            {
                containsTextProp.AddValueChanged(this, delegate { this.UpdateAdorner(this); });
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Invoked whenever an unhandled System.Windows.DragDrop.DragEnter attached routed event reaches an element derived from this class in its route. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">
        /// Provides data about the event.
        /// </param>
        protected override void OnDragEnter(DragEventArgs e)
        {
            this.myAdornerLayer.RemoveAdorners<AdornerLabel>(this);

            base.OnDragEnter(e);
        }

        /// <summary>
        /// Invoked whenever an unhandled System.Windows.DragDrop.DragLeave attached routed event reaches an element derived from this class in its route. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">
        /// Provides data about the event.
        /// </param>
        protected override void OnDragLeave(DragEventArgs e)
        {
            this.UpdateAdorner(this);

            base.OnDragLeave(e);
        }

        /// <summary>
        /// Is called when content in this editing control changes.
        /// </summary>
        /// <param name="e">
        /// The arguments that are associated with the <see cref="E:System.Windows.Controls.Primitives.TextBoxBase.TextChanged"/> event.
        /// </param>
        protected override void OnTextChanged(TextChangedEventArgs e)
        {
            this.HasText = !(this.Text != null && String.IsNullOrEmpty(this.Text));

            base.OnTextChanged(e);
        }

        /// <summary>
        /// Determines whether the <see cref="InfoTextBox"/> is Visible
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The <see cref="System.EventArgs"/> instance containing the event data.
        /// </param>
        private new static void IsVisibleChanged(object sender, EventArgs e)
        {
            var infoTextBox = sender as InfoTextBox;
            if (infoTextBox == null)
            {
                return;
            }

            infoTextBox.UpdateAdorner(infoTextBox, !infoTextBox.IsVisible);
        }

        /// <summary>
        /// Updates the adorner when the label changes
        /// </summary>
        /// <param name="d">
        /// The dependency object
        /// </param>
        /// <param name="e">
        /// The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.
        /// </param>
        private static void LabelPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var infoTextBox = d as InfoTextBox;

            if (infoTextBox != null)
            {
                infoTextBox.UpdateAdorner(infoTextBox);
            }

            var isVisiblePropertyDescriptor = DependencyPropertyDescriptor.FromProperty(IsVisibleProperty, typeof(InfoTextBox));
            isVisiblePropertyDescriptor.AddValueChanged(d, IsVisibleChanged);
        }

        /// <summary>
        /// Updates the adorner.
        /// </summary>
        /// <param name="elem">
        /// The element
        /// </param>
        /// <param name="hide">
        /// if set to <see langword="true"/> hide the adorner
        /// </param>
        private void UpdateAdorner(FrameworkElement elem, bool hide = false)
        {
            if (elem == null || this.myAdornerLayer == null)
            {
                return;
            }

            this.myAdornerLabel = new AdornerLabel(this, this.Note, this.NoteStyle);
            this.myAdornerLayer.RemoveAdorners<AdornerLabel>(elem);

            if (!((InfoTextBox)elem).HasText && !elem.IsFocused && !hide)
            {
                this.myAdornerLayer.Add(this.myAdornerLabel);
            }
        }

        #endregion
    }

    /// <summary>
    /// The label to display on the <see cref="InfoTextBox"/>
    /// </summary>
    [SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Extension class")]
    public class AdornerLabel : Adorner
    {
        #region Constants and Fields

        /// <summary>
        ///   The textBlock
        /// </summary>
        private readonly TextBlock textBlock;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AdornerLabel"/> class.
        /// </summary>
        /// <param name="adornedElement">
        /// The adorned element.
        /// </param>
        /// <param name="label">
        /// The label.
        /// </param>
        /// <param name="labelStyle">
        /// The label style.
        /// </param>
        public AdornerLabel(UIElement adornedElement, string label, Style labelStyle)
            : base(adornedElement)
        {
            this.textBlock = new TextBlock { Style = labelStyle, Text = label };
        }

        #endregion

        #region Properties

        /// <summary>
        ///   Gets the number of visual child elements within this element.
        /// </summary>
        /// <value></value>
        /// <returns>The number of visual child elements for this element.</returns>
        protected override int VisualChildrenCount
        {
            get
            {
                return 1;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// When overridden in a derived class, positions child elements and determines a size for a <see cref="T:System.Windows.FrameworkElement"/> derived class.
        /// </summary>
        /// <param name="finalSize">
        /// The final area within the parent that this element should use to arrange itself and its children.
        /// </param>
        /// <returns>
        /// The actual size used.
        /// </returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            this.textBlock.Arrange(new Rect(finalSize));
            return finalSize;
        }

        /// <summary>
        /// Overrides <see cref="M:System.Windows.Media.Visual.GetVisualChild(System.Int32)"/>, and returns a child at the specified index from a collection of child elements.
        /// </summary>
        /// <param name="index">
        /// The zero-based index of the requested child element in the collection.
        /// </param>
        /// <returns>
        /// The requested child element. This should not return <see langword="null"/>; if the provided index is out of range, an exception is thrown.
        /// </returns>
        protected override Visual GetVisualChild(int index)
        {
            return this.textBlock;
        }

        /// <summary>
        /// Implements any custom measuring behavior for the adorner.
        /// </summary>
        /// <param name="constraint">
        /// A size to constrain the adorner to.
        /// </param>
        /// <returns>
        /// A <see cref="T:System.Windows.Size"/> object representing the amount of layout space needed by the adorner.
        /// </returns>
        protected override Size MeasureOverride(Size constraint)
        {
            this.textBlock.Measure(constraint);
            return constraint;
        }

        #endregion

        // return the count of the visuals
    }
}