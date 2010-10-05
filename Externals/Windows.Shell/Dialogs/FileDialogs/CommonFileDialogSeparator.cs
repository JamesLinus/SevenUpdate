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
    using System.Diagnostics;

    /// <summary>
    /// Defines the class for the simplest separator controls.
    /// </summary>
    public class CommonFileDialogSeparator : CommonFileDialogControl
    {
        #region Methods

        /// <summary>
        /// Attach the Separator control to the dialog object
        /// </summary>
        /// <param name="dialog">
        /// Target dialog
        /// </param>
        internal override void Attach(IFileDialogCustomize dialog)
        {
            Debug.Assert(dialog != null, "CommonFileDialogSeparator.Attach: dialog parameter can not be null");

            // Add a separator
            dialog.AddSeparator(this.Id);

            // Sync unmanaged properties with managed properties
            this.SyncUnmanagedProperties();
        }

        #endregion
    }
}