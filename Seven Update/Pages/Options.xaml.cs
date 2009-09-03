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
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;
using SevenUpdate.Controls;
using SevenUpdate.Properties;
using SevenUpdate.WCF;
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
        private static readonly Uri SUALoc = new Uri("http://ittakestime.org/su/Apps.sul");

        private ObservableCollection<SUA> userAppList;

        #endregion

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
        private void DownloadSUL()
        {
            try
            {
                File.Delete(Shared.UserStore + @"Apps.sul");
            }
            catch
            {
                return;
            }
            var wc = new WebClient();

            wc.DownloadFileCompleted += WcDownloadFileCompleted;

            wc.DownloadFileAsync(SUALoc, Shared.UserStore + @"Apps.sul");
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
            //tbLastUpdated.Text = App.RM.GetString("LastUpdated") + " " + Settings.Default.lastListUpdate.ToShortDateString() + " " + App.RM.GetString("At") + " " +
            //                     Settings.Default.lastListUpdate.ToShortTimeString();
        }

        /// <summary>
        /// Loads the list of Seven Update applications and sets the ui
        /// </summary>
        private void LoadSUL()
        {
            ObservableCollection<SUA> officalAppList = null;
            if (File.Exists(Shared.UserStore + @"Apps.sul"))
                officalAppList = Shared.Deserialize<ObservableCollection<SUA>>(Shared.UserStore + @"Apps.sul");
            userAppList = Shared.Deserialize<ObservableCollection<SUA>>(Shared.AppsFile);

            if (officalAppList != null)
            {
                for (var x = 0; x < officalAppList.Count; x++)
                {
                    if (Directory.Exists(Shared.ConvertPath(officalAppList[x].Directory, true, officalAppList[x].Is64Bit)))
                        continue;
                    officalAppList.RemoveAt(x);
                    x--;
                }
            }

            if (userAppList != null)
            {
                for (var x = 0; x < userAppList.Count; x++)
                {
                    if (!Directory.Exists(Shared.ConvertPath(userAppList[x].Directory, true, userAppList[x].Is64Bit)))
                    {
                        userAppList.RemoveAt(x);
                        x--;
                        continue;
                    }
                    if (officalAppList == null)
                        continue;
                    for (var y = 0; y < officalAppList.Count; y++)
                    {
                        if (officalAppList[y].Source != userAppList[x].Source)
                            continue;
                        officalAppList[y].IsEnabled = userAppList[x].IsEnabled;
                        userAppList[x] = officalAppList[y];
                    }
                }
                if (userAppList == null && officalAppList != null)
                    userAppList = officalAppList;


            }

            Dispatcher.BeginInvoke(InvokeList);
        }

        private void InvokeList()
        {
            if (userAppList != null)
            {
                listView.ItemsSource = userAppList;
                userAppList.CollectionChanged += UserAppList_CollectionChanged;
                AddSortBinding();
                tbLastUpdated.Text = App.RM.GetString("LastUpdated") + " " + Settings.Default.lastListUpdate.ToShortDateString() + " " + App.RM.GetString("At") + " " +
                                     Settings.Default.lastListUpdate.ToShortTimeString();

            }
            else
            {
                tbLastUpdated.Text = App.RM.GetString("CouldNotConnect");
                listView.Cursor = Cursors.Arrow;
            }

        }

        /// <summary>
        /// Enables the listview columns to be sorted
        /// </summary>
        private void AddSortBinding()
        {
            var gv = (GridView)listView.View;

            var col = gv.Columns[1];
            ListViewSorter.SetSortBindingMember(col, new Binding("ApplicationName"));

            col = gv.Columns[2];
            ListViewSorter.SetSortBindingMember(col, new Binding("Publisher"));

            col = gv.Columns[3];
            ListViewSorter.SetSortBindingMember(col, new Binding("Architecture"));

            ListViewSorter.SetCustomSorter(listView, new ListViewExtensions.SUASorter());
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

            options.IncludeRecommended = ((bool)chkRecommendedUpdates.IsChecked);


            if (cbAutoUpdateMethod.SelectedIndex == 3)
            {
                options.AutoOption = AutoUpdateOption.Never;

                Admin.SaveSettings(false, options, userAppList);
            }
            else
                Admin.SaveSettings(true, options, userAppList);
        }

        #endregion

        #region UI Events

        private void WcDownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            if (e.Error != null)
                return;
            Settings.Default.lastListUpdate = DateTime.Now;

            Settings.Default.Save();

            LoadSUL();
        }

        private void cbAutoUpdateMethod_SelectionChanged(object sender, SelectionChangedEventArgs e)
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

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            listView.Cursor = Cursors.Wait;
            LoadSettings();

            if (!Settings.Default.lastListUpdate.Date.Equals(DateTime.Now.Date) || !File.Exists(Shared.AppsFile))
            {
                var thread = new Thread(DownloadSUL);
                thread.Start();
            }
            else
            {
                var thread = new Thread(LoadSUL);
                thread.Start();
            }
        }

        #region ListView Events

        private void UserAppList_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            // update the view when item change is NOT caused by replacement
            if (e.Action != NotifyCollectionChangedAction.Replace)
                return;
            var dataView = CollectionViewSource.GetDefaultView(listView.ItemsSource);
            dataView.Refresh();
        }

        private void Thumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            ListViewExtensions.Thumb_DragDelta(sender, ((Thumb)e.OriginalSource));
        }

        #endregion

        #region TextBlocks

        private void TextBlock_MouseEnter(object sender, MouseEventArgs e)
        {
            var textBlock = ((TextBlock)sender);
            textBlock.TextDecorations = TextDecorations.Underline;
        }

        private void TextBlock_MouseLeave(object sender, MouseEventArgs e)
        {
            var textBlock = ((TextBlock)sender);
            textBlock.TextDecorations = null;
        }

        private void tbRefresh_MouseDown(object sender, MouseButtonEventArgs e)
        {
            listView.Cursor = Cursors.Wait;
            var thread = new Thread(LoadSUL);
            thread.Start();
        }

        #endregion

        #region Buttons

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            SaveSettings();
            MainWindow.NavService.GoBack();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.NavService.GoBack();
        }

        #endregion

        #endregion
    }
}