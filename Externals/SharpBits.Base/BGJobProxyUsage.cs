// ***********************************************************************
// <copyright file="BGJobProxyUsage.cs" project="SharpBits.Base" assembly="SharpBits.Base" solution="SevenUpdate" company="Xidar Solutions">
//     Copyright (c) xidar solutions. All rights reserved.
// </copyright>
// <author username="xidar">xidar</author>
// <author username="sevenalive">Robert Baker</author>
// <license href="http://sharpbits.codeplex.com/license">BSD License</license> 
// ***********************************************************************

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
