// ***********************************************************************
// <copyright file="BGJobTimes.cs" project="SharpBits.Base" assembly="SharpBits.Base" solution="SevenUpdate" company="Xidar Solutions">
//     Copyright (c) xidar solutions. All rights reserved.
// </copyright>
// <author username="xidar">xidar</author>
// <author username="sevenalive">Robert Baker</author>
// <license href="http://sharpbits.codeplex.com/license">BSD License</license> 
// ***********************************************************************

namespace SharpBits.Base
{
    using System.Runtime.InteropServices;

    /// <summary>The BG_JOB_TIMES structure provides job-related timestamps.</summary>
    [StructLayout(LayoutKind.Sequential, Pack = 4, Size = 0)]
    internal struct BGJobTimes
    {
        /// <summary>Time the job was created.</summary>
        public FileTime CreationTime;

        /// <summary>Time the job was last modified or bytes were transferred.</summary>
        public FileTime ModificationTime;

        /// <summary>Time the job entered the BG_JOB_STATE_TRANSFERRED state.</summary>
        public FileTime TransferCompletionTime;
    }
}
