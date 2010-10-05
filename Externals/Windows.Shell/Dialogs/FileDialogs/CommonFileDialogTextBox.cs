//***********************************************************************
// Assembly         : Windows.Shell
// Author           : sevenalive
// Created          : 09-17-2010
// Last Modified By : sevenalive
// Last Modified On : 10-05-2010
// Description      : 
// Copyright        : (c) Seven Software. All rights reserved.
//***********************************************************************

namespace Microsoft.Windows.Dialogs.Controls
{
    using System;
    using System.Diagnostics;

    /// <summary>
    /// Defines the text box controls in the Common File Dialog.
    /// </summary>
    public class CommonFileDialogTextBox : CommonFileDialogControl
    {
        #region Constants and Fields

        /// <summary>
        ///   Holds an instance of the customized (/native) dialog and should
        ///   be null until after the Attach() call is made.
        /// </summary>
        private IFileDialogCustomize customizedDialog;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///   Creates a new instance of this class.
        /// </summary>
        protected CommonFileDialogTextBox()
            : base(String.Empty)
        {
        }

        /// <summary>
        /// Creates a new instance of this class with the specified text.
        /// </summary>
        /// <param name="text">
        /// The text to display for this control.
        /// </param>
        protected CommonFileDialogTextBox(string text)
            : base(text)
        {
        }

        /// <summary>
        /// Creates a new instance of this class with the specified name and text.
        /// </summary>
        /// <param name="name">
        /// The name of this control.
        /// </param>
        /// <param name="text">
        /// The text to display for this control.
        /// </param>
        protected CommonFileDialogTextBox(string name, string text)
            : base(name, text)
        {
        }

        #endregion

        #region Properties

        /// <summary>
        ///   Gets or sets a value for the text string contained in the CommonFileDialogTextBox.
        /// </summary>
        public override string Text
        {
            get
            {
                if (!this.Closed)
                {
                    this.SyncValue();
                }

                return base.Text;
            }

            set
            {
                if (this.customizedDialog != null)
                {
                    this.customizedDialog.SetEditBoxText(this.Id, value);
                }

                base.Text = value;
            }
        }

        /// <summary>
        /// </summary>
        internal bool Closed { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Attach the TextBox control to the dialog object
        /// </summary>
        /// <param name="dialog">
        /// Target dialog
        /// </param>
        internal override void Attach(IFileDialogCustomize dialog)
        {
            Debug.Assert(dialog != null, "CommonFileDialogTextBox.Attach: dialog parameter can not be null");

            // Add a text entry control
            dialog.AddEditBox(this.Id, this.Text);

            // Set to local instance in order to gate access to same.
            this.customizedDialog = dialog;

            // Sync unmanaged properties with managed properties
            this.SyncUnmanagedProperties();

            this.Closed = false;
        }

        /// <summary>
        /// </summary>
        internal void SyncValue()
        {
            // Make sure that the local native dialog instance is NOT 
            // null. If it's null, just return the "textValue" var,
            // otherwise, use the native call to get the text value, 
            // setting the textValue member variable then return it.
            if (this.customizedDialog == null)
            {
                return;
            }

            string textValue;
            this.customizedDialog.GetEditBoxText(this.Id, out textValue);

            base.Text = textValue;
        }

        #endregion
    }
}