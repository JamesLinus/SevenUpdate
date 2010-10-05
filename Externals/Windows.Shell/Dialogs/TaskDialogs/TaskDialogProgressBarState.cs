//***********************************************************************
// Assembly         : Windows.Shell
// Author           : sevenalive
// Created          : 09-17-2010
// Last Modified By : sevenalive
// Last Modified On : 10-05-2010
// Description      : 
// Copyright        : (c) Seven Software. All rights reserved.
//***********************************************************************

namespace Microsoft.Windows.Dialogs
{
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Sets the state of a task dialog progress bar.
    /// </summary>
    [SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue")]
    public enum TaskDialogProgressBarState
    {
        /// <summary>
        ///   Normal state.
        /// </summary>
        Normal = TaskDialogNativeMethods.PBST.Normal, 

        /// <summary>
        ///   An error occurred.
        /// </summary>
        Error = TaskDialogNativeMethods.PBST.Error, 

        /// <summary>
        ///   The progress is paused.
        /// </summary>
        Paused = TaskDialogNativeMethods.PBST.Paused, 

        /// <summary>
        ///   Displays marquee (indeterminate) style progress
        /// </summary>
        Marquee, 
    }
}