#region GNU Public License v3

// Copyright 2007, 2008 Robert Baker, aka Seven ALive.
// This file is part of Seven Update.
// 
//     Seven Update is free software: you can redistribute it and/or modify
//     it under the terms of the GNU General Public License as published by
//     the Free Software Foundation, either version 3 of the License, or
//     (at your option) any later version.
// 
//     Seven Update is distributed in the hope that it will be useful,
//     but WITHOUT ANY WARRANTY; without even the implied warranty of
//     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//     GNU General Public License for more details.
// 
//    You should have received a copy of the GNU General Public License
//     along with Seven Update.  If not, see <http://www.gnu.org/licenses/>.

#endregion

#region

using System;
using System.Collections;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;

#endregion

namespace SevenUpdate.Controls
{
    public static class ListViewSorter
    {
        public static readonly DependencyProperty IsListviewSortableProperty = DependencyProperty.RegisterAttached("IsListviewSortable", typeof (Boolean), typeof (ListViewSorter),
                                                                                                                   new FrameworkPropertyMetadata(false,
                                                                                                                                                 new PropertyChangedCallback(OnRegisterSortableGrid)));

        private static ListSortDirection lastDirection = ListSortDirection.Ascending;
        private static GridViewColumnHeader lastHeaderClicked;

        public static DependencyProperty CustomSorterProperty = DependencyProperty.RegisterAttached("CustomSorter", typeof (IComparer), typeof (ListViewSorter));
        private static ListView lv;
        public static DependencyProperty SortBindingMemberProperty = DependencyProperty.RegisterAttached("SortBindingMember", typeof (BindingBase), typeof (ListViewSorter));

        public static IComparer GetCustomSorter(DependencyObject obj)
        {
            return (IComparer) obj.GetValue(CustomSorterProperty);
        }

        public static void SetCustomSorter(DependencyObject obj, IComparer value)
        {
            obj.SetValue(CustomSorterProperty, value);
        }

        public static BindingBase GetSortBindingMember(DependencyObject obj)
        {
            return (BindingBase) obj.GetValue(SortBindingMemberProperty);
        }

        public static void SetSortBindingMember(DependencyObject obj, BindingBase value)
        {
            obj.SetValue(SortBindingMemberProperty, value);
        }

        public static Boolean GetIsListviewSortable(DependencyObject obj)
        {
            //return true;
            return (Boolean) obj.GetValue(IsListviewSortableProperty);
        }

        public static void SetIsListviewSortable(DependencyObject obj, Boolean value)
        {
            obj.SetValue(IsListviewSortableProperty, value);
        }

        private static void OnRegisterSortableGrid(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            var grid = obj as ListView;
            if (grid == null) return;
            lv = grid;
            RegisterSortableGridView(grid, args);
        }

        private static void RegisterSortableGridView(IInputElement grid, DependencyPropertyChangedEventArgs args)
        {
            if (args.NewValue is Boolean && (Boolean) args.NewValue) grid.AddHandler(ButtonBase.ClickEvent, new RoutedEventHandler(GridViewColumnHeaderClickedHandler));
            else grid.AddHandler(ButtonBase.ClickEvent, new RoutedEventHandler(GridViewColumnHeaderClickedHandler));
        }

        private static void GridViewColumnHeaderClickedHandler(object sender, RoutedEventArgs e)
        {
            var headerClicked = e.OriginalSource as GridViewColumnHeader;

            if (headerClicked == null) return;
            ListSortDirection direction;
            if (headerClicked != lastHeaderClicked) direction = ListSortDirection.Ascending;

            else
            {
                direction = lastDirection == ListSortDirection.Ascending ? ListSortDirection.Descending : ListSortDirection.Ascending;
            }

            var header = String.Empty;

            try
            {
                header = ((Binding) GetSortBindingMember(headerClicked.Column)).Path.Path;
            }
            catch (Exception)
            {
            }

            if (header == String.Empty) return;

            Sort(header, direction);

            var resourceTemplateName = String.Empty;
            DataTemplate tmpTemplate;

            if (lastHeaderClicked != null)
            {
                resourceTemplateName = "HeaderTemplate";
                tmpTemplate = lv.TryFindResource(resourceTemplateName) as DataTemplate;
                lastHeaderClicked.Column.HeaderTemplate = tmpTemplate;
            }

            switch (direction)
            {
                case ListSortDirection.Ascending:
                    resourceTemplateName = "HeaderTemplateSortAsc";
                    break;
                case ListSortDirection.Descending:
                    resourceTemplateName = "HeaderTemplateSortDesc";
                    break;
            }

            tmpTemplate = lv.TryFindResource(resourceTemplateName) as DataTemplate;
            if (tmpTemplate != null) headerClicked.Column.HeaderTemplate = tmpTemplate;

            lastHeaderClicked = headerClicked;
            lastDirection = direction;
        }

        private static void Sort(string sortBy, ListSortDirection direction)
        {
            var view = (ListCollectionView) CollectionViewSource.GetDefaultView(lv.ItemsSource);

            if (view == null) {}
            else
            {
                try
                {
                    var sorter = (ListViewCustomComparer) GetCustomSorter(lv);
                    if (sorter != null)
                    {
                        sorter.AddSort(sortBy, direction);

                        view.CustomSort = sorter;
                        lv.Items.Refresh();
                    }
                    else
                    {
                        lv.Items.SortDescriptions.Clear();

                        var sd = new SortDescription(sortBy, direction);

                        lv.Items.SortDescriptions.Add(sd);
                        lv.Items.Refresh();
                    }
                }
                catch (Exception)
                {
                }
            }
        }
    }
}