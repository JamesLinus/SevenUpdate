// <copyright file="AuthenticationTarget.cs" project="SharpBits.Base">Xidar</copyright>
// <license href="http://sharpbits.codeplex.com/license" name="BSD License" />

namespace SharpBits.Base
{
    /// <summary>
    ///   The AuthenticationTarget enumeration defines the constant values that specify whether the credentials are used
    ///   for proxy or server user authentication requests.
    /// </summary>
    public enum AuthenticationTarget
    {
        /// <summary>Use no credentials.</summary>
        None = 0, 

        /// <summary>Use credentials for server requests.</summary>
        Server = 1, 

        /// <summary>Use credentials for proxy requests.</summary>
        Proxy = 2, 
    }
}