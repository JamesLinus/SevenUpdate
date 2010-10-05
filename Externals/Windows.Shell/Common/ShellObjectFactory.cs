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
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Runtime.InteropServices;

    using Microsoft.Windows.Internal;

    /// <summary>
    /// </summary>
    internal static class ShellObjectFactory
    {
        #region Constants and Fields

        /// <summary>
        /// </summary>
        internal const string StrParsePreferFolderBrowsing = "Parse Prefer Folder Browsing";

        #endregion

        #region Methods

        /// <summary>
        /// Creates a ShellObject given a native IShellItem interface
        /// </summary>
        /// <param name="nativeShellItem">
        /// </param>
        /// <returns>
        /// A newly constructed ShellObject object
        /// </returns>
        [SuppressMessage("Microsoft.Globalization", "CA1304:SpecifyCultureInfo", MessageId = "System.String.ToLower", 
            Justification = "We are not currently handling globalization or localization")]
        internal static ShellObject Create(IShellItem nativeShellItem)
        {
            // Sanity check
            Debug.Assert(nativeShellItem != null, "nativeShellItem should not be null");

            // Need to make sure we're running on Vista or higher
            if (!CoreHelpers.RunningOnVista)
            {
                throw new PlatformNotSupportedException("Shell Object creation requires Windows Vista or higher operating system.");
            }

            // A lot of APIs need IShellItem2, so just keep a copy of it here
            var nativeShellItem2 = nativeShellItem as IShellItem2;

            // Get the System.ItemType property
            var itemType = ShellHelper.GetItemType(nativeShellItem2);

            if (!string.IsNullOrEmpty(itemType))
            {
                itemType = itemType.ToLower(CultureInfo.CurrentCulture);
            }

            // Get some IShellItem attributes
            ShellNativeMethods.SFGAOs sfgao;
            nativeShellItem2.GetAttributes(ShellNativeMethods.SFGAOs.SfgaoFilesystem | ShellNativeMethods.SFGAOs.SfgaoFolder, out sfgao);

            // Is this item a FileSystem item?
            var isFileSystem = (sfgao & ShellNativeMethods.SFGAOs.SfgaoFilesystem) != 0;

            // Is this item a Folder?
            var isFolder = (sfgao & ShellNativeMethods.SFGAOs.SfgaoFolder) != 0;

            // Shell Library

            // For KnownFolders

            // Create the right type of ShellObject based on the above information 

            // 1. First check if this is a Shell Link
            if (itemType == ".lnk")
            {
                return new ShellLink(nativeShellItem2);
            }

            // 2. Check if this is a container or a single item (entity)
            if (isFolder)
            {
                // 3. If this is a folder, check for types: Shell Library, Shell Folder or Search Container
                ShellLibrary shellLibrary;
                if ((itemType == ".library-ms") && (shellLibrary = ShellLibrary.FromShellItem(nativeShellItem2, true)) != null)
                {
                    return shellLibrary; // we already created this above while checking for Library
                }

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
            return isFileSystem ? (ShellObject)new ShellFile(nativeShellItem2) : new ShellNonFileSystemItem(nativeShellItem2);
        }

        /// <summary>
        /// Creates a ShellObject given a parsing name
        /// </summary>
        /// <param name="parsingName">
        /// </param>
        /// <returns>
        /// A newly constructed ShellObject object
        /// </returns>
        internal static ShellObject Create(string parsingName)
        {
            if (string.IsNullOrEmpty(parsingName))
            {
                throw new ArgumentNullException("parsingName");
            }

            // Create a native shellitem from our path
            IShellItem2 nativeShellItem;
            var guid = new Guid(ShellIidGuid.IShellItem2);
            var retCode = ShellNativeMethods.SHCreateItemFromParsingName(parsingName, IntPtr.Zero, ref guid, out nativeShellItem);

            if (CoreErrorHelper.Succeeded(retCode))
            {
                return Create(nativeShellItem);
            }

            throw new ExternalException("Unable to Create Shell Item.", Marshal.GetExceptionForHR(retCode));
        }

        /// <summary>
        /// Constructs a new Shell object from IDList pointer
        /// </summary>
        /// <param name="idListPtr">
        /// </param>
        /// <returns>
        /// </returns>
        internal static ShellObject Create(IntPtr idListPtr)
        {
            // Throw exception if not running on Win7 or newer.
            CoreHelpers.ThrowIfNotVista();

            var guid = new Guid(ShellIidGuid.IShellItem2);
            IShellItem2 nativeShellItem;
            var retCode = ShellNativeMethods.SHCreateItemFromIDList(idListPtr, ref guid, out nativeShellItem);
            return CoreErrorHelper.Succeeded(retCode) ? Create(nativeShellItem) : null;
        }

        /// <summary>
        /// Constructs a new Shell object from IDList pointer
        /// </summary>
        /// <param name="idListPtr">
        /// </param>
        /// <param name="parent">
        /// </param>
        /// <returns>
        /// </returns>
        internal static ShellObject Create(IntPtr idListPtr, ShellContainer parent)
        {
            IShellItem nativeShellItem;

            var retCode = ShellNativeMethods.SHCreateShellItem(IntPtr.Zero, parent.NativeShellFolder, idListPtr, out nativeShellItem);

            return CoreErrorHelper.Succeeded(retCode) ? Create(nativeShellItem) : null;
        }

        /// <summary>
        /// </summary>
        /// <param name="nativeShellItem">
        /// </param>
        /// <param name="isVirtual">
        /// </param>
        /// <returns>
        /// </returns>
        private static IKnownFolderNative GetNativeKnownFolder(IShellItem nativeShellItem, out bool isVirtual)
        {
            var pidl = IntPtr.Zero;

            try
            {
                // Get the PIDL for the ShellItem
                pidl = ShellHelper.PidlFromShellItem(nativeShellItem);

                if (pidl == IntPtr.Zero)
                {
                    isVirtual = false;
                    return null;
                }

                var knownFolderNative = KnownFolderHelper.FromPidl(pidl);

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

        #endregion
    }
}