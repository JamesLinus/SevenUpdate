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
using System.IO;
using System.Net;
using System.Resources;
using System.Security.Principal;
using System.Threading;
using System.Windows;
using System.Windows.Media.Imaging;
using SevenUpdate.WCF;
using SevenUpdate.Windows;

#endregion

namespace SevenUpdate
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public sealed class App
    {
        #region Global Vars

        /// <summary>
        /// The green shield image
        /// </summary>
        internal static readonly BitmapImage GreenShield = new BitmapImage(new Uri("/Images/GreenShield.png", UriKind.Relative));

        /// <summary>
        /// The red shield image
        /// </summary>
        internal static readonly BitmapImage RedShield = new BitmapImage(new Uri("/Images/RedShield.png", UriKind.Relative));

        /// <summary>
        /// The UI Resource Strings
        /// </summary>
        internal static readonly ResourceManager RM = new ResourceManager("SevenUpdate.Resources.UIStrings", typeof (App).Assembly);

        /// <summary>
        /// The yellowshield image
        /// </summary>
        internal static readonly BitmapImage YellowShield = new BitmapImage(new Uri("/Images/YellowShield.png", UriKind.Relative));

        #region Properties

        /// <summary>
        /// List of Application Seven Update can check for updates
        /// </summary>
        public static Collection<SUA> AppsToUpdate { get { return Shared.Deserialize<Collection<SUA>>(Shared.AppsFile); } }

        /// <summary>
        /// The update settings for Seven Update
        /// </summary>
        public static Config Settings { get { return Shared.DeserializeStruct<Config>(Shared.ConfigFile); } }

        /// <summary>
        /// Collection of applications to update
        /// </summary>
        internal static Collection<SUI> Applications { get; set; }

        /// <summary>
        /// Indicates if an Auto Search was performed
        /// </summary>
        internal static bool IsAutoCheck { get; private set; }


        /// <summary>
        /// If true, Seven Update is currently installing updates.
        /// </summary>
        internal static bool IsInstallInProgress { get; set; }

        #endregion

        #endregion

        #region Methods

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        /// <param name="args">Command line args</param>
        [STAThread]
        private static void Main(string[] args)
        {
            bool createdNew;
            // Makes sure only 1 copy of Seven Update is allowed to run
            using (new Mutex(true, "Seven Update", out createdNew))
            {
                Shared.Locale = Shared.Locale == null ? "en" : Settings.Locale;
                Shared.SerializationErrorEventHandler += Shared_SerializationErrorEventHandler;
                for (var x = 0; x < args.Length; x++)
                {
                    if (!args[0].EndsWith(".sua", StringComparison.OrdinalIgnoreCase)) continue;
                    AddSUA(args[x]);
                    return;
                }
                if (!createdNew) return;
                var id = WindowsIdentity.GetCurrent();
                if (id != null)
                {
                    var p = new WindowsPrincipal(id);
                    IsAdmin = p.IsInRole(WindowsBuiltInRole.Administrator);
                }
                if (args.Length > 1)
                {
                    if (args[0] == "Auto") IsAutoCheck = true;
                    if (args[0] == "Reconnect")
                    {
                        IsAutoCheck = false;
                        IsInstallInProgress = true;
                    }
                }
                var app = new Application();
                app.Run(new MainWindow());
            }
        }

        private static void Shared_SerializationErrorEventHandler(object sender, Shared.SerializationErrorEventArgs e)
        {
            MessageBox.Show(e.File + e.ErrorMessage);
        }

        /// <summary>
        /// Adds an Application for use with Seven Update
        /// </summary>
        /// <param name="suaLoc">The location of the SUA file</param>
        private static void AddSUA(string suaLoc)
        {
            var wc = new WebClient();
            wc.DownloadFile(suaLoc, Shared.UserStore + "add.sua");
            var sua = Shared.Deserialize<SUA>(Shared.UserStore + "add.sua");
            var sul = Shared.Deserialize<Collection<SUA>>(Shared.AppsFile);
            File.Delete(Shared.UserStore + "add.sua");
            var index = sul.IndexOf(sua);
            if (index < 0)
            {
                if (MessageBox.Show(RM.GetString("AllowUpdates") + " " + sua.ApplicationName[0].Value + "?", RM.GetString("SevenUpdate"), MessageBoxButton.YesNo, MessageBoxImage.Question) ==
                    MessageBoxResult.Yes)
                {
                    sul.Add(sua);

                    Admin.AddSUA(sul);
                }
            }
            wc.Dispose();
        }

        #region Recount Methods

        /// <summary>
        /// Gets the total size of a single update
        /// </summary>
        /// <param name="files">The collection of files of an update</param>
        /// <returns>Returns a ulong value of the size of the update</returns>
        internal static ulong GetUpdateSize(Collection<UpdateFile> files)
        {
            ulong size = 0;
            for (var x = 0; x < files.Count; x++) size += files[x].Size;
            return size;
        }

        #endregion

        #endregion

        #region UI methods

        /// <summary>
        /// Gets a value indicating if the current user is running on admin privledges
        /// </summary>
        /// <returns>True if the current user is an admin, otherwise false</returns>
        internal static bool IsAdmin { get; private set; }

        #endregion
    }
}