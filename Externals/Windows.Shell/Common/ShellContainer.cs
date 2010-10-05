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
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.InteropServices;

    using Microsoft.Windows.Internal;

    /// <summary>
    /// Represents the base class for all types of Shell "containers". Any class deriving from this class
    ///   can contain other ShellObjects (e.g. ShellFolder, FileSystemKnownFolder, ShellLibrary, etc)
    /// </summary>
    [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", 
        Justification = "This will complicate the class hierarchy and naming convention used in the Shell area")]
    public abstract class ShellContainer : ShellObject, IEnumerable<ShellObject>
    {
        #region Constants and Fields

        /// <summary>
        /// </summary>
        private IShellFolder desktopFolderEnumeration;

        /// <summary>
        /// </summary>
        private IShellFolder nativeShellFolder;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// </summary>
        internal ShellContainer()
        {
        }

        /// <summary>
        /// </summary>
        /// <param name="shellItem">
        /// </param>
        internal ShellContainer(IShellItem2 shellItem)
            : base(shellItem)
        {
        }

        #endregion

        #region Properties

        /// <summary>
        /// </summary>
        /// <exception cref="Exception">
        /// </exception>
        internal IShellFolder NativeShellFolder
        {
            get
            {
                if (this.nativeShellFolder == null)
                {
                    var guid = new Guid(ShellIidGuid.IShellFolder);
                    var handler = new Guid(ShellBhidGuid.ShellFolderObject);

                    var hr = this.NativeShellItem.BindToHandler(IntPtr.Zero, ref handler, ref guid, out this.nativeShellFolder);

                    if (CoreErrorHelper.Failed(hr))
                    {
                        var str = ShellHelper.GetParsingName(this.NativeShellItem);
                        if (str != null)
                        {
                            if (str == Environment.GetFolderPath(Environment.SpecialFolder.Desktop))
                            {
                                return null;
                            }

                            throw Marshal.GetExceptionForHR((int)hr);
                        }
                    }
                }

                return this.nativeShellFolder;
            }
        }

        #endregion

        #region Implemented Interfaces

        #region IEnumerable

        /// <summary>
        /// </summary>
        /// <returns>
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return new ShellFolderItems(this);
        }

        #endregion

        #region IEnumerable<ShellObject>

        /// <summary>
        /// </summary>
        /// <returns>
        /// </returns>
        public IEnumerator<ShellObject> GetEnumerator()
        {
            if (this.NativeShellFolder == null)
            {
                if (this.desktopFolderEnumeration == null)
                {
                    ShellNativeMethods.SHGetDesktopFolder(out this.desktopFolderEnumeration);
                }

                this.nativeShellFolder = this.desktopFolderEnumeration;
            }

            return new ShellFolderItems(this);
        }

        #endregion

        #endregion

        #region Methods

        /// <summary>
        /// Release resources
        /// </summary>
        /// <param name="disposing">
        /// <B>True</B> indicates that this is being called from Dispose(), rather than the finalizer.
        /// </param>
        protected override void Dispose(bool disposing)
        {
            if (this.nativeShellFolder != null)
            {
                Marshal.ReleaseComObject(this.nativeShellFolder);
                this.nativeShellFolder = null;
            }

            if (this.desktopFolderEnumeration != null)
            {
                Marshal.ReleaseComObject(this.desktopFolderEnumeration);
                this.desktopFolderEnumeration = null;
            }

            base.Dispose(disposing);
        }

        #endregion
    }
}