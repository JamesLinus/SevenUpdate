// ***********************************************************************
// Assembly         : SevenUpdate
// Author           : Robert Baker (sevenalive)
// Last Modified By : Robert Baker (sevenalive)
// Last Modified On : 10-06-2010
// Copyright        : (c) Seven Software. All rights reserved.
// ***********************************************************************
namespace SevenUpdate
{
    using System;
    using System.ComponentModel;
    using System.Globalization;
    using System.Windows.Controls;

    /// <summary>
    /// </summary>
    internal static class ImportanceSorter
    {
        #region Methods

        /// <summary>
        /// Compares two <see cref="Importance"/> objects
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
        internal static int CompareImportance(Importance x, Importance y)
        {
            var xRank = 0;

            switch (x)
            {
                case Importance.Important:
                    xRank = 0;
                    break;
                case Importance.Recommended:
                    xRank = 1;
                    break;
                case Importance.Optional:
                    xRank = 2;
                    break;
                case Importance.Locale:
                    xRank = 3;
                    break;
            }

            var yRank = 0;
            switch (y)
            {
                case Importance.Important:
                    yRank = 0;
                    break;
                case Importance.Recommended:
                    yRank = 1;
                    break;
                case Importance.Optional:
                    yRank = 2;
                    break;
                case Importance.Locale:
                    yRank = 3;
                    break;
            }

            return xRank > yRank ? 1 : (xRank == yRank ? 0 : -1);
        }

        #endregion
    }

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

    /// <summary>
    /// Sorts the SUH Class
    /// </summary>
    internal sealed class SuhSorter : ListViewCustomComparer
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
                var first = (Suh)x;
                var second = (Suh)y;

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