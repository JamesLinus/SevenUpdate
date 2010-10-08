// ***********************************************************************
// <copyright file="TaskDialogProgressBarState.cs"
//            project="System.Windows"
//            assembly="System.Windows"
//            solution="SevenUpdate"
//            company="Seven Software">
//     Copyright (c) Seven Software. All rights reserved.
// </copyright>
// <author username="sevenalive">Robert Baker</author>
// ***********************************************************************
namespace System.Windows.Dialogs.TaskDialogs
{
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Sets the state of a task dialog progress bar.
    /// </summary>
    [SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue", Justification = "Interop")]
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
}