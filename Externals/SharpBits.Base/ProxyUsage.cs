// <copyright file="ProxyUsage.cs" project="SharpBits.Base">Xidar</copyright>
// <license href="http://sharpbits.codeplex.com/license" name="BSD License" />

namespace SharpBits.Base
{
    /// <summary>The proxy use.</summary>
    public enum ProxyUsage
    {
        /// <summary>Use the current configuration.</summary>
        PreConfig, 

        /// <summary>Don't use a proxy.</summary>
        NoProxy, 

        /// <summary>Override proxy settings.</summary>
        Override, 

        /// <summary>Auto detect proxy settings.</summary>
        AutoDetect, 
    }
}