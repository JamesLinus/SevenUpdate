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
    using System.IO;
    using System.Runtime.InteropServices;

    using Microsoft.Windows.Internal;
    using Microsoft.Windows.Shell.PropertySystem;

    /// <summary>
    /// A helper class for Shell Objects
    /// </summary>
    internal static class ShellHelper
    {
        #region Constants and Fields

        /// <summary>
        /// </summary>
        private static PropertyKey itemTypePropertyKey = new PropertyKey(new Guid("28636AA6-953D-11D2-B5D6-00C04FD918D0"), 11);

        #endregion

        #region Methods

        /// <summary>
        /// </summary>
        /// <param name="path">
        /// </param>
        /// <returns>
        /// </returns>
        internal static string GetAbsolutePath(string path)
        {
            return Uri.IsWellFormedUriString(path, UriKind.Absolute) ? path : Path.GetFullPath(path);
        }

        /// <summary>
        /// </summary>
        /// <param name="shellItem">
        /// </param>
        /// <returns>
        /// </returns>
        internal static string GetItemType(IShellItem2 shellItem)
        {
            if (shellItem != null)
            {
                string itemType;

                var hr = shellItem.GetString(ref itemTypePropertyKey, out itemType);

                if (hr == HRESULT.S_OK)
                {
                    return itemType;
                }
            }

            return null;
        }

        /// <summary>
        /// </summary>
        /// <param name="shellItem">
        /// </param>
        /// <returns>
        /// </returns>
        /// <exception cref="COMException">
        /// </exception>
        internal static string GetParsingName(IShellItem shellItem)
        {
            if (shellItem == null)
            {
                return null;
            }

            string path = null;

            IntPtr pszPath;
            var hr = shellItem.GetDisplayName(ShellNativeMethods.SIGDN.Desktopabsoluteparsing, out pszPath);

            if (false == (hr == HRESULT.S_OK || hr == HRESULT.EInvalidarg))
            {
                throw new COMException("GetParsingName", (int)hr);
            }

            if (pszPath != IntPtr.Zero)
            {
                path = Marshal.PtrToStringAuto(pszPath);
                Marshal.FreeCoTaskMem(pszPath);
            }

            return path;
        }

        /// <summary>
        /// </summary>
        /// <param name="name">
        /// </param>
        /// <returns>
        /// </returns>
        internal static IntPtr PidlFromParsingName(string name)
        {
            IntPtr pidl;

            ShellNativeMethods.SFGAOs sfgao;
            var retCode = ShellNativeMethods.SHParseDisplayName(name, IntPtr.Zero, out pidl, 0, out sfgao);

            return CoreErrorHelper.Succeeded(retCode) ? pidl : IntPtr.Zero;
        }

        /// <summary>
        /// </summary>
        /// <param name="nativeShellItem">
        /// </param>
        /// <returns>
        /// </returns>
        internal static IntPtr PidlFromShellItem(IShellItem nativeShellItem)
        {
            var shellItem = Marshal.GetComInterfaceForObject(nativeShellItem, typeof(IShellItem));
            IntPtr pidl;

            var retCode = ShellNativeMethods.SHGetIDListFromObject(shellItem, out pidl);

            return CoreErrorHelper.Succeeded(retCode) ? pidl : IntPtr.Zero;
        }

        #endregion
    }
}