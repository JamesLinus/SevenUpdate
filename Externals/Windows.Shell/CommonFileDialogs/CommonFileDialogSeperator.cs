//Copyright (c) Microsoft Corporation.  All rights reserved.
//Modified by Robert Baker, Seven Software 2010.

#region

using System.Diagnostics;

#endregion

namespace Microsoft.Windows.Dialogs.Controls
{
    /// <summary>
    ///   Defines the class for the simplest separator controls.
    /// </summary>
    public class CommonFileDialogSeparator : CommonFileDialogControl
    {
        /// <summary>
        ///   Attach the Separator control to the dialog object
        /// </summary>
        /// <param name = "dialog">Target dialog</param>
        internal override void Attach(IFileDialogCustomize dialog)
        {
            Debug.Assert(dialog != null, "CommonFileDialogSeparator.Attach: dialog parameter can not be null");

            // Add a separator
            dialog.AddSeparator(Id);

            // Sync unmanaged properties with managed properties
            SyncUnmanagedProperties();
        }
    }
}