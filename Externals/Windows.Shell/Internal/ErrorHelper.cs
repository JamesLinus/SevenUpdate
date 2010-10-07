// ***********************************************************************
// Assembly         : System.Windows
// Author           : Microsoft Corporation
// Last Modified By : Robert Baker (sevenalive)
// Last Modified On : 10-06-2010
// Copyright        : (c) Microsoft Corporation. All rights reserved.
// ***********************************************************************
namespace System.Windows.Internal
{
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// HRESULT Wrapper
    ///   This is intended for Library Internal use only.
    /// </summary>
    public enum Result : uint
    {
        /// <summary>
        ///   Result returns false
        /// </summary>
        False = 0x0001, 

        /// <summary>
        ///   Returns OK
        /// </summary>
        OK = 0x0000, 

        /// <summary>
        ///   Returns invalid argument
        /// </summary>
        InvalidArg = 0x80070057, 

        /// <summary>
        ///   Returns out of memory
        /// </summary>
        OutOfMemory = 0x8007000E, 

        /// <summary>
        ///   Returns No Interface
        /// </summary>
        NoInterface = 0x80004002, 

        /// <summary>
        ///   Returns operation failed
        /// </summary>
        Fail = 0x80004005, 

        /// <summary>
        ///   Returns element not found
        /// </summary>
        ElementNotFound = 0x80070490, 

        /// <summary>
        ///   Returns the type element failed
        /// </summary>
        TypeElementNotFound = 0x8002802B, 

        /// <summary>
        ///   Returns no object
        /// </summary>
        NoObject = 0x800401E5, 

        /// <summary>
        ///   Win32 Error code: error canceled
        /// </summary>
        Win32ErrorCanceled = 1223, 

        /// <summary>
        ///   Error canceled
        /// </summary>
        ErrorCanceled = 0x800704C7, 

        /// <summary>
        ///   The requested resource is in use
        /// </summary>
        ResourceInUse = 0x800700AA, 
    }

    /// <summary>
    /// Provide Error Message Helper Methods.
    ///   This is intended for Library Internal use only.
    /// </summary>
    public static class ErrorHelper
    {
        #region Constants and Fields

        /// <summary>
        ///   This is intended for Library Internal use only.
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "IGNORED", Justification = "OK")]
        public const int Ignored = (int)Result.OK;

        /// <summary>
        ///   This is intended for Library Internal use only.
        /// </summary>
        private const int FacilityWin32 = 7;

        #endregion

        #region Public Methods

        /// <summary>
        /// This is intended for Library Internal use only.
        /// </summary>
        /// <parameter name="result">
        /// The error code.
        /// </parameter>
        /// <returns>
        /// True if the error code indicates failure.
        /// </returns>
        public static bool Failed(Result result)
        {
            return (int)result < 0;
        }

        /// <summary>
        /// This is intended for Library Internal use only.
        /// </summary>
        /// <parameter name="win32ErrorCode">
        /// The Windows API error code.
        /// </parameter>
        /// <returns>
        /// The equivalent HRESULT.
        /// </returns>
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
        /// <parameter name="result">
        /// The COM error code.
        /// </parameter>
        /// <parameter name="win32ErrorCode">
        /// The Win32 error code.
        /// </parameter>
        /// <returns>
        /// Indicates that the Win32 error code corresponds to the COM error code.
        /// </returns>
        public static bool Matches(int result, int win32ErrorCode)
        {
            return result == HResultFromWin32(win32ErrorCode);
        }

        /// <summary>
        /// This is intended for Library Internal use only.
        /// </summary>
        /// <parameter name="result">
        /// The error code.
        /// </parameter>
        /// <returns>
        /// True if the error code indicates success.
        /// </returns>
        public static bool Succeeded(int result)
        {
            return result >= 0;
        }

        #endregion
    }
}