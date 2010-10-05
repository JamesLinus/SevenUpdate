//***********************************************************************
// Assembly         : Windows.Shell
// Author           : sevenalive
// Created          : 09-17-2010
// Last Modified By : sevenalive
// Last Modified On : 10-05-2010
// Description      : 
// Copyright        : (c) Seven Software. All rights reserved.
//***********************************************************************

namespace Microsoft.Windows.Internal
{
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// HRESULT Wrapper
    ///   This is intended for Library Internal use only.
    /// </summary>
    [SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "HRESULT")]
    [SuppressMessage("Microsoft.Design", "CA1028:EnumStorageShouldBeInt32", Justification = "The base type for all of these value is uint")]
    public enum HRESULT : uint
    {
        /// <summary>
        ///   S_FALSE
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
        [SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "FALSE")]
        SFalse = 0x0001, 

        /// <summary>
        ///   S_OK
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
        [SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "OK")]
        S_OK = 0x0000, 

        /// <summary>
        ///   E_INVALIDARG
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
        [SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "INVALIDARG")]
        EInvalidarg = 0x80070057, 

        /// <summary>
        ///   E_OUTOFMEMORY
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
        [SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "OUTOFMEMORY")]
        EOutofmemory = 0x8007000E, 

        /// <summary>
        ///   E_NOINTERFACE
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
        [SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "NOINTERFACE")]
        ENointerface = 0x80004002, 

        /// <summary>
        ///   E_FAIL
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
        [SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "FAIL")]
        EFail = 0x80004005, 

        /// <summary>
        ///   E_ELEMENTNOTFOUND
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
        [SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "ELEMENTNOTFOUND")]
        EElementnotfound = 0x80070490, 

        /// <summary>
        ///   TYPE_E_ELEMENTNOTFOUND
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
        [SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "TYPE")]
        TypeEElementnotfound = 0x8002802B, 

        /// <summary>
        ///   NO_OBJECT
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
        [SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "NO_OBJECT")]
        NOObject = 0x800401E5, 

        /// <summary>
        ///   Win32 Error code: ERROR_CANCELLED
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
        [SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "ERROR")]
        [SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "CANCELLED")]
        ErrorCancelled = 1223, 

        /// <summary>
        ///   ERROR_CANCELLED
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores")]
        [SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "ERROR")]
        [SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "CANCELLED")]
        EErrorCancelled = 0x800704C7, 

        /// <summary>
        ///   The requested resource is in use
        /// </summary>
        ResourceinUse = 0x800700AA, 
    }

    /// <summary>
    /// Provide Error Message Helper Methods.
    ///   This is intended for Library Internal use only.
    /// </summary>
    public static class CoreErrorHelper
    {
        #region Constants and Fields

        /// <summary>
        ///   This is intended for Library Internal use only.
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "IGNORED")]
        public const int IGNORED = (int)HRESULT.S_OK;

        /// <summary>
        ///   This is intended for Library Internal use only.
        /// </summary>
        private const int FacilityWIN32 = 7;

        #endregion

        #region Public Methods

        /// <summary>
        /// This is intended for Library Internal use only.
        /// </summary>
        /// <param name="hResult">
        /// The error code.
        /// </param>
        /// <returns>
        /// True if the error code indicates failure.
        /// </returns>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "h")]
        public static bool Failed(HRESULT hResult)
        {
            return (int)hResult < 0;
        }

        /// <summary>
        /// This is intended for Library Internal use only.
        /// </summary>
        /// <param name="win32ErrorCode">
        /// The Windows API error code.
        /// </param>
        /// <returns>
        /// The equivalent HRESULT.
        /// </returns>
        public static int HResultFromWin32(int win32ErrorCode)
        {
            if (win32ErrorCode > 0)
            {
                win32ErrorCode = (int)(((uint)win32ErrorCode & 0x0000FFFF) | (FacilityWIN32 << 16) | 0x80000000);
            }

            return win32ErrorCode;
        }

        /// <summary>
        /// This is intended for Library Internal use only.
        /// </summary>
        /// <param name="hresult">
        /// The COM error code.
        /// </param>
        /// <param name="win32ErrorCode">
        /// The Win32 error code.
        /// </param>
        /// <returns>
        /// Inticates that the Win32 error code corresponds to the COM error code.
        /// </returns>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "hresult")]
        public static bool Matches(int hresult, int win32ErrorCode)
        {
            return hresult == HResultFromWin32(win32ErrorCode);
        }

        /// <summary>
        /// This is intended for Library Internal use only.
        /// </summary>
        /// <param name="hresult">
        /// The error code.
        /// </param>
        /// <returns>
        /// True if the error code indicates success.
        /// </returns>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "hresult")]
        public static bool Succeeded(int hresult)
        {
            return hresult >= 0;
        }

        #endregion
    }
}