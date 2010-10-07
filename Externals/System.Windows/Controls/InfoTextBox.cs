// ***********************************************************************
// Assembly         : System.Windows
// Author           : Robert Baker (sevenalive)
// Last Modified By : Robert Baker (sevenalive)
// Last Modified On : 10-06-2010
// Copyright        : (c) Seven Software. All rights reserved.
// ***********************************************************************
namespace System.Windows.Controls
{
    using System.ComponentModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Windows.Documents;

    /// <summary>
    /// A <see cref="TextBox"/> that included help text
    /// </summary>
    [SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Control")]
    public sealed class InfoTextBox : TextBox
    {
        #region Constants and Fields

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
        private static readonly DependencyProperty HasTextProperty = DependencyProperty.Register(
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

            var resourceDictionary = new ResourceDictionary { Source = new Uri("/System.Windows;component/Resources/Dictionary.xaml", UriKind.Relative) };
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
                this.SetValue(HasTextProperty, value);
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
        /// <parameter name="e">
        /// Provides data about the event.
        /// </parameter>
        protected override void OnDragEnter(DragEventArgs e)
        {
            this.myAdornerLayer.RemoveAdorners<AdornerLabel>(this);

            base.OnDragEnter(e);
        }

        /// <summary>
        /// Invoked whenever an unhandled System.Windows.DragDrop.DragLeave attached routed event reaches an element derived from this class in its route. Implement this method to add class handling for this event.
        /// </summary>
        /// <parameter name="e">
        /// Provides data about the event.
        /// </parameter>
        protected override void OnDragLeave(DragEventArgs e)
        {
            this.UpdateAdorner(this);

            base.OnDragLeave(e);
        }

        /// <summary>
        /// Is called when content in this editing control changes.
        /// </summary>
        /// <parameter name="e">
        /// The arguments that are associated with the <see cref="E:System.Windows.Controls.Primitives.TextBoxBase.TextChanged"/> event.
        /// </parameter>
        protected override void OnTextChanged(TextChangedEventArgs e)
        {
            this.HasText = !(this.Text != null && String.IsNullOrEmpty(this.Text));

            base.OnTextChanged(e);
        }

        /// <summary>
        /// Determines whether the <see cref="InfoTextBox"/> is Visible
        /// </summary>
        /// <parameter name="sender">
        /// The sender.
        /// </parameter>
        /// <parameter name="e">
        /// The <see cref="System.EventArgs"/> instance containing the event data.
        /// </parameter>
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
        /// <parameter name="d">
        /// The dependency object
        /// </parameter>
        /// <parameter name="e">
        /// The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.
        /// </parameter>
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
        /// <parameter name="elem">
        /// The element
        /// </parameter>
        /// <parameter name="hide">
        /// if set to <see langword="true"/> hide the adorner
        /// </parameter>
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
}