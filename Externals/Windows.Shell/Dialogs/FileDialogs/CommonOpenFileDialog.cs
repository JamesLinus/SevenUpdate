//***********************************************************************
// Assembly         : Windows.Shell
// Author           : sevenalive
// Created          : 09-17-2010
// Last Modified By : sevenalive
// Last Modified On : 10-05-2010
// Description      : 
// Copyright        : (c) Seven Software. All rights reserved.
//***********************************************************************

namespace Microsoft.Windows.Dialogs
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.InteropServices;
    using System.Security.Permissions;

    using Microsoft.Windows.Shell;

    /// <summary>
    /// Creates a Vista or Windows 7 Common File Dialog, allowing the user to select one or more files.
    /// </summary>
    [FileDialogPermission(SecurityAction.Demand, Open = true)]
    public sealed class CommonOpenFileDialog : CommonFileDialog
    {
        #region Constants and Fields

        /// <summary>
        /// </summary>
        private bool allowNonFileSystem;

        /// <summary>
        /// </summary>
        private bool isFolderPicker;

        /// <summary>
        /// </summary>
        private bool multiselect;

        /// <summary>
        /// </summary>
        private INativeFileOpenDialog openDialogCoClass;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///   Creates a new instance of this class.
        /// </summary>
        public CommonOpenFileDialog()
        {
            // For Open file dialog, allow read only files.
            this.EnsureReadOnly = true;
        }

        /// <summary>
        /// Creates a new instance of this class with the specified name.
        /// </summary>
        /// <param name="name">
        /// The name of this dialog.
        /// </param>
        public CommonOpenFileDialog(string name)
            : base(name)
        {
            // For Open file dialog, allow read only files.
            this.EnsureReadOnly = true;
        }

        #endregion

        #region Properties

        /// <summary>
        ///   Gets or sets a value that determines whether the user can select non-filesystem items, 
        ///   such as <b>Library</b>, <b>Search Connectors</b>, or <b>Known Folders</b>.
        /// </summary>
        public bool AllowNonFileSystemItems
        {
            get
            {
                return this.allowNonFileSystem;
            }

            set
            {
                this.allowNonFileSystem = value;
            }
        }

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
                this.CheckFileNamesAvailable();
                return this.fileNames;
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
                this.CheckFileItemsAvailable();

                // temp collection to hold our shellobjects
                ICollection<ShellObject> resultItems = new Collection<ShellObject>();

                // Loop through our existing list of filenames, and try to create a concrete type of
                // ShellObject (e.g. ShellLibrary, FileSystemFolder, ShellFile, etc)
                foreach (var si in this.items)
                {
                    resultItems.Add(ShellObjectFactory.Create(si));
                }

                return resultItems;
            }
        }

        /// <summary>
        ///   Gets or sets a value that determines whether the user can select folders or files.
        ///   Default value is false.
        /// </summary>
        public bool IsFolderPicker
        {
            get
            {
                return this.isFolderPicker;
            }

            set
            {
                this.isFolderPicker = value;
            }
        }

        /// <summary>
        ///   Gets or sets a value that determines whether the user can select more than one file.
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Multiselect", 
            Justification = "This is following the same convention as the Winforms CFD")]
        public bool Multiselect
        {
            get
            {
                return this.multiselect;
            }

            set
            {
                this.multiselect = value;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// </summary>
        /// <param name="flags">
        /// </param>
        /// <returns>
        /// </returns>
        internal override ShellNativeMethods.FOSs GetDerivedOptionFlags(ShellNativeMethods.FOSs flags)
        {
            if (this.multiselect)
            {
                flags |= ShellNativeMethods.FOSs.FosAllowmultiselect;
            }

            if (this.isFolderPicker)
            {
                flags |= ShellNativeMethods.FOSs.FosPickfolders;
            }

            if (!this.allowNonFileSystem)
            {
                flags |= ShellNativeMethods.FOSs.FosForcefilesystem;
            }

            if (this.allowNonFileSystem)
            {
                flags |= ShellNativeMethods.FOSs.FosAllnonstorageitems;
            }

            return flags;
        }

        /// <summary>
        /// </summary>
        /// <returns>
        /// </returns>
        internal override IFileDialog GetNativeFileDialog()
        {
            Debug.Assert(this.openDialogCoClass != null, "Must call Initialize() before fetching dialog interface");
            return this.openDialogCoClass;
        }

        /// <summary>
        /// </summary>
        /// <param name="items">
        /// </param>
        internal override void PopulateWithIShellItems(Collection<IShellItem> items)
        {
            IShellItemArray resultsArray;
            uint count;

            this.openDialogCoClass.GetResults(out resultsArray);
            resultsArray.GetCount(out count);
            items.Clear();
            for (var i = 0; i < count; i++)
            {
                items.Add(GetShellItemAt(resultsArray, i));
            }
        }

        /// <summary>
        /// </summary>
        protected override void CleanUpNativeFileDialog()
        {
            if (this.openDialogCoClass != null)
            {
                Marshal.ReleaseComObject(this.openDialogCoClass);
            }
        }

        /// <summary>
        /// </summary>
        protected override void InitializeNativeFileDialog()
        {
            if (this.openDialogCoClass == null)
            {
                this.openDialogCoClass = new INativeFileOpenDialog();
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="names">
        /// </param>
        protected override void PopulateWithFileNames(Collection<string> names)
        {
            IShellItemArray resultsArray;
            uint count;

            this.openDialogCoClass.GetResults(out resultsArray);
            resultsArray.GetCount(out count);
            names.Clear();
            for (var i = 0; i < count; i++)
            {
                names.Add(GetFileNameFromShellItem(GetShellItemAt(resultsArray, i)));
            }
        }

        #endregion
    }
}