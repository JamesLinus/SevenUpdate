// ***********************************************************************
// <copyright file="AuthenticationScheme.cs" project="SharpBits.Base" assembly="SharpBits.Base" solution="SevenUpdate" company="Xidar Solutions">
//     Copyright (c) xidar solutions. All rights reserved.
// </copyright>
// <author username="xidar">xidar</author>
// <author username="sevenalive">Robert Baker</author>
// <license href="http://sharpbits.codeplex.com/license">BSD License</license> 
// ***********************************************************************

namespace SharpBits.Base
{
    /// <summary>
    ///   The AuthenticationScheme enumeration defines the constant values that specify the authentication scheme to
    ///   usewhen a proxy or server requests user authentication.
    /// </summary>
    public enum AuthenticationScheme
    {
        /// <summary>Use no authentication scheme.</summary>
        None = 0, 

        /// <summary>Basic is a scheme in which the user name and password are sent in clear-text to the server or proxy.</summary>
        Basic = 1, 

        /// <summary>Digest is a challenge-response scheme that uses a server-specified data string for the challenge.</summary>
        Digest, 

        /// <summary>
        ///   Windows NT LAN Manager (NTLM) is a challenge-response scheme that uses the credentials of the user for
        ///   authentication in a Windows network environment.
        /// </summary>
        Ntlm, 

        /// <summary>
        ///   Simple and Protected Negotiation protocol (SNEGO) is a challenge-response scheme that negotiates with the
        ///   server or proxy to determine which scheme to use for authentication. Examples are the Kerberos protocol
        ///   and NTLM.
        /// </summary>
        Negotiate, 

        /// <summary>
        ///   Passport is a centralized authentication service provided by Microsoft that offers a single logon for
        ///   member sites.
        /// </summary>
        Passport
    }
}
