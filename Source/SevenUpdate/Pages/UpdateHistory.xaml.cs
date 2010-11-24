// ***********************************************************************
// <copyright file="UpdateHistory.xaml.cs"
//            project="SevenUpdate"
//            assembly="SevenUpdate"
//            solution="SevenUpdate"
//            company="Seven Software">
//     Copyright (c) Seven Software. All rights reserved.
// </copyright>
// <author username="sevenalive">Robert Baker</author>
// <license href="http://www.gnu.org/licenses/gpl-3.0.txt" name="GNU General Public License 3">
//  This file is part of Seven Update.
//    Seven Update is free software: you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation, either version 3 of the License, or
//    (at your option) any later version.
//    Seven Update is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.
//    You should have received a copy of the GNU General Public License
//    along with Seven Update.  If not, see http://www.gnu.org/licenses/.
// </license>
// ***********************************************************************
namespace SevenUpdate.Pages
{
    using System;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.IO;
    using System.Windows;
    using System.Windows.Data;
    using System.Windows.Input;

    using SevenUpdate.Windows;

    /// <summary>Interaction logic for UpdateHistory.xaml</summary>
    public partial class UpdateHistory
    {
        #region Constants and Fields

        /// <summary>Gets or sets a collection of SUH items</summary>
        private ObservableCollection<Suh> updateHistory;

        #endregion

        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref = "UpdateHistory" /> class.</summary>
        public UpdateHistory()
        {
            this.InitializeComponent();
        }

        #endregion

        #region Methods

        /// <summary>Gets the update history and loads it to the listView</summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void GetHistory(object sender, RoutedEventArgs e)
        {
            if (!File.Exists(App.HistoryFile))
            {
                return;
            }

            this.updateHistory = Utilities.Deserialize<ObservableCollection<Suh>>(App.HistoryFile);
            if (this.updateHistory == null)
            {
                return;
            }

            this.lvUpdateHistory.ItemsSource = this.updateHistory;
            this.updateHistory.CollectionChanged += this.RefreshDataView;
        }


        /// <summary>Goes back to the Main page</summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void NavigateToMainPage(object sender, RoutedEventArgs e)
        {
            MainWindow.NavService.Navigate(new Uri(@"/SevenUpdate;component/Pages/Main.xaml", UriKind.Relative));
        }

        /// <summary>Updates the <see cref="CollectionView"/> when the collection changes</summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Collections.Specialized.NotifyCollectionChangedEventArgs"/> instance containing the event data.</param>
        private void RefreshDataView(object sender, NotifyCollectionChangedEventArgs e)
        {
            // update the view when item change is NOT caused by replacement
            if (e.Action != NotifyCollectionChangedAction.Replace)
            {
                return;
            }

            var dataView = CollectionViewSource.GetDefaultView(this.lvUpdateHistory.ItemsSource);
            dataView.Refresh();
        }

        /// <summary>Shows the selected update details</summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        private void ShowDetails(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount != 2 || this.lvUpdateHistory.SelectedIndex == -1)
            {
                return;
            }

            var details = new UpdateDetails();
            details.ShowDialog(this.updateHistory[this.lvUpdateHistory.SelectedIndex]);
        }

        /// <summary>Shows the selected update details</summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void ShowDetailsDialog(object sender, RoutedEventArgs e)
        {
            var details = new UpdateDetails();
            details.ShowDialog(this.updateHistory[this.lvUpdateHistory.SelectedIndex]);
        }

        #endregion
    }
}