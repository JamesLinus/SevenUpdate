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
    using System.Runtime.InteropServices;
    using System.Security;

    /// <summary>
    /// Internal class that contains interop declarations for 
    ///   functions that are considered benign but that
    ///   are performance critical.
    /// </summary>
    /// <remarks>
    /// Functions that are benign but not performance critical 
    ///   should be located in the NativeMethods class.
    /// </remarks>
    [SuppressUnmanagedCodeSecurity]
    internal static class KnownFoldersSafeNativeMethods
    {
        #region KnownFolders

        /// <summary>
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        internal struct NativeFolderDefinition
        {
            /// <summary>
            /// </summary>
            internal FolderCategory category;

            /// <summary>
            /// </summary>
            internal IntPtr name;

            /// <summary>
            /// </summary>
            internal IntPtr description;

            /// <summary>
            /// </summary>
            internal Guid parentId;

            /// <summary>
            /// </summary>
            internal IntPtr relativePath;

            /// <summary>
            /// </summary>
            internal IntPtr parsingName;

            /// <summary>
            /// </summary>
            internal IntPtr tooltip;

            /// <summary>
            /// </summary>
            internal IntPtr localizedName;

            /// <summary>
            /// </summary>
            internal IntPtr icon;

            /// <summary>
            /// </summary>
            internal IntPtr security;

            /// <summary>
            /// </summary>
            internal uint attributes;

            /// <summary>
            /// </summary>
            internal DefinitionOptions definitionOptions;

            /// <summary>
            /// </summary>
            internal Guid folderTypeId;

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
            public static bool operator ==(NativeFolderDefinition x, NativeFolderDefinition y)
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
            public static bool operator !=(NativeFolderDefinition x, NativeFolderDefinition y)
            {
                throw new NotImplementedException();
            }
        }

        #endregion
    }
}