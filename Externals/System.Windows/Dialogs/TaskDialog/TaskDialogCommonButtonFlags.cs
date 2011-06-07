// ***********************************************************************
// <copyright file="TaskDialogCommonButtonFlags.cs" project="System.Windows" assembly="System.Windows" solution="SevenUpdate" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// <license href="http://code.msdn.microsoft.com/WindowsAPICodePack/Project/License.aspx">Microsoft Software License</license>
// ***********************************************************************

namespace System.Windows.Dialogs
{
    /// <summary>
    ///   Specifies the push buttons displayed in the task dialog. If no common buttons are specified and no custom buttons are specified using the Buttons and Buttons members, the task dialog will contain the OK button by default.
    /// </summary>
    [Flags]
    internal enum TaskDialogCommonButtonFlags
    {
        /// <summary>
        ///   The task dialog contains the push button: OK.
        /// </summary>
        OkButton = 0x0001,

        /// <summary>
        ///   The task dialog contains the push button: Yes.
        /// </summary>
        YesButton = 0x0002,

        /// <summary>
        ///   The task dialog contains the push button: No.
        /// </summary>
        NoButton = 0x0004,

        /// <summary>
        ///   The task dialog contains the push button: Cancel. If this button is specified, the task dialog will respond to typical cancel actions (Alt-F4 and Escape).
        /// </summary>
        CancelButton = 0x0008,

        /// <summary>
        ///   The task dialog contains the push button: Retry.
        /// </summary>
        RetryButton = 0x0010,

        /// <summary>
        ///   The task dialog contains the push button: Close.
        /// </summary>
        CloseButton = 0x0020
    }
}