// ***********************************************************************
// <copyright file="BGAuthTarget.cs" project="SharpBits.Base" assembly="SharpBits.Base" solution="SevenUpdate" company="Xidar Solutions">
//     Copyright (c) xidar solutions. All rights reserved.
// </copyright>
// <author username="xidar">xidar</author>
// <author username="sevenalive">Robert Baker</author>
// <license href="http://sharpbits.codeplex.com/license">BSD License</license> 
// ***********************************************************************

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
