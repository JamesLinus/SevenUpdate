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
using System.Windows.Data;
using System.Windows.Input;

namespace SevenUpdate.Pages
{
    /// <summary>
    /// Interaction logic for Update_Info.xaml
    /// </summary>
    public partial class UpdateInfo : Page
    {
        #region Global Vars

        ListViewItem lvi;

        ObservableCollection<Application> selectedApps;

        #endregion

        #region Event Declarations

        internal static event EventHandler<UpdateSelectionChangedEventArgs> UpdateSelectionChangedEventHandler;

        #region EventArgs

        internal class UpdateSelectionChangedEventArgs : EventArgs
        {
            public UpdateSelectionChangedEventArgs(int importantUpdates, int optionalUpdates, ulong importantDownloadSize, ulong optionalDownloadSize)
            {
                this.ImportantUpdates = importantUpdates;

                this.OptionalUpdates = optionalUpdates;

                this.ImportantDownloadSize = importantDownloadSize;

                this.OptionalDownloadSize = optionalDownloadSize;
            }

            /// <summary>
            /// Number of Important Updates selected
            /// </summary>
            internal int ImportantUpdates { get; set; }

            /// <summary>
            /// Number of Optional Updates selected
            /// </summary>
            internal int OptionalUpdates { get; set; }

            /// <summary>
            /// The total download size in bytes of the important updates
            /// </summary>
            internal ulong ImportantDownloadSize { get; set; }

            /// <summary>
            /// The total download size in bytes of the optional updates
            /// </summary>
            internal ulong OptionalDownloadSize { get; set; }
        }

        #endregion

        #endregion

        public UpdateInfo()
        {
            InitializeComponent();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            SevenUpdate.Windows.MainWindow.ns.GoBack();
        }

        private void btnInstall_Click(object sender, RoutedEventArgs e)
        {
            App.Applications = selectedApps;
            SevenUpdate.Windows.MainWindow.ns.GoBack();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            Main.LastPageVisited = "UpdateInfo";
            AddUpdates();
        }

        private void tbUrlInfo_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void tbUrlHelp_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void MenuItem_MouseClick(object sender, RoutedEventArgs e)
        {
            //int appIndex, updateIndex;

            //UpdateInformation hnh = new UpdateInformation();

            //hnh.ApplicationName = App.Applications[appIndex].Name;

            //hnh.HelpUrl = App.Applications[appIndex].HelpUrl;

            //hnh.InfoUrl = App.Applications[appIndex].Updates[updateIndex].InfoUrl;

            //hnh.Publisher = App.Applications[appIndex].Publisher;

            //hnh.PublisherUrl = App.Applications[appIndex].PublisherUrl;

            //hnh.ReleaseDate = App.Applications[appIndex].Updates[updateIndex].ReleaseDate;

            //hnh.Status = UpdateStatus.Hidden;

            //hnh.Importance = App.Applications[appIndex].Updates[updateIndex].Importance;

            //hnh.Description = App.Applications[appIndex].Updates[updateIndex].Description;

            //hnh.UpdateTitle = App.Applications[appIndex].Updates[updateIndex].Title;

            //if (cmiHideUpdate.Header.ToString() == App.RM.GetString("HideUpdate"))
            //{
            //    if (Client.HideUpdate(hnh))
            //    {
            //        cmiHideUpdate.Header = App.RM.GetString("ShowUpdate");
            //        lvi.IsEnabled = false;
                    
            //    }

            //}
            //else
            //{
            //    if (Client.ShowUpdate(hnh))
            //    {
            //        cmiHideUpdate.Header = App.RM.GetString("HideUpdate");
            //        lvi.IsEnabled = true;

            //    }
            //}
        }

        #region Methods

        /// <summary>
        /// Adds the updates to the list
        /// </summary>
        void AddUpdates()
        {
            selectedApps = App.Applications;
            Binding items = new Binding();
            items.Source = selectedApps;
            listView.SetBinding(ItemsControl.ItemsSourceProperty, items);
        }

        void LoadUpdateInfo()
        {
            

        }
        /// <summary>
        /// Shows the update information in the sidebar
        /// </summary>
        void ShowLabels()
        {
            tbPublishedDate.Visibility = Visibility.Visible;
            tbPublishedLabel.Visibility = Visibility.Visible;
            tbUpdateDescription.Visibility = Visibility.Visible;
            tbUpdateTitle.Visibility = Visibility.Visible;
            tbUrlHelp.Visibility = Visibility.Visible;
            tbUrlInfo.Visibility = Visibility.Visible;
            imgIcon.Visibility = Visibility.Hidden;
        }

        /// <summary>
        /// Hides the update information in the sidebar
        /// </summary>
        void HideLabels()
        {
            tbPublishedDate.Visibility = Visibility.Hidden;
            tbPublishedLabel.Visibility = Visibility.Hidden;
            tbUpdateDescription.Visibility = Visibility.Hidden;
            tbUpdateTitle.Visibility = Visibility.Hidden;
            tbUrlHelp.Visibility = Visibility.Hidden;
            tbUrlInfo.Visibility = Visibility.Hidden;
            imgIcon.Visibility = Visibility.Visible;
        }

        private void listView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            lvi = (ListViewItem)listView.ItemContainerGenerator.ContainerFromItem(listView.SelectedItem);
            Application app = listView.SelectedItem as Application;


            if (listView.SelectedIndex == -1)
                HideLabels();
            else 
                ShowLabels();

        }

    //    #region Methods

    //    /// <summary>
    //    /// Adds the updates to the list
    //    /// </summary>
    //    void AddUpdates()
    //    {
    //        ListViewItem item = null;

    //        ListViewGroup group = null;

    //        listView.Items.Clear();

    //        listView.Groups.Clear();

    //        foreach (Application app in Program.Applications)
    //        {
    //            foreach (Update update in app.Updates)
    //            {
    //                int index = CheckGroups(app.Name[0].Value);

    //                if (index < 0)
    //                {
    //                    group = new ListViewGroup(app.Name[0].Value);

    //                    listView.Groups.Add(group);

    //                    item = new ListViewItem(group);
    //                }
    //                else
    //                    item = new ListViewItem(listView.Groups[index]);

    //                item.SubItems.Add(update.Title[0].Value);
    //                if (update.Importance == Importance.Important)
    //                    item.SubItems.Add(Program.RM.GetString("Important"));
    //                else if (update.Importance == Importance.Optional)
    //                    item.SubItems.Add(Program.RM.GetString("Optional"));
    //                else if (update.Importance == Importance.Recommended)
    //                    item.SubItems.Add(Program.RM.GetString("Recommended"));
    //                else if (update.Importance == Importance.Locale)
    //                    item.SubItems.Add(Program.RM.GetString("Locale"));

    //                item.SubItems.Add(Shared.ConvertFileSize(Program.GetUpdateSize(update, app.Name[0].Value, app.Directory, app.Is64Bit, false)));

    //                ulong size = Program.GetUpdateSize(Program.Applications[appIndex].Updates[updateIndex], Program.Applications[appIndex].Name[0].Value, Program.Applications[appIndex].Directory,
    //Program.Applications[appIndex].Is64Bit, true);

    //                if (size > 0)
    //                    item.SubItems.Add(Program.RM.GetString("ReadyToDownload"));
    //                else
    //                    item.SubItems.Add(Program.RM.GetString("ReadyToInstall"));

    //                if (app.Name[0].Value == Program.RM.GetString("SevenUpdate"))
    //                    item.Checked = true;

    //                if (update.Importance == Importance.Locale || update.Importance == Importance.Optional)
    //                    item.Checked = false;

    //                listView.Items.Add(item);
    //            }
    //        }

    //        listView.Items[0].Selected = true;
    //    }

    //    void GetListCount()
    //    {
    //        int[] count = new int[2];
    //        ulong[] fileSize = new ulong[2];
    //        int a, b;

    //        for (int x = 0; x < listView.CheckedItems.Count; x++)
    //        {
    //            a = listView.Groups.IndexOf(listView.CheckedItems[x].Group);

    //            b = listView.CheckedItems[x].Group.Items.IndexOf(listView.CheckedItems[x]);


    //            if (listView.CheckedItems[x].SubItems[2].Text == Program.RM.GetString("Important"))
    //            {
    //                count[0]++;

    //                fileSize[0] += Program.GetUpdateSize(Program.Applications[a].Updates[b], Program.Applications[a].Name[0].Value, Program.Applications[a].Directory, Program.Applications[a].Is64Bit, true);
    //            }

    //            if (listView.CheckedItems[x].SubItems[2].Text == Program.RM.GetString("Recommended"))
    //                if (App.Settings.Recommended)
    //                {
    //                    count[0]++;

    //                    fileSize[0] += Program.GetUpdateSize(Program.Applications[a].Updates[b], Program.Applications[a].Name[0].Value, Program.Applications[a].Directory, Program.Applications[a].Is64Bit, true);
    //                }
    //                else
    //                {
    //                    count[1]++;

    //                    fileSize[1] += Program.GetUpdateSize(Program.Applications[a].Updates[b], Program.Applications[a].Name[0].Value, Program.Applications[a].Directory, Program.Applications[a].Is64Bit, true);
    //                }

    //            if (listView.CheckedItems[x].SubItems[2].Text == Program.RM.GetString("Optional") || listView.CheckedItems[x].SubItems[2].Text == Program.RM.GetString("Locale"))
    //            {
    //                count[1]++;

    //                fileSize[1] += Program.GetUpdateSize(Program.Applications[a].Updates[b], Program.Applications[a].Name[0].Value, Program.Applications[a].Directory, Program.Applications[a].Is64Bit, true);
    //            }

    //        }

    //        if (UpdateSelectionChangedEventHandler != null)
    //            UpdateSelectionChangedEventHandler(this, new UpdateSelectionChangedEventArgs(count[0], count[1], fileSize[0], fileSize[1]));
    //    }

    //    #endregion

    //    #region UI Events

    //    void AppUpdate_Load(object sender, EventArgs e)
    //    {
    //        AddUpdates();
    //    }

    //    #region Buttons

    //    void btnCancel_Click(object sender, EventArgs e)
    //    {
    //        Visible = false;
    //        SendToBack();
    //    }

    //    void btnOK_Click(object sender, EventArgs e)
    //    {
    //        GetListCount();
    //        Visible = false;
    //        SendToBack();
    //    }

    //    #endregion


    //    #region Context Menu

    //    /// <summary>
    //    /// Hides an update
    //    /// </summary>
    //    /// <param name="sender"></param>
    //    /// <param name="e"></param>
    //    void cmsHideUpdate_Click(object sender, EventArgs e)
    //    {

    //    }

    //    void cmsMenu_Opening(object sender, System.ComponentModel.CancelEventArgs e)
    //    {
    //        if (listView.SelectedItems.Count == 0)
    //            e.Cancel = true;
    //        else
    //            listIndex = listView.SelectedItems[0].Index;

    //    }

    //    #endregion

    //    #region ListView

    //    /// <summary>
    //    /// Sets the checkbox state and also grabs the 'Selected Updates' Count
    //    /// </summary>
    //    void listView_ItemChecked(object sender, ItemCheckedEventArgs e)
    //    {
    //        chkSelectAll.Enabled = true;

    //        if (listView.CheckedItems.Count > 0)
    //            lblSelected.Text = Program.RM.GetString("TotalSelected") + " " + listView.CheckedItems.Count + " " + Program.RM.GetString("Updates");
    //        else
    //            if (listView.CheckedItems.Count == 1)
    //                lblSelected.Text = Program.RM.GetString("TotalSelected") + " 0 " + Program.RM.GetString("Update");
    //            else
    //                lblSelected.Text = Program.RM.GetString("TotalSelected") + " 0 " + Program.RM.GetString("Updates");

    //        if (listView.Items.Count == listView.CheckedItems.Count)
    //            chkSelectAll.CheckState = CheckState.Checked;
    //        else
    //            if (listView.CheckedItems.Count == 0)
    //                chkSelectAll.CheckState = CheckState.Unchecked;
    //            else
    //                chkSelectAll.CheckState = CheckState.Indeterminate;

    //        if (listView.CheckedItems.Count == 0)
    //        {
    //            int hidden = 0;

    //            for (int x = 0; x < listView.Items.Count; x++)
    //            {
    //                if (listView.Items[x] != null)
    //                    if (listView.Items[x].ForeColor == SystemColors.GrayText)
    //                        hidden++;
    //            }

    //            if (hidden == listView.Items.Count)
    //            {
    //                chkSelectAll.Enabled = false;
    //            }

    //            chkSelectAll.CheckState = CheckState.Unchecked;

    //        }

    //    }

    //    /// <summary>
    //    /// If any item is not checked it will uncheck the 'Select all' checkbox
    //    /// </summary>
    //    void listView_ItemCheck(object sender, ItemCheckEventArgs e)
    //    {
    //        if (listView.Items[e.Index].ForeColor != SystemColors.WindowText)
    //            e.NewValue = CheckState.Unchecked;
    //    }

    //    /// <summary>
    //    /// Switches the Context menu text
    //    /// </summary>
    //    void listView_SelectedIndexChanged(object sender, EventArgs e)
    //    {
    //        if (listView.SelectedItems.Count > 0)
    //        {
    //            if (listView.SelectedItems[0].ForeColor == SystemColors.GrayText)
    //                cmsHideUpdate.Text = Program.RM.GetString("ShowUpdate");
    //            else
    //                cmsHideUpdate.Text = Program.RM.GetString("HideUpdate");

    //            if (listView.SelectedItems[0].Group != null)
    //            {
    //                updateIndex = listView.SelectedItems[0].Group.Items.IndexOf(listView.SelectedItems[0]);

    //                appIndex = listView.Groups.IndexOf(listView.SelectedItems[0].Group);

    //                sidebar.DisplayUpdateInfo(Program.Applications[appIndex].Updates[updateIndex].Title[0].Value, Program.Applications[appIndex].Updates[updateIndex].ReleaseDate,
    //                Program.Applications[appIndex].Updates[updateIndex].Description[0].Value, Program.Applications[appIndex].Updates[updateIndex].InfoUrl, Program.Applications[appIndex].HelpUrl);

    //                if (Program.Applications[appIndex].Name[0].Value == Program.RM.GetString("SevenUpdate") && Program.Applications[appIndex].Publisher[0].Value == Program.RM.GetString("SevenSoftware"))
    //                    cmsHideUpdate.Enabled = false;
    //                else
    //                {
    //                    cmsHideUpdate.Enabled = true;
    //                }
    //            }
    //        }
    //        else
    //            sidebar.HideText();
    //    }

    //    #endregion

    //    #endregion

        #endregion
    }
}
