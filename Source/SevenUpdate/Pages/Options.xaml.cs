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
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using Microsoft.Windows.Controls;

#endregion

namespace SevenUpdate.Pages
{
    /// <summary>
    ///   Interaction logic for Options.xaml
    /// </summary>
    public partial class Options : Page
    {
        #region Fields

        /// <summary>
        ///   The Seven Update list location
        /// </summary>
        private const string SulLocation = @"http://sevenupdate.com/apps/Apps.sul";

        /// <summary>
        ///   A collection of SUA's that Seven Update can update
        /// </summary>
        private static ObservableCollection<Sua> machineAppList;

        private Config config;

        #endregion

        /// <summary>
        ///   The constructor for the Options Page
        /// </summary>
        public Options()
        {
            InitializeComponent();
            lvApps.AddHandler(Thumb.DragDeltaEvent, new DragDeltaEventHandler(Thumb_DragDelta), true);
        }

        #region Methods

        /// <summary>
        ///   Downloads the Seven Update Application List
        /// </summary>
        private void DownloadSul()
        {
            try
            {
                LoadSul(Base.Deserialize<ObservableCollection<Sua>>(Base.DownloadFile(SulLocation), SulLocation));
            }
            catch (WebException)
            {
                LoadSul();
            }
        }

        /// <summary>
        ///   Loads the list of Seven Update applications and sets the UI, if no appList was downloaded, load the stored list on the system
        /// </summary>
        private void LoadSul(ObservableCollection<Sua> officialAppList = null)
        {
            machineAppList = Base.Deserialize<ObservableCollection<Sua>>(Base.AppsFile);

            if (machineAppList != null)
            {
                for (var x = 0; x < machineAppList.Count; x++)
                {
                    if (
                        Directory.Exists(Base.IsRegistryKey(machineAppList[x].Directory)
                                             ? Base.GetRegistryPath(machineAppList[x].Directory, machineAppList[x].ValueName, machineAppList[x].Is64Bit)
                                             : Base.ConvertPath(machineAppList[x].Directory, true, machineAppList[x].Is64Bit)) && machineAppList[x].IsEnabled)
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
                    if (
                        !Directory.Exists(Base.IsRegistryKey(officialAppList[x].Directory)
                                              ? Base.GetRegistryPath(officialAppList[x].Directory, officialAppList[x].ValueName, officialAppList[x].Is64Bit)
                                              : Base.ConvertPath(officialAppList[x].Directory, true, officialAppList[x].Is64Bit)))
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
        ///   Updates the list with the <see cref = "machineAppList" />
        /// </summary>
        private void UpdateList()
        {
            lvApps.Cursor = Cursors.Arrow;
            if (machineAppList != null)
            {
                lvApps.ItemsSource = machineAppList;
                machineAppList.CollectionChanged += UserAppList_CollectionChanged;
                AddSortBinding();
                lblListStatus.Text = null;
            }
            else
                lblListStatus.Text = Properties.Resources.CouldNotConnect;
        }

        /// <summary>
        ///   Adds the <see cref = "GridViewColumn" />'s of the <see cref = "ListView" /> to be sorted
        /// </summary>
        private void AddSortBinding()
        {
            var gv = (GridView) lvApps.View;

            var col = gv.Columns[1];
            ListViewSorter.SetSortBindingMember(col, new Binding("ApplicationName"));

            col = gv.Columns[2];
            ListViewSorter.SetSortBindingMember(col, new Binding("Publisher"));

            col = gv.Columns[3];
            ListViewSorter.SetSortBindingMember(col, new Binding("Architecture"));

            ListViewSorter.SetCustomSorter(lvApps, new CustomComparers.SuaSorter());
        }

        #endregion

        #region UI Events

        /// <summary>
        ///   Loads the settings and SUA list when the page is loaded
        /// </summary>
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            lvApps.Cursor = Cursors.Wait;
            config = App.Settings;
            DataContext = config;
            new Thread(DownloadSul).Start();
        }

        #region ListView Related

        /// <summary>
        ///   Updates the <see cref = "CollectionView" /> when the <c>userAppList</c> collection changes
        /// </summary>
        private void UserAppList_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            // update the view when item change is NOT caused by replacement
            if (e.Action != NotifyCollectionChangedAction.Replace)
                return;
            var dataView = CollectionViewSource.GetDefaultView(lvApps.ItemsSource);
            dataView.Refresh();
        }

        /// <summary>
        ///   Limit the size of the <see cref = "GridViewColumn" /> when it's being resized
        /// </summary>
        private void Thumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            ListViewExtensions.LimitColumnSize(((Thumb) e.OriginalSource));
        }

        #endregion

        #region Buttons

        /// <summary>
        ///   Saves the settings and goes back to the Main page
        /// </summary>
        /// <param name = "sender" />
        /// <param name = "e" />
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            AdminClient.SaveSettings(config.AutoOption != AutoUpdateOption.Never, config, machineAppList);
        }

        #endregion

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            try
            {
                Process.Start(e.Uri.AbsoluteUri);
            }
            catch
            {
            }
            e.Handled = true;
        }

        #endregion

    }
}