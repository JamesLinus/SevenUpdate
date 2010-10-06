// ***********************************************************************
// Assembly         : Windows.Shell
// Author           : Microsoft
// Created          : 09-17-2010
// Last Modified By : sevenalive (Robert Baker)
// Last Modified On : 10-05-2010
// Description      : 
// Copyright        : (c) Microsoft Corporation. All rights reserved.
// ***********************************************************************

namespace Microsoft.Windows.Internal
{
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// HRESULT Wrapper
    ///   This is intended for Library Internal use only.
    /// </summary>
    [SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "HRESULT", Justification ="OK")]
    [SuppressMessage("Microsoft.Design", "CA1028:EnumStorageShouldBeInt32", Justification = "The base type for all of these value is uint")]
    public enum HRESULT : uint
    {
        /// <summary>
        ///   S_FALSE
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = "OK")]
        [SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "FALSE", Justification = "OK")]
        False = 0x0001, 

        /// <summary>
        ///   S_OK
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = "OK")]
        [SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "OK", Justification = "OK")]
        OK = 0x0000, 

        /// <summary>
        ///   E_INVALIDARG
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = "OK")]
        [SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "INVALIDARG", Justification = "OK")]
        InvalidArg = 0x80070057, 

        /// <summary>
        ///   E_OUTOFMEMORY
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = "OK")]
        [SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "OUTOFMEMORY", Justification = "OK")]
        OutofMemory = 0x8007000E, 

        /// <summary>
        ///   E_NOINTERFACE
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = "OK")]
        [SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "NOINTERFACE", Justification = "OK")]
        NoInterface = 0x80004002, 

        /// <summary>
        ///   E_FAIL
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = "OK")]
        [SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "FAIL", Justification = "OK")]
        Fail = 0x80004005, 

        /// <summary>
        ///   E_ELEMENTNOTFOUND
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = "OK")]
        [SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "ELEMENTNOTFOUND", Justification = "OK")]
        ElementNotFound = 0x80070490, 

        /// <summary>
        ///   TYPE_E_ELEMENTNOTFOUND
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = "OK")]
        [SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "TYPE", Justification = "OK")]
        TypeElementNotFound = 0x8002802B, 

        /// <summary>
        ///   NO_OBJECT
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = "OK")]
        [SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "NO_OBJECT", Justification = "OK")]
        NoObject = 0x800401E5, 

        /// <summary>
        ///   Win32 Error code: ERROR_CANCELLED
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = "OK")]
        [SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "ERROR", Justification = "OK")]
        [SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "CANCELLED", Justification = "OK")]
        Win32ErrorCancelled = 1223, 

        /// <summary>
        ///   ERROR_CANCELLED
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = "OK")]
        [SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "ERROR", Justification = "OK")]
        [SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "CANCELLED", Justification = "OK")]
        ErrorCancelled = 0x800704C7, 

        /// <summary>
        ///   The requested resource is in use
        /// </summary>
        ResourceInUse = 0x800700AA, 
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
        [SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "IGNORED", Justification = "OK")]
        public const int Ignored = (int)HRESULT.OK;

        /// <summary>
        ///   This is intended for Library Internal use only.
        /// </summary>
        private const int FacilityWin32 = 7;

        #endregion

        #region Public Methods

        /// <summary>
        /// This is intended for Library Internal use only.
        /// </summary>
        /// <param name="result">The error code.</param>
        /// <returns>
        /// True if the error code indicates failure.
        /// </returns>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "h", Justification = "OK")]
        public static bool Failed(HRESULT result)
        {
            return (int)result < 0;
        }

        /// <summary>
        /// This is intended for Library Internal use only.
        /// </summary>
        /// <param name="win32ErrorCode">The Windows API error code.</param>
        /// <returns>The equivalent HRESULT.</returns>
        public static int HResultFromWin32(int win32ErrorCode)
        {
            if (win32ErrorCode > 0)
            {
                win32ErrorCode = (int)(((uint)win32ErrorCode & 0x0000FFFF) | (FacilityWin32 << 16) | 0x80000000);
            }

            return win32ErrorCode;
        }

        /// <summary>
        /// This is intended for Library Internal use only.
        /// </summary>
        /// <param name="result">The COM error code.</param>
        /// <param name="win32ErrorCode">The Win32 error code.</param>
        /// <returns>
        /// Indicates that the Win32 error code corresponds to the COM error code.
        /// </returns>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "result", Justification = "OK")]
        public static bool Matches(int result, int win32ErrorCode)
        {
            return result == HResultFromWin32(win32ErrorCode);
        }

        /// <summary>
        /// This is intended for Library Internal use only.
        /// </summary>
        /// <param name="result">
        /// The error code.
        /// </param>
        /// <returns>
        /// True if the error code indicates success.
        /// </returns>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "result", Justification = "OK")]
        public static bool Succeeded(int result)
        {
            return result >= 0;
        }

        #endregion
    }
}