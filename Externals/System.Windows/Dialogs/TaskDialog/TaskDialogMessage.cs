// ***********************************************************************
// <copyright file="TaskDialogMessage.cs" project="System.Windows" assembly="System.Windows" solution="SevenUpdate" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// <license href="http://code.msdn.microsoft.com/WindowsAPICodePack/Project/License.aspx">Microsoft Software License</license>
// ***********************************************************************

namespace System.Windows.Dialogs
{
    using Internal;

    /// <summary>
    ///   Dialog messages.
    /// </summary>
    internal enum TaskDialogMessage
    {
        /// <summary>
        ///   Recreates a task dialog with new contents, simulating the functionality of a multi-page wizard.
        /// </summary>
        NavigatePage = NativeMethods.WMUser + 101,

        /// <summary>
        ///   Simulates the action of a button click in a dialog.
        /// </summary>
        ClickButton = NativeMethods.WMUser + 102,

        /// <summary>
        ///   Parameter = 0 (nonMarque) parameter != 0 (Marquee).
        /// </summary>
        SetMarqueeProgressBar = NativeMethods.WMUser + 103,

        /// <summary>
        ///   Sets the current state of the progress bar.
        /// </summary>
        SetProgressBarState = NativeMethods.WMUser + 104,

        /// <summary>
        ///   Sets the minimum and maximum values for the hosted progress bar.
        /// </summary>
        SetProgressBarRange = NativeMethods.WMUser + 105,

        /// <summary>
        ///   Sets the current position for a progress bar.
        /// </summary>
        SetProgressBarPos = NativeMethods.WMUser + 106,

        /// <summary>
        ///   Indicates whether the hosted progress bar should be displayed in marquee mode.
        /// </summary>
        SetProgressBarMarquee = NativeMethods.WMUser + 107,

        /// <summary>
        ///   Updates a text element in a task dialog.
        /// </summary>
        SetElementText = NativeMethods.WMUser + 108,

        /// <summary>
        ///   Simulates the action of a radio button click in a task dialog.
        /// </summary>
        ClickRadioButton = NativeMethods.WMUser + 110,

        /// <summary>
        ///   Enables or disables a push button in a task dialog.
        /// </summary>
        EnableButton = NativeMethods.WMUser + 111,

        /// <summary>
        ///   Enables or disables a radio button in a task dialog.
        /// </summary>
        EnableRadioButton = NativeMethods.WMUser + 112,

        /// <summary>
        ///   Simulates the action of a verification checkbox click in a task dialog.
        /// </summary>
        ClickVerification = NativeMethods.WMUser + 113,

        /// <summary>
        ///   Updates a text element in a task dialog.
        /// </summary>
        UpdateElementText = NativeMethods.WMUser + 114,

        /// <summary>
        ///   Specifies whether a given task dialog button or command link should have a UAC shield icon; that is,
        ///   whether the action invoked by the button requires elevation.
        /// </summary>
        SetButtonElevationRequiredState = NativeMethods.WMUser + 115,

        /// <summary>
        ///   Refreshes the icon of a task dialog.
        /// </summary>
        UpdateIcon = NativeMethods.WMUser + 116
    }
}