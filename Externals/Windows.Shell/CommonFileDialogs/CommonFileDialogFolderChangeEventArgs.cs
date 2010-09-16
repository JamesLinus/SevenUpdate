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

using System.ComponentModel;

#endregion

namespace Microsoft.Windows.Dialogs
{
    /// <summary>
    ///   Creates the event data associated with <see cref = "CommonFileDialog.FolderChanging" /> event.
    /// </summary>
    public class CommonFileDialogFolderChangeEventArgs : CancelEventArgs
    {
        /// <summary>
        ///   Creates a new instance of this class.
        /// </summary>
        /// <param name = "folder">The name of the folder.</param>
        public CommonFileDialogFolderChangeEventArgs(string folder)
        {
            Folder = folder;
        }

        /// <summary>
        ///   Gets or sets the name of the folder.
        /// </summary>
        public string Folder { get; set; }
    }
}