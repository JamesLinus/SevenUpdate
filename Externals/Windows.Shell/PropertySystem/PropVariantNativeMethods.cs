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
    using global::System.Runtime.InteropServices;

    using Microsoft.Windows.Internal;

    /// <summary>
    /// </summary>
    internal static class PropVariantNativeMethods
    {
        /// <summary>
        /// </summary>
        /// <param name="pvar">
        /// </param>
        [DllImport("Ole32.dll", PreserveSig = false)] // returns hresult
        internal static extern void PropVariantClear([In] [Out] ref PropVariant pvar);

        /// <summary>
        /// </summary>
        /// <param name="vt">
        /// </param>
        /// <param name="lowerBound">
        /// </param>
        /// <param name="cElems">
        /// </param>
        /// <returns>
        /// </returns>
        [DllImport("OleAut32.dll", PreserveSig = true)] // psa is actually returned, not hresult
        internal static extern IntPtr SafeArrayCreateVector(ushort vt, int lowerBound, uint cElems);

        /// <summary>
        /// </summary>
        /// <param name="psa">
        /// </param>
        /// <returns>
        /// </returns>
        [DllImport("OleAut32.dll", PreserveSig = false)] // returns hresult
        internal static extern IntPtr SafeArrayAccessData(IntPtr psa);

        /// <summary>
        /// </summary>
        /// <param name="psa">
        /// </param>
        [DllImport("OleAut32.dll", PreserveSig = false)] // returns hresult
        internal static extern void SafeArrayUnaccessData(IntPtr psa);

        /// <summary>
        /// </summary>
        /// <param name="psa">
        /// </param>
        /// <returns>
        /// </returns>
        [DllImport("OleAut32.dll", PreserveSig = true)] // retuns uint32
        internal static extern uint SafeArrayGetDim(IntPtr psa);

        /// <summary>
        /// </summary>
        /// <param name="psa">
        /// </param>
        /// <param name="nDim">
        /// </param>
        /// <returns>
        /// </returns>
        [DllImport("OleAut32.dll", PreserveSig = false)] // returns hresult
        internal static extern int SafeArrayGetLBound(IntPtr psa, uint nDim);

        /// <summary>
        /// </summary>
        /// <param name="psa">
        /// </param>
        /// <param name="nDim">
        /// </param>
        /// <returns>
        /// </returns>
        [DllImport("OleAut32.dll", PreserveSig = false)] // returns hresult
        internal static extern int SafeArrayGetUBound(IntPtr psa, uint nDim);

        // This decl for SafeArrayGetElement is only valid for cDims==1!
        /// <summary>
        /// </summary>
        /// <param name="psa">
        /// </param>
        /// <param name="rgIndices">
        /// </param>
        /// <returns>
        /// </returns>
        [DllImport("OleAut32.dll", PreserveSig = false)] // returns hresult
        [return: MarshalAs(UnmanagedType.IUnknown)]
        internal static extern object SafeArrayGetElement(IntPtr psa, ref int rgIndices);

        /// <summary>
        /// </summary>
        /// <param name="propvarIn">
        /// </param>
        /// <param name="iElem">
        /// </param>
        /// <param name="ppropvar">
        /// </param>
        /// <returns>
        /// </returns>
        [DllImport("propsys.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern int InitPropVariantFromPropVariantVectorElem([In] ref PropVariant propvarIn, uint iElem, [Out] out PropVariant ppropvar);

        /// <summary>
        /// </summary>
        /// <param name="pftIn">
        /// </param>
        /// <param name="ppropvar">
        /// </param>
        /// <returns>
        /// </returns>
        [DllImport("propsys.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern uint InitPropVariantFromFileTime([In] ref FILETIME pftIn, out PropVariant ppropvar);

        /// <summary>
        /// </summary>
        /// <param name="propVar">
        /// </param>
        /// <returns>
        /// </returns>
        [DllImport("propsys.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.I4)]
        internal static extern int PropVariantGetElementCount([In] ref PropVariant propVar);

        /// <summary>
        /// </summary>
        /// <param name="propVar">
        /// </param>
        /// <param name="iElem">
        /// </param>
        /// <param name="pfVal">
        /// </param>
        [DllImport("propsys.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern void PropVariantGetBooleanElem([In] ref PropVariant propVar, [In] uint iElem, [Out] [MarshalAs(UnmanagedType.Bool)] out bool pfVal);

        /// <summary>
        /// </summary>
        /// <param name="propVar">
        /// </param>
        /// <param name="iElem">
        /// </param>
        /// <param name="pnVal">
        /// </param>
        [DllImport("propsys.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern void PropVariantGetInt16Elem([In] ref PropVariant propVar, [In] uint iElem, [Out] out short pnVal);

        /// <summary>
        /// </summary>
        /// <param name="propVar">
        /// </param>
        /// <param name="iElem">
        /// </param>
        /// <param name="pnVal">
        /// </param>
        [DllImport("propsys.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern void PropVariantGetUInt16Elem([In] ref PropVariant propVar, [In] uint iElem, [Out] out ushort pnVal);

        /// <summary>
        /// </summary>
        /// <param name="propVar">
        /// </param>
        /// <param name="iElem">
        /// </param>
        /// <param name="pnVal">
        /// </param>
        [DllImport("propsys.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern void PropVariantGetInt32Elem([In] ref PropVariant propVar, [In] uint iElem, [Out] out int pnVal);

        /// <summary>
        /// </summary>
        /// <param name="propVar">
        /// </param>
        /// <param name="iElem">
        /// </param>
        /// <param name="pnVal">
        /// </param>
        [DllImport("propsys.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern void PropVariantGetUInt32Elem([In] ref PropVariant propVar, [In] uint iElem, [Out] out uint pnVal);

        /// <summary>
        /// </summary>
        /// <param name="propVar">
        /// </param>
        /// <param name="iElem">
        /// </param>
        /// <param name="pnVal">
        /// </param>
        [DllImport("propsys.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern void PropVariantGetInt64Elem([In] ref PropVariant propVar, [In] uint iElem, [Out] out long pnVal);

        /// <summary>
        /// </summary>
        /// <param name="propVar">
        /// </param>
        /// <param name="iElem">
        /// </param>
        /// <param name="pnVal">
        /// </param>
        [DllImport("propsys.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern void PropVariantGetUInt64Elem([In] ref PropVariant propVar, [In] uint iElem, [Out] out ulong pnVal);

        /// <summary>
        /// </summary>
        /// <param name="propVar">
        /// </param>
        /// <param name="iElem">
        /// </param>
        /// <param name="pnVal">
        /// </param>
        [DllImport("propsys.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern void PropVariantGetDoubleElem([In] ref PropVariant propVar, [In] uint iElem, [Out] out double pnVal);

        /// <summary>
        /// </summary>
        /// <param name="propVar">
        /// </param>
        /// <param name="iElem">
        /// </param>
        /// <param name="pftVal">
        /// </param>
        [DllImport("propsys.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern void PropVariantGetFileTimeElem(
            [In] ref PropVariant propVar, [In] uint iElem, [Out] [MarshalAs(UnmanagedType.Struct)] out global::System.Runtime.InteropServices.ComTypes.FILETIME pftVal);

        /// <summary>
        /// </summary>
        /// <param name="propVar">
        /// </param>
        /// <param name="iElem">
        /// </param>
        /// <param name="ppszVal">
        /// </param>
        [DllImport("propsys.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern void PropVariantGetStringElem([In] ref PropVariant propVar, [In] uint iElem, [Out] [MarshalAs(UnmanagedType.LPWStr)] out string ppszVal);

        /// <summary>
        /// </summary>
        /// <param name="prgf">
        /// </param>
        /// <param name="cElems">
        /// </param>
        /// <param name="ppropvar">
        /// </param>
        [DllImport("propsys.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern void InitPropVariantFromBooleanVector([In] [Out] bool[] prgf, uint cElems, out PropVariant ppropvar);

        /// <summary>
        /// </summary>
        /// <param name="prgn">
        /// </param>
        /// <param name="cElems">
        /// </param>
        /// <param name="ppropvar">
        /// </param>
        [DllImport("propsys.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern void InitPropVariantFromInt16Vector([In] [Out] short[] prgn, uint cElems, out PropVariant ppropvar);

        /// <summary>
        /// </summary>
        /// <param name="prgn">
        /// </param>
        /// <param name="cElems">
        /// </param>
        /// <param name="ppropvar">
        /// </param>
        [DllImport("propsys.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern void InitPropVariantFromUInt16Vector([In] [Out] ushort[] prgn, uint cElems, out PropVariant ppropvar);

        /// <summary>
        /// </summary>
        /// <param name="prgn">
        /// </param>
        /// <param name="cElems">
        /// </param>
        /// <param name="ppropvar">
        /// </param>
        [DllImport("propsys.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern void InitPropVariantFromInt32Vector([In] [Out] int[] prgn, uint cElems, out PropVariant ppropvar);

        /// <summary>
        /// </summary>
        /// <param name="prgn">
        /// </param>
        /// <param name="cElems">
        /// </param>
        /// <param name="ppropvar">
        /// </param>
        [DllImport("propsys.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern void InitPropVariantFromUInt32Vector([In] [Out] uint[] prgn, uint cElems, out PropVariant ppropvar);

        /// <summary>
        /// </summary>
        /// <param name="prgn">
        /// </param>
        /// <param name="cElems">
        /// </param>
        /// <param name="ppropvar">
        /// </param>
        [DllImport("propsys.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern void InitPropVariantFromInt64Vector([In] [Out] long[] prgn, uint cElems, out PropVariant ppropvar);

        /// <summary>
        /// </summary>
        /// <param name="prgn">
        /// </param>
        /// <param name="cElems">
        /// </param>
        /// <param name="ppropvar">
        /// </param>
        [DllImport("propsys.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern void InitPropVariantFromUInt64Vector([In] [Out] ulong[] prgn, uint cElems, out PropVariant ppropvar);

        /// <summary>
        /// </summary>
        /// <param name="prgn">
        /// </param>
        /// <param name="cElems">
        /// </param>
        /// <param name="ppropvar">
        /// </param>
        [DllImport("propsys.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern void InitPropVariantFromDoubleVector([In] [Out] double[] prgn, uint cElems, out PropVariant ppropvar);

        /// <summary>
        /// </summary>
        /// <param name="prgft">
        /// </param>
        /// <param name="cElems">
        /// </param>
        /// <param name="ppropvar">
        /// </param>
        [DllImport("propsys.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern void InitPropVariantFromFileTimeVector(
            [In] [Out] global::System.Runtime.InteropServices.ComTypes.FILETIME[] prgft, uint cElems, out PropVariant ppropvar);

        /// <summary>
        /// </summary>
        /// <param name="prgsz">
        /// </param>
        /// <param name="cElems">
        /// </param>
        /// <param name="ppropvar">
        /// </param>
        [DllImport("propsys.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern void InitPropVariantFromStringVector([In] [Out] string[] prgsz, uint cElems, out PropVariant ppropvar);
    }
}