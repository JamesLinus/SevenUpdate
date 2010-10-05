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
    using System.IO;
    using System.Runtime.InteropServices;

    using Microsoft.Windows.Internal;

    /// <summary>
    /// </summary>
    internal class ShellFolderItems : IEnumerator<ShellObject>
    {
        #region Constants and Fields

        /// <summary>
        /// </summary>
        private readonly ShellContainer nativeShellFolder;

        /// <summary>
        /// </summary>
        private IEnumIDList nativeEnumIdList;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// </summary>
        /// <param name="nativeShellFolder">
        /// </param>
        /// <exception cref="FileNotFoundException">
        /// </exception>
        internal ShellFolderItems(ShellContainer nativeShellFolder)
        {
            this.nativeShellFolder = nativeShellFolder;

            var hr = nativeShellFolder.NativeShellFolder.EnumObjects(
                IntPtr.Zero, ShellNativeMethods.SHCONTs.ShcontfFolders | ShellNativeMethods.SHCONTs.ShcontfNonfolders, out this.nativeEnumIdList);

            if (CoreErrorHelper.Succeeded((int)hr))
            {
                return;
            }

            if (hr == HRESULT.EErrorCancelled)
            {
                throw new FileNotFoundException();
            }

            Marshal.ThrowExceptionForHR((int)hr);
        }

        #endregion

        #region Properties

        /// <summary>
        /// </summary>
        public ShellObject Current { get; private set; }

        /// <summary>
        /// </summary>
        object IEnumerator.Current
        {
            get
            {
                return this.Current;
            }
        }

        #endregion

        #region Implemented Interfaces

        #region IDisposable

        /// <summary>
        /// </summary>
        public void Dispose()
        {
            if (this.nativeEnumIdList == null)
            {
                return;
            }

            Marshal.ReleaseComObject(this.nativeEnumIdList);
            this.nativeEnumIdList = null;
        }

        #endregion

        #region IEnumerator

        /// <summary>
        /// </summary>
        /// <returns>
        /// </returns>
        public bool MoveNext()
        {
            if (this.nativeEnumIdList == null)
            {
                return false;
            }

            IntPtr item;
            uint numItemsReturned;
            const uint itemsRequested = 1;
            var hr = this.nativeEnumIdList.Next(itemsRequested, out item, out numItemsReturned);

            if (numItemsReturned < itemsRequested || hr != HRESULT.S_OK)
            {
                return false;
            }

            this.Current = ShellObjectFactory.Create(item, this.nativeShellFolder);

            return true;
        }

        /// <summary>
        /// </summary>
        public void Reset()
        {
            if (this.nativeEnumIdList != null)
            {
                this.nativeEnumIdList.Reset();
            }
        }

        #endregion

        #endregion
    }
}