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

namespace Microsoft.Windows.Shell
{
    /// <summary>
    ///   Specifies the categories for known folders.
    /// </summary>
    public enum FolderCategory
    {
        /// <summary>
        ///   The folder category is not specified.
        /// </summary>
        None = 0x00,
        /// <summary>
        ///   The folder is a virtual folder. Virtual folders are not part 
        ///   of the file system. For example, Control Panel and 
        ///   Printers are virtual folders. A number of properties 
        ///   such as folder path and redirection do not apply to this category.
        /// </summary>
        Virtual = 0x1,
        /// <summary>
        ///   The folder is fixed. Fixed file system folders are not 
        ///   managed by the Shell and are usually given a permanent 
        ///   path when the system is installed. For example, the 
        ///   Windows and Program Files folders are fixed folders. 
        ///   A number of properties such as redirection do not apply 
        ///   to this category.
        /// </summary>
        Fixed = 0x2,
        /// <summary>
        ///   The folder is a common folder. Common folders are 
        ///   used for sharing data and settings 
        ///   accessible by all users of a system. For example, 
        ///   all users share a common Documents folder as well 
        ///   as their per-user Documents folder.
        /// </summary>
        Common = 0x3,
        /// <summary>
        ///   Each user has their own copy of the folder. Per-user folders 
        ///   are those stored under each user's profile and 
        ///   accessible only by that user.
        /// </summary>
        PerUser = 0x4
    }
}