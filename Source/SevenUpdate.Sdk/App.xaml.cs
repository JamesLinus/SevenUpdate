// <copyright file="App.xaml.cs" project="SevenUpdate.Sdk">Robert Baker</copyright>
// <license href="http://www.gnu.org/licenses/gpl-3.0.txt" name="GNU General Public License 3" />

namespace SevenUpdate.Sdk
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Globalization;
    using System.IO;
    using System.Windows;
    using System.Windows.Shell;

    using SevenSoftware.Windows;
    using SevenSoftware.Windows.ApplicationServices;

    using SevenUpdate.Sdk.Properties;

    /// <summary>Interaction logic for App.xaml.</summary>
    public sealed partial class App
    {
        /// <summary>The user application data location.</summary>
        public static readonly string UserStore =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Seven Update SDK");

        /// <summary>Gets the command line arguments passed to this instance.</summary>
        internal static IList<string> Args { get; private set; }

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
                case @"-newproject":
                    Core.NewProject();
                    break;

                case @"-newupdate":
                    Core.AppIndex = Convert.ToInt32(args[1], CultureInfo.CurrentCulture);

                    Core.NewUpdate();
                    break;

                case @"-edit":
                    Core.AppIndex = Convert.ToInt32(args[1], CultureInfo.CurrentCulture);
                    Core.UpdateIndex = Convert.ToInt32(args[2], CultureInfo.CurrentCulture);
                    Core.EditItem();
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
            Utilities.Locale = Settings.Default.Locale;
            base.OnStartup(e, isFirstInstance);

            if (!isFirstInstance)
            {
                this.Shutdown(1);
            }
            else
            {
                RegisterApplicationRecoveryAndRestart();
                Args = e.Args;
                Directory.CreateDirectory(UserStore);
                Core.Projects = File.Exists(Core.ProjectsFile)
                                    ? Utilities.Deserialize<Collection<Project>>(Core.ProjectsFile) : null;
                SetJumpList();
            }
        }

        /// <summary>Raises the <c>InstanceAwareApplication.StartupNextInstance</c> event.</summary>
        /// <param name="e">The <c>StartupNextInstanceEventArgs</c> instance containing the event data.</param>
        protected override void OnStartupNextInstance(StartupNextInstanceEventArgs e)
        {
            base.OnStartupNextInstance(e);
            ProcessArgs(e.GetArgs());
        }

        /// <summary>Performs recovery by saving the state.</summary>
        /// <param name="parameter">This parameter is not used.</param>
        /// <returns>Return value is not used.</returns>
        static int PerformRecovery(object parameter)
        {
            try
            {
                ApplicationRestartRecoveryManager.ApplicationRecoveryInProgress();
                Core.SaveProject();

                // Save your work here for recovery
                ApplicationRestartRecoveryManager.ApplicationRecoveryFinished(true);
            }
            catch
            {
                ApplicationRestartRecoveryManager.ApplicationRecoveryFinished(false);
            }

            return 0;
        }

        /// <summary>Registers the application to use the Recovery Manager.</summary>
        static void RegisterApplicationRecoveryAndRestart()
        {
            if (Environment.OSVersion.Version.Major < 6)
            {
                return;
            }

            // register for Application Restart
            ApplicationRestartRecoveryManager.RegisterForApplicationRestart(
                new RestartSettings(string.Empty, RestartRestrictions.NotOnReboot));

            // register for Application Recovery
            var recoverySettings = new RecoverySettings(new RecoveryData(PerformRecovery, null), 4000);
            ApplicationRestartRecoveryManager.RegisterForApplicationRecovery(recoverySettings);
        }

        /// <summary>Sets the Windows 7 <c>JumpList</c>.</summary>
        static void SetJumpList()
        {
            // Create JumpTask
            var jumpList = new JumpList { ShowRecentCategory = true };

            // Configure a new JumpTask
            var jumpTask = new JumpTask
                {
                    IconResourcePath =
                        Path.Combine(Directory.GetParent(Utilities.AppDir).FullName, "Shared", @"SevenUpdate.Base.dll"), 
                    IconResourceIndex = 6, 
                    Title = Sdk.Properties.Resources.CreateProject, 
                    Arguments = @"-newproject"
                };

            jumpList.JumpItems.Add(jumpTask);
            JumpList.SetJumpList(Current, jumpList);
        }

        /// <summary>The unregister application recovery and restart.</summary>
        static void UnregisterApplicationRecoveryAndRestart()
        {
            if (Environment.OSVersion.Version.Major < 6)
            {
                return;
            }

            ApplicationRestartRecoveryManager.UnregisterApplicationRestart();
            ApplicationRestartRecoveryManager.UnregisterApplicationRecovery();
        }
    }
}