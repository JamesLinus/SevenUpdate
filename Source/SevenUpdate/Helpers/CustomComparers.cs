// ***********************************************************************
// Assembly         : SevenUpdate
// Author           : sevenalive
// Created          : 09-17-2010
//
// Last Modified By : sevenalive
// Last Modified On : 10-05-2010
// Description      : 
//
// Copyright        : (c) Seven Software. All rights reserved.
// ***********************************************************************
namespace SevenUpdate
{
    using System;
    using System.ComponentModel;
    using System.Globalization;

    using Microsoft.Windows.Controls;

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
                var xc = (Sua)x;
                var yc = (Sua)y;

                string valx = String.Empty, valy = String.Empty;
                var result = 0;

                foreach (var sort in this.GetSortColumnList())
                {
                    switch (sort)
                    {
                        case "Name":

                            valx = Base.GetLocaleString(xc.Name);
                            valy = Base.GetLocaleString(yc.Name);
                            break;

                        case "Publisher":
                            valx = Base.GetLocaleString(xc.Publisher);
                            valy = Base.GetLocaleString(yc.Publisher);
                            break;
                        case "Is64Bit":
                            valx = xc.Is64Bit.ToString(CultureInfo.CurrentCulture);
                            valy = yc.Is64Bit.ToString(CultureInfo.CurrentCulture);

                            break;
                    }

                    if (this.SortColumns[sort] == ListSortDirection.Ascending)
                    {
                        result = String.Compare(valx, valy, StringComparison.CurrentCulture);
                    }
                    else
                    {
                        result = (-1) * String.Compare(valx, valy, StringComparison.CurrentCulture);
                    }

                    if (result != 0)
                    {
                        break;
                    }

                    continue;
                }

                return result;
            }
            catch
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
                var xc = (Suh)x;
                var yc = (Suh)y;

                string valx = String.Empty, valy = String.Empty;
                var result = 0;

                foreach (var sortColumn in this.GetSortColumnList())
                {
                    switch (sortColumn)
                    {
                        case "Name":
                            valx = Base.GetLocaleString(xc.Name);
                            valy = Base.GetLocaleString(yc.Name);
                            break;
                        case "DateInstalled":
                            valx = xc.InstallDate;
                            valy = yc.InstallDate;
                            break;
                    }

                    switch (sortColumn)
                    {
                        case "Importance":
                            result = ImportanceSorter.CompareImportance(xc.Importance, yc.Importance);
                            break;
                        case "Status":
                            if (xc.Status == yc.Status)
                            {
                                result = 0;
                            }
                            else if (xc.Status == UpdateStatus.Successful)
                            {
                                result = 1;
                            }
                            else
                            {
                                result = -1;
                            }

                            break;
                        case "Size":
                            if (xc.UpdateSize > yc.UpdateSize)
                            {
                                result = 1;
                            }
                            else if (xc.UpdateSize == yc.UpdateSize)
                            {
                                result = 0;
                            }
                            else
                            {
                                result = -1;
                            }

                            break;
                        default:
                            result = String.Compare(valx, valy, StringComparison.CurrentCulture);
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
            catch
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
                var xc = (Update)x;
                var yc = (Update)y;

                string valx = String.Empty, valy = String.Empty;
                var result = 0;

                foreach (var sortColumn in this.GetSortColumnList())
                {
                    switch (sortColumn)
                    {
                        case "Name":

                            valx = Base.GetLocaleString(xc.Name);
                            valy = Base.GetLocaleString(yc.Name);
                            break;
                    }

                    switch (sortColumn)
                    {
                        case "Importance":
                            result = ImportanceSorter.CompareImportance(xc.Importance, yc.Importance);
                            break;
                        case "Size":
                            if (xc.Size > yc.Size)
                            {
                                result = 1;
                            }
                            else if (xc.Size == yc.Size)
                            {
                                result = 0;
                            }
                            else
                            {
                                result = -1;
                            }

                            break;
                        default:
                            result = String.Compare(valx, valy, StringComparison.CurrentCulture);
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
            catch
            {
                return 0;
            }
        }

        #endregion
    }
}