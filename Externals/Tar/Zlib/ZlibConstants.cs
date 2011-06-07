//-----------------------------------------------------------------------
// <copyright file="ZlibConstants.cs" project="Tar" assembly="Tar" solution="SevenTools" company="Dino Chiesa">
//     Copyright (c) Dino Chiesa. All rights reserved.
// </copyright>
// <author username="sevenalive">Robert Baker</author>
// <summary></summary>
//-----------------------------------------------------------------------

namespace Zlib
{
    /// <summary>A bunch of constants used in the Zlib interface.</summary>
    public static class ZlibConstants
    {
        #region Constants and Fields

        /// <summary>The size of the working buffer used in the ZlibCodec class. Defaults to 8192 bytes.</summary>
        public const int WorkingBufferSizeDefault = 16384;

        /*
        /// <summary>The minimum size of the working buffer used in the ZlibCodec class.  Currently it is 128 bytes.</summary>
        public const int WorkingBufferSizeMin = 1024;
*/

        /// <summary>There was an error with the working buffer.</summary>
        public const int ZBufError = -5;

        /// <summary>There was an error with the data - not enough data, bad data, etc.</summary>
        public const int ZDataError = -3;

        /// <summary>The operation ended in need of a dictionary.</summary>
        public const int ZNeedDict = 2;

        /// <summary>Indicates that the last operation reached the end of the stream.</summary>
        public const int ZStreamEnd = 1;

        /// <summary>There was an error with the stream - not enough data, not open and readable, etc.</summary>
        public const int ZStreamError = -2;

        /// <summary>indicates everything is A-OK</summary>
        public const int Zok = 0;

        /// <summary>The default number of window bits for the Deflate algorithm.</summary>
        internal const int WindowBitsDefault = WindowBitsMax;

        /// <summary>The maximum number of window bits for the Deflate algorithm.</summary>
        internal const int WindowBitsMax = 15; // 32K LZ77 window

        #endregion
    }
}