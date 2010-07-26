#region

using System;
using System.Collections;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;

#endregion

namespace Microsoft.Windows.Controls
{
    /// <summary>
    ///   Sorts a <see cref = "ListView" />
    /// </summary>
    public static class ListViewSorter
    {
        /// <summary>
        ///   True if the <see cref = "ListView" /> is sortable, otherwise false
        /// </summary>
        private static readonly DependencyProperty IsListviewSortableProperty = DependencyProperty.RegisterAttached("IsListViewSortable", typeof (Boolean), typeof (ListViewSorter),
                                                                                                                    new FrameworkPropertyMetadata(false,
                                                                                                                                                  new PropertyChangedCallback(OnRegisterSortableGrid)));

        /// <summary>
        ///   A string indicating what to sort the colum by
        /// </summary>
        private static DependencyProperty CustomSorterProperty = DependencyProperty.RegisterAttached("CustomSorter", typeof (IComparer), typeof (ListViewSorter));

        /// <summary>
        ///   Indicates the last direction the <see cref = "GridViewColumn" /> was sorted
        /// </summary>
        private static ListSortDirection lastDirection = ListSortDirection.Ascending;

        /// <summary>
        ///   Indicates the last <see cref = "GridViewColumnHeader" /> clicked
        /// </summary>
        private static GridViewColumnHeader lastHeaderClicked;

        /// <summary>
        ///   The <see cref = "ListView" /> to sort
        /// </summary>
        private static ListView lv;

        /// <summary>
        ///   The Binding to use to sort the GridView column
        /// </summary>
        private static DependencyProperty SortBindingMemberProperty = DependencyProperty.RegisterAttached("SortBindingMember", typeof (BindingBase), typeof (ListViewSorter));

        /// <summary>
        ///   Gets the <see cref = "GridViewColumn" /> sorter
        /// </summary>
        /// <param name = "obj">The <see cref = "DependencyObject" /> to get the sorter from</param>
        /// <returns>an <see cref = "IComparer" /> for CustomSorter</returns>
        private static IComparer GetCustomSorter(DependencyObject obj)
        {
            return (IComparer) obj.GetValue(CustomSorterProperty);
        }

        /// <summary>
        ///   Sets the <see cref = "GridViewColumn" /> sorter
        /// </summary>
        /// <param name = "obj">The <see cref = "DependencyObject" /> to set the sorter to</param>
        /// <param name = "value">the <see cref = "IComparer" /> to set as the sorter</param>
        public static void SetCustomSorter(DependencyObject obj, IComparer value)
        {
            obj.SetValue(CustomSorterProperty, value);
        }

        /// <summary>
        ///   Gets the sorting binding member of the <see cref = "GridViewColumn" />
        /// </summary>
        /// <param name = "obj">The <see cref = "DependencyObject" /> to get the binding member from</param>
        private static BindingBase GetSortBindingMember(DependencyObject obj)
        {
            return (BindingBase) obj.GetValue(SortBindingMemberProperty);
        }

        /// <summary>
        ///   Sets the sorting binding member of the <see cref = "GridViewColumn" />
        /// </summary>
        /// <param name = "obj">The <see cref = "DependencyObject" /> to set the Binding Member to</param>
        /// <param name = "value"></param>
        public static void SetSortBindingMember(DependencyObject obj, BindingBase value)
        {
            obj.SetValue(SortBindingMemberProperty, value);
        }

        /// <summary>
        ///   Sets a value indicating if the <see cref = "ListView" /> is sortable
        /// </summary>
        /// <param name = "obj">The <see cref = "DependencyObject" /> to set the IsListViewSortable property to</param>
        /// <param name = "value"></param>
        public static void SetIsListViewSortable(DependencyObject obj, Boolean value)
        {
            obj.SetValue(IsListviewSortableProperty, value);
        }

        /// <summary>
        ///   Occurs when the <see cref = "ListView" /> registers if it's sortable
        /// </summary>
        private static void OnRegisterSortableGrid(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            var grid = obj as ListView;
            if (grid == null)
                return;
            lv = grid;
            RegisterSortableGridView(grid, args);
        }

        /// <summary>
        ///   Registers the <see cref = "ListView" /> to be sortable
        /// </summary>
        /// <param name = "grid"></param>
        /// <param name = "args"></param>
        private static void RegisterSortableGridView(IInputElement grid, DependencyPropertyChangedEventArgs args)
        {
            if (args.NewValue is Boolean && (Boolean) args.NewValue)
                grid.AddHandler(ButtonBase.ClickEvent, new RoutedEventHandler(GridViewColumnHeader_ClickedHandler));
            else
                grid.AddHandler(ButtonBase.ClickEvent, new RoutedEventHandler(GridViewColumnHeader_ClickedHandler));
        }

        /// <summary>
        ///   Occurs when the <see cref = "GridViewColumnHeader" /> is clicked
        /// </summary>
        private static void GridViewColumnHeader_ClickedHandler(object sender, RoutedEventArgs e)
        {
            var headerClicked = e.OriginalSource as GridViewColumnHeader;

            if (headerClicked == null)
                return;
            ListSortDirection direction;
            if (headerClicked != lastHeaderClicked)
                direction = ListSortDirection.Ascending;

            else
                direction = lastDirection == ListSortDirection.Ascending ? ListSortDirection.Descending : ListSortDirection.Ascending;

            var header = String.Empty;

            try
            {
                header = ((Binding) GetSortBindingMember(headerClicked.Column)).Path.Path;
            }
            catch
            {
            }

            if (header == String.Empty)
                return;

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
            if (tmpTemplate != null)
                headerClicked.Column.HeaderTemplate = tmpTemplate;

            lastHeaderClicked = headerClicked;
            lastDirection = direction;
        }

        /// <summary>
        ///   Sorts a <see cref = "GridViewColumn" />
        /// </summary>
        /// <param name = "sortBy">a string indicating the property to sort the column by</param>
        /// <param name = "direction">the <see cref = "ListSortDirection" /> indicating the sort direction</param>
        private static void Sort(string sortBy, ListSortDirection direction)
        {
            var view = (ListCollectionView) CollectionViewSource.GetDefaultView(lv.ItemsSource);

            if (view == null)
            {
            }
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
                catch
                {
                }
            }
        }
    }
}