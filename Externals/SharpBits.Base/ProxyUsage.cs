// ***********************************************************************
// <copyright file="ProxyUsage.cs" project="SharpBits.Base" assembly="SharpBits.Base" solution="SevenUpdate" company="Xidar Solutions">
//     Copyright (c) xidar solutions. All rights reserved.
// </copyright>
// <author username="xidar">xidar</author>
// <author username="sevenalive">Robert Baker</author>
// <license href="http://sharpbits.codeplex.com/license">BSD License</license> 
// ***********************************************************************

namespace SharpBits.Base
{
    /// <summary>
    ///   The proxy use.
    /// </summary>
    public enum ProxyUsage
    {
        /// <summary>
        ///   Use the current configuration.
        /// </summary>
        PreConfig,

        /// <summary>
        ///   Don't use a proxy.
        /// </summary>
        NoProxy,

        /// <summary>
        ///   Override proxy settings.
        /// </summary>
        Override,

        /// <summary>
        ///   Auto detect proxy settings.
        /// </summary>
        AutoDetect,
    }
}