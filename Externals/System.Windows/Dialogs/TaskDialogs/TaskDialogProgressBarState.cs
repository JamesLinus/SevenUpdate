// ***********************************************************************
// Assembly         : System.Windows
// Author           : Microsoft Corporation
// Last Modified By : Robert Baker (sevenalive)
// Last Modified On : 10-06-2010
// Copyright        : (c) Microsoft Corporation. All rights reserved.
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