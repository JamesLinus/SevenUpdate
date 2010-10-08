// ***********************************************************************
// <copyright file="SuaSorter.cs"
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
    using System.Globalization;
    using System.Windows.Controls;

    /// <summary>
    /// Sorts the SUA class
    /// </summary>
    internal sealed class SuaSorter : ListViewCustomComparer
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
                var first = (Sua)x;
                var second = (Sua)y;

                string valueX = String.Empty, valueY = String.Empty;
                var result = 0;

                foreach (var sort in this.GetSortColumnList())
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
                        case "Is64Bit":
                            valueX = first.Is64Bit.ToString(CultureInfo.CurrentCulture);
                            valueY = second.Is64Bit.ToString(CultureInfo.CurrentCulture);

                            break;
                    }

                    if (this.SortColumns[sort] == ListSortDirection.Ascending)
                    {
                        result = String.Compare(valueX, valueY, StringComparison.CurrentCulture);
                    }
                    else
                    {
                        result = (-1) * String.Compare(valueX, valueY, StringComparison.CurrentCulture);
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