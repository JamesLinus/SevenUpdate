// ***********************************************************************
// <copyright file="InfoTextBox.cs"
//            project="System.Windows"
//            assembly="System.Windows"
//            solution="SevenUpdate"
//            company="Seven Software">
//     Copyright (c) Ben Dewey. All rights reserved.
// </copyright>
// <author>Ben Dewey</author>
// <author username="sevenalive">Robert Baker</author>
// <license href="http://www.gnu.org/licenses/gpl-3.0.txt" name="GNU General Public License 3">
//  This file is part of Seven Update.
//
//    Seven Update is free software: you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation, either version 3 of the License, or
//    (at your option) any later version.
//
//    Seven Update is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.
//
//    You should have received a copy of the GNU General Public License
//    along with Seven Update.  If not, see http://www.gnu.org/licenses/.
// </license>
// ***********************************************************************
namespace System.Windows.Controls
{
    using System.ComponentModel;
    using System.Windows.Documents;

    /// <summary>A <see cref="TextBox"/> that included help text</summary>
    public sealed class InfoTextBox : TextBox
    {
        #region Constants and Fields

        /// <summary>The text to display when there is no text in the <see cref = "InfoTextBox" /></summary>
        private static readonly DependencyProperty NoteProperty = DependencyProperty.Register("Note", typeof(string), typeof(InfoTextBox), new UIPropertyMetadata(string.Empty, LabelPropertyChanged));

        /// <summary>The style of the Note</summary>
        private static readonly DependencyProperty NoteStyleProperty = DependencyProperty.Register("NoteStyle", typeof(Style), typeof(InfoTextBox), new UIPropertyMetadata(null));

        /// <summary>Indicates if the <see cref = "InfoTextBox" /> has an error</summary>
        private static readonly DependencyProperty HasErrorProperty = DependencyProperty.Register("HasError", typeof(bool), typeof(InfoTextBox), new PropertyMetadata(false, HasErrorPropertyChanged));

        /// <summary>Indicates if the <see cref = "InfoTextBox" /> has text</summary>
        private static readonly DependencyProperty HasTextProperty = DependencyProperty.Register("HasText", typeof(bool), typeof(InfoTextBox), new PropertyMetadata(false));

        /// <summary>The adorner label</summary>
        private AdornerLabel myAdornerLabel;

        /// <summary>The adorner layer</summary>
        private AdornerLayer myAdornerLayer;

        #endregion

        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref = "InfoTextBox" /> class.</summary>
        public InfoTextBox()
        {
            if (this.Resources.Count != 0)
            {
                return;
            }

            var resourceDictionary = new ResourceDictionary
                {
                    Source = new Uri("/System.Windows;component/Resources/Dictionary.xaml", UriKind.Relative)
                };
            this.Resources.MergedDictionaries.Add(resourceDictionary);
        }

        #endregion

        #region Properties

        /// <summary>Gets or sets a value indicating whether the input has a validation error</summary>
        public bool HasError
        {
            get
            {
                return (bool)this.GetValue(HasErrorProperty);
            }

            set
            {
                this.SetValue(HasErrorProperty, value);
            }
        }

        /// <summary>Gets or sets the note to display</summary>
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

        /// <summary>Gets or sets the note style.</summary>
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

        /// <summary>Gets or sets a value indicating whether this instance has text.</summary>
        /// <value><see langword = "true" /> if this instance has text; otherwise, <see langword = "false" />.</value>
        private bool HasText
        {
            get
            {
                if (Validation.GetHasError(this))
                {
                    return true;
                }

                return (bool)this.GetValue(HasTextProperty);
            }

            set
            {
                this.SetValue(HasTextProperty, value);
            }
        }

        #endregion

        #region Public Methods

        /// <summary>Is called when a control template is applied.</summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.myAdornerLayer = AdornerLayer.GetAdornerLayer(this);
            this.myAdornerLabel = new AdornerLabel(this, this.Note, this.NoteStyle);
            this.UpdateAdorner(this);

            var focusProp = DependencyPropertyDescriptor.FromProperty(IsFocusedProperty, typeof(FrameworkElement));
            if (focusProp != null)
            {
                focusProp.AddValueChanged(
                    this,
                    delegate
                        {
                            this.UpdateAdorner(this);
                        });
            }

            var containsTextProp = DependencyPropertyDescriptor.FromProperty(HasTextProperty, typeof(InfoTextBox));
            if (containsTextProp != null)
            {
                containsTextProp.AddValueChanged(
                    this,
                    delegate
                        {
                            this.UpdateAdorner(this);
                        });
            }
        }

        #endregion

        #region Methods

        /// <summary>Invoked whenever an unhandled <see cref="DragDrop"/>.DragEnter attached routed event reaches an element derived from this class in its route. Implement this method to add class handling for this event.</summary>
        /// <param name="e">Provides data about the event.</param>
        protected override void OnDragEnter(DragEventArgs e)
        {
            this.myAdornerLayer.RemoveAdorners<AdornerLabel>(this);

            base.OnDragEnter(e);
        }

        /// <summary>Invoked whenever an unhandled <see cref="DragDrop"/>.DragLeave attached routed event reaches an element derived from this class in its route. Implement this method to add class handling for this event.</summary>
        /// <param name="e">Provides data about the event.</param>
        protected override void OnDragLeave(DragEventArgs e)
        {
            this.UpdateAdorner(this);

            base.OnDragLeave(e);
        }

        /// <summary>Is called when content in this editing control changes.</summary>
        /// <param name="e">The arguments that are associated with the <see cref="E:System.Windows.Controls.Primitives.TextBoxBase.TextChanged"/> event.</param>
        protected override void OnTextChanged(TextChangedEventArgs e)
        {
            this.HasText = !(this.Text != null && String.IsNullOrEmpty(this.Text));

            base.OnTextChanged(e);
        }

        /// <summary>Determines whether the <see cref="InfoTextBox"/> is Visible</summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private static new void IsVisibleChanged(object sender, EventArgs e)
        {
            var infoTextBox = sender as InfoTextBox;
            if (infoTextBox == null)
            {
                return;
            }

            infoTextBox.UpdateAdorner(infoTextBox, !infoTextBox.IsVisible);
        }

        /// <summary>Updates the style when the HasError property changes</summary>
        /// <param name="d">The dependency object</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void HasErrorPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }

        /// <summary>Updates the adorner when the label changes</summary>
        /// <param name="d">The dependency object</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
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

        /// <summary>Updates the adorner.</summary>
        /// <param name="element">The element</param>
        /// <param name="hide">if set to <see langword="true"/> hide the adorner</param>
        private void UpdateAdorner(FrameworkElement element, bool hide = false)
        {
            if (element == null || this.myAdornerLayer == null)
            {
                return;
            }

            this.myAdornerLabel = new AdornerLabel(this, this.Note, this.NoteStyle);
            this.myAdornerLayer.RemoveAdorners<AdornerLabel>(element);

            if (!((InfoTextBox)element).HasText && !element.IsFocused && !hide)
            {
                this.myAdornerLayer.Add(this.myAdornerLabel);
            }
        }

        #endregion
    }
}