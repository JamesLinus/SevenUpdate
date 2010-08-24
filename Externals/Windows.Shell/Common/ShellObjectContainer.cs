//Copyright (c) Microsoft Corporation.  All rights reserved.

#region

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using Microsoft.Windows.Internal;

#endregion

namespace Microsoft.Windows.Shell
{
    /// <summary>
    ///   Represents the base class for all types of Shell "containers". Any class deriving from this class
    ///   can contain other ShellObjects (e.g. ShellFolder, FileSystemKnownFolder, ShellLibrary, etc)
    /// </summary>
    [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "This will complicate the class hierarchy and naming convention used in the Shell area")]
    public abstract class ShellContainer : ShellObject, IEnumerable<ShellObject>
    {
        #region Private Fields

        private IShellFolder desktopFolderEnumeration;
        private IShellFolder nativeShellFolder;

        #endregion

        #region Internal Properties

        internal IShellFolder NativeShellFolder
        {
            get
            {
                if (nativeShellFolder == null)
                {
                    var guid = new Guid(ShellIIDGuid.IShellFolder);
                    var handler = new Guid(ShellBHIDGuid.ShellFolderObject);

                    HRESULT hr = NativeShellItem.BindToHandler(IntPtr.Zero, ref handler, ref guid, out nativeShellFolder);

                    if (CoreErrorHelper.Failed(hr))
                    {
                        string str = ShellHelper.GetParsingName(NativeShellItem);
                        if (str != null)
                        {
                            if (str == Environment.GetFolderPath(Environment.SpecialFolder.Desktop))
                                return null;
                            throw Marshal.GetExceptionForHR((int) hr);
                        }
                    }
                }

                return nativeShellFolder;
            }
        }

        #endregion

        #region Internal Constructor

        internal ShellContainer()
        {
        }

        internal ShellContainer(IShellItem2 shellItem) : base(shellItem)
        {
        }

        #endregion

        #region Disposable Pattern

        /// <summary>
        ///   Release resources
        /// </summary>
        /// <param name = "disposing"><B>True</B> indicates that this is being called from Dispose(), rather than the finalizer.</param>
        protected override void Dispose(bool disposing)
        {
            if (nativeShellFolder != null)
            {
                Marshal.ReleaseComObject(nativeShellFolder);
                nativeShellFolder = null;
            }

            if (desktopFolderEnumeration != null)
            {
                Marshal.ReleaseComObject(desktopFolderEnumeration);
                desktopFolderEnumeration = null;
            }

            base.Dispose(disposing);
        }

        #endregion

        #region IEnumerable<ShellObject> Members

        /// <summary>
        /// </summary>
        /// <returns />
        public IEnumerator<ShellObject> GetEnumerator()
        {
            if (NativeShellFolder == null)
            {
                if (desktopFolderEnumeration == null)
                    ShellNativeMethods.SHGetDesktopFolder(out desktopFolderEnumeration);

                nativeShellFolder = desktopFolderEnumeration;
            }

            return new ShellFolderItems(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new ShellFolderItems(this);
        }

        #endregion
    }
}