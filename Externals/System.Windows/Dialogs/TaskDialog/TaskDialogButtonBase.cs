// ***********************************************************************
// <copyright file="TaskDialogButtonBase.cs" project="System.Windows" assembly="System.Windows" solution="SevenUpdate" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// <license href="http://code.msdn.microsoft.com/WindowsAPICodePack/Project/License.aspx">Microsoft Software License</license>
// ***********************************************************************

namespace System.Windows.Dialogs.TaskDialog
{
    /// <summary>
    ///   Defines the abstract base class for task dialog buttons. Classes that inherit from this class will inherit the
    ///   Text property defined in this class.
    /// </summary>
    /// <remarks>
    ///   ContentProperty allows us to specify the text of the button as the child text of a button element in XAML, as
    ///   well as explicitly set with 'Text=""' Note that this attribute is inherited, so it applies to command-links
    ///   and radio buttons as well.
    /// </remarks>
    public abstract class TaskDialogButtonBase : TaskDialogControl
    {
        #region Constants and Fields

        /// <summary>A value indicating whether this button is the default button,</summary>
        private bool defaultControl;

        /// <summary>A value indicating whether the button is enabled.</summary>
        private bool enabled = true;

        /// <summary>The button text.</summary>
        private string text;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the <see cref="TaskDialogButtonBase" /> class. Creates a new instance on a
        ///   task dialog button.
        /// </summary>
        protected TaskDialogButtonBase()
        {
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="TaskDialogButtonBase" /> class. Creates a new instance on a
        ///   task dialog button with the specified name and text.
        /// </summary>
        /// <param name="name">The name for this button.</param>
        /// <param name="text">The label for this button.</param>
        protected TaskDialogButtonBase(string name, string text)
            : base(name)
        {
            this.text = text;
        }

        #endregion

        // Note that we don't need to explicitly implement the add/remove delegate for the Click event; the hosting
        // dialog only needs the delegate information when the Click event is raised (indirectly) by NativeTaskDialog,
        // so the latest delegate is always available.
        #region Public Events

        /// <summary>Raised when the task dialog button is clicked.</summary>
        public event EventHandler Click;

        #endregion

        #region Public Properties

        /// <summary>Gets or sets a value indicating whether this button is the default button.</summary>
        public bool Default
        {
            get
            {
                return this.defaultControl;
            }

            set
            {
                this.CheckPropertyChangeAllowed("Default");
                this.defaultControl = value;
                this.ApplyPropertyChange("Default");
            }
        }

        /// <summary>
        ///   Gets or sets a value indicating whether the button is enabled. The enabled state can cannot be changed
        ///   before the dialog is shown.
        /// </summary>
        public bool Enabled
        {
            get
            {
                return this.enabled;
            }

            set
            {
                this.CheckPropertyChangeAllowed("Enabled");
                this.enabled = value;
                this.ApplyPropertyChange("Enabled");
            }
        }

        /// <summary>Gets or sets the button text.</summary>
        public string Text
        {
            get
            {
                return this.text;
            }

            set
            {
                this.CheckPropertyChangeAllowed("Text");
                this.text = value;
                this.ApplyPropertyChange("Text");
            }
        }

        #endregion

        #region Public Methods

        /// <summary>Returns the Text property value for this button.</summary>
        /// <returns>A <see cref="System.String" />.</returns>
        public override string ToString()
        {
            return this.text ?? string.Empty;
        }

        #endregion

        #region Methods

        /// <summary>Raises the click event.</summary>
        internal void RaiseClickEvent()
        {
            // Only perform click if the button is enabled.
            if (!this.enabled)
            {
                return;
            }

            if (this.Click != null)
            {
                this.Click(this, EventArgs.Empty);
            }
        }

        #endregion
    }
}
