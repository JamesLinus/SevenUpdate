// ***********************************************************************
// <copyright file="JobPriority.cs" project="SharpBits.Base" assembly="SharpBits.Base" solution="SevenUpdate" company="Xidar Solutions">
//     Copyright (c) xidar solutions. All rights reserved.
// </copyright>
// <author username="xidar">xidar</author>
// <author username="sevenalive">Robert Baker</author>
// <license href="http://sharpbits.codeplex.com/license">BSD License</license> 
// ***********************************************************************

namespace SharpBits.Base
{
    /// <summary>
    ///   The <see cref="BitsJob" /> priority.
    /// </summary>
    public enum JobPriority
    {
        /// <summary>
        ///   Downloads without bandwidth restriction.
        /// </summary>
        Foreground = 0,

        /// <summary>
        ///   Downloads with a 80% bandwidth use.
        /// </summary>
        High = 1,

        /// <summary>
        ///   Downloads using bandwidth available.
        /// </summary>
        Normal = 2,

        /// <summary>
        ///   Download slow, giving other net use priority.
        /// </summary>
        Low = 3,
    }
}