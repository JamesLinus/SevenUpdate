// ***********************************************************************
// Assembly         : System.Windows
// Author           : Robert Baker (sevenalive)
// Last Modified By : Robert Baker (sevenalive)
// Last Modified On : 10-06-2010
// Copyright        : (c) Seven Software. All rights reserved.
// ***********************************************************************
namespace System.Windows
{
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Text;

    /// <summary>
    /// Reads and writes a shortcut
    /// </summary>
    public static class Shortcut
    {
        #region Constants and Fields

        /// <summary>
        /// The max feature length
        /// </summary>
        private const int MaxFeatureLength = 38;

        /// <summary>
        /// The max Guid length
        /// </summary>
        private const int MaxGuidLength = 38;

        /// <summary>
        /// The max path
        /// </summary>
        private const int MaxPath = 260;

        /// <summary>
        /// The path path length
        /// </summary>
        private const int MaxPathLength = 1024;

        /// <summary>
        /// The read constant
        /// </summary>
        private const uint Read = 0;

        #endregion

        #region Interfaces

        /// <summary>
        /// </summary>
        [ComImport]
        [Guid("0000010c-0000-0000-c000-000000000046")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        private interface IPersist
        {
            /// <summary>
            /// </summary>
            /// <parameter name="classID">
            /// </parameter>
            [PreserveSig]
            void GetClassID(out Guid classID);
        }

        /// <summary>
        /// The persistent file for win32
        /// </summary>
        [ComImport]
        [Guid("0000010b-0000-0000-C000-000000000046")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        private interface IPersistFile : IPersist
        {
            /// <summary>
            /// </summary>
            /// <parameter name="cassID">
            /// </parameter>
            new void GetClassID(out Guid classID);

            /// <summary>
            /// </summary>
            /// <returns>
            /// </returns>
            [PreserveSig]
            int IsDirty();

            /// <summary>
            /// </summary>
            /// <parameter name="fileName">
            /// </parameter>
            /// <parameter name="mode">
            /// </parameter>
            [PreserveSig]
            void Load([In] [MarshalAs(UnmanagedType.LPWStr)] string fileName, uint mode);

            /// <summary>
            /// </summary>
            /// <parameter name="fileName">
            /// </parameter>
            /// <parameter name="remember">
            /// </parameter>
            [PreserveSig]
            void Save([In] [MarshalAs(UnmanagedType.LPWStr)] string fileName, [In] [MarshalAs(UnmanagedType.Bool)] bool remember);

            /// <summary>
            /// </summary>
            /// <parameter name="FileName">
            /// </parameter>
            [PreserveSig]
            void SaveCompleted([In] [MarshalAs(UnmanagedType.LPWStr)] string fileName);

            /// <summary>
            /// </summary>
            /// <parameter name="fileName">
            /// </parameter>
            [PreserveSig]
            void GetCurFile([In] [MarshalAs(UnmanagedType.LPWStr)] string fileName);
        }

        /// <summary>
        /// The IShellLink interface allows Shell links to be created, modified, and resolved
        /// </summary>
        [ComImport]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        [Guid("000214F9-0000-0000-C000-000000000046")]
        private interface IShellLink
        {
            /// <summary>
            /// Retrieves the path and file name of a Shell link object
            /// </summary>
            /// <parameter name="file">
            /// </parameter>
            /// <parameter name="maxPath">
            /// </parameter>
            /// <parameter name="data">
            /// </parameter>
            /// <parameter name="flags">
            /// </parameter>
            void GetPath([Out] [MarshalAs(UnmanagedType.LPWStr)] StringBuilder file, int maxPath, out Win32FindData data, int flags);

            /// <summary>
            /// Retrieves the list of item identifiers for a Shell link object
            /// </summary>
            /// <parameter name="indentifer">
            /// </parameter>
            void GetIDList(out IntPtr indentifer);

            /// <summary>
            /// Sets the pointer to an item identifier list (PIDL) for a Shell link object.
            /// </summary>
            /// <parameter name="indentifer">
            /// The indentifer.
            /// </parameter>
            void SetIDList(IntPtr indentifer);

            /// <summary>
            /// Retrieves the description string for a Shell link object
            /// </summary>
            /// <parameter name="name">
            /// The name.
            /// </parameter>
            /// <parameter name="maxName">
            /// Name of the max.
            /// </parameter>
            void GetDescription([Out] [MarshalAs(UnmanagedType.LPWStr)] StringBuilder name, int maxName);

            /// <summary>
            /// Sets the description for a Shell link object. The description can be any application-defined string
            /// </summary>
            /// <parameter name="name">
            /// The name.
            /// </parameter>
            void SetDescription([MarshalAs(UnmanagedType.LPWStr)] string name);

            /// <summary>
            /// Retrieves the name of the working directory for a Shell link object
            /// </summary>
            /// <parameter name="dir">
            /// The dir.
            /// </parameter>
            /// <parameter name="maxPath">
            /// The max path.
            /// </parameter>
            void GetWorkingDirectory([Out] [MarshalAs(UnmanagedType.LPWStr)] StringBuilder dir, int maxPath);

            /// <summary>
            /// Sets the name of the working directory for a Shell link object
            /// </summary>
            /// <parameter name="dir">
            /// The dir.
            /// </parameter>
            void SetWorkingDirectory([MarshalAs(UnmanagedType.LPWStr)] string dir);

            /// <summary>
            /// Retrieves the command-line arguments associated with a Shell link object
            /// </summary>
            /// <parameter name="args">
            /// </parameter>
            /// <parameter name="maxPath">
            /// </parameter>
            void GetArguments([Out] [MarshalAs(UnmanagedType.LPWStr)] StringBuilder args, int maxPath);

            /// <summary>
            /// Sets the command-line arguments for a Shell link object
            /// </summary>
            /// <parameter name="args">
            /// </parameter>
            void SetArguments([MarshalAs(UnmanagedType.LPWStr)] string args);

            /// <summary>
            /// Retrieves the hot key for a Shell link object
            /// </summary>
            /// <parameter name="hotkey">
            /// </parameter>
            void GetHotkey(out short hotkey);

            /// <summary>
            /// Sets a hot key for a Shell link object
            /// </summary>
            /// <parameter name="hotkey">
            /// </parameter>
            void SetHotkey(short hotkey);

            /// <summary>
            /// Retrieves the show command for a Shell link object
            /// </summary>
            /// <parameter name="showCmd">
            /// </parameter>
            void GetShowCmd(out int showCmd);

            /// <summary>
            /// Sets the show command for a Shell link object. The show command sets the initial show state of the window.
            /// </summary>
            /// <parameter name="showCmd">
            /// </parameter>
            void SetShowCmd(int showCmd);

            /// <summary>
            /// Retrieves the location (path and index) of the icon for a Shell link object
            /// </summary>
            /// <parameter name="iconPath">
            /// </parameter>
            /// <parameter name="iconPathLength">
            /// </parameter>
            /// <parameter name="iconIndex">
            /// </parameter>
            void GetIconLocation([Out] [MarshalAs(UnmanagedType.LPWStr)] StringBuilder iconPath, int iconPathLength, out int iconIndex);

            /// <summary>
            /// Sets the location (path and index) of the icon for a Shell link object
            /// </summary>
            /// <parameter name="iconPath">
            /// </parameter>
            /// <parameter name="iconIndex">
            /// </parameter>
            void SetIconLocation([MarshalAs(UnmanagedType.LPWStr)] string iconPath, int iconIndex);

            /// <summary>
            /// Sets the relative path to the Shell link object
            /// </summary>
            /// <parameter name="relativePath">
            /// </parameter>
            /// <parameter name="reserved">
            /// </parameter>
            void SetRelativePath([MarshalAs(UnmanagedType.LPWStr)] string relativePath, int reserved);

            /// <summary>
            /// Sets the path and file name of a Shell link object
            /// </summary>
            /// <parameter name="file">
            /// </parameter>
            void SetPath([MarshalAs(UnmanagedType.LPWStr)] string file);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets data associated with a shortcut
        /// </summary>
        /// <parameter name="shortcutName">
        /// The full path to the shortcut lnk file
        /// </parameter>
        /// <returns>
        /// The data for the shortcut
        /// </returns>
        public static ShellLink GetShortcutData(string shortcutName)
        {
            var link = new ShellLink();
            ((IPersistFile)link).Load(shortcutName, Read);

            var sb = new StringBuilder(MaxPath);
            var shortcut = new ShellLink { Target = GetMsiTargetPath(shortcutName), Name = Path.GetFileNameWithoutExtension(shortcutName) };

            if (shortcut.Target == null)
            {
                Win32FindData data;
                ((IShellLink)link).GetPath(sb, sb.Capacity, out data, 0);
                shortcut.Target = sb.ToString();
            }

            ((IShellLink)link).GetArguments(sb, sb.Capacity);
            shortcut.Arguments = sb.ToString();

            ((IShellLink)link).GetDescription(sb, sb.Capacity);
            shortcut.Description = sb.ToString();

            int iconIndex;
            ((IShellLink)link).GetIconLocation(sb, sb.Capacity, out iconIndex);
            var icon = sb.ToString();
            if (String.IsNullOrWhiteSpace(icon))
            {
                shortcut.Icon = shortcut.Target + @"," + iconIndex;
            }
            else
            {
                shortcut.Icon = sb.ToString();
            }

            shortcut.Location = shortcutName;

            return shortcut;
        }

        #endregion

        #region Methods

        /// <summary>
        /// </summary>
        /// <parameter name="owner">
        /// </parameter>
        /// <parameter name="folder">
        /// </parameter>
        /// <parameter name="token">
        /// </parameter>
        /// <parameter name="flags">
        /// </parameter>
        /// <parameter name="path">
        /// </parameter>
        /// <returns>
        /// </returns>
        [DllImport(@"shfolder.dll", CharSet = CharSet.Auto)]
        internal static extern int SHGetFolderPath(IntPtr owner, int folder, IntPtr token, int flags, StringBuilder path);

        /// <summary>
        /// Gets the target path from a Msi shortcut
        /// </summary>
        /// <parameter name="shortcutPath">
        /// The path to the shortcut lnk file
        /// </parameter>
        /// <returns>
        /// The resolved path to the shortcut
        /// </returns>
        private static string GetMsiTargetPath(string shortcutPath)
        {
            var product = new StringBuilder(MaxGuidLength + 1);
            var feature = new StringBuilder(MaxFeatureLength + 1);
            var component = new StringBuilder(MaxGuidLength + 1);

            MsiGetShortcutTarget(shortcutPath, product, feature, component);

            var pathLength = MaxPathLength;
            var path = new StringBuilder(pathLength);

            var installState = MsiGetComponentPath(product.ToString(), component.ToString(), path, ref pathLength);
            return installState == 4 ? path.ToString() : null;
        }

        /// <summary>
        /// </summary>
        /// <parameter name="productCode">
        /// </parameter>
        /// <parameter name="componentCode">
        /// </parameter>
        /// <parameter name="componentPath">
        /// </parameter>
        /// <parameter name="componentPathBufferSize">
        /// </parameter>
        /// <returns>
        /// </returns>
        [DllImport(@"msi.dll", CharSet = CharSet.Auto)]
        private static extern int MsiGetComponentPath(string productCode, string componentCode, StringBuilder componentPath, ref int componentPathBufferSize);

        /// <summary>
        /// </summary>
        /// <parameter name="targetFile">
        /// </parameter>
        /// <parameter name="productCode">
        /// </parameter>
        /// <parameter name="featureID">
        /// </parameter>
        /// <parameter name="componentCode">
        /// </parameter>
        /// <returns>
        /// </returns>
        [DllImport(@"msi.dll", CharSet = CharSet.Auto)]
        private static extern int MsiGetShortcutTarget(string targetFile, StringBuilder productCode, StringBuilder featureID, StringBuilder componentCode);

        #endregion

        /// <summary>
        /// The Win32 file data
        /// </summary>
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        private struct Win32FindData
        {
            /// <summary>
            ///   The file attributes
            /// </summary>
            private readonly uint FileAttributes;

            /// <summary>
            ///   The time the file was created
            /// </summary>
            private readonly long CreationTime;

            /// <summary>
            ///   The time the file was last accessed
            /// </summary>
            private readonly long LastAccessTime;

            /// <summary>
            ///   The time the file was last written to
            /// </summary>
            private readonly long LastWriteTime;

            /// <summary>
            ///   The file size
            /// </summary>
            private readonly uint FileSizeHigh;

            /// <summary>
            ///   The file size
            /// </summary>
            private readonly uint FileSizeLow;

            /// <summary>
            ///   Reserved data
            /// </summary>
            private readonly uint Reserved0;

            /// <summary>
            ///   Reserved data
            /// </summary>
            private readonly uint Reserved1;

            /// <summary>
            ///   The name of the file
            /// </summary>
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            private readonly string FileName;

            /// <summary>
            ///   The alternate name of the file
            /// </summary>
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 14)]
            private readonly string AlternateFileName;
        }
    }
}