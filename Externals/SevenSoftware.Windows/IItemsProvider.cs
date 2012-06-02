// <copyright file="IItemsProvider.cs" project="SevenSoftware.Windows">Paul McClean</copyright>
// <license href="http://www.gnu.org/licenses/gpl-3.0.txt" name="GNU General Public License 3" />

namespace SevenSoftware.Windows
{
    using System.Collections.Generic;

    /// <summary>
    /// Represents a provider of collection details.
    /// </summary>
    /// <typeparam name="T">The type of items in the collection.</typeparam>
    public interface IItemsProvider<T>
    {
        /// <summary>
        /// Gets the total number of items available.
        /// </summary>
        /// <returns>The total number of items to fetch.</returns>
        int Count { get; }

        /// <summary>
        /// Fetches a range of items.
        /// </summary>
        /// <param name="startIndex">The start index.</param>
        /// <param name="count">The number of items to fetch.</param>
        /// <returns>An IList containing the items in the specified range.</returns>
        IList<T> FetchRange(int startIndex, int count);
    }
}