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

using System;

#endregion

namespace Microsoft.Windows.Dialogs
{
    /// <summary>
    ///   Identifies one of the standard buttons that 
    ///   can be displayed via TaskDialog.
    /// </summary>
    [Flags]
    public enum TaskDialogStandardButtons
    {
        /// <summary>
        ///   No buttons on the dialog.
        /// </summary>
        None = 0x0000,

        /// <summary>
        ///   An "OK" button.
        /// </summary>
        Ok = 0x0001,

        /// <summary>
        ///   A "Yes" button.
        /// </summary>
        Yes = 0x0002,

        /// <summary>
        ///   A "No" button.
        /// </summary>
        No = 0x0004,

        /// <summary>
        ///   A "Cancel" button.
        /// </summary>
        Cancel = 0x0008,

        /// <summary>
        ///   A "Retry" button.
        /// </summary>
        Retry = 0x0010,

        /// <summary>
        ///   A "Close" button.
        /// </summary>
        Close = 0x0020
    }
}