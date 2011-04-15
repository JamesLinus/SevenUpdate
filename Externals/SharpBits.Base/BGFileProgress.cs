// ***********************************************************************
// <copyright file="BGFileProgress.cs"
//            project="SharpBits.Base"
//            assembly="SharpBits.Base"
//            solution="SevenUpdate"
//            company="Xidar Solutions">
//     Copyright (c) xidar solutions. All rights reserved.
// </copyright>
// <author username="xidar">xidar</author>
// <author username="sevenalive">Robert Baker</author>
// <license href="http://sharpbits.codeplex.com/license">BSD License</license> 
// ***********************************************************************
namespace SharpBits.Base
{
    using System.Runtime.InteropServices;

    /// <summary>The BG_FILE_PROGRESS structure provides file-related progress information, such as the number of bytes transferred</summary>
    [StructLayout(LayoutKind.Sequential, Pack = 8, Size = 0)]
    internal struct BGFileProgress
    {
        /// <summary>Size of the file in bytes</summary>
        public ulong BytesTotal;

        /// <summary>Number of bytes transferred.</summary>
        public ulong BytesTransferred;

        /// <summary>For downloads, the value is <see langword = "true" /> if the file is available to the user; otherwise, the value is <see langword = "false" /></summary>
        public int Completed;
    }
}