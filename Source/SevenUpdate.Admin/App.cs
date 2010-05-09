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
using System.IO;
using System.Resources;
using System.ServiceModel;
using System.Threading;
using System.Windows.Forms;
using Microsoft.Win32;
using SevenUpdate.Admin.Properties;
using SevenUpdate.Base;
using SharpBits.Base;
using Application = System.Windows.Application;

#endregion

namespace SevenUpdate.Admin
{
    /// <summary>
    ///   The main class of the application
    /// </summary>
    internal class App
    {
        #region Enums

        /// <summary>
        ///   Defines constants for the notification type, such has SearchComplete
        /// </summary>
        internal enum NotifyType
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
            InstallStarted
        }

        #endregion

        #region Global Vars

        private static ServiceHost host;

        /// <summary>
        ///   The notifyIcon used only when Auto Updating
        /// </summary>
        internal static NotifyIcon NotifyIcon = new NotifyIcon();

        /// <summary>
        ///   The UI Resource Strings
        /// </summary>
        internal static ResourceManager RM = new ResourceManager("SevenUpdate.Admin.Resources.UIStrings", typeof (App).Assembly);

        /// <summary>
        ///   Gets the update configuration settings
        /// </summary>
        public static Config Settings
        {
            get
            {
                var t = Base.Base.Deserialize<Config>(Base.Base.ConfigFile);
                return t ?? new Config {AutoOption = AutoUpdateOption.Notify, IncludeRecommended = false, Locale = "en"};
            }
        }

        /// <summary>
        ///   Gets or Sets a bool value indicating Seven Update UI is currently connected.
        /// </summary>
        internal static bool IsClientConnected { get; private set; }

        internal static bool IsInstall { get; set; }

        internal static Collection<Sui> AppUpdates { get; set; }

        #endregion

        /// <summary>
        ///   The main execution method
        /// </summary>
        /// <param name = "args">The command line arguments</param>
        [STAThread]
        private static void Main(string[] args)
        {
            bool createdNew;
            using (new Mutex(true, "SevenUpdate.Admin", out createdNew))
            {
                try
                {
                    if (createdNew)
                    {
                        host = new ServiceHost(typeof (Service.Service));
                        host.Open();
                        Service.Service.ClientConnected += Service_ClientConnected;
                        Service.Service.ClientDisconnected += Service_ClientDisconnected;
                        host.Faulted += HostFaulted;
                        host.UnknownMessageReceived += HostUnknownMessageReceived;
                        SystemEvents.SessionEnding += SystemEvents_SessionEnding;
                        Service.Service.OnAddApp += Service_OnAddApp;
                        Service.Service.OnHideUpdate += Service_OnHideUpdate;
                        Service.Service.OnHideUpdates += Service_OnHideUpdates;
                        Service.Service.OnSettingsChanged += Service_OnSettingsChanged;
                        Service.Service.OnSetUpdates += Service_OnSetUpdates;
                        Service.Service.OnShowUpdate += Service_OnShowUpdate;
                    }
                }
                catch (FaultException e)
                {
                    if (host != null)
                        host.Abort();
                    if (Service.Service.ErrorOccurred != null)
                        Service.Service.ErrorOccurred(e.Message, ErrorType.FatalError);

                    SystemEvents.SessionEnding -= SystemEvents_SessionEnding;
                    Base.Base.ReportError(e, Base.Base.AllUserStore);
                    ShutdownApp();
                }
                catch (Exception e)
                {
                    if (host != null)
                        host.Abort();
                    if (Service.Service.ErrorOccurred != null)
                        Service.Service.ErrorOccurred(e.Message, ErrorType.FatalError);

                    SystemEvents.SessionEnding -= SystemEvents_SessionEnding;
                    Base.Base.ReportError(e, Base.Base.AllUserStore);
                    ShutdownApp();
                }

                Base.Base.Locale = Base.Base.Locale == null ? "en" : Settings.Locale;

                if (!Directory.Exists(Base.Base.AllUserStore))
                    Directory.CreateDirectory(Base.Base.AllUserStore);

                NotifyIcon.Icon = Resources.icon;
                NotifyIcon.Visible = false;
                NotifyIcon.BalloonTipClicked += RunSevenUpdate;
                NotifyIcon.Click += RunSevenUpdate;

                #region Arguments

                try
                {
                    if (args.Length > 0)
                    {
                        var app = new Application();
                        switch (args[0])
                        {
                            case "Abort":
                                using (FileStream fs = File.Create(Base.Base.AllUserStore + "abort.lock"))
                                {
                                    fs.WriteByte(0);
                                    fs.Close();
                                }
                                break;

                            case "Auto":

                                #region code

                                if (createdNew)
                                {
                                    if (File.Exists(Base.Base.AllUserStore + "abort.lock"))
                                        File.Delete(Base.Base.AllUserStore + "abort.lock");

                                    NotifyIcon.Text = RM.GetString("CheckingForUpdates");
                                    NotifyIcon.Visible = true;
                                    Search.SearchDoneEventHandler += Search_SearchDone_EventHandler;
                                    Search.ErrorOccurredEventHandler += Search_ErrorOccurred_EventHandler;
                                    Search.SearchForUpdates(Base.Base.Deserialize<Collection<Sua>>(Base.Base.AppsFile));

                                    app.Run();
                                }
                                else
                                    ShutdownApp();

                                #endregion

                                break;
                            case "Install":

                                #region code

                                if (createdNew)
                                {
                                    try
                                    {
                                        if (File.Exists(Base.Base.AllUserStore + "abort.lock"))
                                            File.Delete(Base.Base.AllUserStore + "abort.lock");
                                        IsInstall = true;

                                        app.Run();
                                    }
                                    catch (Exception e)
                                    {
                                        Base.Base.ReportError(e, Base.Base.AllUserStore);
                                    }
                                }
                                else
                                    ShutdownApp();

                                #endregion

                                break;

                            case "Wait":
                                app.Run();
                                break;
                        }
                    }
                }
                catch (Exception e)
                {
                    Base.Base.ReportError(e, Base.Base.AllUserStore);
                }

                #endregion
            }
            SystemEvents.SessionEnding -= SystemEvents_SessionEnding;
            try
            {
                if (NotifyIcon != null)
                {
                    NotifyIcon.Visible = false;
                    NotifyIcon.Dispose();
                    NotifyIcon = null;
                }
            }
            catch
            {
            }
        }

        #region Methods

        /// <summary>
        ///   Shuts down the process and removes the icon of the notification bar
        /// </summary>
        internal static void ShutdownApp()
        {
            if (NotifyIcon != null)
            {
                try
                {
                    NotifyIcon.Visible = false;
                    NotifyIcon.Dispose();
                    NotifyIcon = null;
                }
                catch
                {
                }
            }

            if (host != null)
            {
                host.Closed += Host_Closed;
                try
                {
                    host.Abort();
                }
                catch
                {
                    Environment.Exit(0);
                }
            }
            else
                Environment.Exit(0);
        }

        private static void Host_Closed(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        /// <summary>
        ///   Updates the notify icon text
        /// </summary>
        /// <param name = "text">The string to set the notifyIcon text</param>
        internal static void UpdateNotifyIcon(string text)
        {
            NotifyIcon.Text = text;
        }

        /// <summary>
        ///   Updates the notifyIcon state
        /// </summary>
        /// <param name = "filter">The <see cref = "NotifyType" /> to set the notifyIcon to.</param>
        internal static void UpdateNotifyIcon(NotifyType filter)
        {
            NotifyIcon.Visible = true;
            switch (filter)
            {
                case NotifyType.DownloadStarted:
                    NotifyIcon.Text = RM.GetString("DownloadingUpdates");
                    break;
                case NotifyType.DownloadComplete:
                    NotifyIcon.Text = RM.GetString("UpdatesDownloadedViewThem");
                    NotifyIcon.ShowBalloonTip(5000, RM.GetString("UpdatesDownloaded"), RM.GetString("UpdatesDownloadedViewThem"), ToolTipIcon.Info);
                    break;
                case NotifyType.InstallStarted:
                    NotifyIcon.Text = RM.GetString("InstallingUpdates");
                    break;
                case NotifyType.SearchComplete:
                    NotifyIcon.Text = RM.GetString("UpdatesFoundViewThem");
                    NotifyIcon.ShowBalloonTip(5000, RM.GetString("UpdatesFound"), RM.GetString("UpdatesFoundViewThem"), ToolTipIcon.Info);
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
                if (NotifyIcon.Text == RM.GetString("UpdatesFoundViewThem") || NotifyIcon.Text == RM.GetString("UpdatesDownloadedViewThem") ||
                    NotifyIcon.Text == RM.GetString("CheckingForUpdates"))
                    Base.Base.StartProcess(Base.Base.AppDir + "SevenUpdate.exe", "Auto");
                else
                    Base.Base.StartProcess(Base.Base.AppDir + "SevenUpdate.exe", "Reconnect");
            }
            else
                Base.Base.StartProcess("schtasks.exe", "/Run /TN \"SevenUpdate\"");

            if (NotifyIcon.Text == RM.GetString("UpdatesFoundViewThem") || NotifyIcon.Text == RM.GetString("UpdatesDownloadedViewThem") ||
                NotifyIcon.Text == RM.GetString("CheckingForUpdates"))
                ShutdownApp();
        }

        #endregion

        #region Event Methods

        /// <summary>
        ///   Prevents the system from shutting down until the installation is safely stopped
        /// </summary>
        private static void SystemEvents_SessionEnding(object sender, SessionEndingEventArgs e)
        {
            if (NotifyIcon != null)
            {
                try
                {
                    NotifyIcon.Visible = false;
                    NotifyIcon.Dispose();
                    NotifyIcon = null;
                }
                catch
                {
                }
            }

            using (FileStream fs = File.Create(Base.Base.AllUserStore + "abort.lock"))
            {
                fs.WriteByte(0);
                fs.Close();
            }
            e.Cancel = true;
        }

        /// <summary>
        ///   Error Event when the .NetPipe binding faults
        /// </summary>
        private static void HostFaulted(object sender, EventArgs e)
        {
            IsClientConnected = false;
            Base.Base.ReportError("Host Fault", Base.Base.AllUserStore);
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
            Base.Base.ReportError(e.Message.ToString(), Base.Base.AllUserStore);
        }

        /// <summary>
        ///   Runs when the search for updates has completed for an auto update
        /// </summary>
        private static void Search_SearchDone_EventHandler(object sender, SearchCompletedEventArgs e)
        {
            AppUpdates = e.Applications;
            if (e.Applications.Count > 0)
            {
                if (Settings.AutoOption == AutoUpdateOption.Notify)
                    Application.Current.Dispatcher.BeginInvoke(UpdateNotifyIcon, NotifyType.SearchComplete);
                else
                {
                    Application.Current.Dispatcher.BeginInvoke(UpdateNotifyIcon, NotifyType.DownloadStarted);
                    Download.DownloadUpdates(JobPriority.Normal);
                }
            }
            else
                ShutdownApp();
        }

        /// <summary>
        ///   Runs when there is an error searching for updates
        /// </summary>
        private static void Search_ErrorOccurred_EventHandler(object sender, ErrorOccurredEventArgs e)
        {
            if (e.Type == ErrorType.FatalNetworkError)
                ShutdownApp();
        }

        #region Service events

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
        }

        private static void Service_OnShowUpdate(object sender, Service.Service.OnShowUpdateEventArgs e)
        {
            var show = Base.Base.Deserialize<Collection<Suh>>(Base.Base.HiddenFile) ?? new Collection<Suh>();

            if (show.Count == 0)
                File.Delete(Base.Base.HiddenFile);
            else
            {
                show.Remove(e.HiddenUpdate);
                Base.Base.Serialize(show, Base.Base.HiddenFile);
            }
            ShutdownApp();
        }

        private static void Service_OnSetUpdates(object sender, Service.Service.OnSetUpdatesEventArgs e)
        {
            AppUpdates = e.AppUpdates;
        }

        private static void Service_OnSettingsChanged(object sender, Service.Service.OnSettingsChangedEventArgs e)
        {
            if (!e.AutoOn)
            {
                if (Environment.OSVersion.Version.Major < 6)
                    Registry.LocalMachine.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", true).DeleteValue("Seven Update Automatic Checking", false);
                else
                    Base.Base.StartProcess("schtasks.exe", "/Change /Disable /TN \"SevenUpdate.Admin\"");
            }
            else
            {
                if (Environment.OSVersion.Version.Major < 6)
                    Registry.SetValue(@"HKEY_LOCAL_MACHINE\Software\Microsoft\Windows\CurrentVersion\Run", "Seven Update Automatic Checking",
                                      Base.Base.AppDir + @"SevenUpdate.Helper.exe ");
                else
                    Base.Base.StartProcess("schtasks.exe", "/Change /Enable /TN \"SevenUpdate.Admin\"");
            }
            Base.Base.Serialize(e.Apps, Base.Base.AppsFile);
            Base.Base.Serialize(e.Options, Base.Base.ConfigFile);
            ShutdownApp();
        }

        private static void Service_OnHideUpdates(object sender, Service.Service.OnHideUpdatesEventArgs e)
        {
            Base.Base.Serialize(e.HiddenUpdates, Base.Base.HiddenFile);
            ShutdownApp();
        }

        private static void Service_OnHideUpdate(object sender, Service.Service.OnHideUpdateEventArgs e)
        {
            var hidden = Base.Base.Deserialize<Collection<Suh>>(Base.Base.HiddenFile) ?? new Collection<Suh>();
            hidden.Add(e.HiddenUpdate);

            Base.Base.Serialize(hidden, Base.Base.HiddenFile);
        }

        private static void Service_OnAddApp(object sender, Service.Service.OnAddAppEventArgs e)
        {
            var sul = Base.Base.Deserialize<Collection<Sua>>(Base.Base.AppsFile);
            var index = sul.IndexOf(e.App);
            if (index < 0)
                sul.Add(e.App);

            Base.Base.Serialize(sul, Base.Base.AppsFile);
            ShutdownApp();
        }

        #endregion

        #endregion
    }
}