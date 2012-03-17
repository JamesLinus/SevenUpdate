// ***********************************************************************
// <copyright file="NativeMethods.cs" project="SharpBits.Base" assembly="SharpBits.Base" solution="SevenUpdate" company="Xidar Solutions">
//     Copyright (c) xidar solutions. All rights reserved.
// </copyright>
// <author username="xidar">xidar</author>
// <author username="sevenalive">Robert Baker</author>
// <license href="http://sharpbits.codeplex.com/license">BSD License</license> 
// ***********************************************************************

namespace SharpBits.Base
{
    using System;
    using System.Runtime.InteropServices;
    using System.Text;

    /// <summary>Win32 native methods.</summary>
    internal static class NativeMethods
    {
        /// <summary>
        ///   Converts a string-format security identifier (SID) into a valid, functional SID. You can use this function
        ///   to retrieve a SID that the ConvertSidToStringSid function converted to string format.
        /// </summary>
        /// <param name="sid">A pointer to a <c>null</c>-terminated string containing the string-format SID to convert. The SID string can use either the standard S-R-I-S-S� format for SID strings, or the SID string constant format, such as "BA" for built-in administrators.</param>
        /// <param name="sidPointer">A pointer to a variable that receives a pointer to the converted SID. To free the returned buffer, call the LocalFree function.</param>
        /// <returns><c>True</c> if function succeeded.</returns>
        [DllImport(@"advapi32.dll", CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool ConvertStringSidToSidW(string sid, ref IntPtr sidPointer);

        /// <summary>
        ///   Retrieves the name of the account for this SID and the name of the first domain on which this SID is
        ///   found.
        /// </summary>
        /// <param name="systemName">A pointer to a <c>null</c>-terminated character string that specifies the target computer. This string can be the name of a remote computer. If this parameter is <c>null</c>, the account name translation begins on the local system. If the name cannot be resolved on the local system, this function will try to resolve the name using domain controllers trusted by the local system. Generally, specify a value only when the account is in an untrusted domain and the name of a computer in that domain is known.</param>
        /// <param name="sid">A pointer to the SID to look up.</param>
        /// <param name="name">A pointer to a buffer that receives a <c>null</c>-terminated string that contains the account name that corresponds to the sid parameter.</param>
        /// <param name="nameSize">On input, specifies the size, of the name buffer. If the function fails because the buffer is too small or if name is zero, name receives the required buffer size, including the terminating <c>null</c> character.</param>
        /// <param name="referencedDomainName">A pointer to a buffer that receives a <c>null</c>-terminated string that contains the name of the domain where the account name was found.</param>
        /// <param name="domainNameSize">On input, specifies the size, of the <paramref name="referencedDomainName" />
        /// buffer. If the function fails because the buffer is too small or if <paramref name="referencedDomainName" />
        /// is zero, <paramref name="referencedDomainName" /> receives the required buffer size, including the
        /// terminating <c>null</c> character.</param>
        /// <param name="use">A pointer to a variable that receives a <c>SidNameUse</c> value that indicates the type of the account.</param>
        /// <returns><c>True</c> if function succeeded.</returns>
        [DllImport(@"advapi32.dll", CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool LookupAccountSidW(
                string systemName, 
                IntPtr sid, 
                StringBuilder name, 
                ref long nameSize, 
                StringBuilder referencedDomainName, 
                ref long domainNameSize, 
                ref int use);
    }
}