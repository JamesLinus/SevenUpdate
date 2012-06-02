// <copyright file="InternalInflateConstants.cs" project="Tar">Dino Chiesa</copyright>
// <license href="http://www.gnu.org/licenses/gpl-3.0.txt" name="GNU General Public License 3" />

namespace Zlib
{
    /// <summary>
    /// </summary>
    internal static class InternalInflateConstants
    {
        internal static readonly int[] InflateMask = new[]
            {
                0x00000000, 0x00000001, 0x00000003, 0x00000007, 0x0000000f, 0x0000001f, 0x0000003f, 0x0000007f, 0x000000ff
                , 0x000001ff, 0x000003ff, 0x000007ff, 0x00000fff, 0x00001fff, 0x00003fff, 0x00007fff, 0x0000ffff
            };

        // And'ing with mask[n] masks the lower n bits
    }
}