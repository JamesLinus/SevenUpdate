// ***********************************************************************
// <copyright file="TaskDialogButtonBase.cs"
//            project="System.Windows"
//            assembly="System.Windows"
//            solution="SevenUpdate"
//            company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// <license href="http://code.msdn.microsoft.com/WindowsAPICodePack/Project/License.aspx">Microsoft Software License</license>
// ***********************************************************************

namespace System.Windows.Dialogs
{
    /// <summary>Defines the abstract base class for task dialog buttons.Classes that inherit from this class will inheritthe Text property defined in this class.</summary>
    public abstract class TaskDialogButtonBase : TaskDialogControl
    {
        #region Constants and Fields

        /// <summary>Default control</summary>
        private bool defaultControl;

        /// <summary><see langword="true" /> to enable, <see langword="false" /> to disable</summary>
        private bool enabled = true;

        /// <summary>The text for the button</summary>
        private string text;

        #endregion

        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref="TaskDialogButtonBase" /> class.</summary>
        protected TaskDialogButtonBase()
        {
        }

        /// <summary>Initializes a new instance of the <see cref="TaskDialogButtonBase" /> class.</summary>
        /// <param name="name">The name for this button.</param>
        /// <param name="text">The label for this button.</param>
        protected TaskDialogButtonBase(string name, string text)
            : base(name)
        {
            this.text = text;
        }

        #endregion

        #region Events

        /// <summary>Raised when the task dialog button is clicked.</summary>
        public event EventHandler Click;

        #endregion

        #region Properties

        /// <summary>Gets or sets a value indicating whether this button is the default button.</summary>
        /// <value><see langword="true" /> if default; otherwise, <see langword="false" />.</value>
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

        /// <summary>Gets or sets a value indicating whether whether thebutton is enabled. The enabled state can cannot be changedbefore the dialog is shown.</summary>
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

            var handler = this.Click;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        #endregion
    }
}