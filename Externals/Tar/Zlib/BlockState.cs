//-----------------------------------------------------------------------
// <copyright file="BlockState.cs" project="Zlib" assembly="Zlib" solution="Zlib" company="Dino Chiesa">
//     Copyright (c) Dino Chiesa. All rights reserved.
// </copyright>
// <author username="Cheeso">Dino Chiesa</author>
// <summary></summary>
//-----------------------------------------------------------------------

namespace Zlib
{
    /// <summary>
    /// </summary>
    internal enum BlockState
    {
        NeedMore = 0, // block not completed, need more input or more output

        BlockDone, // block flush performed

        FinishStarted, // finish started, need only more output at next deflate

        FinishDone // finish done, accept no more input or output
    }
}