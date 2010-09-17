//Copyright (c) Microsoft Corporation.  All rights reserved.
//Modified by Robert Baker, Seven Software 2010.

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