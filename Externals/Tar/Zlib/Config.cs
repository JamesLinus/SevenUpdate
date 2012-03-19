// <copyright file="Config.cs" project="Tar">Dino Chiesa</copyright>
// <license href="http://www.gnu.org/licenses/gpl-3.0.txt" name="GNU General Public License 3" />

namespace Zlib
{
    /// <summary>
    /// </summary>
    internal class Config
    {
        internal readonly DeflateFlavor Flavor;

        internal readonly int GoodLength; // reduce lazy search above this match length

        internal readonly int MaxChainLength;

        // Attempt to find a better match only when the current match is strictly smaller than this value. This
        // mechanism is used only for
        // compression levels >= 4.  For levels 1,2,3: MaxLazy is actually
        // MaxInsertLength. (See DeflateFast)
        internal readonly int MaxLazy; // do not perform lazy search above this match length

        internal readonly int NiceLength; // quit search above this match length

        private static readonly Config[] Table = new[]
            {
                new Config(0, 0, 0, 0, DeflateFlavor.Store), new Config(4, 4, 8, 4, DeflateFlavor.Fast), 
                new Config(4, 5, 16, 8, DeflateFlavor.Fast), new Config(4, 6, 32, 32, DeflateFlavor.Fast), 
                new Config(4, 4, 16, 16, DeflateFlavor.Slow), new Config(8, 16, 32, 32, DeflateFlavor.Slow), 
                new Config(8, 16, 128, 128, DeflateFlavor.Slow), new Config(8, 32, 128, 256, DeflateFlavor.Slow), 
                new Config(32, 128, 258, 1024, DeflateFlavor.Slow), new Config(32, 258, 258, 4096, DeflateFlavor.Slow)
            };

        /// <param name="goodLength"></param><param name="maxLazy"></param> <param name="niceLength"></param><param name="maxChainLength"></param><param name="flavor"></param>
        private Config(int goodLength, int maxLazy, int niceLength, int maxChainLength, DeflateFlavor flavor)
        {
            this.GoodLength = goodLength;
            this.MaxLazy = maxLazy;
            this.NiceLength = niceLength;
            this.MaxChainLength = maxChainLength;
            this.Flavor = flavor;
        }

        /// <param name="level">
        /// </param><returns></returns>
        public static Config Lookup(CompressionLevel level)
        {
            return Table[(int)level];
        }

        // Use a faster search when the previous match is longer than this

        // To speed up deflation, hash chains are never searched beyond this
        // length.  A higher limit improves compression ratio but degrades the speed.
    }
}