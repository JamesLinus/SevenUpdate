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
    using System.IO;
    using System.Runtime.InteropServices;

    /// <summary>
    /// Represents a registered non file system Known Folder
    /// </summary>
    [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", 
        Justification = "This will complicate the class hierarchy and naming convention used in the Shell area")]
    internal sealed class NonFileSystemKnownFolder : ShellNonFileSystemFolder, IKnownFolder
    {
        #region Constants and Fields

        /// <summary>
        /// </summary>
        private IKnownFolderNative knownFolderNative;

        /// <summary>
        /// </summary>
        private KnownFolderSettings knownFolderSettings;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// </summary>
        /// <param name="shellItem">
        /// </param>
        internal NonFileSystemKnownFolder(IShellItem2 shellItem)
            : base(shellItem)
        {
        }

        /// <summary>
        /// </summary>
        /// <param name="kf">
        /// </param>
        internal NonFileSystemKnownFolder(IKnownFolderNative kf)
        {
            Debug.Assert(kf != null);
            this.knownFolderNative = kf;

            // Set the native shell item
            // and set it on the base class (ShellObject)
            var guid = new Guid(ShellIidGuid.IShellItem2);
            this.knownFolderNative.GetShellItem(0, ref guid, out this.nativeShellItem);
        }

        #endregion

        #region Properties

        /// <summary>
        ///   Gets this known folder's canonical name.
        /// </summary>
        /// <value>A <see cref = "System.String" /> object.</value>
        public string CanonicalName
        {
            get
            {
                return this.KnownFolderSettings.CanonicalName;
            }
        }

        /// <summary>
        ///   Gets the category designation for this known folder.
        /// </summary>
        /// <value>A <see cref = "FolderCategory" /> value.</value>
        public FolderCategory Category
        {
            get
            {
                return this.KnownFolderSettings.Category;
            }
        }

        /// <summary>
        ///   Gets an value that describes this known folder's behaviors.
        /// </summary>
        /// <value>A <see cref = "DefinitionOptions" /> value.</value>
        public DefinitionOptions DefinitionOptions
        {
            get
            {
                return this.KnownFolderSettings.DefinitionOptions;
            }
        }

        /// <summary>
        ///   Gets this known folder's description.
        /// </summary>
        /// <value>A <see cref = "System.String" /> object.</value>
        public string Description
        {
            get
            {
                return this.KnownFolderSettings.Description;
            }
        }

        /// <summary>
        ///   Gets this known folder's file attributes, 
        ///   such as "read-only".
        /// </summary>
        /// <value>A <see cref = "System.IO.FileAttributes" /> value.</value>
        public FileAttributes FileAttributes
        {
            get
            {
                return this.KnownFolderSettings.FileAttributes;
            }
        }

        /// <summary>
        ///   Gets the unique identifier for this known folder.
        /// </summary>
        /// <value>A <see cref = "System.Guid" /> value.</value>
        public Guid FolderId
        {
            get
            {
                return this.KnownFolderSettings.FolderId;
            }
        }

        /// <summary>
        ///   Gets a string representation of this known folder's type.
        /// </summary>
        /// <value>A <see cref = "System.String" /> object.</value>
        public string FolderType
        {
            get
            {
                return this.KnownFolderSettings.FolderType;
            }
        }

        /// <summary>
        ///   Gets the unique identifier for this known folder's type.
        /// </summary>
        /// <value>A <see cref = "System.Guid" /> value.</value>
        public Guid FolderTypeId
        {
            get
            {
                return this.KnownFolderSettings.FolderTypeId;
            }
        }

        /// <summary>
        ///   Gets this known folder's localized name.
        /// </summary>
        /// <value>A <see cref = "System.String" /> object.</value>
        public string LocalizedName
        {
            get
            {
                return this.KnownFolderSettings.LocalizedName;
            }
        }

        /// <summary>
        ///   Gets the resource identifier for this 
        ///   known folder's localized name.
        /// </summary>
        /// <value>A <see cref = "System.String" /> object.</value>
        public string LocalizedNameResourceId
        {
            get
            {
                return this.KnownFolderSettings.LocalizedNameResourceId;
            }
        }

        /// <summary>
        ///   Gets the unique identifier for this known folder's parent folder.
        /// </summary>
        /// <value>A <see cref = "System.Guid" /> value.</value>
        public Guid ParentId
        {
            get
            {
                return this.KnownFolderSettings.ParentId;
            }
        }

        /// <summary>
        ///   Gets the path for this known folder.
        /// </summary>
        /// <value>A <see cref = "System.String" /> object.</value>
        public string Path
        {
            get
            {
                return this.KnownFolderSettings.Path;
            }
        }

        /// <summary>
        ///   Gets a value that indicates whether this known folder's path exists on the computer.
        /// </summary>
        /// <value>A bool<see cref = "System.Boolean" /> value.</value>
        /// <remarks>
        ///   If this property value is <b>false</b>, 
        ///   the folder might be a virtual folder (<see cref = "Category" /> property will
        ///   be <see cref = "FolderCategory.Virtual" /> for virtual folders)
        /// </remarks>
        public bool PathExists
        {
            get
            {
                return this.KnownFolderSettings.PathExists;
            }
        }

        /// <summary>
        ///   Gets a value that states whether this known folder 
        ///   can have its path set to a new value, 
        ///   including any restrictions on the redirection.
        /// </summary>
        /// <value>A <see cref = "RedirectionCapabilities" /> value.</value>
        public RedirectionCapability Redirection
        {
            get
            {
                return this.KnownFolderSettings.Redirection;
            }
        }

        /// <summary>
        ///   Gets this known folder's relative path.
        /// </summary>
        /// <value>A <see cref = "System.String" /> object.</value>
        public string RelativePath
        {
            get
            {
                return this.KnownFolderSettings.RelativePath;
            }
        }

        /// <summary>
        ///   Gets this known folder's security attributes.
        /// </summary>
        /// <value>A <see cref = "System.String" /> object.</value>
        public string Security
        {
            get
            {
                return this.KnownFolderSettings.Security;
            }
        }

        /// <summary>
        ///   Gets this known folder's tool tip text.
        /// </summary>
        /// <value>A <see cref = "System.String" /> object.</value>
        public string Tooltip
        {
            get
            {
                return this.KnownFolderSettings.Tooltip;
            }
        }

        /// <summary>
        ///   Gets the resource identifier for this 
        ///   known folder's tool tip text.
        /// </summary>
        /// <value>A <see cref = "System.String" /> object.</value>
        public string TooltipResourceId
        {
            get
            {
                return this.KnownFolderSettings.TooltipResourceId;
            }
        }

        /// <summary>
        /// </summary>
        private KnownFolderSettings KnownFolderSettings
        {
            get
            {
                if (this.knownFolderNative == null)
                {
                    // We need to get the PIDL either from the NativeShellItem,
                    // or from base class's property (if someone already set it on us).
                    // Need to use the PIDL to get the native IKnownFolder interface.

                    // Get teh PIDL for the ShellItem
                    if (this.nativeShellItem != null && this.PIDL == IntPtr.Zero)
                    {
                        this.PIDL = ShellHelper.PidlFromShellItem(this.nativeShellItem);
                    }

                    // If we have a valid PIDL, get the native IKnownFolder
                    if (this.PIDL != IntPtr.Zero)
                    {
                        this.knownFolderNative = KnownFolderHelper.FromPidl(this.PIDL);
                    }

                    Debug.Assert(this.knownFolderNative != null);
                }

                // If this is the first time this property is being called,
                // get the native Folder Defination (KnownFolder properties)
                return this.knownFolderSettings ?? (this.knownFolderSettings = new KnownFolderSettings(this.knownFolderNative));
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Release resources
        /// </summary>
        /// <param name="disposing">
        /// Indicates that this mothod is being called from Dispose() rather than the finalizer.
        /// </param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.knownFolderSettings = null;
            }

            if (this.knownFolderNative != null)
            {
                Marshal.ReleaseComObject(this.knownFolderNative);
                this.knownFolderNative = null;
            }

            base.Dispose(disposing);
        }

        #endregion
    }
}