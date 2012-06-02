// <copyright file="SuaSorter.cs" project="SevenUpdate">Robert Baker</copyright>
// <license href="http://www.gnu.org/licenses/gpl-3.0.txt" name="GNU General Public License 3" />

namespace SevenUpdate.CustomComparer
{
    using System;
    using System.ComponentModel;

    using SevenSoftware.Windows.Controls;

    /// <summary>Sorts the SUA class.</summary>
    internal sealed class SuaSorter : ListViewCustomComparer
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
            var first = (Sua)x;
            var second = (Sua)y;

            string valueX = string.Empty, valueY = string.Empty;
            int result = 0;

            foreach (string sort in this.SortColumnList)
            {
                switch (sort)
                {
                    case "Name":

                        valueX = Utilities.GetLocaleString(first.Name);
                        valueY = Utilities.GetLocaleString(second.Name);
                        break;

                    case "Publisher":
                        valueX = Utilities.GetLocaleString(first.Publisher);
                        valueY = Utilities.GetLocaleString(second.Publisher);
                        break;
                    case "Platform":
                        valueX = first.Platform.ToString();
                        valueY = second.Platform.ToString();

                        break;
                }

                if (this.SortColumns[sort] == ListSortDirection.Ascending)
                {
                    result = string.Compare(valueX, valueY, StringComparison.CurrentCulture);
                }
                else
                {
                    result = (-1) * string.Compare(valueX, valueY, StringComparison.CurrentCulture);
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