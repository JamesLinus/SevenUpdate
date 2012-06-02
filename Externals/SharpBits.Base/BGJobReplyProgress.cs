// <copyright file="BGJobReplyProgress.cs" project="SharpBits.Base">Xidar</copyright>
// <license href="http://sharpbits.codeplex.com/license" name="BSD License" />

namespace SharpBits.Base
{
    using System.Runtime.InteropServices;

    /// <summary>
    ///   The BG_JOB_REPLY_PROGRESS structure provides progress information related to the reply portion of an
    ///   upload-reply job.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    internal struct BGJobReplyProgress
    {
        /// <summary>Size of the file in bytes. The value is BG_SIZE_UNKNOWN if the reply has not begun.</summary>
        public readonly ulong BytesTotal;

        /// <summary>Number of bytes transferred.</summary>
        public readonly ulong BytesTransferred;
    }
}