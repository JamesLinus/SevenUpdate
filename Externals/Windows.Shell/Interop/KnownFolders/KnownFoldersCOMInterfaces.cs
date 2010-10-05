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

    using Microsoft.Windows.Internal;

    // Disable warning if a method declaration hides another inherited from a parent COM interface
    // To successfully import a COM interface, all inherited methods need to be declared again with 
    // the exception of those already declared in "IUnknown"
#pragma warning disable 0108

    /// <summary>
    /// </summary>
    [ComImport]
    [Guid(KnownFoldersIidGuid.IKnownFolder)]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IKnownFolderNative
    {
        /// <summary>
        /// </summary>
        /// <returns>
        /// </returns>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        Guid GetId();

        /// <summary>
        /// </summary>
        /// <returns>
        /// </returns>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        FolderCategory GetCategory();

        /// <summary>
        /// </summary>
        /// <param name="i">
        /// </param>
        /// <param name="interfaceGuid">
        /// </param>
        /// <param name="shellItem">
        /// </param>
        /// <returns>
        /// </returns>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        [PreserveSig]
        HRESULT GetShellItem([In] int i, ref Guid interfaceGuid, [Out] [MarshalAs(UnmanagedType.Interface)] out IShellItem2 shellItem);

        /// <summary>
        /// </summary>
        /// <param name="option">
        /// </param>
        /// <returns>
        /// </returns>
        [return: MarshalAs(UnmanagedType.LPWStr)]
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        string GetPath([In] int option);

        /// <summary>
        /// </summary>
        /// <param name="i">
        /// </param>
        /// <param name="path">
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void SetPath([In] int i, [In] string path);

        /// <summary>
        /// </summary>
        /// <param name="i">
        /// </param>
        /// <param name="itemIdentifierListPointer">
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetIDList([In] int i, [Out] out IntPtr itemIdentifierListPointer);

        /// <summary>
        /// </summary>
        /// <returns>
        /// </returns>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        Guid GetFolderType();

        /// <summary>
        /// </summary>
        /// <returns>
        /// </returns>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        RedirectionCapability GetRedirectionCapabilities();

        /// <summary>
        /// </summary>
        /// <param name="definition">
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetFolderDefinition([Out] [MarshalAs(UnmanagedType.Struct)] out KnownFoldersSafeNativeMethods.NativeFolderDefinition definition);
    }

    /// <summary>
    /// </summary>
    [ComImport]
    [Guid(KnownFoldersIidGuid.IKnownFolderManager)]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IKnownFolderManager
    {
        /// <summary>
        /// </summary>
        /// <param name="csidl">
        /// </param>
        /// <param name="knownFolderID">
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void FolderIdFromCsidl(int csidl, [Out] out Guid knownFolderID);

        /// <summary>
        /// </summary>
        /// <param name="id">
        /// </param>
        /// <param name="csidl">
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void FolderIdToCsidl([In] [MarshalAs(UnmanagedType.LPStruct)] Guid id, [Out] out int csidl);

        /// <summary>
        /// </summary>
        /// <param name="folders">
        /// </param>
        /// <param name="count">
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetFolderIds([Out] out IntPtr folders, [Out] out uint count);

        /// <summary>
        /// </summary>
        /// <param name="id">
        /// </param>
        /// <param name="knownFolder">
        /// </param>
        /// <returns>
        /// </returns>
        [PreserveSig]
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        HRESULT GetFolder([In] [MarshalAs(UnmanagedType.LPStruct)] Guid id, [Out] [MarshalAs(UnmanagedType.Interface)] out IKnownFolderNative knownFolder);

        /// <summary>
        /// </summary>
        /// <param name="canonicalName">
        /// </param>
        /// <param name="knownFolder">
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetFolderByName(string canonicalName, [Out] [MarshalAs(UnmanagedType.Interface)] out IKnownFolderNative knownFolder);

        /// <summary>
        /// </summary>
        /// <param name="knownFolderGuid">
        /// </param>
        /// <param name="knownFolderDefinition">
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void RegisterFolder(
            [In] [MarshalAs(UnmanagedType.LPStruct)] Guid knownFolderGuid, [In] ref KnownFoldersSafeNativeMethods.NativeFolderDefinition knownFolderDefinition);

        /// <summary>
        /// </summary>
        /// <param name="knownFolderGuid">
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void UnregisterFolder([In] [MarshalAs(UnmanagedType.LPStruct)] Guid knownFolderGuid);

        /// <summary>
        /// </summary>
        /// <param name="path">
        /// </param>
        /// <param name="mode">
        /// </param>
        /// <param name="knownFolder">
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void FindFolderFromPath(
            [In] [MarshalAs(UnmanagedType.LPWStr)] string path, [In] int mode, [Out] [MarshalAs(UnmanagedType.Interface)] out IKnownFolderNative knownFolder);

        /// <summary>
        /// </summary>
        /// <param name="pidl">
        /// </param>
        /// <param name="knownFolder">
        /// </param>
        /// <returns>
        /// </returns>
        [PreserveSig]
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        HRESULT FindFolderFromIDList(IntPtr pidl, [Out] [MarshalAs(UnmanagedType.Interface)] out IKnownFolderNative knownFolder);

        /// <summary>
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void Redirect();
    }

    /// <summary>
    /// </summary>
    [ComImport]
    [Guid("4df0c730-df9d-4ae3-9153-aa6b82e9795a")]
    internal class KnownFolderManagerClass : IKnownFolderManager
    {
        #region Implemented Interfaces

        #region IKnownFolderManager

        /// <summary>
        /// </summary>
        /// <param name="pidl">
        /// </param>
        /// <param name="knownFolder">
        /// </param>
        /// <returns>
        /// </returns>
        [PreserveSig]
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        public virtual extern HRESULT FindFolderFromIDList(IntPtr pidl, [Out] [MarshalAs(UnmanagedType.Interface)] out IKnownFolderNative knownFolder);

        /// <summary>
        /// </summary>
        /// <param name="path">
        /// </param>
        /// <param name="mode">
        /// </param>
        /// <param name="knownFolder">
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        public virtual extern void FindFolderFromPath(
            [In] [MarshalAs(UnmanagedType.LPWStr)] string path, [In] int mode, [Out] [MarshalAs(UnmanagedType.Interface)] out IKnownFolderNative knownFolder);

        /// <summary>
        /// </summary>
        /// <param name="csidl">
        /// </param>
        /// <param name="knownFolderID">
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        public virtual extern void FolderIdFromCsidl(int csidl, [Out] out Guid knownFolderID);

        /// <summary>
        /// </summary>
        /// <param name="id">
        /// </param>
        /// <param name="csidl">
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        public virtual extern void FolderIdToCsidl([In] [MarshalAs(UnmanagedType.LPStruct)] Guid id, [Out] out int csidl);

        /// <summary>
        /// </summary>
        /// <param name="id">
        /// </param>
        /// <param name="knownFolder">
        /// </param>
        /// <returns>
        /// </returns>
        [PreserveSig]
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        public virtual extern HRESULT GetFolder(
            [In] [MarshalAs(UnmanagedType.LPStruct)] Guid id, [Out] [MarshalAs(UnmanagedType.Interface)] out IKnownFolderNative knownFolder);

        /// <summary>
        /// </summary>
        /// <param name="canonicalName">
        /// </param>
        /// <param name="knownFolder">
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        public virtual extern void GetFolderByName(string canonicalName, [Out] [MarshalAs(UnmanagedType.Interface)] out IKnownFolderNative knownFolder);

        /// <summary>
        /// </summary>
        /// <param name="folders">
        /// </param>
        /// <param name="count">
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        public virtual extern void GetFolderIds([Out] out IntPtr folders, [Out] out uint count);

        /// <summary>
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        public virtual extern void Redirect();

        /// <summary>
        /// </summary>
        /// <param name="knownFolderGuid">
        /// </param>
        /// <param name="knownFolderDefinition">
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        public virtual extern void RegisterFolder(
            [In] [MarshalAs(UnmanagedType.LPStruct)] Guid knownFolderGuid, [In] ref KnownFoldersSafeNativeMethods.NativeFolderDefinition knownFolderDefinition);

        /// <summary>
        /// </summary>
        /// <param name="knownFolderGuid">
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        public virtual extern void UnregisterFolder([In] [MarshalAs(UnmanagedType.LPStruct)] Guid knownFolderGuid);

        #endregion

        #endregion
    }
}