// ***********************************************************************
// <copyright file="NativeMethods.cs"
//            project="SevenUpdate.Helper"
//            assembly="SevenUpdate.Helper"
//            solution="SevenUpdate"
//            company="Seven Software">
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
namespace SevenUpdate.Helper
{
    using System.Runtime.InteropServices;

    /// <summary>The Win32 native methods</summary>
    internal static class NativeMethods
    {
        /// <summary>Moves the file using the windows command</summary>
        /// <param name="sourceFileName">The current name of the file or directory on the local computer.</param>
        /// <param name="newFileName">The new name of the file or directory on the local computer.</param>
        /// <param name="flags">The flags that determine how to move the file</param>
        /// <returns>If the function succeeds, the return value is nonzero. If the function fails, the return value is zero (0). To get extended error information, call GetLastError.</returns>
        [DllImport(@"kernel32.dll", CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool MoveFileExW(string sourceFileName, string newFileName, int flags);
    }
}