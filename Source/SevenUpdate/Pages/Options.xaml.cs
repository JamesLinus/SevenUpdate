// ***********************************************************************
// <copyright file="Options.xaml.cs"
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
    using System.IO;
    using System.Net;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Input;
    using System.Windows.Navigation;

    /// <summary>Interaction logic for Options.xaml</summary>
    public partial class Options
    {
        #region Constants and Fields

        /// <summary>A collection of <see cref = "Sua" /> that Seven Update can update</summary>
        private static ObservableCollection<Sua> machineAppList;

        /// <summary>The program configuration</summary>
        private Config config;

        #endregion

        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref = "Options" /> class.</summary>
        public Options()
        {
            this.InitializeComponent();

            this.lvApps.AddHandler(Thumb.DragDeltaEvent, new DragDeltaEventHandler(this.RestrictColumn), true);
            this.btnSave.IsShieldNeeded = !Core.Instance.IsAdmin;
        }

        #endregion

        #region Methods

        /// <summary>Downloads the Seven Update Application List</summary>
        private void DownloadSul()
        {
            try
            {
                var apps = Utilities.Deserialize<ObservableCollection<Sua>>(Utilities.DownloadFile(App.SulLocation));
                this.LoadSul(apps);
            }
            catch (Exception ex)
            {
                Utilities.ReportError(ex, ErrorType.FatalError);
                if (!(ex is NullReferenceException || ex is WebException))
                {
                    throw;
                }

                this.LoadSul();
            }
        }

        /// <summary>Loads the settings and <see cref="Sua"/> list when the page is loaded</summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void Init(object sender, RoutedEventArgs e)
        {
            this.lvApps.Cursor = Cursors.Wait;
            this.config = Core.Settings;
            this.DataContext = this.config;

            Task.Factory.StartNew(this.DownloadSul);
        }

        /// <summary>Loads the list of Seven Update applications and sets the UI, if no application list was downloaded, load the stored list on the system</summary>
        private void LoadSul()
        {
            this.LoadSul(null);
        }

        /// <summary>Loads the list of Seven Update applications and sets the UI, if no application list was downloaded, load the stored list on the system</summary>
        /// <param name="officialApplicationList">The official application list from the server.</param>
        private void LoadSul(ObservableCollection<Sua> officialApplicationList)
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

            if (machineAppList != null)
            {
                for (var x = 0; x < machineAppList.Count; x++)
                {
                    if (Directory.Exists(Utilities.IsRegistryKey(machineAppList[x].Directory)
                                ? Utilities.GetRegistryValue(machineAppList[x].Directory, machineAppList[x].ValueName, machineAppList[x].Platform)
                                : Utilities.ConvertPath(machineAppList[x].Directory, true, machineAppList[x].Platform)) && machineAppList[x].IsEnabled)
                    {
                        continue;
                    }

                    // Remove the application from the list if it is no longer installed or not enabled
                    machineAppList.RemoveAt(x);
                    x--;
                }
            }

            if (officialApplicationList != null)
            {
                for (var x = 0; x < officialApplicationList.Count; x++)
                {
                    if (!Directory.Exists(Utilities.IsRegistryKey(officialApplicationList[x].Directory)
                                ? Utilities.GetRegistryValue(officialApplicationList[x].Directory, officialApplicationList[x].ValueName, officialApplicationList[x].Platform)
                                : Utilities.ConvertPath(officialApplicationList[x].Directory, true, officialApplicationList[x].Platform)))
                    {
                        // Remove the application from the list if it is not installed
                        officialApplicationList.RemoveAt(x);
                        x--;
                    }
                    else
                    {
                        if (machineAppList == null)
                        {
                            continue;
                        }

                        for (var y = 0; y < machineAppList.Count; y++)
                        {
                            // Check if the app in both lists are the same
                            if (officialApplicationList[x].Directory == machineAppList[y].Directory && officialApplicationList[x].Platform == machineAppList[y].Platform)
                            {
                                // if (officialAppList[x].Source != machineAppList[y].Source)
                                // continue;
                                officialApplicationList[x].IsEnabled = machineAppList[y].IsEnabled;
                                machineAppList.RemoveAt(y);
                                y--;
                            }

                            continue;
                        }
                    }
                }

                if (machineAppList != null)
                {
                    foreach (var t in machineAppList)
                    {
                        officialApplicationList.Add(t);
                    }
                }
            }

            if (officialApplicationList != null)
            {
                machineAppList = officialApplicationList;
            }

            this.Dispatcher.BeginInvoke(this.UpdateList);
        }

        /// <summary>Navigates to a Uri</summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Navigation.RequestNavigateEventArgs"/> instance containing the event data.</param>
        private void NavigateToUri(object sender, RequestNavigateEventArgs e)
        {
            Utilities.StartProcess(e.Uri.AbsoluteUri);

            e.Handled = true;
        }

        /// <summary>Limit the size of the <see cref="GridViewColumn"/> when it's being resized</summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Controls.Primitives.DragDeltaEventArgs"/> instance containing the event data.</param>
        private void RestrictColumn(object sender, DragDeltaEventArgs e)
        {
            ListViewExtensions.LimitColumnSize((Thumb)e.OriginalSource);
        }

        /// <summary>Saves the settings and goes back to the Main page</summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void SaveSettings(object sender, RoutedEventArgs e)
        {
            if (WcfService.SaveSettings(this.config.AutoOption != AutoUpdateOption.Never, this.config, machineAppList))
            {
                Core.NavigateToMainPage();
            }
        }

        /// <summary>Updates the list with the <see cref="machineAppList"/></summary>
        private void UpdateList()
        {
            this.lvApps.Cursor = Cursors.Arrow;
            if (machineAppList != null)
            {
                this.lvApps.ItemsSource = machineAppList;
            }
        }

        /// <summary>Goes back to the Main page</summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void NavigateToMainPage(object sender, RoutedEventArgs e)
        {
            Core.NavigateToMainPage();
        }

        #endregion
    }
}