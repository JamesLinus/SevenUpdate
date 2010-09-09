﻿#region

using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

#endregion

namespace Microsoft.Windows.Shell
{
    public class ShortcutInterop
    {
        #region InstallState enum

        private enum InstallState
        {
            NotUsed = -7,
            BadConfig = -6,
            Incomplete = -5,
            SourceAbsent = -4,
            MoreData = -3,
            InvalidArg = -2,
            Unknown = -1,
            Broken = 0,
            Advertised = 1,
            Removed = 1,
            Absent = 2,
            Local = 3,
            Source = 4,
            Default = 5
        }

        #endregion

        private const uint StgmRead = 0;
        private const int MaxPath = 260;

        private const int MaxFeatureLength = 38;
        private const int MaxGuidLength = 38;
        private const int MaxPathLength = 1024;

        [DllImport("msi.dll", CharSet = CharSet.Auto)]
        private static extern int MsiGetShortcutTarget(string targetFile, StringBuilder productCode, StringBuilder featureID, StringBuilder componentCode);


        [DllImport("msi.dll", CharSet = CharSet.Auto)]
        private static extern InstallState MsiGetComponentPath(string productCode, string componentCode, StringBuilder componentPath, ref int componentPathBufferSize);


        [DllImport("shfolder.dll", CharSet = CharSet.Auto)]
        internal static extern int SHGetFolderPath(IntPtr hwndOwner, int nFolder, IntPtr hToken, int dwFlags, StringBuilder lpszPath);

        #region Nested type: IPersist

        [ComImport, Guid("0000010c-0000-0000-c000-000000000046"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IPersist
        {
            [PreserveSig]
            void GetClassID(out Guid pClassID);
        }

        #endregion

        #region Nested type: IPersistFile

        [ComImport, Guid("0000010b-0000-0000-C000-000000000046"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IPersistFile : IPersist
        {
            new void GetClassID(out Guid pClassID);

            [PreserveSig]
            int IsDirty();

            [PreserveSig]
            void Load([In, MarshalAs(UnmanagedType.LPWStr)] string pszFileName, uint dwMode);

            [PreserveSig]
            void Save([In, MarshalAs(UnmanagedType.LPWStr)] string pszFileName, [In, MarshalAs(UnmanagedType.Bool)] bool fRemember);

            [PreserveSig]
            void SaveCompleted([In, MarshalAs(UnmanagedType.LPWStr)] string pszFileName);

            [PreserveSig]
            void GetCurFile([In, MarshalAs(UnmanagedType.LPWStr)] string ppszFileName);
        }

        #endregion

        #region Nested type: IShellLinkW

        /// <summary>
        ///   The IShellLink interface allows Shell links to be created, modified, and resolved
        /// </summary>
        [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("000214F9-0000-0000-C000-000000000046")]
        private interface IShellLinkW
        {
            /// <summary>
            ///   Retrieves the path and file name of a Shell link object
            /// </summary>
            void GetPath([Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszFile, int cchMaxPath, out WIN32_FIND_DATAW pfd, SlgpFlags fFlags);

            /// <summary>
            ///   Retrieves the list of item identifiers for a Shell link object
            /// </summary>
            void GetIDList(out IntPtr ppidl);

            /// <summary>
            ///   Sets the pointer to an item identifier list (PIDL) for a Shell link object.
            /// </summary>
            void SetIDList(IntPtr pidl);

            /// <summary>
            ///   Retrieves the description string for a Shell link object
            /// </summary>
            void GetDescription([Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszName, int cchMaxName);

            /// <summary>
            ///   Sets the description for a Shell link object. The description can be any application-defined string
            /// </summary>
            void SetDescription([MarshalAs(UnmanagedType.LPWStr)] string pszName);

            /// <summary>
            ///   Retrieves the name of the working directory for a Shell link object
            /// </summary>
            void GetWorkingDirectory([Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszDir, int cchMaxPath);

            /// <summary>
            ///   Sets the name of the working directory for a Shell link object
            /// </summary>
            void SetWorkingDirectory([MarshalAs(UnmanagedType.LPWStr)] string pszDir);

            /// <summary>
            ///   Retrieves the command-line arguments associated with a Shell link object
            /// </summary>
            void GetArguments([Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszArgs, int cchMaxPath);

            /// <summary>
            ///   Sets the command-line arguments for a Shell link object
            /// </summary>
            void SetArguments([MarshalAs(UnmanagedType.LPWStr)] string pszArgs);

            /// <summary>
            ///   Retrieves the hot key for a Shell link object
            /// </summary>
            void GetHotkey(out short pwHotkey);

            /// <summary>
            ///   Sets a hot key for a Shell link object
            /// </summary>
            void SetHotkey(short wHotkey);

            /// <summary>
            ///   Retrieves the show command for a Shell link object
            /// </summary>
            void GetShowCmd(out int piShowCmd);

            /// <summary>
            ///   Sets the show command for a Shell link object. The show command sets the initial show state of the window.
            /// </summary>
            void SetShowCmd(int iShowCmd);

            /// <summary>
            ///   Retrieves the location (path and index) of the icon for a Shell link object
            /// </summary>
            void GetIconLocation([Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszIconPath, int cchIconPath, out int piIcon);

            /// <summary>
            ///   Sets the location (path and index) of the icon for a Shell link object
            /// </summary>
            void SetIconLocation([MarshalAs(UnmanagedType.LPWStr)] string pszIconPath, int iIcon);

            /// <summary>
            ///   Sets the relative path to the Shell link object
            /// </summary>
            void SetRelativePath([MarshalAs(UnmanagedType.LPWStr)] string pszPathRel, int dwReserved);

            /// <summary>
            ///   Attempts to find the target of a Shell link, even if it has been moved or renamed
            /// </summary>
            void Resolve(IntPtr hwnd, SlrFlags fFlags);

            /// <summary>
            ///   Sets the path and file name of a Shell link object
            /// </summary>
            void SetPath([MarshalAs(UnmanagedType.LPWStr)] string pszFile);
        }

        #endregion

        #region Nested type: SLGP_FLAGS

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

        #endregion

        #region Nested type: SLR_FLAGS

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

        // CLSID_ShellLink from ShlGuid.h 

        #region Nested type: ShellLink

        [ComImport, Guid("00021401-0000-0000-C000-000000000046")]
        public class ShellLink
        {
        }

        public class Shortcut
        {
            public string Name { get; set; }
            public string Location { get; set; }
            public string Arguments { get; set; }
            public string Description { get; set; }
            public string Icon { get; set; }
            public string Target { get; set; }
        }

        #endregion

        #region Nested type: WIN32_FIND_DATAW

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        private struct WIN32_FIND_DATAW
        {
            public uint dwFileAttributes;
            public long ftCreationTime;
            public long ftLastAccessTime;
            public long ftLastWriteTime;
            public uint nFileSizeHigh;
            public uint nFileSizeLow;
            public uint dwReserved0;
            public uint dwReserved1;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string cFileName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 14)]
            public string cAlternateFileName;
        }

        #endregion

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
                WIN32_FIND_DATAW data;
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
                shortcut.Icon = shortcut.Target + @"," + piIcon;
            else
                shortcut.Icon = sb.ToString();

            shortcut.Location = filename;


            return shortcut;
        }

        public static string ResolveMsiShortcut(string file)
        {
            var product = new StringBuilder(MaxGuidLength + 1);
            var feature = new StringBuilder(MaxFeatureLength + 1);
            var component = new StringBuilder(MaxGuidLength + 1);

            MsiGetShortcutTarget(file, product, feature, component);

            int pathLength = MaxPathLength;
            var path = new StringBuilder(pathLength);

            var installState = MsiGetComponentPath(product.ToString(), component.ToString(), path, ref pathLength);
            return installState == InstallState.Local ? path.ToString() : null;
        }
    }
}