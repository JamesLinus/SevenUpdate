// Copyright (c) Microsoft Corporation.  All rights reserved.

#region

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using Microsoft.Windows.Internal;

#endregion

namespace Microsoft.Windows.Shell
{
    internal static class ShellObjectFactory
    {
        internal const string STR_PARSE_PREFER_FOLDER_BROWSING = "Parse Prefer Folder Browsing";

        /// <summary>
        ///   Creates a ShellObject given a native IShellItem interface
        /// </summary>
        /// <param name = "nativeShellItem"></param>
        /// <returns>A newly constructed ShellObject object</returns>
        [SuppressMessage("Microsoft.Globalization", "CA1304:SpecifyCultureInfo", MessageId = "System.String.ToLower", Justification = "We are not currently handling globalization or localization")]
        internal static ShellObject Create(IShellItem nativeShellItem)
        {
            // Sanity check
            Debug.Assert(nativeShellItem != null, "nativeShellItem should not be null");

            // Need to make sure we're running on Vista or higher
            if (!CoreHelpers.RunningOnVista)
                throw new PlatformNotSupportedException("Shell Object creation requires Windows Vista or higher operating system.");

            // A lot of APIs need IShellItem2, so just keep a copy of it here
            var nativeShellItem2 = nativeShellItem as IShellItem2;

            // Get the System.ItemType property
            string itemType = ShellHelper.GetItemType(nativeShellItem2);

            if (!string.IsNullOrEmpty(itemType))
                itemType = itemType.ToLower();

            // Get some IShellItem attributes
            ShellNativeMethods.SFGAO sfgao;
            nativeShellItem2.GetAttributes(ShellNativeMethods.SFGAO.SFGAO_FILESYSTEM | ShellNativeMethods.SFGAO.SFGAO_FOLDER, out sfgao);

            // Is this item a FileSystem item?
            bool isFileSystem = (sfgao & ShellNativeMethods.SFGAO.SFGAO_FILESYSTEM) != 0;

            // Is this item a Folder?
            bool isFolder = (sfgao & ShellNativeMethods.SFGAO.SFGAO_FOLDER) != 0;

            // Shell Library

            // For KnownFolders

            // Create the right type of ShellObject based on the above information 

            // 1. First check if this is a Shell Link
            if (itemType == ".lnk")
                return new ShellLink(nativeShellItem2);
            // 2. Check if this is a container or a single item (entity)
            if (isFolder)
            {
                // 3. If this is a folder, check for types: Shell Library, Shell Folder or Search Container
                ShellLibrary shellLibrary;
                if ((itemType == ".library-ms") && (shellLibrary = ShellLibrary.FromShellItem(nativeShellItem2, true)) != null)
                    return shellLibrary; // we already created this above while checking for Library
                // 4. It's a ShellFolder
                bool isKnownFolderVirtual;
                if (isFileSystem)
                {
                    // 5. Is it a (File-System / Non-Virtual) Known Folder
                    if ((GetNativeKnownFolder(nativeShellItem2, out isKnownFolderVirtual) != null) && !isKnownFolderVirtual)
                    {
                        var kf = new FileSystemKnownFolder(nativeShellItem2);
                        return kf;
                    }
                    return new ShellFileSystemFolder(nativeShellItem2);
                }
                // 5. Is it a (Non File-System / Virtual) Known Folder
                if ((GetNativeKnownFolder(nativeShellItem2, out isKnownFolderVirtual) != null) && isKnownFolderVirtual)
                {
                    var kf = new NonFileSystemKnownFolder(nativeShellItem2);
                    return kf;
                }
                return new ShellNonFileSystemFolder(nativeShellItem2);
            }
            // 6. If this is an entity (single item), check if its filesystem or not
            return isFileSystem ? (ShellObject) new ShellFile(nativeShellItem2) : new ShellNonFileSystemItem(nativeShellItem2);
        }

        private static IKnownFolderNative GetNativeKnownFolder(IShellItem nativeShellItem, out bool isVirtual)
        {
            IntPtr pidl = IntPtr.Zero;

            try
            {
                // Get the PIDL for the ShellItem
                pidl = ShellHelper.PidlFromShellItem(nativeShellItem);

                if (pidl == IntPtr.Zero)
                {
                    isVirtual = false;
                    return null;
                }

                IKnownFolderNative knownFolderNative = KnownFolderHelper.FromPIDL(pidl);

                if (knownFolderNative != null)
                {
                    // If we have a valid IKnownFolder, try to get its category
                    KnownFoldersSafeNativeMethods.NativeFolderDefinition nativeFolderDefinition;
                    knownFolderNative.GetFolderDefinition(out nativeFolderDefinition);

                    // Get the category type and see if it's virtual
                    isVirtual = nativeFolderDefinition.category == FolderCategory.Virtual;

                    return knownFolderNative;
                }
                else
                {
                    // KnownFolderHelper.FromPIDL could not create a valid KnownFolder from the given PIDL.
                    // Return null to indicate the given IShellItem is not a KnownFolder. Also set our out parameter
                    // to default value.
                    isVirtual = false;
                    return null;
                }
            }
            finally
            {
                ShellNativeMethods.ILFree(pidl);
            }
        }

        /// <summary>
        ///   Creates a ShellObject given a parsing name
        /// </summary>
        /// <param name = "parsingName"></param>
        /// <returns>A newly constructed ShellObject object</returns>
        internal static ShellObject Create(string parsingName)
        {
            if (string.IsNullOrEmpty(parsingName))
                throw new ArgumentNullException("parsingName");

            // Create a native shellitem from our path
            IShellItem2 nativeShellItem;
            var guid = new Guid(ShellIIDGuid.IShellItem2);
            int retCode = ShellNativeMethods.SHCreateItemFromParsingName(parsingName, IntPtr.Zero, ref guid, out nativeShellItem);

            if (CoreErrorHelper.Succeeded(retCode))
                return Create(nativeShellItem);
            throw new ExternalException("Unable to Create Shell Item.", Marshal.GetExceptionForHR(retCode));
        }

        /// <summary>
        ///   Constructs a new Shell object from IDList pointer
        /// </summary>
        /// <param name = "idListPtr"></param>
        /// <returns></returns>
        internal static ShellObject Create(IntPtr idListPtr)
        {
            // Throw exception if not running on Win7 or newer.
            CoreHelpers.ThrowIfNotVista();

            var guid = new Guid(ShellIIDGuid.IShellItem2);
            IShellItem2 nativeShellItem;
            int retCode = ShellNativeMethods.SHCreateItemFromIDList(idListPtr, ref guid, out nativeShellItem);
            return CoreErrorHelper.Succeeded(retCode) ? Create(nativeShellItem) : null;
        }

        /// <summary>
        ///   Constructs a new Shell object from IDList pointer
        /// </summary>
        /// <param name = "idListPtr"></param>
        /// <param name = "parent"></param>
        /// <returns></returns>
        internal static ShellObject Create(IntPtr idListPtr, ShellContainer parent)
        {
            IShellItem nativeShellItem;

            int retCode = ShellNativeMethods.SHCreateShellItem(IntPtr.Zero, parent.NativeShellFolder, idListPtr, out nativeShellItem);

            return CoreErrorHelper.Succeeded(retCode) ? Create(nativeShellItem) : null;
        }
    }
}