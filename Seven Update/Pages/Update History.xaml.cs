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

using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using SevenUpdate.Controls;
using SevenUpdate.Windows;

#endregion

namespace SevenUpdate.Pages
{
    /// <summary>
    /// Interaction logic for Update_History.xaml
    /// </summary>
    public partial class UpdateHistory : Page
    {
        #region Global Vars

        /// <summary>
        /// The location of the update history file
        /// </summary>
        public static readonly string HistoryFile = Shared.AllUserStore + "History.suh";

        /// <summary>
        /// The list of history items
        /// </summary>
        private ObservableCollection<SUH> history;

        #endregion

        public UpdateHistory()
        {
            InitializeComponent();
        }

        #region Methods

        /// <summary>
        /// Gets the update history and loads it to the listView
        /// </summary>
        private void GetHistory()
        {
            history = Shared.Deserialize<ObservableCollection<SUH>>(HistoryFile);
            if (history == null)
                return;

            listView.ItemsSource = history;
            history.CollectionChanged += History_CollectionChanged;
            AddSortBinding();
        }

        private void AddSortBinding()
        {
            var gv = (GridView) listView.View;

            var col = gv.Columns[0];
            ListViewSorter.SetSortBindingMember(col, new Binding("Name"));

            col = gv.Columns[1];
            ListViewSorter.SetSortBindingMember(col, new Binding("Status"));

            col = gv.Columns[2];
            ListViewSorter.SetSortBindingMember(col, new Binding("Importance"));

            col = gv.Columns[3];
            ListViewSorter.SetSortBindingMember(col, new Binding("DateInstalled"));

            ListViewSorter.SetCustomSorter(listView, new ListViewExtensions.SUHSorter());
        }

        #endregion

        #region UI Events

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            GetHistory();
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.NavService.GoBack();
        }

        #region ListView

        private void History_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            // update the view when item change is NOT caused by replacement
            if (e.Action != NotifyCollectionChangedAction.Replace)
                return;
            var dataView = CollectionViewSource.GetDefaultView(listView.ItemsSource);
            dataView.Refresh();
        }

        private void listView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount != 2 || listView.SelectedIndex == -1)
                return;
            var details = new UpdateDetails();
            details.ShowDialog(history[listView.SelectedIndex]);
        }

        private void MenuItem_MouseClick(object sender, RoutedEventArgs e)
        {
            var details = new UpdateDetails();
            details.ShowDialog(history[listView.SelectedIndex]);
        }

        #endregion

        #endregion
    }
}