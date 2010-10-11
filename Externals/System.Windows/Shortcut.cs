// ***********************************************************************
// <copyright file="Shortcut.cs"
//            project="System.Windows"
//            assembly="System.Windows"
//            solution="SevenUpdate"
//            company="Seven Software">
//     Copyright (c) Seven Software. All rights reserved.
// </copyright>
// <author username="sevenalive">Robert Baker</author>
// ***********************************************************************
namespace System.Windows
{
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Text;

    /// <summary>
    /// The Msi Component install state
    /// </summary>
    internal enum InstallState
    {
        /// <summary>
        ///   The component being requested is disabled on the computer.
        /// </summary>
        NotUsed = -7, 

        /// <summary>
        ///   The configuration data is corrupt.
        /// </summary>
        BadConfig = -6, 

        /// <summary>
        ///   The installation is incomplete
        /// </summary>
        Incomplete = -5, 

        /// <summary>
        ///   The component source is inaccessible.
        /// </summary>
        SourceAbsent = -4, 

        /// <summary>
        ///   One of the function parameters is invalid.
        /// </summary>
        InvalidArg = -2, 

        /// <summary>
        ///   The product code or component ID is unknown.
        /// </summary>
        Unknown = -1, 

        /// <summary>
        ///   The shortcut is advertised
        /// </summary>
        Advertised = 1, 

        /// <summary>
        ///   The component has been removed
        /// </summary>
        Removed = 1, 

        /// <summary>
        ///   The component is not installed.
        /// </summary>
        Absent = 2, 

        /// <summary>
        ///   The component is installed locally.
        /// </summary>
        Local = 3, 

        /// <summary>
        ///   The component is installed to run from source.
        /// </summary>
        Source = 4, 
    }

    /// <summary>
    /// Reads and writes a shortcut
    /// </summary>
    public static class Shortcut
    {
        #region Constants and Fields

        /// <summary>
        ///   The max feature length
        /// </summary>
        private const int MaxFeatureLength = 38;

        /// <summary>
        ///   The max Guid length
        /// </summary>
        private const int MaxGuidLength = 38;

        /// <summary>
        ///   The max path
        /// </summary>
        private const int MaxPath = 260;

        /// <summary>
        ///   The path path length
        /// </summary>
        private const int MaxPathLength = 1024;

        /// <summary>
        ///   The read constant
        /// </summary>
        private const uint Read = 0;

        #endregion

        #region Interfaces

        /// <summary>
        /// The interface for a Persistent file
        /// </summary>
        [ComImport]
        [Guid("0000010c-0000-0000-c000-000000000046")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        private interface IPersist
        {
            /// <summary>
            /// Gets the class ID.
            /// </summary>
            /// <param name="classID">
            /// The class ID.
            /// </param>
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
            /// Gets the class ID.
            /// </summary>
            /// <param name="classID">
            /// The class ID.
            /// </param>
            new void GetClassID(out Guid classID);

            /// <summary>
            /// Determines whether this instance is dirty.
            /// </summary>
            /// <returns>
            /// the error result
            /// </returns>
            [PreserveSig]
            int IsDirty();

            /// <summary>
            /// Loads the specified file name.
            /// </summary>
            /// <param name="fileName">
            /// Name of the file.
            /// </param>
            /// <param name="mode">
            /// The mode how to load the file
            /// </param>
            [PreserveSig]
            void Load([In] [MarshalAs(UnmanagedType.LPWStr)] string fileName, uint mode);

            /// <summary>
            /// Saves the specified file name.
            /// </summary>
            /// <param name="fileName">
            /// Name of the file.
            /// </param>
            /// <param name="remember">
            /// if set to <see langword="true"/> [remember].
            /// </param>
            [PreserveSig]
            void Save([In] [MarshalAs(UnmanagedType.LPWStr)] string fileName, [In] [MarshalAs(UnmanagedType.Bool)] bool remember);

            /// <summary>
            /// Saves the shortcut
            /// </summary>
            /// <param name="fileName">
            /// Name of the file.
            /// </param>
            [PreserveSig]
            void SaveCompleted([In] [MarshalAs(UnmanagedType.LPWStr)] string fileName);

            /// <summary>
            /// Gets the shortcut from the filename
            /// </summary>
            /// <param name="fileName">
            /// Name of the file.
            /// </param>
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
            /// <param name="file">
            /// The filename of the shortcut
            /// </param>
            /// <param name="maxPath">
            /// The max path.
            /// </param>
            /// <param name="data">
            /// The data to get
            /// </param>
            /// <param name="flags">
            /// The options to specify the path is retrieved.
            /// </param>
            void GetPath([Out] [MarshalAs(UnmanagedType.LPWStr)] StringBuilder file, int maxPath, out Win32FindData data, int flags);

            /// <summary>
            /// Retrieves the list of item identifiers for a Shell link object
            /// </summary>
            /// <param name="indentifer">
            /// The indentifer list.
            /// </param>
            void GetIDList(out IntPtr indentifer);

            /// <summary>
            /// Sets the pointer to an item identifier list (PIDL) for a Shell link object.
            /// </summary>
            /// <param name="indentifer">
            /// The indentifer list
            /// </param>
            void SetIDList(IntPtr indentifer);

            /// <summary>
            /// Retrieves the description string for a Shell link object
            /// </summary>
            /// <param name="description">
            /// The description.
            /// </param>
            /// <param name="maxName">
            /// Name of the max.
            /// </param>
            void GetDescription([Out] [MarshalAs(UnmanagedType.LPWStr)] StringBuilder description, int maxName);

            /// <summary>
            /// Sets the description for a Shell link object. The description can be any application-defined string
            /// </summary>
            /// <param name="description">
            /// The description.
            /// </param>
            void SetDescription([MarshalAs(UnmanagedType.LPWStr)] string description);

            /// <summary>
            /// Retrieves the name of the working directory for a Shell link object
            /// </summary>
            /// <param name="dir">
            /// The working directory.
            /// </param>
            /// <param name="maxPath">
            /// The max path.
            /// </param>
            void GetWorkingDirectory([Out] [MarshalAs(UnmanagedType.LPWStr)] StringBuilder dir, int maxPath);

            /// <summary>
            /// Sets the name of the working directory for a Shell link object
            /// </summary>
            /// <param name="dir">
            /// The working directory.
            /// </param>
            void SetWorkingDirectory([MarshalAs(UnmanagedType.LPWStr)] string dir);

            /// <summary>
            /// Retrieves the command-line arguments associated with a Shell link object
            /// </summary>
            /// <param name="args">
            /// The arguments for the shortcut
            /// </param>
            /// <param name="maxPath">
            /// The max path.
            /// </param>
            void GetArguments([Out] [MarshalAs(UnmanagedType.LPWStr)] StringBuilder args, int maxPath);

            /// <summary>
            /// Sets the command-line arguments for a Shell link object
            /// </summary>
            /// <param name="args">
            /// The arguments for the shortcut
            /// </param>
            void SetArguments([MarshalAs(UnmanagedType.LPWStr)] string args);

            /// <summary>
            /// Retrieves the hot key for a Shell link object
            /// </summary>
            /// <param name="hotkey">
            /// The hotkey.
            /// </param>
            void GetHotkey(out short hotkey);

            /// <summary>
            /// Sets a hot key for a Shell link object
            /// </summary>
            /// <param name="hotkey">
            /// The hotkey.
            /// </param>
            void SetHotkey(short hotkey);

            /// <summary>
            /// Retrieves the show command for a Shell link object
            /// </summary>
            /// <param name="showCmd">
            /// The show CMD.
            /// </param>
            void GetShowCmd(out int showCmd);

            /// <summary>
            /// Sets the show command for a Shell link object. The show command sets the initial show state of the window.
            /// </summary>
            /// <param name="showCmd">
            /// The show CMD.
            /// </param>
            void SetShowCmd(int showCmd);

            /// <summary>
            /// Retrieves the location (path and index) of the icon for a Shell link object
            /// </summary>
            /// <param name="iconPath">
            /// The icon path.
            /// </param>
            /// <param name="iconPathLength">
            /// Length of the icon path.
            /// </param>
            /// <param name="iconIndex">
            /// Index of the icon.
            /// </param>
            void GetIconLocation([Out] [MarshalAs(UnmanagedType.LPWStr)] StringBuilder iconPath, int iconPathLength, out int iconIndex);

            /// <summary>
            /// Sets the location (path and index) of the icon for a Shell link object
            /// </summary>
            /// <param name="iconPath">
            /// The icon path.
            /// </param>
            /// <param name="iconIndex">
            /// Index of the icon.
            /// </param>
            void SetIconLocation([MarshalAs(UnmanagedType.LPWStr)] string iconPath, int iconIndex);

            /// <summary>
            /// Sets the relative path to the Shell link object
            /// </summary>
            /// <param name="relativePath">
            /// The relative path.
            /// </param>
            /// <param name="reserved">
            /// The reserved.
            /// </param>
            void SetRelativePath([MarshalAs(UnmanagedType.LPWStr)] string relativePath, int reserved);

            /// <summary>
            /// Sets the path and file name of a Shell link object
            /// </summary>
            /// <param name="file">
            /// The file to set the path
            /// </param>
            void SetPath([MarshalAs(UnmanagedType.LPWStr)] string file);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets data associated with a shortcut
        /// </summary>
        /// <param name="shortcutName">
        /// The full path to the shortcut lnk file
        /// </param>
        /// <returns>
        /// The data for the shortcut
        /// </returns>
        public static ShellLink GetShortcutData(string shortcutName)
        {
            var link = new ShellLink();
            ((IPersistFile)link).Load(shortcutName, Read);

            var sb = new StringBuilder(MaxPath);
            var shortcut = new ShellLink
                {
                    Target = GetMsiTargetPath(shortcutName), 
                    Name = Path.GetFileNameWithoutExtension(shortcutName)
                };

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
        /// Gets the target path from a Msi shortcut
        /// </summary>
        /// <param name="shortcutPath">
        /// The path to the shortcut lnk file
        /// </param>
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
            return installState == InstallState.Source ? path.ToString() : null;
        }

        /// <summary>
        /// The MsiGetComponentPath function returns the full path to an installed component. If the key path for the component is a registry key then the registry key is returned.
        /// </summary>
        /// <param name="productCode">
        /// The product code.
        /// </param>
        /// <param name="componentCode">
        /// The component code.
        /// </param>
        /// <param name="componentPath">
        /// A pointer to a variable that receives the path to the component. This parameter can be <see langword="null"/>. If the component is a registry key, the registry roots are represented numerically. If this is a registry subkey path, there is a backslash at the end of the Key Path. If this is a registry value key path, there is no backslash at the end. For example, a registry path on a 32-bit operating system of HKEY_CURRENT_USER\SOFTWARE\Microsoft is returned as "01:\SOFTWARE\Microsoft\". The registry roots returned on 32-bit operating systems are defined as shown in the following table.
        /// </param>
        /// <param name="componentPathBufferSize">
        /// A pointer to a variable that specifies the size, in characters, of the buffer pointed to by the <paramref name="componentPath"/> parameter. On input, this is the full size of the buffer, including a space for a terminating <see langword="null"/> character. If the buffer passed in is too small, the count returned does not include the terminating <see langword="null"/> character.
        /// </param>
        /// <returns>
        /// the install state of the component
        /// </returns>
        [DllImport(@"msi.dll", CharSet = CharSet.Auto)]
        private static extern InstallState MsiGetComponentPath(string productCode, string componentCode, StringBuilder componentPath, ref int componentPathBufferSize);

        /// <summary>
        /// Gets the target for the msi shortcut
        /// </summary>
        /// <param name="targetFile">
        /// A <see langword="null"/>-terminated string specifying the full path to a shortcut.
        /// </param>
        /// <param name="productCode">
        /// A GUID for the product code of the shortcut. This string buffer must be 39 characters long. The first 38 characters are for the GUID, and the last character is for the terminating <see langword="null"/> character. This parameter can be <see langword="null"/>.
        /// </param>
        /// <param name="featureID">
        /// The feature name of the shortcut. The string buffer must be MAX_FEATURE_CHARS+1 characters long. This parameter can be <see langword="null"/>.
        /// </param>
        /// <param name="componentCode">
        /// A GUID of the component code. This string buffer must be 39 characters long. The first 38 characters are for the GUID, and the last character is for the terminating <see langword="null"/> character. This parameter can be <see langword="null"/>.
        /// </param>
        /// <returns>
        /// the return code
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