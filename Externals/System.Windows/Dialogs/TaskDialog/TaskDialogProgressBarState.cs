// ***********************************************************************
// <copyright file="TaskDialogProgressBarState.cs" project="System.Windows" assembly="System.Windows" solution="SevenUpdate" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// <license href="http://code.msdn.microsoft.com/WindowsAPICodePack/Project/License.aspx">Microsoft Software License</license>
// ***********************************************************************

namespace System.Windows.Dialogs.TaskDialog
{
    /// <summary>Sets the state of a task dialog progress bar.</summary>
    public enum TaskDialogProgressBarState
    {
        /// <summary>Uninitialized state, this should never occur.</summary>
        None = 0, 

        /// <summary>Normal state.</summary>
        Normal = ProgressBarState.Normal, 

        /// <summary>An error occurred.</summary>
        Error = ProgressBarState.Error, 

        /// <summary>The progress is paused.</summary>
        Paused = ProgressBarState.Paused, 

        /// <summary>Displays marquee (indeterminate) style progress</summary>
        Marquee
    }
}
