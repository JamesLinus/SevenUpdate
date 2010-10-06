// ***********************************************************************
// Assembly         : Windows.Shell
// Author           : Microsoft
// Created          : 09-17-2010
// Last Modified By : sevenalive (Robert Baker)
// Last Modified On : 10-05-2010
// Description      : 
// Copyright        : (c) Microsoft Corporation. All rights reserved.
// ***********************************************************************

namespace Microsoft.Windows.Dialogs.TaskDialogs
{
    /// <summary>
    /// Specifies the initial display location for a task dialog.
    /// </summary>
    public enum TaskDialogStartupLocation
    {
        /// <summary>
        ///   The window placed in the center of the screen.
        /// </summary>
        CenterScreen, 

        /// <summary>
        ///   The window centered relative to the window that launched the dialog.
        /// </summary>
        CenterOwner
    }
}