// <copyright file="ListViewSorter.cs" project="SevenSoftware.Windows">Thomas Levesque</copyright>
// <license href="http://www.gnu.org/licenses/gpl-3.0.txt" name="GNU General Public License" />

using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;

namespace SevenSoftware.Windows.Controls
{
    /// <summary>Sorts a <c>ListView</c>.</summary>
    public static class ListViewSorter
    {
        /// <summary>Indicates if the <c>ListView</c> will auto sort.</summary>
        static readonly DependencyProperty AutoSortProperty = DependencyProperty.RegisterAttached(
            "AutoSort", typeof(bool), typeof(ListViewSorter), new UIPropertyMetadata(false, AutoSortCallback));

        /// <summary>Indicates a custom sorter that will be used.</summary>
        static readonly DependencyProperty CustomSorterProperty = DependencyProperty.RegisterAttached(
            "CustomSorter", 
            typeof(string), 
            typeof(ListViewSorter), 
            new FrameworkPropertyMetadata(null, CustomSorterCallback));

        /// <summary>The property name to sort.</summary>
        static readonly DependencyProperty PropertyNameProperty = DependencyProperty.RegisterAttached(
            "PropertyName", typeof(string), typeof(ListViewSorter), new UIPropertyMetadata(null));

        /// <summary>The sort arrow up.</summary>
        static readonly DependencyProperty SortGlyphAscendingProperty =
            DependencyProperty.RegisterAttached(
                "SortGlyphAscending", typeof(ImageSource), typeof(ListViewSorter), new UIPropertyMetadata(null));

        /// <summary>The sort arrow down.</summary>
        static readonly DependencyProperty SortGlyphDescendingProperty =
            DependencyProperty.RegisterAttached(
                "SortGlyphDescending", typeof(ImageSource), typeof(ListViewSorter), new UIPropertyMetadata(null));

        /// <summary>The column header that was sorted.</summary>
        static readonly DependencyProperty SortedColumnHeaderProperty =
            DependencyProperty.RegisterAttached(
                "SortedColumnHeader", typeof(GridViewColumnHeader), typeof(ListViewSorter), new UIPropertyMetadata(null));

        /// <summary>The current sort direction.</summary>
        static ListSortDirection currentSortDirection;

        /// <summary>Sets the auto sort.</summary>
        /// <param name="obj">The dependency object.</param>
        /// <param name="value">If set to <c>True</c> auto sorting will be used.</param>
        public static void SetAutoSort(DependencyObject obj, bool value)
        {
            if (obj == null)
            {
                throw new ArgumentNullException("obj");
            }

            obj.SetValue(AutoSortProperty, value);
        }

        /// <summary>Sets the <c>GridViewColumn</c> sorter.</summary>
        /// <param name="obj">The <c>DependencyObject</c> to set the sorter to.</param>
        /// <param name="comparerName">The <c>IComparer</c> to set as the sorter.</param>
        public static void SetCustomSorter(DependencyObject obj, string comparerName)
        {
            if (obj == null)
            {
                throw new ArgumentNullException("obj");
            }

            if (string.IsNullOrWhiteSpace(comparerName))
            {
                throw new ArgumentNullException("comparerName");
            }

            obj.SetValue(CustomSorterProperty, comparerName);
        }

        /// <summary>Sets the name of the property.</summary>
        /// <param name="obj">The dependency object.</param>
        /// <param name="name">The value.</param>
        public static void SetPropertyName(DependencyObject obj, string name)
        {
            if (obj == null)
            {
                throw new ArgumentNullException("obj");
            }

            if (name == null)
            {
                throw new ArgumentNullException("name");
            }

            obj.SetValue(PropertyNameProperty, name);
        }

        /// <summary>Sets the sort glyph ascending.</summary>
        /// <param name="obj">The dependency object.</param>
        /// <param name="value">The value.</param>
        public static void SetSortGlyphAscending(DependencyObject obj, ImageSource value)
        {
            if (obj == null)
            {
                throw new ArgumentNullException("obj");
            }

            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            obj.SetValue(SortGlyphAscendingProperty, value);
        }

        /// <summary>Sets the sort glyph descending.</summary>
        /// <param name="obj">The dependency object.</param>
        /// <param name="value">The value.</param>
        public static void SetSortGlyphDescending(DependencyObject obj, ImageSource value)
        {
            if (obj == null)
            {
                throw new ArgumentNullException("obj");
            }

            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            obj.SetValue(SortGlyphDescendingProperty, value);
        }

        /// <summary>Sets the sorted column header.</summary>
        /// <param name="obj">The dependency object.</param>
        /// <param name="header">The column header.</param>
        public static void SetSortedColumnHeader(DependencyObject obj, GridViewColumnHeader header)
        {
            if (obj == null)
            {
                throw new ArgumentNullException("obj");
            }

            if (header == null)
            {
                throw new ArgumentNullException("header");
            }

            obj.SetValue(SortedColumnHeaderProperty, header);
        }

        /// <summary>Adds the sort glyph.</summary>
        /// <param name="columnHeader">The column header.</param>
        /// <param name="sortGlyph">The sort glyph.</param>
        static void AddSortGlyph(GridViewColumnHeader columnHeader, ImageSource sortGlyph)
        {
            AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(columnHeader);
            var glyph = new SortGlyphAdorner(columnHeader, sortGlyph);
            adornerLayer.Add(glyph);
        }

        /// <summary>Applies the custom sort.</summary>
        /// <param name="view">The collection view.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="listView">The list view to sort.</param>
        /// <param name="sortedColumnHeader">The sorted column header.</param>
        static void ApplyCustomSort(
            ListCollectionView view, string propertyName, ItemsControl listView, GridViewColumnHeader sortedColumnHeader)
        {
            if (view == null)
            {
                throw new ArgumentNullException("view");
            }

            if (listView == null)
            {
                throw new ArgumentNullException("listView");
            }

            if (sortedColumnHeader == null)
            {
                throw new ArgumentNullException("sortedColumnHeader");
            }

            if (string.IsNullOrEmpty(propertyName))
            {
                throw new ArgumentNullException("propertyName");
            }

            if (view.SortDescriptions.Count > 0)
            {
                SortDescription currentSort = view.SortDescriptions[0];
                if (currentSort.PropertyName == propertyName)
                {
                    currentSortDirection = currentSort.Direction == ListSortDirection.Ascending
                                               ? ListSortDirection.Descending : ListSortDirection.Ascending;
                }
            }

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

            GridViewColumnHeader currentSortedColumnHeader = GetSortedColumnHeader(listView);
            if (currentSortedColumnHeader != null)
            {
                RemoveSortGlyph(currentSortedColumnHeader);
            }

            if (string.IsNullOrEmpty(propertyName))
            {
                return;
            }

            ImageSource sort = currentSortDirection == ListSortDirection.Ascending
                                   ? GetSortGlyphAscending(listView) : GetSortGlyphDescending(listView);

            AddSortGlyph(sortedColumnHeader, sort);
            SetSortedColumnHeader(listView, sortedColumnHeader);
        }

        /// <summary>Applies the sort.</summary>
        /// <param name="view">The collection view.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="listView">The list view.</param>
        /// <param name="sortedColumnHeader">The sorted column header.</param>
        static void ApplySort(
            ICollectionView view, 
            string propertyName, 
            DependencyObject listView, 
            GridViewColumnHeader sortedColumnHeader)
        {
            var direction = ListSortDirection.Ascending;

            if (view.SortDescriptions.Count > 0)
            {
                SortDescription currentSort = view.SortDescriptions[0];
                if (currentSort.PropertyName == propertyName)
                {
                    direction = currentSort.Direction == ListSortDirection.Ascending
                                    ? ListSortDirection.Descending : ListSortDirection.Ascending;
                }

                view.SortDescriptions.Clear();

                GridViewColumnHeader currentSortedColumnHeader = GetSortedColumnHeader(listView);
                if (currentSortedColumnHeader != null)
                {
                    RemoveSortGlyph(currentSortedColumnHeader);
                }
            }

            if (string.IsNullOrEmpty(propertyName))
            {
                return;
            }

            view.SortDescriptions.Add(new SortDescription(propertyName, direction));
            ImageSource sort = direction == ListSortDirection.Ascending
                                   ? GetSortGlyphAscending(listView) : GetSortGlyphDescending(listView);
            AddSortGlyph(sortedColumnHeader, sort);

            SetSortedColumnHeader(listView, sortedColumnHeader);
        }

        /// <summary>The auto sort callback.</summary>
        /// <param name="o">The dependency object.</param>
        /// <param name="e">The <c>System.Windows.DependencyPropertyChangedEventArgs</c> instance containing the event data.</param>
        static void AutoSortCallback(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var listView = o as ItemsControl;
            if (listView == null)
            {
                return;
            }

            if (GetAutoSort(listView))
            {
                return;
            }

            var oldValue = (bool)e.OldValue;
            var newValue = (bool)e.NewValue;
            if (oldValue && !newValue)
            {
                listView.RemoveHandler(ButtonBase.ClickEvent, new RoutedEventHandler(SortColumn));
            }

            if (!oldValue && newValue)
            {
                listView.AddHandler(ButtonBase.ClickEvent, new RoutedEventHandler(SortColumn));
            }
        }

        /// <summary>Customs the sorter callback.</summary>
        /// <param name="o">The dependency object.</param>
        /// <param name="e">The <c>System.Windows.DependencyPropertyChangedEventArgs</c> instance containing the event data.</param>
        static void CustomSorterCallback(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var listView = o as ItemsControl;
            if (listView == null)
            {
                return;
            }

            if (GetAutoSort(listView))
            {
                return;
            }

            if (string.IsNullOrEmpty(GetCustomSorter(listView)))
            {
                return;
            }

            if (e.OldValue != null && e.NewValue == null)
            {
                listView.RemoveHandler(ButtonBase.ClickEvent, new RoutedEventHandler(SortColumn));
            }

            if (e.OldValue == null && e.NewValue != null)
            {
                listView.AddHandler(ButtonBase.ClickEvent, new RoutedEventHandler(SortColumn));
            }
        }

        /// <summary>Gets the ancestor.</summary>
        /// <typeparameter name="T">The Ancestor class</typeparameter>
        /// <param name="reference">The reference.</param>
        /// <returns>Returns the ancestor class.</returns>
        /// <typeparam name="T">The ancestor class.</typeparam>
        static T GetAncestor<T>(DependencyObject reference) where T : DependencyObject
        {
            DependencyObject parent = VisualTreeHelper.GetParent(reference);
            while (!(parent is T))
            {
                parent = VisualTreeHelper.GetParent(parent);
            }

            return (T)parent;
        }

        /// <summary>Gets the auto sort.</summary>
        /// <param name="obj">The dependency object.</param>
        /// <returns><c>True</c> if the sorting is done automatic; otherwise, <c>False</c>.</returns>
        static bool GetAutoSort(DependencyObject obj)
        {
            return (bool)obj.GetValue(AutoSortProperty);
        }

        /// <summary>Gets the <c>GridViewColumn</c> sorter.</summary>
        /// <param name="obj">The <c>DependencyObject</c> to get the sorter from.</param>
        /// <returns>An <c>IComparer</c> for CustomSorter.</returns>
        static string GetCustomSorter(DependencyObject obj)
        {
            return (string)obj.GetValue(CustomSorterProperty);
        }

        /// <summary>Gets the name of the property.</summary>
        /// <param name="obj">The dependency object.</param>
        /// <returns>The property name.</returns>
        static string GetPropertyName(DependencyObject obj)
        {
            return (string)obj.GetValue(PropertyNameProperty);
        }

        /// <summary>Gets the sort glyph ascending.</summary>
        /// <param name="obj">The dependency object.</param>
        /// <returns>The <c>ImageSource</c> for the sort glyph.</returns>
        static ImageSource GetSortGlyphAscending(DependencyObject obj)
        {
            return (ImageSource)obj.GetValue(SortGlyphAscendingProperty);
        }

        /// <summary>Gets the sort glyph descending.</summary>
        /// <param name="obj">The dependency object.</param>
        /// <returns>The <c>ImageSource</c> for the sort glyph.</returns>
        static ImageSource GetSortGlyphDescending(DependencyObject obj)
        {
            return (ImageSource)obj.GetValue(SortGlyphDescendingProperty);
        }

        /// <summary>Gets the sorted column header.</summary>
        /// <param name="obj">The dependency object.</param>
        /// <returns>Returns the column header.</returns>
        static GridViewColumnHeader GetSortedColumnHeader(DependencyObject obj)
        {
            return (GridViewColumnHeader)obj.GetValue(SortedColumnHeaderProperty);
        }

        /// <summary>Removes the sort glyph.</summary>
        /// <param name="columnHeader">The column header.</param>
        static void RemoveSortGlyph(UIElement columnHeader)
        {
            AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(columnHeader);
            Adorner[] adorners = adornerLayer.GetAdorners(columnHeader);
            if (adorners == null)
            {
                return;
            }

            foreach (var adorner in adorners.OfType<SortGlyphAdorner>())
            {
                adornerLayer.Remove(adorner);
            }
        }

        /// <summary>Handles the Click event of the <c>GridViewColumnHeader</c> control.</summary>
        /// <param name="sender">The object that called the event.</param>
        /// <param name="e">The <c>System.Windows.RoutedEventArgs</c> instance containing the event data.</param>
        static void SortColumn(object sender, RoutedEventArgs e)
        {
            var headerClicked = e.OriginalSource as GridViewColumnHeader;
            if (headerClicked == null || headerClicked.Column == null)
            {
                return;
            }

            string propertyName = GetPropertyName(headerClicked.Column);
            if (string.IsNullOrEmpty(propertyName))
            {
                return;
            }

            var listView = GetAncestor<ListView>(headerClicked);
            if (listView == null)
            {
                return;
            }

            string sorter = GetCustomSorter(listView);
            currentSortDirection = currentSortDirection == ListSortDirection.Ascending
                                       ? ListSortDirection.Descending : ListSortDirection.Ascending;
            if (sorter != null)
            {
                var view = (ListCollectionView)CollectionViewSource.GetDefaultView(listView.ItemsSource);
                if (view != null)
                {
                    ApplyCustomSort(view, propertyName, listView, headerClicked);
                }
            }
            else if (GetAutoSort(listView))
            {
                ApplySort(listView.Items, propertyName, listView, headerClicked);
            }
        }

        /// <summary>The sort glyph.</summary>
        class SortGlyphAdorner : Adorner
        {
            /// <summary>The ascending arrow.</summary>
            static readonly Geometry AscGeometry = Geometry.Parse("M 0,0 L 10,0 L 5,5 Z");

            /// <summary>The descending arrow.</summary>
            static readonly Geometry DescGeometry = Geometry.Parse("M 0,5 L 10,5 L 5,0 Z");

            /// <summary>The column header.</summary>
            readonly GridViewColumnHeader columnHeader;

            /// <summary>The sort glyph image.</summary>
            readonly ImageSource sortGlyph;

            /// <summary>Initializes a new instance of the <see cref="SortGlyphAdorner" /> class.</summary>
            /// <param name="columnHeader">The column header.</param>
            /// <param name="sortGlyph">The sort glyph.</param>
            public SortGlyphAdorner(GridViewColumnHeader columnHeader, ImageSource sortGlyph) : base(columnHeader)
            {
                this.columnHeader = columnHeader;
                this.sortGlyph = sortGlyph;
            }

            /// <summary>
            ///   When overridden in a derived class, participates in rendering operations that are directed by the
            ///   layout system. The rendering instructions for this element are not used directly when this method is
            ///   invoked, and are instead preserved for later asynchronous use by layout and drawing.
            /// </summary>
            /// <param name="drawingContext">The drawing instructions for a specific element. This context is provided to the layout system.</param>
            protected override void OnRender(DrawingContext drawingContext)
            {
                if (drawingContext == null)
                {
                    throw new ArgumentNullException("drawingContext");
                }

                base.OnRender(drawingContext);

                if (sortGlyph != null)
                {
                    double x = columnHeader.ActualWidth - 13;
                    double y = columnHeader.ActualHeight / (2 - 5);
                    var rect = new Rect(x, y, 10, 10);
                    drawingContext.DrawImage(sortGlyph, rect);
                }
                else
                {
                    if (AdornedElement.RenderSize.Width < 20)
                    {
                        return;
                    }

                    drawingContext.PushTransform(
                        new TranslateTransform(
                            AdornedElement.RenderSize.Width - 15, (AdornedElement.RenderSize.Height - 5) / 2));

                    drawingContext.DrawGeometry(
                        Brushes.LightGray, 
                        new Pen(Brushes.Gray, 1.0), 
                        currentSortDirection == ListSortDirection.Ascending ? AscGeometry : DescGeometry);

                    drawingContext.Pop();
                }
            }
        }
    }
}