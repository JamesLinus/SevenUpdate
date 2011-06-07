// ***********************************************************************
// <copyright file="BGAuthCredentials.cs" project="SharpBits.Base" assembly="SharpBits.Base" solution="SevenUpdate" company="Xidar Solutions">
//     Copyright (c) xidar solutions. All rights reserved.
// </copyright>
// <author username="xidar">xidar</author>
// <author username="sevenalive">Robert Baker</author>
// <license href="http://sharpbits.codeplex.com/license">BSD License</license> 
// ***********************************************************************

namespace SharpBits.Base
{
    /// <summary>
    ///   The BG_AUTH_CREDENTIALS structure identifies the target (proxy or server), authentication scheme, and the
    ///   user's credentials to use for user authentication requests. The structure is passed to the
    ///   IBackgroundCopyJob2::SetCredentials method.
    /// </summary>
    internal struct BGAuthCredentials
    {
        #region Constants and Fields

        /// <summary>
        ///   Identifies the credentials to use for the specified authentication scheme. For details, see the BG_AUTH_CREDENTIALS_UNION union.
        /// </summary>
        public BGAuthCredentialsUnion Credentials;

        /// <summary>
        ///   Identifies the scheme to use for authentication (for example, Basic or NTLM). For a list of values, see the BG_AUTH_SCHEME enumeration.
        /// </summary>
        public BGAuthScheme Scheme;

        /// <summary>
        ///   Identifies whether to use the credentials for a proxy or server authentication request. For a list of values, see the BG_AUTH_TARGET enumeration.
        /// </summary>
        public BGAuthTarget Target;

        #endregion
    }
}