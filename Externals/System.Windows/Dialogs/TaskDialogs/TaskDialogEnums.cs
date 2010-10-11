// ***********************************************************************
// <copyright file="TaskDialogEnums.cs"
//            project="System.Windows"
//            assembly="System.Windows"
//            solution="SevenUpdate"
//            company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// <license href="http://code.msdn.microsoft.com/WindowsAPICodePack/Project/License.aspx">Microsoft Software License</license>
// ***********************************************************************
namespace System.Windows.Dialogs.TaskDialogs
{
    #region TaskDialogExpandedDetailsLocation

    /// <summary>
    /// Specifies the options for expand/collapse sections in dialogs.
    /// </summary>
    public enum TaskDialogExpandedDetailsLocation
    {
        /// <summary>
        ///   Do not show the content.
        /// </summary>
        Hide, 

        /// <summary>
        ///   Show the content.
        /// </summary>
        ExpandContent, 

        /// <summary>
        ///   Expand the footer content.
        /// </summary>
        ExpandFooter
    }

    #endregion

    #region TaskDialogProgressBarState

    /// <summary>
    /// Sets the state of a task dialog progress bar.
    /// </summary>
    public enum TaskDialogProgressBarState
    {
        /// <summary>
        ///   Normal state.
        /// </summary>
        Normal = TaskDialogNativeMethods.ProgressBarStatus.Normal, 

        /// <summary>
        ///   An error occurred.
        /// </summary>
        Error = TaskDialogNativeMethods.ProgressBarStatus.Error, 

        /// <summary>
        ///   The progress is paused.
        /// </summary>
        Paused = TaskDialogNativeMethods.ProgressBarStatus.Paused, 

        /// <summary>
        ///   Displays marquee (indeterminate) style progress
        /// </summary>
        Marquee, 
    }

    #endregion

    #region TaskDialogResult

    /// <summary>
    /// Indicates the various buttons and options clicked by the user on the task dialog.
    /// </summary>
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

    #endregion

    #region TaskDialogStartupLocation

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

    #endregion

    #region TaskDialogStandardButtons

    /// <summary>
    /// Identifies one of the standard buttons that 
    ///   can be displayed via <see cref="TaskDialog"/>.
    /// </summary>
    [Flags]
    public enum TaskDialogStandardButtons
    {
        /// <summary>
        ///   No buttons on the dialog.
        /// </summary>
        None = 0x0000, 

        /// <summary>
        ///   An "OK" button.
        /// </summary>
        Ok = 0x0001, 

        /// <summary>
        ///   A "Yes" button.
        /// </summary>
        Yes = 0x0002, 

        /// <summary>
        ///   A "No" button.
        /// </summary>
        No = 0x0004, 

        /// <summary>
        ///   A "Cancel" button.
        /// </summary>
        Cancel = 0x0008, 

        /// <summary>
        ///   A "Retry" button.
        /// </summary>
        Retry = 0x0010, 

        /// <summary>
        ///   A "Close" button.
        /// </summary>
        Close = 0x0020
    }

    #endregion

    #region TaskDialogStandardIcon

    /// <summary>
    /// Specifies the icon displayed in a task dialog.
    /// </summary>
    public enum TaskDialogStandardIcon
    {
        /// <summary>
        ///   Displays no icons (default).
        /// </summary>
        None = 0, 

        /// <summary>
        ///   Displays the warning icon.
        /// </summary>
        Warning = 65535, 

        /// <summary>
        ///   Displays the error icon.
        /// </summary>
        Error = 65534, 

        /// <summary>
        ///   Displays the Information icon.
        /// </summary>
        Information = 65533, 

        /// <summary>
        ///   Displays the User Account Control shield.
        /// </summary>
        Shield = UInt16.MaxValue - 3, 

        /// <summary>
        ///   Displays the User Account Control shield.
        /// </summary>
        ShieldBlue = UInt16.MaxValue - 4, 

        /// <summary>
        ///   Displays the User Account Control shield with gray background.
        /// </summary>
        ShieldGray = UInt16.MaxValue - 8, 

        /// <summary>
        ///   Displays a warning shield with yellow background.
        /// </summary>
        SecurityWarning = UInt16.MaxValue - 5, 

        /// <summary>
        ///   Displays an error shield with red background.
        /// </summary>
        SecurityError = UInt16.MaxValue - 6, 

        /// <summary>
        ///   Displays a success shield with green background.
        /// </summary>
        ShieldGreen = UInt16.MaxValue - 7, 
    }

    #endregion
}