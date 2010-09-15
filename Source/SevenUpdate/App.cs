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
using System.Threading;
using System.Windows;
using Microsoft.Windows.Dialogs;
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
                    string appName = Base.GetLocaleString(sua.Name);
                    if (
                        Core.ShowMessage(SevenUpdate.Properties.Resources.Add + " " + appName + " " + SevenUpdate.Properties.Resources.ToSevenUpdate, TaskDialogStandardIcon.ShieldBlue,
                                         TaskDialogStandardButtons.Cancel, SevenUpdate.Properties.Resources.AllowUpdates + appName + "?", null, SevenUpdate.Properties.Resources.Add, true) !=
                        TaskDialogResult.Cancel)
                        AdminClient.AddSua(sua);
                }
                catch
                {
                }
                Environment.Exit(0);
            }

            Directory.CreateDirectory(Base.UserStore);


            if (args.Length > 0)
            {
                if (args[0] == "Auto")
                    Core.IsAutoCheck = true;
                if (args[0] == "Reconnect")
                    Core.IsReconnect = true;
            }

            if (Process.GetProcessesByName("SevenUpdate.Admin").Length <= 0)
                return;
            Core.IsReconnect = true;
            Core.IsAutoCheck = false;
        }
    }

    /// <summary>
    ///   Interaction logic to load the app
    /// </summary>
    internal static class StartUp
    {
        /// <summary>
        ///   Initializes the app resources
        /// </summary>
        private static void InitResources()
        {
            if (Application.Current == null)
                return;

            if (Application.Current.Resources.MergedDictionaries.Count > 0)
                return;
            // merge in your application resources
            Application.Current.Resources.MergedDictionaries.Add(Application.LoadComponent(new Uri("SevenUpdate;component/Resources/Dictionary.xaml", UriKind.Relative)) as ResourceDictionary);
        }

        /// <summary>
        ///   The main entry point for the application.
        /// </summary>
        /// <param name = "args">Command line <c>args</c></param>
        [STAThread]
        private static void Main(string[] args)
        {
            bool createdNew;
            // Makes sure only 1 copy of Seven Update is allowed to run
            using (new Mutex(true, "SevenUpdate", out createdNew))
            {
                App.Init(args);

                if (!createdNew)
                    return;
                var app = new Application();
                InitResources();
                app.Run(new MainWindow());
            }
        }
    }
}