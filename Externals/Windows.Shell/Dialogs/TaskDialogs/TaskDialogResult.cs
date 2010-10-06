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
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Indicates the various buttons and options clicked by the user on the task dialog.
    /// </summary>
    [SuppressMessage("Microsoft.Design", "CA1027:MarkEnumsWithFlags", Justification = "Interop")]
    [SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue", Justification = "Interop")]
    public enum TaskDialogResult
    {
        /// <summary>
        ///   "OK" button was clicked
        /// </summary>
        Ok = 0x0001, 

        /// <summary>
        ///   "Yes" button was clicked
        /// </summary>
        Yes = 0x0002, 

        /// <summary>
        ///   "No" button was clicked
        /// </summary>
        No = 0x0004, 

        /// <summary>
        ///   "Cancel" button was clicked
        /// </summary>
        Cancel = 0x0008, 

        /// <summary>
        ///   "Retry" button was clicked
        /// </summary>
        Retry = 0x0010, 

        /// <summary>
        ///   "Close" button was clicked
        /// </summary>
        Close = 0x0020, 

        /// <summary>
        ///   A custom button was clicked.
        /// </summary>
        CustomButtonClicked = 0x0100, 
    }
}