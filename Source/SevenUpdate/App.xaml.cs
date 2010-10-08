// ***********************************************************************
// <copyright file="App.xaml.cs"
//            project="SevenUpdate"
//            assembly="SevenUpdate"
//            solution="SevenUpdate"
//            company="Seven Software">
//     Copyright (c) Seven Software. All rights reserved.
// </copyright>
// <author username="sevenalive">Robert Baker</author>
// ***********************************************************************
namespace SevenUpdate
{
    using System;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Windows;
    using System.Windows.Dialogs.TaskDialogs;

    using SevenUpdate.Properties;

    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public sealed partial class App
    {
        #region Methods

        /// <summary>
        /// Gets the application ready for startup
        /// </summary>
        /// <param name="args">
        /// The command line arguments passed to the application
        /// </param>
        internal static void Init(string[] args)
        {
            Utilities.Locale = Settings.Default.locale;
            foreach (var t in args.Where(t => args[0].EndsWith(@".sua", StringComparison.OrdinalIgnoreCase)))
            {
                var suaLoc = t;
                try
                {
                    suaLoc = suaLoc.Replace(@"sevenupdate://", null);
                    var sua = Utilities.Deserialize<Sua>(Utilities.DownloadFile(suaLoc), suaLoc);
                    var appName = Utilities.GetLocaleString(sua.Name);
                    if (
                        Core.ShowMessage(
                            String.Format(CultureInfo.CurrentCulture, SevenUpdate.Properties.Resources.AddToSevenUpdate, appName), 
                            TaskDialogStandardIcon.ShieldBlue, 
                            TaskDialogStandardButtons.Cancel, 
                            String.Format(CultureInfo.CurrentCulture, SevenUpdate.Properties.Resources.AllowUpdates, appName), 
                            null, 
                            SevenUpdate.Properties.Resources.Add, 
                            true) != TaskDialogResult.Cancel)
                    {
                        AdminClient.AddSua(sua);
                    }
                }
                catch (Exception)
                {
                }

                Environment.Exit(0);
            }

            Directory.CreateDirectory(Utilities.UserStore);

            if (Process.GetProcessesByName("SevenUpdate.Admin").Length <= 0 || File.Exists(Utilities.AllUserStore + @"updates.sui"))
            {
                return;
            }

            Core.IsReconnect = true;
        }

        /// <summary>
        /// Raises the <see cref="InstanceAwareApplication.Startup"/> event.
        /// </summary>
        /// <param name="e">
        /// The <see cref="System.Windows.StartupEventArgs"/> instance containing the event data.
        /// </param>
        /// <param name="isFirstInstance">
        /// If set to <c>true</c> the current instance is the first application instance.
        /// </param>
        protected override void OnStartup(StartupEventArgs e, bool isFirstInstance)
        {
            base.OnStartup(e, isFirstInstance);
            Init(e.Args);
            Core.SetJumpList();
            if (!isFirstInstance)
            {
                this.Shutdown(1);
            }
        }

        /// <summary>
        /// Raises the <see cref="InstanceAwareApplication.StartupNextInstance"/> event.
        /// </summary>
        /// <param name="e">
        /// The <see cref="StartupNextInstanceEventArgs"/> instance containing the event data.
        /// </param>
        protected override void OnStartupNextInstance(StartupNextInstanceEventArgs e)
        {
            base.OnStartupNextInstance(e);

            if (e.GetArgs().Length <= 0)
            {
                return;
            }

            switch (e.GetArgs()[0])
            {
                case "-check":
                    Core.NavService.Navigate(new Uri(@"Pages\Main.xaml", UriKind.Relative));
                    Core.CheckForUpdates(true);
                    break;
                case "-history":
                    Core.NavService.Navigate(new Uri(@"Pages\UpdateHistory.xaml", UriKind.Relative));
                    break;
                case "-hidden":
                    Core.NavService.Navigate(new Uri(@"Pages\RestoreUpdates.xaml", UriKind.Relative));
                    break;
                case "-settings":
                    Core.NavService.Navigate(new Uri(@"Pages\Options.xaml", UriKind.Relative));
                    break;
            }
        }

        #endregion
    }
}