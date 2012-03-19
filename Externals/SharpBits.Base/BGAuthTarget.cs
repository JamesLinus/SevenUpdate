// <copyright file="BGAuthTarget.cs" project="SharpBits.Base">Xidar</copyright>
// <license href="http://sharpbits.codeplex.com/license" name="BSD License" />

namespace SharpBits.Base
{
    /// <summary>The location from which to download the code.</summary>
    internal enum BGAuthTarget
    {
        /// <summary>Use credentials for server requests.</summary>
        Server = 1, 

        /// <summary>Use credentials for proxy requests.</summary>
        Proxy = 2, 
    }
}