/*Copyright 2007-09 Robert Baker, aka Seven ALive.
This file is part of Seven Update.

    Seven Update is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    Seven Update is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with Seven Update.  If not, see <http://www.gnu.org/licenses/>.*/
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Resources;
using System.Security.AccessControl;
using System.Security.Principal;
using System.ServiceModel;
using System.Threading;
using System.Windows;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using SevenUpdate.WCF;

namespace SevenUpdate
{
    class App
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
        /// The update settings for Seven Update
        /// </summary>
        public static Config Settings
        {
            get { return Shared.DeserializeStruct<Config>(Shared.appStore + "Settings.xml"); }
        }

        /// <summary>
        /// Indicates if the Seven Update UI is currently connected.
        /// </summary>
        internal static bool IsClientConnected { get; set; }

        internal static bool IsInstallAborted { get; set; }

        /// <summary>
        /// The notifyIcon used only when Auto Updating
        /// </summary>
        internal static System.Windows.Forms.NotifyIcon NotifyIcon = new System.Windows.Forms.NotifyIcon();

        /// <summary>
        /// The UI Resource Strings
        /// </summary>
        internal static ResourceManager RM = new ResourceManager("SevenUpdate.Resources.UIStrings", typeof(App).Assembly);
        #endregion

        [STAThread]
        static void Main(string[] args)
        {
            bool createdNew;
            ServiceHost host = new ServiceHost(typeof(EventService)); ;
            using (Mutex mutex = new Mutex(true, "Seven Update.Admin", out createdNew))
            {

                try
                {

                    host.Open();
                    EventService.ClientConnected += new EventService.CallbackDelegate(EventService_ClientConnected);
                    EventService.ClientDisconnected += new EventService.CallbackDelegate(EventService_ClientDisconnected);
                    host.Faulted += new EventHandler(host_Faulted);
                    SystemEvents.SessionEnding += new SessionEndingEventHandler(SystemEvents_SessionEnding);

                }
                catch (Exception e)
                {
                    if (host != null)
                        host.Close();
                    Shared.ReportError(e.Message, Shared.appStore);
                    Environment.Exit(0);
                }

                if (Shared.Locale == null)
                    Shared.Locale = "en";
                else
                    Shared.Locale = Settings.Locale;

                if (!Directory.Exists(Shared.appStore))
                    Directory.CreateDirectory(Shared.appStore);

                NotifyIcon.Icon = SevenUpdate.Properties.Resources.icon;
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
                                if (File.Exists(Shared.userStore + "SUApps.sul"))
                                {
                                    File.Delete(Shared.appStore + "SUApps.sul");

                                    File.Move(Shared.userStore + "SUApps.sul", Shared.appStore + "SUApps.sul");

                                    SetFileSecurity(Shared.appStore + "SUApps.sul");
                                }
                                #endregion
                                break;
                            case "Options-On":
                                #region code
                                if (File.Exists(Shared.userStore + "Settings.xml"))
                                {
                                    File.Delete(Shared.appStore + "Settings.xml");

                                    File.Move(Shared.userStore + "Settings.xml", Shared.appStore + "Settings.xml");

                                    SetFileSecurity(Shared.appStore + "Settings.xml");
                                }

                                if (File.Exists(Shared.userStore + "SUApps.sul"))
                                {
                                    File.Delete(Shared.appStore + "SUApps.sul");
                                    File.Move(Shared.userStore + "SUApps.sul", Shared.appStore + "SUApps.sul");
                                }

                                SetFileSecurity(Shared.appStore + "SUApps.sul");

                                if (Environment.OSVersion.Version.Major < 6)
                                {
                                    Registry.SetValue(@"HKEY_LOCAL_MACHINE\Software\Microsoft\Windows\CurrentVersion\Run", "Seven Update Automatic Checking", Environment.CurrentDirectory + @"\Seven Update.Helper.exe ");
                                }
                                else
                                {
                                    Process proc = new Process();

                                    proc.StartInfo.FileName = Shared.ConvertPath(@"%WINDIR%\system32\schtasks.exe", true, true);

                                    proc.StartInfo.Verb = "runas";

                                    proc.StartInfo.UseShellExecute = true;

                                    proc.StartInfo.Arguments = "/Change /Enable /TN \"Seven Update.Admin\"";

                                    proc.StartInfo.CreateNoWindow = true;

                                    proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;

                                    proc.Start();
                                }
                                #endregion
                                break;
                            case "Options-Off":
                                #region code
                                if (File.Exists(Shared.userStore + "Settings.xml"))
                                {
                                    File.Delete(Shared.appStore + "Settings.xml");

                                    File.Move(Shared.userStore + "Settings.xml", Shared.appStore + "Settings.xml");

                                    SetFileSecurity(Shared.appStore + "Settings.xml");
                                }
                                if (File.Exists(Shared.userStore + "SUApps.sul"))
                                {
                                    File.Delete(Shared.appStore + "SUApps.sul");

                                    File.Move(Shared.userStore + "SUApps.sul", Shared.appStore + "SUApps.sul");

                                    SetFileSecurity(Shared.appStore + "SUApps.sul");
                                }
                                if (Environment.OSVersion.Version.Major < 6)
                                {
                                    Registry.LocalMachine.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", true).DeleteValue("Seven Update Automatic Checking", false);
                                }
                                else
                                {
                                    Process proc = new Process();

                                    proc.StartInfo.FileName = Shared.ConvertPath(@"%WINDIR%\system32\schtasks.exe", true, true);

                                    proc.StartInfo.Verb = "runas";

                                    proc.StartInfo.UseShellExecute = true;

                                    proc.StartInfo.Arguments = "/Change /Disable /TN \"Seven Update.Admin\"";

                                    proc.StartInfo.CreateNoWindow = true;

                                    proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;

                                    proc.Start();
                                }
                                #endregion
                                break;
                            case "HideUpdate":
                                #region code
                                ObservableCollection<UpdateInformation> hidden = Shared.DeserializeCollection<UpdateInformation>(Shared.appStore + "Hidden Updates.xml");

                                hidden.Add(Shared.Deserialize<UpdateInformation>(Shared.userStore + "HnH Update.xml"));

                                File.Delete(Shared.userStore + "HnH Update.xml");

                                Shared.SerializeCollection<UpdateInformation>(hidden, Shared.appStore + "Hidden Updates.xml");

                                SetFileSecurity(Shared.appStore + "Hidden Updates.xml");
                                #endregion
                                break;
                            case "ShowUpdate":
                                #region code
                                ObservableCollection<UpdateInformation> show = Shared.DeserializeCollection<UpdateInformation>(Shared.appStore + "Hidden Updates.xml");

                                show.Remove(Shared.Deserialize<UpdateInformation>(Shared.userStore + "HnH Update.xml"));

                                File.Delete(Shared.userStore + "HnH Update.xml");

                                if (show.Count == 0)
                                    File.Delete(Shared.appStore + "Hidden Updates.xml");
                                else
                                    Shared.SerializeCollection<UpdateInformation>(show, Shared.appStore + "Hidden Updates.xml");
                                #endregion
                                break;
                            case "HideUpdates":
                                #region code
                                File.Delete(Shared.appStore + "Hidden Updates.xml");

                                File.Move(Shared.userStore + "Hidden Updates.xml", Shared.appStore + "Hidden Updates.xml");

                                SetFileSecurity(Shared.appStore + "Hidden Updates.xml");
                                #endregion
                                break;
                            case "Auto":
                                #region code
                                if (createdNew)
                                {
                                    NotifyIcon.Text = RM.GetString("CheckingForUpdates") + "...";
                                    NotifyIcon.Visible = true;
                                    Search.SearchDoneEventHandler += new EventHandler<Search.SearchDoneEventArgs>(Search_SearchDoneEventHandler);
                                    Search.SearchForUpdates(Shared.DeserializeCollection<SUA>(Shared.appStore + "SUApps.sul"));
                                    System.Windows.Application app = new System.Windows.Application();
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
                                        System.Windows.Application app = new System.Windows.Application();
                                        app.Run();
                                    }
                                    catch (Exception e)
                                    {
                                        Shared.ReportError(e.Message, Shared.appStore);
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
                    Shared.ReportError(e.Message, Shared.appStore);
                }
                #endregion
            }
            if (host != null)
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
            if (NotifyIcon.Visible)
            {
                switch (filter)
                {
                    case NotifyType.DownloadStarted:
                        NotifyIcon.Text = RM.GetString("DownloadingUpdates") + "...";
                        break;
                    case NotifyType.DownloadComplete:
                        NotifyIcon.BalloonTipClicked += new EventHandler(RunSevenUpdate);
                        NotifyIcon.Click += new EventHandler(RunSevenUpdate);
                        NotifyIcon.Text = RM.GetString("UpdatesDownloadedViewThem");
                        NotifyIcon.ShowBalloonTip(5000, RM.GetString("UpdatesDownloaded"), RM.GetString("UpdatesDownloadedViewThem"), System.Windows.Forms.ToolTipIcon.Info);
                        break;
                    case NotifyType.InstallStarted: NotifyIcon.Text = RM.GetString("InstallingUpdates") + "..."; break;
                    case NotifyType.SearchComplete:
                        NotifyIcon.Text = RM.GetString("UpdatesFoundViewThem");
                        NotifyIcon.BalloonTipClicked += new EventHandler(RunSevenUpdate);
                        NotifyIcon.Click += new EventHandler(RunSevenUpdate);
                        NotifyIcon.ShowBalloonTip(5000, RM.GetString("UpdatesFound"), RM.GetString("UpdatesFoundViewThem"), System.Windows.Forms.ToolTipIcon.Info);
                        break;
                }
            }
        }

        /// <summary>
        /// Starts Seven Update UI
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static void RunSevenUpdate(object sender, EventArgs e)
        {
            Process proc = new Process();

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

                FileSecurity fs = new FileSecurity(file, AccessControlSections.All);

                fs.SetOwner(userAdmin);

                try
                {
                    IdentityReference users = new NTAccount("Users");

                    fs.AddAccessRule(new FileSystemAccessRule(users, FileSystemRights.ReadAndExecute, AccessControlType.Allow));
                }
                catch (Exception) { }

                fs.PurgeAccessRules(user);

                try
                {
                    File.SetAccessControl(file, fs);
                }
                catch (Exception) { }
            }
            catch (Exception) { }
        }

        #endregion

        #endregion

        #region Event Methods

        static void SystemEvents_SessionEnding(object sender, SessionEndingEventArgs e)
        {
            Install.Abort = true;
            e.Cancel = true;
        }
        /// <summary>
        /// Occurs when Seven Update UI connects to the admin process
        /// </summary>
        static void EventService_ClientConnected()
        {
            IsClientConnected = true;
            Download.DownloadUpdates(Shared.DeserializeCollection<Application>(Shared.userStore + "Update List.xml"), SharpBits.Base.JobPriority.ForeGround);
        }

        /// <summary>
        /// Occurs when the Seven Update UI disconnected
        /// </summary>
        static void EventService_ClientDisconnected()
        {
            IsClientConnected = false;
        }
        /// <summary>
        /// Errror Event when the .NetPipe binding faults
        /// </summary>
        static void host_Faulted(object sender, EventArgs e)
        {
            IsClientConnected = false;
            Shared.ReportError("Host Fault", Shared.appStore);
        }

        /// <summary>
        /// Runs when the search for updates has completed for an autoupdate
        /// </summary>
        static void Search_SearchDoneEventHandler(object sender, Search.SearchDoneEventArgs e)
        {
            if (e.Applications.Count > 0)
            {
                if (Settings.AutoOption == AutoUpdateOption.Notify)
                {
                    DispatcherObjectDelegates.BeginInvoke<NotifyType>(System.Windows.Application.Current.Dispatcher, UpdateNotifyIcon, NotifyType.SearchComplete);
                }
                else
                {
                    DispatcherObjectDelegates.BeginInvoke<NotifyType>(System.Windows.Application.Current.Dispatcher, UpdateNotifyIcon, NotifyType.DownloadStarted);
                    Download.DownloadUpdates(e.Applications, SharpBits.Base.JobPriority.Normal);
                }
            }
            else
                Environment.Exit(0);
        }

        #endregion
    }
}