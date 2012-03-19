// <copyright file="BGJobProxyUsage.cs" project="SharpBits.Base">Xidar</copyright>
// <license href="http://sharpbits.codeplex.com/license" name="BSD License" />

namespace SharpBits.Base
{
    /// <summary>
    ///   The BG_JOB_PROXY_USAGE enumeration type defines constant values that you use to specify which proxy to use for
    ///   file transfers.
    /// </summary>
    internal enum BGJobProxyUsage
    {
        /// <summary>Use the proxy and proxy bypass list settings defined by each user to transfer files.</summary>
        PreConfig = 0, 

        /// <summary>Do not use a proxy to transfer files.</summary>
        NoProxy = 1, 

        /// <summary>Use the application's proxy and proxy bypass list to transfer files.</summary>
        Override = 2, 

        /// <summary>Automatically detect proxy settings. BITS detects proxy settings for each file in the job.</summary>
        AutoDetect = 3, 
    }
}