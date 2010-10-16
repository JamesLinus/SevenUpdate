// ***********************************************************************
// <copyright file="Main.xaml.cs"
//            project="SevenUpdate"
//            assembly="SevenUpdate"
//            solution="SevenUpdate"
//            company="Seven Software">
//     Copyright (c) Seven Software. All rights reserved.
// </copyright>
// <author username="sevenalive">Robert Baker</author>
// <license href="http://www.gnu.org/licenses/gpl-3.0.txt" name="GNU General Public License 3">
//  This file is part of Seven Update.
//
//    Seven Update is free software: you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation, either version 3 of the License, or
//    (at your option) any later version.
//
//    Seven Update is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.
//
//    You should have received a copy of the GNU General Public License
//    along with Seven Update.  If not, see http://www.gnu.org/licenses/.
// </license>
// ***********************************************************************
namespace SevenUpdate.Pages
{
    using System;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Threading.Tasks;
    using System.Timers;
    using System.Windows;
    using System.Windows.Input;

    using SevenUpdate.Properties;
    using SevenUpdate.Service;
    using SevenUpdate.Windows;

    /// <summary>Interaction logic for Main.xaml</summary>
    public sealed partial class Main
    {
        #region Constants and Fields

        /// <summary><see langword = "true" /> if the page was already initialized</summary>
        private bool init;

        /// <summary>A timer to check if seven update is trying to connect</summary>
        private Timer timer;

        #endregion

        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref = "Main" /> class.</summary>
        public Main()
        {
            this.InitializeComponent();

            RestoreUpdates.RestoredHiddenUpdate += SettingsChanged;
            WcfService.SettingsChanged += SettingsChanged;
        }

        #endregion

        #region Methods

        /// <summary>Checks for updates after settings were changed</summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private static void SettingsChanged(object sender, EventArgs e)
        {
            Core.CheckForUpdates(true);
        }

        /// <summary>Checks for updates</summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        private void CheckForUpdates(object sender, MouseButtonEventArgs e)
        {
            Core.CheckForUpdates();
        }

        /// <summary>Check if Seven Update is still trying to connect to <see cref="WcfService"/></summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Timers.ElapsedEventArgs"/> instance containing the event data.</param>
        private void CheckIfConnecting(object sender, ElapsedEventArgs e)
        {
            this.timer.Enabled = false;
            this.timer.Stop();
            if (Core.Instance.UpdateAction != UpdateAction.ConnectingToService)
            {
                return;
            }

            WcfService.AdminError(new Exception(Properties.Resources.CouldNotConnectService));
        }

        /// <summary>Loads settings and UI for the page</summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void Init(object sender, RoutedEventArgs e)
        {
            if (Utilities.RebootNeeded)
            {
                Core.Instance.UpdateAction = UpdateAction.RebootNeeded;
            }

            if (this.init)
            {
                return;
            }

            Core.Instance.UpdateAction = UpdateAction.NoUpdates;
            if (Core.IsReconnect)
            {
                Core.Instance.UpdateAction = UpdateAction.ConnectingToService;
                this.timer = new Timer
                    {
                        Enabled = true,
                        Interval = 30000
                    };
                this.timer.Elapsed += this.CheckIfConnecting;
                WcfService.Connect();
            }
            else if (File.Exists(Utilities.AllUserStore + @"updates.sui"))
            {
                var lastCheck = File.GetLastWriteTime(Utilities.AllUserStore + @"updates.sui");

                var today = DateTime.Now;

                if (lastCheck.Month == today.Month && lastCheck.Year == today.Year)
                {
                    if (lastCheck.Day == today.Day || lastCheck.Day + 1 == today.Day || lastCheck.Day + 2 == today.Day || lastCheck.Day + 3 == today.Day || lastCheck.Day + 4 == today.Day ||
                        lastCheck.Day + 5 == today.Day)
                    {
                        WcfService.Disconnect();
                        if (File.Exists(Utilities.AllUserStore + @"updates.sui"))
                        {
                            Task.Factory.StartNew(() => Search.SetUpdatesFound(Utilities.Deserialize<Collection<Sui>>(Utilities.AllUserStore + @"updates.sui")));
                        }
                    }
                }
                else
                {
                    try
                    {
                        File.Delete(Utilities.AllUserStore + @"updates.sui");
                    }
                    catch (Exception)
                    {
                    }

                    Core.Instance.UpdateAction = UpdateAction.CheckForUpdates;
                }
            }
            else
            {
                try
                {
                    if (Settings.Default.lastUpdateCheck == DateTime.MinValue)
                    {
                        Core.Instance.UpdateAction = UpdateAction.CheckForUpdates;
                    }

                    if (!Settings.Default.lastUpdateCheck.Date.Equals(DateTime.Now.Date))
                    {
                        Core.Instance.UpdateAction = UpdateAction.CheckForUpdates;
                    }
                }
                catch (Exception)
                {
                }
            }

            this.init = true;
        }

        /// <summary>Navigates to the Options page</summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        private void NavigateToOptions(object sender, MouseButtonEventArgs e)
        {
            App.NavService.Navigate(new Uri(@"/SevenUpdate;component/Pages/Options.xaml", UriKind.Relative));
        }

        /// <summary>Navigates to the Restore Updates page</summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        private void NavigateToRestoreUpdates(object sender, MouseButtonEventArgs e)
        {
            App.NavService.Navigate(new Uri(@"/SevenUpdate;component/Pages/RestoreUpdates.xaml", UriKind.Relative));
        }

        /// <summary>Navigates to the Update History page</summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        private void NavigateToUpdateHistory(object sender, MouseButtonEventArgs e)
        {
            App.NavService.Navigate(new Uri(@"/SevenUpdate;component/Pages/UpdateHistory.xaml", UriKind.Relative));
        }

        /// <summary>Shows the About Dialog window</summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        private void ShowAboutDialog(object sender, MouseButtonEventArgs e)
        {
            var about = new About();
            about.ShowDialog();
        }

        #endregion
    }
}