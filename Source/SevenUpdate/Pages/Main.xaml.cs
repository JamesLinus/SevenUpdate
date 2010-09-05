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
using System.Linq;
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
        /// <summary>
        ///   The constructor for the Main page
        /// </summary>
        public Main()
        {
            InitializeComponent();

            #region Event Handler Declarations

            UpdateInfo.CanceledSelection += CanceledSelection;
            UpdateInfo.UpdateSelectionChanged += UpdateSelectionChanged;
            infoBar.btnAction.Click += Action_Click;
            infoBar.lblViewImportantUpdates.MouseDown += ViewImportantUpdates_MouseDown;
            infoBar.lblViewOptionalUpdates.MouseDown += ViewOptionalUpdates_MouseDown;
            Search.SearchDone += SearchCompleted;
            Search.ErrorOccurred += ErrorOccurred;
            ServiceCallBack.DownloadProgressChanged += DownloadProgressChanged;
            ServiceCallBack.DownloadDone += DownloadCompleted;
            ServiceCallBack.InstallProgressChanged += InstallProgressChanged;
            ServiceCallBack.InstallDone += InstallCompleted;
            ServiceCallBack.ErrorOccurred += ErrorOccurred;
            RestoreUpdates.RestoredHiddenUpdate += RestoredHiddenUpdate;
            AdminClient.SettingsChanged += Admin_SettingsChanged;
            AdminClient.ServiceError += ErrorOccurred;

            #endregion

            LoadSettings();

            if (App.IsReconnect)
                infoBar.UiLayout = UILayout.ConnectingToService;
            else if (App.IsAutoCheck)
                CheckForUpdates(true);
            else if (!Settings.Default.lastUpdateCheck.Date.Equals(DateTime.Now.Date))
                infoBar.UiLayout = UILayout.CheckForUpdates;
        }

        #region Update Event Methods

        /// <summary>
        ///   Updates the UI when the installation progress has changed
        /// </summary>
        /// <param name = "e">The InstallProgress data</param>
        private void InstallProgressChanged(InstallProgressChangedEventArgs e)
        {
            if (App.IsReconnect)
            {
                infoBar.UiLayout = UILayout.Installing;
                App.IsReconnect = false;
            }

            //if (e.CurrentProgress == -1)
            //    infoBar.lblStatus.Text = Properties.Resources.PreparingInstall + "...";
            //else
            //{
            //    infoBar.lblStatus.Text = Properties.Resources.Installing + " " + e.UpdateName;

            //    if (e.TotalUpdates > 1)
            //        infoBar.lblStatus.Text += Environment.NewLine + e.UpdatesComplete + " " + Properties.Resources.OutOf + " " + e.TotalUpdates + ", " + e.CurrentProgress + "% " +
            //                                  Properties.Resources.Complete;
            //    else
            //        infoBar.lblStatus.Text += ", " + e.CurrentProgress + "% " + Properties.Resources.Complete;
            //}
        }

        /// <summary>
        ///   Updates the UI when the download progress has changed
        /// </summary>
        /// <param name = "e">The DownloadProgress data</param>
        private void DownloadProgressChanged(DownloadProgressChangedEventArgs e)
        {
            if (App.IsReconnect)
            {
                infoBar.UiLayout = UILayout.Downloading;
                App.IsReconnect = false;
            }
            //if (e.BytesTotal > 0 && e.BytesTransferred > 0)
            //{
            //    infoBar.lblStatus.Text = Properties.Resources.DownloadingUpdates + " (" + Base.ConvertFileSize(e.BytesTotal) + ", " + (e.BytesTransferred*100/e.BytesTotal).ToString("F0") + " % " +
            //                             Properties.Resources.Complete + ")";
            //}
            //else
            //{
            //    infoBar.lblStatus.Text = Properties.Resources.DownloadingUpdates + " (" + e.FilesTransferred + " " + Properties.Resources.OutOf + " " + e.FilesTotal + " " + Properties.Resources.Files +
            //                             " " + Properties.Resources.Complete + ")";
            //}
        }

        /// <summary>
        ///   Updates the UI when the installation has completed
        /// </summary>
        /// <param name = "e">The InstallCompleted data</param>
        private void InstallCompleted(InstallCompletedEventArgs e)
        {
            Settings.Default.lastInstall = DateTime.Now;
            lblUpdatesInstalled.Text = Properties.Resources.TodayAt + " " + DateTime.Now.ToShortTimeString();
            // if a reboot is needed lets say it
            if (!Base.RebootNeeded)
                infoBar.UiLayout = UILayout.InstallationCompleted; // , e.UpdatesInstalled, e.UpdatesFailed);
            else
                infoBar.UiLayout = UILayout.RebootNeeded;
        }

        /// <summary>
        ///   Updates the UI when the downloading of updates has completed
        /// </summary>
        /// <param name = "e">The DownloadCompleted data</param>
        private void DownloadCompleted(DownloadCompletedEventArgs e)
        {
            if (e.ErrorOccurred)
                infoBar.UiLayout = UILayout.ErrorOccurred;
            else
                infoBar.UiLayout = App.IsAutoCheck ? UILayout.DownloadCompleted : UILayout.Installing;
        }

        /// <summary>
        ///   Updates the UI the search for updates has completed
        /// </summary>
        /// <param name = "e">The SearchComplete data</param>
        private void SearchCompleted(SearchCompletedEventArgs e)
        {
            if (e.Applications.Count > 0)
            {
                App.Applications = e.Applications;

                var count = new int[2];

                foreach (Update t1 in e.Applications.SelectMany(t => t.Updates))
                {
                    switch (t1.Importance)
                    {
                        case Importance.Important:
                            count[0]++;
                            break;
                        case Importance.Locale:
                        case Importance.Optional:

                            count[1]++;
                            break;
                        case Importance.Recommended:
                            if (App.Settings.IncludeRecommended)
                                count[0]++;
                            else
                                count[1]++;
                            break;
                    }
                }

                #region GUI Updating

                if (count[0] > 0 || count[1] > 0)
                {
                    infoBar.UiLayout = UILayout.UpdatesFound;

                    if (count[0] > 0 && count[1] > 0)
                        infoBar.line.Y1 = 50;

                    infoBar.lblSelectedUpdates.Text = Properties.Resources.NoUpdatesSelected;

                    if (count[0] > 0)
                    {
                        if (count[0] == 1)
                            infoBar.lblViewImportantUpdates.Text = count[0] + " " + Properties.Resources.ImportantUpdateAvailable + " ";
                        else
                            infoBar.lblViewImportantUpdates.Text = count[0] + " " + Properties.Resources.ImportantUpdatesAvailable;
                    }
                    else
                        infoBar.lblViewImportantUpdates.Visibility = Visibility.Collapsed;

                    if (count[1] > 0)
                    {
                        if (count[0] == 0)
                        {
                            //infoBar.imgSideBanner.Source = null;
                            //infoBar.imgSideBanner.Source = (BitmapImage) App.Resources["GreenSide"];
                            //infoBar.imgShieldIcon.Source = (BitmapImage) App.Resources["GreenShield"];
                            infoBar.lblHeading.Text = Properties.Resources.NoImportantUpdates;
                        }

                        if (count[1] == 1)
                            infoBar.lblViewOptionalUpdates.Text = count[1] + " " + Properties.Resources.OptionalUpdateAvailable;
                        else
                            infoBar.lblViewOptionalUpdates.Text = count[1] + " " + Properties.Resources.OptionalUpdatesAvailable;

                        infoBar.lblViewOptionalUpdates.Visibility = Visibility.Visible;
                    }
                    else
                        infoBar.lblViewOptionalUpdates.Visibility = Visibility.Collapsed;
                }
                //End Code

                #endregion
            }
            else
            {
                infoBar.UiLayout = UILayout.NoUpdates;
            }
        }

        /// <summary>
        ///   Sets the UI when an error occurs
        /// </summary>
        private void ErrorOccurred(object sender, ErrorOccurredEventArgs e)
        {
            switch (e.Type)
            {
                case ErrorType.FatalNetworkError:
                    infoBar.UiLayout = UILayout.ErrorOccurred;//, Properties.Resources.CheckConnection);

                    break;
                case ErrorType.InstallationError:
                    infoBar.UiLayout = UILayout.ErrorOccurred;//, e.Exception);
                    break;


                case ErrorType.SearchError:
                    break;
                case ErrorType.DownloadError:
                        infoBar.UiLayout = UILayout.ErrorOccurred;//, e.Exception);
                    break;
                case ErrorType.GeneralErrorNonFatal:

                    break;
                case ErrorType.FatalError:
                        infoBar.UiLayout = UILayout.ErrorOccurred;//, e.Exception);

                    break;
            }
        }

        #region Invoker Events

        /// <summary>
        ///   Sets the UI when the install progress has changed
        /// </summary>
        private void InstallProgressChanged(object sender, InstallProgressChangedEventArgs e)
        {
            if (!Dispatcher.CheckAccess())
                Dispatcher.BeginInvoke(InstallProgressChanged, e);
            else
                InstallProgressChanged(e);
        }

        /// <summary>
        ///   Sets the UI when the download progress has changed
        /// </summary>
        private void DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            if (!Dispatcher.CheckAccess())
                Dispatcher.BeginInvoke(DownloadProgressChanged, e);
            else
                DownloadProgressChanged(e);
        }

        /// <summary>
        ///   Sets the UI when the search for updates has completed
        /// </summary>
        private void SearchCompleted(object sender, SearchCompletedEventArgs e)
        {
            if (!Dispatcher.CheckAccess())
                Dispatcher.BeginInvoke(SearchCompleted, e);
            else
                SearchCompleted(e);
        }

        /// <summary>
        ///   Sets the UI when the installation of updates has completed
        /// </summary>
        private void InstallCompleted(object sender, InstallCompletedEventArgs e)
        {
            if (!Dispatcher.CheckAccess())
                Dispatcher.BeginInvoke(InstallCompleted, e);
            else
                InstallCompleted(e);
        }

        /// <summary>
        ///   Sets the UI when the downloading of updates has completed
        /// </summary>
        private void DownloadCompleted(object sender, DownloadCompletedEventArgs e)
        {
            if (!Dispatcher.CheckAccess())
                Dispatcher.BeginInvoke(DownloadCompleted, e);
            else
                DownloadCompleted(e);
        }

        #endregion

        #endregion

        #region Update Methods

        /// <summary>
        ///   Checks for updates
        /// </summary>
        /// <param name = "auto"><c>true</c> if it's called because of an auto update check, otherwise <c>false</c></param>
        private void CheckForUpdates(bool auto)
        {
            if (auto)
            {
                if (!App.IsInstallInProgress && !Base.RebootNeeded)
                    CheckForUpdates();
            }
            else
                CheckForUpdates();
        }

        /// <summary>
        ///   Checks for updates
        /// </summary>
        private void CheckForUpdates()
        {
            if (!App.IsInstallInProgress)
            {
                if (Base.RebootNeeded == false)
                {
                    infoBar.UiLayout = UILayout.CheckingForUpdates;
                    Settings.Default.lastUpdateCheck = DateTime.Now;
                    Search.SearchForUpdatesAync(App.AppsToUpdate);
                }
                else
                {
                    infoBar.UiLayout = UILayout.RebootNeeded;
                    MessageBox.Show(Properties.Resources.RebootNeededFirst, Properties.Resources.SevenUpdate, MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            else
                MessageBox.Show(Properties.Resources.AlreadyUpdating, Properties.Resources.SevenUpdate, MessageBoxButton.OK, MessageBoxImage.Information);
        }

        /// <summary>
        ///   Downloads updates
        /// </summary>
        private void DownloadInstallUpdates()
        {
            for (var x = 0; x < App.Applications.Count; x++)
            {
                for (var y = 0; y < App.Applications[x].Updates.Count; y++)
                {
                    if (App.Applications[x].Updates[y].Selected)
                        continue;
                    App.Applications[x].Updates.RemoveAt(y);
                    y--;
                }
                if (App.Applications[x].Updates.Count != 0)
                    continue;
                App.Applications.RemoveAt(x);
                x--;
            }

            if (App.Applications.Count > 0)
            {
                var sla = new LicenseAgreement();
                if (sla.LoadLicenses() == false)
                {
                    infoBar.UiLayout = UILayout.Canceled;
                    return;
                }

                if (AdminClient.Install())
                    infoBar.UiLayout = infoBar.lblHeading.Text == Properties.Resources.DownloadAndInstallUpdates ? UILayout.Downloading : UILayout.Installing;
                else
                    infoBar.UiLayout = UILayout.Canceled;
            }
            else
                infoBar.UiLayout = UILayout.Canceled;
        }

        #endregion

        #region UI Methods

        /// <summary>
        ///   Loads the application settings and initializes event handlers for the pages
        /// </summary>
        private void LoadSettings()
        {
            if (Settings.Default.lastUpdateCheck != DateTime.MinValue)
            {
                if (Settings.Default.lastUpdateCheck.Date.Equals(DateTime.Now.Date))
                    lblRecentCheck.Text = Properties.Resources.TodayAt + " " + Settings.Default.lastUpdateCheck.ToShortTimeString();
                else
                    lblRecentCheck.Text = Settings.Default.lastUpdateCheck.ToShortDateString() + " " + Properties.Resources.At + " " + Settings.Default.lastUpdateCheck.ToShortTimeString();
            }
            else
                lblRecentCheck.Text = Properties.Resources.Never;

            if (Settings.Default.lastInstall != DateTime.MinValue)
            {
                if (Settings.Default.lastInstall.Equals(DateTime.Now))
                    lblUpdatesInstalled.Text = Properties.Resources.TodayAt + " " + Settings.Default.lastInstall.ToShortTimeString();
                else
                    lblUpdatesInstalled.Text = Settings.Default.lastInstall.ToShortDateString() + " " + Properties.Resources.At + " " + Settings.Default.lastInstall.ToShortTimeString();
            }
            else
                lblUpdatesInstalled.Text = Properties.Resources.Never;

            infoBar.UiLayout = Base.RebootNeeded ? UILayout.RebootNeeded : UILayout.NoUpdates;
        }

        #endregion

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
            CheckForUpdates();
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

        /// <summary>
        ///   Navigates to the Update Info page
        /// </summary>
        private static void ViewOptionalUpdates_MouseDown(object sender, MouseButtonEventArgs e)
        {
            UpdateInfo.DisplayOptionalUpdates = true;
            MainWindow.NavService.Navigate(new Uri(@"Pages\UpdateInfo.xaml", UriKind.Relative));
        }

        /// <summary>
        ///   Navigates to the Update Info page
        /// </summary>
        private static void ViewImportantUpdates_MouseDown(object sender, MouseButtonEventArgs e)
        {
            UpdateInfo.DisplayOptionalUpdates = false;
            MainWindow.NavService.Navigate(new Uri(@"Pages\UpdateInfo.xaml", UriKind.Relative));
        }

        #endregion

        /// <summary>
        ///   When the user cancels their selection of updates and also hides at least one update, let's re-check for updates
        /// </summary>
        private void CanceledSelection(object sender, EventArgs e)
        {
            CheckForUpdates();
        }

        /// <summary>
        ///   Updates the UI after the user selects updates to install
        /// </summary>
        private void UpdateSelectionChanged(object sender, UpdateSelectionChangedEventArgs e)
        {
            if (App.Applications.Count == 0)
            {
                CheckForUpdates();
                return;
            }

            infoBar.lblViewOptionalUpdates.Visibility = Visibility.Collapsed;
            infoBar.lblViewImportantUpdates.Visibility = Visibility.Collapsed;

            foreach (Update t1 in
                App.Applications.TakeWhile(t => infoBar.lblViewImportantUpdates.Visibility != Visibility.Visible || infoBar.lblViewOptionalUpdates.Visibility != Visibility.Visible).SelectMany(
                    t => t.Updates.TakeWhile(t1 => infoBar.lblViewImportantUpdates.Visibility != Visibility.Visible || infoBar.lblViewOptionalUpdates.Visibility != Visibility.Visible)))
            {
                switch (t1.Importance)
                {
                    case Importance.Important:
                        infoBar.lblViewImportantUpdates.Visibility = Visibility.Visible;
                        break;
                    case Importance.Locale:
                    case Importance.Optional:
                        infoBar.lblViewOptionalUpdates.Visibility = Visibility.Visible;
                        break;
                    case Importance.Recommended:
                        if (App.Settings.IncludeRecommended)
                            infoBar.lblViewImportantUpdates.Visibility = Visibility.Visible;
                        else
                            infoBar.lblViewOptionalUpdates.Visibility = Visibility.Visible;
                        break;
                }
            }

            if (infoBar.lblViewOptionalUpdates.Visibility == Visibility.Collapsed || infoBar.lblViewImportantUpdates.Visibility == Visibility.Collapsed ||
                infoBar.spnlUpdateInfo.Visibility == Visibility.Collapsed)
                infoBar.line.Y1 = 25;
            else
                infoBar.line.Y1 = 50;

            #region GUI Updating

            if (e.ImportantUpdates > 0)
            {
                if (e.ImportantUpdates == 1)
                    infoBar.lblSelectedUpdates.Text = e.ImportantUpdates + " " + Properties.Resources.ImportantUpdateSelected;
                else
                    infoBar.lblSelectedUpdates.Text = e.ImportantUpdates + " " + Properties.Resources.ImportantUpdatesSelected;

                if (e.ImportantDownloadSize > 0)
                    infoBar.lblSelectedUpdates.Text += ", " + Base.ConvertFileSize(e.ImportantDownloadSize);
            }

            if (e.OptionalUpdates > 0)
            {
                if (e.ImportantUpdates == 0)
                {
                    if (e.OptionalUpdates == 1)
                        infoBar.lblSelectedUpdates.Text = e.OptionalUpdates + " " + Properties.Resources.OptionalUpdateSelected;
                    else
                        infoBar.lblSelectedUpdates.Text = e.OptionalUpdates + " " + Properties.Resources.OptionalUpdatesSelected;
                }
                else
                {
                    if (e.OptionalUpdates == 1)
                        infoBar.lblSelectedUpdates.Text += Environment.NewLine + e.OptionalUpdates + " " + Properties.Resources.OptionalUpdateSelected;
                    else
                        infoBar.lblSelectedUpdates.Text += Environment.NewLine + e.OptionalUpdates + " " + Properties.Resources.OptionalUpdatesSelected;
                }

                if (e.OptionalDownloadSize > 0)
                    infoBar.lblSelectedUpdates.Text += ", " + Base.ConvertFileSize(e.OptionalDownloadSize);
            }

            if (e.ImportantDownloadSize == 0 && e.OptionalDownloadSize == 0)
                infoBar.lblHeading.Text = Properties.Resources.InstallUpdatesForPrograms;
            else
                infoBar.lblHeading.Text = Properties.Resources.DownloadAndInstallUpdates;

            if (e.ImportantUpdates > 0 || e.OptionalUpdates > 0)
            {
                infoBar.btnAction.Visibility = Visibility.Visible;
                infoBar.lblSelectedUpdates.FontWeight = FontWeights.Bold;
            }
            else
            {
                //infoBar.imgSideBanner.Source = (BitmapImage) App.Resources["YellowSide"];

                infoBar.lblSelectedUpdates.Text = Properties.Resources.NoUpdatesSelected;
                infoBar.lblSelectedUpdates.Height = Double.NaN;
                infoBar.lblSelectedUpdates.FontWeight = FontWeights.Normal;
                infoBar.btnAction.Visibility = Visibility.Collapsed;
            }

            #endregion
        }

        /// <summary>
        ///   Checks for updates after hidden updates have been restored
        /// </summary>
        private void RestoredHiddenUpdate(object sender, EventArgs e)
        {
            CheckForUpdates(true);
        }

        private void Admin_SettingsChanged(object sender, EventArgs e)
        {
            CheckForUpdates(true);
        }

        /// <summary>
        ///   Executes action based on current label. Installed, cancels, and/or searches for updates. it also can reboot the computer.
        /// </summary>
        private void Action_Click(object sender, RoutedEventArgs e)
        {
            if (infoBar.btnAction.ButtonText == Properties.Resources.InstallUpdates)
                DownloadInstallUpdates();
            else if (infoBar.btnAction.ButtonText == Properties.Resources.StopDownload || infoBar.btnAction.ButtonText == Properties.Resources.StopInstallation)
            {
                //Cancel installation of updates
                if (AdminClient.AbortInstall())
                    infoBar.UiLayout = UILayout.Canceled;
                return;
            }
            else if (infoBar.btnAction.ButtonText == Properties.Resources.TryAgain || infoBar.btnAction.ButtonText == Properties.Resources.CheckForUpdates)
                CheckForUpdates();
            else if (infoBar.btnAction.ButtonText == Properties.Resources.RestartNow)
                Base.StartProcess("shutdown.exe", "-r -t 00");
        }

        #endregion
    }
}