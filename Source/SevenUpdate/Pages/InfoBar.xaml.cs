// ***********************************************************************
// <copyright file="InfoBar.xaml.cs"
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
    using System.Globalization;
    using System.IO;
    using System.Windows;
    using System.Windows.Input;
    using System.Windows.Shell;

    using SevenUpdate.Properties;
    using SevenUpdate.Windows;

    /// <summary>The layout for the Info Panel</summary>
    public enum UpdateAction
    {
        /// <summary>Canceled Updates</summary>
        Canceled, 

        /// <summary>Check for updates</summary>
        CheckForUpdates, 

        /// <summary>Checking for updates</summary>
        CheckingForUpdates, 

        /// <summary>When connecting to the admin service</summary>
        ConnectingToService, 

        /// <summary>When downloading of updates has been completed</summary>
        DownloadCompleted, 

        /// <summary>Downloading updates</summary>
        Downloading, 

        /// <summary>An Error Occurred when downloading/installing updates</summary>
        ErrorOccurred, 

        /// <summary>When installation of updates have completed</summary>
        InstallationCompleted, 

        /// <summary>Installing Updates</summary>
        Installing, 

        /// <summary>No updates have been found</summary>
        NoUpdates, 

        /// <summary>A reboot is needed to finish installing updates</summary>
        RebootNeeded, 

        /// <summary>Updates have been found</summary>
        UpdatesFound, 
    }

    /// <summary>Interaction logic for InfoBar.xaml</summary>
    public sealed partial class InfoBar
    {
        #region Constants and Fields

        /// <summary>Indicates if Seven Update will only install updates</summary>
        private bool isInstallOnly;

        #endregion

        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref = "InfoBar" /> class.</summary>
        public InfoBar()
        {
            this.InitializeComponent();
            Search.ErrorOccurred += this.ErrorOccurred;
            AdminClient.ServiceError += this.ErrorOccurred;
            Search.SearchCompleted += this.SearchCompleted;
            UpdateInfo.UpdateSelectionChanged += this.UpdateSelectionChanged;
            Core.UpdateActionChanged += this.SetUI;
            ServiceCallBack.DownloadProgressChanged += this.DownloadProgressChanged;
            ServiceCallBack.DownloadDone += this.DownloadCompleted;
            ServiceCallBack.InstallProgressChanged += this.InstallProgressChanged;
            ServiceCallBack.InstallDone += this.InstallCompleted;
            ServiceCallBack.ErrorOccurred += this.ErrorOccurred;
        }

        #endregion

        #region Methods

        /// <summary>Updates the UI when the downloading of updates completes</summary>
        /// <param name="e">The <see cref="SevenUpdate.DownloadCompletedEventArgs"/> instance containing the event data.</param>
        private static void DownloadCompleted(DownloadCompletedEventArgs e)
        {
            Core.Instance.UpdateAction = e.ErrorOccurred ? UpdateAction.ErrorOccurred : UpdateAction.Installing;
        }

        /// <summary>Updates the UI when the downloading of updates has completed</summary>
        /// <param name="sender">The sender</param>
        /// <param name="e">The <see cref="SevenUpdate.DownloadCompletedEventArgs"/> instance containing the event data.</param>
        private void DownloadCompleted(object sender, DownloadCompletedEventArgs e)
        {
            if (!this.Dispatcher.CheckAccess())
            {
                this.Dispatcher.BeginInvoke(DownloadCompleted, e);
            }
            else
            {
                DownloadCompleted(e);
            }
        }

        /// <summary>Downloads updates</summary>
        private void DownloadInstallUpdates()
        {
            for (var x = 0; x < Core.Applications.Count; x++)
            {
                for (var y = 0; y < Core.Applications[x].Updates.Count; y++)
                {
                    if (Core.Applications[x].Updates[y].Selected)
                    {
                        continue;
                    }

                    Core.Applications[x].Updates.RemoveAt(y);
                    y--;
                }

                if (Core.Applications[x].Updates.Count != 0)
                {
                    continue;
                }

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
                    try
                    {
                        File.Delete(Utilities.AllUserStore + @"updates.sui");
                    }
                    catch (Exception)
                    {
                    }

                    Core.Instance.UpdateAction = this.isInstallOnly ? UpdateAction.Installing : UpdateAction.Downloading;
                    Settings.Default.lastInstall = DateTime.Now;
                }
                else
                {
                    Core.Instance.UpdateAction = UpdateAction.Canceled;
                }
            }
            else
            {
                Core.Instance.UpdateAction = UpdateAction.Canceled;
            }
        }

        /// <summary>Updates the UI when the download progress has changed</summary>
        /// <param name="e">The DownloadProgress data</param>
        private void DownloadProgressChanged(DownloadProgressChangedEventArgs e)
        {
            if (Core.IsReconnect)
            {
                Core.Instance.UpdateAction = UpdateAction.Downloading;
                Core.IsReconnect = false;
            }

            if (e.BytesTotal > 0 && e.BytesTransferred > 0)
            {
                if (e.BytesTotal == e.BytesTransferred)
                {
                    return;
                }

                var progress = e.BytesTransferred * 100 / e.BytesTotal;
                App.TaskBar.ProgressState = TaskbarItemProgressState.Normal;
                App.TaskBar.ProgressValue = Convert.ToDouble(progress) / 100;
                this.tbStatus.Text = String.Format(
                    CultureInfo.CurrentCulture, 
                    Properties.Resources.DownloadPercentProgress, 
                    Utilities.ConvertFileSize(e.BytesTotal), 
                    progress.ToString("F0", CultureInfo.CurrentCulture));
            }
            else
            {
                App.TaskBar.ProgressState = TaskbarItemProgressState.Indeterminate;
                this.tbStatus.Text = String.Format(CultureInfo.CurrentCulture, Properties.Resources.DownloadProgress, e.FilesTransferred, e.FilesTotal);
            }
        }

        /// <summary>Updates the UI when the download progress has changed</summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="SevenUpdate.DownloadProgressChangedEventArgs"/> instance containing the event data.</param>
        private void DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            if (!this.Dispatcher.CheckAccess())
            {
                this.Dispatcher.BeginInvoke(this.DownloadProgressChanged, e);
            }
            else
            {
                this.DownloadProgressChanged(e);
            }
        }

        /// <summary>Sets the UI when an error occurs</summary>
        /// <param name="e">The <see cref="SevenUpdate.ErrorOccurredEventArgs"/> instance containing the event data.</param>
        private void ErrorOccurred(ErrorOccurredEventArgs e)
        {
            Core.Instance.UpdateAction = UpdateAction.ErrorOccurred;
            switch (e.Type)
            {
                case ErrorType.FatalNetworkError:
                    this.tbStatus.Text = Properties.Resources.CheckConnection;
                    break;
                case ErrorType.InstallationError:
                case ErrorType.SearchError:
                case ErrorType.DownloadError:
                case ErrorType.GeneralErrorNonFatal:
                case ErrorType.FatalError:
                    this.tbStatus.Text = e.Exception;
                    break;
            }
        }

        /// <summary>Sets the UI when an error has occurred</summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="SevenUpdate.ErrorOccurredEventArgs"/> instance containing the event data.</param>
        private void ErrorOccurred(object sender, ErrorOccurredEventArgs e)
        {
            if (!this.Dispatcher.CheckAccess())
            {
                this.Dispatcher.BeginInvoke(this.ErrorOccurred, e);
            }
            else
            {
                this.ErrorOccurred(e);
            }
        }

        /// <summary>Updates the UI when the installation has completed</summary>
        /// <param name="e">The InstallCompleted data</param>
        private void InstallCompleted(InstallCompletedEventArgs e)
        {
            Settings.Default.lastInstall = DateTime.Now;
            Core.Instance.IsAdmin = false;

            // if a reboot is needed lets say it
            if (Utilities.RebootNeeded)
            {
                Core.Instance.UpdateAction = UpdateAction.RebootNeeded;
                return;
            }

            Core.Instance.UpdateAction = UpdateAction.InstallationCompleted;

            if (e.UpdatesFailed <= 0)
            {
                this.tbStatus.Text = e.UpdatesInstalled == 1
                                         ? Properties.Resources.UpdateInstalled
                                         : String.Format(CultureInfo.CurrentCulture, Properties.Resources.UpdatesInstalled, e.UpdatesInstalled);
                return;
            }

            Core.Instance.UpdateAction = UpdateAction.ErrorOccurred;

            if (e.UpdatesInstalled == 0)
            {
                this.tbStatus.Text = e.UpdatesFailed == 1
                                         ? Properties.Resources.UpdateFailed
                                         : String.Format(CultureInfo.CurrentCulture, Properties.Resources.UpdatesFailed, e.UpdatesFailed);
            }
            else
            {
                this.tbStatus.Text = String.Format(CultureInfo.CurrentCulture, Properties.Resources.UpdatesInstalledFailed, e.UpdatesInstalled, e.UpdatesFailed);
            }
        }

        /// <summary>Sets the UI when the installation of updates has completed</summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="SevenUpdate.InstallCompletedEventArgs"/> instance containing the event data.</param>
        private void InstallCompleted(object sender, InstallCompletedEventArgs e)
        {
            if (!this.Dispatcher.CheckAccess())
            {
                this.Dispatcher.BeginInvoke(this.InstallCompleted, e);
            }
            else
            {
                this.InstallCompleted(e);
            }
        }

        /// <summary>Updates the UI when the installation progress has changed</summary>
        /// <param name="e">The InstallProgress data</param>
        private void InstallProgressChanged(InstallProgressChangedEventArgs e)
        {
            if (Core.IsReconnect)
            {
                Core.Instance.UpdateAction = UpdateAction.Installing;
                Core.IsReconnect = false;
            }

            if (e.CurrentProgress == -1)
            {
                this.tbStatus.Text = Properties.Resources.PreparingInstall;
                App.TaskBar.ProgressState = TaskbarItemProgressState.Indeterminate;
            }
            else
            {
                App.TaskBar.ProgressState = TaskbarItemProgressState.Normal;
                App.TaskBar.ProgressValue = e.CurrentProgress;
                this.tbStatus.Text = e.TotalUpdates > 1
                                         ? String.Format(
                                             CultureInfo.CurrentCulture, 
                                             Properties.Resources.InstallExtendedProgress, 
                                             e.UpdateName, 
                                             e.UpdatesComplete, 
                                             e.TotalUpdates, 
                                             e.CurrentProgress)
                                         : String.Format(CultureInfo.CurrentCulture, Properties.Resources.InstallProgress, e.UpdateName, e.CurrentProgress);
            }
        }

        /// <summary>Sets the UI when the install progress has changed</summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="SevenUpdate.InstallProgressChangedEventArgs"/> instance containing the event data.</param>
        private void InstallProgressChanged(object sender, InstallProgressChangedEventArgs e)
        {
            if (!this.Dispatcher.CheckAccess())
            {
                this.Dispatcher.BeginInvoke(this.InstallProgressChanged, e);
            }
            else
            {
                this.InstallProgressChanged(e);
            }
        }

        /// <summary>Performs an action based on the <see cref="UpdateAction"/></summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void PerformAction(object sender, RoutedEventArgs e)
        {
            switch (Core.Instance.UpdateAction)
            {
                case UpdateAction.DownloadCompleted:
                case UpdateAction.UpdatesFound:
                    this.DownloadInstallUpdates();
                    break;
                case UpdateAction.Downloading:
                case UpdateAction.Installing:
                    if (AdminClient.AbortInstall())
                    {
                        Core.Instance.UpdateAction = UpdateAction.Canceled;
                    }

                    break;

                case UpdateAction.CheckForUpdates:
                case UpdateAction.Canceled:
                case UpdateAction.ErrorOccurred:
                    Core.Instance.UpdateAction = UpdateAction.CheckingForUpdates;
                    Core.CheckForUpdates();
                    break;
                case UpdateAction.RebootNeeded:
                    Utilities.StartProcess(@"shutdown.exe", "-r -t 00");
                    break;
            }
        }

        /// <summary>Updates the UI the search for updates has completed</summary>
        /// <param name="e">The SearchComplete data</param>
        private void SearchCompleted(SearchCompletedEventArgs e)
        {
            if (Core.Instance.UpdateAction == UpdateAction.ErrorOccurred)
            {
                return;
            }

            Core.Applications = e.Applications as Collection<Sui>;
            if (Core.Applications == null)
            {
                Core.Instance.UpdateAction = UpdateAction.NoUpdates;
                return;
            }

            if (Core.Applications.Count > 0)
            {
                try
                {
                    Utilities.Serialize(Core.Applications, Utilities.AllUserStore + @"updates.sui");
                }
                catch (Exception)
                {
                }

                if (Core.Settings.IncludeRecommended)
                {
                    e.ImportantCount += e.RecommendedCount;
                }
                else
                {
                    e.OptionalCount += e.RecommendedCount;
                }

                if (e.ImportantCount > 0 || e.OptionalCount > 0)
                {
                    Core.Instance.UpdateAction = UpdateAction.UpdatesFound;

                    if (e.ImportantCount > 0 && e.OptionalCount > 0)
                    {
                        this.line.Y1 = 50;
                    }

                    if (e.ImportantCount > 0)
                    {
                        this.tbViewImportantUpdates.Text = String.Format(
                            CultureInfo.CurrentCulture, 
                            e.ImportantCount == 1 ? Properties.Resources.ImportantUpdateAvailable : Properties.Resources.ImportantUpdatesAvailable, 
                            e.ImportantCount);

                        this.tbViewImportantUpdates.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        this.tbViewImportantUpdates.Visibility = Visibility.Collapsed;
                    }

                    if (e.OptionalCount > 0)
                    {
                        if (e.ImportantCount == 0)
                        {
                            this.tbHeading.Text = Properties.Resources.NoImportantUpdates;
                        }

                        this.tbViewOptionalUpdates.Text = String.Format(
                            CultureInfo.CurrentCulture, 
                            e.OptionalCount == 1 ? Properties.Resources.OptionalUpdateAvailable : Properties.Resources.OptionalUpdatesAvailable, 
                            e.OptionalCount);

                        this.tbViewOptionalUpdates.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        this.tbViewOptionalUpdates.Visibility = Visibility.Collapsed;
                    }
                }

                // End Code
            }
            else
            {
                Core.Instance.UpdateAction = UpdateAction.NoUpdates;
            }
        }

        /// <summary>Sets the UI when the search for updates has completed</summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="SevenUpdate.SearchCompletedEventArgs"/> instance containing the event data.</param>
        private void SearchCompleted(object sender, SearchCompletedEventArgs e)
        {
            if (!this.Dispatcher.CheckAccess())
            {
                this.Dispatcher.BeginInvoke(this.SearchCompleted, e);
            }
            else
            {
                this.SearchCompleted(e);
            }
        }

        /// <summary>Handles the MouseDown event of the ImportantUpdates control.</summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        private void SelectImportantUpdates(object sender, MouseButtonEventArgs e)
        {
            UpdateInfo.DisplayOptionalUpdates = false;
            App.NavService.Navigate(new Uri(@"/SevenUpdate;component/Pages/UpdateInfo.xaml", UriKind.Relative));
        }

        /// <summary>Selects optional updates and navigates to the <see cref="UpdateInfo"/> page</summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        private void SelectOptionalUpdates(object sender, MouseButtonEventArgs e)
        {
            UpdateInfo.DisplayOptionalUpdates = true;
            App.NavService.Navigate(new Uri(@"/SevenUpdate;component/Pages/UpdateInfo.xaml", UriKind.Relative));
        }

        /// <summary>Sets the data context for the page</summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void SetDataContext(object sender, RoutedEventArgs e)
        {
            this.DataContext = Core.Instance;
        }

        /// <summary>Sets the UI based on the <see cref="UpdateAction"/></summary>
        /// <param name="action">The action.</param>
        private void SetUI(UpdateAction action)
        {
            this.btnAction.IsShieldNeeded = false;
            this.btnAction.Visibility = Visibility.Collapsed;
            this.tbHeading.Visibility = Visibility.Collapsed;
            this.tbStatus.Visibility = Visibility.Collapsed;
            this.tbSelectedUpdates.Visibility = Visibility.Collapsed;
            this.tbSelectedUpdates.FontWeight = FontWeights.Normal;
            this.tbViewOptionalUpdates.Visibility = Visibility.Collapsed;
            this.tbViewImportantUpdates.Visibility = Visibility.Collapsed;
            this.line.Visibility = Visibility.Collapsed;
            App.TaskBar.ProgressState = TaskbarItemProgressState.None;

            switch (action)
            {
                case UpdateAction.Canceled:
                    this.tbHeading.Text = Properties.Resources.UpdatesCanceled;
                    this.tbStatus.Text = Properties.Resources.CancelInstallation;
                    this.btnAction.ButtonText = Properties.Resources.TryAgain;

                    this.tbHeading.Visibility = Visibility.Visible;
                    this.tbStatus.Visibility = Visibility.Visible;
                    this.btnAction.Visibility = Visibility.Visible;

                    break;

                case UpdateAction.CheckForUpdates:
                    this.tbHeading.Text = Properties.Resources.CheckForUpdatesHeading;
                    this.tbStatus.Text = Properties.Resources.InstallLatestUpdates;
                    this.btnAction.ButtonText = Properties.Resources.CheckForUpdates;

                    this.tbHeading.Visibility = Visibility.Visible;
                    this.tbStatus.Visibility = Visibility.Visible;
                    this.btnAction.Visibility = Visibility.Visible;

                    break;

                case UpdateAction.CheckingForUpdates:
                    this.tbHeading.Text = Properties.Resources.CheckingForUpdates;
                    this.tbHeading.Visibility = Visibility.Visible;
                    this.line.Y1 = 25;

                    App.TaskBar.ProgressState = TaskbarItemProgressState.Indeterminate;

                    break;

                case UpdateAction.ConnectingToService:
                    this.tbHeading.Text = Properties.Resources.ConnectingToService;

                    this.tbHeading.Visibility = Visibility.Visible;
                    break;

                case UpdateAction.DownloadCompleted:
                    this.tbHeading.Text = Properties.Resources.UpdatesReadyInstalled;
                    this.btnAction.ButtonText = Properties.Resources.InstallUpdates;

                    this.tbHeading.Visibility = Visibility.Visible;
                    this.tbSelectedUpdates.Visibility = Visibility.Visible;
                    this.btnAction.Visibility = Visibility.Visible;
                    this.line.Visibility = Visibility.Visible;
                    this.line.Y1 = 25;
                    this.btnAction.IsShieldNeeded = !Core.Instance.IsAdmin;

                    break;

                case UpdateAction.Downloading:
                    this.tbHeading.Text = Properties.Resources.DownloadingUpdates;
                    this.tbStatus.Text = Properties.Resources.PreparingDownload;
                    this.btnAction.ButtonText = Properties.Resources.StopDownload;

                    this.tbHeading.Visibility = Visibility.Visible;
                    this.tbStatus.Visibility = Visibility.Visible;
                    this.btnAction.Visibility = Visibility.Visible;

                    this.btnAction.IsShieldNeeded = !Core.Instance.IsAdmin;

                    App.TaskBar.ProgressState = TaskbarItemProgressState.Indeterminate;
                    break;

                case UpdateAction.ErrorOccurred:
                    this.tbHeading.Text = Properties.Resources.ErrorOccurred;
                    this.tbStatus.Text = Properties.Resources.UnknownErrorOccurred;
                    this.btnAction.ButtonText = Properties.Resources.TryAgain;

                    this.tbHeading.Visibility = Visibility.Visible;
                    this.tbStatus.Visibility = Visibility.Visible;
                    this.btnAction.Visibility = Visibility.Visible;

                    App.TaskBar.ProgressState = TaskbarItemProgressState.Error;
                    break;

                case UpdateAction.InstallationCompleted:
                    this.tbHeading.Text = Properties.Resources.UpdatesInstalledTitle;

                    this.tbHeading.Visibility = Visibility.Visible;
                    this.tbStatus.Visibility = Visibility.Visible;

                    break;

                case UpdateAction.Installing:
                    this.tbHeading.Text = Properties.Resources.InstallingUpdates;
                    this.tbStatus.Text = Properties.Resources.PreparingInstall;
                    this.btnAction.ButtonText = Properties.Resources.StopInstallation;

                    this.tbHeading.Visibility = Visibility.Visible;
                    this.tbStatus.Visibility = Visibility.Visible;
                    this.btnAction.Visibility = Visibility.Visible;

                    this.btnAction.IsShieldNeeded = !Core.Instance.IsAdmin;
                    App.TaskBar.ProgressState = TaskbarItemProgressState.Indeterminate;
                    break;

                case UpdateAction.NoUpdates:
                    this.tbHeading.Text = Properties.Resources.ProgramsUpToDate;
                    this.tbStatus.Text = Properties.Resources.NoNewUpdates;

                    this.tbHeading.Visibility = Visibility.Visible;
                    this.tbStatus.Visibility = Visibility.Visible;

                    break;

                case UpdateAction.RebootNeeded:
                    this.tbHeading.Text = Properties.Resources.RebootNeeded;
                    this.tbStatus.Text = Properties.Resources.SaveAndReboot;
                    this.btnAction.ButtonText = Properties.Resources.RestartNow;

                    this.tbHeading.Visibility = Visibility.Visible;
                    this.tbStatus.Visibility = Visibility.Visible;
                    this.btnAction.Visibility = Visibility.Visible;
                    break;

                case UpdateAction.UpdatesFound:

                    this.tbHeading.Text = Properties.Resources.DownloadAndInstallUpdates;
                    this.tbSelectedUpdates.Text = Properties.Resources.NoUpdatesSelected;
                    this.btnAction.ButtonText = Properties.Resources.InstallUpdates;

                    this.tbHeading.Visibility = Visibility.Visible;
                    this.tbSelectedUpdates.Visibility = Visibility.Visible;
                    this.line.Visibility = Visibility.Visible;
                    this.btnAction.IsShieldNeeded = !Core.Instance.IsAdmin;
                    break;
            }
        }

        /// <summary>Sets the UI when the update action is changed</summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void SetUI(object sender, EventArgs e)
        {
            if (!this.Dispatcher.CheckAccess())
            {
                this.Dispatcher.BeginInvoke(() => this.SetUI(Core.Instance.UpdateAction));
            }
            else
            {
                this.SetUI(Core.Instance.UpdateAction);
            }
        }

        /// <summary>Updates the UI when the update selection changes</summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="UpdateInfo.UpdateSelectionChangedEventArgs"/> instance containing the event data.</param>
        private void UpdateSelectionChanged(object sender, UpdateInfo.UpdateSelectionChangedEventArgs e)
        {
            if (e.ImportantUpdates > 0)
            {
                this.tbViewImportantUpdates.Visibility = Visibility.Visible;
                this.tbSelectedUpdates.Text = e.ImportantUpdates == 1
                                                  ? Properties.Resources.ImportantUpdateSelected
                                                  : String.Format(CultureInfo.CurrentCulture, Properties.Resources.ImportantUpdatesSelected, e.ImportantUpdates);

                if (e.ImportantDownloadSize > 0)
                {
                    this.tbSelectedUpdates.Text += ", " + Utilities.ConvertFileSize(e.ImportantDownloadSize);
                }
            }

            if (e.OptionalUpdates > 0)
            {
                this.tbViewOptionalUpdates.Visibility = Visibility.Visible;
                if (e.ImportantUpdates == 0)
                {
                    this.tbSelectedUpdates.Text = e.OptionalUpdates == 1
                                                      ? Properties.Resources.OptionalUpdateSelected
                                                      : String.Format(CultureInfo.CurrentCulture, Properties.Resources.OptionalUpdatesSelected, e.OptionalUpdates);
                }
                else
                {
                    if (e.OptionalUpdates == 1)
                    {
                        this.tbSelectedUpdates.Text += Environment.NewLine + Properties.Resources.OptionalUpdateSelected;
                    }
                    else
                    {
                        this.tbSelectedUpdates.Text += Environment.NewLine +
                                                       String.Format(CultureInfo.CurrentCulture, Properties.Resources.OptionalUpdatesSelected, e.OptionalUpdates);
                    }
                }

                if (e.OptionalDownloadSize > 0)
                {
                    this.tbSelectedUpdates.Text += ", " + Utilities.ConvertFileSize(e.OptionalDownloadSize);
                }
            }

            if ((e.ImportantDownloadSize == 0 && e.OptionalDownloadSize == 0) && (e.ImportantUpdates > 0 || e.OptionalUpdates > 0))
            {
                this.isInstallOnly = true;
                this.tbHeading.Text = Properties.Resources.InstallUpdatesForPrograms;
            }
            else
            {
                this.tbHeading.Text = Properties.Resources.DownloadAndInstallUpdates;
                this.isInstallOnly = false;
            }

            if (e.ImportantUpdates > 0 || e.OptionalUpdates > 0)
            {
                this.tbSelectedUpdates.FontWeight = FontWeights.Bold;
                this.btnAction.Visibility = Visibility.Visible;
            }
            else
            {
                this.tbSelectedUpdates.Text = Properties.Resources.NoUpdatesSelected;
                this.tbSelectedUpdates.FontWeight = FontWeights.Normal;
                this.btnAction.Visibility = Visibility.Collapsed;
            }
        }

        #endregion
    }
}