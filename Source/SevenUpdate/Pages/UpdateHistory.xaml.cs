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

using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
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
    public partial class UpdateHistory : Page
    {
        #region Fields

        /// <summary>
        ///   The location of the update history file
        /// </summary>
        private static readonly string HistoryFile = Base.AllUserStore + "History.suh";

        /// <summary>
        ///   Gets or Sets a collection of SUH items
        /// </summary>
        private ObservableCollection<Suh> updateHistory;

        #endregion

        /// <summary>
        ///   The constructor for the Update History page
        /// </summary>
        public UpdateHistory()
        {
            InitializeComponent();
        }

        #region Methods

        /// <summary>
        ///   Gets the update history and loads it to the listView
        /// </summary>
        private void GetHistory()
        {
            updateHistory = Base.Deserialize<ObservableCollection<Suh>>(HistoryFile);
            if (updateHistory == null)
                return;

            lvUpdateHistory.ItemsSource = updateHistory;
            updateHistory.CollectionChanged += History_CollectionChanged;
        }

        #endregion

        #region UI Events

        /// <summary>
        ///   Loads the update history when the page is loaded
        /// </summary>
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            GetHistory();
        }

        #region ListView Related

        /// <summary>
        ///   Updates the <see cref = "CollectionView" /> when the <c>updateHistory</c> collection changes
        /// </summary>
        private void History_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            // update the view when item change is NOT caused by replacement
            if (e.Action != NotifyCollectionChangedAction.Replace)
                return;
            var dataView = CollectionViewSource.GetDefaultView(lvUpdateHistory.ItemsSource);
            dataView.Refresh();
        }

        /// <summary>
        ///   Shows the selected update details
        /// </summary>
        private void ListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount != 2 || lvUpdateHistory.SelectedIndex == -1)
                return;
            var details = new UpdateDetails();
            details.ShowDialog(updateHistory[lvUpdateHistory.SelectedIndex]);
        }

        /// <summary>
        ///   Shows the selected update details
        /// </summary>
        private void MenuItem_MouseClick(object sender, RoutedEventArgs e)
        {
            var details = new UpdateDetails();
            details.ShowDialog(updateHistory[lvUpdateHistory.SelectedIndex]);
        }

        #endregion

        #endregion
    }
}