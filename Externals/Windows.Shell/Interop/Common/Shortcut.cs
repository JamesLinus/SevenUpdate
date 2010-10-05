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
    using System.Text;

    /// <summary>
    /// </summary>
    public static class ShortcutInterop
    {
        #region Constants and Fields

        /// <summary>
        /// </summary>
        private const int MaxFeatureLength = 38;

        /// <summary>
        /// </summary>
        private const int MaxGuidLength = 38;

        /// <summary>
        /// </summary>
        private const int MaxPath = 260;

        /// <summary>
        /// </summary>
        private const int MaxPathLength = 1024;

        /// <summary>
        /// </summary>
        private const uint StgmRead = 0;

        #endregion

        #region Enums

        /// <summary>
        /// </summary>
        private enum InstallState
        {
            /// <summary>
            /// </summary>
            NotUsed = -7, 

            /// <summary>
            /// </summary>
            BadConfig = -6, 

            /// <summary>
            /// </summary>
            Incomplete = -5, 

            /// <summary>
            /// </summary>
            SourceAbsent = -4, 

            /// <summary>
            /// </summary>
            MoreData = -3, 

            /// <summary>
            /// </summary>
            InvalidArg = -2, 

            /// <summary>
            /// </summary>
            Unknown = -1, 

            /// <summary>
            /// </summary>
            Broken = 0, 

            /// <summary>
            /// </summary>
            Advertised = 1, 

            /// <summary>
            /// </summary>
            Removed = 1, 

            /// <summary>
            /// </summary>
            Absent = 2, 

            /// <summary>
            /// </summary>
            Local = 3, 

            /// <summary>
            /// </summary>
            Source = 4, 

            /// <summary>
            /// </summary>
            Default = 5
        }

        /// <summary>
        /// </summary>
        [Flags]
        private enum SlgpFlags
        {
            /// <summary>
            ///   Retrieves the standard short (8.3 format) file name
            /// </summary>
            SlgpShortPath = 0x1, 

            /// <summary>
            ///   Retrieves the Universal Naming Convention (UNC) path name of the file
            /// </summary>
            SlgpUncPriority = 0x2, 

            /// <summary>
            ///   Retrieves the raw path name. A raw path is something that might not exist and may include environment variables that need to be expanded
            /// </summary>
            SlgpRawPath = 0x4
        }

        /// <summary>
        /// </summary>
        [Flags]
        private enum SlrFlags
        {
            /// <summary>
            ///   Do not display a dialog box if the link cannot be resolved. When SLR_NO_UI is set,
            ///   the high-order word of fFlags can be set to a time-out value that specifies the
            ///   maximum amount of time to be spent resolving the link. The function returns if the
            ///   link cannot be resolved within the time-out duration. If the high-order word is set
            ///   to zero, the time-out duration will be set to the default value of 3,000 milliseconds
            ///   (3 seconds). To specify a value, set the high word of fFlags to the desired time-out
            ///   duration, in milliseconds.
            /// </summary>
            SlrNoUI = 0x1, 

            /// <summary>
            ///   Obsolete and no longer used
            /// </summary>
            SlrAnyMatch = 0x2, 

            /// <summary>
            ///   If the link object has changed, update its path and list of identifiers.
            ///   If SLR_UPDATE is set, you do not need to call IPersistFile::IsDirty to determine
            ///   whether or not the link object has changed.
            /// </summary>
            SlrUpdate = 0x4, 

            /// <summary>
            ///   Do not update the link information
            /// </summary>
            SlrNoUpdate = 0x8, 

            /// <summary>
            ///   Do not execute the search heuristics
            /// </summary>
            SlrNoSearch = 0x10, 

            /// <summary>
            ///   Do not use distributed link tracking
            /// </summary>
            SlrNoTrack = 0x20, 

            /// <summary>
            ///   Disable distributed link tracking. By default, distributed link tracking tracks
            ///   removable media across multiple devices based on the volume name. It also uses the
            ///   Universal Naming Convention (UNC) path to track remote file systems whose drive letter
            ///   has changed. Setting SLR_NOLINKINFO disables both types of tracking.
            /// </summary>
            SlrNoLinkInfo = 0x40, 

            /// <summary>
            ///   Call the Microsoft Windows Installer
            /// </summary>
            SlrInvokeMsi = 0x80
        }

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
            /// <param name="pClassID">
            /// </param>
            [PreserveSig]
            void GetClassID(out Guid pClassID);
        }

        /// <summary>
        /// </summary>
        [ComImport]
        [Guid("0000010b-0000-0000-C000-000000000046")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        private interface IPersistFile : IPersist
        {
            /// <summary>
            /// </summary>
            /// <param name="pClassID">
            /// </param>
            new void GetClassID(out Guid pClassID);

            /// <summary>
            /// </summary>
            /// <returns>
            /// </returns>
            [PreserveSig]
            int IsDirty();

            /// <summary>
            /// </summary>
            /// <param name="pszFileName">
            /// </param>
            /// <param name="dwMode">
            /// </param>
            [PreserveSig]
            void Load([In] [MarshalAs(UnmanagedType.LPWStr)] string pszFileName, uint dwMode);

            /// <summary>
            /// </summary>
            /// <param name="pszFileName">
            /// </param>
            /// <param name="fRemember">
            /// </param>
            [PreserveSig]
            void Save([In] [MarshalAs(UnmanagedType.LPWStr)] string pszFileName, [In] [MarshalAs(UnmanagedType.Bool)] bool fRemember);

            /// <summary>
            /// </summary>
            /// <param name="pszFileName">
            /// </param>
            [PreserveSig]
            void SaveCompleted([In] [MarshalAs(UnmanagedType.LPWStr)] string pszFileName);

            /// <summary>
            /// </summary>
            /// <param name="ppszFileName">
            /// </param>
            [PreserveSig]
            void GetCurFile([In] [MarshalAs(UnmanagedType.LPWStr)] string ppszFileName);
        }

        /// <summary>
        /// The IShellLink interface allows Shell links to be created, modified, and resolved
        /// </summary>
        [ComImport]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        [Guid("000214F9-0000-0000-C000-000000000046")]
        private interface IShellLinkW
        {
            /// <summary>
            /// Retrieves the path and file name of a Shell link object
            /// </summary>
            /// <param name="pszFile">
            /// </param>
            /// <param name="cchMaxPath">
            /// </param>
            /// <param name="pfd">
            /// </param>
            /// <param name="fFlags">
            /// </param>
            void GetPath([Out] [MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszFile, int cchMaxPath, out WIN32FindDataw pfd, SlgpFlags fFlags);

            /// <summary>
            /// Retrieves the list of item identifiers for a Shell link object
            /// </summary>
            /// <param name="ppidl">
            /// </param>
            void GetIDList(out IntPtr ppidl);

            /// <summary>
            /// Sets the pointer to an item identifier list (PIDL) for a Shell link object.
            /// </summary>
            /// <param name="pidl">
            /// </param>
            void SetIDList(IntPtr pidl);

            /// <summary>
            /// Retrieves the description string for a Shell link object
            /// </summary>
            /// <param name="pszName">
            /// </param>
            /// <param name="cchMaxName">
            /// </param>
            void GetDescription([Out] [MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszName, int cchMaxName);

            /// <summary>
            /// Sets the description for a Shell link object. The description can be any application-defined string
            /// </summary>
            /// <param name="pszName">
            /// </param>
            void SetDescription([MarshalAs(UnmanagedType.LPWStr)] string pszName);

            /// <summary>
            /// Retrieves the name of the working directory for a Shell link object
            /// </summary>
            /// <param name="pszDir">
            /// </param>
            /// <param name="cchMaxPath">
            /// </param>
            void GetWorkingDirectory([Out] [MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszDir, int cchMaxPath);

            /// <summary>
            /// Sets the name of the working directory for a Shell link object
            /// </summary>
            /// <param name="pszDir">
            /// </param>
            void SetWorkingDirectory([MarshalAs(UnmanagedType.LPWStr)] string pszDir);

            /// <summary>
            /// Retrieves the command-line arguments associated with a Shell link object
            /// </summary>
            /// <param name="pszArgs">
            /// </param>
            /// <param name="cchMaxPath">
            /// </param>
            void GetArguments([Out] [MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszArgs, int cchMaxPath);

            /// <summary>
            /// Sets the command-line arguments for a Shell link object
            /// </summary>
            /// <param name="pszArgs">
            /// </param>
            void SetArguments([MarshalAs(UnmanagedType.LPWStr)] string pszArgs);

            /// <summary>
            /// Retrieves the hot key for a Shell link object
            /// </summary>
            /// <param name="pwHotkey">
            /// </param>
            void GetHotkey(out short pwHotkey);

            /// <summary>
            /// Sets a hot key for a Shell link object
            /// </summary>
            /// <param name="wHotkey">
            /// </param>
            void SetHotkey(short wHotkey);

            /// <summary>
            /// Retrieves the show command for a Shell link object
            /// </summary>
            /// <param name="piShowCmd">
            /// </param>
            void GetShowCmd(out int piShowCmd);

            /// <summary>
            /// Sets the show command for a Shell link object. The show command sets the initial show state of the window.
            /// </summary>
            /// <param name="iShowCmd">
            /// </param>
            void SetShowCmd(int iShowCmd);

            /// <summary>
            /// Retrieves the location (path and index) of the icon for a Shell link object
            /// </summary>
            /// <param name="pszIconPath">
            /// </param>
            /// <param name="cchIconPath">
            /// </param>
            /// <param name="piIcon">
            /// </param>
            void GetIconLocation([Out] [MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszIconPath, int cchIconPath, out int piIcon);

            /// <summary>
            /// Sets the location (path and index) of the icon for a Shell link object
            /// </summary>
            /// <param name="pszIconPath">
            /// </param>
            /// <param name="iIcon">
            /// </param>
            void SetIconLocation([MarshalAs(UnmanagedType.LPWStr)] string pszIconPath, int iIcon);

            /// <summary>
            /// Sets the relative path to the Shell link object
            /// </summary>
            /// <param name="pszPathRel">
            /// </param>
            /// <param name="dwReserved">
            /// </param>
            void SetRelativePath([MarshalAs(UnmanagedType.LPWStr)] string pszPathRel, int dwReserved);

            /// <summary>
            /// Attempts to find the target of a Shell link, even if it has been moved or renamed
            /// </summary>
            /// <param name="hwnd">
            /// </param>
            /// <param name="fFlags">
            /// </param>
            void Resolve(IntPtr hwnd, SlrFlags fFlags);

            /// <summary>
            /// Sets the path and file name of a Shell link object
            /// </summary>
            /// <param name="pszFile">
            /// </param>
            void SetPath([MarshalAs(UnmanagedType.LPWStr)] string pszFile);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// </summary>
        /// <param name="file">
        /// </param>
        /// <returns>
        /// </returns>
        public static string ResolveMsiShortcut(string file)
        {
            var product = new StringBuilder(MaxGuidLength + 1);
            var feature = new StringBuilder(MaxFeatureLength + 1);
            var component = new StringBuilder(MaxGuidLength + 1);

            MsiGetShortcutTarget(file, product, feature, component);

            var pathLength = MaxPathLength;
            var path = new StringBuilder(pathLength);

            var installState = MsiGetComponentPath(product.ToString(), component.ToString(), path, ref pathLength);
            return installState == InstallState.Local ? path.ToString() : null;
        }

        /// <summary>
        /// </summary>
        /// <param name="filename">
        /// </param>
        /// <returns>
        /// </returns>
        public static Shortcut ResolveShortcut(string filename)
        {
            var link = new ShellLink();
            ((IPersistFile)link).Load(filename, StgmRead);

            // TODO: if I can get hold of the hwnd call resolve first. This handles moved and renamed files.  
            // ((IShellLinkW)link).Resolve(hwnd, 0) 
            var sb = new StringBuilder(MaxPath);
            var shortcut = new Shortcut { Target = ResolveMsiShortcut(filename), Name = Path.GetFileNameWithoutExtension(filename) };

            if (shortcut.Target == null)
            {
                WIN32FindDataw data;
                ((IShellLinkW)link).GetPath(sb, sb.Capacity, out data, 0);
                shortcut.Target = sb.ToString();
            }

            ((IShellLinkW)link).GetArguments(sb, sb.Capacity);
            shortcut.Arguments = sb.ToString();

            ((IShellLinkW)link).GetDescription(sb, sb.Capacity);
            shortcut.Description = sb.ToString();

            int piIcon;
            ((IShellLinkW)link).GetIconLocation(sb, sb.Capacity, out piIcon);
            var icon = sb.ToString();
            if (String.IsNullOrWhiteSpace(icon))
            {
                shortcut.Icon = shortcut.Target + @"," + piIcon;
            }
            else
            {
                shortcut.Icon = sb.ToString();
            }

            shortcut.Location = filename;

            return shortcut;
        }

        #endregion

        #region Methods

        /// <summary>
        /// </summary>
        /// <param name="hwndOwner">
        /// </param>
        /// <param name="nFolder">
        /// </param>
        /// <param name="hToken">
        /// </param>
        /// <param name="dwFlags">
        /// </param>
        /// <param name="lpszPath">
        /// </param>
        /// <returns>
        /// </returns>
        [DllImport("shfolder.dll", CharSet = CharSet.Auto)]
        internal static extern int SHGetFolderPath(IntPtr hwndOwner, int nFolder, IntPtr hToken, int dwFlags, StringBuilder lpszPath);

        /// <summary>
        /// </summary>
        /// <param name="productCode">
        /// </param>
        /// <param name="componentCode">
        /// </param>
        /// <param name="componentPath">
        /// </param>
        /// <param name="componentPathBufferSize">
        /// </param>
        /// <returns>
        /// </returns>
        [DllImport("msi.dll", CharSet = CharSet.Auto)]
        private static extern InstallState MsiGetComponentPath(string productCode, string componentCode, StringBuilder componentPath, ref int componentPathBufferSize);

        /// <summary>
        /// </summary>
        /// <param name="targetFile">
        /// </param>
        /// <param name="productCode">
        /// </param>
        /// <param name="featureID">
        /// </param>
        /// <param name="componentCode">
        /// </param>
        /// <returns>
        /// </returns>
        [DllImport("msi.dll", CharSet = CharSet.Auto)]
        private static extern int MsiGetShortcutTarget(string targetFile, StringBuilder productCode, StringBuilder featureID, StringBuilder componentCode);

        #endregion

        /// <summary>
        /// </summary>
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        private struct WIN32FindDataw
        {
            /// <summary>
            /// </summary>
            public readonly uint DWFileAttributes;

            /// <summary>
            /// </summary>
            public readonly long FTCreationTime;

            /// <summary>
            /// </summary>
            public readonly long FTLastAccessTime;

            /// <summary>
            /// </summary>
            public readonly long FTLastWriteTime;

            /// <summary>
            /// </summary>
            public readonly uint NFileSizeHigh;

            /// <summary>
            /// </summary>
            public readonly uint NFileSizeLow;

            /// <summary>
            /// </summary>
            public readonly uint DWReserved0;

            /// <summary>
            /// </summary>
            public readonly uint DWReserved1;

            /// <summary>
            /// </summary>
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public readonly string CFileName;

            /// <summary>
            /// </summary>
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 14)]
            public readonly string CAlternateFileName;

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
            public static bool operator ==(WIN32FindDataw x, WIN32FindDataw y)
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
            public static bool operator !=(WIN32FindDataw x, WIN32FindDataw y)
            {
                throw new NotImplementedException();
            }
        }

        // CLSID_ShellLink from ShlGuid.h 

        /// <summary>
        /// </summary>
        [ComImport]
        [Guid("00021401-0000-0000-C000-000000000046")]
        private class ShellLink
        {
        }
    }

    /// <summary>
    /// </summary>
    public class Shortcut
    {
        #region Properties

        /// <summary>
        /// </summary>
        public string Arguments { get; set; }

        /// <summary>
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// </summary>
        public string Icon { get; set; }

        /// <summary>
        /// </summary>
        public string Location { get; set; }

        /// <summary>
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// </summary>
        public string Target { get; set; }

        #endregion
    }
}