#region GNU Public License v3

// Copyright 2007-2010 Robert Baker, aka Seven ALive.
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
//    along with Seven Update.  If not, see <http://www.gnu.org/licenses/>.

#endregion

#region

using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using SevenUpdate.Base;
using SevenUpdate.Properties;
using SevenUpdate.Windows;

#endregion

namespace SevenUpdate.Pages
{
    /// <summary>
    /// Interaction logic for Main.xaml
    /// </summary>
    public sealed partial class Main : Page
    {
        #region Global Vars

        /// <summary>
        /// Gets a green side image
        /// </summary>
        private readonly BitmapImage greenSide = new BitmapImage(new Uri("/Images/GreenSide.png", UriKind.Relative));

        /// <summary>
        /// Gets a red side image
        /// </summary>
        private readonly BitmapImage redSide = new BitmapImage(new Uri("/Images/RedSide.png", UriKind.Relative));

        /// <summary>
        /// Gets the Seven Update icon
        /// </summary>
        private readonly BitmapImage suIcon = new BitmapImage(new Uri("/Images/Icon.png", UriKind.Relative));

        /// <summary>
        /// Gets a yellow side image
        /// </summary>
        private readonly BitmapImage yellowSide = new BitmapImage(new Uri("/Images/YellowSide.png", UriKind.Relative));

        #endregion

        #region Enums

        /// <summary>
        /// The layout for the Info Panel
        /// </summary>
        private enum UILayout
        {
            /// <summary>Canceled Updates</summary>
            Canceled,

            /// <summary>
            /// Checking for updates
            /// </summary>
            CheckingForUpdates,

            /// <summary>
            /// When connecting to the admin service
            /// </summary>
            ConnectingToService,

            /// <summary>
            /// When downloading of updates has been completed
            /// </summary>
            DownloadCompleted,

            /// <summary>
            /// Downloading updates
            /// </summary>
            Downloading,

            /// <summary>
            /// An Error Occurred when downloading/installing updates
            /// </summary>
            ErrorOccurred,

            /// <summary>
            /// When installation of updates have completed
            /// </summary>
            InstallationCompleted,

            /// <summary>
            /// Installing Updates
            /// </summary>
            Installing,

            /// <summary>
            /// No updates have been found
            /// </summary>
            NoUpdates,

            /// <summary>
            /// A reboot is needed to finish installing updates
            /// </summary>
            RebootNeeded,

            /// <summary>
            /// No updates have been found
            /// </summary>
            UpdatesFound,
        }

        #endregion

        /// <summary>
        /// The constructor for the Main page
        /// </summary>
        public Main()
        {
            InitializeComponent();
            LoadSettings();
            infoBar.imgAdminShield.Visibility = App.IsAdmin ? Visibility.Collapsed : Visibility.Visible;

            if (App.IsReconnect)
                SetUI(UILayout.ConnectingToService);

            if (App.IsAutoCheck)
                CheckForUpdates(true);
            else if (!Settings.Default.lastUpdateCheck.Date.Equals(DateTime.Now.Date))
                CheckForUpdates();
        }

        #region Update Event Methods

        /// <summary>
        /// Updates the UI when the installation progress has changed
        /// </summary>
        /// <param name="e">The InstallProgress data</param>
        private void InstallProgressChanged(InstallProgressChangedEventArgs e)
        {
            if (App.IsReconnect)
            {
                SetUI(UILayout.Installing);
                App.IsReconnect = false;
            }

            if (e.CurrentProgress == -1)
                infoBar.tbStatus.Text = App.RM.GetString("PreparingInstall") + "...";
            else
            {
                infoBar.tbStatus.Text = App.RM.GetString("Installing") + " " + e.UpdateName;

                if (e.TotalUpdates > 1)
                    infoBar.tbStatus.Text += Environment.NewLine + e.UpdatesComplete + " " + App.RM.GetString("OutOf") + " " + e.TotalUpdates + ", " + e.CurrentProgress + "% " +
                                             App.RM.GetString("Complete");
                else
                    infoBar.tbStatus.Text += ", " + e.CurrentProgress + "% " + App.RM.GetString("Complete");
            }
        }

        /// <summary>
        /// Updates the UI when the download progress has changed
        /// </summary>
        /// <param name="e">The DownloadProgress data</param>
        private void DownloadProgressChanged(DownloadProgressChangedEventArgs e)
        {
            if (App.IsReconnect)
            {
                SetUI(UILayout.Downloading);
                App.IsReconnect = false;
            }
            if (e.BytesTotal > 0 && e.BytesTransferred > 0)
                infoBar.tbStatus.Text = App.RM.GetString("DownloadingUpdates") + " (" + Base.Base.ConvertFileSize(e.BytesTotal) + ", " + (e.BytesTransferred * 100 / e.BytesTotal).ToString("F0") +
                                        " % " + App.RM.GetString("Complete") + ")";
            else
            {
                infoBar.tbStatus.Text = App.RM.GetString("DownloadingUpdates") + " (" + e.FilesTransferred + " " + App.RM.GetString("OutOf") + " " + e.FilesTotal + " " + App.RM.GetString("Files") +
                                        " " + App.RM.GetString("Complete") + ")";
            }
        }

        /// <summary>
        /// Updates the UI when the installation has completed
        /// </summary>
        /// <param name="e">The InstallCompleted data</param>
        private void InstallCompleted(InstallCompletedEventArgs e)
        {
            Settings.Default.lastInstall = DateTime.Now;
            tbUpdatesInstalled.Text = App.RM.GetString("TodayAt") + " " + DateTime.Now.ToShortTimeString();
            // if a reboot is needed lets say it
            if (!Base.Base.RebootNeeded)
                SetUI(UILayout.InstallationCompleted, e.UpdatesInstalled, e.UpdatesFailed);
            else
                SetUI(UILayout.RebootNeeded);
        }

        /// <summary>
        /// Updates the UI when the downloading of updates has completed
        /// </summary>
        /// <param name="e">The DownloadCompleted data</param>
        private void DownloadCompleted(DownloadCompletedEventArgs e)
        {
            if (e.ErrorOccurred)
                SetUI(UILayout.ErrorOccurred);
            else
            {
                SetUI(App.IsAutoCheck ? UILayout.DownloadCompleted : UILayout.Installing);
            }
        }

        /// <summary>
        /// Updates the UI the search for updates has completed
        /// </summary>
        /// <param name="e">The SearchComplete data</param>
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
                    SetUI(UILayout.UpdatesFound);

                    if (count[0] > 0 && count[1] > 0)
                        infoBar.line.Y1 = 50;

                    infoBar.tbSelectedUpdates.Text = App.RM.GetString("NoUpdatesSelected");

                    if (count[0] > 0)
                    {
                        if (count[0] == 1)
                            infoBar.tbViewImportantUpdates.Text = count[0] + " " + App.RM.GetString("ImportantUpdateAvailable") + " ";
                        else
                            infoBar.tbViewImportantUpdates.Text = count[0] + " " + App.RM.GetString("ImportantUpdatesAvailable");
                    }
                    else
                        infoBar.tbViewImportantUpdates.Visibility = Visibility.Collapsed;

                    if (count[1] > 0)
                    {
                        if (count[0] == 0)
                        {
                            infoBar.imgSide.Source = greenSide;
                            infoBar.imgShield.Source = App.GreenShield;
                            infoBar.tbHeading.Text = App.RM.GetString("NoImportantUpdates");
                        }

                        if (count[1] == 1)
                            infoBar.tbViewOptionalUpdates.Text = count[1] + " " + App.RM.GetString("OptionalUpdateAvailable");
                        else
                            infoBar.tbViewOptionalUpdates.Text = count[1] + " " + App.RM.GetString("OptionalUpdatesAvailable");

                        infoBar.tbViewOptionalUpdates.Visibility = Visibility.Visible;
                    }
                    else
                        infoBar.tbViewOptionalUpdates.Visibility = Visibility.Collapsed;
                }
                //End Code

                #endregion
            }
            else
            {
                if (!Dispatcher.CheckAccess())
                    Dispatcher.BeginInvoke(SetUI, UILayout.NoUpdates);
                else
                    SetUI(UILayout.NoUpdates);
            }
        }

        /// <summary>
        /// Sets the UI when an error occurs
        /// </summary>
        private void ErrorOccurredEventHandler(object sender, ErrorOccurredEventArgs e)
        {
            switch (e.Type)
            {
                case ErrorType.FatalNetworkError:
                    if (!Dispatcher.CheckAccess())
                        Dispatcher.BeginInvoke(SetUI, UILayout.ErrorOccurred, App.RM.GetString("CheckConnection"));
                    else
                        SetUI(UILayout.ErrorOccurred, App.RM.GetString("CheckConnection"));

                    break;
                case ErrorType.InstallationError:
                    if (!Dispatcher.CheckAccess())
                        Dispatcher.BeginInvoke(SetUI, UILayout.ErrorOccurred, e.Exception.Message);
                    else
                        SetUI(UILayout.ErrorOccurred, e.Exception.Message);
                    break;


                case ErrorType.SearchError:
                    break;
                case ErrorType.DownloadError:
                    if (!Dispatcher.CheckAccess())
                        Dispatcher.BeginInvoke(SetUI, UILayout.ErrorOccurred, e.Exception.Message);
                    else
                        SetUI(UILayout.ErrorOccurred, e.Exception.Message);
                    break;
                case ErrorType.GeneralErrorNonFatal:

                    break;
                case ErrorType.FatalError:
                    if (!Dispatcher.CheckAccess())
                        Dispatcher.BeginInvoke(SetUI, UILayout.ErrorOccurred, e.Exception.Message);
                    else
                        SetUI(UILayout.ErrorOccurred, e.Exception.Message);

                    break;
            }
        }

        #region Invoker Events

        /// <summary>
        /// Sets the UI when the install progress has changed
        /// </summary>
        private void InstallProgressChangedEventHandler(object sender, InstallProgressChangedEventArgs e)
        {
            if (!Dispatcher.CheckAccess())
                Dispatcher.BeginInvoke(InstallProgressChanged, e);
            else
                InstallProgressChanged(e);
        }

        /// <summary>
        /// Sets the UI when the download progress has changed
        /// </summary>
        private void DownloadProgressChangedEventHandler(object sender, DownloadProgressChangedEventArgs e)
        {
            if (!Dispatcher.CheckAccess())
                Dispatcher.BeginInvoke(DownloadProgressChanged, e);
            else
                DownloadProgressChanged(e);
        }

        /// <summary>
        /// Sets the UI when the search for updates has completed
        /// </summary>
        private void SearchCompletedEventHandler(object sender, SearchCompletedEventArgs e)
        {
            if (!Dispatcher.CheckAccess())
                Dispatcher.BeginInvoke(SearchCompleted, e);
            else
                SearchCompleted(e);
        }

        /// <summary>
        /// Sets the UI when the installation of updates has completed
        /// </summary>
        private void InstallCompletedEventHandler(object sender, InstallCompletedEventArgs e)
        {
            if (!Dispatcher.CheckAccess())
                Dispatcher.BeginInvoke(InstallCompleted, e);
            else
                InstallCompleted(e);
        }

        /// <summary>
        /// Sets the UI when the downloading of updates has completed
        /// </summary>
        private void DownloadCompletedEventHandler(object sender, DownloadCompletedEventArgs e)
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
        /// Checks for updates
        /// </summary>
        /// <param name="auto"><c>true</c> if it's called because of an auto update check, otherwise <c>false</c></param>
        private void CheckForUpdates(bool auto)
        {
            if (auto)
            {
                if (!App.IsInstallInProgress && !Base.Base.RebootNeeded)
                    CheckForUpdates();
            }
            else
                CheckForUpdates();
        }

        /// <summary>
        /// Checks for updates
        /// </summary>
        private void CheckForUpdates()
        {
            if (!App.IsInstallInProgress)
            {
                if (Base.Base.RebootNeeded == false)
                {
                    SetUI(UILayout.CheckingForUpdates);
                    Settings.Default.lastUpdateCheck = DateTime.Now;
                    Search.SearchForUpdatesAync(App.AppsToUpdate);
                }
                else
                {
                    SetUI(UILayout.RebootNeeded);
                    MessageBox.Show(App.RM.GetString("RebootNeededFirst"), App.RM.GetString("SevenUpdate"), MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            else
                MessageBox.Show(App.RM.GetString("AlreadyUpdating"), App.RM.GetString("SevenUpdate"), MessageBoxButton.OK, MessageBoxImage.Information);
        }

        /// <summary>
        /// Downloads updates
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
                    SetUI(UILayout.Canceled);
                    return;
                }

                if (Admin.Install())
                {
                    Admin.Connect();
                    SetUI(infoBar.tbHeading.Text == App.RM.GetString("DownloadAndInstallUpdates") ? UILayout.Downloading : UILayout.Installing);
                }
                else
                    SetUI(UILayout.Canceled);
            }
            else
                SetUI(UILayout.Canceled);
        }

        #endregion

        #region UI Methods

        /// <summary>
        /// Loads the application settings and initializes event handlers for the pages
        /// </summary>
        private void LoadSettings()
        {
            if (Settings.Default.lastUpdateCheck != DateTime.MinValue)
            {
                if (Settings.Default.lastUpdateCheck.Date.Equals(DateTime.Now.Date))
                    tbRecentCheck.Text = App.RM.GetString("TodayAt") + " " + Settings.Default.lastUpdateCheck.ToShortTimeString();
                else
                    tbRecentCheck.Text = Settings.Default.lastUpdateCheck.ToShortDateString() + " " + App.RM.GetString("At") + " " + Settings.Default.lastUpdateCheck.ToShortTimeString();
            }
            else
                tbRecentCheck.Text = App.RM.GetString("Never");

            if (Settings.Default.lastInstall != DateTime.MinValue)
            {
                if (Settings.Default.lastInstall.Equals(DateTime.Now))
                    tbUpdatesInstalled.Text = App.RM.GetString("TodayAt") + " " + Settings.Default.lastInstall.ToShortTimeString();
                else
                    tbUpdatesInstalled.Text = Settings.Default.lastInstall.ToShortDateString() + " " + App.RM.GetString("At") + " " + Settings.Default.lastInstall.ToShortTimeString();
            }
            else
                tbUpdatesInstalled.Text = App.RM.GetString("Never");

            SetUI(Base.Base.RebootNeeded ? UILayout.RebootNeeded : UILayout.NoUpdates);

            #region Event Handler Declarations

            UpdateInfo.CanceledSelectionEventHandler += CanceledSelectionEventHandler;
            UpdateInfo.UpdateSelectionChangedEventHandler += UpdateSelectionChangedEventHandler;
            infoBar.btnAction.Click += btnAction_Click;
            infoBar.tbViewImportantUpdates.MouseDown += tbViewImportantUpdates_MouseDown;
            infoBar.tbViewOptionalUpdates.MouseDown += tbViewOptionalUpdates_MouseDown;
            Search.SearchDoneEventHandler += SearchCompletedEventHandler;
            Search.ErrorOccurredEventHandler += ErrorOccurredEventHandler;
            AdminCallBack.DownloadProgressChangedEventHandler += DownloadProgressChangedEventHandler;
            AdminCallBack.DownloadDoneEventHandler += DownloadCompletedEventHandler;
            AdminCallBack.InstallProgressChangedEventHandler += InstallProgressChangedEventHandler;
            AdminCallBack.InstallDoneEventHandler += InstallCompletedEventHandler;
            AdminCallBack.ErrorOccurredEventHandler += ErrorOccurredEventHandler;
            RestoreUpdates.RestoredHiddenUpdateEventHandler += RestoredHiddenUpdateEventHandler;
            Admin.SettingsChangedEventHandler += Admin_SettingsChangedEventHandler;
            Admin.ServiceErrorEventHandler += ErrorOccurredEventHandler;

            #endregion
        }

        /// <summary>
        /// Sets the Main Page UI
        /// </summary>
        /// <param name="layout">type of layout to set</param>
        private void SetUI(UILayout layout)
        {
            SetUI(layout, null, 0, 0);
        }

        /// <summary>
        /// Sets the Main Page and <see cref="InfoBar" /> UI
        /// </summary>
        /// <param name="layout">The <see cref="UILayout" /> to set the UI to</param>
        /// <param name="errorDescription">The description of the error that occurred</param>
        private void SetUI(UILayout layout, string errorDescription)
        {
            SetUI(layout, errorDescription, 0, 0);
        }

        /// <summary>
        /// Sets the Main Page and <see cref="InfoBar" /> UI
        /// </summary>
        /// <param name="layout">The <see cref="UILayout" /> to set the UI to</param>
        /// <param name="updatesInstalled">The number of updates installed</param>
        /// <param name="updatesFailed">The number of updates failed</param>
        private void SetUI(UILayout layout, int updatesInstalled, int updatesFailed)
        {
            SetUI(layout, null, updatesInstalled, updatesFailed);
        }

        /// <summary>
        /// Sets the Main Page and <see cref="InfoBar" /> UI
        /// </summary>
        /// <param name="layout">The <see cref="UILayout" /> to set the UI to</param>
        /// <param name="errorDescription">The description of the error that occurred</param>
        /// <param name="updatesInstalled">The number of updates installed</param>
        /// <param name="updatesFailed">The number of updates failed</param>
        private void SetUI(UILayout layout, string errorDescription, int updatesInstalled, int updatesFailed)
        {
            switch (layout)
            {
                case UILayout.Canceled:

                    #region GUI Code

                    infoBar.btnAction.Visibility = Visibility.Visible;
                    infoBar.tbStatus.Visibility = Visibility.Visible;
                    infoBar.pbProgressBar.Visibility = Visibility.Collapsed;
                    infoBar.spUpdateInfo.Visibility = Visibility.Collapsed;
                    infoBar.line.Visibility = Visibility.Collapsed;
                    infoBar.imgAdminShield.Visibility = Visibility.Collapsed;
                    infoBar.tbSelectedUpdates.Visibility = Visibility.Collapsed;

                    infoBar.tbHeading.Text = App.RM.GetString("UpdatesCanceled");
                    infoBar.tbStatus.Text = App.RM.GetString("CancelInstallation");
                    infoBar.tbAction.Text = App.RM.GetString("TryAgain");

                    infoBar.imgShield.Source = App.RedShield;
                    infoBar.imgSide.Source = redSide;

                    #endregion

                    #region Code

                    App.IsInstallInProgress = false;

                    #endregion

                    break;
                case UILayout.CheckingForUpdates:

                    #region GUI Code

                    infoBar.tbViewImportantUpdates.Visibility = Visibility.Collapsed;
                    infoBar.spUpdateInfo.Visibility = Visibility.Collapsed;
                    infoBar.tbSelectedUpdates.Visibility = Visibility.Collapsed;
                    infoBar.tbStatus.Visibility = Visibility.Collapsed;
                    infoBar.pbProgressBar.Visibility = Visibility.Visible;
                    infoBar.btnAction.Visibility = Visibility.Collapsed;

                    infoBar.tbSelectedUpdates.FontWeight = FontWeights.Normal;

                    infoBar.tbHeading.Text = App.RM.GetString("CheckingForUpdates") + "...";
                    tbRecentCheck.Text = App.RM.GetString("TodayAt") + " " + DateTime.Now.ToShortTimeString();

                    infoBar.imgShield.Source = suIcon;
                    infoBar.imgSide.Source = null;

                    #endregion

                    #region Code

                    App.IsInstallInProgress = true;

                    #endregion

                    break;
                case UILayout.ConnectingToService:

                    #region GUI Code

                    infoBar.tbSelectedUpdates.Visibility = Visibility.Collapsed;
                    infoBar.spUpdateInfo.Visibility = Visibility.Collapsed;
                    infoBar.tbStatus.Visibility = Visibility.Visible;
                    infoBar.pbProgressBar.Visibility = Visibility.Visible;
                    infoBar.btnAction.Visibility = Visibility.Collapsed;
                    infoBar.line.Visibility = Visibility.Collapsed;

                    infoBar.tbStatus.Text = App.RM.GetString("GettingInstallationStatus");
                    infoBar.tbHeading.Text = App.RM.GetString("ConnectingToService") + "...";

                    infoBar.imgShield.Source = App.YellowShield;
                    infoBar.imgSide.Source = yellowSide;

                    #endregion

                    #region Code

                    Admin.Connect();

                    #endregion

                    break;
                case UILayout.Downloading:

                    #region GUI Code

                    if (!App.IsAdmin)
                        infoBar.imgAdminShield.Visibility = Visibility.Visible;

                    infoBar.tbSelectedUpdates.Visibility = Visibility.Collapsed;
                    infoBar.spUpdateInfo.Visibility = Visibility.Collapsed;
                    infoBar.tbStatus.Visibility = Visibility.Visible;
                    infoBar.pbProgressBar.Visibility = Visibility.Visible;
                    infoBar.btnAction.Visibility = Visibility.Visible;
                    infoBar.line.Visibility = Visibility.Collapsed;

                    infoBar.tbHeading.Text = App.RM.GetString("DownloadingUpdates") + "...";
                    infoBar.tbStatus.Text = App.RM.GetString("PreparingDownload");
                    infoBar.tbAction.Text = App.RM.GetString("StopDownload");

                    infoBar.imgShield.Source = App.YellowShield;
                    infoBar.imgSide.Source = yellowSide;

                    #endregion

                    #region Code

                    App.IsInstallInProgress = false;

                    #endregion

                    break;
                case UILayout.DownloadCompleted:

                    #region GUI Code

                    if (!App.IsAdmin)
                        infoBar.imgAdminShield.Visibility = Visibility.Visible;

                    infoBar.tbStatus.Visibility = Visibility.Collapsed;
                    infoBar.tbSelectedUpdates.Visibility = Visibility.Visible;
                    infoBar.tbStatus.Visibility = Visibility.Collapsed;
                    infoBar.pbProgressBar.Visibility = Visibility.Collapsed;
                    infoBar.btnAction.Visibility = Visibility.Collapsed;
                    infoBar.line.Visibility = Visibility.Visible;

                    infoBar.tbHeading.Text = App.RM.GetString("UpdatesReadyInstalled");
                    infoBar.tbAction.Text = App.RM.GetString("InstallUpdates");

                    infoBar.imgShield.Source = App.YellowShield;
                    infoBar.imgSide.Source = yellowSide;

                    #endregion

                    #region Code

                    App.IsInstallInProgress = false;

                    #endregion

                    break;
                case UILayout.ErrorOccurred:

                    #region GUI Code

                    infoBar.imgAdminShield.Visibility = Visibility.Collapsed;

                    infoBar.spUpdateInfo.Visibility = Visibility.Collapsed;
                    infoBar.tbSelectedUpdates.Visibility = Visibility.Collapsed;
                    infoBar.btnAction.Visibility = Visibility.Visible;
                    infoBar.tbStatus.Visibility = Visibility.Visible;
                    infoBar.pbProgressBar.Visibility = Visibility.Collapsed;
                    infoBar.line.Visibility = Visibility.Collapsed;

                    infoBar.tbHeading.Text = App.RM.GetString("ErrorOccurred");
                    infoBar.tbAction.Text = App.RM.GetString("TryAgain");
                    infoBar.tbStatus.Text = errorDescription ?? App.RM.GetString("UnknownErrorOccurred");

                    infoBar.imgSide.Source = redSide;
                    infoBar.imgShield.Source = App.RedShield;

                    #endregion

                    #region Code

                    App.IsInstallInProgress = false;

                    #endregion

                    break;
                case UILayout.Installing:

                    #region GUI Code

                    if (!App.IsAdmin)
                        infoBar.imgAdminShield.Visibility = Visibility.Visible;

                    infoBar.line.Visibility = Visibility.Collapsed;
                    infoBar.btnAction.Visibility = Visibility.Visible;
                    infoBar.pbProgressBar.Visibility = Visibility.Visible;
                    infoBar.tbStatus.Visibility = Visibility.Visible;
                    infoBar.tbSelectedUpdates.Visibility = Visibility.Collapsed;
                    infoBar.spUpdateInfo.Visibility = Visibility.Collapsed;

                    infoBar.tbAction.Text = App.RM.GetString("StopInstallation");
                    infoBar.tbStatus.Text = App.RM.GetString("PreparingInstall");
                    infoBar.tbHeading.Text = App.RM.GetString("InstallingUpdates") + "...";

                    infoBar.imgShield.Source = suIcon;

                    #endregion

                    #region Code

                    App.IsInstallInProgress = true;

                    #endregion

                    break;
                case UILayout.InstallationCompleted:

                    #region GUI Code

                    if (!App.IsAdmin)
                        infoBar.imgAdminShield.Visibility = Visibility.Visible;

                    infoBar.imgSide.Source = null;
                    infoBar.btnAction.Visibility = Visibility.Collapsed;
                    infoBar.pbProgressBar.Visibility = Visibility.Collapsed;
                    infoBar.line.Visibility = Visibility.Collapsed;
                    infoBar.tbStatus.Visibility = Visibility.Visible;
                    infoBar.tbSelectedUpdates.Visibility = Visibility.Collapsed;
                    infoBar.spUpdateInfo.Visibility = Visibility.Collapsed;

                    infoBar.tbHeading.Text = App.RM.GetString("UpdatesInstalled");
                    infoBar.imgShield.Source = App.GreenShield;
                    infoBar.imgSide.Source = greenSide;

                    #region Update Status

                    infoBar.tbStatus.Text = App.RM.GetString("Succeeded") + ": " + updatesInstalled + " ";

                    if (updatesInstalled == 1)
                        infoBar.tbStatus.Text += App.RM.GetString("Update");
                    else
                        infoBar.tbStatus.Text += App.RM.GetString("Updates");

                    if (updatesFailed > 0)
                    {
                        if (updatesInstalled == 0)
                            infoBar.tbStatus.Text = App.RM.GetString("Failed") + ": " + updatesFailed + " ";
                        else
                            infoBar.tbStatus.Text += ", " + App.RM.GetString("Failed") + ": " + updatesFailed + " ";

                        if (updatesFailed == 1)
                            infoBar.tbStatus.Text += App.RM.GetString("Update");
                        else
                            infoBar.tbStatus.Text += App.RM.GetString("Updates");
                    }

                    #endregion

                    tbUpdatesInstalled.Text = App.RM.GetString("TodayAt") + " " + DateTime.Now.ToShortTimeString();

                    #endregion

                    #region Code

                    Settings.Default.lastInstall = DateTime.Now;
                    App.IsInstallInProgress = false;

                    #endregion

                    break;
                case UILayout.NoUpdates:

                    #region GUI Code

                    infoBar.spUpdateInfo.Visibility = Visibility.Collapsed;
                    infoBar.btnAction.Visibility = Visibility.Collapsed;
                    infoBar.line.Visibility = Visibility.Collapsed;
                    infoBar.tbStatus.Visibility = Visibility.Visible;
                    infoBar.tbSelectedUpdates.Visibility = Visibility.Collapsed;
                    infoBar.pbProgressBar.Visibility = Visibility.Collapsed;

                    infoBar.tbHeading.Text = App.RM.GetString("ProgramsUpToDate");
                    infoBar.tbStatus.Text = App.RM.GetString("NoNewUpdates");

                    infoBar.imgSide.Source = greenSide;
                    infoBar.imgShield.Source = App.GreenShield;

                    #endregion

                    #region Code

                    App.IsInstallInProgress = false;

                    #endregion

                    break;
                case UILayout.RebootNeeded:

                    #region GUI Code

                    infoBar.imgAdminShield.Visibility = Visibility.Collapsed;
                    infoBar.btnAction.Visibility = Visibility.Visible;
                    infoBar.tbStatus.Visibility = Visibility.Visible;
                    infoBar.tbSelectedUpdates.Visibility = Visibility.Collapsed;
                    infoBar.spUpdateInfo.Visibility = Visibility.Collapsed;
                    infoBar.pbProgressBar.Visibility = Visibility.Collapsed;
                    infoBar.line.Visibility = Visibility.Collapsed;

                    infoBar.tbAction.Text = App.RM.GetString("RestartNow");
                    infoBar.tbHeading.Text = App.RM.GetString("RebootNeeded");
                    infoBar.tbStatus.Text = App.RM.GetString("SaveAndReboot");

                    infoBar.imgShield.Source = App.YellowShield;
                    infoBar.imgSide.Source = yellowSide;

                    #endregion

                    break;

                case UILayout.UpdatesFound:

                    #region GUI Code

                    if (!App.IsAdmin)
                        infoBar.imgAdminShield.Visibility = Visibility.Visible;

                    infoBar.tbStatus.Visibility = Visibility.Collapsed;
                    infoBar.pbProgressBar.Visibility = Visibility.Collapsed;
                    infoBar.spUpdateInfo.Visibility = Visibility.Visible;
                    infoBar.tbSelectedUpdates.Visibility = Visibility.Visible;
                    infoBar.btnAction.Visibility = Visibility.Collapsed;
                    infoBar.tbViewOptionalUpdates.Visibility = Visibility.Visible;
                    infoBar.tbViewImportantUpdates.Visibility = Visibility.Visible;
                    infoBar.line.Visibility = Visibility.Visible;

                    infoBar.line.Y1 = 25;

                    infoBar.tbHeading.Text = App.RM.GetString("DownloadAndInstallUpdates");
                    infoBar.tbAction.Text = App.RM.GetString("InstallUpdates");

                    infoBar.imgSide.Source = yellowSide;
                    infoBar.imgShield.Source = App.YellowShield;

                    #endregion

                    #region Code

                    App.IsInstallInProgress = false;

                    #endregion

                    break;
            }
        }

        #endregion

        #region UI Events

        #region TextBlock

        /// <summary>
        /// Underlines the text when mouse is over the <see cref="TextBlock" />
        /// </summary>
        private void TextBlock_MouseEnter(object sender, MouseEventArgs e)
        {
            var textBlock = ((TextBlock) sender);
            textBlock.TextDecorations = TextDecorations.Underline;
        }

        /// <summary>
        /// Removes the Underlined text when mouse is leaves the <see cref="TextBlock" />
        /// </summary>
        private void TextBlock_MouseLeave(object sender, MouseEventArgs e)
        {
            var textBlock = ((TextBlock) sender);
            textBlock.TextDecorations = null;
        }

        /// <summary>
        /// Navigates to the Options page
        /// </summary>
        private void tbChangeSettings_MouseDown(object sender, MouseButtonEventArgs e)
        {
            MainWindow.NavService.Navigate(new Uri(@"Pages\Options.xaml", UriKind.Relative));
        }

        /// <summary>
        /// Checks for updates
        /// </summary>
        private void tbCheckForUpdates_MouseDown(object sender, MouseButtonEventArgs e)
        {
            CheckForUpdates();
        }

        /// <summary>
        /// Navigates to the Update History page
        /// </summary>
        private void tbViewUpdateHistory_MouseDown(object sender, MouseButtonEventArgs e)
        {
            MainWindow.NavService.Navigate(new Uri(@"Pages\UpdateHistory.xaml", UriKind.Relative));
        }

        /// <summary>
        /// Navigates to the Restore Updates page
        /// </summary>
        private void tbRestoreHiddenUpdates_MouseDown(object sender, MouseButtonEventArgs e)
        {
            MainWindow.NavService.Navigate(new Uri(@"Pages\RestoreUpdates.xaml", UriKind.Relative));
        }

        /// <summary>
        /// Shows the About Dialog window
        /// </summary>
        private void tbAbout_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var about = new About();
            about.ShowDialog();
        }

        /// <summary>
        /// Navigates to the Update Info page
        /// </summary>
        private static void tbViewOptionalUpdates_MouseDown(object sender, MouseButtonEventArgs e)
        {
            UpdateInfo.DisplayOptionalUpdates = true;
            MainWindow.NavService.Navigate(new Uri(@"Pages\UpdateInfo.xaml", UriKind.Relative));
        }

        /// <summary>
        /// Navigates to the Update Info page
        /// </summary>
        private static void tbViewImportantUpdates_MouseDown(object sender, MouseButtonEventArgs e)
        {
            UpdateInfo.DisplayOptionalUpdates = false;
            MainWindow.NavService.Navigate(new Uri(@"Pages\UpdateInfo.xaml", UriKind.Relative));
        }

        #endregion

        /// <summary>
        /// When the user cancels their selection of updates and also hides at least one update, let's re-check for updates
        /// </summary>
        private void CanceledSelectionEventHandler(object sender, EventArgs e)
        {
            CheckForUpdates();
        }

        /// <summary>
        /// Updates the UI after the user selects updates to install
        /// </summary>
        private void UpdateSelectionChangedEventHandler(object sender, UpdateSelectionChangedEventArgs e)
        {
            if (App.Applications.Count == 0)
            {
                CheckForUpdates();
                return;
            }

            infoBar.tbViewOptionalUpdates.Visibility = Visibility.Collapsed;
            infoBar.tbViewImportantUpdates.Visibility = Visibility.Collapsed;

            foreach (Update t1 in
                App.Applications.TakeWhile(t => infoBar.tbViewImportantUpdates.Visibility != Visibility.Visible || infoBar.tbViewOptionalUpdates.Visibility != Visibility.Visible).SelectMany(
                    t => t.Updates.TakeWhile(t1 => infoBar.tbViewImportantUpdates.Visibility != Visibility.Visible || infoBar.tbViewOptionalUpdates.Visibility != Visibility.Visible)))
            {
                switch (t1.Importance)
                {
                    case Importance.Important:
                        infoBar.tbViewImportantUpdates.Visibility = Visibility.Visible;
                        break;
                    case Importance.Locale:
                    case Importance.Optional:
                        infoBar.tbViewOptionalUpdates.Visibility = Visibility.Visible;
                        break;
                    case Importance.Recommended:
                        if (App.Settings.IncludeRecommended)
                            infoBar.tbViewImportantUpdates.Visibility = Visibility.Visible;
                        else
                            infoBar.tbViewOptionalUpdates.Visibility = Visibility.Visible;
                        break;
                }
            }

            if (infoBar.tbViewOptionalUpdates.Visibility == Visibility.Collapsed || infoBar.tbViewImportantUpdates.Visibility == Visibility.Collapsed)
                infoBar.line.Y1 = 25;
            else
                infoBar.line.Y1 = 50;

            #region GUI Updating

            if (e.ImportantUpdates > 0)
            {
                if (e.ImportantUpdates == 1)
                    infoBar.tbSelectedUpdates.Text = e.ImportantUpdates + " " + App.RM.GetString("ImportantUpdateSelected");
                else
                    infoBar.tbSelectedUpdates.Text = e.ImportantUpdates + " " + App.RM.GetString("ImportantUpdatesSelected");

                if (e.ImportantDownloadSize > 0)
                    infoBar.tbSelectedUpdates.Text += ", " + Base.Base.ConvertFileSize(e.ImportantDownloadSize);
            }

            if (e.OptionalUpdates > 0)
            {
                if (e.ImportantUpdates == 0)
                {
                    if (e.OptionalUpdates == 1)
                        infoBar.tbSelectedUpdates.Text = e.OptionalUpdates + " " + App.RM.GetString("OptionalUpdateSelected");
                    else
                        infoBar.tbSelectedUpdates.Text = e.OptionalUpdates + " " + App.RM.GetString("OptionalUpdatesSelected");
                }
                else
                {
                    if (e.OptionalUpdates == 1)
                        infoBar.tbSelectedUpdates.Text += Environment.NewLine + e.OptionalUpdates + " " + App.RM.GetString("OptionalUpdateSelected");
                    else
                        infoBar.tbSelectedUpdates.Text += Environment.NewLine + e.OptionalUpdates + " " + App.RM.GetString("OptionalUpdatesSelected");
                }

                if (e.OptionalDownloadSize > 0)
                    infoBar.tbSelectedUpdates.Text += ", " + Base.Base.ConvertFileSize(e.OptionalDownloadSize);
            }

            if (e.ImportantDownloadSize == 0 && e.OptionalDownloadSize == 0)
                infoBar.tbHeading.Text = App.RM.GetString("InstallUpdatesForPrograms");
            else
                infoBar.tbHeading.Text = App.RM.GetString("DownloadAndInstallUpdates");

            if (e.ImportantUpdates > 0 || e.OptionalUpdates > 0)
            {
                infoBar.btnAction.Visibility = Visibility.Visible;
                infoBar.tbSelectedUpdates.FontWeight = FontWeights.Bold;
            }
            else
            {
                infoBar.tbSelectedUpdates.Text = App.RM.GetString("NoUpdatesSelected");
                infoBar.tbSelectedUpdates.FontWeight = FontWeights.Normal;
                infoBar.btnAction.Visibility = Visibility.Collapsed;
            }

            #endregion
        }

        /// <summary>
        /// Checks for updates after hidden updates have been restored
        /// </summary>
        private void RestoredHiddenUpdateEventHandler(object sender, EventArgs e)
        {
            CheckForUpdates(true);
        }

        private void Admin_SettingsChangedEventHandler(object sender, EventArgs e)
        {
            CheckForUpdates(true);
        }

        /// <summary>
        /// Executes action based on current label. Installed, cancels, and/or searches for updates. it also can reboot the computer.
        /// </summary>
        private void btnAction_Click(object sender, RoutedEventArgs e)
        {
            if (infoBar.tbAction.Text == App.RM.GetString("InstallUpdates"))
                DownloadInstallUpdates();
            else if (infoBar.tbAction.Text == App.RM.GetString("StopDownload") || infoBar.tbAction.Text == App.RM.GetString("StopInstallation"))
            {
                //Cancel installation of updates
                if (Admin.AbortInstall())
                    SetUI(UILayout.Canceled);
                return;
            }
            else if (infoBar.tbAction.Text == App.RM.GetString("TryAgain"))
                CheckForUpdates();
            else if (infoBar.tbAction.Text == App.RM.GetString("RestartNow"))
            {
                Base.Base.StartProcess("shutdown.exe", "-r -t 00");
            }
        }

        #endregion
    }
}