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
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.ServiceModel;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;
using Microsoft.Win32;
using SevenUpdate.Admin.Properties;
using Application = System.Windows.Application;
using Timer = System.Timers.Timer;

#endregion

namespace SevenUpdate.Admin
{
    /// <summary>
    ///   The main class of the application
    /// </summary>
    internal static class App
    {
        #region Enums

        /// <summary>
        ///   Defines constants for the notification type, such has SearchComplete
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

        #region Fields

        private static Collection<Sui> apps;

        private static ServiceHost host;

        /// <summary>
        ///   The notifyIcon used only when Auto Updating
        /// </summary>
        private static NotifyIcon notifyIcon;

        private static bool isAutoInstall;

        private static bool isInstalling;

        #endregion

        #region Properties

        /// <summary>
        ///   Gets the update configuration settings
        /// </summary>
        private static Config Settings
        {
            get
            {
                var t = Base.Deserialize<Config>(Base.ConfigFile);
                return t ?? new Config {AutoOption = AutoUpdateOption.Notify, IncludeRecommended = false};
            }
        }

        /// <summary>
        ///   Gets or Sets a bool value indicating Seven Update UI is currently connected.
        /// </summary>
        private static bool IsClientConnected { get; set; }

        #endregion

        /// <summary>
        ///   The main execution method
        /// </summary>
        /// <param name = "args">The command line arguments</param>
        [STAThread]
        private static void Main(string[] args)
        {
            bool createdNew;
            var timer = new Timer(30000);
            timer.Elapsed += timer_Elapsed;
            timer.Start();

            using (new Mutex(true, "SevenUpdate.Admin", out createdNew))
            {
                #region WCF Hosts

                try
                {
                    if (createdNew)
                    {
                        host = new ServiceHost(typeof (Service.Service));
                        host.Faulted += HostFaulted;
                        host.UnknownMessageReceived += HostUnknownMessageReceived;
                        Service.Service.ClientConnected += Service_ClientConnected;
                        Service.Service.ClientDisconnected += Service_ClientDisconnected;
                        Service.Service.DownloadUpdates += Service_DownloadUpdates;
                        SystemEvents.SessionEnding += SystemEvents_SessionEnding;
                        Search.SearchCompleted += Search_SearchCompleted;
                        Search.ErrorOccurred += Search_ErrorOccurred;
                        Download.DownloadCompleted += Download_DownloadCompleted;
                        Download.DownloadProgressChanged += Download_DownloadProgressChanged;
                        Install.InstallCompleted += Install_InstallCompleted;
                        Install.InstallProgressChanged += Install_InstallProgressChanged;
                        host.Open();
                        if (!Directory.Exists(Base.AllUserStore))
                            Directory.CreateDirectory(Base.AllUserStore);
                    }
                }
                catch (FaultException e)
                {
                    Base.ReportError(e, Base.AllUserStore);
                    if (Service.Service.ErrorOccurred != null)
                        Service.Service.ErrorOccurred(e.Message, ErrorType.FatalError);

                    SystemEvents.SessionEnding -= SystemEvents_SessionEnding;
                    ShutdownApp();
                }
                catch (Exception e)
                {
                    Base.ReportError(e, Base.AllUserStore);
                    if (Service.Service.ErrorOccurred != null)
                        Service.Service.ErrorOccurred(e.Message, ErrorType.FatalError);

                    SystemEvents.SessionEnding -= SystemEvents_SessionEnding;

                    ShutdownApp();
                }

                #endregion
            }
            var app = new Application();

            #region Arguments

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
                            File.Delete(Base.AllUserStore + "abort.lock");
                        isAutoInstall = true;
                        isInstalling = true;
                        notifyIcon = new NotifyIcon {Icon = Resources.icon, Text = Resources.CheckingForUpdates, Visible = true};
                        notifyIcon.BalloonTipClicked += RunSevenUpdate;
                        notifyIcon.Click += RunSevenUpdate;
                        Search.ErrorOccurred += Search_ErrorOccurred;
                        Search.SearchForUpdates(Base.Deserialize<Collection<Sua>>(Base.AppsFile));
                    }
                    else
                        ShutdownApp();
                }
            }
            catch (Exception e)
            {
                Base.ReportError(e, Base.AllUserStore);
            }

            #endregion

            app.Run();
            SystemEvents.SessionEnding -= SystemEvents_SessionEnding;
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

        private static void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (isInstalling)
                return;
            if (Process.GetProcessesByName("SevenUpdate").Length < 1)
                ShutdownApp();
        }

        #region Events

        private static void Install_InstallProgressChanged(object sender, InstallProgressChangedEventArgs e)
        {
            if (Service.Service.InstallProgressChanged != null && IsClientConnected)
                Service.Service.InstallProgressChanged(e.UpdateName, e.CurrentProgress, e.UpdatesComplete, e.TotalUpdates);
            Application.Current.Dispatcher.BeginInvoke(UpdateNotifyIcon, String.Format(Resources.InstallProgress, e.CurrentProgress));
        }

        private static void Install_InstallCompleted(object sender, InstallCompletedEventArgs e)
        {
            isInstalling = false;

            if (Service.Service.InstallCompleted != null && IsClientConnected)
                Service.Service.InstallCompleted(e.UpdatesInstalled, e.UpdatesFailed);

            ShutdownApp();
        }

        private static void Download_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            isInstalling = true;
            if (Service.Service.DownloadProgressChanged != null && IsClientConnected)
                Service.Service.DownloadProgressChanged(e.BytesTransferred, e.BytesTotal, e.FilesTransferred, e.FilesTotal);
            Application.Current.Dispatcher.BeginInvoke(UpdateNotifyIcon, String.Format(Resources.DownloadProgress, e.FilesTransferred, e.FilesTotal));
        }

        private static void Download_DownloadCompleted(object sender, DownloadCompletedEventArgs e)
        {
            if ((Settings.AutoOption == AutoUpdateOption.Install && isAutoInstall) || !isAutoInstall)
            {
                if (Service.Service.DownloadCompleted != null && IsClientConnected)
                    Service.Service.DownloadCompleted(false);
                Application.Current.Dispatcher.BeginInvoke(UpdateNotifyIcon, NotifyType.InstallStarted);
                Install.InstallUpdates(apps);
                isInstalling = true;
            }
            else
            {
                isInstalling = false;
                Application.Current.Dispatcher.BeginInvoke(UpdateNotifyIcon, NotifyType.DownloadComplete);
            }
        }

        /// <summary>
        ///   Runs when the search for updates has completed for an auto update
        /// </summary>
        private static void Search_SearchCompleted(object sender, SearchCompletedEventArgs e)
        {
            if (e.Applications.Count > 0)
            {
                apps = e.Applications;
                if (Settings.AutoOption == AutoUpdateOption.Notify)
                    Application.Current.Dispatcher.BeginInvoke(UpdateNotifyIcon, NotifyType.SearchComplete);
                else
                {
                    Application.Current.Dispatcher.BeginInvoke(UpdateNotifyIcon, NotifyType.DownloadStarted);
                    Task.Factory.StartNew(() => Download.DownloadUpdates(e.Applications));
                    isInstalling = true;
                }
            }
            else
            {
                isInstalling = false;
                ShutdownApp();
            }
        }

        /// <summary>
        ///   Runs when there is an error searching for updates
        /// </summary>
        private static void Search_ErrorOccurred(object sender, ErrorOccurredEventArgs e)
        {
            if (e.Type == ErrorType.FatalNetworkError)
                ShutdownApp();
        }

        /// <summary>
        ///   Prevents the system from shutting down until the installation is safely stopped
        /// </summary>
        private static void SystemEvents_SessionEnding(object sender, SessionEndingEventArgs e)
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

        #endregion

        #region Service events

        private static void Service_DownloadUpdates(Collection<Sui> appUpdates)
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
        ///   Error Event when the .NetPipe binding faults
        /// </summary>
        private static void HostFaulted(object sender, EventArgs e)
        {
            IsClientConnected = false;
            Base.ReportError("Host Fault", Base.AllUserStore);
            if (Service.Service.ErrorOccurred != null)
                Service.Service.ErrorOccurred(@"Communication with the update service has been interrupted and cannot be resumed", ErrorType.FatalError);
            try
            {
                host.Abort();
            }
            catch
            {
            }

            ShutdownApp();
        }

        private static void HostUnknownMessageReceived(object sender, UnknownMessageReceivedEventArgs e)
        {
            Base.ReportError(e.Message.ToString(), Base.AllUserStore);
        }

        /// <summary>
        ///   Occurs when Seven Update UI connects to the admin process
        /// </summary>
        private static void Service_ClientConnected()
        {
            IsClientConnected = true;
        }

        /// <summary>
        ///   Occurs when the Seven Update UI disconnected
        /// </summary>
        private static void Service_ClientDisconnected()
        {
            IsClientConnected = false;

            if (isInstalling == false)
                ShutdownApp();
        }

        #endregion

        #region Methods

        /// <summary>
        ///   Shuts down the process and removes the icon of the notification bar
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
        ///   Updates the notify icon text
        /// </summary>
        /// <param name = "text">The string to set the notifyIcon text</param>
        private static void UpdateNotifyIcon(string text)
        {
            if (notifyIcon != null)
                notifyIcon.Text = text;
        }

        /// <summary>
        ///   Updates the notifyIcon state
        /// </summary>
        /// <param name = "filter">The <see cref = "NotifyType" /> to set the notifyIcon to.</param>
        private static void UpdateNotifyIcon(NotifyType filter)
        {
            if (notifyIcon == null)
                return;

            notifyIcon.Visible = true;
            switch (filter)
            {
                case NotifyType.DownloadStarted:
                    notifyIcon.Text = Resources.DownloadingUpdates;
                    break;
                case NotifyType.DownloadComplete:
                    notifyIcon.Text = Resources.UpdatesDownloadedViewThem;
                    notifyIcon.ShowBalloonTip(5000, Resources.UpdatesDownloaded, Resources.UpdatesDownloadedViewThem, ToolTipIcon.Info);
                    break;
                case NotifyType.InstallStarted:
                    notifyIcon.Text = Resources.InstallingUpdates;
                    break;
                case NotifyType.SearchComplete:
                    notifyIcon.Text = Resources.UpdatesFoundViewThem;
                    notifyIcon.ShowBalloonTip(5000, Resources.UpdatesFound, Resources.UpdatesFoundViewThem, ToolTipIcon.Info);
                    break;
                case NotifyType.InstallCompleted:
                    notifyIcon.Text = Resources.InstallationCompleted;
                    notifyIcon.ShowBalloonTip(5000, Resources.UpdatesInstalled, Resources.InstallationCompleted, ToolTipIcon.Info);
                    break;
            }
        }

        /// <summary>
        ///   Starts Seven Update UI
        /// </summary>
        private static void RunSevenUpdate(object sender, EventArgs e)
        {
            if (Environment.OSVersion.Version.Major < 6)
            {
                if (notifyIcon.Text == Resources.UpdatesFoundViewThem || notifyIcon.Text == Resources.UpdatesDownloadedViewThem || notifyIcon.Text == Resources.CheckingForUpdates)
                    Base.StartProcess(Base.AppDir + "SevenUpdate.exe", "Auto");
                else
                    Base.StartProcess(Base.AppDir + "SevenUpdate.exe", "Reconnect");
            }
            else
                Base.StartProcess("schtasks.exe", "/Run /TN \"SevenUpdate\"");

            if (notifyIcon.Text == Resources.UpdatesFoundViewThem || notifyIcon.Text == Resources.UpdatesDownloadedViewThem || notifyIcon.Text == Resources.CheckingForUpdates)
                ShutdownApp();
        }

        #endregion
    }
}