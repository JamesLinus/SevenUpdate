// <copyright file="BGAuthCredentialsUnion.cs" project="SharpBits.Base">Xidar</copyright>
// <license href="http://sharpbits.codeplex.com/license" name="BSD License" />

namespace SharpBits.Base
{
    using System.Runtime.InteropServices;

    /// <summary>
    ///   The BG_AUTH_CREDENTIALS_UNION union identifies the credentials to use for the authentication scheme specified
    ///   in the BG_AUTH_CREDENTIALS structure.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Size = 8)]
    internal struct BGAuthCredentialsUnion
    {
        /// <summary>
        ///   Identifies the user name and password of the user to authenticate. For details, see the
        ///   BG_BASIC_CREDENTIALS structure.
        /// </summary>
        public BGBasicCredentials Basic;
    }
}