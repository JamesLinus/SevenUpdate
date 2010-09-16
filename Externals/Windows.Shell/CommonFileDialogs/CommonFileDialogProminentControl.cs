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

using System.Windows.Markup;

#endregion

namespace Microsoft.Windows.Dialogs.Controls
{
    /// <summary>
    ///   Defines the properties and constructors for all prominent controls in the Common File Dialog.
    /// </summary>
    [ContentProperty("Items")]
    public abstract class CommonFileDialogProminentControl : CommonFileDialogControl
    {
        /// <summary>
        ///   Creates a new instance of this class.
        /// </summary>
        protected CommonFileDialogProminentControl()
        {
        }

        /// <summary>
        ///   Creates a new instance of this class with the specified text.
        /// </summary>
        /// <param name = "text">The text to display for this control.</param>
        protected CommonFileDialogProminentControl(string text) : base(text)
        {
        }

        /// <summary>
        ///   Creates a new instance of this class with the specified name and text.
        /// </summary>
        /// <param name = "name">The name of this control.</param>
        /// <param name = "text">The text to display for this control.</param>
        protected CommonFileDialogProminentControl(string name, string text) : base(name, text)
        {
        }

        /// <summary>
        ///   Gets or sets the prominent value of this control.
        /// </summary>
        /// <remarks>
        ///   Only one control can be specified as prominent. If more than one control is specified prominent, 
        ///   then an 'E_UNEXPECTED' exception will be thrown when these controls are added to the dialog. 
        ///   A group box control can only be specified as prominent if it contains one control and that control is of type 'CommonFileDialogProminentControl'.
        /// </remarks>
        public bool IsProminent { get; set; }
    }
}