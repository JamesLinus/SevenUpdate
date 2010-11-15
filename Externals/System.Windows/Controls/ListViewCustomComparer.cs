// ***********************************************************************
// <copyright file="ListViewCustomComparer.cs"
//            project="System.Windows"
//            assembly="System.Windows"
//            solution="SevenUpdate"
//            company="Seven Software">
//     Copyright (c) Seven Software. All rights reserved.
// </copyright>
// <author username="sevenalive">Robert Baker</author>
// <license href="http://www.gnu.org/licenses/gpl-3.0.txt" name="GNU General Public License 3">
//  This file is part of Seven Update.
//    Seven Update is free software: you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation, either version 3 of the License, or
//    (at your option) any later version.
//    Seven Update is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.
//    You should have received a copy of the GNU General Public License
//    along with Seven Update.  If not, see http://www.gnu.org/licenses/.
// </license>
// ***********************************************************************
namespace System.Windows.Controls
{
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;

    /// <summary>Enables the listView sorter to compare classes</summary>
    public abstract class ListViewCustomComparer : IComparer
    {
        #region Properties

        /// <summary>Gets a dictionary of SortColumns</summary>
        protected Dictionary<string, ListSortDirection> SortColumns { get; private set; }

        /// <summary>Gets a List of strings from a column that needs to be sorted</summary>
        /// <returns>A collection of columns that are sorted</returns>
        protected IEnumerable<string> SortColumnList
        {
            get
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
        }

        #endregion

        #region Public Methods

        /// <summary>Adds a column to the <see cref="SortColumns"/></summary>
        /// <param name="sortColumn">a string representing a column to be sorted</param>
        /// <param name="direction">the direction to sort</param>
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

        /// <summary>Compares two objects and returns a value indicating whether one is less than, equal to, or greater than the other.</summary>
        /// <param name="x">The first object to compare.</param>
        /// <param name="y">The second object to compare.</param>
        /// <returns>A signed integer that indicates the relative values of <paramref name="x"/> and <paramref name="y"/>, as shown in the following table.Value Meaning Less than zero <paramref name="x"/> is less than <paramref name="y"/>. Zero <paramref name="x"/> equals <paramref name="y"/>. Greater than zero <paramref name="x"/> is greater than <paramref name="y"/>.</returns>
        /// <exception cref="T:System.ArgumentException">Neither <paramref name="x"/> nor <paramref name="y"/> implements the <see cref="T:System.IComparable"/> interface.-or- <paramref name="x"/> and <paramref name="y"/> are of different types and neither one can handle comparisons with the other. </exception>
        public abstract int Compare(object x, object y);

        #endregion

        #endregion

        #region Methods

        /// <summary>Clears the sort columns</summary>
        private void ClearSort()
        {
            this.SortColumns.Clear();
        }

        #endregion
    }
}