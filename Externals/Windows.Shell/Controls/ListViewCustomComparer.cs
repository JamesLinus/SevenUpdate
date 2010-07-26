#region

using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

#endregion

namespace Microsoft.Windows.Controls
{
    /// <summary>
    ///   Enables the listView sorter to compare classes
    /// </summary>
    public abstract class ListViewCustomComparer : IComparer
    {
        /// <summary>
        ///   Gets or Sets a List of SortColumns
        /// </summary>
        protected Dictionary<string, ListSortDirection> SortColumns = new Dictionary<string, ListSortDirection>();

        /// <summary>
        ///   Adds a column to the <see cref = "SortColumns" />
        /// </summary>
        /// <param name = "sortColumn">a string representing a column to be sorted</param>
        /// <param name = "direction">the direction to sort</param>
        public void AddSort(string sortColumn, ListSortDirection direction)
        {
            ClearSort();

            SortColumns.Add(sortColumn, direction);
        }

        /// <summary>
        ///   Clears the sort columns
        /// </summary>
        private void ClearSort()
        {
            SortColumns.Clear();
        }

        /// <summary>
        ///   Gets a List of strings from a column that needs to be sorted
        /// </summary>
        /// <returns></returns>
        protected IEnumerable<string> GetSortColumnList()
        {
            var result = new List<string>();
            var temp = new Stack<string>();

            foreach (var col in SortColumns.Keys)
                temp.Push(col);

            while (temp.Count > 0)
                result.Add(temp.Pop());

            return result;
        }

        #region Implementation of IComparer

        /// <summary>
        ///   Compares two objects and returns a value indicating whether one is less than, equal to, or greater than the other.
        /// </summary>
        /// <returns>Value  Condition  Less than zero
        ///   <paramref name = "x" />
        ///   is less than
        ///   <paramref name = "y" />
        ///   . Zero
        ///   <paramref name = "x" />
        ///   equals
        ///   <paramref name = "y" />
        ///   .
        ///   Greater than zero
        ///   <paramref name = "x" />
        ///   is greater than
        ///   <paramref name = "y" />
        ///   .</returns>
        /// <param name = "x">The first object to compare.</param>
        /// <param name = "y">The second object to compare.</param>
        public abstract int Compare(object x, object y);

        #endregion
    }
}