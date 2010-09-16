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

using System.Diagnostics.CodeAnalysis;

#endregion

namespace Microsoft.Windows.Internal
{
    /// <summary>
    ///   Class to hold string references to common interop DLLs.
    /// </summary>
    public static class CommonDllNames
    {
        /// <summary>
        ///   Comctl32.DLL
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Ctl")] public const string ComCtl32 = "comctl32.dll";

        /// <summary>
        ///   Kernel32.dll
        /// </summary>
        public const string Kernel32 = "kernel32.dll";

        /// <summary>
        ///   Comdlg32.dll
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Dlg")] public const string ComDlg32 = "comdlg32.dll";

        /// <summary>
        ///   User32.dll
        /// </summary>
        public const string User32 = "user32.dll";

        /// <summary>
        ///   Shell32.dll
        /// </summary>
        public const string Shell32 = "shell32.dll";
    }
}