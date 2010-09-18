//Copyright (c) Microsoft Corporation.  All rights reserved.
//Modified by Robert Baker, Seven Software 2010.

#region

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using Microsoft.Windows.Shell;

#endregion

namespace Microsoft.Windows.Dialogs
{
    /// <summary>
    ///   Creates a Vista or Windows 7 Common File Dialog, allowing the user to select one or more files.
    /// </summary>
    [FileDialogPermission(SecurityAction.Demand, Open = true)]
    public sealed class CommonOpenFileDialog : CommonFileDialog
    {
        private NativeFileOpenDialog openDialogCoClass;

        /// <summary>
        ///   Creates a new instance of this class.
        /// </summary>
        public CommonOpenFileDialog()
        {
            // For Open file dialog, allow read only files.
            EnsureReadOnly = true;
        }

        /// <summary>
        ///   Creates a new instance of this class with the specified name.
        /// </summary>
        /// <param name = "name">The name of this dialog.</param>
        public CommonOpenFileDialog(string name) : base(name)
        {
            // For Open file dialog, allow read only files.
            EnsureReadOnly = true;
        }

        #region Public API specific to Open

        private bool allowNonFileSystem;
        private bool isFolderPicker;
        private bool multiselect;

        /// <summary>
        ///   Gets a collection of the selected file names.
        /// </summary>
        /// <remarks>
        ///   This property should only be used when the
        ///   <see cref = "CommonOpenFileDialog.Multiselect" />
        ///   property is <b>true</b>.
        /// </remarks>
        public Collection<string> FileNames
        {
            get
            {
                CheckFileNamesAvailable();
                return fileNames;
            }
        }

        /// <summary>
        ///   Gets a collection of the selected items as ShellObject objects.
        /// </summary>
        /// <remarks>
        ///   This property should only be used when the
        ///   <see cref = "CommonOpenFileDialog.Multiselect" />
        ///   property is <b>true</b>.
        /// </remarks>
        public ICollection<ShellObject> FilesAsShellObject
        {
            get
            {
                // Check if we have selected files from the user.              
                CheckFileItemsAvailable();

                // temp collection to hold our shellobjects
                ICollection<ShellObject> resultItems = new Collection<ShellObject>();

                // Loop through our existing list of filenames, and try to create a concrete type of
                // ShellObject (e.g. ShellLibrary, FileSystemFolder, ShellFile, etc)
                foreach (var si in items)
                    resultItems.Add(ShellObjectFactory.Create(si));

                return resultItems;
            }
        }

        /// <summary>
        ///   Gets or sets a value that determines whether the user can select more than one file.
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Multiselect", Justification = "This is following the same convention as the Winforms CFD")]
        public bool Multiselect { get { return multiselect; } set { multiselect = value; } }

        /// <summary>
        ///   Gets or sets a value that determines whether the user can select folders or files.
        ///   Default value is false.
        /// </summary>
        public bool IsFolderPicker { get { return isFolderPicker; } set { isFolderPicker = value; } }

        /// <summary>
        ///   Gets or sets a value that determines whether the user can select non-filesystem items, 
        ///   such as <b>Library</b>, <b>Search Connectors</b>, or <b>Known Folders</b>.
        /// </summary>
        public bool AllowNonFileSystemItems { get { return allowNonFileSystem; } set { allowNonFileSystem = value; } }

        #endregion

        internal override IFileDialog GetNativeFileDialog()
        {
            Debug.Assert(openDialogCoClass != null, "Must call Initialize() before fetching dialog interface");
            return openDialogCoClass;
        }

        protected override void InitializeNativeFileDialog()
        {
            if (openDialogCoClass == null)
                openDialogCoClass = new NativeFileOpenDialog();
        }

        protected override void CleanUpNativeFileDialog()
        {
            if (openDialogCoClass != null)
                Marshal.ReleaseComObject(openDialogCoClass);
        }

        protected override void PopulateWithFileNames(Collection<string> names)
        {
            IShellItemArray resultsArray;
            uint count;

            openDialogCoClass.GetResults(out resultsArray);
            resultsArray.GetCount(out count);
            names.Clear();
            for (var i = 0; i < count; i++)
                names.Add(GetFileNameFromShellItem(GetShellItemAt(resultsArray, i)));
        }

        internal override void PopulateWithIShellItems(Collection<IShellItem> items)
        {
            IShellItemArray resultsArray;
            uint count;

            openDialogCoClass.GetResults(out resultsArray);
            resultsArray.GetCount(out count);
            items.Clear();
            for (var i = 0; i < count; i++)
                items.Add(GetShellItemAt(resultsArray, i));
        }

        internal override ShellNativeMethods.FOS GetDerivedOptionFlags(ShellNativeMethods.FOS flags)
        {
            if (multiselect)
                flags |= ShellNativeMethods.FOS.FOS_ALLOWMULTISELECT;
            if (isFolderPicker)
                flags |= ShellNativeMethods.FOS.FOS_PICKFOLDERS;
            if (!allowNonFileSystem)
                flags |= ShellNativeMethods.FOS.FOS_FORCEFILESYSTEM;
            if (allowNonFileSystem)
                flags |= ShellNativeMethods.FOS.FOS_ALLNONSTORAGEITEMS;

            return flags;
        }
    }
}