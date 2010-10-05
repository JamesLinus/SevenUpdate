//***********************************************************************
// Assembly         : Windows.Shell
// Author           : sevenalive
// Created          : 09-17-2010
// Last Modified By : sevenalive
// Last Modified On : 10-05-2010
// Description      : 
// Copyright        : (c) Seven Software. All rights reserved.
//***********************************************************************

namespace Microsoft.Windows.Shell.PropertySystem
{
    using global::System;
    using global::System.Runtime.CompilerServices;
    using global::System.Runtime.InteropServices;

    using Microsoft.Windows.Internal;

    // Disable warning if a method declaration hides another inherited from a parent COM interface
    // To successfully import a COM interface, all inherited methods need to be declared again with 
    // the exception of those already declared in "IUnknown"
#pragma warning disable 108

    #region Property System COM Interfaces

    /// <summary>
    /// </summary>
    [ComImport]
    [Guid(ShellIidGuid.IPropertyStore)]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IPropertyStore
    {
        /// <summary>
        /// </summary>
        /// <param name="cProps">
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetCount([Out] out uint cProps);

        /// <summary>
        /// </summary>
        /// <param name="iProp">
        /// </param>
        /// <param name="pkey">
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetAt([In] uint iProp, out PropertyKey pkey);

        /// <summary>
        /// </summary>
        /// <param name="key">
        /// </param>
        /// <param name="pv">
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetValue([In] ref PropertyKey key, out PropVariant pv);

        /// <summary>
        /// </summary>
        /// <param name="key">
        /// </param>
        /// <param name="pv">
        /// </param>
        /// <returns>
        /// </returns>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        [PreserveSig]
        [return: MarshalAs(UnmanagedType.I4)]
        int SetValue([In] ref PropertyKey key, [In] ref PropVariant pv);

        /// <summary>
        /// </summary>
        /// <returns>
        /// </returns>
        [PreserveSig]
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        HRESULT Commit();
    }

    /// <summary>
    /// </summary>
    [ComImport]
    [Guid(ShellIidGuid.IPropertyDescriptionList)]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IPropertyDescriptionList
    {
        /// <summary>
        /// </summary>
        /// <param name="pcElem">
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetCount(out uint pcElem);

        /// <summary>
        /// </summary>
        /// <param name="iElem">
        /// </param>
        /// <param name="riid">
        /// </param>
        /// <param name="ppv">
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetAt([In] uint iElem, [In] ref Guid riid, [MarshalAs(UnmanagedType.Interface)] out IPropertyDescription ppv);
    }

    /// <summary>
    /// </summary>
    [ComImport]
    [Guid(ShellIidGuid.IPropertyDescription)]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IPropertyDescription
    {
        /// <summary>
        /// </summary>
        /// <param name="pkey">
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetPropertyKey(out PropertyKey pkey);

        /// <summary>
        /// </summary>
        /// <param name="ppszName">
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetCanonicalName([MarshalAs(UnmanagedType.LPWStr)] out string ppszName);

        /// <summary>
        /// </summary>
        /// <param name="pvartype">
        /// </param>
        /// <returns>
        /// </returns>
        [PreserveSig]
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        HRESULT GetPropertyType(out VarEnum pvartype);

        /// <summary>
        /// </summary>
        /// <param name="ppszName">
        /// </param>
        /// <returns>
        /// </returns>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        [PreserveSig]
        HRESULT GetDisplayName(out IntPtr ppszName);

        /// <summary>
        /// </summary>
        /// <param name="ppszInvite">
        /// </param>
        /// <returns>
        /// </returns>
        [PreserveSig]
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        HRESULT GetEditInvitation(out IntPtr ppszInvite);

        /// <summary>
        /// </summary>
        /// <param name="mask">
        /// </param>
        /// <param name="ppdtFlags">
        /// </param>
        /// <returns>
        /// </returns>
        [PreserveSig]
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        HRESULT GetTypeFlags([In] PropertyTypeFlags mask, out PropertyTypeFlags ppdtFlags);

        /// <summary>
        /// </summary>
        /// <param name="ppdvFlags">
        /// </param>
        /// <returns>
        /// </returns>
        [PreserveSig]
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        HRESULT GetViewFlags(out PropertyViewFlags ppdvFlags);

        /// <summary>
        /// </summary>
        /// <param name="pcxChars">
        /// </param>
        /// <returns>
        /// </returns>
        [PreserveSig]
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        HRESULT GetDefaultColumnWidth(out uint pcxChars);

        /// <summary>
        /// </summary>
        /// <param name="pdisplaytype">
        /// </param>
        /// <returns>
        /// </returns>
        [PreserveSig]
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        HRESULT GetDisplayType(out PropertyDisplayType pdisplaytype);

        /// <summary>
        /// </summary>
        /// <param name="pcsFlags">
        /// </param>
        /// <returns>
        /// </returns>
        [PreserveSig]
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        HRESULT GetColumnState(out PropertyColumnState pcsFlags);

        /// <summary>
        /// </summary>
        /// <param name="pgr">
        /// </param>
        /// <returns>
        /// </returns>
        [PreserveSig]
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        HRESULT GetGroupingRange(out PropertyGroupingRange pgr);

        /// <summary>
        /// </summary>
        /// <param name="prdt">
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetRelativeDescriptionType(out PropertySystemNativeMethods.PropdescRelativedescriptionType prdt);

        /// <summary>
        /// </summary>
        /// <param name="propvar1">
        /// </param>
        /// <param name="propvar2">
        /// </param>
        /// <param name="ppszDesc1">
        /// </param>
        /// <param name="ppszDesc2">
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetRelativeDescription(
            [In] ref PropVariant propvar1, 
            [In] ref PropVariant propvar2, 
            [MarshalAs(UnmanagedType.LPWStr)] out string ppszDesc1, 
            [MarshalAs(UnmanagedType.LPWStr)] out string ppszDesc2);

        /// <summary>
        /// </summary>
        /// <param name="psd">
        /// </param>
        /// <returns>
        /// </returns>
        [PreserveSig]
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        HRESULT GetSortDescription(out PropertySortDescription psd);

        /// <summary>
        /// </summary>
        /// <param name="fDescending">
        /// </param>
        /// <param name="ppszDescription">
        /// </param>
        /// <returns>
        /// </returns>
        [PreserveSig]
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        HRESULT GetSortDescriptionLabel([In] bool fDescending, out IntPtr ppszDescription);

        /// <summary>
        /// </summary>
        /// <param name="paggtype">
        /// </param>
        /// <returns>
        /// </returns>
        [PreserveSig]
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        HRESULT GetAggregationType(out PropertyAggregationType paggtype);

        /// <summary>
        /// </summary>
        /// <param name="pcontype">
        /// </param>
        /// <param name="popDefault">
        /// </param>
        /// <returns>
        /// </returns>
        [PreserveSig]
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        HRESULT GetConditionType(out PropertyConditionType pcontype, out PropertyConditionOperation popDefault);

        /// <summary>
        /// </summary>
        /// <param name="riid">
        /// </param>
        /// <param name="ppv">
        /// </param>
        /// <returns>
        /// </returns>
        [PreserveSig]
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        HRESULT GetEnumTypeList([In] ref Guid riid, [Out] [MarshalAs(UnmanagedType.Interface)] out IPropertyEnumTypeList ppv);

        /// <summary>
        /// </summary>
        /// <param name="propvar">
        /// </param>
        /// <param name="ppropvar">
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void CoerceToCanonicalValue([In] ref PropVariant propvar, out PropVariant ppropvar);

        /// <summary>
        /// </summary>
        /// <param name="propvar">
        /// </param>
        /// <param name="pdfFlags">
        /// </param>
        /// <param name="ppszDisplay">
        /// </param>
        /// <returns>
        /// </returns>
        [PreserveSig]
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        HRESULT FormatForDisplay([In] ref PropVariant propvar, [In] ref PropertyDescriptionFormat pdfFlags, [MarshalAs(UnmanagedType.LPWStr)] out string ppszDisplay);

        /// <summary>
        /// </summary>
        /// <param name="propvar">
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void IsValueCanonical([In] ref PropVariant propvar);
    }

    /// <summary>
    /// </summary>
    [ComImport]
    [Guid(ShellIidGuid.IPropertyDescription2)]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IPropertyDescription2 : IPropertyDescription
    {
        /// <summary>
        /// </summary>
        /// <param name="pkey">
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetPropertyKey(out PropertyKey pkey);

        /// <summary>
        /// </summary>
        /// <param name="ppszName">
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetCanonicalName([MarshalAs(UnmanagedType.LPWStr)] out string ppszName);

        /// <summary>
        /// </summary>
        /// <param name="pvartype">
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetPropertyType(out VarEnum pvartype);

        /// <summary>
        /// </summary>
        /// <param name="ppszName">
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetDisplayName([MarshalAs(UnmanagedType.LPWStr)] out string ppszName);

        /// <summary>
        /// </summary>
        /// <param name="ppszInvite">
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetEditInvitation([MarshalAs(UnmanagedType.LPWStr)] out string ppszInvite);

        /// <summary>
        /// </summary>
        /// <param name="mask">
        /// </param>
        /// <param name="ppdtFlags">
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetTypeFlags([In] PropertyTypeFlags mask, out PropertyTypeFlags ppdtFlags);

        /// <summary>
        /// </summary>
        /// <param name="ppdvFlags">
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetViewFlags(out PropertyViewFlags ppdvFlags);

        /// <summary>
        /// </summary>
        /// <param name="pcxChars">
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetDefaultColumnWidth(out uint pcxChars);

        /// <summary>
        /// </summary>
        /// <param name="pdisplaytype">
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetDisplayType(out PropertyDisplayType pdisplaytype);

        /// <summary>
        /// </summary>
        /// <param name="pcsFlags">
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetColumnState(out uint pcsFlags);

        /// <summary>
        /// </summary>
        /// <param name="pgr">
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetGroupingRange(out PropertyGroupingRange pgr);

        /// <summary>
        /// </summary>
        /// <param name="prdt">
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetRelativeDescriptionType(out PropertySystemNativeMethods.PropdescRelativedescriptionType prdt);

        /// <summary>
        /// </summary>
        /// <param name="propvar1">
        /// </param>
        /// <param name="propvar2">
        /// </param>
        /// <param name="ppszDesc1">
        /// </param>
        /// <param name="ppszDesc2">
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetRelativeDescription(
            [In] ref PropVariant propvar1, 
            [In] ref PropVariant propvar2, 
            [MarshalAs(UnmanagedType.LPWStr)] out string ppszDesc1, 
            [MarshalAs(UnmanagedType.LPWStr)] out string ppszDesc2);

        /// <summary>
        /// </summary>
        /// <param name="psd">
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetSortDescription(out PropertySortDescription psd);

        /// <summary>
        /// </summary>
        /// <param name="fDescending">
        /// </param>
        /// <param name="ppszDescription">
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetSortDescriptionLabel([In] int fDescending, [MarshalAs(UnmanagedType.LPWStr)] out string ppszDescription);

        /// <summary>
        /// </summary>
        /// <param name="paggtype">
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetAggregationType(out PropertyAggregationType paggtype);

        /// <summary>
        /// </summary>
        /// <param name="pcontype">
        /// </param>
        /// <param name="popDefault">
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetConditionType(out PropertyConditionType pcontype, out PropertyConditionOperation popDefault);

        /// <summary>
        /// </summary>
        /// <param name="riid">
        /// </param>
        /// <param name="ppv">
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetEnumTypeList([In] ref Guid riid, out IntPtr ppv);

        /// <summary>
        /// </summary>
        /// <param name="propvar">
        /// </param>
        /// <param name="ppropvar">
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void CoerceToCanonicalValue([In] ref PropVariant propvar, out PropVariant ppropvar);

        /// <summary>
        /// </summary>
        /// <param name="propvar">
        /// </param>
        /// <param name="pdfFlags">
        /// </param>
        /// <param name="ppszDisplay">
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void FormatForDisplay([In] ref PropVariant propvar, [In] ref PropertyDescriptionFormat pdfFlags, [MarshalAs(UnmanagedType.LPWStr)] out string ppszDisplay);

        /// <summary>
        /// </summary>
        /// <param name="propvar">
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void IsValueCanonical([In] ref PropVariant propvar);

        /// <summary>
        /// </summary>
        /// <param name="propvar">
        /// </param>
        /// <param name="ppszImageRes">
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetImageReferenceForValue(ref PropVariant propvar, [Out] [MarshalAs(UnmanagedType.LPWStr)] out string ppszImageRes);
    }

    /// <summary>
    /// </summary>
    [ComImport]
    [Guid(ShellIidGuid.IPropertyEnumType)]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IPropertyEnumType
    {
        /// <summary>
        /// </summary>
        /// <param name="penumtype">
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetEnumType([Out] out PropEnumType penumtype);

        /// <summary>
        /// </summary>
        /// <param name="ppropvar">
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetValue([Out] out PropVariant ppropvar);

        /// <summary>
        /// </summary>
        /// <param name="ppropvar">
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetRangeMinValue([Out] out PropVariant ppropvar);

        /// <summary>
        /// </summary>
        /// <param name="ppropvar">
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetRangeSetValue([Out] out PropVariant ppropvar);

        /// <summary>
        /// </summary>
        /// <param name="ppszDisplay">
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetDisplayText([Out] [MarshalAs(UnmanagedType.LPWStr)] out string ppszDisplay);
    }

    /// <summary>
    /// </summary>
    /// <summary>
    /// </summary>
    [ComImport]
    [Guid(ShellIidGuid.IPropertyEnumTypeList)]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IPropertyEnumTypeList
    {
        /// <summary>
        /// </summary>
        /// <param name="pctypes">
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetCount([Out] out uint pctypes);

        /// <summary>
        /// </summary>
        /// <param name="itype">
        /// </param>
        /// <param name="riid">
        /// </param>
        /// <param name="ppv">
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetAt(
            [In] uint itype, 
            [In] ref Guid riid, 
            // riid may be IID_IPropertyEnumType
            [Out] [MarshalAs(UnmanagedType.Interface)] out IPropertyEnumType ppv);

        /// <summary>
        /// </summary>
        /// <param name="index">
        /// </param>
        /// <param name="riid">
        /// </param>
        /// <param name="ppv">
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void GetConditionAt([In] uint index, [In] ref Guid riid, out IntPtr ppv);

        /// <summary>
        /// </summary>
        /// <param name="propvarCmp">
        /// </param>
        /// <param name="pnIndex">
        /// </param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        void FindMatchingIndex([In] ref PropVariant propvarCmp, [Out] out uint pnIndex);
    }

    #endregion

#pragma warning restore 108
}