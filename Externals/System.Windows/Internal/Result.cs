// ***********************************************************************
// <copyright file="Result.cs" project="System.Windows" assembly="System.Windows" solution="SevenUpdate" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// <license href="http://code.msdn.microsoft.com/WindowsAPICodePack/Project/License.aspx">Microsoft Software License</license>
// ***********************************************************************

namespace System.Windows.Internal
{
    /// <summary>HRESULT Wrapper This is intended for Library Internal use only.</summary>
    public enum Result : uint
    {
        /// <summary>Result returns <c>False</c>.</summary>
        False = 0x0001,

        /// <summary>Returns OK.</summary>
        Ok = 0x0000,

        /// <summary>Returns invalid argument.</summary>
        InvalidArg = 0x80070057,

        /// <summary>Returns out of memory.</summary>
        OutOfMemory = 0x8007000E,

        /// <summary>Returns operation failed.</summary>
        Fail = 0x80004005,
    }
}