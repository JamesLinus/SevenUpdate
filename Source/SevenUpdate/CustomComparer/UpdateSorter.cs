// ***********************************************************************
// <copyright file="UpdateSorter.cs" project="SevenUpdate" assembly="SevenUpdate" solution="SevenUpdate" company="Seven Software">
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
// <summary>
//   Interaction logic for UpdateSorter.xaml
// .</summary>
// ***********************************************************************

namespace SevenUpdate.CustomComparer
{
    using System;
    using System.ComponentModel;
    using System.Windows.Controls;

    /// <summary>
    ///   Sorts the Update Class.
    /// </summary>
    internal sealed class UpdateSorter : ListViewCustomComparer
    {
        #region Public Methods

        /// <summary>
        ///   Compares two objects and returns a value indicating whether one is less than, equal to, or greater than the other.
        /// </summary>
        /// <param name="x">
        ///   The first object to compare.
        /// </param>
        /// <param name="y">
        ///   The second object to compare.
        /// </param>
        /// <returns>
        ///   Value Condition Less than zero <paramref name = "x" /> is less than <paramref name = "y" />. Zero <paramref name = "x" /> equals <paramref
        ///    name = "y" />.Greater than zero <paramref name = "x" /> is greater than <paramref name = "y" />.
        /// </returns>
        public override int Compare(object x, object y)
        {
            var first = (Update)x;
            var second = (Update)y;

            string valueX = string.Empty, valueY = string.Empty;
            var result = 0;

            foreach (var sortColumn in this.SortColumnList)
            {
                switch (sortColumn)
                {
                    case "Name":

                        valueX = Utilities.GetLocaleString(first.Name);
                        valueY = Utilities.GetLocaleString(second.Name);
                        break;
                }

                switch (sortColumn)
                {
                    case "Importance":
                        result = ImportanceSorter.CompareImportance(first.Importance, second.Importance);
                        break;
                    case "Size":
                        if (first.Size > second.Size)
                        {
                            result = 1;
                        }
                        else if (first.Size == second.Size)
                        {
                            result = 0;
                        }
                        else
                        {
                            result = -1;
                        }

                        break;
                    default:
                        result = string.Compare(valueX, valueY, StringComparison.CurrentCulture);
                        break;
                }

                if (this.SortColumns[sortColumn] == ListSortDirection.Descending)
                {
                    result = (-1) * result;
                }

                if (result != 0)
                {
                    break;
                }

                continue;
            }

            return result;
        }

        #endregion
    }
}