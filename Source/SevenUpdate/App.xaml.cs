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
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using Microsoft.Windows;
using Microsoft.Windows.Dialogs;
using SevenUpdate.Properties;

#endregion

namespace SevenUpdate
{
    /// <summary>
    ///   Interaction logic for App.xaml
    /// </summary>
    public sealed partial class App
    {
        /// <summary>
        ///   Raises the <see cref = "InstanceAwareApplication.Startup" /> event.
        /// </summary>
        /// <param name = "e">The <see cref = "System.Windows.StartupEventArgs" /> instance containing the event data.</param>
        /// <param name = "isFirstInstance">If set to <c>true</c> the current instance is the first application instance.</param>
        protected override void OnStartup(StartupEventArgs e, bool isFirstInstance)
        {
            base.OnStartup(e, isFirstInstance);
            Init(e.Args);
            Core.SetJumpList();
            if (!isFirstInstance)
                Shutdown(1);
        }

        /// <summary>
        ///   Raises the <see cref = "InstanceAwareApplication.StartupNextInstance" /> event.
        /// </summary>
        /// <param name = "e">The <see cref = "Microsoft.Windows.StartupNextInstanceEventArgs" /> instance containing the event data.</param>
        protected override void OnStartupNextInstance(StartupNextInstanceEventArgs e)
        {
            base.OnStartupNextInstance(e);

            if (e.Args.Length <= 0)
                return;
            switch (e.Args[0])
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

        /// <summary>
        ///   Gets the application ready for startup
        /// </summary>
        /// <param name = "args">The command line arguments passed to the application</param>
        internal static void Init(string[] args)
        {
            Base.Locale = Settings.Default.locale;
            foreach (var t in args.Where(t => args[0].EndsWith(@".sua", StringComparison.OrdinalIgnoreCase)))
            {
                var suaLoc = t;
                try
                {
                    suaLoc = suaLoc.Replace(@"sevenupdate://", null);
                    var sua = Base.Deserialize<Sua>(Base.DownloadFile(suaLoc), suaLoc);
                    var appName = Base.GetLocaleString(sua.Name);
                    if (
                        Core.ShowMessage(String.Format(SevenUpdate.Properties.Resources.AddToSevenUpdate, appName), TaskDialogStandardIcon.ShieldBlue, TaskDialogStandardButtons.Cancel,
                                         String.Format(SevenUpdate.Properties.Resources.AllowUpdates, appName), null, SevenUpdate.Properties.Resources.Add, true) != TaskDialogResult.Cancel)
                        AdminClient.AddSua(sua);
                }
                catch
                {
                }
                Environment.Exit(0);
            }

            Directory.CreateDirectory(Base.UserStore);

            if (Process.GetProcessesByName("SevenUpdate.Admin").Length <= 0 || File.Exists(Base.AllUserStore + "updates.sui"))
                return;
            Core.IsReconnect = true;
        }
    }
}