// ***********************************************************************
// <copyright file="ErrorHelper.cs" project="SevenSoftware.Windows" assembly="SevenSoftware.Windows" solution="SevenUpdate" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// <license href="http://code.msdn.microsoft.com/WindowsAPICodePack/Project/License.aspx">Microsoft Software License</license>
// ***********************************************************************

namespace SevenSoftware.Windows.Internal
{
    /// <summary>Provide Error Message Helper Methods.This is intended for Library Internal use only.</summary>
    public static class ErrorHelper
    {
        /// <summary>This is intended for Library Internal use only.</summary>
        public const int Ignored = (int)Result.Ok;

        /// <summary>This is intended for Library Internal use only.</summary>
        /// <param name="result">The error code.</param>
        /// <returns>True if the error code indicates failure.</returns>
        public static bool Failed(Result result)
        {
            return !Succeeded(result);
        }

        /// <summary>This is intended for Library Internal use only.</summary>
        /// <param name="result">The error code.</param>
        /// <returns>True if the error code indicates failure.</returns>
        public static bool Failed(int result)
        {
            return !Succeeded(result);
        }

        /// <summary>This is intended for Library Internal use only.</summary>
        /// <param name="win32ErrorCode">The Windows API error code.</param>
        /// <returns>The equivalent HRESULT.</returns>
        public static int HResultFromWin32(int win32ErrorCode)
        {
            if (win32ErrorCode > 0)
            {
                win32ErrorCode = (int)(((uint)win32ErrorCode & 0x0000FFFF) | (7 << 16) | 0x80000000);
            }

            return win32ErrorCode;
        }

        /// <summary>This is intended for Library Internal use only.</summary>
        /// <param name="result">The COM error code.</param>
        /// <param name="win32ErrorCode">The Win32 error code.</param>
        /// <returns>Inticates that the Win32 error code corresponds to the COM error code.</returns>
        public static bool Matches(int result, int win32ErrorCode)
        {
            return result == HResultFromWin32(win32ErrorCode);
        }

        /// <summary>This is intended for Library Internal use only.</summary>
        /// <param name="result">The error code.</param>
        /// <returns>True if the error code indicates success.</returns>
        public static bool Succeeded(int result)
        {
            return result >= 0;
        }

        /// <summary>This is intended for Library Internal use only.</summary>
        /// <param name="result">The error code.</param>
        /// <returns>True if the error code indicates success.</returns>
        public static bool Succeeded(Result result)
        {
            return Succeeded((int)result);
        }
    }
}