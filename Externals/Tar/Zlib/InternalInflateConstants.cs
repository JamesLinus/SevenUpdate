//-----------------------------------------------------------------------
// <copyright file="InternalInflateConstants.cs" project="Zlib" assembly="Zlib" solution="Zlib" company="Dino Chiesa">
//     Copyright (c) Dino Chiesa. All rights reserved.
// </copyright>
// <author username="Cheeso">Dino Chiesa</author>
// <summary></summary>
//-----------------------------------------------------------------------

namespace Zlib
{
    /// <summary>
    /// </summary>
    internal static class InternalInflateConstants
    {
        #region Constants and Fields

        internal static readonly int[] InflateMask = new[]
            {
                0x00000000, 0x00000001, 0x00000003, 0x00000007, 0x0000000f, 0x0000001f, 0x0000003f, 0x0000007f, 0x000000ff,
                0x000001ff, 0x000003ff, 0x000007ff, 0x00000fff, 0x00001fff, 0x00003fff, 0x00007fff, 0x0000ffff
            };

        #endregion

        // And'ing with mask[n] masks the lower n bits
    }
}
