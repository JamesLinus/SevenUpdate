// ***********************************************************************
// <copyright file="NativeMethods.cs" project="SevenUpdate.Base" assembly="SevenUpdate.Base" solution="SevenUpdate" company="Seven Software">
//     Copyright (c) Seven Software. All rights reserved.
// </copyright>
// <author username="sevenalive">Robert Baker</author>
// <license href="http://www.gnu.org/licenses/gpl-3.0.txt" name="GNU General Public License 3">
//  This file is part of Seven Update.
//    Seven Update is free software: you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation, either version 3 of the License, or
//    (at your option) any later version.
//    Seven Update is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.
//    You should have received a copy of the GNU General Public License
//    along with Seven Update.  If not, see http://www.gnu.org/licenses/.
// </license>
// ***********************************************************************

namespace SevenUpdate
{
    using System.Runtime.InteropServices;
    using System.Text;

    /// <summary>Contains Win32 native methods.</summary>
    internal static class NativeMethods
    {
        /// <summary>Moves the file using the windows command.</summary>
        /// <param name="sourceFileName">The current name of the file or directory on the local computer.</param>
        /// <param name="newFileName">The new name of the file or directory on the local computer.</param>
        /// <param name="flags">The flags that determine how to move the file.</param>
        /// <returns>If the function succeeds, the return value is nonzero. If the function fails, the return value is zero (0). To get extended error information, call GetLastError.</returns>
        [DllImport(@"kernel32.dll", CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool MoveFileExW(string sourceFileName, string newFileName, int flags);

        /// <summary>The MsiGetComponentPath function returns the full path to an installed component. If the key path for the component is a registry key then the registry key is returned.</summary>
        /// <param name="productCode">The product code.</param>
        /// <param name="componentCode">The component code.</param>
        /// <param name="componentPath">A pointer to a variable that receives the path to the component. This parameter can be <see langword="null" />. If the component is a registry key, the registry roots are represented numerically. If this is a registry subkey path, there is a backslash at the end of the Key Path. If this is a registry value key path, there is no backslash at the end. For example, a registry path on a 32-bit operating system of HKEY_CURRENT_USER\SOFTWARE\Microsoft is returned as "01:\SOFTWARE\Microsoft\". The registry roots returned on 32-bit operating systems are defined as shown in the following table.</param>
        /// <param name="componentPathBufferSize">A pointer to a variable that specifies the size, in characters, of the buffer pointed to by the <paramref name="componentPath" /> parameter. On input, this is the full size of the buffer, including a space for a terminating <see langword="null" /> character. If the buffer passed in is too small, the count returned does not include the terminating <see langword="null" /> character.</param>
        /// <returns>The install state of the component.</returns>
        [DllImport(@"msi.dll", CharSet = CharSet.Unicode)]
        internal static extern int MsiGetComponentPath(string productCode, string componentCode, StringBuilder componentPath, ref int componentPathBufferSize);

        /// <summary>Gets the target for the msi shortcut.</summary>
        /// <param name="targetFile">A <see langword="null" />-terminated string specifying the full path to a shortcut.</param>
        /// <param name="productCode">A GUID for the product code of the shortcut. This string buffer must be 39 characters long. The first 38 characters are for the GUID, and the last character is for the terminating <see langword="null" /> character. This parameter can be <see langword="null" />.</param>
        /// <param name="featureID">The feature name of the shortcut. The string buffer must be MAX_FEATURE_CHARS+1 characters long. This parameter can be <see langword="null" />.</param>
        /// <param name="componentCode">A GUID of the component code. This string buffer must be 39 characters long. The first 38 characters are for the GUID, and the last character is for the terminating <see langword="null" /> character. This parameter can be <see langword="null" />.</param>
        /// <returns>The return code.</returns>
        [DllImport(@"msi.dll", CharSet = CharSet.Unicode)]
        internal static extern int MsiGetShortcutTarget(string targetFile, StringBuilder productCode, StringBuilder featureID, StringBuilder componentCode);
    }
}