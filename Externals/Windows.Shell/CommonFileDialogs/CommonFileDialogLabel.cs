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

using System.Diagnostics;

#endregion

namespace Microsoft.Windows.Dialogs.Controls
{
    /// <summary>
    ///   Defines the label controls in the Common File Dialog.
    /// </summary>
    public class CommonFileDialogLabel : CommonFileDialogControl
    {
        /// <summary>
        ///   Creates a new instance of this class.
        /// </summary>
        public CommonFileDialogLabel()
        {
        }

        /// <summary>
        ///   Creates a new instance of this class with the specified text.
        /// </summary>
        /// <param name = "text">The text to display for this control.</param>
        public CommonFileDialogLabel(string text) : base(text)
        {
        }

        /// <summary>
        ///   Creates a new instance of this class with the specified name and text.
        /// </summary>
        /// <param name = "name">The name of this control.</param>
        /// <param name = "text">The text to display for this control.</param>
        public CommonFileDialogLabel(string name, string text) : base(name, text)
        {
        }

        /// <summary>
        ///   Attach this control to the dialog object
        /// </summary>
        /// <param name = "dialog">Target dialog</param>
        internal override void Attach(IFileDialogCustomize dialog)
        {
            Debug.Assert(dialog != null, "CommonFileDialog.Attach: dialog parameter can not be null");

            // Add a text control
            dialog.AddText(Id, Text);

            // Sync unmanaged properties with managed properties
            SyncUnmanagedProperties();
        }
    }
}