// <copyright file="BGFileRange.cs" project="SharpBits.Base">Xidar</copyright>
// <license href="http://sharpbits.codeplex.com/license" name="BSD License" />

namespace SharpBits.Base
{
    using System.Runtime.InteropServices;

    /// <summary>The BG_FILE_RANGE structure identifies a range of bytes to download from a file.</summary>
    [StructLayout(LayoutKind.Sequential, Pack = 8, Size = 0)]
    internal struct BGFileRange
    {
        /// <summary>The length to the end of the file.</summary>
        public const ulong BGLengthToEof = unchecked((ulong)-1);

        /// <summary>Zero-based offset to the beginning of the range of bytes to download from a file.</summary>
        public ulong InitialOffset;

        /// <summary>
        ///   Number of bytes in the range. To indicate that the range extends to the end of the file, specify
        ///   BG_LENGTH_TO_EOF.
        /// </summary>
        public ulong Length;
    }
}