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
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Threading;

    using Microsoft.Windows.Internal;

    /// <summary>
    /// A Shell Library in the Shell Namespace
    /// </summary>
    [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", 
        Justification = "This will complicate the class hierarchy and naming convention used in the Shell area")]
    public sealed class ShellLibrary : ShellContainer, IList<ShellFileSystemFolder>
    {
        #region Constants and Fields

        /// <summary>
        /// </summary>
        internal const string FileExtension = ".library-ms";

        /// <summary>
        /// </summary>
        private static readonly Guid[] FolderTypesGuids = {
                                                              new Guid(ShellKfidGuid.GenericLibrary), new Guid(ShellKfidGuid.DocumentsLibrary), 
                                                              new Guid(ShellKfidGuid.MusicLibrary), new Guid(ShellKfidGuid.PicturesLibrary), 
                                                              new Guid(ShellKfidGuid.VideosLibrary)
                                                          };

        /// <summary>
        /// </summary>
        private readonly IKnownFolder knownFolder;

        /// <summary>
        /// </summary>
        private INativeShellLibrary nativeShellLibrary;

        #endregion

        // Construct the ShellLibrary object from a native Shell Library
        #region Constructors and Destructors

        /// <summary>
        /// Creates a shell library in the Libraries Known Folder, 
        ///   using the given shell library name.
        /// </summary>
        /// <param name="libraryName">
        /// The name of this library
        /// </param>
        /// <param name="overwrite">
        /// Allow overwriting an existing library; if one exists with the same name
        /// </param>
        public ShellLibrary(string libraryName, bool overwrite)
        {
            CoreHelpers.ThrowIfNotWin7();

            if (String.IsNullOrEmpty(libraryName))
            {
                throw new ArgumentNullException("libraryName", "libraryName cannot be empty.");
            }

            this.Name = libraryName;
            var guid = new Guid(ShellKfidGuid.Libraries);

            var flags = overwrite ? ShellNativeMethods.LIBRARYSAVEFLAGS.LsfOverrideexisting : ShellNativeMethods.LIBRARYSAVEFLAGS.LsfFailifthere;

            this.nativeShellLibrary = (INativeShellLibrary)new ShellLibraryCOClass();
            this.nativeShellLibrary.SaveInKnownFolder(ref guid, libraryName, flags, out this.nativeShellItem);
        }

        /// <summary>
        /// Creates a shell library in a given Known Folder, 
        ///   using the given shell library name.
        /// </summary>
        /// <param name="libraryName">
        /// The name of this library
        /// </param>
        /// <param name="sourceKnownFolder">
        /// The known folder
        /// </param>
        /// <param name="overwrite">
        /// Override an existing library with the same name
        /// </param>
        public ShellLibrary(string libraryName, IKnownFolder sourceKnownFolder, bool overwrite)
        {
            CoreHelpers.ThrowIfNotWin7();
            if (String.IsNullOrEmpty(libraryName))
            {
                throw new ArgumentNullException("libraryName", "libraryName cannot be empty.");
            }

            this.knownFolder = sourceKnownFolder;

            this.Name = libraryName;
            var guid = this.knownFolder.FolderId;

            var flags = overwrite ? ShellNativeMethods.LIBRARYSAVEFLAGS.LsfOverrideexisting : ShellNativeMethods.LIBRARYSAVEFLAGS.LsfFailifthere;

            this.nativeShellLibrary = (INativeShellLibrary)new ShellLibraryCOClass();
            this.nativeShellLibrary.SaveInKnownFolder(ref guid, libraryName, flags, out this.nativeShellItem);
        }

        /// <summary>
        /// Creates a shell library in a given local folder, 
        ///   using the given shell library name.
        /// </summary>
        /// <param name="libraryName">
        /// The name of this library
        /// </param>
        /// <param name="folderPath">
        /// The path to the local folder
        /// </param>
        /// <param name="overwrite">
        /// Override an existing library with the same name
        /// </param>
        public ShellLibrary(string libraryName, string folderPath, bool overwrite)
        {
            CoreHelpers.ThrowIfNotWin7();

            if (String.IsNullOrEmpty(libraryName))
            {
                throw new ArgumentNullException("libraryName", "libraryName cannot be empty.");
            }

            if (!Directory.Exists(folderPath))
            {
                throw new DirectoryNotFoundException("Folder path not found.");
            }

            this.Name = libraryName;

            var flags = overwrite ? ShellNativeMethods.LIBRARYSAVEFLAGS.LsfOverrideexisting : ShellNativeMethods.LIBRARYSAVEFLAGS.LsfFailifthere;

            var guid = new Guid(ShellIidGuid.IShellItem);

            IShellItem shellItemIn;
            ShellNativeMethods.SHCreateItemFromParsingName(folderPath, IntPtr.Zero, ref guid, out shellItemIn);

            this.nativeShellLibrary = (INativeShellLibrary)new ShellLibraryCOClass();
            this.nativeShellLibrary.Save(shellItemIn, libraryName, flags, out this.nativeShellItem);
        }

        /// <summary>
        /// </summary>
        /// <param name="nativeShellLibrary">
        /// </param>
        private ShellLibrary(INativeShellLibrary nativeShellLibrary)
        {
            CoreHelpers.ThrowIfNotWin7();
            this.nativeShellLibrary = nativeShellLibrary;
        }

        /// <summary>
        /// Creates a shell library in the Libraries Known Folder, 
        ///   using the given IKnownFolder
        /// </summary>
        /// <param name="sourceKnownFolder">
        /// KnownFolder from which to create the new Shell Library
        /// </param>
        /// <param name="isReadOnly">
        /// If <B>true</B> , opens the library in read-only mode.
        /// </param>
        private ShellLibrary(IKnownFolder sourceKnownFolder, bool isReadOnly)
        {
            CoreHelpers.ThrowIfNotWin7();

            Debug.Assert(sourceKnownFolder != null);

            // Keep a reference locally
            this.knownFolder = sourceKnownFolder;

            this.nativeShellLibrary = (INativeShellLibrary)new ShellLibraryCOClass();

            var flags = isReadOnly ? ShellNativeMethods.STGMs.Read : ShellNativeMethods.STGMs.ReadWrite;

            // Get the IShellItem2
            this.nativeShellItem = ((ShellObject)sourceKnownFolder).NativeShellItem2;

            var guid = sourceKnownFolder.FolderId;

            // Load the library from the IShellItem2
            try
            {
                this.nativeShellLibrary.LoadLibraryFromKnownFolder(ref guid, flags);
            }
            catch (InvalidCastException)
            {
                throw new ArgumentException("The given known folder is not a valid library.", "sourceKnownFolder");
            }
            catch (NotImplementedException)
            {
                throw new ArgumentException("The given known folder is not a valid library.", "sourceKnownFolder");
            }
        }

        /// <summary>
        /// 
        ///   Release resources
        /// </summary>
        ~ShellLibrary()
        {
            this.Dispose(false);
        }

        #endregion

        #region Properties

        /// <summary>
        ///   Indicates whether this feature is supported on the current platform.
        /// </summary>
        public new static bool IsPlatformSupported
        {
            get
            {
                // We need Windows 7 onwards ...
                return CoreHelpers.RunningOnWin7;
            }
        }

        /// <summary>
        ///   Get a the known folder FOLDERID_Libraries
        /// </summary>
        public static IKnownFolder LibrariesKnownFolder
        {
            get
            {
                CoreHelpers.ThrowIfNotWin7();
                return KnownFolderHelper.FromKnownFolderId(new Guid(ShellKfidGuid.Libraries));
            }
        }

        /// <summary>
        ///   The count of the items in the list.
        /// </summary>
        public int Count
        {
            get
            {
                return this.ItemsList.Count;
            }
        }

        /// <summary>
        ///   By default, this folder is the first location 
        ///   added to the library. The default save folder 
        ///   is both the default folder where files can 
        ///   be saved, and also where the library XML 
        ///   file will be saved, if no other path is specified
        /// </summary>
        public string DefaultSaveFolder
        {
            get
            {
                var guid = new Guid(ShellIidGuid.IShellItem);

                IShellItem saveFolderItem;

                this.nativeShellLibrary.GetDefaultSaveFolder(ShellNativeMethods.DEFAULTSAVEFOLDERTYPE.DsftDetect, ref guid, out saveFolderItem);

                return ShellHelper.GetParsingName(saveFolderItem);
            }

            set
            {
                if (String.IsNullOrEmpty(value))
                {
                    throw new ArgumentNullException("value");
                }

                if (!Directory.Exists(value))
                {
                    throw new DirectoryNotFoundException("DefaultSaveFolder Path not found.");
                }

                var fullPath = new DirectoryInfo(value).FullName;

                var guid = new Guid(ShellIidGuid.IShellItem);
                IShellItem saveFolderItem;

                ShellNativeMethods.SHCreateItemFromParsingName(fullPath, IntPtr.Zero, ref guid, out saveFolderItem);

                this.nativeShellLibrary.SetDefaultSaveFolder(ShellNativeMethods.DEFAULTSAVEFOLDERTYPE.DsftDetect, saveFolderItem);

                this.nativeShellLibrary.Commit();
            }
        }

        /// <summary>
        ///   The Resource Reference to the icon.
        /// </summary>
        public IconReference IconResourceId
        {
            get
            {
                string iconRef;
                this.nativeShellLibrary.GetIcon(out iconRef);
                return new IconReference(iconRef);
            }

            set
            {
                this.nativeShellLibrary.SetIcon(value.ReferencePath);
                this.nativeShellLibrary.Commit();
            }
        }

        /// <summary>
        ///   Whether the library will be pinned to the 
        ///   Explorer Navigation Pane
        /// </summary>
        public bool IsPinnedToNavigationPane
        {
            get
            {
                ShellNativeMethods.LIBRARYOPTIONFLAGSs flags;

                this.nativeShellLibrary.GetOptions(out flags);

                return (flags & ShellNativeMethods.LIBRARYOPTIONFLAGSs.LofPinnedtonavpane) == ShellNativeMethods.LIBRARYOPTIONFLAGSs.LofPinnedtonavpane;
            }

            set
            {
                var flags = ShellNativeMethods.LIBRARYOPTIONFLAGSs.LofDefault;

                if (value)
                {
                    flags |= ShellNativeMethods.LIBRARYOPTIONFLAGSs.LofPinnedtonavpane;
                }
                else
                {
                    flags &= ~ShellNativeMethods.LIBRARYOPTIONFLAGSs.LofPinnedtonavpane;
                }

                this.nativeShellLibrary.SetOptions(ShellNativeMethods.LIBRARYOPTIONFLAGSs.LofPinnedtonavpane, flags);
                this.nativeShellLibrary.Commit();
            }
        }

        /// <summary>
        ///   Indicates whether this list is read-only or not.
        /// </summary>
        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        ///   One of predefined Library types
        /// </summary>
        /// <exception cref = "COMException">Will throw if no Library Type is set</exception>
        public LibraryFolderType LibraryType
        {
            get
            {
                Guid folderTypeGuid;
                this.nativeShellLibrary.GetFolderType(out folderTypeGuid);

                return GetFolderTypefromGuid(folderTypeGuid);
            }

            set
            {
                var guid = FolderTypesGuids[(int)value];
                this.nativeShellLibrary.SetFolderType(ref guid);
                this.nativeShellLibrary.Commit();
            }
        }

        /// <summary>
        ///   The Guid of the Library type
        /// </summary>
        /// <exception cref = "COMException">Will throw if no Library Type is set</exception>
        public Guid LibraryTypeId
        {
            get
            {
                Guid folderTypeGuid;
                this.nativeShellLibrary.GetFolderType(out folderTypeGuid);

                return folderTypeGuid;
            }
        }

        /// <summary>
        ///   The name of the library, every library must 
        ///   have a name
        /// </summary>
        /// <exception cref = "COMException">Will throw if no Icon is set</exception>
        public override string Name
        {
            get
            {
                if (base.Name == null && this.NativeShellItem != null)
                {
                    base.Name = Path.GetFileNameWithoutExtension(ShellHelper.GetParsingName(this.NativeShellItem));
                }

                return base.Name;
            }
        }

        /// <summary>
        /// </summary>
        internal override IShellItem NativeShellItem
        {
            get
            {
                return this.NativeShellItem2;
            }
        }

        /// <summary>
        /// </summary>
        internal override IShellItem2 NativeShellItem2
        {
            get
            {
                return this.nativeShellItem;
            }
        }

        /// <summary>
        /// </summary>
        private List<ShellFileSystemFolder> ItemsList
        {
            get
            {
                return this.GetFolders();
            }
        }

        #endregion

        #region Indexers

        /// <summary>
        ///   Retrieves the folder at the specified index
        /// </summary>
        /// <param name = "index">The index of the folder to retrieve.</param>
        /// <returns>A folder.</returns>
        public ShellFileSystemFolder this[int index]
        {
            get
            {
                return this.ItemsList[index];
            }

            set
            {
                // Index related options are not supported by IShellLibrary
                // doesn't support them.
                throw new NotImplementedException();
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Load the library using a number of options
        /// </summary>
        /// <param name="libraryName">
        /// The name of the library
        /// </param>
        /// <param name="isReadOnly">
        /// If <B>true</B>, loads the library in read-only mode.
        /// </param>
        /// <returns>
        /// A ShellLibrary Object
        /// </returns>
        public static ShellLibrary Load(string libraryName, bool isReadOnly)
        {
            CoreHelpers.ThrowIfNotWin7();

            var kf = KnownFolders.Libraries;
            var librariesFolderPath = (kf != null) ? kf.Path : string.Empty;

            var guid = new Guid(ShellIidGuid.IShellItem);
            IShellItem nativeShellItem;
            var shellItemPath = Path.Combine(librariesFolderPath, libraryName + FileExtension);
            var hr = ShellNativeMethods.SHCreateItemFromParsingName(shellItemPath, IntPtr.Zero, ref guid, out nativeShellItem);

            if (!CoreErrorHelper.Succeeded(hr))
            {
                throw Marshal.GetExceptionForHR(hr);
            }

            var nativeShellLibrary = (INativeShellLibrary)new ShellLibraryCOClass();
            var flags = isReadOnly ? ShellNativeMethods.STGMs.Read : ShellNativeMethods.STGMs.ReadWrite;
            nativeShellLibrary.LoadLibraryFromItem(nativeShellItem, flags);

            var library = new ShellLibrary(nativeShellLibrary) { nativeShellItem = (IShellItem2)nativeShellItem, Name = libraryName };

            return library;
        }

        /// <summary>
        /// Load the library using a number of options
        /// </summary>
        /// <param name="libraryName">
        /// The name of the library.
        /// </param>
        /// <param name="folderPath">
        /// The path to the library.
        /// </param>
        /// <param name="isReadOnly">
        /// If <B>true</B>, opens the library in read-only mode.
        /// </param>
        /// <returns>
        /// A ShellLibrary Object
        /// </returns>
        public static ShellLibrary Load(string libraryName, string folderPath, bool isReadOnly)
        {
            CoreHelpers.ThrowIfNotWin7();

            // Create the shell item path
            var shellItemPath = Path.Combine(folderPath, libraryName + FileExtension);
            var item = ShellFile.FromFilePath(shellItemPath);

            var nativeShellItem = item.NativeShellItem;
            var nativeShellLibrary = (INativeShellLibrary)new ShellLibraryCOClass();
            var flags = isReadOnly ? ShellNativeMethods.STGMs.Read : ShellNativeMethods.STGMs.ReadWrite;
            nativeShellLibrary.LoadLibraryFromItem(nativeShellItem, flags);

            var library = new ShellLibrary(nativeShellLibrary) { nativeShellItem = (IShellItem2)nativeShellItem, Name = libraryName };

            return library;
        }

        /// <summary>
        /// Load the library using a number of options
        /// </summary>
        /// <param name="sourceKnownFolder">
        /// A known folder.
        /// </param>
        /// <param name="isReadOnly">
        /// If <B>true</B>, opens the library in read-only mode.
        /// </param>
        /// <returns>
        /// A ShellLibrary Object
        /// </returns>
        public static ShellLibrary Load(IKnownFolder sourceKnownFolder, bool isReadOnly)
        {
            CoreHelpers.ThrowIfNotWin7();
            return new ShellLibrary(sourceKnownFolder, isReadOnly);
        }

        /// <summary>
        /// Shows the library management dialog which enables users to mange the library folders and default save location.
        /// </summary>
        /// <param name="libraryName">
        /// The name of the library
        /// </param>
        /// <param name="folderPath">
        /// The path to the library.
        /// </param>
        /// <param name="windowHandle">
        /// The parent window,or IntPtr.Zero for no parent
        /// </param>
        /// <param name="title">
        /// A title for the library management dialog, or null to use the library name as the title
        /// </param>
        /// <param name="instruction">
        /// An optional help string to display for the library management dialog
        /// </param>
        /// <param name="allowAllLocations">
        /// If true, do not show warning dialogs about locations that cannot be indexed
        /// </param>
        /// <remarks>
        /// If the library is already open in read-write mode, the dialog will not save the changes.
        /// </remarks>
        public static void ShowManageLibraryUI(string libraryName, string folderPath, IntPtr windowHandle, string title, string instruction, bool allowAllLocations)
        {
            // this method is not safe for MTA consumption and will blow
            // Access Violations if called from an MTA thread so we wrap this
            // call up into a Worker thread that performs all operations in a
            // single threaded apartment
            using (var shellLibrary = Load(libraryName, folderPath, true)) ShowManageLibraryUI(shellLibrary, windowHandle, title, instruction, allowAllLocations);
        }

        /// <summary>
        /// Shows the library management dialog which enables users to mange the library folders and default save location.
        /// </summary>
        /// <param name="libraryName">
        /// The name of the library
        /// </param>
        /// <param name="windowHandle">
        /// The parent window,or IntPtr.Zero for no parent
        /// </param>
        /// <param name="title">
        /// A title for the library management dialog, or null to use the library name as the title
        /// </param>
        /// <param name="instruction">
        /// An optional help string to display for the library management dialog
        /// </param>
        /// <param name="allowAllLocations">
        /// If true, do not show warning dialogs about locations that cannot be indexed
        /// </param>
        /// <remarks>
        /// If the library is already open in read-write mode, the dialog will not save the changes.
        /// </remarks>
        public static void ShowManageLibraryUI(string libraryName, IntPtr windowHandle, string title, string instruction, bool allowAllLocations)
        {
            // this method is not safe for MTA consumption and will blow
            // Access Violations if called from an MTA thread so we wrap this
            // call up into a Worker thread that performs all operations in a
            // single threaded apartment
            using (var shellLibrary = Load(libraryName, true)) ShowManageLibraryUI(shellLibrary, windowHandle, title, instruction, allowAllLocations);
        }

        /// <summary>
        /// Shows the library management dialog which enables users to mange the library folders and default save location.
        /// </summary>
        /// <param name="sourceKnownFolder">
        /// A known folder.
        /// </param>
        /// <param name="windowHandle">
        /// The parent window,or IntPtr.Zero for no parent
        /// </param>
        /// <param name="title">
        /// A title for the library management dialog, or null to use the library name as the title
        /// </param>
        /// <param name="instruction">
        /// An optional help string to display for the library management dialog
        /// </param>
        /// <param name="allowAllLocations">
        /// If true, do not show warning dialogs about locations that cannot be indexed
        /// </param>
        /// <remarks>
        /// If the library is already open in read-write mode, the dialog will not save the changes.
        /// </remarks>
        public static void ShowManageLibraryUI(IKnownFolder sourceKnownFolder, IntPtr windowHandle, string title, string instruction, bool allowAllLocations)
        {
            // this method is not safe for MTA consumption and will blow
            // Access Violations if called from an MTA thread so we wrap this
            // call up into a Worker thread that performs all operations in a
            // single threaded apartment
            using (var shellLibrary = Load(sourceKnownFolder, true)) ShowManageLibraryUI(shellLibrary, windowHandle, title, instruction, allowAllLocations);
        }

        /// <summary>
        /// Add an existing folder to this library
        /// </summary>
        /// <param name="folderPath">
        /// The path to the folder to be added to the library.
        /// </param>
        public void Add(string folderPath)
        {
            if (!Directory.Exists(folderPath))
            {
                throw new DirectoryNotFoundException("Folder path not found.");
            }

            this.Add(ShellFileSystemFolder.FromFolderPath(folderPath));
        }

        /// <summary>
        /// Close the library, and release its associated file system resources
        /// </summary>
        public void Close()
        {
            this.Dispose();
        }

        /// <summary>
        /// Determines if an item with the specified path exists in the collection.
        /// </summary>
        /// <param name="fullPath">
        /// The path of the item.
        /// </param>
        /// <returns>
        /// <B>true</B> if the item exists in the collection.
        /// </returns>
        [SuppressMessage("Microsoft.Globalization", "CA1309:UseOrdinalStringComparison", MessageId = "System.String.Equals(System.String,System.StringComparison)", 
            Justification = "We are not currently handling globalization or localization")]
        public bool Contains(string fullPath)
        {
            if (String.IsNullOrEmpty(fullPath))
            {
                throw new ArgumentNullException("fullPath");
            }

            var list = this.ItemsList;

            return list.Any(folder => fullPath.Equals(folder.Path, StringComparison.InvariantCultureIgnoreCase));
        }

        /// <summary>
        /// Remove a folder or search connector
        /// </summary>
        /// <param name="folderPath">
        /// The path of the item to remove.
        /// </param>
        /// <returns>
        /// <B>true</B> if the item was removed.
        /// </returns>
        public bool Remove(string folderPath)
        {
            var item = ShellFileSystemFolder.FromFolderPath(folderPath);
            return Remove(item);
        }

        #endregion

        #region Implemented Interfaces

        #region ICollection<ShellFileSystemFolder>

        /// <summary>
        /// Add a new FileSystemFolder or SearchConnector
        /// </summary>
        /// <param name="item">
        /// The folder to add to the library.
        /// </param>
        public void Add(ShellFileSystemFolder item)
        {
            this.nativeShellLibrary.AddFolder(item.NativeShellItem);
            this.nativeShellLibrary.Commit();
        }

        /// <summary>
        /// Clear all items of this Library
        /// </summary>
        public void Clear()
        {
            var list = this.ItemsList;
            foreach (var folder in list)
            {
                this.nativeShellLibrary.RemoveFolder(folder.NativeShellItem);
            }

            this.nativeShellLibrary.Commit();
        }

        /// <summary>
        /// Determines if a folder exists in the collection.
        /// </summary>
        /// <param name="item">
        /// The folder.
        /// </param>
        /// <returns>
        /// <B>true</B>, if the folder exists in the collection.
        /// </returns>
        [SuppressMessage("Microsoft.Globalization", "CA1309:UseOrdinalStringComparison", MessageId = "System.String.Equals(System.String,System.StringComparison)", 
            Justification = "We are not currently handling globalization or localization")]
        public bool Contains(ShellFileSystemFolder item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }

            var list = this.ItemsList;

            return list.Any(folder => item.Path.Equals(folder.Path, StringComparison.InvariantCultureIgnoreCase));
        }

        /// <summary>
        /// Remove a folder or search connector
        /// </summary>
        /// <param name="item">
        /// The item to remove.
        /// </param>
        /// <returns>
        /// <B>true</B> if the item was removed.
        /// </returns>
        public bool Remove(ShellFileSystemFolder item)
        {
            try
            {
                this.nativeShellLibrary.RemoveFolder(item.NativeShellItem);
                this.nativeShellLibrary.Commit();
            }
            catch (COMException)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Copies the collection to an array.
        /// </summary>
        /// <param name="array">
        /// The array to copy to.
        /// </param>
        /// <param name="arrayIndex">
        /// The index in the array at which to start the copy.
        /// </param>
        void ICollection<ShellFileSystemFolder>.CopyTo(ShellFileSystemFolder[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IEnumerable

        /// <summary>
        /// Retrieves the collection enumerator.
        /// </summary>
        /// <returns>
        /// The enumerator.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.ItemsList.GetEnumerator();
        }

        #endregion

        #region IEnumerable<ShellFileSystemFolder>

        /// <summary>
        /// Retrieves the collection enumerator.
        /// </summary>
        /// <returns>
        /// The enumerator.
        /// </returns>
        public new IEnumerator<ShellFileSystemFolder> GetEnumerator()
        {
            return this.ItemsList.GetEnumerator();
        }

        #endregion

        #region IList<ShellFileSystemFolder>

        /// <summary>
        /// Searches for the specified FileSystemFolder and returns the zero-based index of the
        ///   first occurrence within Library list.
        /// </summary>
        /// <param name="item">
        /// The item to search for.
        /// </param>
        /// <returns>
        /// The index of the item in the collection, or -1 if the item does not exist.
        /// </returns>
        public int IndexOf(ShellFileSystemFolder item)
        {
            return this.ItemsList.IndexOf(item);
        }

        /// <summary>
        /// Inserts a FileSystemFolder at the specified index.
        /// </summary>
        /// <param name="index">
        /// The index to insert at.
        /// </param>
        /// <param name="item">
        /// The FileSystemFolder to insert.
        /// </param>
        void IList<ShellFileSystemFolder>.Insert(int index, ShellFileSystemFolder item)
        {
            // Index related options are not supported by IShellLibrary
            // doesn't support them.
            throw new NotImplementedException();
        }

        /// <summary>
        /// Removes an item at the specified index.
        /// </summary>
        /// <param name="index">
        /// The index to remove.
        /// </param>
        void IList<ShellFileSystemFolder>.RemoveAt(int index)
        {
            // Index related options are not supported by IShellLibrary
            // doesn't support them.
            throw new NotImplementedException();
        }

        #endregion

        #endregion

        #region Methods

        /// <summary>
        /// Load the library using a number of options
        /// </summary>
        /// <param name="nativeShellItem">
        /// IShellItem
        /// </param>
        /// <param name="isReadOnly">
        /// read-only flag
        /// </param>
        /// <returns>
        /// A ShellLibrary Object
        /// </returns>
        internal static ShellLibrary FromShellItem(IShellItem nativeShellItem, bool isReadOnly)
        {
            CoreHelpers.ThrowIfNotWin7();

            var nativeShellLibrary = (INativeShellLibrary)new ShellLibraryCOClass();

            var flags = isReadOnly ? ShellNativeMethods.STGMs.Read : ShellNativeMethods.STGMs.ReadWrite;

            nativeShellLibrary.LoadLibraryFromItem(nativeShellItem, flags);

            var library = new ShellLibrary(nativeShellLibrary) { nativeShellItem = (IShellItem2)nativeShellItem };

            return library;
        }

        /// <summary>
        /// Release resources
        /// </summary>
        /// <param name="disposing">
        /// Indicates that this was called from Dispose(), rather than from the finalizer.
        /// </param>
        protected override void Dispose(bool disposing)
        {
            if (this.nativeShellLibrary != null)
            {
                Marshal.ReleaseComObject(this.nativeShellLibrary);
                this.nativeShellLibrary = null;
            }

            base.Dispose(disposing);
        }

        /// <summary>
        /// </summary>
        /// <param name="folderTypeGuid">
        /// </param>
        /// <returns>
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// </exception>
        private static LibraryFolderType GetFolderTypefromGuid(Guid folderTypeGuid)
        {
            for (var i = 0; i < FolderTypesGuids.Length; i++)
            {
                if (folderTypeGuid.Equals(FolderTypesGuids[i]))
                {
                    return (LibraryFolderType)i;
                }
            }

            throw new ArgumentOutOfRangeException("folderTypeGuid", "Invalid FoldeType Guid");
        }

        /// <summary>
        /// </summary>
        /// <param name="shellLibrary">
        /// </param>
        /// <param name="windowHandle">
        /// </param>
        /// <param name="title">
        /// </param>
        /// <param name="instruction">
        /// </param>
        /// <param name="allowAllLocations">
        /// </param>
        private static void ShowManageLibraryUI(ShellObject shellLibrary, IntPtr windowHandle, string title, string instruction, bool allowAllLocations)
        {
            var hr = 0;

            var staWorker =
                new Thread(
                    () =>
                        {
                            hr = ShellNativeMethods.SHShowManageLibraryUI(
                                shellLibrary.NativeShellItem, 
                                windowHandle, 
                                title, 
                                instruction, 
                                allowAllLocations
                                    ? ShellNativeMethods.LIBRARYMANAGEDIALOGOPTIONS.LmdNounindexablelocationwarning
                                    : ShellNativeMethods.LIBRARYMANAGEDIALOGOPTIONS.LmdDefault);
                        });

            staWorker.SetApartmentState(ApartmentState.STA);
            staWorker.Start();
            staWorker.Join();

            if (!CoreErrorHelper.Succeeded(hr))
            {
                Marshal.ThrowExceptionForHR(hr);
            }
        }

        /// <summary>
        /// </summary>
        /// <returns>
        /// </returns>
        private List<ShellFileSystemFolder> GetFolders()
        {
            var list = new List<ShellFileSystemFolder>();
            IShellItemArray itemArray;

            var shellItemArrayGuid = new Guid(ShellIidGuid.IShellItemArray);

            var hr = this.nativeShellLibrary.GetFolders(ShellNativeMethods.LIBRARYFOLDERFILTER.LffAllitems, ref shellItemArrayGuid, out itemArray);

            if (!CoreErrorHelper.Succeeded((int)hr))
            {
                return list;
            }

            uint count;
            itemArray.GetCount(out count);

            for (uint i = 0; i < count; ++i)
            {
                IShellItem shellItem;
                itemArray.GetItemAt(i, out shellItem);
                list.Add(new ShellFileSystemFolder(shellItem as IShellItem2));
            }

            Marshal.ReleaseComObject(itemArray);

            return list;
        }

        #endregion
    }
}