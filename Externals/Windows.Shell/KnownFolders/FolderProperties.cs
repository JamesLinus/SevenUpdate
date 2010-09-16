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
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Media.Imaging;

#endregion

namespace Microsoft.Windows.Shell
{
    /// <summary>
    ///   Structure used internally to store property values for 
    ///   a known folder. This structure holds the information
    ///   returned in the FOLDER_DEFINITION structure, and 
    ///   resources referenced by fields in NativeFolderDefinition,
    ///   such as icon and tool tip.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct FolderProperties
    {
        internal string name;
        internal FolderCategory category;
        internal string canonicalName;
        internal string description;
        internal Guid parentId;
        internal string parent;
        internal string relativePath;
        internal string parsingName;
        internal string tooltipResourceId;
        internal string tooltip;
        internal string localizedName;
        internal string localizedNameResourceId;
        internal string iconResourceId;
        internal BitmapSource icon;
        internal DefinitionOptions definitionOptions;
        internal FileAttributes fileAttributes;
        internal Guid folderTypeId;
        internal string folderType;
        internal Guid folderId;
        internal string path;
        internal bool pathExists;
        internal RedirectionCapabilities redirection;
        internal string security;
    }
}