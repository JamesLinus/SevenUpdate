// ***********************************************************************
// Assembly         : System.Windows
// Author           : Robert Baker (sevenalive)
// Last Modified By : Robert Baker (sevenalive)
// Last Modified On : 10-06-2010
// Copyright        : (c) Seven Software. All rights reserved.
// ***********************************************************************
namespace System.Windows.Controls
{
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;

    /// <summary>
    /// Enables the listView sorter to compare classes
    /// </summary>
    public abstract class ListViewCustomComparer : IComparer
    {
        #region Properties

        /// <summary>
        ///   Gets or sets a dictionary of SortColumns
        /// </summary>
        protected Dictionary<string, ListSortDirection> SortColumns { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Adds a column to the <see cref="SortColumns"/>
        /// </summary>
        /// <parameter name="sortColumn">
        /// a string representing a column to be sorted
        /// </parameter>
        /// <parameter name="direction">
        /// the direction to sort
        /// </parameter>
        public void AddSort(string sortColumn, ListSortDirection direction)
        {
            this.ClearSort();
            if (this.SortColumns == null)
            {
                this.SortColumns = new Dictionary<string, ListSortDirection>();
            }

            this.SortColumns.Add(sortColumn, direction);
        }

        #endregion

        #region Implemented Interfaces

        #region IComparer

        /// <summary>
        /// Compares two objects and returns a value indicating whether one is less than, equal to, or greater than the other.
        /// </summary>
        /// <parameter name="x">
        /// The first object to compare.
        /// </parameter>
        /// <parameter name="y">
        /// The second object to compare.
        /// </parameter>
        /// <returns>
        /// A signed integer that indicates the relative values of <parameterref name="x"/> and <parameterref name="y"/>, as shown in the following table.Value Meaning Less than zero <parameterref name="x"/> is less than <parameterref name="y"/>. Zero <parameterref name="x"/> equals <parameterref name="y"/>. Greater than zero <parameterref name="x"/> is greater than <parameterref name="y"/>.
        /// </returns>
        /// <exception cref="T:System.ArgumentException">
        /// Neither <parameterref name="x"/> nor <parameterref name="y"/> implements the <see cref="T:System.IComparable"/> interface.-or- <parameterref name="x"/> and <parameterref name="y"/> are of different types and neither one can handle comparisons with the other. 
        /// </exception>
        public abstract int Compare(object x, object y);

        #endregion

        #endregion

        #region Methods

        /// <summary>
        /// Gets a List of strings from a column that needs to be sorted
        /// </summary>
        /// <returns>
        /// A collection of columns that are sorted
        /// </returns>
        protected IEnumerable<string> GetSortColumnList()
        {
            var result = new List<string>();
            var temp = new Stack<string>();

            foreach (var col in this.SortColumns.Keys)
            {
                temp.Push(col);
            }

            while (temp.Count > 0)
            {
                result.Add(temp.Pop());
            }

            return result;
        }

        /// <summary>
        /// Clears the sort columns
        /// </summary>
        private void ClearSort()
        {
            this.SortColumns.Clear();
        }

        #endregion
    }
}