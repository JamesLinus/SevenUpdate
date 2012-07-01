// <copyright file="TaskDialogTickEventArgs.cs" project="SevenSoftware.Windows" company="Microsoft Corporation">Microsoft Corporation</copyright>
// <license href="http://code.msdn.microsoft.com/WindowsAPICodePack/Project/License.aspx" name="Microsoft Software License" />

using System;

namespace SevenSoftware.Windows.Dialogs.TaskDialog
{
    /// <summary>The event data for a TaskDialogTick event.</summary>
    public class TaskDialogTickEventArgs : EventArgs
    {
        /// <summary>Initializes a new instance of the <see cref="TaskDialogTickEventArgs" /> class.</summary>
        /// <param name="ticks">The total number of ticks since the control was activated.</param>
        public TaskDialogTickEventArgs(int ticks)
        {
            Ticks = ticks;
        }

        /// <summary>Gets a value that determines the current number of ticks.</summary>
        public int Ticks { get; private set; }
    }
}