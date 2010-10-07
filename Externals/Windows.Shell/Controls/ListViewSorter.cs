// ***********************************************************************
// Assembly         : System.Windows
// Author           : Robert Baker (sevenalive)
// Last Modified By : Robert Baker (sevenalive)
// Last Modified On : 10-06-2010
// Copyright        : (c) Seven Software. All rights reserved.
// ***********************************************************************
namespace System.Windows.Controls
{
    using System.Collections;
    using System.ComponentModel;
    using System.Linq;
    using System.Windows.Controls.Primitives;
    using System.Windows.Data;
    using System.Windows.Documents;
    using System.Windows.Media;

    /// <summary>
    /// Sorts a <see cref="ListView"/>
    /// </summary>
    public sealed class ListViewSorter
    {
        #region Constants and Fields

        /// <summary>
        ///   Indicates if the <see cref = "ListView" /> will auto sort
        /// </summary>
        public static readonly DependencyProperty AutoSortProperty = DependencyProperty.RegisterAttached(
            "AutoSort", typeof(bool), typeof(ListViewSorter), new UIPropertyMetadata(false, AutoSortCallback));

        /// <summary>
        ///   Indicates a custom sorter that will be used
        /// </summary>
        public static readonly DependencyProperty CustomSorterProperty = DependencyProperty.RegisterAttached(
            "CustomSorter", typeof(string), typeof(ListViewSorter), new FrameworkPropertyMetadata(null, CustomSorterCallback));

        /// <summary>
        ///   The property name to sort
        /// </summary>
        public static readonly DependencyProperty PropertyNameProperty = DependencyProperty.RegisterAttached(
            "PropertyName", typeof(string), typeof(ListViewSorter), new UIPropertyMetadata(null));

        /// <summary>
        ///   The sort arrow
        /// </summary>
        public static readonly DependencyProperty ShowSortGlyphProperty = DependencyProperty.RegisterAttached(
            "ShowSortGlyph", typeof(bool), typeof(ListViewSorter), new UIPropertyMetadata(true));

        /// <summary>
        ///   The sort arrow up
        /// </summary>
        public static readonly DependencyProperty SortGlyphAscendingProperty = DependencyProperty.RegisterAttached(
            "SortGlyphAscending", typeof(ImageSource), typeof(ListViewSorter), new UIPropertyMetadata(null));

        /// <summary>
        ///   The sort arrow down
        /// </summary>
        public static readonly DependencyProperty SortGlyphDescendingProperty = DependencyProperty.RegisterAttached(
            "SortGlyphDescending", typeof(ImageSource), typeof(ListViewSorter), new UIPropertyMetadata(null));

        /// <summary>
        ///   The column header that was sorted
        /// </summary>
        private static readonly DependencyProperty SortedColumnHeaderProperty = DependencyProperty.RegisterAttached(
            "SortedColumnHeader", typeof(GridViewColumnHeader), typeof(ListViewSorter), new UIPropertyMetadata(null));

        /// <summary>
        ///   The current sort direction
        /// </summary>
        private static ListSortDirection currentSortDirection;

        #endregion

        #region Public Methods

        /// <summary>
        /// Applies the custom sort.
        /// </summary>
        /// <parameter name="view">
        /// The collection view
        /// </parameter>
        /// <parameter name="propertyName">
        /// Name of the property.
        /// </parameter>
        /// <parameter name="listView">
        /// The list view to sort
        /// </parameter>
        /// <parameter name="sortedColumnHeader">
        /// The sorted column header.
        /// </parameter>
        public static void ApplyCustomSort(ListCollectionView view, string propertyName, ListView listView, GridViewColumnHeader sortedColumnHeader)
        {
            if (view.SortDescriptions.Count > 0)
            {
                var currentSort = view.SortDescriptions[0];
                if (currentSort.PropertyName == propertyName)
                {
                    currentSortDirection = currentSort.Direction == ListSortDirection.Ascending ? ListSortDirection.Descending : ListSortDirection.Ascending;
                }
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
            catch (Exception)
            {
            }

            var currentSortedColumnHeader = GetSortedColumnHeader(listView);
            if (currentSortedColumnHeader != null)
            {
                RemoveSortGlyph(currentSortedColumnHeader);
            }

            if (string.IsNullOrEmpty(propertyName))
            {
                return;
            }

            if (GetShowSortGlyph(listView))
            {
                AddSortGlyph(
                    sortedColumnHeader, 
                    currentSortDirection, 
                    currentSortDirection == ListSortDirection.Ascending ? GetSortGlyphAscending(listView) : GetSortGlyphDescending(listView));
            }

            SetSortedColumnHeader(listView, sortedColumnHeader);
        }

        /// <summary>
        /// Applies the sort.
        /// </summary>
        /// <parameter name="view">
        /// The collection view.
        /// </parameter>
        /// <parameter name="propertyName">
        /// Name of the property.
        /// </parameter>
        /// <parameter name="listView">
        /// The list view.
        /// </parameter>
        /// <parameter name="sortedColumnHeader">
        /// The sorted column header.
        /// </parameter>
        public static void ApplySort(ICollectionView view, string propertyName, ListView listView, GridViewColumnHeader sortedColumnHeader)
        {
            var direction = ListSortDirection.Ascending;

            if (view.SortDescriptions.Count > 0)
            {
                var currentSort = view.SortDescriptions[0];
                if (currentSort.PropertyName == propertyName)
                {
                    direction = currentSort.Direction == ListSortDirection.Ascending ? ListSortDirection.Descending : ListSortDirection.Ascending;
                }

                view.SortDescriptions.Clear();

                var currentSortedColumnHeader = GetSortedColumnHeader(listView);
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
            if (GetShowSortGlyph(listView))
            {
                AddSortGlyph(sortedColumnHeader, direction, direction == ListSortDirection.Ascending ? GetSortGlyphAscending(listView) : GetSortGlyphDescending(listView));
            }

            SetSortedColumnHeader(listView, sortedColumnHeader);
        }

        /// <summary>
        /// Gets the ancestor.
        /// </summary>
        /// <typeparameter name="T">
        /// The Ancestor class
        /// </typeparameter>
        /// <parameter name="reference">
        /// The reference.
        /// </parameter>
        /// <returns>
        /// Returns the ancestor class
        /// </returns>
        public static T GetAncestor<T>(DependencyObject reference) where T : DependencyObject
        {
            var parent = VisualTreeHelper.GetParent(reference);
            while (!(parent is T))
            {
                parent = VisualTreeHelper.GetParent(parent);
            }

            return (T)parent;
        }

        // Using a DependencyProperty as the backing store for Command.  This enables animation, styling, binding, etc...

        /// <summary>
        /// Gets the auto sort.
        /// </summary>
        /// <parameter name="obj">
        /// The dependency object
        /// </parameter>
        /// <returns>
        /// <see langword="true"/> if the sorting is done automatic; otherwise, <see langword="false"/>
        /// </returns>
        public static bool GetAutoSort(DependencyObject obj)
        {
            return (bool)obj.GetValue(AutoSortProperty);
        }

        // Using a DependencyProperty as the backing store for AutoSort.  This enables animation, styling, binding, etc...

        /// <summary>
        /// Gets the name of the property.
        /// </summary>
        /// <parameter name="obj">
        /// The dependency object
        /// </parameter>
        /// <returns>
        /// The property name
        /// </returns>
        public static string GetPropertyName(DependencyObject obj)
        {
            return (string)obj.GetValue(PropertyNameProperty);
        }

        // Using a DependencyProperty as the backing store for PropertyName.  This enables animation, styling, binding, etc...

        /// <summary>
        /// Gets the show sort glyph.
        /// </summary>
        /// <parameter name="obj">
        /// The dependency object
        /// </parameter>
        /// <returns>
        /// <see langword="true"/> if the sort glyph is shown; otherwise, <see langword="false"/>
        /// </returns>
        public static bool GetShowSortGlyph(DependencyObject obj)
        {
            return (bool)obj.GetValue(ShowSortGlyphProperty);
        }

        // Using a DependencyProperty as the backing store for ShowSortGlyph.  This enables animation, styling, binding, etc...

        /// <summary>
        /// Gets the sort glyph ascending.
        /// </summary>
        /// <parameter name="obj">
        /// The dependency object
        /// </parameter>
        /// <returns>
        /// The <see cref="ImageSource"/> for the sort glyph
        /// </returns>
        public static ImageSource GetSortGlyphAscending(DependencyObject obj)
        {
            return (ImageSource)obj.GetValue(SortGlyphAscendingProperty);
        }

        // Using a DependencyProperty as the backing store for SortGlyphAscending.  This enables animation, styling, binding, etc...

        /// <summary>
        /// Gets the sort glyph descending.
        /// </summary>
        /// <parameter name="obj">
        /// The dependency object
        /// </parameter>
        /// <returns>
        /// The <see cref="ImageSource"/> for the sort glyph
        /// </returns>
        public static ImageSource GetSortGlyphDescending(DependencyObject obj)
        {
            return (ImageSource)obj.GetValue(SortGlyphDescendingProperty);
        }

        /// <summary>
        /// Sets the auto sort.
        /// </summary>
        /// <parameter name="obj">
        /// The dependency object
        /// </parameter>
        /// <parameter name="value">
        /// if set to <see langword="true"/> [value].
        /// </parameter>
        public static void SetAutoSort(DependencyObject obj, bool value)
        {
            obj.SetValue(AutoSortProperty, value);
        }

        /// <summary>
        /// Sets the <see cref="GridViewColumn"/> sorter
        /// </summary>
        /// <parameter name="obj">
        /// The <see cref="DependencyObject"/> to set the sorter to
        /// </parameter>
        /// <parameter name="value">
        /// the <see cref="IComparer"/> to set as the sorter
        /// </parameter>
        public static void SetCustomSorter(DependencyObject obj, string value)
        {
            obj.SetValue(CustomSorterProperty, value);
        }

        /// <summary>
        /// Sets the name of the property.
        /// </summary>
        /// <parameter name="obj">
        /// The dependency object
        /// </parameter>
        /// <parameter name="value">
        /// The value.
        /// </parameter>
        public static void SetPropertyName(DependencyObject obj, string value)
        {
            obj.SetValue(PropertyNameProperty, value);
        }

        /// <summary>
        /// Sets the show sort glyph.
        /// </summary>
        /// <parameter name="obj">
        /// The dependency object
        /// </parameter>
        /// <parameter name="value">
        /// if set to <see langword="true"/> [value].
        /// </parameter>
        public static void SetShowSortGlyph(DependencyObject obj, bool value)
        {
            obj.SetValue(ShowSortGlyphProperty, value);
        }

        /// <summary>
        /// Sets the sort glyph ascending.
        /// </summary>
        /// <parameter name="obj">
        /// The dependency object
        /// </parameter>
        /// <parameter name="value">
        /// The value.
        /// </parameter>
        public static void SetSortGlyphAscending(DependencyObject obj, ImageSource value)
        {
            obj.SetValue(SortGlyphAscendingProperty, value);
        }

        /// <summary>
        /// Sets the sort glyph descending.
        /// </summary>
        /// <parameter name="obj">
        /// The dependency object
        /// </parameter>
        /// <parameter name="value">
        /// The value.
        /// </parameter>
        public static void SetSortGlyphDescending(DependencyObject obj, ImageSource value)
        {
            obj.SetValue(SortGlyphDescendingProperty, value);
        }

        #endregion

        // Using a DependencyProperty as the backing store for SortGlyphDescending.  This enables animation, styling, binding, etc...
        #region Methods

        /// <summary>
        /// Adds the sort glyph.
        /// </summary>
        /// <parameter name="columnHeader">
        /// The column header.
        /// </parameter>
        /// <parameter name="direction">
        /// The direction.
        /// </parameter>
        /// <parameter name="sortGlyph">
        /// The sort glyph.
        /// </parameter>
        private static void AddSortGlyph(GridViewColumnHeader columnHeader, ListSortDirection direction, ImageSource sortGlyph)
        {
            var adornerLayer = AdornerLayer.GetAdornerLayer(columnHeader);
            adornerLayer.Add(new SortGlyphAdorner(columnHeader, direction, sortGlyph));
        }

        /// <summary>
        /// The auto sort callback
        /// </summary>
        /// <parameter name="o">
        /// The dependency object
        /// </parameter>
        /// <parameter name="e">
        /// The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.
        /// </parameter>
        private static void AutoSortCallback(DependencyObject o, DependencyPropertyChangedEventArgs e)
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

        /// <summary>
        /// Customs the sorter callback.
        /// </summary>
        /// <parameter name="o">
        /// The dependency object
        /// </parameter>
        /// <parameter name="e">
        /// The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.
        /// </parameter>
        private static void CustomSorterCallback(DependencyObject o, DependencyPropertyChangedEventArgs e)
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

            if (String.IsNullOrEmpty(GetCustomSorter(listView)))
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

        /// <summary>
        /// Gets the <see cref="GridViewColumn"/> sorter
        /// </summary>
        /// <parameter name="obj">
        /// The <see cref="DependencyObject"/> to get the sorter from
        /// </parameter>
        /// <returns>
        /// an <see cref="IComparer"/> for CustomSorter
        /// </returns>
        private static string GetCustomSorter(DependencyObject obj)
        {
            return (string)obj.GetValue(CustomSorterProperty);
        }

        /// <summary>
        /// Gets the sorted column header.
        /// </summary>
        /// <parameter name="obj">
        /// The dependency object.
        /// </parameter>
        /// <returns>
        /// Returns the column header
        /// </returns>
        private static GridViewColumnHeader GetSortedColumnHeader(DependencyObject obj)
        {
            return (GridViewColumnHeader)obj.GetValue(SortedColumnHeaderProperty);
        }

        /// <summary>
        /// Removes the sort glyph.
        /// </summary>
        /// <parameter name="columnHeader">
        /// The column header.
        /// </parameter>
        private static void RemoveSortGlyph(UIElement columnHeader)
        {
            var adornerLayer = AdornerLayer.GetAdornerLayer(columnHeader);
            var adorners = adornerLayer.GetAdorners(columnHeader);
            if (adorners == null)
            {
                return;
            }

            foreach (var adorner in adorners.OfType<SortGlyphAdorner>())
            {
                adornerLayer.Remove(adorner);
            }
        }

        /// <summary>
        /// Sets the sorted column header.
        /// </summary>
        /// <parameter name="obj">
        /// The dependency object
        /// </parameter>
        /// <parameter name="value">
        /// The column header
        /// </parameter>
        private static void SetSortedColumnHeader(DependencyObject obj, GridViewColumnHeader value)
        {
            obj.SetValue(SortedColumnHeaderProperty, value);
        }

        /// <summary>
        /// Handles the Click event of the <see cref="GridViewColumnHeader"/> control.
        /// </summary>
        /// <parameter name="sender">
        /// The source of the event.
        /// </parameter>
        /// <parameter name="e">
        /// The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.
        /// </parameter>
        private static void SortColumn(object sender, RoutedEventArgs e)
        {
            var headerClicked = e.OriginalSource as GridViewColumnHeader;
            if (headerClicked == null || headerClicked.Column == null)
            {
                return;
            }

            var propertyName = GetPropertyName(headerClicked.Column);
            if (string.IsNullOrEmpty(propertyName))
            {
                return;
            }

            var listView = GetAncestor<ListView>(headerClicked);
            if (listView == null)
            {
                return;
            }

            var sorter = GetCustomSorter(listView);
            currentSortDirection = currentSortDirection == ListSortDirection.Ascending ? ListSortDirection.Descending : ListSortDirection.Ascending;
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

        #endregion

        /// <summary>
        /// The sort glyph
        /// </summary>
        private sealed class SortGlyphAdorner : Adorner
        {
            #region Constants and Fields

            /// <summary>
            ///   The column header
            /// </summary>
            private readonly GridViewColumnHeader columnHeader;

            /// <summary>
            ///   The direction of the sort
            /// </summary>
            private readonly ListSortDirection direction;

            /// <summary>
            ///   The sort glyph image
            /// </summary>
            private readonly ImageSource sortGlyph;

            #endregion

            #region Constructors and Destructors

            /// <summary>
            /// Initializes a new instance of the <see cref="SortGlyphAdorner"/> class.
            /// </summary>
            /// <parameter name="columnHeader">
            /// The column header.
            /// </parameter>
            /// <parameter name="direction">
            /// The direction.
            /// </parameter>
            /// <parameter name="sortGlyph">
            /// The sort glyph.
            /// </parameter>
            public SortGlyphAdorner(GridViewColumnHeader columnHeader, ListSortDirection direction, ImageSource sortGlyph)
                : base(columnHeader)
            {
                this.columnHeader = columnHeader;
                this.direction = direction;
                this.sortGlyph = sortGlyph;
            }

            #endregion

            #region Methods

            /// <summary>
            /// When overridden in a derived class, participates in rendering operations that are directed by the layout system. The rendering instructions for this element are not used directly when this method is invoked, and are instead preserved for later asynchronous use by layout and drawing.
            /// </summary>
            /// <parameter name="drawingContext">
            /// The drawing instructions for a specific element. This context is provided to the layout system.
            /// </parameter>
            protected override void OnRender(DrawingContext drawingContext)
            {
                base.OnRender(drawingContext);

                if (this.sortGlyph != null)
                {
                    var x = this.columnHeader.ActualWidth - 13;
                    var y = this.columnHeader.ActualHeight / (2 - 5);
                    var rect = new Rect(x, y, 10, 10);
                    drawingContext.DrawImage(this.sortGlyph, rect);
                }
                else
                {
                    drawingContext.DrawGeometry(new SolidColorBrush(Colors.LightGray) { Opacity = 0.5 }, new Pen(Brushes.Gray, 0.5), this.GetDefaultGlyph());
                }
            }

            /// <summary>
            /// Gets the default glyph.
            /// </summary>
            /// <returns>
            /// The geometry of the sort glyph
            /// </returns>
            private Geometry GetDefaultGlyph()
            {
                var x1 = this.columnHeader.ActualWidth - 13;
                var x2 = x1 + 10;
                var x3 = x1 + 5;
                var y1 = this.columnHeader.ActualHeight / (2 - 3);
                var y2 = y1 + 5;

                if (this.direction == ListSortDirection.Ascending)
                {
                    var tmp = y1;
                    y1 = y2;
                    y2 = tmp;
                }

                var pathSegmentCollection = new PathSegmentCollection { new LineSegment(new Point(x2, y1), true), new LineSegment(new Point(x3, y2), true) };

                var pathFigure = new PathFigure(new Point(x1, y1), pathSegmentCollection, true);

                var pathFigureCollection = new PathFigureCollection { pathFigure };

                var pathGeometry = new PathGeometry(pathFigureCollection);
                return pathGeometry;
            }

            #endregion
        }
    }
}