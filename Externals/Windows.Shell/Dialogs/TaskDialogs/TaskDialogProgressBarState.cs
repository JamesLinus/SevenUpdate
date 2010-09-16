#region GNU Public License Version 3

// Copyright 2007-2010 Robert Baker, Seven Software.
// This file is part of Seven Update.
//   
//      Seven Update is free software: you can redistribute it and/or modify
//      it under the terms of the GNU General Public License as published by
//      the Free Software Foundation, either version 3 of the License, or
//      (at your option) any later version.
//  
//      Seven Update is distributed in the hope that it will be useful,
//      but WITHOUT ANY WARRANTY; without even the implied warranty of
//      MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//      GNU General Public License for more details.
//   
//      You should have received a copy of the GNU General Public License
//      along with Seven Update.  If not, see <http://www.gnu.org/licenses/>.

#endregion

#region

using System.Diagnostics.CodeAnalysis;

#endregion

namespace Microsoft.Windows.Dialogs
{
    /// <summary>
    ///   Sets the state of a task dialog progress bar.
    /// </summary>
    [SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue")]
    public enum TaskDialogProgressBarState
    {
        /// <summary>
        ///   Normal state.
        /// </summary>
        Normal = TaskDialogNativeMethods.PBST.PBST_NORMAL,

        /// <summary>
        ///   An error occurred.
        /// </summary>
        Error = TaskDialogNativeMethods.PBST.PBST_ERROR,

        /// <summary>
        ///   The progress is paused.
        /// </summary>
        Paused = TaskDialogNativeMethods.PBST.PBST_PAUSED,

        /// <summary>
        ///   Displays marquee (indeterminate) style progress
        /// </summary>
        Marquee,
    }
}