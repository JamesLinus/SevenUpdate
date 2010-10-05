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
    /// Creates the push button controls used by the Common File Dialog.
    /// </summary>
    public class CommonFileDialogButton : CommonFileDialogProminentControl
    {
        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of this class.
        /// </summary>
        protected CommonFileDialogButton()
            : base(String.Empty)
        {
        }

        /// <summary>
        /// Initializes a new instance of this class with the text only.
        /// </summary>
        /// <param name="text">
        /// The text to display for this control.
        /// </param>
        protected CommonFileDialogButton(string text)
            : base(text)
        {
        }

        /// <summary>
        /// Initializes a new instance of this class with the specified name and text.
        /// </summary>
        /// <param name="name">
        /// The name of this control.
        /// </param>
        /// <param name="text">
        /// The text to display for this control.
        /// </param>
        protected CommonFileDialogButton(string name, string text)
            : base(name, text)
        {
        }

        #endregion

        #region Events

        /// <summary>
        ///   Occurs when the user clicks the control. This event is routed from COM via the event sink.
        /// </summary>
        public event EventHandler Click = delegate { };

        #endregion

        #region Methods

        /// <summary>
        /// Attach the PushButton control to the dialog object
        /// </summary>
        /// <param name="dialog">
        /// Target dialog
        /// </param>
        internal override void Attach(IFileDialogCustomize dialog)
        {
            Debug.Assert(dialog != null, "CommonFileDialogButton.Attach: dialog parameter can not be null");

            // Add a push button control
            dialog.AddPushButton(this.Id, this.Text);

            // Make this control prominent if needed
            if (this.IsProminent)
            {
                dialog.MakeProminent(this.Id);
            }

            // Sync unmanaged properties with managed properties
            this.SyncUnmanagedProperties();
        }

        /// <summary>
        /// </summary>
        internal void RaiseClickEvent()
        {
            // Make sure that this control is enabled and has a specified delegate
            if (this.Enabled)
            {
                this.Click(this, EventArgs.Empty);
            }
        }

        #endregion
    }
}