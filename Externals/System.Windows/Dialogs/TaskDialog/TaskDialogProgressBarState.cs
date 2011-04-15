// ***********************************************************************
// <copyright file="TaskDialogProgressBarState.cs"
//            project="System.Windows"
//            assembly="System.Windows"
//            solution="SevenUpdate"
//            company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// <license href="http://code.msdn.microsoft.com/WindowsAPICodePack/Project/License.aspx">Microsoft Software License</license>
// ***********************************************************************
namespace System.Windows.Dialogs
{
    /// <summary>Sets the state of a task dialog progress bar.</summary>
    public enum TaskDialogProgressBarState
    {
        /// <summary>Normal status</summary>
        Normal = 0x0001, 

        /// <summary>Red progress</summary>
        Error = 0x0002, 

        /// <summary>Yellow progress</summary>
        Paused = 0x0003, 

        /// <summary>Displays marquee (indeterminate) style progress</summary>
        Marquee, 
    }
}