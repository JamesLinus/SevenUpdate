// <copyright file="TaskDialogControl.cs" project="SevenSoftware.Windows" company="Microsoft Corporation">Microsoft Corporation</copyright>
// <license href="http://code.msdn.microsoft.com/WindowsAPICodePack/Project/License.aspx" name="Microsoft Software License" />

namespace SevenSoftware.Windows.Dialogs.TaskDialog
{
    /// <summary>Declares the abstract base class for all custom task dialog controls.</summary>
    public abstract class TaskDialogControl : DialogControl
    {
        /// <summary>
        ///   Initializes a new instance of the <see cref="TaskDialogControl" /> class. Creates a new instance of a task
        ///   dialog control.
        /// </summary>
        protected TaskDialogControl()
        {
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="TaskDialogControl" /> class. Creates a new instance of a task
        ///   dialog control with the specified name.
        /// </summary>
        /// <param name="name">The name for this control.</param>
        protected TaskDialogControl(string name) : base(name)
        {
        }
    }
}