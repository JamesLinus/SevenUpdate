// ***********************************************************************
// <copyright file="App.cs"
//            project="SevenUpdate.Admin"
//            assembly="SevenUpdate.Admin"
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
namespace SevenUpdate.Admin
{
    using System;
    using System.Collections.Generic;
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

    using Application = System.Windows.Application;
    using Timer = System.Timers.Timer;

    /// <summary>The main class of the application</summary>
    internal static class App
    {
        #region Constants and Fields

        /// <summary>The all users application data location</summary>
        public static readonly string AllUserStore = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "Seven Software", "Seven Update");

        /// <summary>The location of the list of applications Seven Update can update</summary>
        public static readonly string ApplicationsFile = Path.Combine(AllUserStore, "Apps.sul");

        /// <summary>The location of the application settings file</summary>
        public static readonly string ConfigFile = Path.Combine(AllUserStore, "App.config");

        /// <summary>The location of the hidden updates file</summary>
        public static readonly string HiddenFile = Path.Combine(AllUserStore, "Hidden.suh");

        /// <summary>The location of the update history file</summary>
        public static readonly string HistoryFile = Path.Combine(AllUserStore, "History.suh");

        /// <summary>The WCF service host</summary>
        private static ElevatedProcessCallback client;

        /// <summary>Gets or sets a value indicating whether the installation was executed by automatic settings</summary>
        private static bool isAutoInstall;

        /// <summary>Gets or sets a value indicating whether Seven Update UI is currently connected.</summary>
        private static bool isClientConnected;

        /// <summary>The notifyIcon used only when Auto Updating</summary>
        private static NotifyIcon notifyIcon;

        /// <summary>Indicates if the program is waiting</summary>
        private static bool waiting;

        #endregion

        #region Enums

        /// <summary>Defines constants for the notification type, such has SearchComplete</summary>
        private enum NotifyType
        {
            /// <summary>Indicates searching is completed</summary>
            SearchComplete, 

            /// <summary>Indicates the downloading of updates has started</summary>
            DownloadStarted, 

            /// <summary>Indicates download has completed</summary>
            DownloadComplete, 

            /// <summary>Indicates that the installation of updates has begun</summary>
            InstallStarted, 

            /// <summary>Indicates that the installation of updates has completed</summary>
            InstallCompleted
        }

        #endregion

        #region Properties

        /// <summary>Gets or sets the collection of applications to update</summary>
        internal static Collection<Sui> Applications { get; set; }

        /// <summary>Gets a value indicating whether the program is currently installing updates</summary>
        internal static bool IsInstalling { get; private set; }

        /// <summary>Gets Seven Updates program settings</summary>
        private static Config Settings
        {
            get
            {
                return File.Exists(ConfigFile) ? Utilities.Deserialize<Config>(ConfigFile) : new Config { AutoOption = AutoUpdateOption.Notify, IncludeRecommended = false };
            }
        }

        #endregion

        #region Methods

        /// <summary>Adds an update to the history</summary>
        /// <param name="sender">The object that called the event.</param>
        /// <param name="e">The event data</param>
        private static void AddHistory(object sender, UpdateInstalledEventArgs e)
        {
            var history = File.Exists(HistoryFile) ? Utilities.Deserialize<Collection<Suh>>(HistoryFile) : new Collection<Suh>();
            history.Add(e.Update);
            Utilities.Serialize(history, HistoryFile);
        }

        /// <summary>Checks if Seven Update is running</summary>
        /// <param name="sender">The object that called the event.</param>
        /// <param name="e">The <see cref="System.Timers.ElapsedEventArgs"/> instance containing the event data.</param>
        private static void CheckIfRunning(object sender, ElapsedEventArgs e)
        {
            Task.Factory.StartNew(
                () =>
                    {
                        if (File.Exists(Path.Combine(AllUserStore, "abort.lock")))
                        {
                            Download.CancelDownload();
                            Install.CancelInstall();
                            try
                            {
                                File.Delete(Path.Combine(AllUserStore, "abort.lock"));
                            }
                            catch (IOException)
                            {
                            }
                        }

                        if (client == null)
                        {
                            StartWcfHost();
                        }

                        if (IsInstalling)
                        {
                            return;
                        }

                        if (Process.GetProcessesByName("SevenUpdate").Length > 0 || waiting)
                        {
                            return;
                        }

#if (!DEBUG)
                        ShutdownApp();
#endif
                    });
        }

        /// <summary>Reports that the download has completed and starts update installation if necessary</summary>
        /// <param name="sender">The object that called the event.</param>
        /// <param name="e">The <see cref="DownloadCompletedEventArgs"/> instance containing the event data.</param>
        private static void DownloadCompleted(object sender, DownloadCompletedEventArgs e)
        {
            if ((Settings.AutoOption == AutoUpdateOption.Install && isAutoInstall) || !isAutoInstall)
            {
                if (isClientConnected)
                {
                    client.OnDownloadCompleted(sender, e);
                }

                Application.Current.Dispatcher.BeginInvoke(UpdateNotifyIcon, NotifyType.InstallStarted);
                IsInstalling = true;
                File.Delete(Path.Combine(AllUserStore, "updates.sui"));
                Task.Factory.StartNew(() => Install.InstallUpdates(Applications, Path.Combine(AllUserStore, "downloads")));
            }
            else
            {
                IsInstalling = false;
                Application.Current.Dispatcher.BeginInvoke(UpdateNotifyIcon, NotifyType.DownloadComplete);
            }
        }

        /// <summary>Reports that the download progress has changed</summary>
        /// <param name="sender">The object that called the event.</param>
        /// <param name="e">The <see cref="DownloadProgressChangedEventArgs"/> instance containing the event data.</param>
        private static void DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            IsInstalling = true;
            if (isClientConnected)
            {
                client.OnDownloadProgressChanged(sender, e);
            }

            Application.Current.Dispatcher.BeginInvoke(UpdateNotifyIcon, String.Format(CultureInfo.CurrentCulture, Resources.DownloadProgress, e.FilesTransferred, e.FilesTotal));
        }

        /// <summary>Runs when there is an error searching for updates</summary>
        /// <param name="sender">The object that called the event.</param>
        /// <param name="e">The <see cref="ErrorOccurredEventArgs"/> instance containing the event data.</param>
        private static void ErrorOccurred(object sender, ErrorOccurredEventArgs e)
        {
            if (e.ErrorType == ErrorType.FatalNetworkError)
            {
                ShutdownApp();
            }
        }

        /// <summary>Reports the installation has completed</summary>
        /// <param name="sender">The object that called the event.</param>
        /// <param name="e">The <see cref="InstallCompletedEventArgs"/> instance containing the event data.</param>
        private static void InstallCompleted(object sender, InstallCompletedEventArgs e)
        {
            IsInstalling = false;
            File.Delete(Path.Combine(AllUserStore, "updates.sui"));
            if (isClientConnected)
            {
                client.OnInstallCompleted(sender, e);
            }
            else
            {
                ShutdownApp();
            }
        }

        /// <summary>Reports when the installation progress has changed</summary>
        /// <param name="sender">The object that called the event.</param>
        /// <param name="e">The <see cref="InstallProgressChangedEventArgs"/> instance containing the event data.</param>
        private static void InstallProgressChanged(object sender, InstallProgressChangedEventArgs e)
        {
            if (isClientConnected)
            {
                client.OnInstallProgressChanged(sender, e);
            }

            Application.Current.Dispatcher.BeginInvoke(UpdateNotifyIcon, String.Format(CultureInfo.CurrentCulture, Resources.InstallProgress, e.CurrentProgress));
        }

        /// <summary>The main execution method</summary>
        /// <param name="args">The command line arguments</param>
        [STAThread]
        private static void Main(string[] args)
        {
            bool createdNew;
            using (new Mutex(true, "SevenUpdate.Admin", out createdNew))
            {
                if (createdNew)
                {
                    StartWcfHost();
                    SystemEvents.SessionEnding += PreventClose;
                    Search.SearchCompleted += SearchCompleted;
                    Search.ErrorOccurred += ErrorOccurred;
                    Download.DownloadCompleted += DownloadCompleted;
                    Download.DownloadProgressChanged += DownloadProgressChanged;
                    Install.InstallCompleted += InstallCompleted;
                    Install.InstallProgressChanged += InstallProgressChanged;
                    Install.UpdateInstalled += AddHistory;
                    if (!Directory.Exists(AllUserStore))
                    {
                        Directory.CreateDirectory(AllUserStore);
                    }
                }
            }

            var app = new Application();

            using (notifyIcon = new NotifyIcon())
            {
                notifyIcon.Icon = Resources.trayIcon;
                notifyIcon.Text = Resources.CheckingForUpdates;
                notifyIcon.Visible = false;

                ProcessArgs(args);

                using (var timer = new Timer(3000))
                {
                    timer.Elapsed += CheckIfRunning;
                    timer.Start();
                    app.Run();
                }

                if (client != null)
                {
                    client.ElevatedProcessStopped();
                    client.Close();
                }

                try
                {
                    File.Delete(Path.Combine(AllUserStore, "abort.lock"));
                }
                catch (IOException e)
                {
                    ErrorOccurred(null, new ErrorOccurredEventArgs(Utilities.GetExceptionAsString(e), ErrorType.FatalError));
                }
                notifyIcon.Icon = null;
            }

            SystemEvents.SessionEnding -= PreventClose;
        }

        /// <summary>Prevents the system from shutting down until the installation is safely stopped</summary>
        /// <param name="sender">The object that called the event.</param>
        /// <param name="e">The <see cref="Microsoft.Win32.SessionEndingEventArgs"/> instance containing the event data.</param>
        private static void PreventClose(object sender, SessionEndingEventArgs e)
        {
            if (notifyIcon != null)
            {
                notifyIcon.Visible = false;
                notifyIcon.Dispose();
                notifyIcon = null;
            }

            using (var fs = File.Create(Path.Combine(AllUserStore, "abort.lock")))
            {
                fs.WriteByte(0);
            }

            e.Cancel = true;
        }

        /// <summary>Processes the command line arguments</summary>
        /// <param name="args">The arguments to process</param>
        private static void ProcessArgs(IList<string> args)
        {
            if (args.Count <= 0)
            {
            }
            else
            {
                if (args[0] == "Abort")
                {
                    try
                    {
                        using (var fs = File.Create(Path.Combine(AllUserStore, "abort.lock")))
                        {
                            fs.WriteByte(0);
                        }
                    }
                    catch (Exception e)
                    {
                        if (!(e is OperationCanceledException || e is UnauthorizedAccessException || e is InvalidOperationException || e is NotSupportedException))
                        {
                            ErrorOccurred(null, new ErrorOccurredEventArgs(Utilities.GetExceptionAsString(e), ErrorType.FatalError));
                            throw;
                        }

                        Utilities.ReportError(e, ErrorType.GeneralError);
                    }

                    ShutdownApp();
                }

                if (args[0] == "Auto")
                {
                    if (File.Exists(Path.Combine(AllUserStore, "abort.lock")))
                    {
                        try
                        {
                            File.Delete(Path.Combine(AllUserStore, "abort.lock"));
                        }
                        catch (Exception e)
                        {
                            if (!(e is OperationCanceledException || e is UnauthorizedAccessException || e is InvalidOperationException || e is NotSupportedException))
                            {
                                ErrorOccurred(null, new ErrorOccurredEventArgs(Utilities.GetExceptionAsString(e), ErrorType.FatalError));
                                throw;
                            }

                            Utilities.ReportError(e, ErrorType.GeneralError);
                        }
                    }

                    isAutoInstall = true;
                    IsInstalling = true;
                    notifyIcon.BalloonTipClicked += RunSevenUpdate;
                    notifyIcon.Click += RunSevenUpdate;
                    notifyIcon.Visible = true;
                    Search.ErrorOccurred += ErrorOccurred;

                    Collection<Sua> apps = null;
                    if (File.Exists(ApplicationsFile))
                    {
                        apps = Utilities.Deserialize<Collection<Sua>>(ApplicationsFile);
                    }

                    Search.SearchForUpdates(apps, Path.Combine(AllUserStore, "downloads"));
                }
                else
                {
                    ShutdownApp();
                }
            }
        }

        /// <summary>Starts Seven Update UI</summary>
        /// <param name="sender">The object that called the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private static void RunSevenUpdate(object sender, EventArgs e)
        {
            if (Environment.OSVersion.Version.Major < 6)
            {
                if (notifyIcon.Text == Resources.UpdatesFoundViewThem || notifyIcon.Text == Resources.UpdatesDownloadedViewThem || notifyIcon.Text == Resources.CheckingForUpdates)
                {
                    Utilities.StartProcess(Path.Combine(Utilities.AppDir, "SevenUpdate.exe"), @"Auto");
                }
                else
                {
                    Utilities.StartProcess(Path.Combine(Utilities.AppDir, "SevenUpdate.exe"), @"Reconnect");
                }
            }
            else
            {
                Utilities.StartProcess(@"schtasks.exe", "/Run /TN \"SevenUpdate\"");
            }

            if (notifyIcon.Text == Resources.UpdatesFoundViewThem || notifyIcon.Text == Resources.UpdatesDownloadedViewThem || notifyIcon.Text == Resources.CheckingForUpdates)
            {
                ShutdownApp();
            }
        }

        /// <summary>Runs when the search for updates has completed for an auto update</summary>
        /// <param name="sender">The object that called the event.</param>
        /// <param name="e">The <see cref="SearchCompletedEventArgs"/> instance containing the event data.</param>
        private static void SearchCompleted(object sender, SearchCompletedEventArgs e)
        {
            IsInstalling = false;
            Applications = e.Applications as Collection<Sui>;
            if (Applications == null)
            {
                return;
            }

            if (Applications.Count > 0)
            {
                if (Applications[0].AppInfo.SuiUrl == @"http://sevenupdate.com/apps/SevenUpdate.sui" || Applications[0].AppInfo.SuiUrl == @"http://sevenupdate.com/apps/SevenUpdate-dev.sui")
                {
                    var sevenUpdate = Applications[0];
                    Applications.Clear();
                    Applications.Add(sevenUpdate);
                    e.OptionalCount = 0;
                    e.ImportantCount = 1;
                }

                Utilities.Serialize(Applications, Path.Combine(AllUserStore, "updates.sui"));

                Utilities.StartProcess(@"cacls.exe", "\"" + Path.Combine(AllUserStore, "updates.sui") + "\" /c /e /g Users:F");

                if (Settings.AutoOption == AutoUpdateOption.Notify)
                {
                    Application.Current.Dispatcher.BeginInvoke(UpdateNotifyIcon, NotifyType.SearchComplete);
                }
                else
                {
                    Application.Current.Dispatcher.BeginInvoke(UpdateNotifyIcon, NotifyType.DownloadStarted);
                    Task.Factory.StartNew(() => Download.DownloadUpdates(Applications, "SevenUpdate", Path.Combine(AllUserStore, "downloads")));
                    IsInstalling = true;
                }
            }
            else
            {
                ShutdownApp();
            }
        }

        /// <summary>Shuts down the process and removes the icon of the notification bar</summary>
        private static void ShutdownApp()
        {
            if (client != null)
            {
                if (client.State != CommunicationState.Closed)
                {
                    try
                    {
                        client.Close();
                    }
                    catch (CommunicationObjectAbortedException)
                    {
                    }
                    catch (CommunicationObjectFaultedException)
                    {
                    }
                }
            }

            if (notifyIcon != null)
            {
                notifyIcon.Icon = null;
                notifyIcon.Visible = false;
                notifyIcon.Dispose();
                notifyIcon = null;
            }

            try
            {
                File.Delete(Path.Combine(AllUserStore, @"abort.lock"));
            }
            catch (IOException)
            {
            }
            Environment.Exit(0);
        }

        /// <summary>Starts the WCF service</summary>
        private static void StartWcfHost()
        {
            var binding = new NetNamedPipeBinding { Name = "sevenupdatebinding", Security = { Mode = NetNamedPipeSecurityMode.Transport } };
            var address = new EndpointAddress("net.pipe://localhost/sevenupdate/");

            try
            {
                client = new ElevatedProcessCallback(new InstanceContext(new WcfServiceCallback()), binding, address);
                client.ElevatedProcessStarted();
                isClientConnected = true;
            }
            catch (EndpointNotFoundException)
            {
                client = null;
                isClientConnected = false;
            }
            catch (FaultException)
            {
                client = null;
                isClientConnected = false;
            }
        }

        /// <summary>Updates the notify icon text</summary>
        /// <param name="text">The string to set the <see cref="notifyIcon"/> text</param>
        private static void UpdateNotifyIcon(string text)
        {
            if (notifyIcon != null)
            {
                notifyIcon.Text = text;
            }
        }

        /// <summary>Updates the <see cref="notifyIcon"/> state</summary>
        /// <param name="filter">The <see cref="NotifyType"/> to set the <see cref="notifyIcon"/> to.</param>
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