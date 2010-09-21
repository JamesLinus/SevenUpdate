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
using System.Windows;
using System.Windows.Input;
using SevenUpdate.Properties;
using SevenUpdate.Windows;

#endregion

namespace SevenUpdate.Pages
{

    #region Enums

    /// <summary>
    ///   The layout for the Info Panel
    /// </summary>
    public enum UpdateAction
    {
        /// <summary>
        ///   Canceled Updates
        /// </summary>
        Canceled,

        /// <summary>
        ///   Check for updates
        /// </summary>
        CheckForUpdates,

        /// <summary>
        ///   Checking for updates
        /// </summary>
        CheckingForUpdates,

        /// <summary>
        ///   When connecting to the admin service
        /// </summary>
        ConnectingToService,

        /// <summary>
        ///   When downloading of updates has been completed
        /// </summary>
        DownloadCompleted,

        /// <summary>
        ///   Downloading updates
        /// </summary>
        Downloading,

        /// <summary>
        ///   An Error Occurred when downloading/installing updates
        /// </summary>
        ErrorOccurred,

        /// <summary>
        ///   When installation of updates have completed
        /// </summary>
        InstallationCompleted,

        /// <summary>
        ///   Installing Updates
        /// </summary>
        Installing,

        /// <summary>
        ///   No updates have been found
        /// </summary>
        NoUpdates,

        /// <summary>
        ///   A reboot is needed to finish installing updates
        /// </summary>
        RebootNeeded,

        /// <summary>
        ///   Updates have been found
        /// </summary>
        UpdatesFound,
    }

    #endregion

    /// <summary>
    ///   Interaction logic for InfoBar.xaml
    /// </summary>
    public sealed partial class InfoBar
    {
        #region Fields

        private bool isInstallOnly;

        #endregion

        /// <summary>
        ///   A Control that displays update progress and information
        /// </summary>
        public InfoBar()
        {
            InitializeComponent();
            Search.ErrorOccurred += ErrorOccurred;
            AdminClient.ServiceError += ErrorOccurred;
            Search.SearchCompleted += SearchCompleted;
            UpdateInfo.UpdateSelectionChanged += UpdateInfo_UpdateSelectionChanged;
            Core.UpdateActionChanged += UpdateAction_Changed;
            ServiceCallBack.DownloadProgressChanged += DownloadProgressChanged;
            ServiceCallBack.DownloadDone += DownloadCompleted;
            ServiceCallBack.InstallProgressChanged += InstallProgressChanged;
            ServiceCallBack.InstallDone += InstallCompleted;
            ServiceCallBack.ErrorOccurred += ErrorOccurred;
        }

        #region Update Event Methods

        /// <summary>
        ///   Updates the UI the search for updates has completed
        /// </summary>
        /// <param name = "e">The SearchComplete data</param>
        private void SearchCompleted(SearchCompletedEventArgs e)
        {
            if (Core.Instance.UpdateAction == UpdateAction.ErrorOccurred)
                return;
            Core.IsInstallInProgress = false;
            if (e.Applications.Count > 0)
            {
                Core.Applications = e.Applications;

                if (Core.Settings.IncludeRecommended)
                    e.ImportantCount += e.RecommendedCount;

                #region GUI Updating

                if (e.ImportantCount > 0 || e.OptionalCount > 0)
                {
                    Core.Instance.UpdateAction = UpdateAction.UpdatesFound;
                    Settings.Default.updatesFound = true;

                    if (e.ImportantCount > 0 && e.OptionalCount > 0)
                        line.Y1 = 50;

                    if (e.ImportantCount > 0)
                    {
                        tbViewImportantUpdates.Text = String.Format(e.ImportantCount == 1 ? Properties.Resources.ImportantUpdateAvailable : Properties.Resources.ImportantUpdatesAvailable, e.ImportantCount);

                        tbViewImportantUpdates.Visibility = Visibility.Visible;
                    }
                    else
                        tbViewImportantUpdates.Visibility = Visibility.Collapsed;

                    if (e.OptionalCount > 0)
                    {
                        if (e.ImportantCount == 0)
                            tbHeading.Text = Properties.Resources.NoImportantUpdates;

                        tbViewOptionalUpdates.Text = String.Format(e.OptionalCount == 1 ? Properties.Resources.OptionalUpdateAvailable : Properties.Resources.OptionalUpdatesAvailable, e.OptionalCount);

                        tbViewOptionalUpdates.Visibility = Visibility.Visible;
                    }
                    else
                        tbViewOptionalUpdates.Visibility = Visibility.Collapsed;
                }
                //End Code

                #endregion
            }
            else
            {
                Settings.Default.updatesFound = false;
                Core.Instance.UpdateAction = UpdateAction.NoUpdates;
            }
        }

        /// <summary>
        ///   Sets the UI when an error occurs
        /// </summary>
        private void ErrorOccurred(ErrorOccurredEventArgs e)
        {
            Core.IsInstallInProgress = false;
            Core.Instance.UpdateAction = UpdateAction.ErrorOccurred;
            switch (e.Type)
            {
                case ErrorType.FatalNetworkError:
                    tbStatus.Text = Properties.Resources.CheckConnection;
                    break;
                case ErrorType.InstallationError:
                case ErrorType.SearchError:
                case ErrorType.DownloadError:
                case ErrorType.GeneralErrorNonFatal:
                case ErrorType.FatalError:
                    tbStatus.Text = e.Exception;
                    break;
            }
        }

        /// <summary>
        ///   Updates the UI when the installation progress has changed
        /// </summary>
        /// <param name = "e">The InstallProgress data</param>
        private void InstallProgressChanged(InstallProgressChangedEventArgs e)
        {
            if (Core.IsReconnect)
            {
                Core.Instance.UpdateAction = UpdateAction.Installing;
                Core.IsReconnect = false;
            }

            if (e.CurrentProgress == -1)
                tbStatus.Text = Properties.Resources.PreparingInstall;
            else
            {
                tbStatus.Text = e.TotalUpdates > 1 ? String.Format(Properties.Resources.InstallExtendedProgress, e.UpdateName, e.UpdatesComplete, e.TotalUpdates, e.CurrentProgress) : String.Format(Properties.Resources.InstallProgress, e.UpdateName, e.CurrentProgress);
            }
        }

        /// <summary>
        ///   Updates the UI when the download progress has changed
        /// </summary>
        /// <param name = "e">The DownloadProgress data</param>
        private void DownloadProgressChanged(DownloadProgressChangedEventArgs e)
        {
            if (Core.IsReconnect)
            {
                Core.Instance.UpdateAction = UpdateAction.Downloading;
                Core.IsReconnect = false;
            }

            if (e.BytesTotal > 0 && e.BytesTransferred > 0)
                tbStatus.Text = String.Format(Properties.Resources.DownloadPercentProgress, Base.ConvertFileSize(e.BytesTotal), (e.BytesTransferred*100/e.BytesTotal).ToString("F0"));
            else
                tbStatus.Text = String.Format(Properties.Resources.DownloadProgress, e.FilesTransferred, e.FilesTotal);
        }

        /// <summary>
        ///   Updates the UI when the installation has completed
        /// </summary>
        /// <param name = "e">The InstallCompleted data</param>
        private void InstallCompleted(InstallCompletedEventArgs e)
        {
            Settings.Default.lastInstall = DateTime.Now;
            Core.Instance.IsAdmin = false;
            // if a reboot is needed lets say it

            if (Base.RebootNeeded)
            {
                Core.Instance.UpdateAction = UpdateAction.RebootNeeded;
                return;
            }
            Core.Instance.UpdateAction = UpdateAction.InstallationCompleted;

            #region Update Status

            if (e.UpdatesFailed <= 0)
            {
                tbStatus.Text = e.UpdatesInstalled == 1 ? Properties.Resources.UpdateInstalled : String.Format(Properties.Resources.UpdatesInstalled, e.UpdatesInstalled);
                return;
            }
            Core.Instance.UpdateAction = UpdateAction.ErrorOccurred;

            if (e.UpdatesInstalled == 0)
                tbStatus.Text = e.UpdatesFailed == 1 ? Properties.Resources.UpdateFailed : String.Format(Properties.Resources.UpdatesFailed, e.UpdatesFailed);
            else
                tbStatus.Text = String.Format(Properties.Resources.UpdatesInstalledFailed, e.UpdatesInstalled, e.UpdatesFailed);

            #endregion
        }

        private static void DownloadCompleted(DownloadCompletedEventArgs e)
        {
            if (e.ErrorOccurred)
                Core.Instance.UpdateAction = UpdateAction.ErrorOccurred;
            else
                Core.Instance.UpdateAction = Core.IsAutoCheck ? UpdateAction.DownloadCompleted : UpdateAction.Installing;
        }

        #region Invoker Events

        /// <summary>
        ///   Sets the UI when an error has occurred
        /// </summary>
        private void ErrorOccurred(object sender, ErrorOccurredEventArgs e)
        {
            if (!Dispatcher.CheckAccess())
                Dispatcher.BeginInvoke(ErrorOccurred, e);
            else
                ErrorOccurred(e);
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

        #region Methods

        private void SetUI(UpdateAction action)
        {
            btnAction.IsShieldNeeded = false;
            btnAction.Visibility = Visibility.Collapsed;
            tbHeading.Visibility = Visibility.Collapsed;
            tbStatus.Visibility = Visibility.Collapsed;
            tbSelectedUpdates.Visibility = Visibility.Collapsed;
            tbSelectedUpdates.FontWeight = FontWeights.Normal;
            tbViewOptionalUpdates.Visibility = Visibility.Collapsed;
            tbViewImportantUpdates.Visibility = Visibility.Collapsed;
            line.Visibility = Visibility.Collapsed;

            switch (action)
            {
                case UpdateAction.Canceled:
                    tbHeading.Text = Properties.Resources.UpdatesCanceled;
                    tbStatus.Text = Properties.Resources.CancelInstallation;
                    btnAction.ButtonText = Properties.Resources.TryAgain;

                    tbHeading.Visibility = Visibility.Visible;
                    tbStatus.Visibility = Visibility.Visible;
                    btnAction.Visibility = Visibility.Visible;

                    Core.IsInstallInProgress = false;
                    break;

                case UpdateAction.CheckForUpdates:
                    tbHeading.Text = Properties.Resources.CheckForUpdatesHeading;
                    tbStatus.Text = Properties.Resources.InstallLatestUpdates;
                    btnAction.ButtonText = Properties.Resources.CheckForUpdates;

                    tbHeading.Visibility = Visibility.Visible;
                    tbStatus.Visibility = Visibility.Visible;
                    btnAction.Visibility = Visibility.Visible;

                    break;

                case UpdateAction.CheckingForUpdates:
                    tbHeading.Text = Properties.Resources.CheckingForUpdates;

                    tbHeading.Visibility = Visibility.Visible;
                    line.Y1 = 25;

                    break;

                case UpdateAction.ConnectingToService:
                    tbHeading.Text = Properties.Resources.ConnectingToService;

                    tbHeading.Visibility = Visibility.Visible;
                    break;

                case UpdateAction.DownloadCompleted:
                    tbHeading.Text = Properties.Resources.UpdatesReadyInstalled;
                    btnAction.ButtonText = Properties.Resources.InstallUpdates;

                    tbHeading.Visibility = Visibility.Visible;
                    tbSelectedUpdates.Visibility = Visibility.Visible;
                    btnAction.Visibility = Visibility.Visible;
                    line.Visibility = Visibility.Visible;
                    line.Y1 = 25;
                    btnAction.IsShieldNeeded = !Core.Instance.IsAdmin;

                    Core.IsInstallInProgress = false;
                    break;

                case UpdateAction.Downloading:
                    tbHeading.Text = Properties.Resources.DownloadingUpdates;
                    tbStatus.Text = Properties.Resources.PreparingDownload;
                    btnAction.ButtonText = Properties.Resources.StopDownload;

                    tbHeading.Visibility = Visibility.Visible;
                    tbStatus.Visibility = Visibility.Visible;
                    btnAction.Visibility = Visibility.Visible;

                    btnAction.IsShieldNeeded = !Core.Instance.IsAdmin;

                    Core.IsInstallInProgress = true;
                    break;

                case UpdateAction.ErrorOccurred:
                    tbHeading.Text = Properties.Resources.ErrorOccurred;
                    tbStatus.Text = Properties.Resources.UnknownErrorOccurred;
                    btnAction.ButtonText = Properties.Resources.TryAgain;

                    tbHeading.Visibility = Visibility.Visible;
                    tbStatus.Visibility = Visibility.Visible;
                    btnAction.Visibility = Visibility.Visible;

                    Core.IsInstallInProgress = false;
                    break;

                case UpdateAction.InstallationCompleted:
                    tbHeading.Text = Properties.Resources.UpdatesInstalledTitle;

                    tbHeading.Visibility = Visibility.Visible;
                    tbStatus.Visibility = Visibility.Visible;

                    Core.IsInstallInProgress = false;

                    break;

                case UpdateAction.Installing:
                    tbHeading.Text = Properties.Resources.InstallingUpdates;
                    tbStatus.Text = Properties.Resources.PreparingInstall;
                    btnAction.ButtonText = Properties.Resources.StopInstallation;

                    tbHeading.Visibility = Visibility.Visible;
                    tbStatus.Visibility = Visibility.Visible;
                    btnAction.Visibility = Visibility.Visible;

                    btnAction.IsShieldNeeded = !Core.Instance.IsAdmin;
                    Core.IsInstallInProgress = true;
                    break;

                case UpdateAction.NoUpdates:
                    tbHeading.Text = Properties.Resources.ProgramsUpToDate;
                    tbStatus.Text = Properties.Resources.NoNewUpdates;

                    tbHeading.Visibility = Visibility.Visible;
                    tbStatus.Visibility = Visibility.Visible;

                    Core.IsInstallInProgress = false;
                    break;

                case UpdateAction.RebootNeeded:
                    tbHeading.Text = Properties.Resources.RebootNeeded;
                    tbStatus.Text = Properties.Resources.SaveAndReboot;
                    btnAction.ButtonText = Properties.Resources.RestartNow;

                    tbHeading.Visibility = Visibility.Visible;
                    tbStatus.Visibility = Visibility.Visible;
                    btnAction.Visibility = Visibility.Visible;
                    break;

                case UpdateAction.UpdatesFound:

                    tbHeading.Text = Properties.Resources.DownloadAndInstallUpdates;
                    tbSelectedUpdates.Text = Properties.Resources.NoUpdatesSelected;
                    btnAction.ButtonText = Properties.Resources.InstallUpdates;

                    tbHeading.Visibility = Visibility.Visible;
                    tbSelectedUpdates.Visibility = Visibility.Visible;
                    line.Visibility = Visibility.Visible;
                    btnAction.IsShieldNeeded = !Core.Instance.IsAdmin;
                    break;
            }
        }

        /// <summary>
        ///   Downloads updates
        /// </summary>
        private void DownloadInstallUpdates()
        {
            for (var x = 0; x < Core.Applications.Count; x++)
            {
                for (var y = 0; y < Core.Applications[x].Updates.Count; y++)
                {
                    if (Core.Applications[x].Updates[y].Selected)
                        continue;
                    Core.Applications[x].Updates.RemoveAt(y);
                    y--;
                }
                if (Core.Applications[x].Updates.Count != 0)
                    continue;
                Core.Applications.RemoveAt(x);
                x--;
            }

            if (Core.Applications.Count > 0)
            {
                var sla = new LicenseAgreement();
                if (sla.LoadLicenses() == false)
                {
                    Core.Instance.UpdateAction = UpdateAction.Canceled;
                    return;
                }

                if (AdminClient.Install())
                {
                    Core.Instance.UpdateAction = isInstallOnly ? UpdateAction.Installing : UpdateAction.Downloading;
                    Core.IsInstallInProgress = true;
                    Settings.Default.lastInstall = DateTime.Now;
                }
                else
                    Core.Instance.UpdateAction = UpdateAction.Canceled;
            }
            else
                Core.Instance.UpdateAction = UpdateAction.Canceled;
        }

        #endregion

        #region UI Events

        /// <summary>
        ///   Sets the UI when the update action is changed
        /// </summary>
        private void UpdateAction_Changed(object sender, EventArgs e)
        {
            if (!Dispatcher.CheckAccess())
                Dispatcher.BeginInvoke(() => SetUI(Core.Instance.UpdateAction));
            else
                SetUI(Core.Instance.UpdateAction);
        }

        private void UpdateInfo_UpdateSelectionChanged(object sender, UpdateSelectionChangedEventArgs e)
        {
            if (e.ImportantUpdates > 0)
            {
                tbViewImportantUpdates.Visibility = Visibility.Visible;
                tbSelectedUpdates.Text = e.ImportantUpdates == 1 ? Properties.Resources.ImportantUpdateSelected : String.Format(Properties.Resources.ImportantUpdatesSelected, e.ImportantUpdates);

                if (e.ImportantDownloadSize > 0)
                    tbSelectedUpdates.Text += ", " + Base.ConvertFileSize(e.ImportantDownloadSize);
            }

            if (e.OptionalUpdates > 0)
            {
                tbViewOptionalUpdates.Visibility = Visibility.Visible;
                if (e.ImportantUpdates == 0)
                    tbSelectedUpdates.Text = e.OptionalUpdates == 1 ? Properties.Resources.OptionalUpdateSelected : String.Format(Properties.Resources.OptionalUpdatesSelected, e.OptionalUpdates);
                else
                {
                    if (e.OptionalUpdates == 1)
                        tbSelectedUpdates.Text += Environment.NewLine + Properties.Resources.OptionalUpdateSelected;
                    else
                        tbSelectedUpdates.Text += Environment.NewLine + String.Format(Properties.Resources.OptionalUpdatesSelected, e.OptionalUpdates);
                }

                if (e.OptionalDownloadSize > 0)
                    tbSelectedUpdates.Text += ", " + Base.ConvertFileSize(e.OptionalDownloadSize);
            }

            if ((e.ImportantDownloadSize == 0 && e.OptionalDownloadSize == 0) && (e.ImportantUpdates > 0 || e.OptionalUpdates > 0))
            {
                isInstallOnly = true;
                tbHeading.Text = Properties.Resources.InstallUpdatesForPrograms;
            }
            else
            {
                tbHeading.Text = Properties.Resources.DownloadAndInstallUpdates;
                isInstallOnly = false;
            }

            if (e.ImportantUpdates > 0 || e.OptionalUpdates > 0)
            {
                tbSelectedUpdates.FontWeight = FontWeights.Bold;
                btnAction.Visibility = Visibility.Visible;
            }
            else
            {
                tbSelectedUpdates.Text = Properties.Resources.NoUpdatesSelected;
                tbSelectedUpdates.FontWeight = FontWeights.Normal;
                btnAction.Visibility = Visibility.Collapsed;
            }
        }

        private void Infobar_Loaded(object sender, RoutedEventArgs e)
        {
            DataContext = Core.Instance;
        }

        private void UacButton_Click(object sender, RoutedEventArgs e)
        {
            switch (Core.Instance.UpdateAction)
            {
                case UpdateAction.DownloadCompleted:
                case UpdateAction.UpdatesFound:
                    DownloadInstallUpdates();
                    break;
                case UpdateAction.Downloading:
                case UpdateAction.Installing:
                    if (AdminClient.AbortInstall())
                        Core.Instance.UpdateAction = UpdateAction.Canceled;
                    break;

                case UpdateAction.CheckForUpdates:
                case UpdateAction.Canceled:
                case UpdateAction.ErrorOccurred:
                    Core.Instance.UpdateAction = UpdateAction.CheckingForUpdates;
                    Core.CheckForUpdates();
                    break;
                case UpdateAction.RebootNeeded:
                    Base.StartProcess("shutdown.exe", "-r -t 00");
                    break;
            }
        }

        private void OptionalUpdates_MouseDown(object sender, MouseButtonEventArgs e)
        {
            UpdateInfo.DisplayOptionalUpdates = true;
            MainWindow.NavService.Navigate(new Uri(@"Pages\UpdateInfo.xaml", UriKind.Relative));
        }

        private void ImportantUpdates_MouseDown(object sender, MouseButtonEventArgs e)
        {
            UpdateInfo.DisplayOptionalUpdates = false;
            MainWindow.NavService.Navigate(new Uri(@"Pages\UpdateInfo.xaml", UriKind.Relative));
        }

        #endregion
    }
}