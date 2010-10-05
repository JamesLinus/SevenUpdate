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
    using System.Collections.Generic;

    using Microsoft.Windows.Internal;
    using Microsoft.Windows.Shell.PropertySystem;

    /// <summary>
    /// </summary>
    internal class ShellItemArray : IShellItemArray
    {
        #region Constants and Fields

        /// <summary>
        /// </summary>
        private readonly List<IShellItem> shellItemsList = new List<IShellItem>();

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// </summary>
        /// <param name="shellItems">
        /// </param>
        internal ShellItemArray(IEnumerable<IShellItem> shellItems)
        {
            this.shellItemsList.AddRange(shellItems);
        }

        #endregion

        #region Implemented Interfaces

        #region IShellItemArray

        /// <summary>
        /// </summary>
        /// <param name="pbc">
        /// </param>
        /// <param name="rbhid">
        /// </param>
        /// <param name="riid">
        /// </param>
        /// <param name="ppvOut">
        /// </param>
        /// <returns>
        /// </returns>
        /// <exception cref="NotSupportedException">
        /// </exception>
        public HRESULT BindToHandler(IntPtr pbc, ref Guid rbhid, ref Guid riid, out IntPtr ppvOut)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// </summary>
        /// <param name="ppenumShellItems">
        /// </param>
        /// <returns>
        /// </returns>
        /// <exception cref="NotSupportedException">
        /// </exception>
        public HRESULT EnumItems(out IntPtr ppenumShellItems)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// </summary>
        /// <param name="dwAttribFlags">
        /// </param>
        /// <param name="sfgaoMask">
        /// </param>
        /// <param name="psfgaoAttribs">
        /// </param>
        /// <returns>
        /// </returns>
        /// <exception cref="NotSupportedException">
        /// </exception>
        public HRESULT GetAttributes(ShellNativeMethods.SIATTRIBFLAGS dwAttribFlags, ShellNativeMethods.SFGAOs sfgaoMask, out ShellNativeMethods.SFGAOs psfgaoAttribs)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// </summary>
        /// <param name="pdwNumItems">
        /// </param>
        /// <returns>
        /// </returns>
        public HRESULT GetCount(out uint pdwNumItems)
        {
            pdwNumItems = (uint)this.shellItemsList.Count;
            return HRESULT.S_OK;
        }

        /// <summary>
        /// </summary>
        /// <param name="dwIndex">
        /// </param>
        /// <param name="ppsi">
        /// </param>
        /// <returns>
        /// </returns>
        public HRESULT GetItemAt(uint dwIndex, out IShellItem ppsi)
        {
            var index = (int)dwIndex;

            if (index < this.shellItemsList.Count)
            {
                ppsi = this.shellItemsList[index];
                return HRESULT.S_OK;
            }

            ppsi = null;
            return HRESULT.EFail;
        }

        /// <summary>
        /// </summary>
        /// <param name="keyType">
        /// </param>
        /// <param name="riid">
        /// </param>
        /// <param name="ppv">
        /// </param>
        /// <returns>
        /// </returns>
        /// <exception cref="NotSupportedException">
        /// </exception>
        public HRESULT GetPropertyDescriptionList(ref PropertyKey keyType, ref Guid riid, out IntPtr ppv)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// </summary>
        /// <param name="flags">
        /// </param>
        /// <param name="riid">
        /// </param>
        /// <param name="ppv">
        /// </param>
        /// <returns>
        /// </returns>
        /// <exception cref="NotSupportedException">
        /// </exception>
        public HRESULT GetPropertyStore(int flags, ref Guid riid, out IntPtr ppv)
        {
            throw new NotSupportedException();
        }

        #endregion

        #endregion
    }
}