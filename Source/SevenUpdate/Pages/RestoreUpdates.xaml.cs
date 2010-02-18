#region GNU Public License v3

// Copyright 2007-2010 Robert Baker, Seven Software.
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
//    along with Seven Update.  If not, see <http://www.gnu.org/licenses/>.

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
using System.Windows.Media.Imaging;
using SevenUpdate.Base;
using SevenUpdate.Controls;
using SevenUpdate.Windows;

#endregion

namespace SevenUpdate.Pages
{
    /// <summary>
    /// Interaction logic for Update_History.xaml
    /// </summary>
    public sealed partial class RestoreUpdates : Page
    {
        #region Global Vars

        /// <summary>
        /// The disabled shield image
        /// </summary>
        private readonly BitmapImage disabledShield = new BitmapImage(new Uri("/Images/ShieldDisabled.png", UriKind.Relative));

        /// <summary>The UAC shield</summary>
        private readonly BitmapImage shield = new BitmapImage(new Uri("/Images/Shield.png", UriKind.Relative));

        /// <summary>
        /// Gets or Sets a collection of SUH items
        /// </summary>
        private ObservableCollection<SUH> hiddenUpdates;

        #endregion

        /// <summary>
        /// The constructor for Restore Updates page
        /// </summary>
        public RestoreUpdates()
        {
            InitializeComponent();
            if (App.IsAdmin)
                imgShield.Visibility = Visibility.Collapsed;
            listView.AddHandler(Thumb.DragDeltaEvent, new DragDeltaEventHandler(Thumb_DragDelta), true);
        }

        #region Event Declarations

        /// <summary>
        /// Occurs when one or more hidden updates have been restored
        /// </summary>
        public static event EventHandler<EventArgs> RestoredHiddenUpdateEventHandler;

        #endregion

        #region Methods

        /// <summary>
        /// Gets the hidden updates and loads them in the listView
        /// </summary>
        private void GetHiddenUpdates()
        {
            hiddenUpdates = Base.Base.Deserialize<ObservableCollection<SUH>>(Base.Base.HiddenFile);
            if (hiddenUpdates == null)
                return;
            listView.ItemsSource = hiddenUpdates;
            hiddenUpdates.CollectionChanged += HiddenUpdates_CollectionChanged;
            AddSortBinding();
        }

        /// <summary>
        /// Adds the <see cref="GridViewColumn" />'s of the <see cref="ListView" /> to be sorted
        /// </summary>
        private void AddSortBinding()
        {
            var gv = (GridView) listView.View;

            var col = gv.Columns[1];
            ListViewSorter.SetSortBindingMember(col, new Binding("Name"));

            col = gv.Columns[2];
            ListViewSorter.SetSortBindingMember(col, new Binding("Importance"));

            col = gv.Columns[3];
            ListViewSorter.SetSortBindingMember(col, new Binding("Size"));

            ListViewSorter.SetCustomSorter(listView, new ListViewExtensions.SUHSorter());
        }

        #endregion

        #region UI Events

        #region Buttons

        /// <summary>
        /// Navigates to the Main page
        /// </summary>
        private void CancelClick(object sender, RoutedEventArgs e)
        {
            MainWindow.NavService.GoBack();
        }

        /// <summary>
        /// Unhides one or more updates and navigates to the Main page
        /// </summary>
        private void RestoreClick(object sender, RoutedEventArgs e)
        {
            for (var x = 0; x < hiddenUpdates.Count; x++)
            {
                if (hiddenUpdates[x].Status != UpdateStatus.Visible)
                    continue;
                hiddenUpdates.RemoveAt(x);
                x--;
            }
            if (Admin.HideUpdates(hiddenUpdates))
            {
                if (RestoredHiddenUpdateEventHandler != null)
                    RestoredHiddenUpdateEventHandler(this, new EventArgs());
            }
            MainWindow.NavService.GoBack();
        }

        #endregion

        #region ListView Related

        /// <summary>
        /// Updates the <see cref="CollectionView" /> when the <c>hiddenUpdates</c> collection changes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HiddenUpdates_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            // update the view when item change is NOT caused by replacement
            if (e.Action != NotifyCollectionChangedAction.Replace)
                return;
            var dataView = CollectionViewSource.GetDefaultView(listView.ItemsSource);
            dataView.Refresh();
        }

        /// <summary>
        /// Updates the UI when an update check box is clicked
        /// </summary>
        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            var checkedCount = hiddenUpdates.Count(t => t.Status == UpdateStatus.Visible);

            tbSelected.Text = App.RM.GetString("TotalSelected") + " " + checkedCount + " ";
            if (checkedCount > 0)
            {
                tbSelected.Text += checkedCount == 1 ? App.RM.GetString("Updates") : App.RM.GetString("Update");
                btnRestore.IsEnabled = true;
                imgShield.Source = shield;
            }
            else
            {
                tbSelected.Text += App.RM.GetString("Updates");
                btnRestore.IsEnabled = false;
                imgShield.Source = disabledShield;
            }
        }

        /// <summary>
        /// Limit the size of the <see cref="GridViewColumn" /> when it's being resized
        /// </summary>
        private void Thumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            ListViewExtensions.LimitColumnSize(((Thumb) e.OriginalSource));
        }

        /// <summary>
        /// Shows the selected update details
        /// </summary>
        private void MenuItem_MouseClick(object sender, RoutedEventArgs e)
        {
            var details = new UpdateDetails();
            details.ShowDialog(hiddenUpdates[listView.SelectedIndex]);
        }

        /// <summary>
        /// Shows the selected update details
        /// </summary>
        private void ListViewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount != 2 || listView.SelectedIndex == -1)
                return;
            var details = new UpdateDetails();
            details.ShowDialog(hiddenUpdates[listView.SelectedIndex]);
        }

        #endregion

        /// <summary>
        /// Loads the collection of hidden updates when the page is loaded
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            GetHiddenUpdates();
        }

        #endregion
    }
}