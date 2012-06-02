// <copyright file="BGFileProgress.cs" project="SharpBits.Base">Xidar</copyright>
// <license href="http://sharpbits.codeplex.com/license" name="BSD License" />

namespace SharpBits.Base
{
    using System.Runtime.InteropServices;

    /// <summary>
    ///   The BG_FILE_PROGRESS structure provides file-related progress information, such as the number of bytes
    ///   transferred.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 8, Size = 0)]
    internal struct BGFileProgress
    {
        /// <summary>Size of the file in bytes.</summary>
        public readonly ulong BytesTotal;

        /// <summary>Number of bytes transferred.</summary>
        public readonly ulong BytesTransferred;

        /// <summary>
        ///   For downloads, the value is <c>True</c> if the file is available to the user; otherwise, the value is
        ///   <c>False</c>.
        /// </summary>
        public readonly int Completed;
    }
}