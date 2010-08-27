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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Resources;
using System.Threading;
using System.Windows;

using SevenUpdate.Windows;

#endregion

namespace SevenUpdate
{
    /// <summary>
    ///   Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        #region Global Vars

        internal new static ResourceDictionary Resources;

        #region Properties

        /// <summary>
        ///   Gets or Sets a collection of software that Seven Update can check for updates
        /// </summary>
        public static IEnumerable<Sua> AppsToUpdate { get { return Base.Deserialize<Collection<Sua>>(Base.AppsFile); } }

        /// <summary>
        ///   Gets the update configuration settings
        /// </summary>
        public static Config Settings
        {
            get
            {
                var t = Base.Deserialize<Config>(Base.ConfigFile);
                return t ?? new Config {AutoOption = AutoUpdateOption.Notify, IncludeRecommended = false};
            }
        }

        /// <summary>
        ///   Gets or Sets a collection of applications to update
        /// </summary>
        internal static Collection<Sui> Applications { get; set; }

        ///// <summary>
        /////   Gets a value indicating if the current user is running on admin privileges
        ///// </summary>
        ///// <returns><c>true</c> if the current user is an admin, otherwise <c>false</c></returns>
        //internal static bool IsAdmin { get; private set; }

        /// <summary>
        ///   Gets a value indicating if an auto check is being performed
        /// </summary>
        internal static bool IsAutoCheck { get; private set; }

        /// <summary>
        ///   Gets or Sets a value indicating if an install is currently in progress
        /// </summary>
        internal static bool IsInstallInProgress { get; set; }

        /// <summary>
        ///   Gets or Sets a value indicating if an install is currently in progress and Seven Update was started after an autocheck
        /// </summary>
        internal static bool IsReconnect { get; set; }

        #endregion

        /// <summary>
        ///   Gets the resources for the application
        /// </summary>
        internal static ResourceManager RM { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        ///   Gets the app ready for startup
        /// </summary>
        /// <param name = "args">The command line arguments passed to the app</param>
        internal static void Init(string[] args)
        {
            foreach (string t in args.Where(t => args[0].EndsWith(".sua", StringComparison.OrdinalIgnoreCase)))
            {
                string suaLoc = t;
                try
                {
                    suaLoc = suaLoc.Replace("sevenupdate://", null);
                    var sua = Base.Deserialize<Sua>(Base.DownloadFile(suaLoc), suaLoc);
                    if (
                        MessageBox.Show(RM.GetString("AllowUpdates") + " " + Base.GetLocaleString(sua.Name) + "?", RM.GetString("SevenUpdate"), MessageBoxButton.YesNo,
                                        MessageBoxImage.Question) == MessageBoxResult.Yes)
                        AdminClient.AddSua(sua);
                }
                catch
                {
                }
                Environment.Exit(0);
            }

            Directory.CreateDirectory(Base.UserStore);
            Base.Locale = SevenUpdate.Properties.Settings.Default.locale;
            RM = new ResourceManager("SevenUpdate.Resources.UIStrings", ResourceAssembly);

            if (args.Length > 0)
            {
                if (args[0] == "Auto")
                    IsAutoCheck = true;
                if (args[0] == "Reconnect")
                    IsReconnect = true;
            }

            if (Process.GetProcessesByName("SevenUpdate.Admin").Length > 0)
            {
                IsReconnect = true;
                IsAutoCheck = false;
            }
        }

        #region Recount Methods

        /// <summary>
        ///   Gets the total size of a single update
        /// </summary>
        /// <param name = "files">the collection of files of an update</param>
        /// <returns>a ulong value of the size of the update</returns>
        internal static ulong GetUpdateSize(IEnumerable<UpdateFile> files)
        {
            return files.Aggregate<UpdateFile, ulong>(0, (current, t) => current + t.FileSize);
        }

        #endregion

        #endregion
    }

    /// <summary>
    ///   Interaction logic to load the app
    /// </summary>
    public static class StartUp
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
            Application.Current.Resources.MergedDictionaries.Add(
                Application.LoadComponent(new Uri("SevenUpdate;component/Resources/Dictionary.xaml", UriKind.Relative)) as ResourceDictionary);
            App.Resources = Application.Current.Resources;
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