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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;

namespace SevenUpdate.Classes
{
   static class ListViewExtensions
    {
        private static GridViewColumn lastColumnSorted;

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

        internal static void OnColumnHeaderClick(ListView listView, RoutedEventArgs e)
        {
            try
            {
                GridViewColumn column = ((GridViewColumnHeader)e.OriginalSource).Column;
                if (lastColumnSorted != null)
                {
                    lastColumnSorted.HeaderTemplate = null;
                }
                SortDescriptionCollection sorts = listView.Items.SortDescriptions;
                RenderSort(listView, sorts, column, GetSortDirection(sorts, column));
            }
            catch (InvalidCastException) { }
        }

        private static ListSortDirection GetSortDirection(SortDescriptionCollection sorts, GridViewColumn column)
        {
            if (column == null)
                return ListSortDirection.Ascending;
            if (column == lastColumnSorted && sorts[0].Direction == ListSortDirection.Ascending)
            {
                return ListSortDirection.Descending;
            }
            return ListSortDirection.Ascending;
        }

        private static void RenderSort(ListView listView, SortDescriptionCollection sorts, GridViewColumn column, ListSortDirection direction)
        {
            if (column == null)
                return;
            column.HeaderTemplate = (DataTemplate)listView.FindResource("HeaderTemplate" + direction);

            Binding columnBinding = column.DisplayMemberBinding as Binding;
            if (columnBinding != null)
            {
                sorts.Clear();
                sorts.Add(new SortDescription(columnBinding.Path.Path, direction));
                lastColumnSorted = column;
            }
        }

    }
}
