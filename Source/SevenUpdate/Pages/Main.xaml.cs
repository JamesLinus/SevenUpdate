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
using System.Diagnostics;
using System.Linq;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using SevenUpdate.Properties;
using SevenUpdate.Windows;

#endregion

namespace SevenUpdate.Pages
{
    /// <summary>
    ///   Interaction logic for Main.xaml
    /// </summary>
    public sealed partial class Main : Page
    {
        private Timer timer;

        /// <summary>
        ///   The constructor for the Main page
        /// </summary>
        public Main()
        {
            InitializeComponent();

            #region Event Handler Declarations

            UpdateInfo.UpdateSelectionChanged += UpdateSelectionChanged;
            RestoreUpdates.RestoredHiddenUpdate += RestoredHiddenUpdate;
            AdminClient.SettingsChanged += Admin_SettingsChanged;

            #endregion

            Core.Instance.UpdateAction = Base.RebootNeeded ? UpdateAction.RebootNeeded : UpdateAction.NoUpdates;
        }

        #region UI Events

        #region TextBlock

        /// <summary>
        ///   Navigates to the Options page
        /// </summary>
        private void ChangeSettings_MouseDown(object sender, MouseButtonEventArgs e)
        {
            MainWindow.NavService.Navigate(new Uri(@"Pages\Options.xaml", UriKind.Relative));
        }

        /// <summary>
        ///   Checks for updates
        /// </summary>
        private void CheckForUpdates_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Core.CheckForUpdates();
        }

        /// <summary>
        ///   Navigates to the Update History page
        /// </summary>
        private void ViewUpdateHistory_MouseDown(object sender, MouseButtonEventArgs e)
        {
            MainWindow.NavService.Navigate(new Uri(@"Pages\UpdateHistory.xaml", UriKind.Relative));
        }

        /// <summary>
        ///   Navigates to the Restore Updates page
        /// </summary>
        private void RestoreHiddenUpdates_MouseDown(object sender, MouseButtonEventArgs e)
        {
            MainWindow.NavService.Navigate(new Uri(@"Pages\RestoreUpdates.xaml", UriKind.Relative));
        }

        /// <summary>
        ///   Shows the About Dialog window
        /// </summary>
        private void About_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var about = new About();
            about.ShowDialog();
        }

        #endregion

        /// <summary>
        ///   Updates the UI after the user selects updates to install
        /// </summary>
        private void UpdateSelectionChanged(object sender, UpdateSelectionChangedEventArgs e)
        {
            if (Core.Applications.Count != 0)
                return;
            Core.CheckForUpdates();
        }

        /// <summary>
        ///   Checks for updates after hidden updates have been restored
        /// </summary>
        private void RestoredHiddenUpdate(object sender, EventArgs e)
        {
            Core.CheckForUpdates(true);
        }

        private void Admin_SettingsChanged(object sender, EventArgs e)
        {
            Core.CheckForUpdates(true);
        }


        #endregion

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (Core.IsReconnect)
            {
                Core.Instance.UpdateAction = UpdateAction.ConnectingToService;
                timer = new Timer {Enabled = true, Interval = 30000};
                timer.Elapsed += timer_Elapsed;
                AdminClient.Connect();
            }
            else if (Core.IsAutoCheck)
                Core.CheckForUpdates(true);
            else if (!Settings.Default.lastUpdateCheck.Date.Equals(DateTime.Now.Date))
                Core.Instance.UpdateAction = UpdateAction.CheckForUpdates;
        }

        void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            timer.Enabled = false;
            timer.Stop();
            if (Core.Instance.UpdateAction != UpdateAction.ConnectingToService)
                return;
            AdminClient.KillAdmin();
            Core.Instance.UpdateAction = UpdateAction.CheckForUpdates;
        }
    }
}