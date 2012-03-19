// <copyright file="BGBasicCredentials.cs" project="SharpBits.Base">Xidar</copyright>
// <license href="http://sharpbits.codeplex.com/license" name="BSD License" />

namespace SharpBits.Base
{
    using System.Runtime.InteropServices;

    /// <summary>The BG_BASIC_CREDENTIALS structure identifies the user name and password to authenticate.</summary>
    internal struct BGBasicCredentials
    {
        /// <summary>
        ///   Null-terminated string that contains the password in clear-text. The password is limited to 300
        ///   characters, not including the <c>null</c> terminator. The password can be blank. Set to <c>null</c> if
        ///   <c>UserName</c> is <c>null</c>. BITS encrypts the password before persisting the job if a network
        ///   disconnect occurs or the user logs off.
        /// </summary>
        [MarshalAs(UnmanagedType.LPWStr)]
        internal string Password;

        /// <summary>
        ///   Null-terminated string that contains the user name to authenticate. The user name is limited to 300
        ///   characters, not including the null terminator. The format of the user name depends on the authentication
        ///   scheme requested. For example, for Basic, NTLM, and Negotiate authentication, the user name is of the form
        ///   "domain\user name" or "user name". For Passport authentication, the user name is an e-mail address. If
        ///   null, default credentials for this session context are used.
        /// </summary>
        [MarshalAs(UnmanagedType.LPWStr)]
        internal string UserName;
    }
}