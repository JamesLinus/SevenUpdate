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
    ///   Indicates the various buttons and options clicked by the user on the task dialog.
    /// </summary>
    [SuppressMessage("Microsoft.Design", "CA1027:MarkEnumsWithFlags"), SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue")]
    public enum TaskDialogResult
    {
        /// <summary>
        ///   "OK" button was clicked
        /// </summary>
        Ok = 0x0001,

        /// <summary>
        ///   "Yes" button was clicked
        /// </summary>
        Yes = 0x0002,

        /// <summary>
        ///   "No" button was clicked
        /// </summary>
        No = 0x0004,

        /// <summary>
        ///   "Cancel" button was clicked
        /// </summary>
        Cancel = 0x0008,

        /// <summary>
        ///   "Retry" button was clicked
        /// </summary>
        Retry = 0x0010,

        /// <summary>
        ///   "Close" button was clicked
        /// </summary>
        Close = 0x0020,

        /// <summary>
        ///   A custom button was clicked.
        /// </summary>
        CustomButtonClicked = 0x0100,
    }
}