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
using System.Runtime.InteropServices;
using System.Security;

#endregion

namespace Microsoft.Windows.Shell
{
    /// <summary>
    ///   Internal class that contains interop declarations for 
    ///   functions that are considered benign but that
    ///   are performance critical.
    /// </summary>
    /// <remarks>
    ///   Functions that are benign but not performance critical 
    ///   should be located in the NativeMethods class.
    /// </remarks>
    [SuppressUnmanagedCodeSecurity]
    internal static class KnownFoldersSafeNativeMethods
    {
        #region KnownFolders

        [StructLayout(LayoutKind.Sequential)]
        internal struct NativeFolderDefinition
        {
            internal FolderCategory category;
            internal IntPtr name;
            internal IntPtr description;
            internal Guid parentId;
            internal IntPtr relativePath;
            internal IntPtr parsingName;
            internal IntPtr tooltip;
            internal IntPtr localizedName;
            internal IntPtr icon;
            internal IntPtr security;
            internal UInt32 attributes;
            internal DefinitionOptions definitionOptions;
            internal Guid folderTypeId;
        }

        #endregion
    }
}