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
//     along with Seven Update.  If not, see <http://www.gnu.org/licenses/>.

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
using SevenUpdate.Properties;
using SevenUpdate.WCF;
using SharpBits.Base;
using Application=System.Windows.Application;

#endregion

namespace SevenUpdate
{
    internal class App
    {
        #region Enums

        internal enum NotifyType
        {
            /// <summary>
            /// Indicates searching is completed
            /// </summary>
            SearchComplete,
            /// <summary>
            /// Indicates the downloading of updates has started
            /// </summary>
            DownloadStarted,
            /// <summary>
            /// Indicates download has completed
            /// </summary>
            DownloadComplete,
            /// <summary>
            /// Indicates that the installation of updates has begun
            /// </summary>
            InstallStarted,
            /// <summary>
            /// Indicates Install has completed
            /// </summary>
            InstallComplete
        }

        #endregion

        #region Global Vars

        /// <summary>
        /// The notifyIcon used only when Auto Updating
        /// </summary>
        internal static NotifyIcon NotifyIcon = new NotifyIcon();

        /// <summary>
        /// The UI Resource Strings
        /// </summary>
        internal static ResourceManager RM = new ResourceManager("SevenUpdate.Resources.UIStrings", typeof (App).Assembly);

        /// <summary>
        /// The update settings for Seven Update
        /// </summary>
        public static Config Settings { get { return Shared.DeserializeStruct<Config>(Shared.ConfigFile); } }

        /// <summary>
        /// Indicates if the Seven Update UI is currently connected.
        /// </summary>
        internal static bool IsClientConnected { get; set; }

        internal static bool IsInstallAborted { get; set; }

        #endregion

        [STAThread]
        private static void Main(string[] args)
        {
            bool createdNew;
            var host = new ServiceHost(typeof (EventService));
            using (new Mutex(true, "Seven Update.Admin", out createdNew))
            {
                try
                {
                    host.Open();
                    EventService.ClientConnected += EventService_ClientConnected;
                    EventService.ClientDisconnected += EventService_ClientDisconnected;
                    host.Faulted += HostFaulted;
                    SystemEvents.SessionEnding += SystemEvents_SessionEnding;
                }
                catch (Exception e)
                {
                    host.Close();
                    Shared.ReportError(e.Message, Shared.AllUserStore);
                    Environment.Exit(0);
                }

                Shared.Locale = Shared.Locale == null ? "en" : Settings.Locale;

                if (!Directory.Exists(Shared.AllUserStore)) Directory.CreateDirectory(Shared.AllUserStore);

                NotifyIcon.Icon = Resources.icon;
                NotifyIcon.Visible = false;

                #region Arguments

                try
                {
                    if (args.Length > 0)
                    {
                        switch (args[0])
                        {
                            case "sua":

                                #region code

                                if (File.Exists(Shared.UserStore + "Apps.sul"))
                                {
                                    File.Delete(Shared.AppsFile);

                                    File.Move(Shared.UserStore + "Apps.sul", Shared.AppsFile);

                                    SetFileSecurity(Shared.AppsFile);
                                }

                                #endregion

                                break;
                            case "Options-On":

                                #region code

                                if (File.Exists(Shared.UserStore + "App.config"))
                                {
                                    File.Delete(Shared.ConfigFile);

                                    File.Move(Shared.UserStore + "App.config", Shared.ConfigFile);

                                    SetFileSecurity(Shared.ConfigFile);
                                }

                                if (File.Exists(Shared.UserStore + "Apps.sul"))
                                {
                                    File.Delete(Shared.AppsFile);
                                    File.Move(Shared.UserStore + "Apps.sul", Shared.AppsFile);
                                }

                                SetFileSecurity(Shared.AppsFile);

                                if (Environment.OSVersion.Version.Major < 6)
                                    Registry.SetValue(@"HKEY_LOCAL_MACHINE\Software\Microsoft\Windows\CurrentVersion\Run", "Seven Update Automatic Checking",
                                                      Environment.CurrentDirectory + @"\Seven Update.Helper.exe ");
                                else
                                {
                                    var proc = new Process
                                                   {
                                                       StartInfo =
                                                           {
                                                               FileName = Shared.ConvertPath(@"%WINDIR%\system32\schtasks.exe", true, true),
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

                                if (File.Exists(Shared.UserStore + "App.config"))
                                {
                                    File.Delete(Shared.ConfigFile);

                                    File.Move(Shared.UserStore + "App.config", Shared.ConfigFile);

                                    SetFileSecurity(Shared.ConfigFile);
                                }
                                if (File.Exists(Shared.UserStore + "Apps.sul"))
                                {
                                    File.Delete(Shared.AppsFile);

                                    File.Move(Shared.UserStore + "Apps.sul", Shared.AppsFile);

                                    SetFileSecurity(Shared.AppsFile);
                                }
                                if (Environment.OSVersion.Version.Major < 6)
                                {
// ReSharper disable PossibleNullReferenceException
                                    Registry.LocalMachine.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", true).DeleteValue( // ReSharper restore PossibleNullReferenceException
                                        "Seven Update Automatic Checking", false);
                                }
                                else
                                {
                                    var proc = new Process
                                                   {
                                                       StartInfo =
                                                           {
                                                               FileName = Shared.ConvertPath(@"%WINDIR%\system32\schtasks.exe", true, true),
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

                                var hidden = Shared.Deserialize<Collection<SUH>>(Shared.HiddenFile);

                                hidden.Add(Shared.Deserialize<SUH>(Shared.UserStore + "HnH Update.xml"));

                                File.Delete(Shared.UserStore + "HnH Update.xml");

                                Shared.Serialize(hidden, Shared.HiddenFile);

                                SetFileSecurity(Shared.HiddenFile);

                                #endregion

                                break;
                            case "ShowUpdate":

                                #region code

                                var show = Shared.Deserialize<Collection<SUH>>(Shared.HiddenFile);

                                show.Remove(Shared.Deserialize<SUH>(Shared.UserStore + "HnH Update.xml"));

                                File.Delete(Shared.UserStore + "HnH Update.xml");

                                if (show.Count == 0) File.Delete(Shared.HiddenFile);
                                else Shared.Serialize(show, Shared.HiddenFile);

                                #endregion

                                break;
                            case "HideUpdates":

                                #region code

                                File.Delete(Shared.HiddenFile);

                                File.Move(Shared.UserStore + "Hidden Updates.xml", Shared.HiddenFile);

                                SetFileSecurity(Shared.HiddenFile);

                                #endregion

                                break;
                            case "Auto":

                                #region code

                                if (createdNew)
                                {
                                    NotifyIcon.Text = RM.GetString("CheckingForUpdates") + "...";
                                    NotifyIcon.Visible = true;
                                    Search.SearchDoneEventHandler += Search_SearchDoneEventHandler;
                                    Search.SearchForUpdates(Shared.Deserialize<Collection<SUA>>(Shared.AppsFile));
                                    var app = new Application();
                                    app.Run();
                                }
                                else Environment.Exit(0);

                                #endregion

                                break;
                            case "Install":

                                #region code

                                if (createdNew)
                                {
                                    try
                                    {
                                        var app = new Application();
                                        app.Run();
                                    }
                                    catch (Exception e)
                                    {
                                        Shared.ReportError(e.Message, Shared.AllUserStore);
                                    }
                                }
                                else Environment.Exit(0);

                                #endregion

                                break;
                        }
                    }
                }
                catch (Exception e)
                {
                    Shared.ReportError(e.Message, Shared.AllUserStore);
                }

                #endregion
            }
            host.Close();
            SystemEvents.SessionEnding -= SystemEvents_SessionEnding;
        }

        #region Methods

        /// <summary>
        /// Updates the notify icon text
        /// </summary>
        /// <param name="text">A string to set the notifyIcon text</param>
        internal static void UpdateNotifyIcon(string text)
        {
            NotifyIcon.Text = text;
        }

        /// <summary>
        /// Updates the notifyIcon state
        /// </summary>
        /// <param name="filter">Indicates how to update the icon depending on the event that happened</param>
        internal static void UpdateNotifyIcon(NotifyType filter)
        {
            if (!NotifyIcon.Visible) return;
            switch (filter)
            {
                case NotifyType.DownloadStarted:
                    NotifyIcon.Text = RM.GetString("DownloadingUpdates") + "...";
                    break;
                case NotifyType.DownloadComplete:
                    NotifyIcon.BalloonTipClicked += RunSevenUpdate;
                    NotifyIcon.Click += RunSevenUpdate;
                    NotifyIcon.Text = RM.GetString("UpdatesDownloadedViewThem");
                    NotifyIcon.ShowBalloonTip(5000, RM.GetString("UpdatesDownloaded"), // ReSharper disable AssignNullToNotNullAttribute
                                              RM.GetString("UpdatesDownloadedViewThem"), ToolTipIcon.Info);
// ReSharper restore AssignNullToNotNullAttribute
                    break;
                case NotifyType.InstallStarted:
                    NotifyIcon.Text = RM.GetString("InstallingUpdates") + "...";
                    break;
                case NotifyType.SearchComplete:
                    NotifyIcon.Text = RM.GetString("UpdatesFoundViewThem");
                    NotifyIcon.BalloonTipClicked += RunSevenUpdate;
                    NotifyIcon.Click += RunSevenUpdate;
                    NotifyIcon.ShowBalloonTip(5000, RM.GetString("UpdatesFound"), // ReSharper disable AssignNullToNotNullAttribute
                                              RM.GetString("UpdatesFoundViewThem"), ToolTipIcon.Info);
// ReSharper restore AssignNullToNotNullAttribute
                    break;
            }
        }

        /// <summary>
        /// Starts Seven Update UI
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void RunSevenUpdate(object sender, EventArgs e)
        {
            var proc = new Process();

            if (Environment.GetCommandLineArgs()[0] == "Auto")
            {
                proc.StartInfo.FileName = Shared.ConvertPath(@"%WINDIR%\system32\schtasks.exe", true, true);

                proc.StartInfo.Verb = "runas";

                proc.StartInfo.UseShellExecute = true;

                proc.StartInfo.Arguments = "/Run /TN \"Seven Update\"";

                proc.StartInfo.CreateNoWindow = true;

                proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                proc.Start();
                Environment.Exit(0);
            }
            else
            {
                proc.StartInfo.FileName = Shared.ConvertPath(@"%PROGRAMFILES%\Seven Update\Seven Update.exe", true, true);
                proc.StartInfo.UseShellExecute = true;
                proc.StartInfo.Arguments = "Reconnect";
                proc.Start();
            }
        }

        #region Security

        /// <summary>
        /// Sets the ACLS of a file, removes the current user and sets the owner as the Administrators group
        /// </summary>
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
                catch {}

                fs.PurgeAccessRules(user);

                try
                {
                    File.SetAccessControl(file, fs);
                }
                catch {}
            }
            catch {}
        }

        #endregion

        #endregion

        #region Event Methods

        private static void SystemEvents_SessionEnding(object sender, SessionEndingEventArgs e)
        {
            Install.Abort = true;
            e.Cancel = true;
        }

        /// <summary>
        /// Occurs when Seven Update UI connects to the admin process
        /// </summary>
        private static void EventService_ClientConnected()
        {
            IsClientConnected = true;
            Download.DownloadUpdates(Shared.Deserialize<Collection<SUI>>(Shared.UserStore + "Update List.xml"), JobPriority.ForeGround);
        }

        /// <summary>
        /// Occurs when the Seven Update UI disconnected
        /// </summary>
        private static void EventService_ClientDisconnected()
        {
            IsClientConnected = false;
        }

        /// <summary>
        /// Errror Event when the .NetPipe binding faults
        /// </summary>
        private static void HostFaulted(object sender, EventArgs e)
        {
            IsClientConnected = false;
            Shared.ReportError("Host Fault", Shared.AllUserStore);
        }

        /// <summary>
        /// Runs when the search for updates has completed for an autoupdate
        /// </summary>
        private static void Search_SearchDoneEventHandler(object sender, Search.SearchDoneEventArgs e)
        {
            if (e.Applications.Count > 0)
            {
                if (Settings.AutoOption == AutoUpdateOption.Notify) Application.Current.Dispatcher.BeginInvoke(UpdateNotifyIcon, NotifyType.SearchComplete);
                else
                {
                    Application.Current.Dispatcher.BeginInvoke(UpdateNotifyIcon, NotifyType.DownloadStarted);
                    Download.DownloadUpdates(e.Applications, JobPriority.Normal);
                }
            }
            else Environment.Exit(0);
        }

        #endregion
    }
}