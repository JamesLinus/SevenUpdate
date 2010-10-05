//***********************************************************************
// Assembly         : Windows.Shell
// Author           : sevenalive
// Created          : 09-17-2010
// Last Modified By : sevenalive
// Last Modified On : 10-05-2010
// Description      : 
// Copyright        : (c) Seven Software. All rights reserved.
//***********************************************************************

namespace Microsoft.Windows.Shell
{
    using System;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Windows.Media.Imaging;

    /// <summary>
    /// Structure used internally to store property values for 
    ///   a known folder. This structure holds the information
    ///   returned in the FOLDER_DEFINITION structure, and 
    ///   resources referenced by fields in NativeFolderDefinition,
    ///   such as icon and tool tip.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct FolderProperties
    {
        /// <summary>
        /// </summary>
        internal string name;

        /// <summary>
        /// </summary>
        internal FolderCategory category;

        /// <summary>
        /// </summary>
        internal string canonicalName;

        /// <summary>
        /// </summary>
        internal string description;

        /// <summary>
        /// </summary>
        internal Guid parentId;

        /// <summary>
        /// </summary>
        internal string parent;

        /// <summary>
        /// </summary>
        internal string relativePath;

        /// <summary>
        /// </summary>
        internal string parsingName;

        /// <summary>
        /// </summary>
        internal string tooltipResourceId;

        /// <summary>
        /// </summary>
        internal string tooltip;

        /// <summary>
        /// </summary>
        internal string localizedName;

        /// <summary>
        /// </summary>
        internal string localizedNameResourceId;

        /// <summary>
        /// </summary>
        internal string iconResourceId;

        /// <summary>
        /// </summary>
        internal BitmapSource icon;

        /// <summary>
        /// </summary>
        internal DefinitionOptions definitionOptions;

        /// <summary>
        /// </summary>
        internal FileAttributes fileAttributes;

        /// <summary>
        /// </summary>
        internal Guid folderTypeId;

        /// <summary>
        /// </summary>
        internal string folderType;

        /// <summary>
        /// </summary>
        internal Guid folderId;

        /// <summary>
        /// </summary>
        internal string path;

        /// <summary>
        /// </summary>
        internal bool pathExists;

        /// <summary>
        /// </summary>
        internal RedirectionCapability redirection;

        /// <summary>
        /// </summary>
        internal string security;

        /// <summary>
        /// </summary>
        /// <returns>
        /// </returns>
        /// <exception cref="NotImplementedException">
        /// </exception>
        public override int GetHashCode()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// </summary>
        /// <param name="obj">
        /// </param>
        /// <returns>
        /// </returns>
        /// <exception cref="NotImplementedException">
        /// </exception>
        public override bool Equals(object obj)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// </summary>
        /// <param name="x">
        /// </param>
        /// <param name="y">
        /// </param>
        /// <returns>
        /// </returns>
        /// <exception cref="NotImplementedException">
        /// </exception>
        public static bool operator ==(FolderProperties x, FolderProperties y)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// </summary>
        /// <param name="x">
        /// </param>
        /// <param name="y">
        /// </param>
        /// <returns>
        /// </returns>
        /// <exception cref="NotImplementedException">
        /// </exception>
        public static bool operator !=(FolderProperties x, FolderProperties y)
        {
            throw new NotImplementedException();
        }
    }
}