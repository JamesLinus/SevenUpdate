// ***********************************************************************
// <copyright file="ErrorHelper.cs" project="System.Windows" assembly="System.Windows" solution="SevenUpdate" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// <license href="http://code.msdn.microsoft.com/WindowsAPICodePack/Project/License.aspx">Microsoft Software License</license>
// ***********************************************************************

namespace System.Windows.Internal
{
    /// <summary>Provide Error Message Helper Methods.This is intended for Library Internal use only.</summary>
    public static class ErrorHelper
    {
        #region Constants and Fields

        /// <summary>This is intended for Library Internal use only.</summary>
        public const int Ignored = (int)Result.Ok;

        #endregion

        #region Public Methods

        /// <summary>This is intended for Library Internal use only.</summary>
        /// <param name="result">  The error code.</param>
        /// <returns>True if the error code indicates failure.</returns>
        public static bool Failed(Result result)
        {
            return (int)result < 0;
        }

        /// <summary>This is intended for Library Internal use only.</summary>
        /// <param name="result">  The error code.</param>
        /// <returns>True if the error code indicates success.</returns>
        public static bool Succeeded(int result)
        {
            return result >= 0;
        }

        #endregion
    }
}