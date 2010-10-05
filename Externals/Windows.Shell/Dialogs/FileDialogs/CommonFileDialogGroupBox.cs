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
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Windows.Markup;

    /// <summary>
    /// Represents a group box control for the Common File Dialog.
    /// </summary>
    /// note
    [ContentProperty("Items")]
    public class CommonFileDialogGroupBox : CommonFileDialogProminentControl
    {
        #region Constructors and Destructors

        /// <summary>
        ///   Creates a new instance of this class.
        /// </summary>
        protected CommonFileDialogGroupBox()
            : base(String.Empty)
        {
            this.Initialize();
        }

        /// <summary>
        /// Create a new instance of this class with the specified text.
        /// </summary>
        /// <param name="text">
        /// The text to display for this control.
        /// </param>
        protected CommonFileDialogGroupBox(string text)
            : base(text)
        {
            this.Initialize();
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
        protected CommonFileDialogGroupBox(string name, string text)
            : base(name, text)
        {
            this.Initialize();
        }

        #endregion

        #region Properties

        /// <summary>
        ///   Gets the collection of controls for this group box.
        /// </summary>
        public Collection<DialogControl> Items { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        /// Attach the GroupBox control to the dialog object
        /// </summary>
        /// <param name="dialog">
        /// Target dialog
        /// </param>
        internal override void Attach(IFileDialogCustomize dialog)
        {
            Debug.Assert(dialog != null, "CommonFileDialogGroupBox.Attach: dialog parameter can not be null");

            // Start a visual group
            dialog.StartVisualGroup(this.Id, this.Text);

            // Add child controls
            foreach (CommonFileDialogControl item in this.Items)
            {
                item.HostingDialog = this.HostingDialog;
                item.Attach(dialog);
            }

            // End visual group
            dialog.EndVisualGroup();

            // Make this control prominent if needed
            if (this.IsProminent)
            {
                dialog.MakeProminent(this.Id);
            }

            // Sync unmanaged properties with managed properties
            this.SyncUnmanagedProperties();
        }

        /// <summary>
        /// Initializes the item collection for this class.
        /// </summary>
        private void Initialize()
        {
            this.Items = new Collection<DialogControl>();
        }

        #endregion
    }
}