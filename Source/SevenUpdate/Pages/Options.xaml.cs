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

using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using SevenUpdate.Base;
using SevenUpdate.Controls;
using SevenUpdate.Windows;

#endregion

namespace SevenUpdate.Pages
{
    /// <summary>
    /// Interaction logic for Options.xaml
    /// </summary>
    public partial class Options : Page
    {
        #region Global Vars

        /// <summary>
        /// The Seven Update list location
        /// </summary>
        private const string SulLocation = @"http://sevenupdate.com/apps/Apps.sul";

        /// <summary>
        /// A collection of SUA's that Seven Update can update
        /// </summary>
        private ObservableCollection<Sua> machineAppList;

        #endregion

        /// <summary>
        /// The constructor for the Options Page
        /// </summary>
        public Options()
        {
            InitializeComponent();
            listView.AddHandler(Thumb.DragDeltaEvent, new DragDeltaEventHandler(Thumb_DragDelta), true);
            if (App.IsAdmin)
                btnSave.Content = App.RM.GetString("Save");
        }

        #region Methods

        /// <summary>
        /// Downloads the Seven Update Application List
        /// </summary>
        private void DownloadSul()
        {
            try
            {
                LoadSul(Base.Base.Deserialize<ObservableCollection<Sua>>(Base.Base.DownloadFile(SulLocation), SulLocation));
            }
            catch (WebException)
            {
                LoadSul();
            }
        }

        /// <summary>
        /// Loads the configuration and sets the UI
        /// </summary>
        private void LoadSettings()
        {
            switch (App.Settings.AutoOption)
            {
                case AutoUpdateOption.Install:
                    cbAutoUpdateMethod.SelectedIndex = 0;
                    break;

                case AutoUpdateOption.Download:
                    cbAutoUpdateMethod.SelectedIndex = 1;
                    break;

                case AutoUpdateOption.Notify:
                    cbAutoUpdateMethod.SelectedIndex = 2;
                    break;

                case AutoUpdateOption.Never:
                    cbAutoUpdateMethod.SelectedIndex = 3;
                    break;
            }

            chkRecommendedUpdates.IsChecked = App.Settings.IncludeRecommended;
        }

        /// <summary>
        /// Loads the list of Seven Update applications and sets the UI, if no appList was downloaded, load the stored list on the system
        /// </summary>
        private void LoadSul(ObservableCollection<Sua> officialAppList = null)
        {
            machineAppList = Base.Base.Deserialize<ObservableCollection<Sua>>(Base.Base.AppsFile);

            if (machineAppList != null)
            {
                for (var x = 0; x < machineAppList.Count; x++)
                {
                    if (Directory.Exists(Base.Base.ConvertPath(machineAppList[x].Directory, true, machineAppList[x].Is64Bit)) && machineAppList[x].IsEnabled)
                        continue;
                    // Remove the application from the list if it is no longer installed or not enabled
                    machineAppList.RemoveAt(x);
                    x--;
                }
            }


            if (officialAppList != null)
            {
                for (var x = 0; x < officialAppList.Count; x++)
                {
                    if (!Directory.Exists(Base.Base.ConvertPath(officialAppList[x].Directory, true, officialAppList[x].Is64Bit)))
                    {
                        // Remove the application from the applist if it is not installed
                        officialAppList.RemoveAt(x);
                        x--;
                    }
                    else
                    {
                        if (machineAppList == null)
                            continue;
                        for (var y = 0; y < machineAppList.Count; y++)
                        {
                            // Check if the app in both lists are the same
                            if (officialAppList[x].Directory == machineAppList[y].Directory && officialAppList[x].Is64Bit == machineAppList[y].Is64Bit)
                            {
                                //if (officialAppList[x].Source != machineAppList[y].Source)
                                //    continue;

                                officialAppList[x].IsEnabled = machineAppList[y].IsEnabled;
                                machineAppList.RemoveAt(y);
                                y--;

                                break;
                            }
                            continue;
                        }
                    }
                }
                if (machineAppList != null)
                {
                    foreach (Sua t in machineAppList)
                        officialAppList.Add(t);
                }
            }

            if (officialAppList != null)
                machineAppList = officialAppList;
            Dispatcher.BeginInvoke(UpdateList);
        }

        /// <summary>
        /// Updates the list with the <see cref="machineAppList" />
        /// </summary>
        private void UpdateList()
        {
            listView.Cursor = Cursors.Arrow;
            if (machineAppList != null)
            {
                listView.ItemsSource = machineAppList;
                machineAppList.CollectionChanged += UserAppList_CollectionChanged;
                AddSortBinding();
                tbListStatus.Text = null;
            }
            else
                tbListStatus.Text = App.RM.GetString("CouldNotConnect");
        }

        /// <summary>
        /// Adds the <see cref="GridViewColumn" />'s of the <see cref="ListView" /> to be sorted
        /// </summary>
        private void AddSortBinding()
        {
            var gv = (GridView) listView.View;

            var col = gv.Columns[1];
            ListViewSorter.SetSortBindingMember(col, new Binding("ApplicationName"));

            col = gv.Columns[2];
            ListViewSorter.SetSortBindingMember(col, new Binding("Publisher"));

            col = gv.Columns[3];
            ListViewSorter.SetSortBindingMember(col, new Binding("Architecture"));

            ListViewSorter.SetCustomSorter(listView, new ListViewExtensions.SuaSorter());
        }

        /// <summary>
        /// Saves the Settings
        /// </summary>
        private void SaveSettings()
        {
            var options = new Config();

            if (cbAutoUpdateMethod.SelectedIndex == 0)
                options.AutoOption = AutoUpdateOption.Install;

            if (cbAutoUpdateMethod.SelectedIndex == 1)
                options.AutoOption = AutoUpdateOption.Download;

            if (cbAutoUpdateMethod.SelectedIndex == 2)
                options.AutoOption = AutoUpdateOption.Notify;

            if (cbAutoUpdateMethod.SelectedIndex == 3)
                options.AutoOption = AutoUpdateOption.Never;

            if (chkRecommendedUpdates.IsChecked != null)
                options.IncludeRecommended = ((bool) chkRecommendedUpdates.IsChecked);


            if (cbAutoUpdateMethod.SelectedIndex == 3)
            {
                options.AutoOption = AutoUpdateOption.Never;

                Admin.SaveSettings(false, options, machineAppList);
            }
            else
                Admin.SaveSettings(true, options, machineAppList);
        }

        #endregion

        #region UI Events

        /// <summary>
        /// When the AutoUpdate selection changes update the shield image
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AutoUpdateMethod_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (cbAutoUpdateMethod.SelectedIndex)
            {
                case 0:
                    imgShield.Source = App.GreenShield;
                    break;
                case 1:
                    imgShield.Source = App.GreenShield;
                    break;
                case 2:
                    imgShield.Source = null;
                    break;
                case 3:
                    imgShield.Source = App.RedShield;
                    break;
            }
        }

        /// <summary>
        /// Loads the settings and SUA list when the page is loaded
        /// </summary>
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            listView.Cursor = Cursors.Wait;
            LoadSettings();
            new Thread(DownloadSul).Start();
        }

        #region ListView Related

        /// <summary>
        /// Updates the <see cref="CollectionView" /> when the <c>userAppList</c> collection changes
        /// </summary>
        private void UserAppList_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            // update the view when item change is NOT caused by replacement
            if (e.Action != NotifyCollectionChangedAction.Replace)
                return;
            var dataView = CollectionViewSource.GetDefaultView(listView.ItemsSource);
            dataView.Refresh();
        }

        /// <summary>
        /// Limit the size of the <see cref="GridViewColumn" /> when it's being resized
        /// </summary>
        private void Thumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            ListViewExtensions.LimitColumnSize(((Thumb) e.OriginalSource));
        }

        #endregion

        #region Buttons

        /// <summary>
        /// Saves the settings and goes back to the Main page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            SaveSettings();
            MainWindow.NavService.GoBack();
        }

        /// <summary>
        /// Goes back to the Main page without saving the settings
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.NavService.GoBack();
        }

        #endregion

        #endregion
    }
}