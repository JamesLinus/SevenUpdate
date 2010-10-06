// ***********************************************************************
// Assembly         : SevenUpdate.Admin
// Author           : sevenalive
// Created          : 09-17-2010
//
// Last Modified By : sevenalive
// Last Modified On : 10-05-2010
// Description      : 
//
// Copyright        : (c) Seven Software. All rights reserved.
// ***********************************************************************
namespace SevenUpdate.Admin
{
    using System;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.ServiceModel;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Timers;
    using System.Windows.Forms;

    using Microsoft.Win32;

    using SevenUpdate.Admin.Properties;
    using SevenUpdate.Service;

    using Application = System.Windows.Application;
    using Timer = System.Timers.Timer;

    /// <summary>
    /// The main class of the application
    /// </summary>
    internal static class App
    {
        #region Constants and Fields

        /// <summary>
        ///   The collection of applications to update
        /// </summary>
        private static Collection<Sui> apps;

        /// <summary>
        ///   The WCF service host
        /// </summary>
        private static ServiceHost host;

        /// <summary>
        ///   Indicates if the installation was executed by automatic settings
        /// </summary>
        private static bool isAutoInstall;

        /// <summary>
        ///   Indicates if the program is currently installing updates
        /// </summary>
        private static bool isInstalling;

        /// <summary>
        ///   The notifyIcon used only when Auto Updating
        /// </summary>
        private static NotifyIcon notifyIcon;

        /// <summary>
        ///   Indicates if the program is waiting
        /// </summary>
        private static bool waiting;

        #endregion

        #region Enums

        /// <summary>
        /// Defines constants for the notification type, such has SearchComplete
        /// </summary>
        private enum NotifyType
        {
            /// <summary>
            ///   Indicates searching is completed
            /// </summary>
            SearchComplete, 

            /// <summary>
            ///   Indicates the downloading of updates has started
            /// </summary>
            DownloadStarted, 

            /// <summary>
            ///   Indicates download has completed
            /// </summary>
            DownloadComplete, 

            /// <summary>
            ///   Indicates that the installation of updates has begun
            /// </summary>
            InstallStarted, 

            /// <summary>
            ///   Indicates that the installation of updates has completed
            /// </summary>
            InstallCompleted
        }

        #endregion

        #region Properties

        /// <summary>
        ///   Gets or sets a value indicating whether Seven Update UI is currently connected.
        /// </summary>
        private static bool IsClientConnected { get; set; }

        /// <summary>
        ///   Gets Seven Updates program settings
        /// </summary>
        private static Config Settings
        {
            get
            {
                var t = Base.Deserialize<Config>(Base.ConfigFile);
                return t ?? new Config { AutoOption = AutoUpdateOption.Notify, IncludeRecommended = false };
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Reports that the download has completed and starts update installation if necessary
        /// </summary>
        /// <param name="sender">
        /// The source of the event.
        /// </param>
        /// <param name="e">
        /// The <see cref="SevenUpdate.DownloadCompletedEventArgs"/> instance containing the event data.
        /// </param>
        private static void DownloadCompleted(object sender, DownloadCompletedEventArgs e)
        {
            if ((Settings.AutoOption == AutoUpdateOption.Install && isAutoInstall) || !isAutoInstall)
            {
                if (Service.DownloadCompleted != null && IsClientConnected)
                {
                    Service.DownloadCompleted(false);
                }

                Application.Current.Dispatcher.BeginInvoke(UpdateNotifyIcon, NotifyType.InstallStarted);
                isInstalling = true;
                File.Delete(Base.AllUserStore + "updates.sui");
                Install.InstallUpdates(apps);
            }
            else
            {
                isInstalling = false;
                Application.Current.Dispatcher.BeginInvoke(UpdateNotifyIcon, NotifyType.DownloadComplete);
            }
        }

        /// <summary>
        /// Reports that the download progress has changed
        /// </summary>
        /// <param name="sender">
        /// The source of the event.
        /// </param>
        /// <param name="e">
        /// The <see cref="SevenUpdate.DownloadProgressChangedEventArgs"/> instance containing the event data.
        /// </param>
        private static void DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            isInstalling = true;
            if (Service.DownloadProgressChanged != null && IsClientConnected)
            {
                Service.DownloadProgressChanged(e.BytesTransferred, e.BytesTotal, e.FilesTransferred, e.FilesTotal);
            }

            Application.Current.Dispatcher.BeginInvoke(
                UpdateNotifyIcon, String.Format(CultureInfo.CurrentCulture, Resources.DownloadProgress, e.FilesTransferred, e.FilesTotal));
        }

        /// <summary>
        /// Starts the downloading of updates
        /// </summary>
        /// <param name="appUpdates">
        /// The collection of applications and updates
        /// </param>
        private static void DownloadUpdates(Collection<Sui> appUpdates)
        {
            try
            {
                apps = appUpdates;
                Task.Factory.StartNew(() => Download.DownloadUpdates(apps, true));
            }
            catch (Exception e)
            {
                Base.ReportError(e, Base.AllUserStore);
            }
        }

        /// <summary>
        /// Runs when there is an error searching for updates
        /// </summary>
        /// <param name="sender">
        /// The source of the event.
        /// </param>
        /// <param name="e">
        /// The <see cref="SevenUpdate.ErrorOccurredEventArgs"/> instance containing the event data.
        /// </param>
        private static void ErrorOccurred(object sender, ErrorOccurredEventArgs e)
        {
            if (e.Type == ErrorType.FatalNetworkError)
            {
                ShutdownApp();
            }
        }

        /// <summary>
        /// Reports an error when the host faults
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The <see cref="System.EventArgs"/> instance containing the event data.
        /// </param>
        private static void HostFaulted(object sender, EventArgs e)
        {
            IsClientConnected = false;
            Base.ReportError("Host Fault", Base.AllUserStore);
            if (Service.ErrorOccurred != null)
            {
                Service.ErrorOccurred(@"Communication with the update service has been interrupted and cannot be resumed", ErrorType.FatalError);
            }

            try
            {
                host.Abort();
            }
            catch
            {
            }

            ShutdownApp();
        }

        /// <summary>
        /// Reports an error if an unknown message received
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The <see cref="System.ServiceModel.UnknownMessageReceivedEventArgs"/> instance containing the event data.
        /// </param>
        private static void HostUnknownMessageReceived(object sender, UnknownMessageReceivedEventArgs e)
        {
            Base.ReportError(e.Message.ToString(), Base.AllUserStore);
        }

        /// <summary>
        /// Reports the installation has completed
        /// </summary>
        /// <param name="sender">
        /// The source of the event.
        /// </param>
        /// <param name="e">
        /// The <see cref="SevenUpdate.InstallCompletedEventArgs"/> instance containing the event data.
        /// </param>
        private static void InstallCompleted(object sender, InstallCompletedEventArgs e)
        {
            isInstalling = false;
            if (Service.InstallCompleted != null && IsClientConnected)
            {
                Service.InstallCompleted(e.UpdatesInstalled, e.UpdatesFailed);
            }

            File.Delete(Base.AllUserStore + "updates.sui");
            ShutdownApp();
        }

        /// <summary>
        /// Reports when the installation progress has changed
        /// </summary>
        /// <param name="sender">
        /// The source of the event.
        /// </param>
        /// <param name="e">
        /// The <see cref="SevenUpdate.InstallProgressChangedEventArgs"/> instance containing the event data.
        /// </param>
        private static void InstallProgressChanged(object sender, InstallProgressChangedEventArgs e)
        {
            if (Service.InstallProgressChanged != null && IsClientConnected)
            {
                Service.InstallProgressChanged(e.UpdateName, e.CurrentProgress, e.UpdatesComplete, e.TotalUpdates);
            }

            Application.Current.Dispatcher.BeginInvoke(UpdateNotifyIcon, String.Format(CultureInfo.CurrentCulture, Resources.InstallProgress, e.CurrentProgress));
        }

        /// <summary>
        /// The main execution method
        /// </summary>
        /// <param name="args">
        /// The command line arguments
        /// </param>
        [STAThread]
        private static void Main(string[] args)
        {
            bool createdNew;
            var timer = new Timer(10000);
            timer.Elapsed += ShutdownApp;
            timer.Start();

            using (new Mutex(true, "SevenUpdate.Admin", out createdNew))
            {
                try
                {
                    if (createdNew)
                    {
                        host = new ServiceHost(typeof(Service));
                        host.Faulted += HostFaulted;
                        host.UnknownMessageReceived += HostUnknownMessageReceived;
                        Service.ClientConnected += ServiceClientConnected;
                        Service.ClientDisconnected += ServiceClientDisconnected;
                        Service.DownloadUpdates += DownloadUpdates;
                        SystemEvents.SessionEnding += PreventClose;
                        Search.SearchCompleted += SearchCompleted;
                        Search.ErrorOccurred += ErrorOccurred;
                        Download.DownloadCompleted += DownloadCompleted;
                        Download.DownloadProgressChanged += DownloadProgressChanged;
                        Install.InstallCompleted += InstallCompleted;
                        Install.InstallProgressChanged += InstallProgressChanged;
                        host.Open();
                        if (!Directory.Exists(Base.AllUserStore))
                        {
                            Directory.CreateDirectory(Base.AllUserStore);
                        }
                    }
                }
                catch (FaultException e)
                {
                    Base.ReportError(e, Base.AllUserStore);
                    if (Service.ErrorOccurred != null)
                    {
                        Service.ErrorOccurred(e.Message, ErrorType.FatalError);
                    }

                    SystemEvents.SessionEnding -= PreventClose;
                    ShutdownApp();
                }
                catch (Exception e)
                {
                    Base.ReportError(e, Base.AllUserStore);
                    if (Service.ErrorOccurred != null)
                    {
                        Service.ErrorOccurred(e.Message, ErrorType.FatalError);
                    }

                    SystemEvents.SessionEnding -= PreventClose;

                    ShutdownApp();
                }
            }

            var app = new Application();

            try
            {
                if (args.Length > 0)
                {
                    if (args[0] == "Abort")
                    {
                        using (var fs = File.Create(Base.AllUserStore + "abort.lock"))
                        {
                            fs.WriteByte(0);
                            fs.Close();
                            ShutdownApp();
                        }
                    }

                    if (args[0] == "Auto")
                    {
                        if (File.Exists(Base.AllUserStore + "abort.lock"))
                        {
                            File.Delete(Base.AllUserStore + "abort.lock");
                        }

                        isAutoInstall = true;
                        isInstalling = true;
                        notifyIcon = new NotifyIcon { Icon = Resources.trayIcon, Text = Resources.CheckingForUpdates, Visible = true };
                        notifyIcon.BalloonTipClicked += RunSevenUpdate;
                        notifyIcon.Click += RunSevenUpdate;
                        Search.ErrorOccurred += ErrorOccurred;
                        Search.SearchForUpdates(Base.Deserialize<Collection<Sua>>(Base.AppsFile));
                    }
                    else
                    {
                        ShutdownApp();
                    }
                }
            }
            catch (Exception e)
            {
                Base.ReportError(e, Base.AllUserStore);
            }

            app.Run();
            SystemEvents.SessionEnding -= PreventClose;
            try
            {
                if (notifyIcon != null)
                {
                    notifyIcon.Visible = false;
                    notifyIcon.Dispose();
                    notifyIcon = null;
                }
            }
            catch
            {
            }
        }

        /// <summary>
        /// Prevents the system from shutting down until the installation is safely stopped
        /// </summary>
        /// <param name="sender">
        /// The source of the event.
        /// </param>
        /// <param name="e">
        /// The <see cref="Microsoft.Win32.SessionEndingEventArgs"/> instance containing the event data.
        /// </param>
        private static void PreventClose(object sender, SessionEndingEventArgs e)
        {
            if (notifyIcon != null)
            {
                try
                {
                    notifyIcon.Visible = false;
                    notifyIcon.Dispose();
                    notifyIcon = null;
                }
                catch
                {
                }
            }

            using (var fs = File.Create(Base.AllUserStore + "abort.lock"))
            {
                fs.WriteByte(0);
                fs.Close();
            }

            e.Cancel = true;
        }

        /// <summary>
        /// Starts Seven Update UI
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The <see cref="System.EventArgs"/> instance containing the event data.
        /// </param>
        private static void RunSevenUpdate(object sender, EventArgs e)
        {
            if (Environment.OSVersion.Version.Major < 6)
            {
                if (notifyIcon.Text == Resources.UpdatesFoundViewThem || notifyIcon.Text == Resources.UpdatesDownloadedViewThem ||
                    notifyIcon.Text == Resources.CheckingForUpdates)
                {
                    Base.StartProcess(Base.AppDir + @"SevenUpdate.exe", @"Auto");
                }
                else
                {
                    Base.StartProcess(Base.AppDir + @"SevenUpdate.exe", @"Reconnect");
                }
            }
            else
            {
                Base.StartProcess(@"schtasks.exe", "/Run /TN \"SevenUpdate\"");
            }

            if (notifyIcon.Text == Resources.UpdatesFoundViewThem || notifyIcon.Text == Resources.UpdatesDownloadedViewThem ||
                notifyIcon.Text == Resources.CheckingForUpdates)
            {
                ShutdownApp();
            }
        }

        /// <summary>
        /// Runs when the search for updates has completed for an auto update
        /// </summary>
        /// <param name="sender">
        /// The source of the event.
        /// </param>
        /// <param name="e">
        /// The <see cref="SevenUpdate.SearchCompletedEventArgs"/> instance containing the event data.
        /// </param>
        private static void SearchCompleted(object sender, SearchCompletedEventArgs e)
        {
            isInstalling = false;
            apps = e.Applications as Collection<Sui>;
            if (apps.Count > 0)
            {
                Base.Serialize(apps, Base.AllUserStore + "updates.sui");

                if (Settings.AutoOption == AutoUpdateOption.Notify)
                {
                    Application.Current.Dispatcher.BeginInvoke(UpdateNotifyIcon, NotifyType.SearchComplete);
                }
                else
                {
                    Application.Current.Dispatcher.BeginInvoke(UpdateNotifyIcon, NotifyType.DownloadStarted);
                    Task.Factory.StartNew(() => Download.DownloadUpdates(apps));
                    isInstalling = true;
                }
            }
            else
            {
                ShutdownApp();
            }
        }

        /// <summary>
        /// Occurs when Seven Update UI connects to the admin process
        /// </summary>
        private static void ServiceClientConnected()
        {
            IsClientConnected = true;
        }

        /// <summary>
        /// Occurs when the Seven Update UI disconnected
        /// </summary>
        private static void ServiceClientDisconnected()
        {
            IsClientConnected = false;

            if (isInstalling == false)
            {
                ShutdownApp();
            }
        }

        /// <summary>
        /// Shuts down the process and removes the icon of the notification bar
        /// </summary>
        private static void ShutdownApp()
        {
            if (notifyIcon != null)
            {
                try
                {
                    notifyIcon.Visible = false;
                    notifyIcon.Dispose();
                    notifyIcon = null;
                }
                catch
                {
                }
            }

            Environment.Exit(0);
        }

        /// <summary>
        /// Shutdowns the application
        /// </summary>
        /// <param name="sender">
        /// The source of the event.
        /// </param>
        /// <param name="e">
        /// The <see cref="System.Timers.ElapsedEventArgs"/> instance containing the event data.
        /// </param>
        private static void ShutdownApp(object sender, ElapsedEventArgs e)
        {
            if (isInstalling)
            {
                return;
            }

            if (Process.GetProcessesByName("SevenUpdate").Length > 0 && waiting)
            {
                ShutdownApp();
            }

            if (Process.GetProcessesByName("SevenUpdate").Length < 1 && !waiting)
            {
                ShutdownApp();
            }
        }

        /// <summary>
        /// Updates the notify icon text
        /// </summary>
        /// <param name="text">
        /// The string to set the <see cref="notifyIcon"/> text
        /// </param>
        private static void UpdateNotifyIcon(string text)
        {
            if (notifyIcon != null)
            {
                notifyIcon.Text = text;
            }
        }

        /// <summary>
        /// Updates the <see cref="notifyIcon"/> state
        /// </summary>
        /// <param name="filter">
        /// The <see cref="NotifyType"/> to set the <see cref="notifyIcon"/> to.
        /// </param>
        private static void UpdateNotifyIcon(NotifyType filter)
        {
            if (notifyIcon == null)
            {
                return;
            }

            notifyIcon.Visible = true;
            switch (filter)
            {
                case NotifyType.DownloadStarted:
                    notifyIcon.Text = Resources.DownloadingUpdates;
                    break;
                case NotifyType.DownloadComplete:
                    waiting = true;
                    notifyIcon.Text = Resources.UpdatesDownloadedViewThem;
                    notifyIcon.ShowBalloonTip(5000, Resources.UpdatesDownloaded, Resources.UpdatesDownloadedViewThem, ToolTipIcon.Info);
                    break;
                case NotifyType.InstallStarted:
                    notifyIcon.Text = Resources.InstallingUpdates;
                    break;
                case NotifyType.SearchComplete:
                    waiting = true;
                    notifyIcon.Text = Resources.UpdatesFoundViewThem;
                    notifyIcon.ShowBalloonTip(5000, Resources.UpdatesFound, Resources.UpdatesFoundViewThem, ToolTipIcon.Info);
                    break;
                case NotifyType.InstallCompleted:
                    notifyIcon.Text = Resources.InstallationCompleted;
                    notifyIcon.ShowBalloonTip(5000, Resources.UpdatesInstalled, Resources.InstallationCompleted, ToolTipIcon.Info);
                    ShutdownApp();
                    break;
            }
        }

        #endregion
    }
}