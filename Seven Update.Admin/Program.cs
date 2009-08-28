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
using Microsoft.Win32;
using SevenUpdate.WCF;
using System.Windows;

namespace SevenUpdate
{
    class Program
    {
        #region Global Vars

        /// <summary>
        /// List of Application Seven Update can check for updates
        /// </summary>
        public static ObservableCollection<SUA> AppsToUpdate
        {
            get { return Shared.DeserializeCollection<SUA>(Shared.appStore + "SUApps.sul"); }
        }

        /// <summary>
        /// The update settings for Seven Update
        /// </summary>
        public static Config Settings
        {
            get { return Shared.DeserializeStruct<Config>(Shared.appStore + "Settings.xml"); }
        }

        /// <summary>
        /// Collection of updates and the program
        /// </summary>
        static Collection<Application> Applications { get; set; }

        /// <summary>
        /// Indicates if the current installation automatically started
        /// </summary>
        static bool AutoInstall { get; set; }

        internal static Avalon.Windows.Controls.NotifyIcon NotifyIcon { get; set; }

        /// <summary>
        /// The UI Resource Strings
        /// </summary>
        internal static ResourceManager RM = new ResourceManager("SevenUpdate.Resources.UIStrings", typeof(Program).Assembly);
        #endregion

        #region Methods
        [STAThread]
        static void Main(string[] args)
        {
            bool createdNew;
            using (Mutex mutex = new Mutex(true, "Seven Update.Admin", out createdNew))
            {
                try
                {
                    ServiceHost host = new ServiceHost(typeof(EventService));
                    host.Open();
                    host.Faulted += new EventHandler(host_Faulted);

                }
                catch (Exception e)
                {
                    ReportError(e.Message);
                    Environment.Exit(0);
                }
                if (!Directory.Exists(Shared.appStore))
                    Directory.CreateDirectory(Shared.appStore);
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
                                    Registry.SetValue(@"HKEY_LOCAL_MACHINE\Software\Microsoft\Windows\CurrentVersion\Run", "Seven Update Automatic Checking", Environment.CurrentDirectory + @"\SU Helper.exe ");
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
                                    //NotifyIcon = new System.Windows.Forms.NotifyIcon();
                                    //NotifyIcon.Icon = SevenUpdate.Properties.Resources.icon;
                                    //NotifyIcon.Visible = true;
                                    //NotifyIcon.Text = RM.GetString("CheckingForUpdates") + "...";
                                    AutoInstall = true;
                                    Search.SearchDoneEventHandler += new EventHandler<Search.SearchDoneEventArgs>(Search_SearchDoneEventHandler);
                                    Search.SearchForUpdates(AppsToUpdate);
                                    System.Windows.Application.Current.Run();
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
                                        EventService.ClientConnected += new EventService.CallbackDelegate(EventService_ClientConnected);
                                        EventService.ClientDisconnected += new EventService.CallbackDelegate(EventService_ClientDisconnected);
                                        System.Windows.Application.Current.Run();
                                    }
                                    catch (Exception e)
                                    {
                                        ReportError(e.Message);
                                    }
                                }
                                else
                                    Environment.Exit(0);
                                #endregion
                                break;
                            default:
                                #region code
                                //   Environment.Exit(1);
                                #endregion
                                break;
                        }
                    }
                    else
                    {
                        Environment.Exit(1);
                    }
                }
                catch (Exception e)
                {
                    ReportError(e.Message);
                }
            }
        }

        internal static void ReportError(string message)
        {
            TextWriter tw = new StreamWriter(Shared.appStore + "error.log");

            tw.WriteLine(DateTime.Now.ToString() + ": " + message);

            tw.Close();
        }

        static void host_Faulted(object sender, EventArgs e)
        {
            TextWriter tw = new StreamWriter(Shared.appStore + "error.log");

            tw.WriteLine(DateTime.Now.ToString() + ": " + "faulted");

            tw.Close();
        }

        static void worker_DoWork(object sender, DoWorkEventArgs e)
        {

            Applications = Shared.DeserializeCollection<Application>(Shared.appStore + "Update List.xml");
            Download.DownloadDoneEventHandler += new EventHandler<EventArgs>(Download_DownloadDoneEventHandler);
            Download.DownloadUpdates(Applications);
        }

        static void Search_SearchDoneEventHandler(object sender, Search.SearchDoneEventArgs e)
        {
            if (e.Applications.Count > 0)
            {
                Applications = e.Applications;
                if ((Settings.AutoOption == AutoUpdateOption.Download || Settings.AutoOption == AutoUpdateOption.Install && AutoInstall == true) || AutoInstall == false)
                {
                    NotifyIcon.Text = RM.GetString("DownloadingUpdates") + "...";
                    Download.DownloadDoneEventHandler += new EventHandler<EventArgs>(Download_DownloadDoneEventHandler);
                    Download.DownloadUpdates(e.Applications);
                }
                else
                {
                    NotifyIcon.Text = RM.GetString("UpdatesFoundViewThem");
                    NotifyIcon.BalloonTipClick += new RoutedEventHandler(RunSevenUpdate);
                    NotifyIcon.Click += new RoutedEventHandler(RunSevenUpdate);
                    NotifyIcon.ShowBalloonTip(5000, RM.GetString("UpdatesFound"), RM.GetString("UpdatesFoundViewThem"), Avalon.Windows.Controls.NotifyBalloonIcon.Info);
                }
            }
            else
                Environment.Exit(0);
        }

        static void RunSevenUpdate(object sender, RoutedEventArgs e)
        {
            Process proc = new Process();

            proc.StartInfo.FileName = Shared.ConvertPath(@"%WINDIR%\system32\schtasks.exe", true, true);

            proc.StartInfo.Verb = "runas";

            proc.StartInfo.UseShellExecute = true;

            proc.StartInfo.Arguments = "/Run /TN \"Seven Update\"";

            proc.StartInfo.CreateNoWindow = true;

            proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;

            proc.Start();
            Environment.Exit(0);
        }

        static void Download_DownloadDoneEventHandler(object sender, EventArgs e)
        {
            if (!Download.ErrorOccurred)
            {
                if ((Settings.AutoOption == AutoUpdateOption.Install && AutoInstall == true) || AutoInstall == false)
                {
                    if (AutoInstall)
                    {
                        NotifyIcon.Text = RM.GetString("InstallingUpdates") + "...";
                    }
                    Install.InstallUpdates(Applications);

                }
                else
                {
                    NotifyIcon.BalloonTipClick += new RoutedEventHandler(RunSevenUpdate);
                    NotifyIcon.Click += new RoutedEventHandler(RunSevenUpdate);
                    NotifyIcon.Text = RM.GetString("UpdatesDownloadedViewThem");
                    NotifyIcon.ShowBalloonTip(5000, RM.GetString("UpdatesDownloaded"), RM.GetString("UpdatesDownloadedViewThem"), Avalon.Windows.Controls.NotifyBalloonIcon.Info);
                }
            }
            else
                Environment.Exit(0);
        }

        static void EventService_ClientDisconnected()
        {
            Install.Abort = true;
            Download.Abort = true;
        }

        static void EventService_ClientConnected()
        {
            Applications = Shared.DeserializeCollection<Application>(Shared.appStore + "Update List.xml");
            Download.DownloadDoneEventHandler += new EventHandler<EventArgs>(Download_DownloadDoneEventHandler);
            Download.DownloadUpdates(Applications);
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

        /// <summary>
        /// Specifies if the current user running Seven Update is an administrator
        /// </summary>
        /// <returns></returns>
        static internal bool IsAdmin()
        {
            WindowsIdentity id = WindowsIdentity.GetCurrent();
            WindowsPrincipal p = new WindowsPrincipal(id);
            return p.IsInRole(WindowsBuiltInRole.Administrator);
        }

        #endregion

        #endregion
    }
}