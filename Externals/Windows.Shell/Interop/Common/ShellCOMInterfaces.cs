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
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Runtime.InteropServices.ComTypes;
    using System.Text;

    using Microsoft.Windows.Internal;
    using Microsoft.Windows.Shell.PropertySystem;

    using FILETIME = System.Runtime.InteropServices.ComTypes.FILETIME;

    /// <summary>
    /// </summary>
    internal enum SICHINTF : uint
    {
        /// <summary>
        /// </summary>
        SichintDisplay = 0x00000000, 

        /// <summary>
        /// </summary>
        SichintCanonical = 0x10000000, 

        /// <summary>
        /// </summary>
        SichintTestFilesyspathIFNotEqual = 0x20000000, 

        /// <summary>
        /// </summary>
        SichintAllfields = 0x80000000
    }

    // Disable warning if a method declaration hides another inherited from a parent COM interface
    // To successfully import a COM interface, all inherited methods need to be declared again with 
    // the exception of those already declared in "IUnknown"
#pragma warning disable 108

    #region COM Interfaces

    /// <summary>
    /// </summary>
    [ComImport]
    [Guid(ShellIidGuid.IModalWindow)]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IModalWindow
    {
        /// <summary>
        /// </summary>
        /// <param name="parent">
        /// </param>
        /// <returns>
        /// </returns>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        [PreserveSig]
        int Show([In] IntPtr parent);
    }

    /// <summary>
    /// </summary>
    [ComImport]
    [Guid(ShellIidGuid.IShellItem)]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IShellItem
    {
        // Not supported: IBindCtx.
        /// <summary>
        /// </summary>
        /// <param name="pbc">
        /// </param>
        /// <param name="bhid">
        /// </param>
        /// <param name="riid">
        /// </param>
        /// <param name="ppv">
        /// </param>
        /// <returns>
        /// </returns>
        [PreserveSig]
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        HRESULT BindToHandler([In] IntPtr pbc, [In] ref Guid bhid, [In] ref Guid riid, [Out] [MarshalAs(UnmanagedType.Interface)] out IShellFolder ppv);

        /// <summary>
        /// </summary>
        /// <param name="ppsi">
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetParent([MarshalAs(UnmanagedType.Interface)] out IShellItem ppsi);

        /// <summary>
        /// </summary>
        /// <param name="sigdnName">
        /// </param>
        /// <param name="ppszName">
        /// </param>
        /// <returns>
        /// </returns>
        [PreserveSig]
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        HRESULT GetDisplayName([In] ShellNativeMethods.SIGDN sigdnName, out IntPtr ppszName);

        /// <summary>
        /// </summary>
        /// <param name="sfgaoMask">
        /// </param>
        /// <param name="psfgaoAttribs">
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetAttributes([In] ShellNativeMethods.SFGAOs sfgaoMask, out ShellNativeMethods.SFGAOs psfgaoAttribs);

        /// <summary>
        /// </summary>
        /// <param name="psi">
        /// </param>
        /// <param name="hint">
        /// </param>
        /// <param name="piOrder">
        /// </param>
        /// <returns>
        /// </returns>
        [PreserveSig]
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        HRESULT Compare([In] [MarshalAs(UnmanagedType.Interface)] IShellItem psi, [In] SICHINTF hint, out int piOrder);
    }

    /// <summary>
    /// </summary>
    [ComImport]
    [Guid(ShellIidGuid.IShellItem2)]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IShellItem2 : IShellItem
    {
        // Not supported: IBindCtx.
        /// <summary>
        /// </summary>
        /// <param name="pbc">
        /// </param>
        /// <param name="bhid">
        /// </param>
        /// <param name="riid">
        /// </param>
        /// <param name="ppv">
        /// </param>
        /// <returns>
        /// </returns>
        [PreserveSig]
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        HRESULT BindToHandler([In] IntPtr pbc, [In] ref Guid bhid, [In] ref Guid riid, [Out] [MarshalAs(UnmanagedType.Interface)] out IShellFolder ppv);

        /// <summary>
        /// </summary>
        /// <param name="ppsi">
        /// </param>
        /// <returns>
        /// </returns>
        [PreserveSig]
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        HRESULT GetParent([MarshalAs(UnmanagedType.Interface)] out IShellItem ppsi);

        /// <summary>
        /// </summary>
        /// <param name="sigdnName">
        /// </param>
        /// <param name="ppszName">
        /// </param>
        /// <returns>
        /// </returns>
        [PreserveSig]
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        HRESULT GetDisplayName([In] ShellNativeMethods.SIGDN sigdnName, [MarshalAs(UnmanagedType.LPWStr)] out string ppszName);

        /// <summary>
        /// </summary>
        /// <param name="sfgaoMask">
        /// </param>
        /// <param name="psfgaoAttribs">
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetAttributes([In] ShellNativeMethods.SFGAOs sfgaoMask, out ShellNativeMethods.SFGAOs psfgaoAttribs);

        /// <summary>
        /// </summary>
        /// <param name="psi">
        /// </param>
        /// <param name="hint">
        /// </param>
        /// <param name="piOrder">
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void Compare([In] [MarshalAs(UnmanagedType.Interface)] IShellItem psi, [In] uint hint, out int piOrder);

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
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        [PreserveSig]
        int GetPropertyStore([In] ShellNativeMethods.GETPROPERTYSTOREFLAGSs flags, [In] ref Guid riid, [Out] [MarshalAs(UnmanagedType.Interface)] out IPropertyStore ppv);

        /// <summary>
        /// </summary>
        /// <param name="flags">
        /// </param>
        /// <param name="punkCreate">
        /// </param>
        /// <param name="riid">
        /// </param>
        /// <param name="ppv">
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetPropertyStoreWithCreateObject(
            [In] ShellNativeMethods.GETPROPERTYSTOREFLAGSs flags, [In] [MarshalAs(UnmanagedType.IUnknown)] object punkCreate, [In] ref Guid riid, out IntPtr ppv);

        /// <summary>
        /// </summary>
        /// <param name="rgKeys">
        /// </param>
        /// <param name="cKeys">
        /// </param>
        /// <param name="flags">
        /// </param>
        /// <param name="riid">
        /// </param>
        /// <param name="ppv">
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetPropertyStoreForKeys(
            [In] ref PropertyKey rgKeys, 
            [In] uint cKeys, 
            [In] ShellNativeMethods.GETPROPERTYSTOREFLAGSs flags, 
            [In] ref Guid riid, 
            [Out] [MarshalAs(UnmanagedType.IUnknown)] out IPropertyStore ppv);

        /// <summary>
        /// </summary>
        /// <param name="keyType">
        /// </param>
        /// <param name="riid">
        /// </param>
        /// <param name="ppv">
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetPropertyDescriptionList([In] ref PropertyKey keyType, [In] ref Guid riid, out IntPtr ppv);

        /// <summary>
        /// </summary>
        /// <param name="pbc">
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void Update([In] [MarshalAs(UnmanagedType.Interface)] IBindCtx pbc);

        /// <summary>
        /// </summary>
        /// <param name="key">
        /// </param>
        /// <param name="ppropvar">
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetProperty([In] ref PropertyKey key, out PropVariant ppropvar);

        /// <summary>
        /// </summary>
        /// <param name="key">
        /// </param>
        /// <param name="pclsid">
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetClsid([In] ref PropertyKey key, out Guid pclsid);

        /// <summary>
        /// </summary>
        /// <param name="key">
        /// </param>
        /// <param name="pft">
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetFileTime([In] ref PropertyKey key, out FILETIME pft);

        /// <summary>
        /// </summary>
        /// <param name="key">
        /// </param>
        /// <param name="pi">
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetInt32([In] ref PropertyKey key, out int pi);

        /// <summary>
        /// </summary>
        /// <param name="key">
        /// </param>
        /// <param name="ppsz">
        /// </param>
        /// <returns>
        /// </returns>
        [PreserveSig]
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        HRESULT GetString([In] ref PropertyKey key, [MarshalAs(UnmanagedType.LPWStr)] out string ppsz);

        /// <summary>
        /// </summary>
        /// <param name="key">
        /// </param>
        /// <param name="pui">
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetUInt32([In] ref PropertyKey key, out uint pui);

        /// <summary>
        /// </summary>
        /// <param name="key">
        /// </param>
        /// <param name="pull">
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetUInt64([In] ref PropertyKey key, out ulong pull);

        /// <summary>
        /// </summary>
        /// <param name="key">
        /// </param>
        /// <param name="pf">
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetBool([In] ref PropertyKey key, out int pf);
    }

    /// <summary>
    /// </summary>
    [ComImport]
    [Guid(ShellIidGuid.IShellItemArray)]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IShellItemArray
    {
        // Not supported: IBindCtx.
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
        [PreserveSig]
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        HRESULT BindToHandler([In] [MarshalAs(UnmanagedType.Interface)] IntPtr pbc, [In] ref Guid rbhid, [In] ref Guid riid, out IntPtr ppvOut);

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
        [PreserveSig]
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        HRESULT GetPropertyStore([In] int flags, [In] ref Guid riid, out IntPtr ppv);

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
        [PreserveSig]
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        HRESULT GetPropertyDescriptionList([In] ref PropertyKey keyType, [In] ref Guid riid, out IntPtr ppv);

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
        [PreserveSig]
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        HRESULT GetAttributes([In] ShellNativeMethods.SIATTRIBFLAGS dwAttribFlags, [In] ShellNativeMethods.SFGAOs sfgaoMask, out ShellNativeMethods.SFGAOs psfgaoAttribs);

        /// <summary>
        /// </summary>
        /// <param name="pdwNumItems">
        /// </param>
        /// <returns>
        /// </returns>
        [PreserveSig]
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        HRESULT GetCount(out uint pdwNumItems);

        /// <summary>
        /// </summary>
        /// <param name="dwIndex">
        /// </param>
        /// <param name="ppsi">
        /// </param>
        /// <returns>
        /// </returns>
        [PreserveSig]
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        HRESULT GetItemAt([In] uint dwIndex, [MarshalAs(UnmanagedType.Interface)] out IShellItem ppsi);

        // Not supported: IEnumShellItems (will use GetCount and GetItemAt instead).
        /// <summary>
        /// </summary>
        /// <param name="ppenumShellItems">
        /// </param>
        /// <returns>
        /// </returns>
        [PreserveSig]
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        HRESULT EnumItems([MarshalAs(UnmanagedType.Interface)] out IntPtr ppenumShellItems);
    }

    /// <summary>
    /// </summary>
    [ComImport]
    [Guid(ShellIidGuid.IShellLibrary)]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IShellLibrary
    {
        /// <summary>
        /// </summary>
        /// <param name="library">
        /// </param>
        /// <param name="grfMode">
        /// </param>
        /// <returns>
        /// </returns>
        [PreserveSig]
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        HRESULT LoadLibraryFromItem([In] [MarshalAs(UnmanagedType.Interface)] IShellItem library, [In] ShellNativeMethods.STGMs grfMode);

        /// <summary>
        /// </summary>
        /// <param name="knownfidLibrary">
        /// </param>
        /// <param name="grfMode">
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void LoadLibraryFromKnownFolder([In] ref Guid knownfidLibrary, [In] ShellNativeMethods.STGMs grfMode);

        /// <summary>
        /// </summary>
        /// <param name="location">
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void AddFolder([In] [MarshalAs(UnmanagedType.Interface)] IShellItem location);

        /// <summary>
        /// </summary>
        /// <param name="location">
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void RemoveFolder([In] [MarshalAs(UnmanagedType.Interface)] IShellItem location);

        /// <summary>
        /// </summary>
        /// <param name="lff">
        /// </param>
        /// <param name="riid">
        /// </param>
        /// <param name="ppv">
        /// </param>
        /// <returns>
        /// </returns>
        [PreserveSig]
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        HRESULT GetFolders([In] ShellNativeMethods.LIBRARYFOLDERFILTER lff, [In] ref Guid riid, [MarshalAs(UnmanagedType.Interface)] out IShellItemArray ppv);

        /// <summary>
        /// </summary>
        /// <param name="folderToResolve">
        /// </param>
        /// <param name="timeout">
        /// </param>
        /// <param name="riid">
        /// </param>
        /// <param name="ppv">
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void ResolveFolder(
            [In] [MarshalAs(UnmanagedType.Interface)] IShellItem folderToResolve, 
            [In] uint timeout, 
            [In] ref Guid riid, 
            [MarshalAs(UnmanagedType.Interface)] out IShellItem ppv);

        /// <summary>
        /// </summary>
        /// <param name="dsft">
        /// </param>
        /// <param name="riid">
        /// </param>
        /// <param name="ppv">
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetDefaultSaveFolder([In] ShellNativeMethods.DEFAULTSAVEFOLDERTYPE dsft, [In] ref Guid riid, [MarshalAs(UnmanagedType.Interface)] out IShellItem ppv);

        /// <summary>
        /// </summary>
        /// <param name="dsft">
        /// </param>
        /// <param name="si">
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void SetDefaultSaveFolder([In] ShellNativeMethods.DEFAULTSAVEFOLDERTYPE dsft, [In] [MarshalAs(UnmanagedType.Interface)] IShellItem si);

        /// <summary>
        /// </summary>
        /// <param name="lofOptions">
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetOptions(out ShellNativeMethods.LIBRARYOPTIONFLAGSs lofOptions);

        /// <summary>
        /// </summary>
        /// <param name="lofMask">
        /// </param>
        /// <param name="lofOptions">
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void SetOptions([In] ShellNativeMethods.LIBRARYOPTIONFLAGSs lofMask, [In] ShellNativeMethods.LIBRARYOPTIONFLAGSs lofOptions);

        /// <summary>
        /// </summary>
        /// <param name="ftid">
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetFolderType(out Guid ftid);

        /// <summary>
        /// </summary>
        /// <param name="ftid">
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void SetFolderType([In] ref Guid ftid);

        /// <summary>
        /// </summary>
        /// <param name="icon">
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetIcon([MarshalAs(UnmanagedType.LPWStr)] out string icon);

        /// <summary>
        /// </summary>
        /// <param name="icon">
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void SetIcon([In] [MarshalAs(UnmanagedType.LPWStr)] string icon);

        /// <summary>
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void Commit();

        /// <summary>
        /// </summary>
        /// <param name="folderToSaveIn">
        /// </param>
        /// <param name="libraryName">
        /// </param>
        /// <param name="lsf">
        /// </param>
        /// <param name="savedTo">
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void Save(
            [In] [MarshalAs(UnmanagedType.Interface)] IShellItem folderToSaveIn, 
            [In] [MarshalAs(UnmanagedType.LPWStr)] string libraryName, 
            [In] ShellNativeMethods.LIBRARYSAVEFLAGS lsf, 
            [MarshalAs(UnmanagedType.Interface)] out IShellItem2 savedTo);

        /// <summary>
        /// </summary>
        /// <param name="kfidToSaveIn">
        /// </param>
        /// <param name="libraryName">
        /// </param>
        /// <param name="lsf">
        /// </param>
        /// <param name="savedTo">
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void SaveInKnownFolder(
            [In] ref Guid kfidToSaveIn, 
            [In] [MarshalAs(UnmanagedType.LPWStr)] string libraryName, 
            [In] ShellNativeMethods.LIBRARYSAVEFLAGS lsf, 
            [MarshalAs(UnmanagedType.Interface)] out IShellItem2 savedTo);
    } ;

    /// <summary>
    /// </summary>
    [ComImportAttribute]
    [GuidAttribute("bcc18b79-ba16-442f-80c4-8a59c30c463b")]
    [InterfaceTypeAttribute(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IShellItemImageFactory
    {
        /// <summary>
        /// </summary>
        /// <param name="size">
        /// </param>
        /// <param name="flags">
        /// </param>
        /// <param name="phbm">
        /// </param>
        /// <returns>
        /// </returns>
        [PreserveSig]
        HRESULT GetImage([In] [MarshalAs(UnmanagedType.Struct)] CoreNativeMethods.SIZE size, [In] ShellNativeMethods.SIIGBFs flags, [Out] out IntPtr phbm);
    }

    /// <summary>
    /// </summary>
    /// <summary>
    /// </summary>
    /// <summary>
    /// </summary>
    [ComImport]
    [Guid(ShellIidGuid.IShellFolder)]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [ComConversionLoss]
    internal interface IShellFolder
    {
        /// <summary>
        /// </summary>
        /// <param name="hwnd">
        /// </param>
        /// <param name="pbc">
        /// </param>
        /// <param name="pszDisplayName">
        /// </param>
        /// <param name="pchEaten">
        /// </param>
        /// <param name="ppidl">
        /// </param>
        /// <param name="pdwAttributes">
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void ParseDisplayName(
            IntPtr hwnd, 
            [In] [MarshalAs(UnmanagedType.Interface)] IBindCtx pbc, 
            [In] [MarshalAs(UnmanagedType.LPWStr)] string pszDisplayName, 
            [In] [Out] ref uint pchEaten, 
            [Out] IntPtr ppidl, 
            [In] [Out] ref uint pdwAttributes);

        /// <summary>
        /// </summary>
        /// <param name="hwnd">
        /// </param>
        /// <param name="grfFlags">
        /// </param>
        /// <param name="ppenumIDList">
        /// </param>
        /// <returns>
        /// </returns>
        [PreserveSig]
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        HRESULT EnumObjects([In] IntPtr hwnd, [In] ShellNativeMethods.SHCONTs grfFlags, [MarshalAs(UnmanagedType.Interface)] out IEnumIDList ppenumIDList);

        /// <summary>
        /// </summary>
        /// <param name="pidl">
        /// </param>
        /// <param name="pbc">
        /// </param>
        /// <param name="riid">
        /// </param>
        /// <param name="ppv">
        /// </param>
        /// <returns>
        /// </returns>
        [PreserveSig]
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        HRESULT BindToObject(
            [In] IntPtr pidl, 
            /*[In, MarshalAs(UnmanagedType.Interface)] IBindCtx*/ IntPtr pbc, 
            [In] ref Guid riid, 
            [Out] [MarshalAs(UnmanagedType.Interface)] out IShellFolder ppv);

        /// <summary>
        /// </summary>
        /// <param name="pidl">
        /// </param>
        /// <param name="pbc">
        /// </param>
        /// <param name="riid">
        /// </param>
        /// <param name="ppv">
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void BindToStorage([In] ref IntPtr pidl, [In] [MarshalAs(UnmanagedType.Interface)] IBindCtx pbc, [In] ref Guid riid, out IntPtr ppv);

        /// <summary>
        /// </summary>
        /// <param name="lParam">
        /// </param>
        /// <param name="pidl1">
        /// </param>
        /// <param name="pidl2">
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void CompareIDs([In] IntPtr lParam, [In] ref IntPtr pidl1, [In] ref IntPtr pidl2);

        /// <summary>
        /// </summary>
        /// <param name="hwndOwner">
        /// </param>
        /// <param name="riid">
        /// </param>
        /// <param name="ppv">
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void CreateViewObject([In] IntPtr hwndOwner, [In] ref Guid riid, out IntPtr ppv);

        /// <summary>
        /// </summary>
        /// <param name="cidl">
        /// </param>
        /// <param name="apidl">
        /// </param>
        /// <param name="rgfInOut">
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetAttributesOf([In] uint cidl, [In] IntPtr apidl, [In] [Out] ref uint rgfInOut);

        /// <summary>
        /// </summary>
        /// <param name="hwndOwner">
        /// </param>
        /// <param name="cidl">
        /// </param>
        /// <param name="apidl">
        /// </param>
        /// <param name="riid">
        /// </param>
        /// <param name="rgfReserved">
        /// </param>
        /// <param name="ppv">
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetUIObjectOf([In] IntPtr hwndOwner, [In] uint cidl, [In] IntPtr apidl, [In] ref Guid riid, [In] [Out] ref uint rgfReserved, out IntPtr ppv);

        /// <summary>
        /// </summary>
        /// <param name="pidl">
        /// </param>
        /// <param name="uFlags">
        /// </param>
        /// <param name="pName">
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetDisplayNameOf([In] ref IntPtr pidl, [In] uint uFlags, out IntPtr pName);

        /// <summary>
        /// </summary>
        /// <param name="hwnd">
        /// </param>
        /// <param name="pidl">
        /// </param>
        /// <param name="pszName">
        /// </param>
        /// <param name="uFlags">
        /// </param>
        /// <param name="ppidlOut">
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void SetNameOf([In] IntPtr hwnd, [In] ref IntPtr pidl, [In] [MarshalAs(UnmanagedType.LPWStr)] string pszName, [In] uint uFlags, [Out] IntPtr ppidlOut);
    }

    /// <summary>
    /// </summary>
    /// <summary>
    /// </summary>
    [ComImport]
    [Guid(ShellIidGuid.IEnumIDList)]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IEnumIDList
    {
        /// <summary>
        /// </summary>
        /// <param name="celt">
        /// </param>
        /// <param name="rgelt">
        /// </param>
        /// <param name="pceltFetched">
        /// </param>
        /// <returns>
        /// </returns>
        [PreserveSig]
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        HRESULT Next(uint celt, out IntPtr rgelt, out uint pceltFetched);

        /// <summary>
        /// </summary>
        /// <param name="celt">
        /// </param>
        /// <returns>
        /// </returns>
        [PreserveSig]
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        HRESULT Skip([In] uint celt);

        /// <summary>
        /// </summary>
        /// <returns>
        /// </returns>
        [PreserveSig]
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        HRESULT Reset();

        /// <summary>
        /// </summary>
        /// <param name="ppenum">
        /// </param>
        /// <returns>
        /// </returns>
        [PreserveSig]
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        HRESULT Clone([MarshalAs(UnmanagedType.Interface)] out IEnumIDList ppenum);
    }

    /// <summary>
    /// </summary>
    [ComImport]
    [Guid(ShellIidGuid.IShellLinkW)]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IShellLinkW
    {
        /// <summary>
        /// </summary>
        /// <param name="pszFile">
        /// </param>
        /// <param name="cchMaxPath">
        /// </param>
        /// <param name="pfd">
        /// </param>
        /// <param name="fFlags">
        /// </param>
        void GetPath(
            [Out] [MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszFile, 
            int cchMaxPath, 
            // ref _WIN32_FIND_DATAW pfd,
            IntPtr pfd, 
            uint fFlags);

        /// <summary>
        /// </summary>
        /// <param name="ppidl">
        /// </param>
        void GetIDList(out IntPtr ppidl);

        /// <summary>
        /// </summary>
        /// <param name="pidl">
        /// </param>
        void SetIDList(IntPtr pidl);

        /// <summary>
        /// </summary>
        /// <param name="pszFile">
        /// </param>
        /// <param name="cchMaxName">
        /// </param>
        void GetDescription([Out] [MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszFile, int cchMaxName);

        /// <summary>
        /// </summary>
        /// <param name="pszName">
        /// </param>
        void SetDescription([MarshalAs(UnmanagedType.LPWStr)] string pszName);

        /// <summary>
        /// </summary>
        /// <param name="pszDir">
        /// </param>
        /// <param name="cchMaxPath">
        /// </param>
        void GetWorkingDirectory([Out] [MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszDir, int cchMaxPath);

        /// <summary>
        /// </summary>
        /// <param name="pszDir">
        /// </param>
        void SetWorkingDirectory([MarshalAs(UnmanagedType.LPWStr)] string pszDir);

        /// <summary>
        /// </summary>
        /// <param name="pszArgs">
        /// </param>
        /// <param name="cchMaxPath">
        /// </param>
        void GetArguments([Out] [MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszArgs, int cchMaxPath);

        /// <summary>
        /// </summary>
        /// <param name="pszArgs">
        /// </param>
        void SetArguments([MarshalAs(UnmanagedType.LPWStr)] string pszArgs);

        /// <summary>
        /// </summary>
        /// <param name="wHotKey">
        /// </param>
        void GetHotKey(out short wHotKey);

        /// <summary>
        /// </summary>
        /// <param name="wHotKey">
        /// </param>
        void SetHotKey(short wHotKey);

        /// <summary>
        /// </summary>
        /// <param name="iShowCmd">
        /// </param>
        void GetShowCmd(out uint iShowCmd);

        /// <summary>
        /// </summary>
        /// <param name="iShowCmd">
        /// </param>
        void SetShowCmd(uint iShowCmd);

        /// <summary>
        /// </summary>
        /// <param name="pszIconPath">
        /// </param>
        /// <param name="cchIconPath">
        /// </param>
        /// <param name="iIcon">
        /// </param>
        void GetIconLocation([Out] [MarshalAs(UnmanagedType.LPWStr)] out StringBuilder pszIconPath, int cchIconPath, out int iIcon);

        /// <summary>
        /// </summary>
        /// <param name="pszIconPath">
        /// </param>
        /// <param name="iIcon">
        /// </param>
        void SetIconLocation([MarshalAs(UnmanagedType.LPWStr)] string pszIconPath, int iIcon);

        /// <summary>
        /// </summary>
        /// <param name="pszPathRel">
        /// </param>
        /// <param name="dwReserved">
        /// </param>
        void SetRelativePath([MarshalAs(UnmanagedType.LPWStr)] string pszPathRel, uint dwReserved);

        /// <summary>
        /// </summary>
        /// <param name="hwnd">
        /// </param>
        /// <param name="fFlags">
        /// </param>
        void Resolve(IntPtr hwnd, uint fFlags);

        /// <summary>
        /// </summary>
        /// <param name="pszFile">
        /// </param>
        void SetPath([MarshalAs(UnmanagedType.LPWStr)] string pszFile);
    }

    /// <summary>
    /// </summary>
    [ComImport]
    [Guid(ShellIidGuid.CShellLink)]
    [ClassInterface(ClassInterfaceType.None)]
    internal class CShellLink
    {
    }

    // Summary:
    // Provides the managed definition of the IPersistStream interface, with functionality
    // from IPersist.
    /// <summary>
    /// </summary>
    [ComImport]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("00000109-0000-0000-C000-000000000046")]
    internal interface IPersistStream
    {
        // Summary:
        // Retrieves the class identifier (CLSID) of an object.
        // Parameters:
        // pClassID:
        // When this method returns, contains a reference to the CLSID. This parameter
        // is passed uninitialized.
        /// <summary>
        /// </summary>
        /// <param name="pClassID">
        /// </param>
        [PreserveSig]
        void GetClassID(out Guid pClassID);

        // Summary:
        // Checks an object for changes since it was last saved to its current file.
        // Returns:
        // S_OK if the file has changed since it was last saved; S_FALSE if the file
        // has not changed since it was last saved.
        /// <summary>
        /// </summary>
        /// <returns>
        /// </returns>
        [PreserveSig]
        HRESULT IsDirty();

        /// <summary>
        /// </summary>
        /// <param name="stm">
        /// </param>
        /// <returns>
        /// </returns>
        [PreserveSig]
        HRESULT Load([In] [MarshalAs(UnmanagedType.Interface)] IStream stm);

        /// <summary>
        /// </summary>
        /// <param name="stm">
        /// </param>
        /// <param name="fRemember">
        /// </param>
        /// <returns>
        /// </returns>
        [PreserveSig]
        HRESULT Save([In] [MarshalAs(UnmanagedType.Interface)] IStream stm, bool fRemember);

        /// <summary>
        /// </summary>
        /// <param name="cbSize">
        /// </param>
        /// <returns>
        /// </returns>
        [PreserveSig]
        HRESULT GetSizeMax(out ulong cbSize);
    }

    /// <summary>
    /// </summary>
    [ComImport]
    [Guid(ShellIidGuid.ICondition)]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface II : IPersistStream
    {
        // Summary:
        // Retrieves the class identifier (CLSID) of an object.
        // Parameters:
        // pClassID:
        // When this method returns, contains a reference to the CLSID. This parameter
        // is passed uninitialized.
        /// <summary>
        /// </summary>
        /// <param name="pClassID">
        /// </param>
        [PreserveSig]
        void GetClassID(out Guid pClassID);

        // Summary:
        // Checks an object for changes since it was last saved to its current file.
        // Returns:
        // S_OK if the file has changed since it was last saved; S_FALSE if the file
        // has not changed since it was last saved.
        /// <summary>
        /// </summary>
        /// <returns>
        /// </returns>
        [PreserveSig]
        HRESULT IsDirty();

        /// <summary>
        /// </summary>
        /// <param name="stm">
        /// </param>
        /// <returns>
        /// </returns>
        [PreserveSig]
        HRESULT Load([In] [MarshalAs(UnmanagedType.Interface)] IStream stm);

        /// <summary>
        /// </summary>
        /// <param name="stm">
        /// </param>
        /// <param name="fRemember">
        /// </param>
        /// <returns>
        /// </returns>
        [PreserveSig]
        HRESULT Save([In] [MarshalAs(UnmanagedType.Interface)] IStream stm, bool fRemember);

        /// <summary>
        /// </summary>
        /// <param name="cbSize">
        /// </param>
        /// <returns>
        /// </returns>
        [PreserveSig]
        HRESULT GetSizeMax(out ulong cbSize);

        // For any node, return what kind of node it is.
        /// <summary>
        /// </summary>
        /// <param name="pNodeType">
        /// </param>
        /// <returns>
        /// </returns>
        [PreserveSig]
        HRESULT GetConditionType([Out] out SearchConditionType pNodeType);

        // riid must be IID_IEnumUnknown, IID_IEnumVARIANT or IID_IObjectArray, or in the case of a negation node IID_ICondition.
        // If this is a leaf node, E_FAIL will be returned.
        // If this is a negation node, then if riid is IID_ICondition, *ppv will be set to a single ICondition, otherwise an enumeration of one.
        // If this is a conjunction or a disjunction, *ppv will be set to an enumeration of the subconditions.
        /// <summary>
        /// </summary>
        /// <param name="riid">
        /// </param>
        /// <param name="ppv">
        /// </param>
        /// <returns>
        /// </returns>
        [PreserveSig]
        HRESULT GetSubConditions([In] ref Guid riid, [Out] [MarshalAs(UnmanagedType.Interface)] out object ppv);

        // If this is not a leaf node, E_FAIL will be returned.
        // Retrieve the property name, operation and value from the leaf node.
        // Any one of ppszPropertyName, pcop and ppropvar may be NULL.
        /// <summary>
        /// </summary>
        /// <param name="ppszPropertyName">
        /// </param>
        /// <param name="pcop">
        /// </param>
        /// <param name="ppropvar">
        /// </param>
        /// <returns>
        /// </returns>
        [PreserveSig]
        HRESULT GetComparisonInfo(
            [Out] [MarshalAs(UnmanagedType.LPWStr)] out string ppszPropertyName, [Out] out SearchConditionOperation pcop, [Out] out PropVariant ppropvar);

        // If this is not a leaf node, E_FAIL will be returned.
        // *ppszValueTypeName will be set to the semantic type of the value, or to NULL if this is not meaningful.
        /// <summary>
        /// </summary>
        /// <param name="ppszValueTypeName">
        /// </param>
        /// <returns>
        /// </returns>
        [PreserveSig]
        HRESULT GetValueType([Out] [MarshalAs(UnmanagedType.LPWStr)] out string ppszValueTypeName);

        // If this is not a leaf node, E_FAIL will be returned.
        // If the value of the leaf node is VT_EMPTY, *ppszNormalization will be set to an empty string.
        // If the value is a string (VT_LPWSTR, VT_BSTR or VT_LPSTR), then *ppszNormalization will be set to a
        // character-normalized form of the value.
        // Otherwise, *ppszNormalization will be set to some (character-normalized) string representation of the value.
        /// <summary>
        /// </summary>
        /// <param name="ppszNormalization">
        /// </param>
        /// <returns>
        /// </returns>
        [PreserveSig]
        HRESULT GetValueNormalization([Out] [MarshalAs(UnmanagedType.LPWStr)] out string ppszNormalization);

        // Return information about what parts of the input produced the property, the operation and the value.
        // Any one of ppPropertyTerm, ppOperationTerm and ppValueTerm may be NULL.
        // For a leaf node returned by the parser, the position information of each IRichChunk identifies the tokens that
        // contributed the property/operation/value, the string value is the corresponding part of the input string, and
        // the PROPVARIANT is VT_EMPTY.
        /// <summary>
        /// </summary>
        /// <param name="ppPropertyTerm">
        /// </param>
        /// <param name="ppOperationTerm">
        /// </param>
        /// <param name="ppValueTerm">
        /// </param>
        /// <returns>
        /// </returns>
        [PreserveSig]
        HRESULT GetInputTerms([Out] out IRichChunk ppPropertyTerm, [Out] out IRichChunk ppOperationTerm, [Out] out IRichChunk ppValueTerm);

        // Make a deep copy of this ICondition.
        /// <summary>
        /// </summary>
        /// <param name="ppc">
        /// </param>
        /// <returns>
        /// </returns>
        [PreserveSig]
        HRESULT Clone([Out] out II ppc);
    } ;

    /// <summary>
    /// </summary>
    [ComImport]
    [Guid(ShellIidGuid.IRichChunk)]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IRichChunk
    {
        // The position *pFirstPos is zero-based.
        // Any one of pFirstPos, pLength, ppsz and pValue may be NULL.
        /// <summary>
        /// </summary>
        /// <returns>
        /// </returns>
        [PreserveSig]
        HRESULT GetData(
            


            /*[out, annotation("__out_opt")] ULONG* pFirstPos, [out, annotation("__out_opt")] ULONG* pLength, [out, annotation("__deref_opt_out_opt")] LPWSTR* ppsz, [out, annotation("__out_opt")] PROPVARIANT* pValue*/);
    }

    /// <summary>
    /// </summary>
    [ComImport]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid(ShellIidGuid.IEnumUnknown)]
    internal interface IEnumUnknown
    {
        /// <summary>
        /// </summary>
        /// <param name="requestedNumber">
        /// </param>
        /// <param name="buffer">
        /// </param>
        /// <param name="fetchedNumber">
        /// </param>
        /// <returns>
        /// </returns>
        [PreserveSig]
        HRESULT Next(uint requestedNumber, ref IntPtr buffer, ref uint fetchedNumber);

        /// <summary>
        /// </summary>
        /// <param name="number">
        /// </param>
        /// <returns>
        /// </returns>
        [PreserveSig]
        HRESULT Skip(uint number);

        /// <summary>
        /// </summary>
        /// <returns>
        /// </returns>
        [PreserveSig]
        HRESULT Reset();

        /// <summary>
        /// </summary>
        /// <param name="result">
        /// </param>
        /// <returns>
        /// </returns>
        [PreserveSig]
        HRESULT Clone(out IEnumUnknown result);
    }

    /// <summary>
    /// </summary>
    [ComImport]
    [Guid(ShellIidGuid.IConditionFactory)]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IConditionFactory
    {
        /// <summary>
        /// </summary>
        /// <param name="pcSub">
        /// </param>
        /// <param name="fSimplify">
        /// </param>
        /// <param name="ppcResult">
        /// </param>
        /// <returns>
        /// </returns>
        [PreserveSig]
        HRESULT MakeNot([In] II pcSub, [In] bool fSimplify, [Out] out II ppcResult);

        /// <summary>
        /// </summary>
        /// <param name="ct">
        /// </param>
        /// <param name="peuSubs">
        /// </param>
        /// <param name="fSimplify">
        /// </param>
        /// <param name="ppcResult">
        /// </param>
        /// <returns>
        /// </returns>
        [PreserveSig]
        HRESULT MakeAndOr([In] SearchConditionType ct, [In] IEnumUnknown peuSubs, [In] bool fSimplify, [Out] out II ppcResult);

        /// <summary>
        /// </summary>
        /// <param name="pszPropertyName">
        /// </param>
        /// <param name="cop">
        /// </param>
        /// <param name="pszValueType">
        /// </param>
        /// <param name="ppropvar">
        /// </param>
        /// <param name="richChunk1">
        /// </param>
        /// <param name="richChunk2">
        /// </param>
        /// <param name="richChunk3">
        /// </param>
        /// <param name="fExpand">
        /// </param>
        /// <param name="ppcResult">
        /// </param>
        /// <returns>
        /// </returns>
        [PreserveSig]
        HRESULT MakeLeaf(
            [In] [MarshalAs(UnmanagedType.LPWStr)] string pszPropertyName, 
            [In] SearchConditionOperation cop, 
            [In] [MarshalAs(UnmanagedType.LPWStr)] string pszValueType, 
            [In] ref PropVariant ppropvar, 
            IRichChunk richChunk1, 
            IRichChunk richChunk2, 
            IRichChunk richChunk3, 
            [In] bool fExpand, 
            [Out] out II ppcResult);

        /// <summary>
        /// </summary>
        /// <returns>
        /// </returns>
        [PreserveSig]
        HRESULT Resolve( /*[In] ICondition pc, [In] STRUCTURED_QUERY_RESOLVE_OPTION sqro, [In] ref SYSTEMTIME pstReferenceTime, [Out] out ICondition ppcResolved*/);
    } ;

    /// <summary>
    /// </summary>
    /// <summary>
    /// </summary>
    [ComImport]
    [ClassInterface(ClassInterfaceType.None)]
    [TypeLibType(TypeLibTypeFlags.FCanCreate)]
    [Guid(ShellClsidGuid.ConditionFactory)]
    internal class ConditionFactoryCOClass
    {
    }

    /// <summary>
    /// </summary>
    [ComImport]
    [Guid(ShellIidGuid.ISearchFolderItemFactory)]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface ISearchFolderItemFactory
    {
        /// <summary>
        /// </summary>
        /// <param name="pszDisplayName">
        /// </param>
        /// <returns>
        /// </returns>
        [PreserveSig]
        HRESULT SetDisplayName([In] [MarshalAs(UnmanagedType.LPWStr)] string pszDisplayName);

        /// <summary>
        /// </summary>
        /// <param name="ftid">
        /// </param>
        /// <returns>
        /// </returns>
        [PreserveSig]
        HRESULT SetFolderTypeID([In] Guid ftid);

        /// <summary>
        /// </summary>
        /// <param name="flvm">
        /// </param>
        /// <returns>
        /// </returns>
        [PreserveSig]
        HRESULT SetFolderLogicalViewMode([In] FolderLogicalViewMode flvm);

        /// <summary>
        /// </summary>
        /// <param name="iIconSize">
        /// </param>
        /// <returns>
        /// </returns>
        [PreserveSig]
        HRESULT SetIconSize([In] int iIconSize);

        /// <summary>
        /// </summary>
        /// <param name="cVisibleColumns">
        /// </param>
        /// <param name="rgKey">
        /// </param>
        /// <returns>
        /// </returns>
        [PreserveSig]
        HRESULT SetVisibleColumns([In] uint cVisibleColumns, [In] [MarshalAs(UnmanagedType.LPArray)] PropertyKey[] rgKey);

        /// <summary>
        /// </summary>
        /// <param name="cSortColumns">
        /// </param>
        /// <param name="rgSortColumns">
        /// </param>
        /// <returns>
        /// </returns>
        [PreserveSig]
        HRESULT SetSortColumns([In] uint cSortColumns, [In] [MarshalAs(UnmanagedType.LPArray)] SortColumn[] rgSortColumns);

        /// <summary>
        /// </summary>
        /// <param name="keyGroup">
        /// </param>
        /// <returns>
        /// </returns>
        [PreserveSig]
        HRESULT SetGroupColumn([In] ref PropertyKey keyGroup);

        /// <summary>
        /// </summary>
        /// <param name="cStackKeys">
        /// </param>
        /// <param name="rgStackKeys">
        /// </param>
        /// <returns>
        /// </returns>
        [PreserveSig]
        HRESULT SetStacks([In] uint cStackKeys, [In] [MarshalAs(UnmanagedType.LPArray)] PropertyKey[] rgStackKeys);

        /// <summary>
        /// </summary>
        /// <param name="ppv">
        /// </param>
        /// <returns>
        /// </returns>
        [PreserveSig]
        HRESULT SetScope([In] [MarshalAs(UnmanagedType.Interface)] IShellItemArray ppv);

        /// <summary>
        /// </summary>
        /// <param name="pCondition">
        /// </param>
        /// <returns>
        /// </returns>
        [PreserveSig]
        HRESULT SetCondition([In] II pCondition);

        /// <summary>
        /// </summary>
        /// <param name="riid">
        /// </param>
        /// <param name="ppv">
        /// </param>
        /// <returns>
        /// </returns>
        [PreserveSig]
        int GetShellItem(ref Guid riid, [Out] [MarshalAs(UnmanagedType.Interface)] out IShellItem ppv);

        /// <summary>
        /// </summary>
        /// <param name="ppidl">
        /// </param>
        /// <returns>
        /// </returns>
        [PreserveSig]
        HRESULT GetIDList([Out] IntPtr ppidl);
    } ;

    /// <summary>
    /// </summary>
    /// <summary>
    /// </summary>
    [ComImport]
    [ClassInterface(ClassInterfaceType.None)]
    [TypeLibType(TypeLibTypeFlags.FCanCreate)]
    [Guid(ShellClsidGuid.SearchFolderItemFactory)]
    internal class SearchFolderItemFactoryCOClass
    {
    }

    /// <summary>
    /// </summary>
    [ComImport]
    [Guid(ShellIidGuid.IQuerySolution)]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IQuerySolution : IConditionFactory
    {
        /// <summary>
        /// </summary>
        /// <param name="pcSub">
        /// </param>
        /// <param name="fSimplify">
        /// </param>
        /// <param name="ppcResult">
        /// </param>
        /// <returns>
        /// </returns>
        [PreserveSig]
        HRESULT MakeNot([In] II pcSub, [In] bool fSimplify, [Out] out II ppcResult);

        /// <summary>
        /// </summary>
        /// <param name="ct">
        /// </param>
        /// <param name="peuSubs">
        /// </param>
        /// <param name="fSimplify">
        /// </param>
        /// <param name="ppcResult">
        /// </param>
        /// <returns>
        /// </returns>
        [PreserveSig]
        HRESULT MakeAndOr([In] SearchConditionType ct, [In] IEnumUnknown peuSubs, [In] bool fSimplify, [Out] out II ppcResult);

        /// <summary>
        /// </summary>
        /// <param name="pszPropertyName">
        /// </param>
        /// <param name="cop">
        /// </param>
        /// <param name="pszValueType">
        /// </param>
        /// <param name="ppropvar">
        /// </param>
        /// <param name="richChunk1">
        /// </param>
        /// <param name="richChunk2">
        /// </param>
        /// <param name="richChunk3">
        /// </param>
        /// <param name="fExpand">
        /// </param>
        /// <param name="ppcResult">
        /// </param>
        /// <returns>
        /// </returns>
        [PreserveSig]
        HRESULT MakeLeaf(
            [In] [MarshalAs(UnmanagedType.LPWStr)] string pszPropertyName, 
            [In] SearchConditionOperation cop, 
            [In] [MarshalAs(UnmanagedType.LPWStr)] string pszValueType, 
            [In] ref PropVariant ppropvar, 
            IRichChunk richChunk1, 
            IRichChunk richChunk2, 
            IRichChunk richChunk3, 
            [In] bool fExpand, 
            [Out] out II ppcResult);

        /// <summary>
        /// </summary>
        /// <param name="pc">
        /// </param>
        /// <param name="sqro">
        /// </param>
        /// <param name="pstReferenceTime">
        /// </param>
        /// <param name="ppcResolved">
        /// </param>
        /// <returns>
        /// </returns>
        [PreserveSig]
        HRESULT Resolve([In] II pc, [In] int sqro, [In] ref SYSTEMTIME pstReferenceTime, [Out] out II ppcResolved);

        // Retrieve the condition tree and the "main type" of the solution.
        // ppQueryNode and ppMainType may be NULL.
        /// <summary>
        /// </summary>
        /// <param name="ppQueryNode">
        /// </param>
        /// <param name="ppMainType">
        /// </param>
        /// <returns>
        /// </returns>
        [PreserveSig]
        HRESULT GetQuery([Out] [MarshalAs(UnmanagedType.Interface)] out II ppQueryNode, [Out] [MarshalAs(UnmanagedType.Interface)] out IEntity ppMainType);

        // Identify parts of the input string not accounted for.
        // Each parse error is represented by an IRichChunk where the position information
        // reflect token counts, the string is NULL and the value is a VT_I4
        // where lVal is from the ParseErrorType enumeration. The valid
        // values for riid are IID_IEnumUnknown and IID_IEnumVARIANT.
        /// <summary>
        /// </summary>
        /// <param name="riid">
        /// </param>
        /// <param name="ppParseErrors">
        /// </param>
        /// <returns>
        /// </returns>
        [PreserveSig]
        HRESULT GetErrors([In] ref Guid riid, [Out] out /* void** */ IntPtr ppParseErrors);

        // Report the query string, how it was tokenized and what LCID and word breaker were used (for recognizing keywords).
        // ppszInputString, ppTokens, pLocale and ppWordBreaker may be NULL.
        /// <summary>
        /// </summary>
        /// <param name="ppszInput">
        /// </param>
        /// <param name="ppTokens">
        /// </param>
        /// <param name="plcid">
        /// </param>
        /// <param name="ppWordBreaker">
        /// </param>
        /// <returns>
        /// </returns>
        [PreserveSig]
        HRESULT GetLexicalData(
            [MarshalAs(UnmanagedType.LPWStr)] out string ppszInput, 
            [Out] /* ITokenCollection** */ out IntPtr ppTokens, 
            [Out] out uint plcid, 
            [Out] /* IUnknown** */ out IntPtr ppWordBreaker);
    }

    /// <summary>
    /// </summary>
    [ComImport]
    [Guid(ShellIidGuid.IQueryParser)]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IQueryParser
    {
        // Parse parses an input string, producing a query solution.
        // pCustomProperties should be an enumeration of IRichChunk objects, one for each custom property
        // the application has recognized. pCustomProperties may be NULL, equivalent to an empty enumeration.
        // For each IRichChunk, the position information identifies the character span of the custom property,
        // the string value should be the name of an actual property, and the PROPVARIANT is completely ignored.
        /// <summary>
        /// </summary>
        /// <param name="pszInput">
        /// </param>
        /// <param name="pCustomProperties">
        /// </param>
        /// <param name="ppSolution">
        /// </param>
        /// <returns>
        /// </returns>
        [PreserveSig]
        HRESULT Parse([In] [MarshalAs(UnmanagedType.LPWStr)] string pszInput, [In] IEnumUnknown pCustomProperties, [Out] out IQuerySolution ppSolution);

        // Set a single option. See STRUCTURED_QUERY_SINGLE_OPTION above.
        /// <summary>
        /// </summary>
        /// <param name="option">
        /// </param>
        /// <param name="pOptionValue">
        /// </param>
        /// <returns>
        /// </returns>
        [PreserveSig]
        HRESULT SetOption([In] StructuredQuerySingleOption option, [In] ref PropVariant pOptionValue);

        /// <summary>
        /// </summary>
        /// <param name="option">
        /// </param>
        /// <param name="pOptionValue">
        /// </param>
        /// <returns>
        /// </returns>
        [PreserveSig]
        HRESULT GetOption([In] StructuredQuerySingleOption option, [Out] out PropVariant pOptionValue);

        // Set a multi option. See STRUCTURED_QUERY_MULTIOPTION above.
        /// <summary>
        /// </summary>
        /// <param name="option">
        /// </param>
        /// <param name="pszOptionKey">
        /// </param>
        /// <param name="pOptionValue">
        /// </param>
        /// <returns>
        /// </returns>
        [PreserveSig]
        HRESULT SetMultiOption([In] StructuredQueryMultipleOption option, [In] [MarshalAs(UnmanagedType.LPWStr)] string pszOptionKey, [In] PropVariant pOptionValue);

        // Get a schema provider for browsing the currently loaded schema.
        /// <summary>
        /// </summary>
        /// <param name="ppSchemaProvider">
        /// </param>
        /// <returns>
        /// </returns>
        [PreserveSig]
        HRESULT GetSchemaProvider([Out] out /*ISchemaProvider*/ IntPtr ppSchemaProvider);

        // Restate a condition as a query string according to the currently selected syntax.
        // The parameter fUseEnglish is reserved for future use; must be FALSE.
        /// <summary>
        /// </summary>
        /// <param name="pCondition">
        /// </param>
        /// <param name="fUseEnglish">
        /// </param>
        /// <param name="ppszQuery">
        /// </param>
        /// <returns>
        /// </returns>
        [PreserveSig]
        HRESULT RestateToString([In] II pCondition, [In] bool fUseEnglish, [Out] [MarshalAs(UnmanagedType.LPWStr)] out string ppszQuery);

        // Parse a condition for a given property. It can be anything that would go after 'PROPERTY:' in an AQS expession.
        /// <summary>
        /// </summary>
        /// <param name="pszPropertyName">
        /// </param>
        /// <param name="pszInput">
        /// </param>
        /// <param name="ppSolution">
        /// </param>
        /// <returns>
        /// </returns>
        [PreserveSig]
        HRESULT ParsePropertyValue(
            [In] [MarshalAs(UnmanagedType.LPWStr)] string pszPropertyName, [In] [MarshalAs(UnmanagedType.LPWStr)] string pszInput, [Out] out IQuerySolution ppSolution);

        // Restate a condition for a given property. If the condition contains a leaf with any other property name, or no property name at all,
        // E_INVALIDARG will be returned.
        /// <summary>
        /// </summary>
        /// <param name="pCondition">
        /// </param>
        /// <param name="fUseEnglish">
        /// </param>
        /// <param name="ppszPropertyName">
        /// </param>
        /// <param name="ppszQuery">
        /// </param>
        /// <returns>
        /// </returns>
        [PreserveSig]
        HRESULT RestatePropertyValueToString(
            [In] II pCondition, 
            [In] bool fUseEnglish, 
            [Out] [MarshalAs(UnmanagedType.LPWStr)] out string ppszPropertyName, 
            [Out] [MarshalAs(UnmanagedType.LPWStr)] out string ppszQuery);
    }

    /// <summary>
    /// </summary>
    [ComImport]
    [Guid(ShellIidGuid.IQueryParserManager)]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IQueryParserManager
    {
        // Create a query parser loaded with the schema for a certain catalog localize to a certain language, and initialized with
        // standard defaults. One valid value for riid is IID_IQueryParser.
        /// <summary>
        /// </summary>
        /// <param name="pszCatalog">
        /// </param>
        /// <param name="langidForKeywords">
        /// </param>
        /// <param name="riid">
        /// </param>
        /// <param name="ppQueryParser">
        /// </param>
        /// <returns>
        /// </returns>
        [PreserveSig]
        HRESULT CreateLoadedParser(
            [In] [MarshalAs(UnmanagedType.LPWStr)] string pszCatalog, [In] ushort langidForKeywords, [In] ref Guid riid, [Out] out IQueryParser ppQueryParser);

        // In addition to setting AQS/NQS and automatic wildcard for the given query parser, this sets up standard named entity handlers and
        // sets the keyboard locale as locale for word breaking.
        /// <summary>
        /// </summary>
        /// <param name="fUnderstandNqs">
        /// </param>
        /// <param name="fAutoWildCard">
        /// </param>
        /// <param name="pQueryParser">
        /// </param>
        /// <returns>
        /// </returns>
        [PreserveSig]
        HRESULT InitializeOptions([In] bool fUnderstandNqs, [In] bool fAutoWildCard, [In] IQueryParser pQueryParser);

        // Change one of the settings for the query parser manager, such as the name of the schema binary, or the location of the localized and unlocalized
        // schema binaries. By default, the settings point to the schema binaries used by Windows Shell.
        /// <summary>
        /// </summary>
        /// <param name="option">
        /// </param>
        /// <param name="pOptionValue">
        /// </param>
        /// <returns>
        /// </returns>
        [PreserveSig]
        HRESULT SetOption([In] QueryParserManagerOption option, [In] PropVariant pOptionValue);
    } ;

    /// <summary>
    /// </summary>
    /// <summary>
    /// </summary>
    [ComImport]
    [ClassInterface(ClassInterfaceType.None)]
    [TypeLibType(TypeLibTypeFlags.FCanCreate)]
    [Guid(ShellClsidGuid.QueryParserManager)]
    internal class QueryParserManagerCOClass
    {
    }

    /// <summary>
    /// </summary>
    [ComImport]
    [Guid("24264891-E80B-4fd3-B7CE-4FF2FAE8931F")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IEntity
    {
        // TODO
    }

    /// <summary>
    /// SYSTEMTIME structure with some useful methods
    /// </summary>
    internal struct SYSTEMTIME
    {
        #region Constants and Fields

        /// <summary>
        /// </summary>
        internal ushort wDay;

        /// <summary>
        /// </summary>
        internal ushort wDayOfWeek;

        /// <summary>
        /// </summary>
        internal ushort wHour;

        /// <summary>
        /// </summary>
        internal ushort wMilliseconds;

        /// <summary>
        /// </summary>
        internal ushort wMinute;

        /// <summary>
        /// </summary>
        internal ushort wMonth;

        /// <summary>
        /// </summary>
        internal ushort wSecond;

        /// <summary>
        /// </summary>
        internal ushort wYear;

        #endregion

        #region Operators

        /// <summary>
        /// </summary>
        /// <param name="x">
        /// </param>
        /// <param name="y">
        /// </param>
        /// <returns>
        /// </returns>
        /// <exception cref="NotImplementedException">
        /// </exception>
        public static bool operator ==(SYSTEMTIME x, SYSTEMTIME y)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// </summary>
        /// <param name="x">
        /// </param>
        /// <param name="y">
        /// </param>
        /// <returns>
        /// </returns>
        /// <exception cref="NotImplementedException">
        /// </exception>
        public static bool operator !=(SYSTEMTIME x, SYSTEMTIME y)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// </summary>
        /// <param name="obj">
        /// </param>
        /// <returns>
        /// </returns>
        /// <exception cref="NotImplementedException">
        /// </exception>
        public override bool Equals(object obj)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// </summary>
        /// <returns>
        /// </returns>
        /// <exception cref="NotImplementedException">
        /// </exception>
        public override int GetHashCode()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Convert to System.DateTime
        /// </summary>
        /// <param name="time">
        /// </param>
        /// <returns>
        /// </returns>
        internal static DateTime ToDateTime(SYSTEMTIME time)
        {
            return time.ToDateTime();
        }

        /// <summary>
        /// Convert form System.DateTime
        /// </summary>
        /// <param name="time">
        /// </param>
        internal void FromDateTime(DateTime time)
        {
            this.wYear = (ushort)time.Year;
            this.wMonth = (ushort)time.Month;
            this.wDayOfWeek = (ushort)time.DayOfWeek;
            this.wDay = (ushort)time.Day;
            this.wHour = (ushort)time.Hour;
            this.wMinute = (ushort)time.Minute;
            this.wSecond = (ushort)time.Second;
            this.wMilliseconds = (ushort)time.Millisecond;
        }

        /// <summary>
        /// Convert to System.DateTime
        /// </summary>
        /// <returns>
        /// </returns>
        internal DateTime ToDateTime()
        {
            return new DateTime(this.wYear, this.wMonth, this.wDay, this.wHour, this.wMinute, this.wSecond, this.wMilliseconds);
        }

        #endregion
    }

    #endregion

#pragma warning restore 108
}