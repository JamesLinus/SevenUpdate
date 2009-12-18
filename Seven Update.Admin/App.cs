#region GNU Public License v3

// Copyright 2007, 2008 Robert Baker, aka Seven ALive.
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
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Resources;
using System.Security.AccessControl;
using System.Security.Principal;
using System.ServiceModel;
using System.Threading;
using System.Windows.Forms;
using Microsoft.Win32;
using SevenUpdate.Admin.Properties;
using SevenUpdate.Admin.WCF;
using SevenUpdate.Base;
using SharpBits.Base;
using Application = System.Windows.Application;

#endregion

namespace SevenUpdate.Admin
{
    /// <summary>The main class of the application</summary>
    internal class App
    {
        #region Enums

        /// <summary>Defines constants for the notification type, such has SearchComplete</summary>
        internal enum NotifyType
        {
            /// <summary>Indicates searching is completed</summary>
            SearchComplete,
            /// <summary>Indicates the downloading of updates has started</summary>
            DownloadStarted,
            /// <summary>Indicates download has completed</summary>
            DownloadComplete,
            /// <summary>Indicates that the installation of updates has begun</summary>
            InstallStarted,
            /// <summary>Indicates Install has completed</summary>
            InstallComplete
        }

        #endregion

        #region Global Vars

        /// <summary>The notifyIcon used only when Auto Updating</summary>
        internal static NotifyIcon NotifyIcon = new NotifyIcon();

        /// <summary>The UI Resource Strings</summary>
        internal static ResourceManager RM = new ResourceManager("SevenUpdate.Admin.Resources.UIStrings", typeof (App).Assembly);

        /// <summary>The update settings for Seven Update</summary>
        public static Config Settings { get { return Base.Base.DeserializeStruct<Config>(Base.Base.ConfigFile); } }

        /// <summary>Gets or Sets a bool value indicating Seven Update UI is currently connected.</summary>
        internal static bool IsClientConnected { get; set; }

        internal static bool IsInstall { get; set; }

        #endregion

        /// <summary>
        /// The main execution method
        /// </summary>
        /// <param name="args">The command line arguments</param>
        [STAThread]
        private static void Main(string[] args)
        {
            bool createdNew;
            using (new Mutex(true, "Seven Update.Admin", out createdNew))
            {
                ServiceHost host = null;
                try
                {
                    if (createdNew)
                    {
                        host = new ServiceHost(typeof (EventService));
                        host.Open();
                        EventService.ClientConnected += EventService_ClientConnected;
                        EventService.ClientDisconnected += EventService_ClientDisconnected;
                        host.Faulted += HostFaulted;
                        SystemEvents.SessionEnding += SystemEvents_SessionEnding;
                    }
                }
                catch (Exception e)
                {
                    if (host != null)
                        host.Close();
                    SystemEvents.SessionEnding -= SystemEvents_SessionEnding;
                    Base.Base.ReportError(e.Message, Base.Base.AllUserStore);
                    Environment.Exit(0);
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
                        switch (args[0])
                        {
                            case "Abort":
                                using (FileStream fs = File.Create(Base.Base.AllUserStore + "abort.lock"))
                                {
                                    fs.WriteByte(0);
                                    fs.Close();
                                }
                                break;

                            case "sua":

                                #region code

                                if (File.Exists(Base.Base.UserStore + "Apps.sul"))
                                {
                                    File.Delete(Base.Base.AppsFile);

                                    File.Move(Base.Base.UserStore + "Apps.sul", Base.Base.AppsFile);

                                    SetFileSecurity(Base.Base.AppsFile);
                                }

                                #endregion

                                break;
                            case "Options-On":

                                #region code

                                if (File.Exists(Base.Base.UserStore + "App.config"))
                                {
                                    File.Delete(Base.Base.ConfigFile);

                                    File.Move(Base.Base.UserStore + "App.config", Base.Base.ConfigFile);

                                    SetFileSecurity(Base.Base.ConfigFile);
                                }

                                if (File.Exists(Base.Base.UserStore + "Apps.sul"))
                                {
                                    File.Delete(Base.Base.AppsFile);
                                    File.Move(Base.Base.UserStore + "Apps.sul", Base.Base.AppsFile);
                                }

                                SetFileSecurity(Base.Base.AppsFile);

                                if (Environment.OSVersion.Version.Major < 6)
                                    Registry.SetValue(@"HKEY_LOCAL_MACHINE\Software\Microsoft\Windows\CurrentVersion\Run", "Seven Update Automatic Checking",
                                                      Environment.CurrentDirectory + @"\Seven Update.Helper.exe ");
                                else
                                {
                                    var proc = new Process
                                                   {
                                                       StartInfo =
                                                           {
                                                               FileName = Base.Base.ConvertPath(@"%WINDIR%\system32\schtasks.exe", true, true),
                                                               Verb = "runas",
                                                               UseShellExecute = true,
                                                               Arguments = "/Change /Enable /TN \"Seven Update.Admin\"",
                                                               CreateNoWindow = true,
                                                               WindowStyle = ProcessWindowStyle.Hidden
                                                           }
                                                   };

                                    proc.Start();
                                }

                                #endregion

                                break;
                            case "Options-Off":

                                #region code

                                if (File.Exists(Base.Base.UserStore + "App.config"))
                                {
                                    File.Delete(Base.Base.ConfigFile);

                                    File.Move(Base.Base.UserStore + "App.config", Base.Base.ConfigFile);

                                    SetFileSecurity(Base.Base.ConfigFile);
                                }
                                if (File.Exists(Base.Base.UserStore + "Apps.sul"))
                                {
                                    File.Delete(Base.Base.AppsFile);

                                    File.Move(Base.Base.UserStore + "Apps.sul", Base.Base.AppsFile);

                                    SetFileSecurity(Base.Base.AppsFile);
                                }
                                if (Environment.OSVersion.Version.Major < 6)
                                {
                                    Registry.LocalMachine.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", true).DeleteValue("Seven Update Automatic Checking", false);
                                }
                                else
                                {
                                    var proc = new Process
                                                   {
                                                       StartInfo =
                                                           {
                                                               FileName = Base.Base.ConvertPath(@"%WINDIR%\system32\schtasks.exe", true, true),
                                                               Verb = "runas",
                                                               UseShellExecute = true,
                                                               Arguments = "/Change /Disable /TN \"Seven Update.Admin\"",
                                                               CreateNoWindow = true,
                                                               WindowStyle = ProcessWindowStyle.Hidden
                                                           }
                                                   };

                                    proc.Start();
                                }

                                #endregion

                                break;
                            case "HideUpdate":

                                #region code

                                var hidden = Base.Base.Deserialize<Collection<SUH>>(Base.Base.HiddenFile) ?? new Collection<SUH>();
                                hidden.Add(Base.Base.Deserialize<SUH>(Base.Base.UserStore + "Update.suh"));

                                File.Delete(Base.Base.UserStore + "Update.suh");

                                Base.Base.Serialize(hidden, Base.Base.HiddenFile);

                                SetFileSecurity(Base.Base.HiddenFile);

                                #endregion

                                break;
                            case "ShowUpdate":

                                #region code

                                var show = Base.Base.Deserialize<Collection<SUH>>(Base.Base.HiddenFile) ?? new Collection<SUH>();
                                show.Remove(Base.Base.Deserialize<SUH>(Base.Base.UserStore + "Update.suh"));

                                File.Delete(Base.Base.UserStore + "Update.suh");

                                if (show.Count == 0)
                                    File.Delete(Base.Base.HiddenFile);
                                else
                                    Base.Base.Serialize(show, Base.Base.HiddenFile);

                                #endregion

                                break;
                            case "HideUpdates":

                                #region code

                                File.Delete(Base.Base.HiddenFile);

                                File.Move(Base.Base.UserStore + "Hidden.suh", Base.Base.HiddenFile);

                                SetFileSecurity(Base.Base.HiddenFile);

                                #endregion

                                break;
                            case "Auto":

                                #region code

                                if (createdNew)
                                {
                                    if (File.Exists(Base.Base.AllUserStore + "abort.lock"))
                                        File.Delete(Base.Base.AllUserStore + "abort.lock");
                                    var app = new Application();
                                    NotifyIcon.Text = RM.GetString("CheckingForUpdates") + "...";
                                    NotifyIcon.Visible = true;
                                    Search.SearchDoneEventHandler += Search_SearchDoneEventHandler;
                                    Search.SearchForUpdates(Base.Base.Deserialize<Collection<SUA>>(Base.Base.AppsFile));
                                    app.Run();
                                }
                                else
                                    Environment.Exit(0);

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
                                        var app = new Application();
                                        app.Run();
                                    }
                                    catch (Exception e)
                                    {
                                        Base.Base.ReportError(e.Message, Base.Base.AllUserStore);
                                    }
                                }
                                else
                                    Environment.Exit(0);

                                #endregion

                                break;
                        }
                    }
                }
                catch (Exception e)
                {
                    Base.Base.ReportError(e.Message, Base.Base.AllUserStore);
                }

                #endregion
            }
            SystemEvents.SessionEnding -= SystemEvents_SessionEnding;
        }

        #region Methods

        /// <summary>Updates the notify icon text</summary>
        /// <param name="text">The string to set the notifyIcon text</param>
        internal static void UpdateNotifyIcon(string text)
        {
            NotifyIcon.Text = text;
        }

        /// <summary>
        /// Updates the notifyIcon state
        /// </summary>
        /// <param name="filter">The <see cref="NotifyType" /> to set the notifyIcon to.</param>
        internal static void UpdateNotifyIcon(NotifyType filter)
        {
            NotifyIcon.Visible = true;
            //if (!NotifyIcon.Visible)
            //    return;
            switch (filter)
            {
                case NotifyType.DownloadStarted:
                    NotifyIcon.Text = RM.GetString("DownloadingUpdates") + "...";
                    break;
                case NotifyType.DownloadComplete:
                    NotifyIcon.Text = RM.GetString("UpdatesDownloadedViewThem");
                    NotifyIcon.ShowBalloonTip(5000, RM.GetString("UpdatesDownloaded"), RM.GetString("UpdatesDownloadedViewThem"), ToolTipIcon.Info);
                    break;
                case NotifyType.InstallStarted:
                    NotifyIcon.Text = RM.GetString("InstallingUpdates") + "...";
                    break;
                case NotifyType.SearchComplete:
                    NotifyIcon.Text = RM.GetString("UpdatesFoundViewThem");
                    NotifyIcon.ShowBalloonTip(5000, RM.GetString("UpdatesFound"), RM.GetString("UpdatesFoundViewThem"), ToolTipIcon.Info);
                    break;
            }
        }

        /// <summary>Starts Seven Update UI</summary>
        private static void RunSevenUpdate(object sender, EventArgs e)
        {
            var proc = new Process();

            if (Environment.Version.Major < 6)
            {
                proc.StartInfo.FileName = Base.Base.ConvertPath(@"%PROGRAMFILES%\Seven Software\Seven Update\Seven Update.exe", true, true);
                proc.StartInfo.UseShellExecute = true;
                if (NotifyIcon.Text == RM.GetString("UpdatesFoundViewThem") || NotifyIcon.Text == RM.GetString("UpdatesDownloadedViewThem"))
                    proc.StartInfo.Arguments = "Auto";
                else
                    proc.StartInfo.Arguments = "Reconnect";
                proc.Start();
            }
            else
            {
                proc.StartInfo.FileName = Base.Base.ConvertPath(@"%WINDIR%\system32\schtasks.exe", true, true);

                proc.StartInfo.Verb = "runas";

                proc.StartInfo.UseShellExecute = true;

                proc.StartInfo.Arguments = "/Run /TN \"Seven Update\"";

                proc.StartInfo.CreateNoWindow = true;

                proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;

                proc.Start();
            }

            if (NotifyIcon.Text == RM.GetString("UpdatesFoundViewThem") || NotifyIcon.Text == RM.GetString("UpdatesDownloadedViewThem"))
                Environment.Exit(0);
        }

        #region Security

        /// <summary>Sets the ACLS of a file, removes the current user and sets the owner as the Administrators group</summary>
        /// <param name="file">The complete path to the file</param>
        internal static void SetFileSecurity(string file)
        {
            try
            {
                IdentityReference userAdmin = new NTAccount("Administrators");

                IdentityReference user = new NTAccount(Environment.UserDomainName, Environment.UserName);

                var fs = new FileSecurity(file, AccessControlSections.All);

                fs.SetOwner(userAdmin);

                try
                {
                    IdentityReference users = new NTAccount("Users");

                    fs.AddAccessRule(new FileSystemAccessRule(users, FileSystemRights.ReadAndExecute, AccessControlType.Allow));
                }
                catch (Exception)
                {
                }

                fs.PurgeAccessRules(user);

                try
                {
                    File.SetAccessControl(file, fs);
                }
                catch (Exception)
                {
                }
            }
            catch (Exception)
            {
            }
        }

        #endregion

        #endregion

        #region Event Methods

        /// <summary>Prevents the system from shutting down until the installation is safely stopped</summary>
        private static void SystemEvents_SessionEnding(object sender, SessionEndingEventArgs e)
        {
            using (FileStream fs = File.Create(Base.Base.AllUserStore + "abort.lock"))
            {
                fs.WriteByte(0);
                fs.Close();
            }
            e.Cancel = true;
        }

        /// <summary>Occurs when Seven Update UI connects to the admin process</summary>
        private static void EventService_ClientConnected()
        {
            IsClientConnected = true;
            if (File.Exists(Base.Base.UserStore + "Updates.sui"))
                Download.DownloadUpdates(Base.Base.Deserialize<Collection<SUI>>(Base.Base.UserStore + "Updates.sui"), JobPriority.ForeGround);
        }

        /// <summary>Occurs when the Seven Update UI disconnected</summary>
        private static void EventService_ClientDisconnected()
        {
            IsClientConnected = false;
        }

        /// <summary>Error Event when the .NetPipe binding faults</summary>
        private static void HostFaulted(object sender, EventArgs e)
        {
            IsClientConnected = false;
            Base.Base.ReportError("Host Fault", Base.Base.AllUserStore);
        }

        /// <summary>Runs when the search for updates has completed for an auto update</summary>
        private static void Search_SearchDoneEventHandler(object sender, SearchCompletedEventArgs e)
        {
            if (e.Applications.Count > 0)
            {
                if (Settings.AutoOption == AutoUpdateOption.Notify)
                    Application.Current.Dispatcher.BeginInvoke(UpdateNotifyIcon, NotifyType.SearchComplete);
                else
                {
                    Application.Current.Dispatcher.BeginInvoke(UpdateNotifyIcon, NotifyType.DownloadStarted);
                    Download.DownloadUpdates(e.Applications, JobPriority.Normal);
                }
            }
            else
                Environment.Exit(0);
        }

        #endregion
    }
}