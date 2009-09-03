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
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using SevenUpdate.Controls;
using SevenUpdate.WCF;
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
        /// The red side image
        /// </summary>
        private readonly BitmapImage disabledShield = new BitmapImage(new Uri("/Images/ShieldDisabled.png", UriKind.Relative));

        /// <summary>
        ///  The uac shield
        /// </summary>
        private readonly BitmapImage shield = new BitmapImage(new Uri("/Images/Shield.png", UriKind.Relative));

        private ObservableCollection<SUH> hiddenUpdates;

        #endregion

        public RestoreUpdates()
        {
            InitializeComponent();
            if (App.IsAdmin)
                imgShield.Visibility = Visibility.Collapsed;
            listView.AddHandler(Thumb.DragDeltaEvent, new DragDeltaEventHandler(Thumb_DragDelta), true);
        }

        #region Event Declarations

        public static event EventHandler<EventArgs> RestoredHiddenUpdateEventHandler;

        #endregion

        #region Methods

        /// <summary>
        /// Gets the hidden updates and loads them in the listView
        /// </summary>
        private void GetHiddenUpdates()
        {
            hiddenUpdates = Shared.Deserialize<ObservableCollection<SUH>>(Shared.HiddenFile);
            if (hiddenUpdates == null)
                return;
            listView.ItemsSource = hiddenUpdates;
            hiddenUpdates.CollectionChanged += HiddenUpdates_CollectionChanged;
            AddSortBinding();
        }

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

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.NavService.GoBack();
        }

        private void btnRestore_Click(object sender, RoutedEventArgs e)
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

        #region ListView Events

        private void HiddenUpdates_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            // update the view when item change is NOT caused by replacement
            if (e.Action != NotifyCollectionChangedAction.Replace)
                return;
            var dataView = CollectionViewSource.GetDefaultView(listView.ItemsSource);
            dataView.Refresh();
        }

        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            var checkedCount = 0;
            for (var x = 0; x < hiddenUpdates.Count; x++)
            {
                if (hiddenUpdates[x].Status == UpdateStatus.Visible)
                    checkedCount++;
            }

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

        private void Thumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            ListViewExtensions.Thumb_DragDelta(sender, ((Thumb) e.OriginalSource));
        }

        private void MenuItem_MouseClick(object sender, RoutedEventArgs e)
        {
            var details = new UpdateDetails();
            details.ShowDialog(hiddenUpdates[listView.SelectedIndex]);
        }

        private void listView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount != 2 || listView.SelectedIndex == -1)
                return;
            var details = new UpdateDetails();
            details.ShowDialog(hiddenUpdates[listView.SelectedIndex]);
        }

        #endregion

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            GetHiddenUpdates();
        }

        #endregion
    }
}