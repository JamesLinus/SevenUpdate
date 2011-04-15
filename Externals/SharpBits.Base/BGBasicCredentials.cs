// ***********************************************************************
// <copyright file="BGBasicCredentials.cs"
//            project="SharpBits.Base"
//            assembly="SharpBits.Base"
//            solution="SevenUpdate"
//            company="Xidar Solutions">
//     Copyright (c) xidar solutions. All rights reserved.
// </copyright>
// <author username="xidar">xidar</author>
// <author username="sevenalive">Robert Baker</author>
// <license href="http://sharpbits.codeplex.com/license">BSD License</license> 
// ***********************************************************************
namespace SharpBits.Base
{
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.InteropServices;

    /// <summary>The BG_BASIC_CREDENTIALS structure identifies the user name and password to authenticate.</summary>
    [SuppressMessage("Microsoft.Portability", "CA1900:ValueTypeFieldsShouldBePortable", MessageId = "Password", Justification = "Interop"), StructLayout(LayoutKind.Sequential, Size = 8)]
    internal struct BGBasicCredentials
    {
        /// <summary>Null-terminated string that contains the user name to authenticate. The user name is limited to 300 characters, not including the <see langword = "null" /> terminator. The format of the user name depends on the authentication scheme requested. For example, for Basic, NTLM, and Negotiate authentication, the user name is of the form "domain\user name" or "user name". For Passport authentication, the user name is an e-mail address. If <see langword = "null" />, default credentials for this session context are used.</summary>
        [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields", Justification = "Interop"), MarshalAs(UnmanagedType.LPWStr)]
        internal string UserName;

        /// <summary>Null-terminated string that contains the password in clear-text. The password is limited to 300 characters, not including the <see langword = "null" /> terminator. The password can be blank. Set to <see langword = "null" /> if <see cref = "UserName" /> is <see langword = "null" />. BITS encrypts the password before persisting the job if a network disconnect occurs or the user logs off.</summary>
        [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields", Justification = "Interop"), MarshalAs(UnmanagedType.LPWStr)]
        internal string Password;
    }
}