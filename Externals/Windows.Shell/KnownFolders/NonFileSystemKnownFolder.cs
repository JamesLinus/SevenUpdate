﻿#region GNU Public License Version 3

// Copyright 2007-2010 Robert Baker, Seven Software.
// This file is part of Seven Update.
//   
//      Seven Update is free software: you can redistribute it and/or modify
//      it under the terms of the GNU General Public License as published by
//      the Free Software Foundation, either version 3 of the License, or
//      (at your option) any later version.
//  
//      Seven Update is distributed in the hope that it will be useful,
//      but WITHOUT ANY WARRANTY; without even the implied warranty of
//      MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//      GNU General Public License for more details.
//   
//      You should have received a copy of the GNU General Public License
//      along with Seven Update.  If not, see <http://www.gnu.org/licenses/>.

#endregion

#region

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.InteropServices;

#endregion

namespace Microsoft.Windows.Shell
{
    /// <summary>
    ///   Represents a registered non file system Known Folder
    /// </summary>
    [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "This will complicate the class hierarchy and naming convention used in the Shell area")]
    internal sealed class NonFileSystemKnownFolder : ShellNonFileSystemFolder, IKnownFolder
    {
        #region Private Fields

        private IKnownFolderNative knownFolderNative;
        private KnownFolderSettings knownFolderSettings;

        #endregion

        #region Internal Constructors

        internal NonFileSystemKnownFolder(IShellItem2 shellItem) : base(shellItem)
        {
        }

        internal NonFileSystemKnownFolder(IKnownFolderNative kf)
        {
            Debug.Assert(kf != null);
            knownFolderNative = kf;

            // Set the native shell item
            // and set it on the base class (ShellObject)
            var guid = new Guid(ShellIIDGuid.IShellItem2);
            knownFolderNative.GetShellItem(0, ref guid, out nativeShellItem);
        }

        #endregion

        private KnownFolderSettings KnownFolderSettings
        {
            get
            {
                if (knownFolderNative == null)
                {
                    // We need to get the PIDL either from the NativeShellItem,
                    // or from base class's property (if someone already set it on us).
                    // Need to use the PIDL to get the native IKnownFolder interface.

                    // Get teh PIDL for the ShellItem
                    if (nativeShellItem != null && PIDL == IntPtr.Zero)
                        PIDL = ShellHelper.PidlFromShellItem(nativeShellItem);

                    // If we have a valid PIDL, get the native IKnownFolder
                    if (PIDL != IntPtr.Zero)
                        knownFolderNative = KnownFolderHelper.FromPIDL(PIDL);

                    Debug.Assert(knownFolderNative != null);
                }

                // If this is the first time this property is being called,
                // get the native Folder Defination (KnownFolder properties)
                return knownFolderSettings ?? (knownFolderSettings = new KnownFolderSettings(knownFolderNative));
            }
        }

        #region IKnownFolder Members

        /// <summary>
        ///   Gets the path for this known folder.
        /// </summary>
        /// <value>A <see cref = "System.String" /> object.</value>
        public string Path { get { return KnownFolderSettings.Path; } }

        /// <summary>
        ///   Gets the category designation for this known folder.
        /// </summary>
        /// <value>A <see cref = "FolderCategory" /> value.</value>
        public FolderCategory Category { get { return KnownFolderSettings.Category; } }

        /// <summary>
        ///   Gets this known folder's canonical name.
        /// </summary>
        /// <value>A <see cref = "System.String" /> object.</value>
        public string CanonicalName { get { return KnownFolderSettings.CanonicalName; } }

        /// <summary>
        ///   Gets this known folder's description.
        /// </summary>
        /// <value>A <see cref = "System.String" /> object.</value>
        public string Description { get { return KnownFolderSettings.Description; } }

        /// <summary>
        ///   Gets the unique identifier for this known folder's parent folder.
        /// </summary>
        /// <value>A <see cref = "System.Guid" /> value.</value>
        public Guid ParentId { get { return KnownFolderSettings.ParentId; } }

        /// <summary>
        ///   Gets this known folder's relative path.
        /// </summary>
        /// <value>A <see cref = "System.String" /> object.</value>
        public string RelativePath { get { return KnownFolderSettings.RelativePath; } }

        /// <summary>
        ///   Gets this known folder's parsing name.
        /// </summary>
        /// <value>A <see cref = "System.String" /> object.</value>
        public override string ParsingName { get { return base.ParsingName; } }

        /// <summary>
        ///   Gets this known folder's tool tip text.
        /// </summary>
        /// <value>A <see cref = "System.String" /> object.</value>
        public string Tooltip { get { return KnownFolderSettings.Tooltip; } }

        /// <summary>
        ///   Gets the resource identifier for this 
        ///   known folder's tool tip text.
        /// </summary>
        /// <value>A <see cref = "System.String" /> object.</value>
        public string TooltipResourceId { get { return KnownFolderSettings.TooltipResourceId; } }

        /// <summary>
        ///   Gets this known folder's localized name.
        /// </summary>
        /// <value>A <see cref = "System.String" /> object.</value>
        public string LocalizedName { get { return KnownFolderSettings.LocalizedName; } }

        /// <summary>
        ///   Gets the resource identifier for this 
        ///   known folder's localized name.
        /// </summary>
        /// <value>A <see cref = "System.String" /> object.</value>
        public string LocalizedNameResourceId { get { return KnownFolderSettings.LocalizedNameResourceId; } }

        /// <summary>
        ///   Gets this known folder's security attributes.
        /// </summary>
        /// <value>A <see cref = "System.String" /> object.</value>
        public string Security { get { return KnownFolderSettings.Security; } }

        /// <summary>
        ///   Gets this known folder's file attributes, 
        ///   such as "read-only".
        /// </summary>
        /// <value>A <see cref = "System.IO.FileAttributes" /> value.</value>
        public FileAttributes FileAttributes { get { return KnownFolderSettings.FileAttributes; } }

        /// <summary>
        ///   Gets an value that describes this known folder's behaviors.
        /// </summary>
        /// <value>A <see cref = "DefinitionOptions" /> value.</value>
        public DefinitionOptions DefinitionOptions { get { return KnownFolderSettings.DefinitionOptions; } }

        /// <summary>
        ///   Gets the unique identifier for this known folder's type.
        /// </summary>
        /// <value>A <see cref = "System.Guid" /> value.</value>
        public Guid FolderTypeId { get { return KnownFolderSettings.FolderTypeId; } }

        /// <summary>
        ///   Gets a string representation of this known folder's type.
        /// </summary>
        /// <value>A <see cref = "System.String" /> object.</value>
        public string FolderType { get { return KnownFolderSettings.FolderType; } }

        /// <summary>
        ///   Gets the unique identifier for this known folder.
        /// </summary>
        /// <value>A <see cref = "System.Guid" /> value.</value>
        public Guid FolderId { get { return KnownFolderSettings.FolderId; } }

        /// <summary>
        ///   Gets a value that indicates whether this known folder's path exists on the computer.
        /// </summary>
        /// <value>A bool<see cref = "System.Boolean" /> value.</value>
        /// <remarks>
        ///   If this property value is <b>false</b>, 
        ///   the folder might be a virtual folder (<see cref = "Category" /> property will
        ///   be <see cref = "FolderCategory.Virtual" /> for virtual folders)
        /// </remarks>
        public bool PathExists { get { return KnownFolderSettings.PathExists; } }

        /// <summary>
        ///   Gets a value that states whether this known folder 
        ///   can have its path set to a new value, 
        ///   including any restrictions on the redirection.
        /// </summary>
        /// <value>A <see cref = "RedirectionCapabilities" /> value.</value>
        public RedirectionCapabilities Redirection { get { return KnownFolderSettings.Redirection; } }

        #endregion

        /// <summary>
        ///   Release resources
        /// </summary>
        /// <param name = "disposing">Indicates that this mothod is being called from Dispose() rather than the finalizer.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
                knownFolderSettings = null;

            if (knownFolderNative != null)
            {
                Marshal.ReleaseComObject(knownFolderNative);
                knownFolderNative = null;
            }

            base.Dispose(disposing);
        }
    }
}