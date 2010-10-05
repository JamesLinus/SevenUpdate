//***********************************************************************
// Assembly         : Windows.Shell
// Author           : sevenalive
// Created          : 09-17-2010
// Last Modified By : sevenalive
// Last Modified On : 10-05-2010
// Description      : 
// Copyright        : (c) Seven Software. All rights reserved.
//***********************************************************************

namespace Microsoft.Windows.Dialogs
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    using Microsoft.Windows.Internal;
    using Microsoft.Windows.Shell;
    using Microsoft.Windows.Shell.PropertySystem;

    // Disable warning if a method declaration hides another inherited from a parent COM interface
    // To successfully import a COM interface, all inherited methods need to be declared again with 
    // the exception of those already declared in "IUnknown"
#pragma warning disable 0108

    /// <summary>
    /// </summary>
    [ComImport]
    [Guid(ShellIidGuid.IFileDialog)]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IFileDialog : IModalWindow
    {
        // Defined on IModalWindow - repeated here due to requirements of COM interop layer.
        /// <summary>
        /// </summary>
        /// <param name="parent">
        /// </param>
        /// <returns>
        /// </returns>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        [PreserveSig]
        int Show([In] IntPtr parent);

        // IFileDialog-Specific interface members.

        /// <summary>
        /// </summary>
        /// <param name="cFileTypes">
        /// </param>
        /// <param name="rgFilterSpec">
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void SetFileTypes([In] uint cFileTypes, [In] [MarshalAs(UnmanagedType.LPArray)] ShellNativeMethods.OmdlgFilterspec[] rgFilterSpec);

        /// <summary>
        /// </summary>
        /// <param name="iFileType">
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void SetFileTypeIndex([In] uint iFileType);

        /// <summary>
        /// </summary>
        /// <param name="piFileType">
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetFileTypeIndex(out uint piFileType);

        /// <summary>
        /// </summary>
        /// <param name="pfde">
        /// </param>
        /// <param name="pdwCookie">
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void Advise([In] [MarshalAs(UnmanagedType.Interface)] IFileDialogEvents pfde, out uint pdwCookie);

        /// <summary>
        /// </summary>
        /// <param name="dwCookie">
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void Unadvise([In] uint dwCookie);

        /// <summary>
        /// </summary>
        /// <param name="fos">
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void SetOptions([In] ShellNativeMethods.FOSs fos);

        /// <summary>
        /// </summary>
        /// <param name="pfos">
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetOptions(out ShellNativeMethods.FOSs pfos);

        /// <summary>
        /// </summary>
        /// <param name="psi">
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void SetDefaultFolder([In] [MarshalAs(UnmanagedType.Interface)] IShellItem psi);

        /// <summary>
        /// </summary>
        /// <param name="psi">
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void SetFolder([In] [MarshalAs(UnmanagedType.Interface)] IShellItem psi);

        /// <summary>
        /// </summary>
        /// <param name="ppsi">
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetFolder([MarshalAs(UnmanagedType.Interface)] out IShellItem ppsi);

        /// <summary>
        /// </summary>
        /// <param name="ppsi">
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetCurrentSelection([MarshalAs(UnmanagedType.Interface)] out IShellItem ppsi);

        /// <summary>
        /// </summary>
        /// <param name="pszName">
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void SetFileName([In] [MarshalAs(UnmanagedType.LPWStr)] string pszName);

        /// <summary>
        /// </summary>
        /// <param name="pszName">
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetFileName([MarshalAs(UnmanagedType.LPWStr)] out string pszName);

        /// <summary>
        /// </summary>
        /// <param name="pszTitle">
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void SetTitle([In] [MarshalAs(UnmanagedType.LPWStr)] string pszTitle);

        /// <summary>
        /// </summary>
        /// <param name="pszText">
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void SetOkButtonLabel([In] [MarshalAs(UnmanagedType.LPWStr)] string pszText);

        /// <summary>
        /// </summary>
        /// <param name="pszLabel">
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void SetFileNameLabel([In] [MarshalAs(UnmanagedType.LPWStr)] string pszLabel);

        /// <summary>
        /// </summary>
        /// <param name="ppsi">
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetResult([MarshalAs(UnmanagedType.Interface)] out IShellItem ppsi);

        /// <summary>
        /// </summary>
        /// <param name="psi">
        /// </param>
        /// <param name="fdap">
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void AddPlace([In] [MarshalAs(UnmanagedType.Interface)] IShellItem psi, ShellNativeMethods.FDAP fdap);

        /// <summary>
        /// </summary>
        /// <param name="pszDefaultExtension">
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void SetDefaultExtension([In] [MarshalAs(UnmanagedType.LPWStr)] string pszDefaultExtension);

        /// <summary>
        /// </summary>
        /// <param name="hr">
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void Close([MarshalAs(UnmanagedType.Error)] int hr);

        /// <summary>
        /// </summary>
        /// <param name="guid">
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void SetClientGuid([In] ref Guid guid);

        /// <summary>
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void ClearClientData();

        // Not supported:  IShellItemFilter is not defined, converting to IntPtr.
        /// <summary>
        /// </summary>
        /// <param name="pFilter">
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void SetFilter([MarshalAs(UnmanagedType.Interface)] IntPtr pFilter);
    }

    /// <summary>
    /// </summary>
    [ComImport]
    [Guid(ShellIidGuid.IFileOpenDialog)]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IFileOpenDialog : IFileDialog
    {
        // Defined on IModalWindow - repeated here due to requirements of COM interop layer.
        /// <summary>
        /// </summary>
        /// <param name="parent">
        /// </param>
        /// <returns>
        /// </returns>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        [PreserveSig]
        int Show([In] IntPtr parent);

        // Defined on IFileDialog - repeated here due to requirements of COM interop layer.
        /// <summary>
        /// </summary>
        /// <param name="cFileTypes">
        /// </param>
        /// <param name="rgFilterSpec">
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void SetFileTypes([In] uint cFileTypes, [In] ref ShellNativeMethods.OmdlgFilterspec rgFilterSpec);

        /// <summary>
        /// </summary>
        /// <param name="iFileType">
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void SetFileTypeIndex([In] uint iFileType);

        /// <summary>
        /// </summary>
        /// <param name="piFileType">
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetFileTypeIndex(out uint piFileType);

        /// <summary>
        /// </summary>
        /// <param name="pfde">
        /// </param>
        /// <param name="pdwCookie">
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void Advise([In] [MarshalAs(UnmanagedType.Interface)] IFileDialogEvents pfde, out uint pdwCookie);

        /// <summary>
        /// </summary>
        /// <param name="dwCookie">
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void Unadvise([In] uint dwCookie);

        /// <summary>
        /// </summary>
        /// <param name="fos">
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void SetOptions([In] ShellNativeMethods.FOSs fos);

        /// <summary>
        /// </summary>
        /// <param name="pfos">
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetOptions(out ShellNativeMethods.FOSs pfos);

        /// <summary>
        /// </summary>
        /// <param name="psi">
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void SetDefaultFolder([In] [MarshalAs(UnmanagedType.Interface)] IShellItem psi);

        /// <summary>
        /// </summary>
        /// <param name="psi">
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void SetFolder([In] [MarshalAs(UnmanagedType.Interface)] IShellItem psi);

        /// <summary>
        /// </summary>
        /// <param name="ppsi">
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetFolder([MarshalAs(UnmanagedType.Interface)] out IShellItem ppsi);

        /// <summary>
        /// </summary>
        /// <param name="ppsi">
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetCurrentSelection([MarshalAs(UnmanagedType.Interface)] out IShellItem ppsi);

        /// <summary>
        /// </summary>
        /// <param name="pszName">
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void SetFileName([In] [MarshalAs(UnmanagedType.LPWStr)] string pszName);

        /// <summary>
        /// </summary>
        /// <param name="pszName">
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetFileName([MarshalAs(UnmanagedType.LPWStr)] out string pszName);

        /// <summary>
        /// </summary>
        /// <param name="pszTitle">
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void SetTitle([In] [MarshalAs(UnmanagedType.LPWStr)] string pszTitle);

        /// <summary>
        /// </summary>
        /// <param name="pszText">
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void SetOkButtonLabel([In] [MarshalAs(UnmanagedType.LPWStr)] string pszText);

        /// <summary>
        /// </summary>
        /// <param name="pszLabel">
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void SetFileNameLabel([In] [MarshalAs(UnmanagedType.LPWStr)] string pszLabel);

        /// <summary>
        /// </summary>
        /// <param name="ppsi">
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetResult([MarshalAs(UnmanagedType.Interface)] out IShellItem ppsi);

        /// <summary>
        /// </summary>
        /// <param name="psi">
        /// </param>
        /// <param name="fdap">
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void AddPlace([In] [MarshalAs(UnmanagedType.Interface)] IShellItem psi, ShellNativeMethods.FDAP fdap);

        /// <summary>
        /// </summary>
        /// <param name="pszDefaultExtension">
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void SetDefaultExtension([In] [MarshalAs(UnmanagedType.LPWStr)] string pszDefaultExtension);

        /// <summary>
        /// </summary>
        /// <param name="hr">
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void Close([MarshalAs(UnmanagedType.Error)] int hr);

        /// <summary>
        /// </summary>
        /// <param name="guid">
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void SetClientGuid([In] ref Guid guid);

        /// <summary>
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void ClearClientData();

        // Not supported:  IShellItemFilter is not defined, converting to IntPtr.
        /// <summary>
        /// </summary>
        /// <param name="pFilter">
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void SetFilter([MarshalAs(UnmanagedType.Interface)] IntPtr pFilter);

        // Defined by IFileOpenDialog.
        /// <summary>
        /// </summary>
        /// <param name="ppenum">
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetResults([MarshalAs(UnmanagedType.Interface)] out IShellItemArray ppenum);

        /// <summary>
        /// </summary>
        /// <param name="ppsai">
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetSelectedItems([MarshalAs(UnmanagedType.Interface)] out IShellItemArray ppsai);
    }

    /// <summary>
    /// </summary>
    [ComImport]
    [Guid(ShellIidGuid.IFileSaveDialog)]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IFileSaveDialog : IFileDialog
    {
        // Defined on IModalWindow - repeated here due to requirements of COM interop layer.
        /// <summary>
        /// </summary>
        /// <param name="parent">
        /// </param>
        /// <returns>
        /// </returns>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        [PreserveSig]
        int Show([In] IntPtr parent);

        // Defined on IFileDialog - repeated here due to requirements of COM interop layer.
        /// <summary>
        /// </summary>
        /// <param name="cFileTypes">
        /// </param>
        /// <param name="rgFilterSpec">
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void SetFileTypes([In] uint cFileTypes, [In] ref ShellNativeMethods.OmdlgFilterspec rgFilterSpec);

        /// <summary>
        /// </summary>
        /// <param name="iFileType">
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void SetFileTypeIndex([In] uint iFileType);

        /// <summary>
        /// </summary>
        /// <param name="piFileType">
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetFileTypeIndex(out uint piFileType);

        /// <summary>
        /// </summary>
        /// <param name="pfde">
        /// </param>
        /// <param name="pdwCookie">
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void Advise([In] [MarshalAs(UnmanagedType.Interface)] IFileDialogEvents pfde, out uint pdwCookie);

        /// <summary>
        /// </summary>
        /// <param name="dwCookie">
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void Unadvise([In] uint dwCookie);

        /// <summary>
        /// </summary>
        /// <param name="fos">
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void SetOptions([In] ShellNativeMethods.FOSs fos);

        /// <summary>
        /// </summary>
        /// <param name="pfos">
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetOptions(out ShellNativeMethods.FOSs pfos);

        /// <summary>
        /// </summary>
        /// <param name="psi">
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void SetDefaultFolder([In] [MarshalAs(UnmanagedType.Interface)] IShellItem psi);

        /// <summary>
        /// </summary>
        /// <param name="psi">
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void SetFolder([In] [MarshalAs(UnmanagedType.Interface)] IShellItem psi);

        /// <summary>
        /// </summary>
        /// <param name="ppsi">
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetFolder([MarshalAs(UnmanagedType.Interface)] out IShellItem ppsi);

        /// <summary>
        /// </summary>
        /// <param name="ppsi">
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetCurrentSelection([MarshalAs(UnmanagedType.Interface)] out IShellItem ppsi);

        /// <summary>
        /// </summary>
        /// <param name="pszName">
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void SetFileName([In] [MarshalAs(UnmanagedType.LPWStr)] string pszName);

        /// <summary>
        /// </summary>
        /// <param name="pszName">
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetFileName([MarshalAs(UnmanagedType.LPWStr)] out string pszName);

        /// <summary>
        /// </summary>
        /// <param name="pszTitle">
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void SetTitle([In] [MarshalAs(UnmanagedType.LPWStr)] string pszTitle);

        /// <summary>
        /// </summary>
        /// <param name="pszText">
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void SetOkButtonLabel([In] [MarshalAs(UnmanagedType.LPWStr)] string pszText);

        /// <summary>
        /// </summary>
        /// <param name="pszLabel">
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void SetFileNameLabel([In] [MarshalAs(UnmanagedType.LPWStr)] string pszLabel);

        /// <summary>
        /// </summary>
        /// <param name="ppsi">
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetResult([MarshalAs(UnmanagedType.Interface)] out IShellItem ppsi);

        /// <summary>
        /// </summary>
        /// <param name="psi">
        /// </param>
        /// <param name="fdap">
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void AddPlace([In] [MarshalAs(UnmanagedType.Interface)] IShellItem psi, ShellNativeMethods.FDAP fdap);

        /// <summary>
        /// </summary>
        /// <param name="pszDefaultExtension">
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void SetDefaultExtension([In] [MarshalAs(UnmanagedType.LPWStr)] string pszDefaultExtension);

        /// <summary>
        /// </summary>
        /// <param name="hr">
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void Close([MarshalAs(UnmanagedType.Error)] int hr);

        /// <summary>
        /// </summary>
        /// <param name="guid">
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void SetClientGuid([In] ref Guid guid);

        /// <summary>
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void ClearClientData();

        // Not supported:  IShellItemFilter is not defined, converting to IntPtr.
        /// <summary>
        /// </summary>
        /// <param name="pFilter">
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void SetFilter([MarshalAs(UnmanagedType.Interface)] IntPtr pFilter);

        // Defined by IFileSaveDialog interface.

        /// <summary>
        /// </summary>
        /// <param name="psi">
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void SetSaveAsItem([In] [MarshalAs(UnmanagedType.Interface)] IShellItem psi);

        // Not currently supported: IPropertyStore.
        /// <summary>
        /// </summary>
        /// <param name="pStore">
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void SetProperties([In] [MarshalAs(UnmanagedType.Interface)] IntPtr pStore);

        /// <summary>
        /// </summary>
        /// <param name="pList">
        /// </param>
        /// <param name="fAppendDefault">
        /// </param>
        /// <returns>
        /// </returns>
        [PreserveSig]
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        int SetCollectedProperties([In] IPropertyDescriptionList pList, [In] bool fAppendDefault);

        /// <summary>
        /// </summary>
        /// <param name="ppStore">
        /// </param>
        /// <returns>
        /// </returns>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        [PreserveSig]
        HRESULT GetProperties(out IPropertyStore ppStore);

        // Not currently supported: IPropertyStore, IFileOperationProgressSink.
        /// <summary>
        /// </summary>
        /// <param name="psi">
        /// </param>
        /// <param name="pStore">
        /// </param>
        /// <param name="hwnd">
        /// </param>
        /// <param name="pSink">
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void ApplyProperties(
            [In] [MarshalAs(UnmanagedType.Interface)] IShellItem psi, 
            [In] [MarshalAs(UnmanagedType.Interface)] IntPtr pStore, 
            [In] [ComAliasName("ShellObjects.wireHWND")] ref IntPtr hwnd, 
            [In] [MarshalAs(UnmanagedType.Interface)] IntPtr pSink);
    }

    /// <summary>
    /// </summary>
    [ComImport]
    [Guid(ShellIidGuid.IFileDialogEvents)]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IFileDialogEvents
    {
        // NOTE: some of these callbacks are cancelable - returning S_FALSE means that 
        // the dialog should not proceed (e.g. with closing, changing folder); to 
        // support this, we need to use the PreserveSig attribute to enable us to return
        // the proper HRESULT.

        /// <summary>
        /// </summary>
        /// <param name="pfd">
        /// </param>
        /// <returns>
        /// </returns>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        [PreserveSig]
        HRESULT OnFileOk([In] [MarshalAs(UnmanagedType.Interface)] IFileDialog pfd);

        /// <summary>
        /// </summary>
        /// <param name="pfd">
        /// </param>
        /// <param name="psiFolder">
        /// </param>
        /// <returns>
        /// </returns>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        [PreserveSig]
        HRESULT OnFolderChanging([In] [MarshalAs(UnmanagedType.Interface)] IFileDialog pfd, [In] [MarshalAs(UnmanagedType.Interface)] IShellItem psiFolder);

        /// <summary>
        /// </summary>
        /// <param name="pfd">
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void OnFolderChange([In] [MarshalAs(UnmanagedType.Interface)] IFileDialog pfd);

        /// <summary>
        /// </summary>
        /// <param name="pfd">
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void OnSelectionChange([In] [MarshalAs(UnmanagedType.Interface)] IFileDialog pfd);

        /// <summary>
        /// </summary>
        /// <param name="pfd">
        /// </param>
        /// <param name="psi">
        /// </param>
        /// <param name="pResponse">
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void OnShareViolation(
            [In] [MarshalAs(UnmanagedType.Interface)] IFileDialog pfd, 
            [In] [MarshalAs(UnmanagedType.Interface)] IShellItem psi, 
            out ShellNativeMethods.FdeShareviolationResponse pResponse);

        /// <summary>
        /// </summary>
        /// <param name="pfd">
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void OnTypeChange([In] [MarshalAs(UnmanagedType.Interface)] IFileDialog pfd);

        /// <summary>
        /// </summary>
        /// <param name="pfd">
        /// </param>
        /// <param name="psi">
        /// </param>
        /// <param name="pResponse">
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void OnOverwrite(
            [In] [MarshalAs(UnmanagedType.Interface)] IFileDialog pfd, 
            [In] [MarshalAs(UnmanagedType.Interface)] IShellItem psi, 
            out ShellNativeMethods.FdeOverwriteResponse pResponse);
    }

    /// <summary>
    /// </summary>
    [ComImport]
    [Guid(ShellIidGuid.IFileDialogCustomize)]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IFileDialogCustomize
    {
        /// <summary>
        /// </summary>
        /// <param name="dwIDCtl">
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void EnableOpenDropDown([In] int dwIDCtl);

        /// <summary>
        /// </summary>
        /// <param name="dwIDCtl">
        /// </param>
        /// <param name="pszLabel">
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void AddMenu([In] int dwIDCtl, [In] [MarshalAs(UnmanagedType.LPWStr)] string pszLabel);

        /// <summary>
        /// </summary>
        /// <param name="dwIDCtl">
        /// </param>
        /// <param name="pszLabel">
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void AddPushButton([In] int dwIDCtl, [In] [MarshalAs(UnmanagedType.LPWStr)] string pszLabel);

        /// <summary>
        /// </summary>
        /// <param name="dwIDCtl">
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void AddComboBox([In] int dwIDCtl);

        /// <summary>
        /// </summary>
        /// <param name="dwIDCtl">
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void AddRadioButtonList([In] int dwIDCtl);

        /// <summary>
        /// </summary>
        /// <param name="dwIDCtl">
        /// </param>
        /// <param name="pszLabel">
        /// </param>
        /// <param name="bChecked">
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void AddCheckButton([In] int dwIDCtl, [In] [MarshalAs(UnmanagedType.LPWStr)] string pszLabel, [In] bool bChecked);

        /// <summary>
        /// </summary>
        /// <param name="dwIDCtl">
        /// </param>
        /// <param name="pszText">
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void AddEditBox([In] int dwIDCtl, [In] [MarshalAs(UnmanagedType.LPWStr)] string pszText);

        /// <summary>
        /// </summary>
        /// <param name="dwIDCtl">
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void AddSeparator([In] int dwIDCtl);

        /// <summary>
        /// </summary>
        /// <param name="dwIDCtl">
        /// </param>
        /// <param name="pszText">
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void AddText([In] int dwIDCtl, [In] [MarshalAs(UnmanagedType.LPWStr)] string pszText);

        /// <summary>
        /// </summary>
        /// <param name="dwIDCtl">
        /// </param>
        /// <param name="pszLabel">
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void SetControlLabel([In] int dwIDCtl, [In] [MarshalAs(UnmanagedType.LPWStr)] string pszLabel);

        /// <summary>
        /// </summary>
        /// <param name="dwIDCtl">
        /// </param>
        /// <param name="pdwState">
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetControlState([In] int dwIDCtl, [Out] out ShellNativeMethods.DCONTROLSTATE pdwState);

        /// <summary>
        /// </summary>
        /// <param name="dwIDCtl">
        /// </param>
        /// <param name="dwState">
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void SetControlState([In] int dwIDCtl, [In] ShellNativeMethods.DCONTROLSTATE dwState);

        /// <summary>
        /// </summary>
        /// <param name="dwIDCtl">
        /// </param>
        /// <param name="ppszText">
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetEditBoxText([In] int dwIDCtl, [MarshalAs(UnmanagedType.LPWStr)] out string ppszText);

        /// <summary>
        /// </summary>
        /// <param name="dwIDCtl">
        /// </param>
        /// <param name="pszText">
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void SetEditBoxText([In] int dwIDCtl, [In] [MarshalAs(UnmanagedType.LPWStr)] string pszText);

        /// <summary>
        /// </summary>
        /// <param name="dwIDCtl">
        /// </param>
        /// <param name="pbChecked">
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetCheckButtonState([In] int dwIDCtl, [Out] out bool pbChecked);

        /// <summary>
        /// </summary>
        /// <param name="dwIDCtl">
        /// </param>
        /// <param name="bChecked">
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void SetCheckButtonState([In] int dwIDCtl, [In] bool bChecked);

        /// <summary>
        /// </summary>
        /// <param name="dwIDCtl">
        /// </param>
        /// <param name="dwIDItem">
        /// </param>
        /// <param name="pszLabel">
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void AddControlItem([In] int dwIDCtl, [In] int dwIDItem, [In] [MarshalAs(UnmanagedType.LPWStr)] string pszLabel);

        /// <summary>
        /// </summary>
        /// <param name="dwIDCtl">
        /// </param>
        /// <param name="dwIDItem">
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void RemoveControlItem([In] int dwIDCtl, [In] int dwIDItem);

        /// <summary>
        /// </summary>
        /// <param name="dwIDCtl">
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void RemoveAllControlItems([In] int dwIDCtl);

        /// <summary>
        /// </summary>
        /// <param name="dwIDCtl">
        /// </param>
        /// <param name="dwIDItem">
        /// </param>
        /// <param name="pdwState">
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetControlItemState([In] int dwIDCtl, [In] int dwIDItem, [Out] out ShellNativeMethods.DCONTROLSTATE pdwState);

        /// <summary>
        /// </summary>
        /// <param name="dwIDCtl">
        /// </param>
        /// <param name="dwIDItem">
        /// </param>
        /// <param name="dwState">
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void SetControlItemState([In] int dwIDCtl, [In] int dwIDItem, [In] ShellNativeMethods.DCONTROLSTATE dwState);

        /// <summary>
        /// </summary>
        /// <param name="dwIDCtl">
        /// </param>
        /// <param name="pdwIDItem">
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetSelectedControlItem([In] int dwIDCtl, [Out] out int pdwIDItem);

        /// <summary>
        /// </summary>
        /// <param name="dwIDCtl">
        /// </param>
        /// <param name="dwIDItem">
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void SetSelectedControlItem([In] int dwIDCtl, [In] int dwIDItem);

        // Not valid for OpenDropDown.

        /// <summary>
        /// </summary>
        /// <param name="dwIDCtl">
        /// </param>
        /// <param name="pszLabel">
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void StartVisualGroup([In] int dwIDCtl, [In] [MarshalAs(UnmanagedType.LPWStr)] string pszLabel);

        /// <summary>
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void EndVisualGroup();

        /// <summary>
        /// </summary>
        /// <param name="dwIDCtl">
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void MakeProminent([In] int dwIDCtl);
    }

    /// <summary>
    /// </summary>
    [ComImport]
    [Guid(ShellIidGuid.IFileDialogControlEvents)]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IFileDialogControlEvents
    {
        /// <summary>
        /// </summary>
        /// <param name="pfdc">
        /// </param>
        /// <param name="dwIDCtl">
        /// </param>
        /// <param name="dwIDItem">
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void OnItemSelected([In] [MarshalAs(UnmanagedType.Interface)] IFileDialogCustomize pfdc, [In] int dwIDCtl, [In] int dwIDItem);

        /// <summary>
        /// </summary>
        /// <param name="pfdc">
        /// </param>
        /// <param name="dwIDCtl">
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void OnButtonClicked([In] [MarshalAs(UnmanagedType.Interface)] IFileDialogCustomize pfdc, [In] int dwIDCtl);

        /// <summary>
        /// </summary>
        /// <param name="pfdc">
        /// </param>
        /// <param name="dwIDCtl">
        /// </param>
        /// <param name="bChecked">
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void OnCheckButtonToggled([In] [MarshalAs(UnmanagedType.Interface)] IFileDialogCustomize pfdc, [In] int dwIDCtl, [In] bool bChecked);

        /// <summary>
        /// </summary>
        /// <param name="pfdc">
        /// </param>
        /// <param name="dwIDCtl">
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void OnControlActivating([In] [MarshalAs(UnmanagedType.Interface)] IFileDialogCustomize pfdc, [In] int dwIDCtl);
    }

    // Restore the warning
#pragma warning restore 0108
}