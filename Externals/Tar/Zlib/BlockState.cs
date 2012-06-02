// <copyright file="BlockState.cs" project="Tar">Dino Chiesa</copyright>
// <license href="http://www.gnu.org/licenses/gpl-3.0.txt" name="GNU General Public License 3" />

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