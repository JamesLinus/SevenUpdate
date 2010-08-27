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
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using Microsoft.Windows.Controls;

using SevenUpdate.Windows;

#endregion

namespace SevenUpdate.Pages
{
    /// <summary>
    ///   Interaction logic for Update_History.xaml
    /// </summary>
    public sealed partial class RestoreUpdates : Page
    {
        #region Global Vars

        /// <summary>
        ///   Gets or Sets a collection of SUH items
        /// </summary>
        private ObservableCollection<Suh> hiddenUpdates;

        #endregion

        /// <summary>
        ///   The constructor for Restore Updates page
        /// </summary>
        public RestoreUpdates()
        {
            InitializeComponent();
            lvHiddenUpdates.AddHandler(Thumb.DragDeltaEvent, new DragDeltaEventHandler(Thumb_DragDelta), true);
        }

        #region Event Declarations

        /// <summary>
        ///   Occurs when one or more hidden updates have been restored
        /// </summary>
        public static event EventHandler<EventArgs> RestoredHiddenUpdateEventHandler;

        #endregion

        #region Methods

        /// <summary>
        ///   Gets the hidden updates and loads them in thelvHiddenUpdates
        /// </summary>
        private void GetHiddenUpdates()
        {
            hiddenUpdates = Base.Deserialize<ObservableCollection<Suh>>(Base.HiddenFile);
            if (hiddenUpdates == null)
                return;
            lvHiddenUpdates.ItemsSource = hiddenUpdates;
            hiddenUpdates.CollectionChanged += HiddenUpdates_CollectionChanged;
            AddSortBinding();
        }

        /// <summary>
        ///   Adds the <see cref = "GridViewColumn" />'s of the <see cref = "ListView" /> to be sorted
        /// </summary>
        private void AddSortBinding()
        {
            var gv = (GridView) lvHiddenUpdates.View;

            var col = gv.Columns[1];
            ListViewSorter.SetSortBindingMember(col, new Binding("Name"));

            col = gv.Columns[2];
            ListViewSorter.SetSortBindingMember(col, new Binding("Importance"));

            col = gv.Columns[3];
            ListViewSorter.SetSortBindingMember(col, new Binding("Size"));

            ListViewSorter.SetCustomSorter(lvHiddenUpdates, new CustomComparers.SuhSorter());
        }

        #endregion

        #region UI Events

        #region Buttons

        /// <summary>
        ///   Unhides one or more updates and navigates to the Main page
        /// </summary>
        private void Restore_Click(object sender, RoutedEventArgs e)
        {
            for (var x = 0; x < hiddenUpdates.Count; x++)
            {
                if (hiddenUpdates[x].Status != UpdateStatus.Visible)
                    continue;
                hiddenUpdates.RemoveAt(x);
                x--;
            }
            if (AdminClient.HideUpdates(hiddenUpdates))
            {
                if (RestoredHiddenUpdateEventHandler != null)
                    RestoredHiddenUpdateEventHandler(this, new EventArgs());
            }
            MainWindow.NavService.GoBack();
        }

        #endregion

        #region ListView Related

        /// <summary>
        ///   Updates the <see cref = "CollectionView" /> when the <c>hiddenUpdates</c> collection changes
        /// </summary>
        /// <param name = "sender" />
        /// <param name = "e" />
        private void HiddenUpdates_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            // update the view when item change is NOT caused by replacement
            if (e.Action != NotifyCollectionChangedAction.Replace)
                return;
            var dataView = CollectionViewSource.GetDefaultView(lvHiddenUpdates.ItemsSource);
            dataView.Refresh();
        }

        /// <summary>
        ///   Updates the UI when an update check box is clicked
        /// </summary>
        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            var checkedCount = hiddenUpdates.Count(t => t.Status == UpdateStatus.Visible);

            lblSelectedUpdates.Text = App.RM.GetString("TotalSelected") + " " + checkedCount + " ";
            if (checkedCount > 0)
            {
                lblSelectedUpdates.Text += checkedCount == 1 ? App.RM.GetString("Updates") : App.RM.GetString("Update");
                btnRestore.IsEnabled = true;
            }
            else
            {
                lblSelectedUpdates.Text += App.RM.GetString("Updates");
                btnRestore.IsEnabled = false;
            }
        }

        /// <summary>
        ///   Limit the size of the <see cref = "GridViewColumn" /> when it's being resized
        /// </summary>
        private void Thumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            ListViewExtensions.LimitColumnSize(((Thumb) e.OriginalSource));
        }

        /// <summary>
        ///   Shows the selected update details
        /// </summary>
        private void MenuItem_MouseClick(object sender, RoutedEventArgs e)
        {
            var details = new UpdateDetails();
            details.ShowDialog(hiddenUpdates[lvHiddenUpdates.SelectedIndex]);
        }

        /// <summary>
        ///   Shows the selected update details
        /// </summary>
        private void ListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount != 2 || lvHiddenUpdates.SelectedIndex == -1)
                return;
            var details = new UpdateDetails();
            details.ShowDialog(hiddenUpdates[lvHiddenUpdates.SelectedIndex]);
        }

        #endregion

        /// <summary>
        ///   Loads the collection of hidden updates when the page is loaded
        /// </summary>
        /// <param name = "sender" />
        /// <param name = "e" />
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            GetHiddenUpdates();
        }

        #endregion
    }
}