//-----------------------------------------------------------------------
// <copyright file="InternalConstants.cs" project="Zlib" assembly="Zlib" solution="Zlib" company="Dino Chiesa">
//     Copyright (c) Dino Chiesa. All rights reserved.
// </copyright>
// <author username="Cheeso">Dino Chiesa</author>
// <summary></summary>
//-----------------------------------------------------------------------

namespace Zlib
{
    /// <summary>
    /// </summary>
    internal static class InternalConstants
    {
        #region Constants and Fields

        internal const int BlCodes = 19;

        internal const int DCodes = 30;

        internal const int LCodes = Literals + 1 + LengthCodes;

        internal const int Literals = 256;

        // Bit length codes must not exceed MAX_BL_BITS bits

        internal const int MaxBits = 15;

        internal const int MaxBlBits = 7;

        internal const int Rep36 = 16;

        // repeat previous bit length 3-6 times (2 bits of repeat count)

        // repeat a zero length 11-138 times  (7 bits of repeat count)

        internal const int Repz11138 = 18;

        internal const int Repz310 = 17;

        private const int LengthCodes = 29;

        #endregion
    }
}
