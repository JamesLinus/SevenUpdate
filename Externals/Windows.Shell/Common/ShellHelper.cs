//Copyright (c) Microsoft Corporation.  All rights reserved.
//Modified by Robert Baker, Seven Software 2010.

#region

using System;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Windows.Internal;
using Microsoft.Windows.Shell.PropertySystem;

#endregion

namespace Microsoft.Windows.Shell
{
    /// <summary>
    ///   A helper class for Shell Objects
    /// </summary>
    internal static class ShellHelper
    {
        private static PropertyKey itemTypePropertyKey = new PropertyKey(new Guid("28636AA6-953D-11D2-B5D6-00C04FD918D0"), 11);

        internal static string GetParsingName(IShellItem shellItem)
        {
            if (shellItem == null)
                return null;

            string path = null;

            IntPtr pszPath;
            var hr = shellItem.GetDisplayName(ShellNativeMethods.SIGDN.SIGDN_DESKTOPABSOLUTEPARSING, out pszPath);

            if (false == (hr == HRESULT.S_OK || hr == HRESULT.E_INVALIDARG))
                throw new COMException("GetParsingName", (int) hr);

            if (pszPath != IntPtr.Zero)
            {
                path = Marshal.PtrToStringAuto(pszPath);
                Marshal.FreeCoTaskMem(pszPath);
            }

            return path;
        }

        internal static string GetAbsolutePath(string path)
        {
            return Uri.IsWellFormedUriString(path, UriKind.Absolute) ? path : Path.GetFullPath((path));
        }

        internal static string GetItemType(IShellItem2 shellItem)
        {
            if (shellItem != null)
            {
                string itemType;

                var hr = shellItem.GetString(ref itemTypePropertyKey, out itemType);

                if (hr == HRESULT.S_OK)
                    return itemType;
            }

            return null;
        }

        internal static IntPtr PidlFromParsingName(string name)
        {
            IntPtr pidl;

            ShellNativeMethods.SFGAO sfgao;
            var retCode = ShellNativeMethods.SHParseDisplayName(name, IntPtr.Zero, out pidl, 0, out sfgao);

            return (CoreErrorHelper.Succeeded(retCode) ? pidl : IntPtr.Zero);
        }

        internal static IntPtr PidlFromShellItem(IShellItem nativeShellItem)
        {
            var shellItem = Marshal.GetComInterfaceForObject(nativeShellItem, typeof (IShellItem));
            IntPtr pidl;

            var retCode = ShellNativeMethods.SHGetIDListFromObject(shellItem, out pidl);

            return (CoreErrorHelper.Succeeded(retCode) ? pidl : IntPtr.Zero);
        }
    }
}