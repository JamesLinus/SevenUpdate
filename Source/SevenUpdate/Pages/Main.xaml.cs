// ***********************************************************************
// Assembly         : SevenUpdate
// Author           : sevenalive
// Created          : 09-17-2010
//
// Last Modified By : sevenalive
// Last Modified On : 10-05-2010
// Description      : 
//
// Copyright        : (c) Seven Software. All rights reserved.
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
    using SevenUpdate.Windows;

    /// <summary>
    /// Interaction logic for Main.xaml
    /// </summary>
    public sealed partial class Main
    {
        #region Constants and Fields

        /// <summary>
        /// </summary>
        private bool init;

        /// <summary>
        /// </summary>
        private Timer timer;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///   The constructor for the Main page
        /// </summary>
        public Main()
        {
            this.InitializeComponent();

            RestoreUpdates.RestoredHiddenUpdate += Settings_Changed;
            AdminClient.SettingsChanged += Settings_Changed;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Checks for updates after settings were changed
        /// </summary>
        /// <param name="sender">
        /// </param>
        /// <param name="e">
        /// </param>
        private static void Settings_Changed(object sender, EventArgs e)
        {
            Core.CheckForUpdates(true);
        }

        /// <summary>
        /// Shows the About Dialog window
        /// </summary>
        /// <param name="sender">
        /// </param>
        /// <param name="e">
        /// </param>
        private void About_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var about = new About();
            about.ShowDialog();
        }

        /// <summary>
        /// Navigates to the Options page
        /// </summary>
        /// <param name="sender">
        /// </param>
        /// <param name="e">
        /// </param>
        private void ChangeSettings_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Core.NavService.Navigate(new Uri(@"/SevenUpdate;component/Pages/Options.xaml", UriKind.Relative));
        }

        /// <summary>
        /// Checks for updates
        /// </summary>
        /// <param name="sender">
        /// </param>
        /// <param name="e">
        /// </param>
        private void CheckForUpdates_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Core.CheckForUpdates();
        }

        /// <summary>
        /// </summary>
        /// <param name="sender">
        /// </param>
        /// <param name="e">
        /// </param>
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (Base.RebootNeeded)
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
                this.timer = new Timer { Enabled = true, Interval = 30000 };
                this.timer.Elapsed += this.timer_Elapsed;
                AdminClient.Connect();
            }
            else if (File.Exists(Base.AllUserStore + @"updates.sui"))
            {
                var lastCheck = File.GetLastWriteTime(Base.AllUserStore + @"updates.sui");

                var today = DateTime.Now;

                if (lastCheck.Month == today.Month && lastCheck.Year == today.Year)
                {
                    if (lastCheck.Day == today.Day || lastCheck.Day + 1 == today.Day || lastCheck.Day + 2 == today.Day || lastCheck.Day + 3 == today.Day ||
                        lastCheck.Day + 4 == today.Day || lastCheck.Day + 5 == today.Day)
                    {
                        AdminClient.Disconnect();
                        Task.Factory.StartNew(() => Search.SetUpdatesFound(Base.Deserialize<Collection<Sui>>(Base.AllUserStore + @"updates.sui")));
                    }
                }
                else
                {
                    try
                    {
                        File.Delete(Base.AllUserStore + @"updates.sui");
                    }
                    catch
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
                catch
                {
                }
            }

            this.init = true;
        }

        /// <summary>
        /// Navigates to the Restore Updates page
        /// </summary>
        /// <param name="sender">
        /// </param>
        /// <param name="e">
        /// </param>
        private void RestoreHiddenUpdates_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Core.NavService.Navigate(new Uri(@"/SevenUpdate;component/Pages/RestoreUpdates.xaml", UriKind.Relative));
        }

        /// <summary>
        /// Navigates to the Update History page
        /// </summary>
        /// <param name="sender">
        /// </param>
        /// <param name="e">
        /// </param>
        private void ViewUpdateHistory_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Core.NavService.Navigate(new Uri(@"/SevenUpdate;component/Pages/UpdateHistory.xaml", UriKind.Relative));
        }

        /// <summary>
        /// </summary>
        /// <param name="sender">
        /// </param>
        /// <param name="e">
        /// </param>
        private void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            this.timer.Enabled = false;
            this.timer.Stop();
            if (Core.Instance.UpdateAction != UpdateAction.ConnectingToService)
            {
                return;
            }

            AdminClient.AdminError(new Exception(Properties.Resources.CouldNotConnectService));
        }

        #endregion
    }
}