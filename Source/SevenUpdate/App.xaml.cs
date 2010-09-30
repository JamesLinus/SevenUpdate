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
using Microsoft.Windows.Dialogs;
using Microsoft.Windows.Shell;
using SevenUpdate.Properties;
using SevenUpdate.Windows;

#endregion

namespace SevenUpdate
{
    /// <summary>
    ///   Interaction logic for App.xaml
    /// </summary>
    public sealed partial class App
    {
        private readonly Guid appGuid = new Guid("{4BC799CE-7658-48D9-AC98-829F64638E41}");

        protected override void OnStartup(StartupEventArgs e)
        {
            Init(e.Args);
            Core.SetJumpList();
            var si = new SingleInstance(appGuid);
            si.ArgsRecieved += si_ArgsRecieved;
            si.Run(() =>
                       {
                           new MainWindow().Show();
                           return MainWindow;
                       }, e.Args);
        }

        private void si_ArgsRecieved(string[] args)
        {
            if (args.Length <= 0)
                return;
            switch (args[0])
            {

                case "-check":
                    Core.NavService.Navigate(new Uri(@"Pages\UpdateHistory.xaml", UriKind.Relative));
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
        ///   Gets the app ready for startup
        /// </summary>
        /// <param name = "args">The command line arguments passed to the app</param>
        internal static void Init(string[] args)
        {
            Base.Locale = Settings.Default.locale;
            foreach (var t in args.Where(t => args[0].EndsWith(".sua", StringComparison.OrdinalIgnoreCase)))
            {
                var suaLoc = t;
                try
                {
                    suaLoc = suaLoc.Replace("sevenupdate://", null);
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