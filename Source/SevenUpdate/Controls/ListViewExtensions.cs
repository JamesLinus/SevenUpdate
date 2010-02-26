#region GNU Public License Version 3

// Copyright 2007-2010 Robert Baker, Seven Software.
// This file is part of Seven Update.
//   
//      Seven Update is free software: you can redistribute it and/or modify
//      it under the terms of the GNU General Public License as published by
//      the Free Software Foundation, either version 3 of the License, or
//      (at your option) any later version.
//  
//      Seven Update is distributed in the hope that it will be useful,
//      but WITHOUT ANY WARRANTY; without even the implied warranty of
//      MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//      GNU General Public License for more details.
//   
//      You should have received a copy of the GNU General Public License
//      along with Seven Update.  If not, see <http://www.gnu.org/licenses/>.

#endregion

#region

using System;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using SevenUpdate.Base;

#endregion

namespace SevenUpdate.Controls
{
    /// <summary>
    /// Contains methods that extend the <see cref="ListView" /> control
    /// </summary>
    internal static class ListViewExtensions
    {
        /// <summary>
        /// Limits resizing of a <see cref="GridViewColumn" />
        /// </summary>
        /// <param name="e">The Thumb object of the <see cref="GridViewColumn" /> to limit it's size</param>
        internal static void LimitColumnSize(Thumb e)
        {
            var senderAsThumb = e;
            var header = senderAsThumb.TemplatedParent as GridViewColumnHeader;
            if (header == null)
                return;
            if (!string.IsNullOrEmpty(((string) header.Column.Header)))
            {
                if (header.Column.ActualWidth < 125)
                    header.Column.Width = 125;
            }
            else
                header.Column.Width = 25;
        }

        /// <summary>
        /// Compares two <see cref="Importance" /> objects
        /// </summary>
        /// <param name="x">First object</param>
        /// <param name="y">Second object</param>
        /// <returns>returns an integer that indicates their relationship to one another in the sort order.</returns>
        private static int CompareImportance(Importance x, Importance y)
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

        #region Nested type: SUASorter

        /// <summary>
        /// Sorts the SUA class
        /// </summary>
        internal sealed class SUASorter : ListViewCustomComparer
        {
            /// <summary>
            /// Compares two objects and returns a value indicating whether one is less than, equal to, or greater than the other.
            /// </summary>
            /// <returns>Value  Condition  Less than zero<paramref name="x" /> is less than <paramref name="y" />. Zero <paramref name="x" /> equals <paramref name="y" />.
            /// Greater than zero <paramref name="x" /> is greater than <paramref name="y" />.</returns>
            /// <param name="x">The first object to compare.</param>
            /// <param name="y">The second object to compare.</param>
            public override int Compare(object x, object y)
            {
                try
                {
                    var xc = (Sua) x;
                    var yc = (Sua) y;

                    string valx = String.Empty, valy = String.Empty;
                    var result = 0;

                    foreach (var sortColumn in GetSortColumnList())
                    {
                        switch (sortColumn)
                        {
                            case "ApplicationName":

                                valx = Base.Base.GetLocaleString(xc.Name);
                                valy = Base.Base.GetLocaleString(yc.Name);
                                break;

                            case "Publisher":
                                valx = Base.Base.GetLocaleString(xc.Publisher);
                                valy = Base.Base.GetLocaleString(yc.Publisher);
                                break;
                            case "Architecture":
                                valx = xc.Is64Bit.ToString();
                                valy = yc.Is64Bit.ToString();

                                break;
                        }

                        if (SortColumns[sortColumn] == ListSortDirection.Ascending)
                            result = String.Compare(valx, valy);
                        else
                            result = (-1)*String.Compare(valx, valy);

                        if (result != 0)
                            break;
                        continue;
                    }

                    return result;
                }
                catch
                {
                    return 0;
                }
            }
        }

        #endregion

        #region Nested type: SUHSorter

        /// <summary>
        /// Sorts the SUH Class
        /// </summary>
        internal sealed class SUHSorter : ListViewCustomComparer
        {
            /// <summary>
            /// Compares two objects and returns a value indicating whether one is less than, equal to, or greater than the other.
            /// </summary>
            /// <returns>Value  Condition  Less than zero<paramref name="x" /> is less than <paramref name="y" />. Zero <paramref name="x" /> equals <paramref name="y" />.
            /// Greater than zero <paramref name="x" /> is greater than <paramref name="y" />.</returns>
            /// <param name="x">The first object to compare.</param>
            /// <param name="y">The second object to compare.</param>
            public override int Compare(object x, object y)
            {
                try
                {
                    var xc = (Suh) x;
                    var yc = (Suh) y;

                    string valx = String.Empty, valy = String.Empty;
                    var result = 0;

                    foreach (var sortColumn in GetSortColumnList())
                    {
                        switch (sortColumn)
                        {
                            case "Name":
                                valx = Base.Base.GetLocaleString(xc.Name);
                                valy = Base.Base.GetLocaleString(yc.Name);
                                break;
                            case "DateInstalled":
                                valx = xc.InstallDate;
                                valy = yc.InstallDate;
                                break;
                        }

                        switch (sortColumn)
                        {
                            case "Importance":
                                result = CompareImportance(xc.Importance, yc.Importance);
                                break;
                            case "Status":
                                if (xc.Status == yc.Status)
                                    result = 0;
                                else if (xc.Status == UpdateStatus.Successful)
                                    result = 1;
                                else
                                    result = -1;
                                break;
                            case "Size":
                                if (xc.Size > yc.Size)
                                    result = 1;
                                else if (xc.Size == yc.Size)
                                    result = 0;
                                else
                                    result = -1;
                                break;
                            default:
                                result = String.Compare(valx, valy);
                                break;
                        }

                        if (SortColumns[sortColumn] == ListSortDirection.Descending)
                            result = (-1)*result;

                        if (result != 0)
                            break;
                        continue;
                    }

                    return result;
                }
                catch
                {
                    return 0;
                }
            }
        }

        #endregion

        #region Nested type: UpdateSorter

        /// <summary>
        /// Sorts the Update Class
        /// </summary>
        internal sealed class UpdateSorter : ListViewCustomComparer
        {
            /// <summary>
            /// Compares two objects and returns a value indicating whether one is less than, equal to, or greater than the other.
            /// </summary>
            /// <returns>Value  Condition  Less than zero<paramref name="x" /> is less than <paramref name="y" />. Zero <paramref name="x" /> equals <paramref name="y" />.
            /// Greater than zero <paramref name="x" /> is greater than <paramref name="y" />.</returns>
            /// <param name="x">The first object to compare.</param>
            /// <param name="y">The second object to compare.</param>
            public override int Compare(object x, object y)
            {
                try
                {
                    var xc = (Update) x;
                    var yc = (Update) y;

                    string valx = String.Empty, valy = String.Empty;
                    var result = 0;

                    foreach (var sortColumn in GetSortColumnList())
                    {
                        switch (sortColumn)
                        {
                            case "Name":

                                valx = Base.Base.GetLocaleString(xc.Name);
                                valy = Base.Base.GetLocaleString(yc.Name);
                                break;
                        }

                        switch (sortColumn)
                        {
                            case "Importance":
                                result = CompareImportance(xc.Importance, yc.Importance);
                                break;
                            case "Size":
                                if (xc.Size > yc.Size)
                                    result = 1;
                                else if (xc.Size == yc.Size)
                                    result = 0;
                                else
                                    result = -1;
                                break;
                            default:
                                result = String.Compare(valx, valy);
                                break;
                        }

                        if (SortColumns[sortColumn] == ListSortDirection.Descending)
                            result = (-1)*result;

                        if (result != 0)
                            break;
                        continue;
                    }

                    return result;
                }
                catch
                {
                    return 0;
                }
            }
        }

        #endregion
    }
}