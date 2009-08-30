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
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using SevenUpdate.Properties;
using SevenUpdate.WCF;
using SevenUpdate.Windows;

namespace SevenUpdate.Pages
{
    /// <summary>
    /// Interaction logic for Main.xaml
    /// </summary>
    public partial class Main : Page
    {
        #region Properties

        //internal static string LastPageVisited { get; set; }

        #endregion

        #region Global Vars

        BitmapImage yellowSide = new BitmapImage(new Uri("/Images/YellowSide.png", UriKind.Relative));
        BitmapImage greenSide = new BitmapImage(new Uri("/Images/GreenSide.png", UriKind.Relative));
        BitmapImage redSide = new BitmapImage(new Uri("/Images/RedSide.png", UriKind.Relative));
        BitmapImage suIcon = new BitmapImage(new Uri("/Images/Icon.png", UriKind.Relative));

        ulong totalUpdateSize;

        #endregion

        #region Enums

        /// <summary>
        /// The layout for the Info Panel
        /// </summary>
        enum UILayout
        {

            /// <summary>
            /// Canceled Updates
            /// </summary>
            Canceled,

            /// <summary>
            /// Checking for updates
            /// </summary>
            CheckingForUpdates,

            /// <summary>
            /// When downloading of updates has been completed
            /// </summary>
            DownloadCompleted,

            /// <summary>
            /// Downloading updates
            /// </summary>
            Downloading,

            /// <summary>
            /// Downloading of updates has been suspended
            /// </summary>
            DownloadSuspended,

            /// <summary>
            /// An Error Occurred when downloading/installing updates
            /// </summary>
            ErrorOccurred,

            /// <summary>
            /// When installation of updates have completed
            /// </summary>
            InstallationCompleted,

            /// <summary>
            /// Installing Updates
            /// </summary>
            Installing,

            /// <summary>
            /// No updates have been found
            /// </summary>
            NoUpdates,

            /// <summary>
            /// A reboot is needed to finish installing updates
            /// </summary>
            RebootNeeded,

            /// <summary>
            /// No updates have been found
            /// </summary>
            UpdatesFound,

        }

        #endregion

        public Main()
        {
            InitializeComponent();
            LoadSettings();
            if (App.IsAdmin)
                infoBar.btnAction.Content = App.RM.GetString("InstallUpdates");
            if (App.AutoCheck)
            {
                CheckForUpdates(true);
            }
            else
                if (!Settings.Default.lastUpdateCheck.Contains(DateTime.Now.ToShortDateString()))
                    CheckForUpdates();
        }

        #region UI Events

        #region TextBlock

        private void TextBlock_MouseEnter(object sender, MouseEventArgs e)
        {
            TextBlock textBlock = ((TextBlock)sender);
            textBlock.TextDecorations = TextDecorations.Underline;

        }

        private void TextBlock_MouseLeave(object sender, MouseEventArgs e)
        {
            TextBlock textBlock = ((TextBlock)sender);
            textBlock.TextDecorations = null;
        }

        private void tbChangeSettings_MouseDown(object sender, MouseButtonEventArgs e)
        {
            SevenUpdate.Windows.MainWindow.ns.Navigate(new Uri(@"Pages\Options.xaml", UriKind.Relative));

        }

        private void tbCheckForUpdates_MouseDown(object sender, MouseButtonEventArgs e)
        {
            CheckForUpdates();
            // SevenUpdate.Windows.MainWindow.ns.Navigate(new Uri(@"Pages\Update Info.xaml", UriKind.Relative));
        }

        private void tbViewUpdateHistory_MouseDown(object sender, MouseButtonEventArgs e)
        {
            SevenUpdate.Windows.MainWindow.ns.Navigate(new Uri(@"Pages\Update History.xaml", UriKind.Relative));
        }

        private void tbRestoreHiddenUpdates_MouseDown(object sender, MouseButtonEventArgs e)
        {
            SevenUpdate.Windows.MainWindow.ns.Navigate(new Uri(@"Pages\Restore Updates.xaml", UriKind.Relative));
        }

        private void tbAbout_MouseDown(object sender, MouseButtonEventArgs e)
        {
            SevenUpdate.Windows.About about = new SevenUpdate.Windows.About();
            about.ShowDialog();

            SevenUpdate.Windows.LicenseAgreement la = new SevenUpdate.Windows.LicenseAgreement();
            la.LoadLicenses();

            SevenUpdate.Windows.UpdateDetails ud = new SevenUpdate.Windows.UpdateDetails();
            ud.ShowDialog();
        }

        private void tbViewOptionalUpdates_MouseDown(object sender, MouseButtonEventArgs e)
        {
            SevenUpdate.Windows.MainWindow.ns.Navigate(new Uri(@"Pages\Update Info.xaml", UriKind.Relative));
        }

        private void tbViewImportantUpdates_MouseDown(object sender, MouseButtonEventArgs e)
        {
            SevenUpdate.Windows.MainWindow.ns.Navigate(new Uri(@"Pages\Update Info.xaml", UriKind.Relative));
        }

        #endregion

        /// <summary>
        /// Updates the UI after the user selects updates to install
        /// </summary>
        private void UpdateInfo_UpdateSelectionChangedEventHandler(object sender, UpdateInfo.UpdateSelectionChangedEventArgs e)
        {
            #region GUI Updating

            if (e.ImportantUpdates > 0)
            {

                if (e.ImportantUpdates == 1)
                    infoBar.tbSelectedUpdates.Text = e.ImportantUpdates + " " + App.RM.GetString("ImportantUpdateSelected");
                else
                    infoBar.tbSelectedUpdates.Text = e.ImportantUpdates + " " + App.RM.GetString("ImportantUpdatesSelected");

                if (e.ImportantDownloadSize > 0)
                    infoBar.tbSelectedUpdates.Text += ", " + Shared.ConvertFileSize(e.ImportantDownloadSize);
            }
            if (e.OptionalUpdates > 0)
            {
                if (e.ImportantUpdates == 0)
                    if (e.OptionalUpdates == 1)
                        infoBar.tbSelectedUpdates.Text = e.OptionalUpdates + " " + App.RM.GetString("OptionalUpdateSelected");
                    else
                        infoBar.tbSelectedUpdates.Text = e.OptionalUpdates + " " + App.RM.GetString("OptionalUpdatesSelected");
                else
                    if (e.OptionalUpdates == 1)
                        infoBar.tbSelectedUpdates.Text += Environment.NewLine + e.OptionalUpdates + " " + App.RM.GetString("OptionalUpdateSelected");
                    else
                        infoBar.tbSelectedUpdates.Text += Environment.NewLine + e.OptionalUpdates + " " + App.RM.GetString("OptionalUpdatesSelected");

                if (e.OptionalDownloadSize > 0)
                    infoBar.tbSelectedUpdates.Text += ", " + Shared.ConvertFileSize(e.OptionalDownloadSize);
            }
            else
                infoBar.tbViewOptionalUpdates.Visibility = Visibility.Collapsed;

            if (e.ImportantDownloadSize == 0 && e.OptionalDownloadSize == 0)
            {
                infoBar.tbHeading.Text = App.RM.GetString("InstallUpdatesForPrograms");

                App.NotifyIcon.Text = App.RM.GetString("InstallUpdatesForPrograms");
            }
            else
            {
                infoBar.tbHeading.Text = App.RM.GetString("DownloadAndInstallUpdates");

                App.NotifyIcon.Text = App.RM.GetString("DownloadAndInstallUpdates");
            }

            if (e.ImportantUpdates > 0 || e.OptionalUpdates > 0)
            {
                infoBar.btnAction.Visibility = Visibility.Visible;
                infoBar.tbSelectedUpdates.FontWeight = FontWeights.Bold;
            }
            else
            {
                infoBar.tbSelectedUpdates.Text = App.RM.GetString("NoUpdatesSelected");
                infoBar.tbSelectedUpdates.FontWeight = FontWeights.Normal;
                infoBar.btnAction.Visibility = Visibility.Collapsed;
            }
            #endregion
        }

        /// <summary>
        /// Checks for updates after hidden updates have been restored
        /// </summary>
        private void RestoreUpdates_RestoredHiddenUpdateEventHandler(object sender, EventArgs e)
        {
            CheckForUpdates(true);
        }

        /// <summary>
        /// Executes action based on current label. Installed, cancels, and/or searches for updates. it also can reboot the computer.
        /// </summary>
        private void btnAction_Click(object sender, RoutedEventArgs e)
        {
            if (infoBar.btnAction.Content.ToString() == App.RM.GetString("InstallUpdates"))
            {
                DownloadInstallUpdates();
            }
            else if (infoBar.btnAction.Content.ToString() == App.RM.GetString("CancelUpdates"))
            {
                //Cancel installation of updates
                Admin.AbortInstall();
                SetUI(UILayout.Canceled);
                return;
            }
            else if (infoBar.btnAction.Content.ToString() == App.RM.GetString("TryAgain"))
            { CheckForUpdates(); }
            else if (infoBar.btnAction.Content.ToString() == App.RM.GetString("RestartNow"))
            {
                Process.Start(Environment.GetFolderPath(Environment.SpecialFolder.System) + "\\shutdown.exe", "-r -t 00");

            }
        }

        #endregion

        #region Update Event Methods

        void InstallProgressChanged(Admin.InstallProgressChangedEventArgs e)
        {
            if (e.CurrentProgress == -1)
                infoBar.tbStatus.Text = App.RM.GetString("PreparingInstall") + "...";
            else
            {
                infoBar.tbStatus.Text = App.RM.GetString("Installing") + " " + e.UpdateTitle;

                if (e.TotalUpdates > 1)
                    infoBar.tbStatus.Text += Environment.NewLine + e.UpdatesComplete + " " + App.RM.GetString("OutOf") + " " + e.TotalUpdates + ", " + e.CurrentProgress + "% " + App.RM.GetString("Complete");
                else
                    infoBar.tbStatus.Text += ", " + e.CurrentProgress + "% " + App.RM.GetString("Complete");

            }
        }

        void DownloadProgressChanged(Admin.DownloadProgressChangedEventArgs e)
        {
            try
            {
                infoBar.tbStatus.Text = App.RM.GetString("DownloadingUpdates") + " (" +
                    Shared.ConvertFileSize(e.BytesTotal) + ", " + (e.BytesTransferred * 100 / e.BytesTotal).ToString("F0") + " % " + App.RM.GetString("Complete") + ")";
                App.NotifyIcon.Text = infoBar.tbStatus.Text;

            }
            catch (NullReferenceException)
            {
            }
        }

        void InstallDone(Admin.InstallDoneEventArgs e)
        {
            if (!e.ErrorOccurred)
            {
                Settings.Default.lastInstall = DateTime.Now.ToShortDateString() + " " + App.RM.GetString("At") + " " + DateTime.Now.ToShortTimeString();
                tbUpdatesInstalled.Text = App.RM.GetString("TodayAt") + " " + DateTime.Now.ToShortTimeString();
                // if a reboot is needed lets say it
                if (Shared.RebootNeeded)
                {
                    App.CanCheckForUpdates = false;
                    if (!this.Dispatcher.CheckAccess())
                    {
                        SetUI(UILayout.RebootNeeded);
                    }
                }
                else
                {
                    App.CanCheckForUpdates = true;
                    SetUI(UILayout.InstallationCompleted);
                }
            }
            else
                SetUI(UILayout.ErrorOccurred);
        }

        void DownloadDone(Admin.DownloadDoneEventArgs e)
        {
            if (e.ErrorOccurred)
            {
                SetUI(UILayout.ErrorOccurred);
            }
            else
            {
                SetUI(UILayout.DownloadCompleted);
                if (infoBar.tbSelectedUpdates.Text == App.RM.GetString("NoUpdatesSelected"))
                    infoBar.btnAction.Visibility = Visibility.Collapsed;
            }
        }

        void SearchDone(Search.SearchDoneEventArgs e)
        {
            if (e.Applications.Count > 0)
            {
                App.Applications = e.Applications;

                int[] count = new int[2];

                for (int x = 0; x < e.Applications.Count; x++)
                {
                    for (int y = 0; y < e.Applications[x].Updates.Count; y++)
                    {
                        totalUpdateSize = e.Applications[x].Updates[y].Size;
                        switch (e.Applications[x].Updates[y].Importance)
                        {
                            case Importance.Important:
                                count[0]++;
                                break;
                            case Importance.Locale:
                            case Importance.Optional:

                                count[1]++;
                                break;
                            case Importance.Recommended:
                                if (App.Settings.IncludeRecommended)
                                    count[0]++;
                                else
                                    count[1]++;
                                break;

                        }
                    }
                }
                #region GUI Updating

                if (count[0] > 0 || count[1] > 0)
                {
                    SetUI(UILayout.UpdatesFound);
                    infoBar.tbSelectedUpdates.Text = App.RM.GetString("NoUpdatesSelected");

                    if (count[0] == 1)
                        infoBar.tbViewImportantUpdates.Text = count[0] + " " + App.RM.GetString("ImportantUpdateAvaliable") + " ";
                    else
                        infoBar.tbViewImportantUpdates.Text = count[0] + " " + App.RM.GetString("ImportantUpdatesAvaliable");

                    if (count[0] < 1)
                    {
                        infoBar.tbViewImportantUpdates.Text = App.RM.GetString("NoImportantUpdates");
                    }

                    if (count[1] > 0)
                    {

                        if (count[1] == 1)
                            infoBar.tbViewOptionalUpdates.Text = count[1] + " " + App.RM.GetString("OptionalUpdateAvaliable");
                        else
                            infoBar.tbViewOptionalUpdates.Text = count[1] + " " + App.RM.GetString("OptionalUpdatesAvaliable");

                        infoBar.tbViewOptionalUpdates.Visibility = Visibility.Visible;


                    }
                    else
                    {
                        infoBar.tbViewOptionalUpdates.Visibility = Visibility.Collapsed;
                    }
                }
                //End Code
                #endregion
            }
            else
            {
                if (!this.Dispatcher.CheckAccess())
                {
                    DispatcherObjectDelegates.BeginInvoke<UILayout>(this.Dispatcher, SetUI, UILayout.NoUpdates);
                }
                else
                    SetUI(UILayout.NoUpdates);
            }
        }

        #region Invoker Events

        void Admin_InstallProgressChangedEventHandler(object sender, Admin.InstallProgressChangedEventArgs e)
        {
            if (!this.Dispatcher.CheckAccess())
            {
                DispatcherObjectDelegates.BeginInvoke<Admin.InstallProgressChangedEventArgs>(this.Dispatcher, InstallProgressChanged, e);
            }
            else
                InstallProgressChanged(e);
        }

        void Admin_DownloadProgressChangedEventHandler(object sender, Admin.DownloadProgressChangedEventArgs e)
        {
            if (!this.Dispatcher.CheckAccess())
            {
                DispatcherObjectDelegates.BeginInvoke<Admin.DownloadProgressChangedEventArgs>(this.Dispatcher, DownloadProgressChanged, e);
            }
            else
                DownloadProgressChanged(e);
        }

        internal void Search_SearchDoneEventHandler(object sender, Search.SearchDoneEventArgs e)
        {
            if (!this.Dispatcher.CheckAccess())
            {
                DispatcherObjectDelegates.BeginInvoke<Search.SearchDoneEventArgs>(this.Dispatcher, SearchDone, e);
            }
            else
                SearchDone(e);
        }

        void Admin_InstallDoneEventHandler(object sender, Admin.InstallDoneEventArgs e)
        {
            if (!this.Dispatcher.CheckAccess())
            {
                DispatcherObjectDelegates.BeginInvoke<Admin.InstallDoneEventArgs>(this.Dispatcher, InstallDone, e);
            }
            else
                InstallDone(e);
        }

        void Admin_DownloadDoneEventHandler(object sender, Admin.DownloadDoneEventArgs e)
        {
            if (!this.Dispatcher.CheckAccess())
            {
                DispatcherObjectDelegates.BeginInvoke<Admin.DownloadDoneEventArgs>(this.Dispatcher, DownloadDone, e);
            }
            else
                DownloadDone(e);
        }

        #endregion

        void Admin_ErrorOccurredEventHandler(object sender, Admin.ErrorOccurredEventArgs e)
        {
            if (e.ErrorDescription == "Network Connection Error")
            {
                if (!this.Dispatcher.CheckAccess())
                {
                    DispatcherObjectDelegates.BeginInvoke<UILayout, string>(this.Dispatcher, SetUI, UILayout.ErrorOccurred, App.RM.GetString("CheckConnection"));
                }
                else
                    SetUI(UILayout.ErrorOccurred, App.RM.GetString("CheckConnection"));
            }
            else if (e.ErrorDescription == "Seven Update Server Error")
            {
                if (!this.Dispatcher.CheckAccess())
                {
                    DispatcherObjectDelegates.BeginInvoke<UILayout, string>(this.Dispatcher, SetUI, UILayout.ErrorOccurred, App.RM.GetString("CouldNotConnect"));
                }
                else
                    SetUI(UILayout.ErrorOccurred, App.RM.GetString("CouldNotConnect"));
            }
            else
            {
                if (!this.Dispatcher.CheckAccess())
                {
                    DispatcherObjectDelegates.BeginInvoke<UILayout, string>(this.Dispatcher, SetUI, UILayout.ErrorOccurred, e.ErrorDescription);
                }
                else
                    SetUI(UILayout.ErrorOccurred,e.ErrorDescription);
            }
            try
            {
                Process[] process = Process.GetProcessesByName("Seven Update.Admin");
                for (int x = 0; x < process.Length; x++)
                {
                    process[x].Kill();
                }
            }
            catch (Exception) { }

            Admin.AbortInstall();
            Shared.ReportError(e.ErrorDescription, Shared.userStore);

        }

        #endregion

        #region Update Methods

        /// <summary>
        /// Checks for updates
        /// </summary>
        /// <param name="auto">Indicates if it's an auto function, if it is it disables the warning messages</param>
        void CheckForUpdates(bool auto)
        {
            if (auto)
            {
                if (Process.GetProcessesByName("Seven Update.Admin").Length < 1 && App.CanCheckForUpdates && Shared.RebootNeeded == false)
                {
                    CheckForUpdates();
                }
            }
            else
                CheckForUpdates();
        }

        /// <summary>
        /// Checks for updates
        /// </summary>
        void CheckForUpdates()
        {
            if (Process.GetProcessesByName("Seven Update.Admin").Length < 1)
            {
                if (App.CanCheckForUpdates)
                {
                    if (Shared.RebootNeeded == false)
                    {
                        SetUI(UILayout.CheckingForUpdates);
                        Settings.Default.lastUpdateCheck = DateTime.Now.ToShortDateString() + " " + App.RM.GetString("At") + " " + DateTime.Now.ToShortTimeString();
                        Search.SearchForUpdatesAync(App.AppsToUpdate);
                    }
                    else
                    {
                        SetUI(UILayout.RebootNeeded);
                        MessageBox.Show(App.RM.GetString("RebootNeededFirst"), App.RM.GetString("SevenUpdate"), MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                else
                    MessageBox.Show(App.RM.GetString("AlreadyUpdating"), App.RM.GetString("SevenUpdate"), MessageBoxButton.OK, MessageBoxImage.Information);

            }
            else
                MessageBox.Show(App.RM.GetString("AlreadyUpdating"), App.RM.GetString("SevenUpdate"), MessageBoxButton.OK, MessageBoxImage.Information);

        }

        /// <summary>
        /// Downloads updates
        /// </summary>
        /// <param name="auto">Specifies if Seven Update is doing automatic uodates</param>
        /// <param name="resume">Specifies if resuming downloads</param>
        internal void DownloadInstallUpdates()
        {
            for (int x = 0; x < App.Applications.Count; x++)
            {
                for (int y = 0; y < App.Applications[x].Updates.Count; y++)
                {
                    if (App.Applications[x].Updates[y].Selected == false)
                    {
                        App.Applications[x].Updates.RemoveAt(y);
                        y--;
                    }
                }
                if (App.Applications[x].Updates.Count == 0)
                {
                    App.Applications.RemoveAt(x);
                    x--;
                }

            }

            if (App.Applications.Count > 0)
            {
                LicenseAgreement sla = new LicenseAgreement();
                if (sla.LoadLicenses() == false)
                {
                    SetUI(UILayout.Canceled);
                    return;
                }

                if (Admin.Install())
                {
                    Shared.SerializeCollection<Application>(App.Applications, Shared.userStore + "Update List.xml");
                    Admin.Connect();
                    if (infoBar.tbHeading.Text == App.RM.GetString("DownloadAndInstallUpdates"))
                    {
                        SetUI(UILayout.Downloading);
                    }
                    else
                    {
                        SetUI(UILayout.Installing);
                    }
                }
                else
                {
                    SetUI(UILayout.Canceled);
                }
            }
            else
            {
                SetUI(UILayout.Canceled);
            }
        }

        #endregion

        #region UI Methods

        /// <summary>
        /// Loads the application settings and initializes event handlers for the pages
        /// </summary>
        void LoadSettings()
        {
            if (Settings.Default.lastUpdateCheck.Contains(DateTime.Now.ToShortDateString()))
                tbRecentCheck.Text = App.RM.GetString("TodayAt") + " " + Settings.Default.lastUpdateCheck.Replace(DateTime.Now.ToShortDateString() + " " + App.RM.GetString("At") + " ", null);
            else
                tbRecentCheck.Text = Settings.Default.lastUpdateCheck;

            if (Settings.Default.lastInstall.Contains(DateTime.Now.ToShortDateString()))
                tbUpdatesInstalled.Text = App.RM.GetString("TodayAt") + " " + Settings.Default.lastInstall.Replace(DateTime.Now.ToShortDateString() + " " + App.RM.GetString("At") + " ", null);
            else
                tbUpdatesInstalled.Text = Settings.Default.lastInstall;

            if (Shared.RebootNeeded)
            {
                SetUI(UILayout.RebootNeeded);
            }
            else
            {
                SetUI(UILayout.NoUpdates);
            }

            #region Event Handler Declarations
            UpdateInfo.UpdateSelectionChangedEventHandler += new EventHandler<UpdateInfo.UpdateSelectionChangedEventArgs>(UpdateInfo_UpdateSelectionChangedEventHandler);
            infoBar.btnAction.Click += new RoutedEventHandler(btnAction_Click);
            infoBar.tbViewImportantUpdates.MouseDown += new MouseButtonEventHandler(tbViewImportantUpdates_MouseDown);
            infoBar.tbViewOptionalUpdates.MouseDown += new MouseButtonEventHandler(tbViewOptionalUpdates_MouseDown);
            Search.SearchDoneEventHandler += new EventHandler<Search.SearchDoneEventArgs>(Search_SearchDoneEventHandler);
            Admin.DownloadProgressChangedEventHandler += new EventHandler<Admin.DownloadProgressChangedEventArgs>(Admin_DownloadProgressChangedEventHandler);
            Admin.DownloadDoneEventHandler += new EventHandler<Admin.DownloadDoneEventArgs>(Admin_DownloadDoneEventHandler);
            Admin.InstallProgressChangedEventHandler += new EventHandler<Admin.InstallProgressChangedEventArgs>(Admin_InstallProgressChangedEventHandler);
            Admin.InstallDoneEventHandler += new EventHandler<Admin.InstallDoneEventArgs>(Admin_InstallDoneEventHandler);
            Admin.ErrorOccurredEventHandler += new EventHandler<Admin.ErrorOccurredEventArgs>(Admin_ErrorOccurredEventHandler);
            RestoreUpdates.RestoredHiddenUpdateEventHandler += new EventHandler<EventArgs>(RestoreUpdates_RestoredHiddenUpdateEventHandler);

            #endregion
        }

        /// <summary>
        /// Sets the Main Page UI
        /// </summary>
        /// <param name="layout">Type of layout to set</param>
        void SetUI(UILayout layout)
        {
            SetUI(layout, null);
        }

        /// <summary>
        /// Sets the Main Page UI
        /// </summary>
        /// <param name="layout">Type of layout to set</param>
        /// <param name="errorDescription">Description of error that occurred</param>
        void SetUI(UILayout layout, string errorDescription)
        {
            switch (layout)
            {
                case UILayout.UpdatesFound:
                    #region GUI Code
                    infoBar.tbStatus.Visibility = Visibility.Collapsed;
                    infoBar.pbProgressBar.Visibility = Visibility.Collapsed;
                    infoBar.tbHeading.Text = App.RM.GetString("DownloadAndInstallUpdates");
                    infoBar.imgSide.Source = yellowSide;
                    infoBar.imgShield.Source = App.yellowShield;
                    infoBar.btnAction.Content = App.RM.GetString("InstallUpdates");
                    infoBar.spUpdateInfo.Visibility = Visibility.Visible;
                    infoBar.tbSelectedUpdates.Visibility = Visibility.Visible;
                    infoBar.btnAction.Visibility = Visibility.Collapsed;
                    infoBar.tbViewOptionalUpdates.Visibility = Visibility.Visible;
                    infoBar.tbViewImportantUpdates.Visibility = Visibility.Visible;
                    infoBar.line.Visibility = Visibility.Visible;
                    App.NotifyIcon.Text = App.RM.GetString("DownloadAndInstallUpdates");

                    #endregion

                    #region Code

                    App.UpdatesFound = true;
                    App.InstallInProgress = false;
                    App.CanCheckForUpdates = true;

                    #endregion
                    break;
                case UILayout.NoUpdates:
                    #region GUI Code
                    infoBar.tbStatus.Visibility = Visibility.Visible;
                    infoBar.tbSelectedUpdates.Visibility = Visibility.Collapsed;
                    infoBar.pbProgressBar.Visibility = Visibility.Collapsed;
                    infoBar.tbHeading.Text = App.RM.GetString("ProgramsUpToDate");
                    infoBar.imgSide.Source = greenSide;
                    infoBar.imgShield.Source = App.greenShield;
                    infoBar.pbProgressBar.Visibility = Visibility.Collapsed;
                    infoBar.tbStatus.Text = App.RM.GetString("NoNewUpdates");
                    infoBar.spUpdateInfo.Visibility = Visibility.Collapsed;
                    infoBar.btnAction.Visibility = Visibility.Collapsed;
                    infoBar.line.Visibility = Visibility.Collapsed;

                    App.NotifyIcon.Text = App.RM.GetString("NoNewUpdates");

                    #endregion

                    #region Code

                    App.UpdatesFound = false;
                    App.InstallInProgress = false;
                    App.CanCheckForUpdates = true;

                    #endregion

                    break;
                case UILayout.CheckingForUpdates:
                    #region GUI Code

                    infoBar.tbSelectedUpdates.FontWeight = FontWeights.Normal;
                    infoBar.tbViewImportantUpdates.Visibility = Visibility.Collapsed;
                    infoBar.tbViewOptionalUpdates.Visibility = Visibility.Collapsed;
                    infoBar.tbSelectedUpdates.Visibility = Visibility.Collapsed;
                    infoBar.tbStatus.Visibility = Visibility.Collapsed;
                    infoBar.tbHeading.Text = App.RM.GetString("CheckingForUpdates") + "...";
                    infoBar.imgShield.Source = suIcon;
                    infoBar.imgSide.Source = null;
                    infoBar.pbProgressBar.Visibility = Visibility.Visible;

                    App.NotifyIcon.Text = App.RM.GetString("CheckingForUpdates") + "...";

                    #endregion

                    #region Code
                    App.InstallInProgress = false;
                    App.NotifyIcon.Text = App.RM.GetString("DownloadAndInstallUpdates");
                    if (SevenUpdate.Windows.MainWindow.IsHidden)
                        App.NotifyIcon.ShowBalloonTip(5000, App.RM.GetString("DownloadAndInstallUpdates"), App.RM.GetString("UpdatesAvailable"), Avalon.Windows.Controls.NotifyBalloonIcon.Info);
                    App.CanCheckForUpdates = false;

                    tbRecentCheck.Text = App.RM.GetString("TodayAt") + " " + DateTime.Now.ToShortTimeString();

                    #endregion
                    break;
                case UILayout.Downloading:

                    #region GUI Code


                    infoBar.imgShield.Source = App.yellowShield;
                    infoBar.imgSide.Source = yellowSide;
                    infoBar.tbSelectedUpdates.Visibility = Visibility.Collapsed;
                    infoBar.spUpdateInfo.Visibility = Visibility.Collapsed;
                    infoBar.tbStatus.Visibility = Visibility.Visible;
                    infoBar.pbProgressBar.Visibility = Visibility.Visible;
                    infoBar.btnAction.Visibility = Visibility.Visible;
                    infoBar.btnAction.Content = App.RM.GetString("CancelUpdates");
                    infoBar.tbStatus.Text = App.RM.GetString("PreparingDownload");
                    infoBar.tbHeading.Text = App.RM.GetString("DownloadingUpdates") + "...";
                    infoBar.line.Visibility = Visibility.Collapsed;

                    #endregion

                    #region Code
                    App.InstallInProgress = false;
                    App.CanCheckForUpdates = false;
                    App.NotifyIcon.Text = App.RM.GetString("DownloadingUpdatesBackground");

                    if (SevenUpdate.Windows.MainWindow.IsHidden)
                        App.NotifyIcon.ShowBalloonTip(5000, App.RM.GetString("DownloadingUpdates"), App.RM.GetString("DownloadingUpdatesBackground"), Avalon.Windows.Controls.NotifyBalloonIcon.Info);
                    #endregion

                    break;
                case UILayout.DownloadCompleted:

                    #region GUI Code
                    infoBar.tbStatus.Visibility = Visibility.Collapsed;
                    infoBar.tbSelectedUpdates.Visibility = Visibility.Visible;
                    infoBar.tbStatus.Visibility = Visibility.Collapsed;
                    infoBar.pbProgressBar.Visibility = Visibility.Collapsed;
                    infoBar.btnAction.Visibility = Visibility.Visible;
                    infoBar.btnAction.Content = App.RM.GetString("InstallUpdates");
                    infoBar.tbHeading.Text = App.RM.GetString("UpdatesReadyInstalled");
                    infoBar.imgShield.Source = App.yellowShield;
                    infoBar.imgSide.Source = yellowSide;
                    infoBar.line.Visibility = Visibility.Visible;



                    #endregion

                    #region Code
                    App.InstallInProgress = false;
                    App.CanCheckForUpdates = true;
                    App.NotifyIcon.Text = App.RM.GetString("UpdatesDownloaded");

                    if (SevenUpdate.Windows.MainWindow.IsHidden)
                        App.NotifyIcon.ShowBalloonTip(5000, App.RM.GetString("UpdatesDownloaded"), App.RM.GetString("FinishedDownloading"), Avalon.Windows.Controls.NotifyBalloonIcon.Info);
                    #endregion
                    break;

                case UILayout.Installing:

                    #region GUI Code
                    infoBar.btnAction.Visibility = Visibility.Visible;
                    infoBar.pbProgressBar.Visibility = Visibility.Visible;
                    infoBar.tbStatus.Visibility = Visibility.Visible;
                    infoBar.tbSelectedUpdates.Visibility = Visibility.Collapsed;
                    infoBar.spUpdateInfo.Visibility = Visibility.Collapsed;
                    infoBar.imgShield.Source = suIcon;
                    infoBar.btnAction.Content = App.RM.GetString("CancelUpdates");
                    infoBar.tbStatus.Text = App.RM.GetString("PreparingInstall");
                    infoBar.tbHeading.Text = App.RM.GetString("InstallingUpdates") + "...";

                    infoBar.line.Visibility = Visibility.Collapsed;

                    #endregion

                    #region Code
                    App.InstallInProgress = true;
                    App.CanCheckForUpdates = false;
                    App.NotifyIcon.Text = App.RM.GetString("PreparingInstall");

                    if (SevenUpdate.Windows.MainWindow.IsHidden)
                        App.NotifyIcon.ShowBalloonTip(5000, App.RM.GetString("InstallingUpdates"), App.RM.GetString("InstallingUpdatesBackground"), Avalon.Windows.Controls.NotifyBalloonIcon.Info);
                    #endregion

                    break;

                case UILayout.InstallationCompleted:

                    #region GUI Code

                    infoBar.tbStatus.Visibility = Visibility.Visible;
                    infoBar.tbSelectedUpdates.Visibility = Visibility.Collapsed;
                    infoBar.spUpdateInfo.Visibility = Visibility.Collapsed;
                    infoBar.tbHeading.Text = App.RM.GetString("ProgramsUpToDate");
                    infoBar.imgShield.Source = App.greenShield;
                    infoBar.imgSide.Source = greenSide;
                    infoBar.btnAction.Visibility = Visibility.Collapsed;
                    infoBar.pbProgressBar.Visibility = Visibility.Collapsed;

                    infoBar.line.Visibility = Visibility.Collapsed;

                    if (App.Applications.Count == 1)
                        infoBar.tbStatus.Text = App.RM.GetString("Succeeded") + ": " + App.Applications.Count + " " + App.RM.GetString("Update");
                    else
                        infoBar.tbStatus.Text = App.RM.GetString("Succeeded") + ": " + App.Applications.Count + " " + App.RM.GetString("Updates");

                    App.AddShieldToButton(infoBar.btnAction);

                    #endregion

                    #region Code
                    App.InstallInProgress = false;
                    App.UpdatesFound = false;
                    App.CanCheckForUpdates = true;

                    tbUpdatesInstalled.Text = App.RM.GetString("TodayAt") + " " + DateTime.Now.ToShortTimeString();

                    Settings.Default.lastInstall = DateTime.Now.ToShortDateString() + " " + App.RM.GetString("At") + " " + DateTime.Now.ToShortTimeString();
                    Settings.Default.lastInstall = DateTime.Now.ToShortDateString() + " " + App.RM.GetString("At") + " " + DateTime.Now.ToShortTimeString();

                    App.NotifyIcon.Text = App.RM.GetString("ProgramsUpToDate");

                    if (SevenUpdate.Windows.MainWindow.IsHidden)
                        App.NotifyIcon.ShowBalloonTip(5000, App.RM.GetString("UpdatesComplete"), App.RM.GetString("CompletedInstallingUpdates"), Avalon.Windows.Controls.NotifyBalloonIcon.Info);
                    #endregion
                    break;

                case UILayout.ErrorOccurred:

                    #region GUI Code

                    infoBar.btnAction.Content = App.RM.GetString("TryAgain");
                    infoBar.btnAction.Visibility = Visibility.Visible;
                    infoBar.tbStatus.Visibility = Visibility.Visible;
                    infoBar.pbProgressBar.Visibility = Visibility.Collapsed;
                    infoBar.imgSide.Source = redSide;
                    infoBar.imgShield.Source = App.redShield;
                    infoBar.spUpdateInfo.Visibility = Visibility.Collapsed;
                    infoBar.tbSelectedUpdates.Visibility = Visibility.Collapsed;
                    infoBar.tbHeading.Text = App.RM.GetString("ErrorOccurred");

                    infoBar.line.Visibility = Visibility.Collapsed;


                    App.NotifyIcon.Text = App.RM.GetString("ErrorOccurred");

                    if (errorDescription != null)
                        infoBar.tbStatus.Text = errorDescription;
                    else
                        infoBar.tbStatus.Text = App.RM.GetString("UnknownErrorOccurred");

                    #endregion

                    #region Code
                    App.CanCheckForUpdates = true;
                    App.InstallInProgress = false;
                    App.UpdatesFound = false;

                    if (SevenUpdate.Windows.MainWindow.IsHidden)
                        App.NotifyIcon.ShowBalloonTip(5000, App.RM.GetString("ErrorOccurred"), App.RM.GetString("ErrorOccurred"), Avalon.Windows.Controls.NotifyBalloonIcon.Info);
                    #endregion
                    break;

                case UILayout.RebootNeeded:

                    #region GUI Code

                    infoBar.btnAction.Content = App.RM.GetString("TryAgain");
                    infoBar.btnAction.Visibility = Visibility.Visible;
                    infoBar.tbStatus.Visibility = Visibility.Visible;
                    infoBar.imgShield.Source = App.yellowShield;
                    infoBar.imgSide.Source = yellowSide;
                    infoBar.tbSelectedUpdates.Visibility = Visibility.Collapsed;
                    infoBar.spUpdateInfo.Visibility = Visibility.Collapsed;
                    infoBar.btnAction.Content = App.RM.GetString("RestartNow");
                    infoBar.pbProgressBar.Visibility = Visibility.Collapsed;
                    infoBar.tbHeading.Text = App.RM.GetString("RebootNeeded");
                    infoBar.tbStatus.Text = App.RM.GetString("SaveAndReboot");
                    infoBar.line.Visibility = Visibility.Collapsed;


                    App.NotifyIcon.Text = App.RM.GetString("RebootNeeded");
                    #endregion

                    #region Code
                    App.CanCheckForUpdates = false;
                    App.NotifyIcon.Text = App.RM.GetString("RebootNeeded");

                    if (SevenUpdate.Windows.MainWindow.IsHidden)
                        App.NotifyIcon.ShowBalloonTip(5000, App.RM.GetString("UpdatesComplete"), App.RM.GetString("RebootNeeded"), Avalon.Windows.Controls.NotifyBalloonIcon.Info);
                    #endregion
                    break;

                case UILayout.Canceled:
                    #region GUI Code

                    infoBar.btnAction.Visibility = Visibility.Visible;
                    infoBar.btnAction.Content = App.RM.GetString("TryAgain");
                    infoBar.tbStatus.Visibility = Visibility.Visible;
                    infoBar.pbProgressBar.Visibility = Visibility.Collapsed;
                    infoBar.imgShield.Source = App.redShield;
                    infoBar.imgSide.Source = redSide;
                    infoBar.spUpdateInfo.Visibility = Visibility.Collapsed;
                    infoBar.line.Visibility = Visibility.Collapsed;
                    infoBar.tbSelectedUpdates.Visibility = Visibility.Collapsed;
                    infoBar.tbHeading.Text = App.RM.GetString("UpdatesCanceled");
                    infoBar.tbStatus.Text = App.RM.GetString("CancelInstallation");


                    App.NotifyIcon.Text = App.RM.GetString("UpdatesCanceled");

                    #endregion

                    #region Code
                    try
                    {
                        Process[] process = Process.GetProcessesByName("Seven Update.Admin");
                        for (int x = 0; x < process.Length; x++)
                        {
                            process[x].Kill();
                        }
                    }
                    catch (Exception) { }
                    App.NotifyIcon.Text = App.RM.GetString("UpdatesCanceled");
                    App.CanCheckForUpdates = true;
                    App.UpdatesFound = false;
                    App.InstallInProgress = false;
                    #endregion
                    break;
            }
        }

        #endregion
    }
}
