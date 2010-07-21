//Copyright (c) Microsoft Corporation.  All rights reserved.

#region

using System.Diagnostics.CodeAnalysis;

#endregion

namespace Microsoft.Windows.Internal
{
    /// <summary>
    ///   Class to hold string references to common interop DLLs.
    /// </summary>
    public sealed class CommonDllNames
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

        private CommonDllNames()
        {
            // Remove the public constructor
        }
    }
}