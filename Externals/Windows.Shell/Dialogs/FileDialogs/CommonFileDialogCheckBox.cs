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
    /// Creates the check button controls used by the Common File Dialog.
    /// </summary>
    public class CommonFileDialogCheckBox : CommonFileDialogProminentControl
    {
        #region Constants and Fields

        /// <summary>
        /// </summary>
        private bool isChecked;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///   Creates a new instance of this class.
        /// </summary>
        protected CommonFileDialogCheckBox()
        {
        }

        /// <summary>
        /// Creates a new instance of this class with the specified text.
        /// </summary>
        /// <param name="text">
        /// The text to display for this control.
        /// </param>
        protected CommonFileDialogCheckBox(string text)
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
        protected CommonFileDialogCheckBox(string name, string text)
            : base(name, text)
        {
        }

        /// <summary>
        /// Creates a new instance of this class with the specified text and check state.
        /// </summary>
        /// <param name="text">
        /// The text to display for this control.
        /// </param>
        /// <param name="isChecked">
        /// The check state of this control.
        /// </param>
        protected CommonFileDialogCheckBox(string text, bool isChecked)
            : base(text)
        {
            this.isChecked = isChecked;
        }

        /// <summary>
        /// Creates a new instance of this class with the specified name, text and check state.
        /// </summary>
        /// <param name="name">
        /// The name of this control.
        /// </param>
        /// <param name="text">
        /// The text to display for this control.
        /// </param>
        /// <param name="isChecked">
        /// The check state of this control.
        /// </param>
        protected CommonFileDialogCheckBox(string name, string text, bool isChecked)
            : base(name, text)
        {
            this.isChecked = isChecked;
        }

        #endregion

        #region Events

        /// <summary>
        ///   Occurs when the user changes the check state.
        /// </summary>
        public event EventHandler CheckedChanged = delegate { };

        #endregion

        #region Properties

        /// <summary>
        ///   Gets or sets the state of the check box.
        /// </summary>
        public bool IsChecked
        {
            get
            {
                return this.isChecked;
            }

            set
            {
                // Check if property has changed
                if (this.isChecked == value)
                {
                    return;
                }

                this.isChecked = value;
                this.ApplyPropertyChange("IsChecked");
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Attach the CheckButton control to the dialog object.
        /// </summary>
        /// <param name="dialog">
        /// the target dialog
        /// </param>
        internal override void Attach(IFileDialogCustomize dialog)
        {
            Debug.Assert(dialog != null, "CommonFileDialogCheckBox.Attach: dialog parameter can not be null");

            // Add a check button control
            dialog.AddCheckButton(this.Id, this.Text, this.isChecked);

            // Make this control prominent if needed
            if (this.IsProminent)
            {
                dialog.MakeProminent(this.Id);
            }

            // Make sure this property is set
            this.ApplyPropertyChange("IsChecked");

            // Sync unmanaged properties with managed properties
            this.SyncUnmanagedProperties();
        }

        /// <summary>
        /// </summary>
        internal void RaiseCheckedChangedEvent()
        {
            // Make sure that this control is enabled and has a specified delegate
            if (this.Enabled)
            {
                this.CheckedChanged(this, EventArgs.Empty);
            }
        }

        #endregion
    }
}