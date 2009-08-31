/*Copyright 2007-09 Robert Baker, aka Seven ALive.
This file is part of Seven Update.

    Seven Update is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    Seven Update is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with Seven Update.  If not, see <http://www.gnu.org/licenses/>.*/
using System;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Avalon.Windows.Controls;
using System.Reflection;
using System.Windows.Data;
using System.Collections.ObjectModel;

namespace SevenUpdate
{
    static class ListViewExtensions
    {
        /// <summary>
        /// Sorts the SUA class
        /// </summary>
        internal class SUASorter : ListViewCustomComparer
        {
            public override int Compare(object x, object y)
            {
                try
                {
                    SUA xc = (SUA)x;
                    SUA yc = (SUA)y;

                    string valx = String.Empty, valy = String.Empty;
                    int result = 0;

                    foreach (string sortColumn in GetSortColumnList())
                    {
                        switch (sortColumn)
                        {
                            case "ApplicationName":

                                valx = Shared.GetLocaleString(xc.ApplicationName);
                                valy = Shared.GetLocaleString(yc.ApplicationName);
                                break;

                            case "Publisher":
                                valx = Shared.GetLocaleString(xc.Publisher);
                                valy = Shared.GetLocaleString(yc.Publisher);
                                break;
                            case "Architecture":
                                valx = xc.Is64Bit.ToString();
                                valy = yc.Is64Bit.ToString();

                                break;

                        }

                        if (sortColumns[sortColumn] == ListSortDirection.Ascending)
                            result = String.Compare(valx, valy);
                        else
                            result = (-1) * String.Compare(valx, valy);

                        if (result != 0)
                            break;
                        else
                            continue;

                    }

                    return result;

                }
                catch (Exception)
                {
                    return 0;
                }
            }
        }

        /// <summary>
        /// Sorts the Update Class
        /// </summary>
        internal class UpdateSorter : ListViewCustomComparer
        {
            public override int Compare(object x, object y)
            {
                try
                {
                    Update xc = (Update)x;
                    Update yc = (Update)y;

                    string valx = String.Empty, valy = String.Empty;
                    int result = 0;

                    foreach (string sortColumn in GetSortColumnList())
                    {
                        switch (sortColumn)
                        {
                            case "Name":

                                valx = Shared.GetLocaleString(xc.Name);
                                valy = Shared.GetLocaleString(yc.Name);
                                break;

                        }

                        if (sortColumn == "Importance")
                        {
                            result = ListViewExtensions.CompareImportance(xc.Importance, yc.Importance);

                        }
                        else
                            if (sortColumn == "Size")
                            {
                                if (xc.Size > yc.Size)
                                    result = 1;
                                else
                                    if (xc.Size == yc.Size)
                                        result = 0;
                                    else
                                        result = -1;

                            }
                            else
                                result = String.Compare(valx, valy);

                        if (sortColumns[sortColumn] == ListSortDirection.Descending)
                            result = (-1) * result;

                        if (result != 0)
                            break;
                        else
                            continue;

                    }

                    return result;

                }
                catch (Exception)
                {
                    return 0;
                }
            }
        }

        /// <summary>
        /// Sorts the UpdateInformation Class
        /// </summary>
        internal class UpdateInformationSorter : ListViewCustomComparer
        {
            public override int Compare(object x, object y)
            {
                try
                {
                    UpdateInformation xc = (UpdateInformation)x;
                    UpdateInformation yc = (UpdateInformation)y;

                    string valx = String.Empty, valy = String.Empty;
                    int result = 0;

                    foreach (string sortColumn in GetSortColumnList())
                    {
                        switch (sortColumn)
                        {
                            case "Name":
                                valx = Shared.GetLocaleString(xc.Name);
                                valy = Shared.GetLocaleString(yc.Name);
                                break;
                            case "DateInstalled":
                                valx = xc.InstallDate;
                                valy = yc.InstallDate;
                                break;
                        }

                        if (sortColumn == "Importance")
                        {
                            result = ListViewExtensions.CompareImportance(xc.Importance, yc.Importance);

                        }
                        else
                        {
                            if (sortColumn == "Status")
                            {
                                if (xc.Status == yc.Status)
                                    result = 0;
                                else
                                    if (xc.Status == UpdateStatus.Successful)
                                        result = 1;
                                    else
                                        result = -1;

                            }
                            else
                                if (sortColumn == "Size")
                                {
                                    if (xc.Size > yc.Size)
                                        result = 1;
                                    else
                                        if (xc.Size == yc.Size)
                                            result = 0;
                                        else
                                            result = -1;
                                }
                                else
                                    result = String.Compare(valx, valy);
                        }

                        if (sortColumns[sortColumn] == ListSortDirection.Descending)
                            result = (-1) * result;

                        if (result != 0)
                            break;
                        else
                            continue;

                    }

                    return result;

                }
                catch (Exception)
                {
                    return 0;
                }
            }
        }

        /// <summary>
        /// Limits resizing of columns
        /// </summary>
        internal static void Thumb_DragDelta(object sender, Thumb e)
        {
            Thumb senderAsThumb = e;
            GridViewColumnHeader header = senderAsThumb.TemplatedParent as GridViewColumnHeader;
            if (((string)header.Column.Header) != "" && ((string)header.Column.Header) != null)
            {
                if (header.Column.ActualWidth < 125)
                {
                    header.Column.Width = 125;
                }
            }
            else
                header.Column.Width = 25;

        }

        /// <summary>
        /// Custom compare method to compare update importance
        /// </summary>
        /// <param name="x">First object</param>
        /// <param name="y">Second object</param>
        /// <returns>Returns an integer that indicates their relationship to one another in the sort order.</returns>
        static int CompareImportance(Importance x, Importance y)
        {
            int xRank = 0;

            switch (x)
            {
                case Importance.Important: xRank = 0;
                    break;
                case Importance.Recommended: xRank = 1;
                    break;
                case Importance.Optional: xRank = 2;
                    break;
                case Importance.Locale: xRank = 3;
                    break;
            }

            int yRank = 0;
            switch (y)
            {
                case Importance.Important: yRank = 0;
                    break;
                case Importance.Recommended: yRank = 1;
                    break;
                case Importance.Optional: yRank = 2;
                    break;
                case Importance.Locale: yRank = 3;
                    break;
            }

            if (xRank > yRank)
                return 1;
            else
                if (xRank == yRank)
                    return 0;
                else
                    return -1;
        }

        internal static void OnCollectionChanged(System.Collections.Specialized.NotifyCollectionChangedAction action, object source)
        {
            // update the grid background view when item change is NOT caused by replacement
            try
            {
                if (action != System.Collections.Specialized.NotifyCollectionChangedAction.Replace)
                {
                    ICollectionView dataView = CollectionViewSource.GetDefaultView(source);
                    dataView.Refresh();
                }
            }
            catch (Exception)
            {
                //  MessageBox.Show(ex.Message + "\r\n\r\n" + ex.StackTrace);
            }

        }
    }

}
