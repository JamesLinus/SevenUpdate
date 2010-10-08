// ***********************************************************************
// <copyright file="UpdateSorter.cs"
//            project="SevenUpdate"
//            assembly="SevenUpdate"
//            solution="SevenUpdate"
//            company="Seven Software">
//     Copyright (c) Seven Software. All rights reserved.
// </copyright>
// <author username="sevenalive">Robert Baker</author>
// ***********************************************************************
namespace SevenUpdate.CustomComparer
{
    using System;
    using System.ComponentModel;
    using System.Windows.Controls;

    /// <summary>
    /// Sorts the Update Class
    /// </summary>
    internal sealed class UpdateSorter : ListViewCustomComparer
    {
        #region Public Methods

        /// <summary>
        /// Compares two objects and returns a value indicating whether one is less than, equal to, or greater than the other.
        /// </summary>
        /// <param name="x">
        /// The first object to compare.
        /// </param>
        /// <param name="y">
        /// The second object to compare.
        /// </param>
        /// <returns>
        /// Value  Condition Less than zero <paramref name="x"/> is less than <paramref name="y"/>. Zero <paramref name="x"/> equals <paramref name="y"/>.
        ///   Greater than zero <paramref name="x"/> is greater than <paramref name="y"/>.
        /// </returns>
        public override int Compare(object x, object y)
        {
            try
            {
                var first = (Update)x;
                var second = (Update)y;

                string valueX = String.Empty, valueY = String.Empty;
                var result = 0;

                foreach (var sortColumn in this.GetSortColumnList())
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
                            result = String.Compare(valueX, valueY, StringComparison.CurrentCulture);
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
            catch (Exception)
            {
                return 0;
            }
        }

        #endregion
    }
}