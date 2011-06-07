// ***********************************************************************
// <copyright file="TaskDialogIcon.cs" project="System.Windows" assembly="System.Windows" solution="SevenUpdate" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// <license href="http://code.msdn.microsoft.com/WindowsAPICodePack/Project/License.aspx">Microsoft Software License</license>
// ***********************************************************************

namespace System.Windows.Dialogs
{
    /// <summary>
    ///   The task dialog main icons.
    /// </summary>
    internal enum TaskDialogIcon
    {
        /// <summary>
        ///   An exclamation-point icon appears in the task dialog.
        /// </summary>
        WarningIcon = 65535,

        /// <summary>
        ///   A stop-sign icon appears in the task dialog.
        /// </summary>
        ErrorIcon = 65534,

        /// <summary>
        ///   An icon consisting of a lowercase letter i in a circle appears in the task dialog.
        /// </summary>
        InformationIcon = 65533,

        /// <summary>
        ///   A shield icon appears in the task dialog.
        /// </summary>
        ShieldIcon = 65532
    }
}