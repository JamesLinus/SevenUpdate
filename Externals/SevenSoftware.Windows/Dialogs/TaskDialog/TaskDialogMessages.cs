// <copyright file="TaskDialogMessages.cs" project="SevenSoftware.Windows" company="Microsoft Corporation">Microsoft Corporation</copyright>
// <license href="http://code.msdn.microsoft.com/WindowsAPICodePack/Project/License.aspx" name="Microsoft Software License" />

namespace SevenSoftware.Windows.Dialogs.TaskDialog
{
    /// <summary>The dialog messages.</summary>
    internal enum TaskDialogMessages
    {
        /// <summary>Recreates a task dialog with new contents, simulating the functionality of a multi-page wizard.</summary>
        NavigatePage = 0x0400 + 101, 

        /// <summary>Simulates the action of a button click in a dialog.</summary>
        ClickButton = 0x0400 + 102, // wParam = Button ID

        /// <summary>Parameter = 0 (nonMarque) parameter != 0 (Marquee).</summary>
        SetMarqueeProgressBar = 0x0400 + 103, 

        /// <summary>Sets the current state of the progress bar.</summary>
        SetProgressBarState = 0x0400 + 104, 

        /// <summary>Sets the minimum and maximum values for the hosted progress bar.</summary>
        SetProgressBarRange = 0x0400 + 105, 

        /// <summary>Sets the current position for a progress bar.</summary>
        SetProgressBarPosition = 0x0400 + 106, 

        /// <summary>Indicates whether the hosted progress bar should be displayed in marquee mode.</summary>
        SetProgressBarMarquee = 0x0400 + 107, 

        /// <summary>Updates a text element in a task dialog.</summary>
        SetElementText = 0x0400 + 108, 

        /// <summary>Simulates the action of a radio button click in a task dialog.</summary>
        ClickRadioButton = 0x0400 + 110, 

        /// <summary>Enables or disables a push button in a task dialog.</summary>
        EnableButton = 0x0400 + 111, 

        /// <summary>Enables or disables a radio button in a task dialog.</summary>
        EnableRadioButton = 0x0400 + 112, 

        /// <summary>Simulates the action of a verification checkbox click in a task dialog.</summary>
        ClickVerification = 0x0400 + 113, 

        /// <summary>Updates a text element in a task dialog.</summary>
        UpdateElementText = 0x0400 + 114, 

        /// <summary>
        ///   Specifies whether a given task dialog button or command link should have a UAC shield icon; that is,
        ///   whether the action invoked by the button requires elevation.
        /// </summary>
        SetButtonElevationRequiredState = 0x0400 + 115, 

        /// <summary>Refreshes the icon of a task dialog.</summary>
        UpdateIcon = 0x0400 + 116
    }
}