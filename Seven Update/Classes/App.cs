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
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Resources;
using System.Security.Principal;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using SevenUpdate.Windows;
namespace SevenUpdate
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        #region Global Vars

        /// <summary>
        /// The UI Resource Strings
        /// </summary>
        internal static ResourceManager RM = new ResourceManager("SevenUpdate.Resources.UIStrings", typeof(App).Assembly);

        /// <summary>
        /// The red shield image
        /// </summary>
        internal static BitmapImage redShield = new BitmapImage(new Uri("/Images/RedShield.png", UriKind.Relative));

        /// <summary>
        /// The green shield image
        /// </summary>
        internal static BitmapImage greenShield = new BitmapImage(new Uri("/Images/GreenShield.png", UriKind.Relative));

        /// <summary>
        /// The yellowshield image
        /// </summary>
        internal static BitmapImage yellowShield = new BitmapImage(new Uri("/Images/YellowShield.png", UriKind.Relative));

        #region Properties

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
        /// List of Applications
        /// </summary>
        internal static ObservableCollection<Application> Applications { get; set; }

        /// <summary>
        /// Indicates if an Auto Search was performed
        /// </summary>
        internal static bool IsAutoCheck { get; set; }

        /// <summary>
        /// Specifies if Seven Update is allowed to check for updates
        /// </summary>
        internal static bool CanCheckForUpdates { get; set; }

        /// <summary>
        /// If true, Seven Update is currently installing updates.
        /// </summary>
        internal static bool IsInstallInProgress { get; set; }

        /// <summary>
        /// Indicates the installation was already running due to auto check.
        /// </summary>
        internal static bool IsReconnect { get; set; }

        /// <summary>
        /// Indicates if updates have been found
        /// </summary>
        internal static bool IsUpdatesAvailable { get; set; }

        #endregion

        #endregion

        #region Methods

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        /// <param name="args">Command line args</param>
        [STAThread]
        static void Main(string[] args)
        {
            bool createdNew = true;
            // Makes sure only 1 copy of Seven Update is allowed to run
            using (Mutex mutex = new Mutex(true, "Seven Update", out createdNew))
            {
                if (Shared.Locale == null)
                    Shared.Locale = "en";
                else
                    Shared.Locale = Settings.Locale;

                for (int x = 0; x < args.Length; x++)
                {
                    if (args[0].EndsWith(".sua", StringComparison.OrdinalIgnoreCase))
                    {
                        AddSUA(args[x]);
                        return;
                    }
                }
                if (createdNew)
                {
               
                    if (args.Length > 1)
                    {
                        if (args[0] == "Auto")
                        {
                            IsAutoCheck = true;
                        }
                        if (args[0] == "Reconnect")
                        {
                            IsAutoCheck = false;
                            IsInstallInProgress = true;
                            IsReconnect = true;
                        }

                    }
                    System.Windows.Application app = new System.Windows.Application();
                    app.Run(new MainWindow());
                }
            }
        }

        /// <summary>
        /// Adds an Application for use with Seven Update
        /// </summary>
        /// <param name="suaLoc">The location of the SUA file</param>
        static void AddSUA(string suaLoc)
        {
            WebClient wc = new WebClient();
            wc.DownloadFile(suaLoc, Shared.userStore + "add.sua");
            SUA sua = Shared.Deserialize<SUA>(Shared.userStore + "add.sua");
            ObservableCollection<SUA> sul = Shared.DeserializeCollection<SUA>(Shared.appStore + "SUApps.sul");
            File.Delete(Shared.userStore + "add.sua");
            int index = sul.IndexOf(sua);
            if (index < 0)
            {
                if (MessageBox.Show(RM.GetString("AllowUpdates") + " " + sua.ApplicationName[0].Value + "?", RM.GetString("SevenUpdate"), MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    sul.Add(sua);

                    SevenUpdate.WCF.Admin.AddSUA(sul);
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
        internal static ulong GetUpdateSize(ObservableCollection<UpdateFile> files)
        {
            ulong size = 0;
            for (int x = 0; x < files.Count; x++)
            {
                size += files[x].Size;
            }
            return size;
        }

        #endregion

        #region History Methods

        /// <summary>
        /// Gets Hidden Updates
        /// </summary>
        /// <returns>Returns the list of hidden updates</returns>
        internal static ObservableCollection<UpdateInformation> GetHiddenUpdates()
        {
            return Shared.DeserializeCollection<UpdateInformation>(Shared.appStore + "Hidden Updates.xml");
        }

        /// <summary>
        /// Gets the update history
        /// </summary>
        /// <returns>Returns a list of updates installed</returns>
        internal static ObservableCollection<UpdateInformation> GetHistory()
        {
            return Shared.DeserializeCollection<UpdateInformation>(Shared.appStore + "Update History.xml");
        }

        #endregion

        #endregion

        #region UI methods

        /// <summary>
        /// Specifies if the current user running Seven Update is an administrator
        /// </summary>
        /// <returns></returns>
        static internal bool IsAdmin
        {
            get
            {
                WindowsIdentity id = WindowsIdentity.GetCurrent();
                WindowsPrincipal p = new WindowsPrincipal(id);
                return p.IsInRole(WindowsBuiltInRole.Administrator);
            }
        }

        /// <summary>
        /// Adds a shield to a button
        /// </summary>
        /// <param name="b">The button object you want to add a shield to</param>
        static internal void AddShieldToButton(Button b)
        {
            if (Environment.OSVersion.Version.Major >= 6)
            {
                try
                {

                }
                catch (Exception) { }
            }
        }

        #endregion
    }
}
