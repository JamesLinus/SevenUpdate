// <copyright file="BGAuthScheme.cs" project="SharpBits.Base">Xidar</copyright>
// <license href="http://sharpbits.codeplex.com/license" name="BSD License" />

namespace SharpBits.Base
{
    /// <summary>Authentication scheme used for the background job.</summary>
    internal enum BGAuthScheme
    {
        /// <summary>Basic is a scheme in which the user name and password are sent in clear-text to the server or proxy.</summary>
        Basic = 1, 

        /// <summary>Digest is a challenge-response scheme that uses a server-specified data string for the challenge.</summary>
        Digest = 2, 

        /// <summary>
        ///   Windows NT LAN Manager (NTLM) is a challenge-response scheme that uses the credentials of the user for
        ///   authentication in a Windows network environment.
        /// </summary>
        Ntlm = 3, 

        /// <summary>
        ///   Simple and Protected Negotiation protocol (SNEGO) is a challenge-response scheme that negotiates with the
        ///   server or proxy to determine which scheme to use for authentication. Examples are the Kerberos protocol,
        ///   Secure Socket Layer (SSL), and NTLM.
        /// </summary>
        Negotiate = 4, 

        /// <summary>
        ///   Passport is a centralized authentication service provided by Microsoft that offers a single logon for
        ///   member sites.
        /// </summary>
        Passport = 5, 
    }
}