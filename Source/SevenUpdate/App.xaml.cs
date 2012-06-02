// <copyright file="App.xaml.cs" project="SevenUpdate">Robert Baker</copyright>
// <license href="http://www.gnu.org/licenses/gpl-3.0.txt" name="GNU General Public License 3" />

namespace SevenUpdate
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Net;
    using System.Windows;
    using System.Windows.Shell;

    using Microsoft.Win32;

    using SevenSoftware.Windows;
    using SevenSoftware.Windows.ApplicationServices;
    using SevenSoftware.Windows.Dialogs.TaskDialog;

    using SevenUpdate.Properties;

    /// <summary>Interaction logic for App.xaml.</summary>
    public sealed partial class App
    {
        /// <summary>The all users application data location.</summary>
        public static readonly string AllUserStore =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "Seven Update");

        /// <summary>The location of the list of applications Seven Update can update.</summary>
        public static readonly string ApplicationsFile = Path.Combine(AllUserStore, @"Apps.sul");

        /// <summary>The location of the application settings file.</summary>
        public static readonly string ConfigFile = Path.Combine(AllUserStore, @"App.config");

        /// <summary>The location of the hidden updates file.</summary>
        public static readonly string HiddenFile = Path.Combine(AllUserStore, @"Hidden.suh");

        /// <summary>The location of the update history file.</summary>
        public static readonly string HistoryFile = Path.Combine(AllUserStore, @"History.suh");

        /// <summary>The location of the user application data location.</summary>
        public static readonly string UserStore =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Seven Update");

        /// <summary>The Seven Update list location.</summary>
        internal const string SulLocation = @"http://apps.sevenupdate.com/list.sul";

        /// <summary>Gets the command line arguments passed to this instance.</summary>
        internal static IList<string> Args { get; private set; }

        /// <summary>Gets a value indicating whether Seven Update should be updated to the beta channel.</summary>
        internal static bool IsBeta { get; private set; }

        /// <summary>Gets a value indicating whether Seven Update should be updated to the dev channel.</summary>
        internal static bool IsDev { get; private set; }

        /// <summary>Gets or sets the application TaskBarItemInfo.</summary>
        internal static TaskbarItemInfo TaskBar { get; set; }

        /// <summary>Logs an error.</summary>
        /// <param name="sender">The object that called the event.</param>
        /// <param name="e">The error data to log.</param>
        internal static void LogError(object sender, ErrorOccurredEventArgs e)
        {
            using (var tw = new StreamWriter(Path.Combine(UserStore, "error.log"), true))
            {
                tw.WriteLine("<--Begin-->");
                tw.WriteLine(DateTime.Now + ":");
                tw.WriteLine(e.Exception);
                tw.WriteLine("<--End-->");
                tw.WriteLine();
            }
        }

        /// <summary>Process command line args.</summary>
        /// <param name="args">The list of arguments.</param>
        internal static void ProcessArgs(IList<string> args)
        {
            if (args == null)
            {
                return;
            }

            if (args.Count <= 0)
            {
                return;
            }

            switch (args[0])
            {
                case "-check":
                    Core.NavigateToMainPage();
                    Core.CheckForUpdates(true);
                    break;

                case "-history":
                    SevenUpdate.Windows.MainWindow.NavService.Navigate(
                        new Uri(@"/SevenUpdate;Component/Pages/UpdateHistory.xaml", UriKind.Relative));
                    break;

                case "-hidden":
                    SevenUpdate.Windows.MainWindow.NavService.Navigate(
                        new Uri(@"/SevenUpdate;Component/Pages/RestoreUpdates.xaml", UriKind.Relative));
                    break;

                case "-settings":
                    SevenUpdate.Windows.MainWindow.NavService.Navigate(
                        new Uri(@"/SevenUpdate;Component/Pages/Options.xaml", UriKind.Relative));
                    break;
            }
        }

        /// <summary>Raises the Application.Exit event.</summary>
        /// <param name="e">An ExitEventArgs that contains the event data.</param>
        protected override void OnExit(ExitEventArgs e)
        {
            UnregisterApplicationRecoveryAndRestart();
            base.OnExit(e);
        }

        /// <summary>Raises the <c>InstanceAwareApplication.Startup</c> event.</summary>
        /// <param name="e">The <c>System.Windows.StartupEventArgs</c> instance containing the event data.</param>
        /// <param name="isFirstInstance">If set to <c>True</c> the current instance is the first application instance.</param>
        protected override void OnStartup(StartupEventArgs e, bool isFirstInstance)
        {
            Init();
            if (e.Args.Length > 0)
            {
                if (e.Args[0].EndsWith(@".sua", StringComparison.OrdinalIgnoreCase))
                {
                    e.Args[0] = e.Args[0].Replace(@"sevenupdate://", null);
                    Sua app = null;
                    try
                    {
                        app = Utilities.Deserialize<Sua>(Utilities.DownloadFile(e.Args[0]));
                    }
                    catch (WebException)
                    {
                        Core.ShowMessage(
                            string.Format(
                                CultureInfo.CurrentCulture, SevenUpdate.Properties.Resources.ErrorDownloading, e.Args[0]), 
                            TaskDialogStandardIcon.Error, 
                            TaskDialogStandardButtons.Ok);
                        Environment.Exit(0);
                    }

                    string appName = Utilities.GetLocaleString(app.Name);
                    if (Utilities.IsRunning64BitOS == false && app.Platform == Platform.X64)
                    {
                        Core.ShowMessage(
                            string.Format(
                                CultureInfo.CurrentCulture, SevenUpdate.Properties.Resources.Not64BitCompat, appName), 
                            TaskDialogStandardIcon.Error, 
                            TaskDialogStandardButtons.Ok);
                        Environment.Exit(0);
                    }

                    TaskDialogResult result =
                        Core.ShowMessage(
                            string.Format(
                                CultureInfo.CurrentCulture, SevenUpdate.Properties.Resources.AddToSevenUpdate, appName), 
                            TaskDialogStandardIcon.ShieldBlue, 
                            TaskDialogStandardButtons.Cancel, 
                            string.Format(
                                CultureInfo.CurrentCulture, SevenUpdate.Properties.Resources.AllowUpdates, appName), 
                            null, 
                            SevenUpdate.Properties.Resources.Add, 
                            true);

                    if (result != TaskDialogResult.Cancel)
                    {
                        app.IsEnabled = true;
                        WcfService.AddSua(app);
                    }
                    else
                    {
                        Environment.Exit(0);
                    }
                }
            }

            base.OnStartup(e, isFirstInstance);

            if (!isFirstInstance)
            {
                this.Shutdown(1);
            }
            else
            {
                RegisterApplicationRecoveryAndRestart();
                MyServiceHost.StartService();
                Args = e.Args;
                SetJumpList();
                Utilities.ErrorOccurred += LogError;
            }
        }

        /// <summary>Raises the <c>InstanceAwareApplication.StartupNextInstance</c> event.</summary>
        /// <param name="e">The <c>StartupNextInstanceEventArgs</c> instance containing the event data.</param>
        protected override void OnStartupNextInstance(StartupNextInstanceEventArgs e)
        {
            base.OnStartupNextInstance(e);
            ProcessArgs(e.GetArgs());
        }

        /// <summary>Gets the application ready for startup.</summary>
        private static void Init()
        {
            Utilities.Locale = Settings.Default.Locale;

            Directory.CreateDirectory(UserStore);

            try
            {
                string channel =
                    Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Seven Update", "channel", null).ToString();

                if (channel == "dev")
                {
                    IsDev = true;
                }

                if (channel == "beta")
                {
                    IsBeta = true;
                }
            }
            catch (NullReferenceException)
            {
            }
            catch (AccessViolationException)
            {
            }

            if (Process.GetProcessesByName("SevenUpdate.Admin").Length <= 0
                || File.Exists(Path.Combine(AllUserStore, "updates.sui")))
            {
                return;
            }

            Core.IsReconnect = true;
        }

        /// <summary>Registers the application to use the Recovery Manager.</summary>
        private static void RegisterApplicationRecoveryAndRestart()
        {
            if (Environment.OSVersion.Version.Major < 6)
            {
                return;
            }

            // register for Application Restart
            ApplicationRestartRecoveryManager.RegisterForApplicationRestart(
                new RestartSettings(string.Empty, RestartRestrictions.NotOnReboot));
        }

        /// <summary>Sets the application jump list.</summary>
        private static void SetJumpList()
        {
            var jumpList = new JumpList();

            var jumpTask = new JumpTask
                {
                    IconResourcePath = Path.Combine(Utilities.AppDir, "Shared", @"SevenUpdate.Base.dll"), 
                    IconResourceIndex = 2, 
                    Title = SevenUpdate.Properties.Resources.CheckForUpdates, 
                    Arguments = "-check", 
                };

            jumpList.JumpItems.Add(jumpTask);

            jumpTask = new JumpTask
                {
                    IconResourcePath = Path.Combine(Utilities.AppDir, "Shared", @"SevenUpdate.Base.dll"), 
                    IconResourceIndex = 5, 
                    Title = SevenUpdate.Properties.Resources.RestoreHiddenUpdates, 
                    Arguments = "-hidden"
                };

            jumpList.JumpItems.Add(jumpTask);

            jumpTask = new JumpTask
                {
                    IconResourcePath = Path.Combine(Utilities.AppDir, "Shared", @"SevenUpdate.Base.dll"), 
                    IconResourceIndex = 4, 
                    Title = SevenUpdate.Properties.Resources.ViewUpdateHistory, 
                    Arguments = "-history", 
                };

            jumpList.JumpItems.Add(jumpTask);

            jumpTask = new JumpTask
                {
                    IconResourcePath = Path.Combine(Utilities.AppDir, "Shared", @"SevenUpdate.Base.dll"), 
                    IconResourceIndex = 3, 
                    Title = SevenUpdate.Properties.Resources.ChangeSettings, 
                    Arguments = "-settings", 
                };

            jumpList.JumpItems.Add(jumpTask);

            JumpList.SetJumpList(Current, jumpList);
        }

        /// <summary>The unregister application recovery and restart.</summary>
        private static void UnregisterApplicationRecoveryAndRestart()
        {
            if (Environment.OSVersion.Version.Major < 6)
            {
                return;
            }

            ApplicationRestartRecoveryManager.UnregisterApplicationRestart();
        }
    }
}