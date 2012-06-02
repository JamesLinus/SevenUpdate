// <copyright file="TaskDialogStartupLocation.cs" project="SevenSoftware.Windows" company="Microsoft Corporation">Microsoft Corporation</copyright>
// <license href="http://code.msdn.microsoft.com/WindowsAPICodePack/Project/License.aspx" name="Microsoft Software License" />

namespace SevenSoftware.Windows.Dialogs.TaskDialog
{
    /// <summary>Specifies the initial display location for a task dialog.</summary>
    public enum TaskDialogStartupLocation
    {
        /// <summary>The window placed in the center of the screen.</summary>
        CenterScreen, 

        /// <summary>The window centered relative to the window that launched the dialog.</summary>
        CenterOwner
    }
}