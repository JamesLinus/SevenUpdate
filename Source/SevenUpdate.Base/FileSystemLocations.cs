// Copyright 2007-2010 Robert Baker, Seven Software.
// This file is part of Seven Update.
// Seven Update is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
// Seven Update is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.
// You should have received a copy of the GNU General Public License along with Seven Update.  If not, see <http://www.gnu.org/licenses/>.
namespace SevenUpdate
{
    using System;
    using System.Runtime.InteropServices;
    using System.Text;

    /// <summary>
    /// Contains Win32 native methods
    /// </summary>
    public static class NativeMethods
    {
        #region Constants

        /// <summary>
        ///   ALLUSERS%\Start Menu\Programs
        /// </summary>
        public const int CommonPrograms = 0x0017;

        /// <summary>
        ///   %ALLUSERS%\Start Menu
        /// </summary>
        internal const int CommonStartMenu = 0x0016;

        #endregion

        /// <summary>
        /// Gets the system folder(s) of a path
        /// </summary>
        /// <param name="owner">The HWND owner.</param>
        /// <param name="path">The path to output the expanded system variable</param>
        /// <param name="nFolder">The n folder.</param>
        /// <param name="fCreate">if set to <see langword="true"/> the path will be created</param>
        /// <returns>
        /// a string of the path with expanded system variables
        /// </returns>
        [DllImport("shell32.dll")] // ReSharper disable InconsistentNaming
        public static extern bool SHGetSpecialFolderPath(IntPtr owner, [Out] StringBuilder path, int nFolder, bool fCreate);
    }
}