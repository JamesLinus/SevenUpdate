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
    internal static class PropertySystemNativeMethods
    {
        #region Property Definitions

        /// <summary>
        /// </summary>
        internal enum PropdescRelativedescriptionType
        {
            /// <summary>
            /// </summary>
            PdrdtGeneral, 

            /// <summary>
            /// </summary>
            PdrdtDate, 

            /// <summary>
            /// </summary>
            PdrdtSize, 

            /// <summary>
            /// </summary>
            PdrdtCount, 

            /// <summary>
            /// </summary>
            PdrdtRevision, 

            /// <summary>
            /// </summary>
            PdrdtLength, 

            /// <summary>
            /// </summary>
            PdrdtDuration, 

            /// <summary>
            /// </summary>
            PdrdtSpeed, 

            /// <summary>
            /// </summary>
            PdrdtRate, 

            /// <summary>
            /// </summary>
            PdrdtRating, 

            /// <summary>
            /// </summary>
            PdrdtPriority
        }

        #endregion

        #region Property System Helpers

        /// <summary>
        /// </summary>
        /// <param name="propkey">
        /// </param>
        /// <param name="ppszCanonicalName">
        /// </param>
        /// <returns>
        /// </returns>
        [DllImport("propsys.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern int PSGetNameFromPropertyKey(ref PropertyKey propkey, [Out] [MarshalAs(UnmanagedType.LPWStr)] out string ppszCanonicalName);

        /// <summary>
        /// </summary>
        /// <param name="propkey">
        /// </param>
        /// <param name="riid">
        /// </param>
        /// <param name="ppv">
        /// </param>
        /// <returns>
        /// </returns>
        [DllImport("propsys.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern HRESULT PSGetPropertyDescription(
            ref PropertyKey propkey, ref Guid riid, [Out] [MarshalAs(UnmanagedType.Interface)] out IPropertyDescription ppv);

        /// <summary>
        /// </summary>
        /// <param name="pszCanonicalName">
        /// </param>
        /// <param name="propkey">
        /// </param>
        /// <returns>
        /// </returns>
        [DllImport("propsys.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern int PSGetPropertyKeyFromName([In] [MarshalAs(UnmanagedType.LPWStr)] string pszCanonicalName, out PropertyKey propkey);

        /// <summary>
        /// </summary>
        /// <param name="pszPropList">
        /// </param>
        /// <param name="riid">
        /// </param>
        /// <param name="ppv">
        /// </param>
        /// <returns>
        /// </returns>
        [DllImport("propsys.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern int PSGetPropertyDescriptionListFromString(
            [In] [MarshalAs(UnmanagedType.LPWStr)] string pszPropList, [In] ref Guid riid, out IPropertyDescriptionList ppv);

        #endregion
    }
}