// <copyright file="ProgressBarState.cs" project="SevenSoftware.Windows" company="Microsoft Corporation">Microsoft Corporation</copyright>
// <license href="http://code.msdn.microsoft.com/WindowsAPICodePack/Project/License.aspx" name="Microsoft Software License" />

namespace SevenSoftware.Windows.Dialogs.TaskDialog
{
    /// <summary>Main task dialog configuration struct. NOTE: Packing must be set to 4 to make this work on 64-bit platforms.</summary>
    internal enum ProgressBarState
    {
        /// <summary>Normal status.</summary>
        Normal = 0x0001, 

        /// <summary>Red progress, useful to give feedback that an error has occurred.</summary>
        Error = 0x0002, 

        /// <summary>Yellow progress, useful to give feedback that the operation has paused.</summary>
        Paused = 0x0003
    }
}