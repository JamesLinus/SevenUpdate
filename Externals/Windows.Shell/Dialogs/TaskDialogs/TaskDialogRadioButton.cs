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

namespace Microsoft.Windows.Dialogs
{
    /// <summary>
    ///   Defines a radio button that can be hosted in by a 
    ///   <see cref = "TaskDialog" /> object.
    /// </summary>
    public abstract class TaskDialogRadioButton : TaskDialogButtonBase
    {
        /// <summary>
        ///   Creates a new instance of this class.
        /// </summary>
        public TaskDialogRadioButton()
        {
        }

        /// <summary>
        ///   Creates a new instance of this class with
        ///   the specified name and text.
        /// </summary>
        /// <param name = "name">The name for this control.</param>
        /// <param name = "text">The value for this controls 
        ///   <see cref = "P:Microsoft.Windows.Dialogs.TaskDialogButtonBase.Text" /> property.</param>
        public TaskDialogRadioButton(string name, string text) : base(name, text)
        {
        }
    }
}