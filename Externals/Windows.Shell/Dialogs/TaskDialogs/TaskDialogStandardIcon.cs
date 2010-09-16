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
    ///   Specifies the icon displayed in a task dialog.
    /// </summary>
    public enum TaskDialogStandardIcon
    {
        /// <summary>
        ///   Displays no icons (default).
        /// </summary>
        None = 0,
        /// <summary>
        ///   Displays the warning icon.
        /// </summary>
        Warning = 65535,
        /// <summary>
        ///   Displays the error icon.
        /// </summary>
        Error = 65534,
        /// <summary>
        ///   Displays the Information icon.
        /// </summary>
        Information = 65533,
        /// <summary>
        ///   Displays the User Account Control shield.
        /// </summary>
        Shield = UInt16.MaxValue - 3,
        /// <summary>
        ///   Displays the User Account Control shield.
        /// </summary>
        ShieldBlue = UInt16.MaxValue - 4,
        /// <summary>
        ///   Displays the User Account Control shield with gray background.
        /// </summary>
        ShieldGray = UInt16.MaxValue - 8,
        /// <summary>
        ///   Displays a warning shield with yellow background.
        /// </summary>
        SecurityWarning = UInt16.MaxValue - 5,
        /// <summary>
        ///   Displays an erro shield with red background.
        /// </summary>
        SecurityError = UInt16.MaxValue - 6,
        /// <summary>
        ///   Displays a success shield with green background.
        /// </summary>
        ShieldGreen = UInt16.MaxValue - 7,
    }
}