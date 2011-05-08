// ***********************************************************************
// <copyright file="BGJobReplyProgress.cs" project="SharpBits.Base" assembly="SharpBits.Base" solution="SevenUpdate" company="Xidar Solutions">
//     Copyright (c) xidar solutions. All rights reserved.
// </copyright>
// <author username="xidar">xidar</author>
// <author username="sevenalive">Robert Baker</author>
// <license href="http://sharpbits.codeplex.com/license">BSD License</license> 
// ***********************************************************************

namespace SharpBits.Base
{
    using System.Runtime.InteropServices;

    /// <summary>The BG_JOB_REPLY_PROGRESS structure provides progress information related to the reply portion of an upload-reply job.</summary>
    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    internal struct BGJobReplyProgress
    {
        /// <summary>Size of the file in bytes. The value is BG_SIZE_UNKNOWN if the reply has not begun.</summary>
        public readonly ulong BytesTotal;

        /// <summary>Number of bytes transferred.</summary>
        public readonly ulong BytesTransferred;
    }
}