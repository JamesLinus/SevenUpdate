/*Copyright 2007-09 Robert Baker, aka Seven ALive.
This file is part of Seven Update.

    Seven Update is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    Seven Update is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with Seven Update.  If not, see <http://www.gnu.org/licenses/>.*/
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SevenUpdate.Pages
{
    /// <summary>
    /// Interaction logic for Update_History.xaml
    /// </summary>
    public partial class UpdateHistory : Page
    {
        #region Global Vars

        /// <summary>
        /// The list of history items
        /// </summary>
        Collection<UpdateInformation> history;

        #endregion

        public UpdateHistory()
        {
            InitializeComponent();
        }

        #region Methods

        /// <summary>
        /// Gets the update history and loads it to the listView
        /// </summary>
        void GetHistory()
        {
            history = App.GetHistory();
            listView.ItemsSource = history;


        }

        #endregion

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            GetHistory();
            Main.LastPageVisited = "UpdateHistory";
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            SevenUpdate.Windows.MainWindow.ns.GoBack();
        }

        #region Context Menu

        void cmsMenu_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (listView.SelectedItems.Count == 0)
                e.Cancel = true;
        }

        #endregion

        #region ListView

        #endregion

        private void listView_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            SevenUpdate.Windows.UpdateDetails details = new SevenUpdate.Windows.UpdateDetails();
            details.ShowDialog(history[listView.SelectedIndex]);
        }
    }
}
