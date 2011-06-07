// ***********************************************************************
// <copyright file="BGJobPriority.cs" project="SharpBits.Base" assembly="SharpBits.Base" solution="SevenUpdate" company="Xidar Solutions">
//     Copyright (c) xidar solutions. All rights reserved.
// </copyright>
// <author username="xidar">xidar</author>
// <author username="sevenalive">Robert Baker</author>
// <license href="http://sharpbits.codeplex.com/license">BSD License</license> 
// ***********************************************************************

namespace SharpBits.Base
{
    /// <summary>
    ///   The BG_JOB_PRIORITY enumeration type defines the constant values that you use to specify the priority level of
    ///   the job.
    /// </summary>
    internal enum BGJobPriority
    {
        /// <summary>
        ///   Transfers the job in the foreground.
        /// </summary>
        Foreground = 0,

        /// <summary>
        ///   Transfers the job in the background. This is the highest background priority level.
        /// </summary>
        High = 1,

        /// <summary>
        ///   Transfers the job in the background. This is the default priority level for a job.
        /// </summary>
        Normal = 2,

        /// <summary>
        ///   Transfers the job in the background. This is the lowest background priority level.
        /// </summary>
        Low = 3,
    }
}