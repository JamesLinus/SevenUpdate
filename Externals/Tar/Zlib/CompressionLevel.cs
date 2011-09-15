//-----------------------------------------------------------------------
// <copyright file="CompressionLevel.cs" project="Zlib" assembly="Zlib" solution="Zlib" company="Dino Chiesa">
//     Copyright (c) Dino Chiesa. All rights reserved.
// </copyright>
// <author username="Cheeso">Dino Chiesa</author>
// <summary></summary>
//-----------------------------------------------------------------------

namespace Zlib
{
    /// <summary>The compression level to be used when using a DeflateStream or ZlibStream with CompressionMode.Compress.</summary>
    public enum CompressionLevel
    {
        /// <summary>The fastest but least effective compression.</summary>
        BestSpeed = 1,

        /// <summary>The default compression level, with a good balance of speed and compression efficiency.</summary>
        Default = 6,

        /// <summary>The "best" compression, where best means greatest reduction in size of the input data stream. This is also the slowest compression.</summary>
        BestCompression = 9,
    }
}
