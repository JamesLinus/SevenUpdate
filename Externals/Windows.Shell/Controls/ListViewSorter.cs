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
using System.Collections;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;

#endregion

namespace Microsoft.Windows.Controls
{
    public sealed class ListViewSorter
    {
        private static void CustomSorterCallback(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var listView = o as ItemsControl;
            if (listView == null)
                return;
            if (GetAutoSort(listView))
                return;
            if (String.IsNullOrEmpty(GetCustomSorter(listView)))
                return;
            if (e.OldValue != null && e.NewValue == null)
                listView.RemoveHandler(ButtonBase.ClickEvent, new RoutedEventHandler(ColumnHeader_Click));
            if (e.OldValue == null && e.NewValue != null)
                listView.AddHandler(ButtonBase.ClickEvent, new RoutedEventHandler(ColumnHeader_Click));
        }

        private static void AutoSortCallback(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var listView = o as ItemsControl;
            if (listView == null)
                return;
            if (GetAutoSort(listView))
                return;
            var oldValue = (bool) e.OldValue;
            var newValue = (bool) e.NewValue;
            if (oldValue && !newValue)
                listView.RemoveHandler(ButtonBase.ClickEvent, new RoutedEventHandler(ColumnHeader_Click));
            if (!oldValue && newValue)
                listView.AddHandler(ButtonBase.ClickEvent, new RoutedEventHandler(ColumnHeader_Click));
        }

        #region Public attached properties

        public static readonly DependencyProperty CustomSorterProperty = DependencyProperty.RegisterAttached("CustomSorter", typeof (string), typeof (ListViewSorter),
                                                                                                             new FrameworkPropertyMetadata(null, CustomSorterCallback));

        public static readonly DependencyProperty AutoSortProperty = DependencyProperty.RegisterAttached("AutoSort", typeof (bool), typeof (ListViewSorter),
                                                                                                         new UIPropertyMetadata(false, AutoSortCallback));

        public static readonly DependencyProperty PropertyNameProperty = DependencyProperty.RegisterAttached("PropertyName", typeof (string), typeof (ListViewSorter), new UIPropertyMetadata(null));
        public static readonly DependencyProperty ShowSortGlyphProperty = DependencyProperty.RegisterAttached("ShowSortGlyph", typeof (bool), typeof (ListViewSorter), new UIPropertyMetadata(true));

        public static readonly DependencyProperty SortGlyphAscendingProperty = DependencyProperty.RegisterAttached("SortGlyphAscending", typeof (ImageSource), typeof (ListViewSorter),
                                                                                                                   new UIPropertyMetadata(null));

        public static readonly DependencyProperty SortGlyphDescendingProperty = DependencyProperty.RegisterAttached("SortGlyphDescending", typeof (ImageSource), typeof (ListViewSorter),
                                                                                                                    new UIPropertyMetadata(null));

        /// <summary>
        ///   Gets the <see cref = "GridViewColumn" /> sorter
        /// </summary>
        /// <param name = "obj">The <see cref = "DependencyObject" /> to get the sorter from</param>
        /// <returns>an <see cref = "IComparer" /> for CustomSorter</returns>
        private static string GetCustomSorter(DependencyObject obj)
        {
            return (string) obj.GetValue(CustomSorterProperty);
        }

        /// <summary>
        ///   Sets the <see cref = "GridViewColumn" /> sorter
        /// </summary>
        /// <param name = "obj">The <see cref = "DependencyObject" /> to set the sorter to</param>
        /// <param name = "value">the <see cref = "IComparer" /> to set as the sorter</param>
        public static void SetCustomSorter(DependencyObject obj, string value)
        {
            obj.SetValue(CustomSorterProperty, value);
        }

        // Using a DependencyProperty as the backing store for Command.  This enables animation, styling, binding, etc...

        public static bool GetAutoSort(DependencyObject obj)
        {
            return (bool) obj.GetValue(AutoSortProperty);
        }

        public static void SetAutoSort(DependencyObject obj, bool value)
        {
            obj.SetValue(AutoSortProperty, value);
        }

        // Using a DependencyProperty as the backing store for AutoSort.  This enables animation, styling, binding, etc...

        public static string GetPropertyName(DependencyObject obj)
        {
            return (string) obj.GetValue(PropertyNameProperty);
        }

        public static void SetPropertyName(DependencyObject obj, string value)
        {
            obj.SetValue(PropertyNameProperty, value);
        }

        // Using a DependencyProperty as the backing store for PropertyName.  This enables animation, styling, binding, etc...

        public static bool GetShowSortGlyph(DependencyObject obj)
        {
            return (bool) obj.GetValue(ShowSortGlyphProperty);
        }

        public static void SetShowSortGlyph(DependencyObject obj, bool value)
        {
            obj.SetValue(ShowSortGlyphProperty, value);
        }

        // Using a DependencyProperty as the backing store for ShowSortGlyph.  This enables animation, styling, binding, etc...

        public static ImageSource GetSortGlyphAscending(DependencyObject obj)
        {
            return (ImageSource) obj.GetValue(SortGlyphAscendingProperty);
        }

        public static void SetSortGlyphAscending(DependencyObject obj, ImageSource value)
        {
            obj.SetValue(SortGlyphAscendingProperty, value);
        }

        // Using a DependencyProperty as the backing store for SortGlyphAscending.  This enables animation, styling, binding, etc...

        public static ImageSource GetSortGlyphDescending(DependencyObject obj)
        {
            return (ImageSource) obj.GetValue(SortGlyphDescendingProperty);
        }

        public static void SetSortGlyphDescending(DependencyObject obj, ImageSource value)
        {
            obj.SetValue(SortGlyphDescendingProperty, value);
        }

        // Using a DependencyProperty as the backing store for SortGlyphDescending.  This enables animation, styling, binding, etc...

        #endregion

        #region Private attached properties

        private static readonly DependencyProperty SortedColumnHeaderProperty = DependencyProperty.RegisterAttached("SortedColumnHeader", typeof (GridViewColumnHeader), typeof (ListViewSorter),
                                                                                                                    new UIPropertyMetadata(null));

        private static GridViewColumnHeader GetSortedColumnHeader(DependencyObject obj)
        {
            return (GridViewColumnHeader) obj.GetValue(SortedColumnHeaderProperty);
        }

        private static void SetSortedColumnHeader(DependencyObject obj, GridViewColumnHeader value)
        {
            obj.SetValue(SortedColumnHeaderProperty, value);
        }

        // Using a DependencyProperty as the backing store for SortedColumn.  This enables animation, styling, binding, etc...

        #endregion

        #region Column header click event handler

        private static void ColumnHeader_Click(object sender, RoutedEventArgs e)
        {
            var headerClicked = e.OriginalSource as GridViewColumnHeader;
            if (headerClicked == null || headerClicked.Column == null)
                return;
            var propertyName = GetPropertyName(headerClicked.Column);
            if (string.IsNullOrEmpty(propertyName))
                return;
            var listView = GetAncestor<ListView>(headerClicked);
            if (listView == null)
                return;
            var sorter = GetCustomSorter(listView);
            currentSortDirection = currentSortDirection == ListSortDirection.Ascending ? ListSortDirection.Descending : ListSortDirection.Ascending;
            if (sorter != null)
            {
                var view = (ListCollectionView) CollectionViewSource.GetDefaultView(listView.ItemsSource);
                if (view != null)
                    ApplyCustomSort(view, propertyName, listView, headerClicked);
            }
            else if (GetAutoSort(listView))
                ApplySort(listView.Items, propertyName, listView, headerClicked);
        }

        #endregion

        #region Helper methods

        private static ListSortDirection currentSortDirection;

        public static T GetAncestor<T>(DependencyObject reference) where T : DependencyObject
        {
            DependencyObject parent = VisualTreeHelper.GetParent(reference);
            while (!(parent is T))
                parent = VisualTreeHelper.GetParent(parent);
            return (T) parent;
        }

        public static void ApplySort(ICollectionView view, string propertyName, ListView listView, GridViewColumnHeader sortedColumnHeader)
        {
            var direction = ListSortDirection.Ascending;

            if (view.SortDescriptions.Count > 0)
            {
                var currentSort = view.SortDescriptions[0];
                if (currentSort.PropertyName == propertyName)
                    direction = currentSort.Direction == ListSortDirection.Ascending ? ListSortDirection.Descending : ListSortDirection.Ascending;
                view.SortDescriptions.Clear();

                var currentSortedColumnHeader = GetSortedColumnHeader(listView);
                if (currentSortedColumnHeader != null)
                    RemoveSortGlyph(currentSortedColumnHeader);
            }
            if (string.IsNullOrEmpty(propertyName))
                return;
            view.SortDescriptions.Add(new SortDescription(propertyName, direction));
            if (GetShowSortGlyph(listView))
                AddSortGlyph(sortedColumnHeader, direction, direction == ListSortDirection.Ascending ? GetSortGlyphAscending(listView) : GetSortGlyphDescending(listView));
            SetSortedColumnHeader(listView, sortedColumnHeader);
        }

        public static void ApplyCustomSort(ListCollectionView view, string propertyName, ListView listView, GridViewColumnHeader sortedColumnHeader)
        {
            if (view.SortDescriptions.Count > 0)
            {
                var currentSort = view.SortDescriptions[0];
                if (currentSort.PropertyName == propertyName)
                    currentSortDirection = currentSort.Direction == ListSortDirection.Ascending ? ListSortDirection.Descending : ListSortDirection.Ascending;
            }

            try
            {
                var sorter = Activator.CreateInstance(Type.GetType(GetCustomSorter(listView))) as ListViewCustomComparer;
                if (sorter != null)
                {
                    sorter.AddSort(propertyName, currentSortDirection);

                    view.CustomSort = sorter;

                    listView.Items.Refresh();
                }
                else
                {
                    view.SortDescriptions.Clear();
                    view.SortDescriptions.Add(new SortDescription(propertyName, currentSortDirection));
                }
            }
            catch
            {
            }

            var currentSortedColumnHeader = GetSortedColumnHeader(listView);
            if (currentSortedColumnHeader != null)
                RemoveSortGlyph(currentSortedColumnHeader);
            if (string.IsNullOrEmpty(propertyName))
                return;

            if (GetShowSortGlyph(listView))
                AddSortGlyph(sortedColumnHeader, currentSortDirection, currentSortDirection == ListSortDirection.Ascending ? GetSortGlyphAscending(listView) : GetSortGlyphDescending(listView));
            SetSortedColumnHeader(listView, sortedColumnHeader);
        }

        private static void AddSortGlyph(GridViewColumnHeader columnHeader, ListSortDirection direction, ImageSource sortGlyph)
        {
            var adornerLayer = AdornerLayer.GetAdornerLayer(columnHeader);
            adornerLayer.Add(new SortGlyphAdorner(columnHeader, direction, sortGlyph));
        }

        private static void RemoveSortGlyph(GridViewColumnHeader columnHeader)
        {
            var adornerLayer = AdornerLayer.GetAdornerLayer(columnHeader);
            var adorners = adornerLayer.GetAdorners(columnHeader);
            if (adorners == null)
                return;
            foreach (var adorner in adorners)
            {
                if (adorner is SortGlyphAdorner)
                    adornerLayer.Remove(adorner);
            }
        }

        #endregion

        #region SortGlyphAdorner nested class

        private class SortGlyphAdorner : Adorner
        {
            private readonly GridViewColumnHeader columnHeader;
            private readonly ListSortDirection direction;
            private readonly ImageSource sortGlyph;

            public SortGlyphAdorner(GridViewColumnHeader columnHeader, ListSortDirection direction, ImageSource sortGlyph) : base(columnHeader)
            {
                this.columnHeader = columnHeader;
                this.direction = direction;
                this.sortGlyph = sortGlyph;
            }

            private Geometry GetDefaultGlyph()
            {
                var x1 = columnHeader.ActualWidth - 13;
                var x2 = x1 + 10;
                var x3 = x1 + 5;
                var y1 = columnHeader.ActualHeight/2 - 3;
                var y2 = y1 + 5;

                if (direction == ListSortDirection.Ascending)
                {
                    double tmp = y1;
                    y1 = y2;
                    y2 = tmp;
                }

                var pathSegmentCollection = new PathSegmentCollection();
                pathSegmentCollection.Add(new LineSegment(new Point(x2, y1), true));
                pathSegmentCollection.Add(new LineSegment(new Point(x3, y2), true));

                var pathFigure = new PathFigure(new Point(x1, y1), pathSegmentCollection, true);

                var pathFigureCollection = new PathFigureCollection();
                pathFigureCollection.Add(pathFigure);

                var pathGeometry = new PathGeometry(pathFigureCollection);
                return pathGeometry;
            }

            protected override void OnRender(DrawingContext drawingContext)
            {
                base.OnRender(drawingContext);

                if (sortGlyph != null)
                {
                    var x = columnHeader.ActualWidth - 13;
                    var y = columnHeader.ActualHeight/2 - 5;
                    var rect = new Rect(x, y, 10, 10);
                    drawingContext.DrawImage(sortGlyph, rect);
                }
                else
                    drawingContext.DrawGeometry(new SolidColorBrush(Colors.LightGray) {Opacity = 0.5}, new Pen(Brushes.Gray, 0.5), GetDefaultGlyph());
            }
        }

        #endregion
    }
}