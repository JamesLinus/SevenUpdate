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
using System.Windows.Data;

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
        ObservableCollection<UpdateInformation> history;

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

            history.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(history_CollectionChanged);
            AddSortBinding();
        }



        void AddSortBinding()
        {

            GridView gv = (GridView)listView.View;

            GridViewColumn col = gv.Columns[0];
            Avalon.Windows.Controls.ListViewSorter.SetSortBindingMember(col, new Binding("Name"));

            col = gv.Columns[1];
            Avalon.Windows.Controls.ListViewSorter.SetSortBindingMember(col, new Binding("Status"));

            col = gv.Columns[2];
            Avalon.Windows.Controls.ListViewSorter.SetSortBindingMember(col, new Binding("Importance"));

            col = gv.Columns[3];
            Avalon.Windows.Controls.ListViewSorter.SetSortBindingMember(col, new Binding("DateInstalled"));

            Avalon.Windows.Controls.ListViewSorter.SetCustomSorter(listView, new SevenUpdate.ListViewExtensions.UpdateInformationSorter());
        }

        #endregion

        #region UI Events

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            GetHistory();
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            SevenUpdate.Windows.MainWindow.ns.GoBack();
        }

        #region ListView

        void history_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            ListViewExtensions.OnCollectionChanged(e.Action, listView.ItemsSource);
        }

        void cmsMenu_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (listView.SelectedIndex == -1)
                e.Cancel = true;
        }

        private void listView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2 && listView.SelectedIndex != -1)
            {
                SevenUpdate.Windows.UpdateDetails details = new SevenUpdate.Windows.UpdateDetails();
                details.ShowDialog(history[listView.SelectedIndex]);
            }
        }

        private void MenuItem_MouseClick(object sender, RoutedEventArgs e)
        {
            SevenUpdate.Windows.UpdateDetails details = new SevenUpdate.Windows.UpdateDetails();
            details.ShowDialog(history[listView.SelectedIndex]);
        }

        #endregion

        #endregion
    }
}
