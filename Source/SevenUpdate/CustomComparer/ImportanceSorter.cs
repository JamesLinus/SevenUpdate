// <copyright file="ImportanceSorter.cs" project="SevenUpdate">Robert Baker</copyright>
// <license href="http://www.gnu.org/licenses/gpl-3.0.txt" name="GNU General Public License 3" />

namespace SevenUpdate.CustomComparer
{
    /// <summary>Sorts the update importance class.</summary>
    internal static class ImportanceSorter
    {
        /// <summary>Compares two <c>Importance</c> objects.</summary>
        /// <param name="x">The first object to compare.</param>
        /// <param name="y">The second object to compare.</param>
        /// <returns>Value Condition Less than zero <paramref name="x" /> is less than <paramref name="y" />. Zero
        /// <paramref name="x" /> equals <paramref name="y" />.  Greater than zero <paramref name="x" /> is greater
        /// than <paramref name="y" />.</returns>
        internal static int CompareImportance(Importance x, Importance y)
        {
            int firstRank = 0;

            switch (x)
            {
                case Importance.Important:
                    firstRank = 0;
                    break;
                case Importance.Recommended:
                    firstRank = 1;
                    break;
                case Importance.Optional:
                    firstRank = 2;
                    break;
                case Importance.Locale:
                    firstRank = 3;
                    break;
            }

            int secondRank = 0;
            switch (y)
            {
                case Importance.Important:
                    secondRank = 0;
                    break;
                case Importance.Recommended:
                    secondRank = 1;
                    break;
                case Importance.Optional:
                    secondRank = 2;
                    break;
                case Importance.Locale:
                    secondRank = 3;
                    break;
            }

            return firstRank > secondRank ? 1 : (firstRank == secondRank ? 0 : -1);
        }
    }
}