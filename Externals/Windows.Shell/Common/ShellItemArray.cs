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
using System.Collections.Generic;
using Microsoft.Windows.Internal;
using Microsoft.Windows.Shell.PropertySystem;

#endregion

namespace Microsoft.Windows.Shell
{
    internal class ShellItemArray : IShellItemArray
    {
        private readonly List<IShellItem> shellItemsList = new List<IShellItem>();

        internal ShellItemArray(IEnumerable<IShellItem> shellItems)
        {
            shellItemsList.AddRange(shellItems);
        }

        #region IShellItemArray Members

        public HRESULT BindToHandler(IntPtr pbc, ref Guid rbhid, ref Guid riid, out IntPtr ppvOut)
        {
            throw new NotSupportedException();
        }

        public HRESULT GetPropertyStore(int Flags, ref Guid riid, out IntPtr ppv)
        {
            throw new NotSupportedException();
        }

        public HRESULT GetPropertyDescriptionList(ref PropertyKey keyType, ref Guid riid, out IntPtr ppv)
        {
            throw new NotSupportedException();
        }

        public HRESULT GetAttributes(ShellNativeMethods.SIATTRIBFLAGS dwAttribFlags, ShellNativeMethods.SFGAO sfgaoMask, out ShellNativeMethods.SFGAO psfgaoAttribs)
        {
            throw new NotSupportedException();
        }

        public HRESULT GetCount(out uint pdwNumItems)
        {
            pdwNumItems = (uint) shellItemsList.Count;
            return HRESULT.S_OK;
        }

        public HRESULT GetItemAt(uint dwIndex, out IShellItem ppsi)
        {
            var index = (int) dwIndex;

            if (index < shellItemsList.Count)
            {
                ppsi = shellItemsList[index];
                return HRESULT.S_OK;
            }
            ppsi = null;
            return HRESULT.E_FAIL;
        }

        public HRESULT EnumItems(out IntPtr ppenumShellItems)
        {
            throw new NotSupportedException();
        }

        #endregion
    }
}