// <copyright file="InfoTextBox.cs" project="SevenSoftware.Windows">Ben Dewey</copyright>
// <license href="http://www.gnu.org/licenses/gpl-3.0.txt" name="GNU General Public License" />

using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace SevenSoftware.Windows.Controls
{
    /// <summary>A <c>TextBox</c> that includes help text and error indicators.</summary>
    public sealed class InfoTextBox : TextBox
    {
        /// <summary>Indicates if the <c>InfoTextBox</c> has an error.</summary>
        static readonly DependencyProperty HasErrorProperty = DependencyProperty.Register(
            "HasError", typeof(bool), typeof(InfoTextBox), new PropertyMetadata(false));

        /// <summary>Indicates if the <c>InfoTextBox</c> has text.</summary>
        static readonly DependencyProperty HasTextProperty = DependencyProperty.Register(
            "HasText", typeof(bool), typeof(InfoTextBox), new PropertyMetadata(false));

        /// <summary>Indicates if the <c>InfoTextBox</c> has a warning.</summary>
        static readonly DependencyProperty HasWarningProperty = DependencyProperty.Register(
            "HasWarning", typeof(bool), typeof(InfoTextBox), new PropertyMetadata(false));

        /// <summary>The text to display when there is no text in the <c>InfoTextBox</c>.</summary>
        static readonly DependencyProperty NoteProperty = DependencyProperty.Register(
            "Note", typeof(string), typeof(InfoTextBox), new UIPropertyMetadata(string.Empty, NotePropertyChanged));

        /// <summary>The style of the Note.</summary>
        static readonly DependencyProperty NoteStyleProperty = DependencyProperty.Register(
            "NoteStyle", typeof(Style), typeof(InfoTextBox), new UIPropertyMetadata(null));

        /// <summary>The adorner label.</summary>
        AdornerLabel myAdornerLabel;

        /// <summary>The adorner layer.</summary>
        AdornerLayer myAdornerLayer;

        /// <summary>Initializes a new instance of the <see cref="InfoTextBox" /> class.</summary>
        public InfoTextBox()
        {
            if (Resources.Count != 0)
            {
                return;
            }

            var resourceDictionary = new ResourceDictionary
                {
                   Source = new Uri("/SevenSoftware.Windows;component/Resources/Dictionary.xaml", UriKind.Relative) 
                };
            Resources.MergedDictionaries.Add(resourceDictionary);
        }

        /// <summary>Gets or sets a value indicating whether the input has a validation error.</summary>
        public bool HasError
        {
            get { return (bool)GetValue(HasErrorProperty); }

            set { SetValue(HasErrorProperty, value); }
        }

        /// <summary>Gets or sets a value indicating whether the input has a validation warning.</summary>
        public bool HasWarning
        {
            get { return (bool)GetValue(HasWarningProperty); }

            set { SetValue(HasWarningProperty, value); }
        }

        /// <summary>Gets or sets the note to display.</summary>
        /// <value>The note to display.</value>
        public string Note
        {
            get { return (string)GetValue(NoteProperty); }

            set { SetValue(NoteProperty, value); }
        }

        /// <summary>Gets or sets the note style.</summary>
        /// <value>The note style.</value>
        public Style NoteStyle
        {
            get { return (Style)GetValue(NoteStyleProperty); }

            set { SetValue(NoteStyleProperty, value); }
        }

        /// <summary>Gets or sets a value indicating whether this instance has text.</summary>
        /// <value><c>True</c> if this instance has text; otherwise, <c>False</c>.</value>
        bool HasText
        {
            get
            {
                if (Validation.GetHasError(this))
                {
                    return true;
                }

                return (bool)GetValue(HasTextProperty);
            }

            set { SetValue(HasTextProperty, value); }
        }

        /// <summary>Is called when a control template is applied.</summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            myAdornerLayer = AdornerLayer.GetAdornerLayer(this);
            myAdornerLabel = new AdornerLabel(this, Note, NoteStyle);
            UpdateAdorner(this);

            DependencyPropertyDescriptor focusProp = DependencyPropertyDescriptor.FromProperty(
                IsFocusedProperty, typeof(FrameworkElement));
            if (focusProp != null)
            {
                focusProp.AddValueChanged(this, delegate { UpdateAdorner(this); });
            }

            DependencyPropertyDescriptor containsTextProp = DependencyPropertyDescriptor.FromProperty(
                HasTextProperty, typeof(InfoTextBox));
            if (containsTextProp != null)
            {
                containsTextProp.AddValueChanged(this, delegate { UpdateAdorner(this); });
            }
        }

        /// <summary>
        ///   Invoked whenever an unhandled <c>DragDrop</c>.DragEnter attached routed event reaches an element derived
        ///   from this class in its route. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">Provides data about the event.</param>
        protected override void OnDragEnter(DragEventArgs e)
        {
            RemoveAdorners<AdornerLabel>(myAdornerLayer, this);

            base.OnDragEnter(e);
        }

        /// <summary>
        ///   Invoked whenever an unhandled <c>DragDrop</c>.DragLeave attached routed event reaches an element derived
        ///   from this class in its route. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">Provides data about the event.</param>
        protected override void OnDragLeave(DragEventArgs e)
        {
            UpdateAdorner(this);

            base.OnDragLeave(e);
        }

        /// <summary>Is called when content in this editing control changes.</summary>
        /// <param name="e">The arguments that are associated with the <see cref="E:System.Windows.Controls.Primitives.TextBoxBase.TextChanged" /> event.</param>
        protected override void OnTextChanged(TextChangedEventArgs e)
        {
            HasText = !(Text != null && string.IsNullOrEmpty(Text));

            if (HasText)
            {
                UpdateAdorner(this, true);
            }

            base.OnTextChanged(e);
        }

        /// <summary>Determines whether the <c>InfoTextBox</c> is Visible.</summary>
        /// <param name="sender">The object that called the event.</param>
        /// <param name="e">The <c>System.EventArgs</c> instance containing the event data.</param>
        static new void IsVisibleChanged(object sender, EventArgs e)
        {
            var infoTextBox = sender as InfoTextBox;
            if (infoTextBox == null)
            {
                return;
            }

            infoTextBox.UpdateAdorner(infoTextBox, !infoTextBox.IsVisible);
        }

        /// <summary>Updates the adorner when the label changes.</summary>
        /// <param name="d">The dependency object.</param>
        /// <param name="e">The <c>System.Windows.DependencyPropertyChangedEventArgs</c> instance containing the event data.</param>
        static void NotePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var infoTextBox = d as InfoTextBox;

            if (infoTextBox != null)
            {
                infoTextBox.UpdateAdorner(infoTextBox);
            }

            DependencyPropertyDescriptor isVisiblePropertyDescriptor =
                DependencyPropertyDescriptor.FromProperty(IsVisibleProperty, typeof(InfoTextBox));
            isVisiblePropertyDescriptor.AddValueChanged(d, IsVisibleChanged);
        }

        /// <summary>Removes the adorners.</summary>
        /// <param name="adorner">The adorner.</param>
        /// <param name="element">The element.</param>
        /// <typeparameter name="T">The type of element</typeparameter>
        /// <typeparam name="T">The type of element.</typeparam>
        static void RemoveAdorners<T>(AdornerLayer adorner, UIElement element)
        {
            if (adorner == null)
            {
                throw new ArgumentNullException("adorner");
            }

            if (element == null)
            {
                throw new ArgumentNullException("element");
            }

            Adorner[] adorners = adorner.GetAdorners(element);

            if (adorners == null)
            {
                return;
            }

            for (int i = adorners.Length - 1; i >= 0; i--)
            {
                if (adorners[i] is T)
                {
                    adorner.Remove(adorners[i]);
                }
            }
        }

        /// <summary>Updates the adorner.</summary>
        /// <param name="element">The element.</param>
        /// <param name="hide">If set to <c>True</c> hide the adorner.</param>
        void UpdateAdorner(FrameworkElement element, bool hide = false)
        {
            if (element == null || myAdornerLayer == null)
            {
                return;
            }

            myAdornerLabel = new AdornerLabel(this, Note, NoteStyle);
            RemoveAdorners<AdornerLabel>(myAdornerLayer, element);

            if (!((InfoTextBox)element).HasText && !element.IsFocused && !hide)
            {
                myAdornerLayer.Add(myAdornerLabel);
            }
        }
    }
}