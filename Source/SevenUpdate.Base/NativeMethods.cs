// ***********************************************************************
// Assembly         : SevenUpdate.Base
// Author           : sevenalive
// Created          : 09-17-2010
//
// Last Modified By : sevenalive
// Last Modified On : 10-05-2010
// Description      : 
//
// Copyright        : (c) Seven Software. All rights reserved.
// ***********************************************************************
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
        /// <param name="owner">
        /// The HWND owner.
        /// </param>
        /// <param name="path">
        /// The path to output the expanded system variable
        /// </param>
        /// <param name="nFolder">
        /// The n folder.
        /// </param>
        /// <param name="fCreate">
        /// if set to <see langword="true"/> the path will be created
        /// </param>
        /// <returns>
        /// a string of the path with expanded system variables
        /// </returns>
        [DllImport("shell32.dll")] // ReSharper disable InconsistentNaming
        public static extern bool SHGetSpecialFolderPath(IntPtr owner, [Out] StringBuilder path, int nFolder, bool fCreate);
    }
}