// <copyright file="SuhSorter.cs" project="SevenUpdate">Robert Baker</copyright>
// <license href="http://www.gnu.org/licenses/gpl-3.0.txt" name="GNU General Public License 3" />

namespace SevenUpdate.CustomComparer
{
    using System;
    using System.ComponentModel;

    using SevenSoftware.Windows.Controls;

    /// <summary>Sorts the SUH Class.</summary>
    internal sealed class SuhSorter : ListViewCustomComparer
    {
        /// <summary>
        ///   Compares two objects and returns a value indicating whether one is less than, equal to, or greater than
        ///   the other.
        /// </summary>
        /// <param name="x">The first object to compare.</param>
        /// <param name="y">The second object to compare.</param>
        /// <returns>Value Condition Less than zero <paramref name="x" /> is less than <paramref name="y" />. Zero
        /// <paramref name="x" /> equals <paramref name="y" />.Greater than zero <paramref name="x" /> is greater than
        /// <paramref name="y" />.</returns>
        public override int Compare(object x, object y)
        {
            var first = (Suh)x;
            var second = (Suh)y;

            string valueX = string.Empty, valueY = string.Empty;
            int result = 0;

            foreach (string sortColumn in this.SortColumnList)
            {
                switch (sortColumn)
                {
                    case "Name":
                        valueX = Utilities.GetLocaleString(first.Name);
                        valueY = Utilities.GetLocaleString(second.Name);
                        break;
                    case "DateInstalled":
                        valueX = first.InstallDate;
                        valueY = second.InstallDate;
                        break;
                }

                switch (sortColumn)
                {
                    case "Importance":
                        result = ImportanceSorter.CompareImportance(first.Importance, second.Importance);
                        break;
                    case "Status":
                        if (first.Status == second.Status)
                        {
                            result = 0;
                        }
                        else if (first.Status == UpdateStatus.Successful)
                        {
                            result = 1;
                        }
                        else
                        {
                            result = -1;
                        }

                        break;
                    case "Size":
                        if (first.UpdateSize > second.UpdateSize)
                        {
                            result = 1;
                        }
                        else if (first.UpdateSize == second.UpdateSize)
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
    }
}