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
        Collection<UpdateInformation> hist;

        #endregion

        public UpdateHistory()
        {
            InitializeComponent();
        }

        #region Methods

        /// <summary>
        /// Checks if a group exists in the ListView and returns the index
        /// </summary>
        /// <param name="groupName">Name of the group to check</param>
        /// <returns>Returns the index of the group, -1 if not found</returns>
        int CheckGroups(string groupName)
        {
            //for (int x = 0; x < listView.Groups.Count; x++)
            //{
            //    if (listView.Groups[x].Header == groupName)
            //        return x;
            //}
            return -1;
        }

        /// <summary>
        /// Gets the update history and loads it to the listView
        /// </summary>
        void GetHistory()
        {
            listView.Items.Clear();
            ListViewItem item = null;
            //ListViewGroup group = null;
            listView.Items.Clear();
           // listView.Groups.Clear();
            hist = App.GetHistory();
            foreach (UpdateInformation history in hist)
            {
                string status = null;
                string importance = null;
                if (history.Status == UpdateStatus.Failed)
                    status = App.RM.GetString("Failed");
                else if (history.Status == UpdateStatus.Hidden)
                    status = App.RM.GetString("Hidden");
                else if (history.Status == UpdateStatus.Successful)
                    status = App.RM.GetString("Successful");

                if (history.Importance == Importance.Important)
                    importance = App.RM.GetString("Important");
                else if (history.Importance == Importance.Optional)
                    importance = App.RM.GetString("Optional");
                else if (history.Importance == Importance.Recommended)
                    importance = App.RM.GetString("Recommended");
                else if (history.Importance == Importance.Locale)
                    importance = App.RM.GetString("Locale");

                string[] info = new string[] { history.UpdateTitle[0].Value, status, importance, history.InstallDate };
                int index = CheckGroups(history.ApplicationName[0].Value);
                if (index < 0)
                {
                    //group = new ListViewGroup(history.ApplicationName[0].Value);
                    //listView.Groups.Add(group);
                    //item = new ListViewItem(info, group);
                }
                else
                {
                    //item = new ListViewItem(info, listView.Groups[index]);
                }

                listView.Items.Add(item);
            }
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

        private void listView_SelectedIndexChanged(object sender, EventArgs e)
        {
            //if (listView.SelectedItems.Count > 0)
            //{
            //    if (listView.SelectedItems[0].Group != null)
            //    {
            //        int historyIndex = listView.Groups.IndexOf(listView.SelectedItems[0].Group);
            //        sidebar.DisplayUpdateInfo(hist[historyIndex].UpdateTitle[0].Value, hist[historyIndex].ReleaseDate,
            //           hist[historyIndex].Description[0].Value, hist[historyIndex].InfoUrl, hist[historyIndex].HelpUrl);

            //    }
            //}
        }

        #endregion

        private void listView_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (((ListViewItem)e.Source).IsSelected)
            {
                if (listView.SelectedItems.Count == 1)
                {

                    //Show context menu for single item

                    e.Handled = true;

                }
            }
        }
    }
}
