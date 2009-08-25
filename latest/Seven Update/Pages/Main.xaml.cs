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

        /// <summary>
        /// Indicates if an Auto Search was performed
        /// </summary>
        internal static bool AutoCheck { get; set; }

        internal static string LastPageVisited { get; set; }

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
            if (AutoCheck)
            {
                CheckForUpdates();
            }

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
            if (infoBar.tbViewOptionalUpdates.Cursor == Cursors.Hand)
            {
                if (SevenUpdate.Windows.MainWindow.ns.CanGoForward)
                    SevenUpdate.Windows.MainWindow.ns.GoForward();
                else
                {
                    SevenUpdate.Windows.MainWindow.ns.Navigate(new Uri(@"Pages\Update Info.xaml", UriKind.Relative));
                }
            }
        }

        private void tbViewImportantUpdates_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (infoBar.tbViewImportantUpdates.Cursor == Cursors.Hand)
            {
                if (SevenUpdate.Windows.MainWindow.ns.CanGoForward && LastPageVisited == "UpdateInfo")
                    SevenUpdate.Windows.MainWindow.ns.GoForward();
                else
                {
                    SevenUpdate.Windows.MainWindow.ns.Navigate(new Uri(@"Pages\Update Info.xaml", UriKind.Relative));
                }
            }
        }

        #endregion

        private void RestoreHidden_RestoredHiddenUpdateEventHandler(object sender, EventArgs e)
        {
            CheckForUpdates(true);
        }

        private void btnAction_Click(object sender, RoutedEventArgs e)
        {
            if (infoBar.btnAction.Content.ToString() == App.RM.GetString("InstallUpdates"))
            {
                DownloadInstallUpdates();
            }
            else if (infoBar.btnAction.Content.ToString() == App.RM.GetString("CancelUpdates"))
            {
                //Cancel installation of updates
                Client.AbortInstall();
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

        void InstallProgressChanged(Client.InstallProgressChangedEventArgs e)
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

        void DownloadProgressChanged(Client.DownloadProgressChangedEventArgs e)
        {
            try
            {
                infoBar.tbStatus.Text = App.RM.GetString("DownloadingUpdates") + " (" +
                    Shared.ConvertFileSize(e.BytesTotal) + ", " + (e.BytesTransferred * 100 / e.BytesTotal).ToString("F0") + " % " + App.RM.GetString("Complete") + ")";
                App.taskbarIcon.Text = infoBar.tbStatus.Text;

            }
            catch (NullReferenceException)
            {
            }
        }

        void InstallDone(Client.InstallDoneEventArgs e)
        {
            App.InstallInProgress = false;
            App.UpdatesFound = false;
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

        void DownloadDone(Client.DownloadDoneEventArgs e)
        {
            if (e.ErrorOccurred)
            {
                SetUI(UILayout.ErrorOccurred);
            }
            else
            {
                SetUI(UILayout.DownloadCompleted);
                infoBar.btnAction.Visibility = Visibility.Hidden;

                if (infoBar.tbSelectedUpdates.Text == App.RM.GetString("NoUpdatesSelected"))
                    infoBar.btnAction.Visibility = Visibility.Hidden;
            }
        }

        void SearchDone(Search.SearchDoneEventArgs e)
        {
            if (e.Applications.Count > 0)
            {
                App.Applications = e.Applications;
                
                int importantCount = 0;
                int recommendedCount = 0;
                int optionalCount = 0;
                
                for (int x = 0; x < e.Applications.Count; x++)
                {
                    for (int y = 0; y < e.Applications[x].Updates.Count; y++)
                    {
                        totalUpdateSize = App.GetUpdateSize(e.Applications[x].Updates[y].Files, e.Applications[x].Updates[y].Title[0].Value, e.Applications[x].Name[0].Value, e.Applications[x].Directory, e.Applications[x].Is64Bit);

                        switch (e.Applications[x].Updates[y].Importance)
                        {
                            case Importance.Important:
                                importantCount++;
                                break;
                            case Importance.Locale:
                            case Importance.Optional:
                                
                                optionalCount++; 
                                break;
                            case Importance.Recommended:
                                recommendedCount++;
                                break;

                        }
                    }
                }
                #region GUI Updating

                infoBar.tbSelectedUpdates.Text = App.RM.GetString("NoUpdatesSelected");

                SetUI(UILayout.UpdatesFound);

                if (importantCount > 0 || recommendedCount > 0)
                {
                    SetTextBlock(true);
                    if (App.Settings.IncludeRecommended)
                        importantCount += recommendedCount;
                    else
                        optionalCount += recommendedCount;



                    if (importantCount == 1)
                        infoBar.tbViewImportantUpdates.Text = importantCount + " " + App.RM.GetString("ImportantUpdateAvaliable") + " " ;
                    else
                        infoBar.tbViewImportantUpdates.Text = importantCount + " " + App.RM.GetString("ImportantUpdatesAvaliable");

                    

                    if (optionalCount > 0)
                    {

                        if (optionalCount == 1)
                            infoBar.tbViewOptionalUpdates.Text = optionalCount + " " + App.RM.GetString("OptionalUpdateAvaliable");
                        else
                            infoBar.tbViewOptionalUpdates.Text = optionalCount + " " + App.RM.GetString("OptionalUpdatesAvaliable");

                        infoBar.tbViewOptionalUpdates.Visibility = Visibility.Visible;


                    }
                    else
                    {
                        infoBar.tbViewOptionalUpdates.Visibility = Visibility.Hidden;
                    }
                }
                else
                    if (optionalCount > 0)
                    {

                        if (optionalCount == 1)
                        {
                            infoBar.tbViewOptionalUpdates.Text = optionalCount + " " + App.RM.GetString("OptionalUpdateAvaliable");
                        }
                        else
                        {
                            infoBar.tbViewOptionalUpdates.Text = optionalCount + " " + App.RM.GetString("OptionalUpdatesAvaliable");
                        }

                        infoBar.tbViewOptionalUpdates.Visibility = Visibility.Visible;


                        SetTextBlock(false);
                        infoBar.tbViewImportantUpdates.Text = App.RM.GetString("NoImportantUpdates");

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

        void Client_InstallProgressChangedEventHandler(object sender, Client.InstallProgressChangedEventArgs e)
        {
            if (!this.Dispatcher.CheckAccess())
            {
                DispatcherObjectDelegates.BeginInvoke<Client.InstallProgressChangedEventArgs>(this.Dispatcher, InstallProgressChanged, e);
            }
            else
                InstallProgressChanged(e);
        }

        void Client_DownloadProgressChangedEventHandler(object sender, Client.DownloadProgressChangedEventArgs e)
        {
            if (!this.Dispatcher.CheckAccess())
            {
                DispatcherObjectDelegates.BeginInvoke<Client.DownloadProgressChangedEventArgs>(this.Dispatcher, DownloadProgressChanged, e);
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

        void Client_InstallDoneEventHandler(object sender, Client.InstallDoneEventArgs e)
        {
            if (!this.Dispatcher.CheckAccess())
            {
                DispatcherObjectDelegates.BeginInvoke<Client.InstallDoneEventArgs>(this.Dispatcher, InstallDone, e);
            }
            else
                InstallDone(e);
        }

        void Client_DownloadDoneEventHandler(object sender, Client.DownloadDoneEventArgs e)
        {
            if (!this.Dispatcher.CheckAccess())
            {
                DispatcherObjectDelegates.BeginInvoke<Client.DownloadDoneEventArgs>(this.Dispatcher, DownloadDone, e);
            }
            else
                DownloadDone(e);
        }

        #endregion
        void Client_ErrorOccurredEventHandler(object sender, Client.ErrorOccurredEventArgs e)
        {
            if (e.ErrorDescription == "Network Connection Error")
            {
                if (!this.Dispatcher.CheckAccess())
                {
                    DispatcherObjectDelegates.BeginInvoke<UILayout, string>(this.Dispatcher, SetUI, UILayout.ErrorOccurred, App.RM.GetString("CheckConnection"));
                }
            }
            else if (e.ErrorDescription == "Seven Update Server Error")
            {
                if (!this.Dispatcher.CheckAccess())
                {
                    DispatcherObjectDelegates.BeginInvoke<UILayout, string>(this.Dispatcher, SetUI, UILayout.ErrorOccurred, App.RM.GetString("CouldNotConnect"));
                }
            }
            else
            {
                if (!this.Dispatcher.CheckAccess())
                {
                    DispatcherObjectDelegates.BeginInvoke<UILayout, string>(this.Dispatcher, SetUI, UILayout.ErrorOccurred, e.ErrorDescription);
                }
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

            Client.AbortInstall();
            TextWriter tw = new StreamWriter(Shared.userStore + "error.log");
            tw.WriteLine(DateTime.Now.ToString() + ": " + e.ErrorDescription);
            tw.Close();

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
            if (App.Applications.Count > 0)
            {
                LicenseAgreement sla = new LicenseAgreement();
                if (sla.LoadLicenses() == true)
                {
                    SetUI(UILayout.Canceled);
                    return;
                }

                if (Client.Install())
                {
                    App.RemoveShieldFromButton(infoBar.btnAction);
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
            infoBar.tbViewImportantUpdates.MouseDown += new MouseButtonEventHandler(tbViewImportantUpdates_MouseDown);
            infoBar.tbViewOptionalUpdates.MouseDown += new MouseButtonEventHandler(tbViewOptionalUpdates_MouseDown);
            Search.SearchDoneEventHandler += new EventHandler<Search.SearchDoneEventArgs>(Search_SearchDoneEventHandler);
            Client.DownloadProgressChangedEventHandler += new EventHandler<Client.DownloadProgressChangedEventArgs>(Client_DownloadProgressChangedEventHandler);
            Client.DownloadDoneEventHandler += new EventHandler<Client.DownloadDoneEventArgs>(Client_DownloadDoneEventHandler);
            Client.InstallProgressChangedEventHandler += new EventHandler<Client.InstallProgressChangedEventArgs>(Client_InstallProgressChangedEventHandler);
            Client.InstallDoneEventHandler += new EventHandler<Client.InstallDoneEventArgs>(Client_InstallDoneEventHandler);
            Client.ErrorOccurredEventHandler += new EventHandler<Client.ErrorOccurredEventArgs>(Client_ErrorOccurredEventHandler);
            RestoreUpdates.RestoredHiddenUpdateEventHandler += new EventHandler<EventArgs>(RestoreUpdates_RestoredHiddenUpdateEventHandler);

            #endregion
        }

        void RestoreUpdates_RestoredHiddenUpdateEventHandler(object sender, EventArgs e)
        {
            CheckForUpdates(true);
        }

        void taskbarIcon_TrayMouseDoubleClick(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        void taskbarIcon_TrayBalloonTipClicked(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Sets the TextBlock depending on the current UI state
        /// </summary>
        /// <param name="blue">Turns the TextBlock blue and enables underlining</param>
        void SetTextBlock(bool blue)
        {
            infoBar.tbViewImportantUpdates.MouseEnter -= TextBlock_MouseEnter;
            infoBar.tbViewImportantUpdates.MouseLeave -= TextBlock_MouseLeave;

            if (blue)
            {
                infoBar.tbViewImportantUpdates.Cursor = Cursors.Hand;

                infoBar.tbViewImportantUpdates.Foreground = new SolidColorBrush(Color.FromRgb(0, 102, 204));

                infoBar.tbViewImportantUpdates.MouseEnter += new MouseEventHandler(TextBlock_MouseEnter);
                infoBar.tbViewImportantUpdates.MouseLeave += new MouseEventHandler(TextBlock_MouseLeave);

            }
            else
            {
                infoBar.tbViewImportantUpdates.Cursor = Cursors.Arrow;

                infoBar.tbViewImportantUpdates.Foreground = new SolidColorBrush(Colors.Black);
            }
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
                case UILayout.NoUpdates:
                    #region Code
                    App.InstallInProgress = false;
                    App.CanCheckForUpdates = true;

                    if (layout == UILayout.NoUpdates)
                    {
                        App.UpdatesFound = false;
                        infoBar.Height = 90;
                        infoBar.rectBorder.Height = 90;
                        infoBar.rectBorder.VerticalAlignment = VerticalAlignment.Top;

                        infoBar.tbStatus.Visibility = Visibility.Visible;

                        infoBar.tbSelectedUpdates.Visibility = Visibility.Hidden;

                        infoBar.pbProgressBar.Visibility = Visibility.Hidden;

                        infoBar.tbHeading.Text = App.RM.GetString("ProgramsUpToDate");

                        App.taskbarIcon.Text = App.RM.GetString("NoNewUpdates");

                        infoBar.imgSide.Source = greenSide;

                        infoBar.imgShield.Source = App.greenShield;

                        infoBar.pbProgressBar.Visibility = Visibility.Hidden;

                        App.taskbarIcon.Text = App.RM.GetString("NoNewUpdates");

                        infoBar.tbStatus.Text = App.RM.GetString("NoNewUpdates");

                        infoBar.spUpdateInfo.Visibility = Visibility.Hidden;

                        // verticalLine.Visibility = Visibility.Hidden;

                        infoBar.btnAction.Visibility = Visibility.Hidden;

                    }
                    else
                    {
                        infoBar.Height = double.NaN;
                        infoBar.rectBorder.Height = double.NaN;
                        infoBar.rectBorder.VerticalAlignment = VerticalAlignment.Stretch;

                        infoBar.tbStatus.Visibility = Visibility.Hidden;

                        infoBar.pbProgressBar.Visibility = Visibility.Hidden;

                        infoBar.tbHeading.Text = App.RM.GetString("DownloadAndInstallUpdates");

                        App.taskbarIcon.Text = App.RM.GetString("DownloadAndInstallUpdates");

                        infoBar.imgSide.Source = yellowSide;

                        infoBar.imgShield.Source = App.yellowShield;

                        infoBar.btnAction.Content = App.RM.GetString("InstallUpdates");

                        

                        infoBar.spUpdateInfo.Visibility = Visibility.Visible;

                        infoBar.tbSelectedUpdates.Visibility = Visibility.Visible;

                        //verticalLine.Visibility = Visibility.Visible;

                        infoBar.btnAction.Visibility = Visibility.Hidden;
                    }

                    #endregion
                    break;
                case UILayout.CheckingForUpdates:
                    #region Code
                    infoBar.Height = 90;
                    infoBar.rectBorder.Height = 90;
                    infoBar.rectBorder.VerticalAlignment = VerticalAlignment.Top;
                    

                    infoBar.tbSelectedUpdates.Visibility = Visibility.Hidden;

                    infoBar.tbStatus.Visibility = Visibility.Hidden;

                    infoBar.tbHeading.Text = App.RM.GetString("CheckingForUpdates") + "...";

                    App.taskbarIcon.Text = App.RM.GetString("CheckingForUpdates") +"...";

                    infoBar.imgShield.Source = suIcon;
                    infoBar.imgSide.Source = null;

                    infoBar.pbProgressBar.Visibility = Visibility.Visible;
                    #endregion

                    #region Code
                    App.InstallInProgress = false;
                    App.taskbarIcon.Text = App.RM.GetString("DownloadAndInstallUpdates");
                    if (SevenUpdate.Windows.MainWindow.IsHidden)
                        App.taskbarIcon.ShowBalloonTip(5000, App.RM.GetString("DownloadAndInstallUpdates"), App.RM.GetString("UpdatesAvailable"), System.Windows.Forms.ToolTipIcon.Info);
                    App.CanCheckForUpdates = false;

                    tbRecentCheck.Text = App.RM.GetString("TodayAt") + " " + DateTime.Now.ToShortTimeString();

                    #endregion
                    break;
                case UILayout.Downloading:

                    #region Code
                    App.RemoveShieldFromButton(infoBar.btnAction);

                    infoBar.Height = double.NaN;
                    infoBar.rectBorder.Height = double.NaN;
                    infoBar.rectBorder.VerticalAlignment = VerticalAlignment.Stretch;
                    
                    infoBar.imgShield.Source = App.yellowShield;

                    infoBar.imgSide.Source = yellowSide;

                    infoBar.tbSelectedUpdates.Visibility = Visibility.Hidden;

                    

                    infoBar.spUpdateInfo.Visibility = Visibility.Hidden;

                    // verticalLine.Visibility = Visibility.Hidden;

                    infoBar.tbStatus.Visibility = Visibility.Visible;

                    infoBar.pbProgressBar.Visibility = Visibility.Visible;

                    infoBar.btnAction.Visibility = Visibility.Visible;

                    infoBar.btnAction.Content = App.RM.GetString("CancelUpdates");

                    infoBar.tbStatus.Text = App.RM.GetString("PreparingDownload");

                    infoBar.tbHeading.Text = App.RM.GetString("DownloadingUpdates") + "...";

                    #endregion

                    #region Code
                    App.InstallInProgress = false;
                    App.CanCheckForUpdates = false;

                    App.taskbarIcon.Text = App.RM.GetString("DownloadingUpdatesBackground");
                    if (SevenUpdate.Windows.MainWindow.IsHidden)
                        App.taskbarIcon.ShowBalloonTip(5000, App.RM.GetString("DownloadingUpdates"), App.RM.GetString("DownloadingUpdatesBackground"), System.Windows.Forms.ToolTipIcon.Info);
                    #endregion
                    break;
                case UILayout.DownloadCompleted:

                    #region Code

                    infoBar.Height = double.NaN;
                    infoBar.rectBorder.Height = double.NaN;
                    infoBar.rectBorder.VerticalAlignment = VerticalAlignment.Stretch;
                    

                    infoBar.tbStatus.Visibility = Visibility.Hidden;

                    

                    infoBar.tbSelectedUpdates.Visibility = Visibility.Visible;

                    //verticalLine.Visibility = Visibility.Visible;

                    infoBar.tbStatus.Visibility = Visibility.Hidden;

                    infoBar.pbProgressBar.Visibility = Visibility.Hidden;

                    App.RemoveShieldFromButton(infoBar.btnAction);

                    infoBar.btnAction.Visibility = Visibility.Visible;

                    infoBar.btnAction.Content = App.RM.GetString("InstallUpdates");

                    infoBar.tbHeading.Text = App.RM.GetString("UpdatesReadyInstalled");

                    infoBar.imgShield.Source = App.yellowShield;

                    infoBar.imgSide.Source = yellowSide;

                    #endregion

                    #region Code
                    App.InstallInProgress = false;
                    App.CanCheckForUpdates = true;

                    App.taskbarIcon.Text = App.RM.GetString("UpdatesDownloaded");
                    if (SevenUpdate.Windows.MainWindow.IsHidden)
                        App.taskbarIcon.ShowBalloonTip(5000, App.RM.GetString("UpdatesDownloaded"), App.RM.GetString("FinishedDownloading"), System.Windows.Forms.ToolTipIcon.Info);
                    #endregion
                    break;

                case UILayout.Installing:

                    #region Code

                    infoBar.Height = double.NaN;
                    infoBar.rectBorder.Height = double.NaN;
                    infoBar.rectBorder.VerticalAlignment = VerticalAlignment.Stretch;

                    infoBar.btnAction.Visibility = Visibility.Visible;

                    infoBar.pbProgressBar.Visibility = Visibility.Visible;

                    infoBar.tbStatus.Visibility = Visibility.Visible;

                    // verticalLine.Visibility = Visibility.Hidden;

                    infoBar.tbSelectedUpdates.Visibility = Visibility.Hidden;

                    

                    infoBar.spUpdateInfo.Visibility = Visibility.Hidden;

                    infoBar.imgShield.Source = suIcon;

                    infoBar.btnAction.Content = App.RM.GetString("CancelUpdates");

                    infoBar.tbStatus.Text = App.RM.GetString("PreparingInstall");

                    infoBar.tbHeading.Text = App.RM.GetString("InstallingUpdates") + "...";

                    #endregion

                    #region Code
                    App.InstallInProgress = true;
                    App.CanCheckForUpdates = false;

                    App.taskbarIcon.Text = App.RM.GetString("PreparingInstall");
                    if (SevenUpdate.Windows.MainWindow.IsHidden)
                        App.taskbarIcon.ShowBalloonTip(5000, App.RM.GetString("InstallingUpdates"), App.RM.GetString("InstallingUpdatesBackground"), System.Windows.Forms.ToolTipIcon.Info);
                    #endregion
                    break;

                case UILayout.InstallationCompleted:

                    #region Code
                    infoBar.Height = 90;
                    infoBar.rectBorder.Height = 90;
                    infoBar.rectBorder.VerticalAlignment = VerticalAlignment.Top;

                    App.AddShieldToButton(infoBar.btnAction);
                    infoBar.tbStatus.Visibility = Visibility.Visible;

                    infoBar.tbSelectedUpdates.Visibility = Visibility.Hidden;

                    

                    infoBar.spUpdateInfo.Visibility = Visibility.Hidden;

                    //verticalLine.Visibility = Visibility.Hidden;

                    infoBar.btnAction.Visibility = Visibility.Hidden;

                    infoBar.pbProgressBar.Visibility = Visibility.Hidden;

                    if (App.Applications.Count == 1)
                        infoBar.tbStatus.Text = App.RM.GetString("Succeeded") + ": " + App.Applications.Count + " " + App.RM.GetString("Update");
                    else
                        infoBar.tbStatus.Text = App.RM.GetString("Succeeded") + ": " + App.Applications.Count + " " + App.RM.GetString("Updates");

                    infoBar.tbHeading.Text = App.RM.GetString("ProgramsUpToDate");

                    infoBar.imgShield.Source = App.greenShield;

                    infoBar.imgSide.Source = greenSide;

                    #endregion

                    #region Code
                    App.InstallInProgress = false;
                    App.UpdatesFound = false;
                    Settings.Default.lastInstall = DateTime.Now.ToShortDateString() + " " + App.RM.GetString("At") + " " + DateTime.Now.ToShortTimeString();
                    tbUpdatesInstalled.Text = App.RM.GetString("TodayAt") + " " + DateTime.Now.ToShortTimeString();
                    App.CanCheckForUpdates = true;

                    Settings.Default.lastInstall = DateTime.Now.ToShortDateString() + " " + App.RM.GetString("At") + " " + DateTime.Now.ToShortTimeString();
                    App.taskbarIcon.Text = App.RM.GetString("ProgramsUpToDate");
                    if (SevenUpdate.Windows.MainWindow.IsHidden)
                        App.taskbarIcon.ShowBalloonTip(5000, App.RM.GetString("UpdatesComplete"), App.RM.GetString("CompletedInstallingUpdates"), System.Windows.Forms.ToolTipIcon.Info);
                    #endregion
                    break;

                case UILayout.ErrorOccurred:

                    #region Code

                    infoBar.Height = double.NaN;
                    infoBar.rectBorder.Height = double.NaN;
                    infoBar.rectBorder.VerticalAlignment = VerticalAlignment.Stretch;;

                    infoBar.btnAction.Content = App.RM.GetString("TryAgain");

                    App.RemoveShieldFromButton(infoBar.btnAction);

                    infoBar.btnAction.Visibility = Visibility.Visible;

                    infoBar.tbStatus.Visibility = Visibility.Visible;

                    infoBar.pbProgressBar.Visibility = Visibility.Hidden;

                    infoBar.imgSide.Source = redSide;

                    infoBar.imgShield.Source = App.redShield;

                    

                    infoBar.spUpdateInfo.Visibility = Visibility.Hidden;

                    // verticalLine.Visibility = Visibility.Hidden;

                    infoBar.tbSelectedUpdates.Visibility = Visibility.Hidden;

                    infoBar.tbHeading.Text = App.RM.GetString("ErrorOccurred");

                    App.taskbarIcon.Text = App.RM.GetString("ErrorOccurred");

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
                        App.taskbarIcon.ShowBalloonTip(5000, App.RM.GetString("ErrorOccurred"), App.RM.GetString("ErrorOccurred"), System.Windows.Forms.ToolTipIcon.Info);
                    #endregion
                    break;

                case UILayout.RebootNeeded:

                    #region Code

                    infoBar.Height = double.NaN;
                    infoBar.rectBorder.Height = double.NaN;
                    infoBar.rectBorder.VerticalAlignment = VerticalAlignment.Stretch;

                    infoBar.btnAction.Content = App.RM.GetString("TryAgain");

                    infoBar.btnAction.Visibility = Visibility.Visible;

                    infoBar.tbStatus.Visibility = Visibility.Visible;

                    infoBar.imgShield.Source = App.yellowShield;

                    infoBar.imgSide.Source = yellowSide;

                    infoBar.tbSelectedUpdates.Visibility = Visibility.Hidden;

                    

                    infoBar.spUpdateInfo.Visibility = Visibility.Hidden;

                    //verticalLine.Visibility = Visibility.Hidden;

                    infoBar.btnAction.Content = App.RM.GetString("RestartNow");

                    infoBar.pbProgressBar.Visibility = Visibility.Hidden;

                    infoBar.tbHeading.Text = App.RM.GetString("RebootNeeded");

                    App.taskbarIcon.Text = App.RM.GetString("RebootNeeded");

                    infoBar.tbStatus.Text = App.RM.GetString("SaveAndReboot");

                    App.RemoveShieldFromButton(infoBar.btnAction);

                    #endregion

                    #region Code
                    App.CanCheckForUpdates = false;
                    App.taskbarIcon.Text = App.RM.GetString("RebootNeeded");
                    if (SevenUpdate.Windows.MainWindow.IsHidden)
                        App.taskbarIcon.ShowBalloonTip(5000, App.RM.GetString("UpdatesComplete"), App.RM.GetString("RebootNeeded"), System.Windows.Forms.ToolTipIcon.Info);
                    #endregion
                    break;

                case UILayout.Canceled:
                    #region Code

                    infoBar.Height = double.NaN;
                    infoBar.rectBorder.Height = double.NaN;
                    infoBar.rectBorder.VerticalAlignment = VerticalAlignment.Stretch;
                    infoBar.btnAction.Visibility = Visibility.Visible;

                    infoBar.btnAction.Content = App.RM.GetString("TryAgain");

                    App.RemoveShieldFromButton(infoBar.btnAction);

                    infoBar.tbStatus.Visibility = Visibility.Visible;

                    infoBar.pbProgressBar.Visibility = Visibility.Hidden;

                    infoBar.imgShield.Source = App.redShield;

                    infoBar.imgSide.Source = redSide;

                    

                    infoBar.spUpdateInfo.Visibility = Visibility.Hidden;

                    // verticalLine.Visibility = Visibility.Hidden;

                    infoBar.tbSelectedUpdates.Visibility = Visibility.Hidden;

                    infoBar.tbHeading.Text = App.RM.GetString("UpdatesCanceled");

                    App.taskbarIcon.Text = App.RM.GetString("UpdatesCanceled");

                    infoBar.tbStatus.Text = App.RM.GetString("CancelInstallation");

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
                    App.taskbarIcon.Text = App.RM.GetString("UpdatesCanceled");
                    App.CanCheckForUpdates = true;
                    App.UpdatesFound = false;
                    App.InstallInProgress = false;
                    #endregion
                    break;
            }
        }

        #endregion

        #region AppUpdate Events

        void AppUpdate_UpdateSelectionChangedEventHandler(object sender, SevenUpdate.Pages.UpdateInfo.UpdateSelectionChangedEventArgs e)
        {
            infoBar.btnAction.Visibility = Visibility.Visible;

            SetTextBlock(false);
            infoBar.tbSelectedUpdates.Text = App.RM.GetString("NoUpdatesSelected");

            infoBar.btnAction.Visibility = Visibility.Hidden;

            #region GUI Updating

            if (e.ImportantUpdates > 0)
            {
                SetTextBlock(true);

                infoBar.btnAction.Visibility = Visibility.Visible;

                if (e.ImportantUpdates == 1)
                    infoBar.tbSelectedUpdates.Text = e.ImportantUpdates + " " + App.RM.GetString("ImportantUpdateSelected");
                else
                    infoBar.tbSelectedUpdates.Text = e.ImportantUpdates + " " + App.RM.GetString("ImportantUpdatesSelected");

                if (e.ImportantDownloadSize > 0)
                    infoBar.tbSelectedUpdates.Text += ", " + Shared.ConvertFileSize(e.ImportantDownloadSize);

                infoBar.tbSelectedUpdates.Text += Environment.NewLine;

            }

            if (e.OptionalUpdates > 0)
            {
                SetTextBlock(true);
                infoBar.btnAction.Visibility = Visibility.Visible;

                if (e.ImportantUpdates == 0)
                    if (e.OptionalUpdates == 1)
                        infoBar.tbSelectedUpdates.Text = e.OptionalUpdates + " " + App.RM.GetString("OptionalUpdateSelected");
                    else
                        infoBar.tbSelectedUpdates.Text = e.OptionalUpdates + " " + App.RM.GetString("OptionalUpdatesSelected");
                else
                    if (e.OptionalUpdates == 1)
                        infoBar.tbSelectedUpdates.Text += e.OptionalUpdates + " " + App.RM.GetString("OptionalUpdateSelected");
                    else
                        infoBar.tbSelectedUpdates.Text += e.OptionalUpdates + " " + App.RM.GetString("OptionalUpdatesSelected");

                if (e.OptionalDownloadSize > 0)
                    infoBar.tbSelectedUpdates.Text += ", " + Shared.ConvertFileSize(e.OptionalDownloadSize);

                infoBar.tbSelectedUpdates.Text += Environment.NewLine;
            }
            else
            {
                infoBar.tbViewOptionalUpdates.Visibility = Visibility.Hidden;
            }

            if (e.ImportantDownloadSize == 0 && e.OptionalDownloadSize == 0)
            {
                infoBar.tbHeading.Text = App.RM.GetString("InstallUpdatesForPrograms");

                App.taskbarIcon.Text = App.RM.GetString("InstallUpdatesForPrograms");
            }
            else
            {
                infoBar.tbHeading.Text = App.RM.GetString("DownloadAndInstallUpdates");

                App.taskbarIcon.Text = App.RM.GetString("DownloadAndInstallUpdates");
            }
            #endregion
        }

        #endregion

    }
}
