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
using System.Windows.Controls;
using System;
using System.Collections.ObjectModel;
using SevenUpdate.WCF;
using System.Windows.Controls.Primitives;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace SevenUpdate.Pages
{
    /// <summary>
    /// Interaction logic for Update_History.xaml
    /// </summary>
    public partial class RestoreUpdates : Page
    {
        #region Global Vars

        ObservableCollection<UpdateInformation> hiddenUpdates;

        #endregion

        public RestoreUpdates()
        {
            InitializeComponent();
            if (App.IsAdmin)
                btnRestore.Content = App.RM.GetString("Restore");
            listView.AddHandler(Thumb.DragDeltaEvent, new DragDeltaEventHandler(Thumb_DragDelta), true);
        }

        #region Event Declarations

        public static event EventHandler<EventArgs> RestoredHiddenUpdateEventHandler;

        #endregion

        #region Methods

        /// <summary>
        /// Gets the hidden updates and loads them in the listView
        /// </summary>
        void GetHiddenUpdates()
        {
            hiddenUpdates = App.GetHiddenUpdates();

            listView.ItemsSource = hiddenUpdates;
            hiddenUpdates.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(HiddenUpdates_CollectionChanged);
            AddSortBinding();
        }



        void AddSortBinding()
        {

            GridView gv = (GridView)listView.View;

            GridViewColumn col = gv.Columns[1];
            Avalon.Windows.Controls.ListViewSorter.SetSortBindingMember(col, new Binding("Name"));

            col = gv.Columns[2];
            Avalon.Windows.Controls.ListViewSorter.SetSortBindingMember(col, new Binding("Importance"));

            col = gv.Columns[3];
            Avalon.Windows.Controls.ListViewSorter.SetSortBindingMember(col, new Binding("Size"));

            Avalon.Windows.Controls.ListViewSorter.SetCustomSorter(listView, new SevenUpdate.ListViewExtensions.UpdateInformationSorter());
        }


        #endregion

        #region UI Events

        #region Buttons

        private void btnCancel_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            SevenUpdate.Windows.MainWindow.ns.GoBack();
        }

        private void btnRestore_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            for (int x = 0; x < hiddenUpdates.Count; x++)
            {
                if (hiddenUpdates[x].Status == UpdateStatus.Visible)
                {
                    hiddenUpdates.RemoveAt(x);
                    x--;
                }
            }
            if (Admin.HideUpdates(hiddenUpdates))
            {
                if (RestoredHiddenUpdateEventHandler != null)
                    RestoredHiddenUpdateEventHandler(this, new EventArgs());
            }
            SevenUpdate.Windows.MainWindow.ns.GoBack();
        }

        #endregion

        #region ListView Events

        void HiddenUpdates_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            ListViewExtensions.OnCollectionChanged(e.Action, listView.ItemsSource);
        }

        void cmsMenu_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (listView.SelectedIndex == -1)
                e.Cancel = true;
        }

        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            int checkedCount = 0;
            for (int x = 0; x < hiddenUpdates.Count; x++)
            {
                if (hiddenUpdates[x].Status == UpdateStatus.Visible)
                {
                    checkedCount++;
                }
            }

            if (checkedCount > 0)
            {
                tbSelected.Text = App.RM.GetString("TotalSelected") + " " + checkedCount + " " + App.RM.GetString("Updates");
                btnRestore.IsEnabled = true;
            }
            else
                if (checkedCount == 1)
                {
                    tbSelected.Text = App.RM.GetString("TotalSelected") + " 1 " + App.RM.GetString("Update");
                    btnRestore.IsEnabled = true;
                }
                else
                {
                    tbSelected.Text = App.RM.GetString("TotalSelected") + " 0 " + App.RM.GetString("Updates");
                    btnRestore.IsEnabled = false;
                }
        }

        private void Thumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            ListViewExtensions.Thumb_DragDelta(sender, ((Thumb)e.OriginalSource));
        }

        private void MenuItem_MouseClick(object sender, RoutedEventArgs e)
        {
            SevenUpdate.Windows.UpdateDetails details = new SevenUpdate.Windows.UpdateDetails();
            details.ShowDialog(hiddenUpdates[listView.SelectedIndex]);
        }

        private void listView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2 && listView.SelectedIndex != -1)
            {
                SevenUpdate.Windows.UpdateDetails details = new SevenUpdate.Windows.UpdateDetails();
                details.ShowDialog(hiddenUpdates[listView.SelectedIndex]);
            }
        }

        #endregion

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            GetHiddenUpdates();
        }

        #endregion
    }
}
