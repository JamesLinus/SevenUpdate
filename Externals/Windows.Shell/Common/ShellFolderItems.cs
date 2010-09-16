#region GNU Public License Version 3

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
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Windows.Internal;

#endregion

namespace Microsoft.Windows.Shell
{
    internal class ShellFolderItems : IEnumerator<ShellObject>
    {
        #region Private Fields

        private readonly ShellContainer nativeShellFolder;
        private IEnumIDList nativeEnumIdList;

        #endregion

        #region Internal Constructor

        internal ShellFolderItems(ShellContainer nativeShellFolder)
        {
            this.nativeShellFolder = nativeShellFolder;

            var hr = nativeShellFolder.NativeShellFolder.EnumObjects(IntPtr.Zero, ShellNativeMethods.SHCONT.SHCONTF_FOLDERS | ShellNativeMethods.SHCONT.SHCONTF_NONFOLDERS, out nativeEnumIdList);


            if (CoreErrorHelper.Succeeded((int) hr))
                return;
            if (hr == HRESULT.E_ERROR_CANCELLED)
                throw new FileNotFoundException();
            Marshal.ThrowExceptionForHR((int) hr);
        }

        #endregion

        #region IEnumerator<ShellObject> Members

        public ShellObject Current { get; private set; }

        public void Dispose()
        {
            if (nativeEnumIdList == null)
                return;
            Marshal.ReleaseComObject(nativeEnumIdList);
            nativeEnumIdList = null;
        }

        object IEnumerator.Current { get { return Current; } }

        /// <summary>
        /// </summary>
        /// <returns />
        public bool MoveNext()
        {
            if (nativeEnumIdList == null)
                return false;

            IntPtr item;
            uint numItemsReturned;
            const uint itemsRequested = 1;
            var hr = nativeEnumIdList.Next(itemsRequested, out item, out numItemsReturned);

            if (numItemsReturned < itemsRequested || hr != HRESULT.S_OK)
                return false;

            Current = ShellObjectFactory.Create(item, nativeShellFolder);

            return true;
        }

        /// <summary>
        /// </summary>
        public void Reset()
        {
            if (nativeEnumIdList != null)
                nativeEnumIdList.Reset();
        }

        #endregion
    }
}