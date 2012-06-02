// <copyright file="BGJobTimes.cs" project="SharpBits.Base">Xidar</copyright>
// <license href="http://sharpbits.codeplex.com/license" name="BSD License" />

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