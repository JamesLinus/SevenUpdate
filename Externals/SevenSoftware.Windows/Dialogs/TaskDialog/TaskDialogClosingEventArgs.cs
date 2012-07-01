// <copyright file="TaskDialogClosingEventArgs.cs" project="SevenSoftware.Windows" company="Microsoft Corporation">Microsoft Corporation</copyright>
// <license href="http://code.msdn.microsoft.com/WindowsAPICodePack/Project/License.aspx" name="Microsoft Software License" />

using System.ComponentModel;

namespace SevenSoftware.Windows.Dialogs.TaskDialog
{
    /// <summary>Data associated with <see cref="TaskDialog.Closing" /> event.</summary>
    public class TaskDialogClosingEventArgs : CancelEventArgs
    {
        /// <summary>Gets or sets the text of the custom button that was clicked.</summary>
        public string CustomButton { get; set; }

        /// <summary>Gets or sets the standard button that was clicked.</summary>
        public TaskDialogResult TaskDialogResult { get; set; }
    }
}