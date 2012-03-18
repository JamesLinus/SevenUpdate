// ***********************************************************************
// <copyright file="Options.xaml.cs" project="SevenUpdate" assembly="SevenUpdate" solution="SevenUpdate" company="Seven Software">
//     Copyright (c) Seven Software. All rights reserved.
// </copyright>
// <author username="sevenalive">Robert Baker</author>
// <license href="http://www.gnu.org/licenses/gpl-3.0.txt" name="GNU General Public License 3">
//  This file is part of Seven Update.
//    Seven Update is free software: you can redistribute it and/or modify it under the terms of the GNU General Public
//    License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any
//    later version. Seven Update is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without
//    even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details. You should have received a copy of the GNU General Public License
//    along with Seven Update.  If not, see http://www.gnu.org/licenses/.
// </license>
// <summary>
//   Interaction logic for Options.xaml
// .</summary> ***********************************************************************

namespace SevenUpdate.Pages
{
    using System;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Net;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Controls.Primitives;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Navigation;
    using System.Windows.Threading;

    using SevenSoftware.Windows;
    using SevenSoftware.Windows.Controls;

    /// <summary>Interaction logic for Options.xaml.</summary>
    public partial class Options
    {
        #region Constants and Fields

        /// <summary>The official collection of the applications that Seven Update can update.</summary>
        private static ObservableCollection<Sua> apps;

        /// <summary>The local collection of the apps that Seven Update can update.</summary>
        private static ObservableCollection<Sua> machineAppList;

        /// <summary>The program configuration.</summary>
        private Config config;

        #endregion

        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref="Options" /> class.</summary>
        public Options()
        {
            this.InitializeComponent();

            this.lvApps.AddHandler(Thumb.DragDeltaEvent, new DragDeltaEventHandler(this.RestrictColumn), true);
            this.btnSave.IsShieldNeeded = !Core.Instance.IsAdmin;

            this.MouseLeftButtonDown -= Core.EnableDragOnGlass;
            this.MouseLeftButtonDown += Core.EnableDragOnGlass;
            AeroGlass.CompositionChanged -= this.UpdateUI;
            AeroGlass.CompositionChanged += this.UpdateUI;

            if (AeroGlass.IsGlassEnabled)
            {
                this.tbTitle.Foreground = Brushes.Black;
                this.line.Visibility = Visibility.Collapsed;
                this.rectangle.Visibility = Visibility.Collapsed;
            }
            else
            {
                this.tbTitle.Foreground = new SolidColorBrush(Color.FromRgb(0, 51, 153));
                this.line.Visibility = Visibility.Visible;
                this.rectangle.Visibility = Visibility.Visible;
            }
        }

        #endregion

        #region Methods

        /// <summary>Downloads the Seven Update Application List.</summary>
        private static void DownloadSul()
        {
            try
            {
                apps = Utilities.Deserialize<ObservableCollection<Sua>>(Utilities.DownloadFile(App.SulLocation));
            }
            catch (Exception ex)
            {
                Utilities.ReportError(ex, ErrorType.FatalError, App.SulLocation);
                if (!(ex is NullReferenceException || ex is WebException))
                {
                    throw;
                }
            }
        }

        /// <summary>Navigates to the Seven Update privacy policy.</summary>
        /// <param name="sender">The object that called the event.</param>
        /// <param name="e">The event data.</param>
        private void GoToPrivacyPolicy(object sender, RequestNavigateEventArgs e)
        {
            Utilities.StartProcess("http://sevenupdate.com/privacy");
            e.Handled = true;
        }

        /// <summary>Loads the settings and <c>Sua</c> list when the page is loaded.</summary>
        /// <param name="sender">The object that called the event.</param>
        /// <param name="e">The <c>System.Windows.RoutedEventArgs</c> instance containing the event data.</param>
        private void Init(object sender, RoutedEventArgs e)
        {
            this.lvApps.Cursor = Cursors.Wait;
            this.tbDownloading.Visibility = Visibility.Visible;
            this.config = Core.Settings;
            this.DataContext = this.config;
            this.lvApps.ItemsSource = null;
            machineAppList = null;
            Task.Factory.StartNew(DownloadSul).ContinueWith(
                delegate { this.Dispatcher.BeginInvoke(this.LoadSul, apps); });
        }

        /// <summary>
        ///   Loads the list of Seven Update applications and sets the UI, if no application list was downloaded, load
        ///   the stored list on the system.
        /// </summary>
        /// <param name="officialApplicationList">The official application list from the server.</param>
        private void LoadSul(ObservableCollection<Sua> officialApplicationList = null)
        {
            try
            {
                if (File.Exists(App.ApplicationsFile))
                {
                    machineAppList = Utilities.Deserialize<ObservableCollection<Sua>>(App.ApplicationsFile);
                }
            }
            catch (NullReferenceException)
            {
                machineAppList = null;
            }

            if (machineAppList == null)
            {
                machineAppList = new ObservableCollection<Sua>();
            }

            if (officialApplicationList == null)
            {
                officialApplicationList = new ObservableCollection<Sua>();
            }

            if (machineAppList.Count > 0)
            {
                for (int x = 0; x < machineAppList.Count; x++)
                {
                    if (machineAppList[x].Platform == Platform.X64 && !Utilities.IsRunning64BitOS)
                    {
                    }
                    else
                    {
                        if (
                            Directory.Exists(
                                Utilities.IsRegistryKey(machineAppList[x].Directory)
                                    ? Utilities.GetRegistryValue(
                                        machineAppList[x].Directory, 
                                        machineAppList[x].ValueName, 
                                        machineAppList[x].Platform)
                                    : Utilities.ConvertPath(
                                        machineAppList[x].Directory, true, machineAppList[x].Platform))
                            && machineAppList[x].IsEnabled)
                        {
                            continue;
                        }
                    }

                    // Remove the application from the list if it is no longer installed or not enabled
                    machineAppList.RemoveAt(x);
                    x--;
                    continue;
                }
            }

            if (officialApplicationList.Count > 0)
            {
                for (int x = 0; x < officialApplicationList.Count; x++)
                {
                    if (officialApplicationList[x].Platform == Platform.X64 && !Utilities.IsRunning64BitOS)
                    {
                        officialApplicationList.RemoveAt(x);
                        x--;
                        continue;
                    }

                    if (
                        !Directory.Exists(
                            Utilities.IsRegistryKey(officialApplicationList[x].Directory)
                                ? Utilities.GetRegistryValue(
                                    officialApplicationList[x].Directory, 
                                    officialApplicationList[x].ValueName, 
                                    officialApplicationList[x].Platform)
                                : Utilities.ConvertPath(
                                    officialApplicationList[x].Directory, true, officialApplicationList[x].Platform)))
                    {
                        // Remove the application from the list if it is not installed
                        officialApplicationList.RemoveAt(x);
                        x--;
                        continue;
                    }

                    if (machineAppList.Count < 1)
                    {
                        continue;
                    }

                    for (int y = 0; y < machineAppList.Count; y++)
                    {
                        // Check if the app in both lists are the same
                        if (officialApplicationList[x].Directory == machineAppList[y].Directory
                            && officialApplicationList[x].Platform == machineAppList[y].Platform)
                        {
                            // if (officialAppList[x].Source != machineAppList[y].Source) continue;
                            officialApplicationList[x].IsEnabled = machineAppList[y].IsEnabled;
                            machineAppList.RemoveAt(y);
                            y--;
                        }

                        continue;
                    }
                }

                if (machineAppList.Count > 0)
                {
                    foreach (var t in machineAppList)
                    {
                        officialApplicationList.Add(t);
                    }
                }
            }

            if (officialApplicationList.Count > 0)
            {
                machineAppList = officialApplicationList;
            }

            this.Dispatcher.BeginInvoke(new Action(this.UpdateList));
        }

        /// <summary>Goes back to the Main page.</summary>
        /// <param name="sender">The object that called the event.</param>
        /// <param name="e">The <c>System.Windows.RoutedEventArgs</c> instance containing the event data.</param>
        private void NavigateToMainPage(object sender, RoutedEventArgs e)
        {
            Core.NavigateToMainPage();
        }

        /// <summary>Navigates to a Uri.</summary>
        /// <param name="sender">The object that called the event.</param>
        /// <param name="e">The <c>System.Windows.Navigation.RequestNavigateEventArgs</c> instance containing the event data.</param>
        private void NavigateToUri(object sender, RequestNavigateEventArgs e)
        {
            Utilities.StartProcess(e.Uri.AbsoluteUri);

            e.Handled = true;
        }

        /// <summary>Limit the size of the <c>GridViewColumn</c> when it's being resized.</summary>
        /// <param name="sender">The object that called the event.</param>
        /// <param name="e">The <c>System.Windows.Controls.Primitives.DragDeltaEventArgs</c> instance containing the event data.</param>
        private void RestrictColumn(object sender, DragDeltaEventArgs e)
        {
            ListViewExtensions.LimitColumnSize((Thumb)e.OriginalSource);
        }

        /// <summary>Saves the settings and goes back to the Main page.</summary>
        /// <param name="sender">The object that called the event.</param>
        /// <param name="e">The <c>System.Windows.RoutedEventArgs</c> instance containing the event data.</param>
        private void SaveSettings(object sender, RoutedEventArgs e)
        {
            if (WcfService.SaveSettings(this.config.AutoOption != AutoUpdateOption.Never, this.config, machineAppList))
            {
                Core.NavigateToMainPage();
            }
        }

        /// <summary>Updates the list with the <c>machineAppList</c>.</summary>
        private void UpdateList()
        {
            this.lvApps.Cursor = Cursors.Arrow;
            if (machineAppList.Count < 1)
            {
            }

            this.tbDownloading.Visibility = Visibility.Hidden;
            this.lvApps.ItemsSource = machineAppList;
        }

        /// <summary>Changes the UI depending on whether Aero Glass is enabled.</summary>
        /// <param name="sender">The object that called the event.</param>
        /// <param name="e">The <c>CompositionChangedEventArgs</c> instance containing the event data.</param>
        private void UpdateUI(object sender, CompositionChangedEventArgs e)
        {
            if (e.IsGlassEnabled)
            {
                this.tbTitle.Foreground = Brushes.Black;
                this.line.Visibility = Visibility.Collapsed;
                this.rectangle.Visibility = Visibility.Collapsed;
            }
            else
            {
                this.tbTitle.Foreground = new SolidColorBrush(Color.FromRgb(0, 51, 153));
                this.line.Visibility = Visibility.Visible;
                this.rectangle.Visibility = Visibility.Visible;
            }
        }

        #endregion
    }
}