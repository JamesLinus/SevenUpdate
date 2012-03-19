// <copyright file="ListViewCustomComparer.cs" project="SevenSoftware.Windows">Robert Baker</copyright>
// <license href="http://www.gnu.org/licenses/gpl-3.0.txt" name="GNU General Public License" />

namespace SevenSoftware.Windows.Controls
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;

    /// <summary>Enables the listView sorter to compare classes.</summary>
    public abstract class ListViewCustomComparer : IComparer
    {
        /// <summary>Gets a List of strings from a column that needs to be sorted.</summary>
        /// <returns>A collection of columns that are sorted.</returns>
        protected IEnumerable<string> SortColumnList
        {
            get
            {
                var result = new List<string>();
                var temp = new Stack<string>();

                foreach (string col in this.SortColumns.Keys)
                {
                    temp.Push(col);
                }

                while (temp.Count > 0)
                {
                    result.Add(temp.Pop());
                }

                return result;
            }
        }

        /// <summary>Gets a dictionary of SortColumns.</summary>
        protected Dictionary<string, ListSortDirection> SortColumns { get; private set; }

        /// <summary>Adds a column to the <c>SortColumns</c>.</summary>
        /// <param name="sortColumn">A string representing a column to be sorted.</param>
        /// <param name="direction">The direction to sort.</param>
        public void AddSort(string sortColumn, ListSortDirection direction)
        {
            if (string.IsNullOrEmpty(sortColumn))
            {
                throw new ArgumentNullException("sortColumn");
            }

            this.ClearSort();
            if (this.SortColumns == null)
            {
                this.SortColumns = new Dictionary<string, ListSortDirection>();
            }

            this.SortColumns.Add(sortColumn, direction);
        }

        /// <summary>
        ///   Compares two objects and returns a value indicating whether one is less than, equal to, or greater than
        ///   the other.
        /// </summary>
        /// <param name="x">The first object to compare.</param>
        /// <param name="y">The second object to compare.</param>
        /// <returns>A signed integer that indicates the relative values of <paramref name="x" /> and <paramref name="y"
        /// />, as shown in the following table.Value Meaning Less than zero <paramref name="x" /> is less than
        /// <paramref name="y" />. Zero <paramref name="x" /> equals
        /// <paramref name="y" />. Greater than zero <paramref name="x" /> is greater than <paramref name="y" />.</returns>
        public abstract int Compare(object x, object y);

        /// <summary>Clears the sort columns.</summary>
        private void ClearSort()
        {
            if (this.SortColumns != null)
            {
                this.SortColumns.Clear();
            }
        }
    }
}