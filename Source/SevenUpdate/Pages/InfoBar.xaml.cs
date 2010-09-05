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

using System.ComponentModel;
using System.Windows.Controls;


#endregion

namespace SevenUpdate.Pages
{
    #region Enums

    /// <summary>
    ///   The layout for the Info Panel
    /// </summary>
    public enum UILayout
    {
        /// <summary>
        ///   Canceled Updates
        /// </summary>
        Canceled,

        /// <summary>
        ///   Check for updates
        /// </summary>
        CheckForUpdates,

        /// <summary>
        ///   Checking for updates
        /// </summary>
        CheckingForUpdates,

        /// <summary>
        ///   When connecting to the admin service
        /// </summary>
        ConnectingToService,

        /// <summary>
        ///   When downloading of updates has been completed
        /// </summary>
        DownloadCompleted,

        /// <summary>
        ///   Downloading updates
        /// </summary>
        Downloading,

        /// <summary>
        ///   An Error Occurred when downloading/installing updates
        /// </summary>
        ErrorOccurred,

        /// <summary>
        ///   When installation of updates have completed
        /// </summary>
        InstallationCompleted,

        /// <summary>
        ///   Installing Updates
        /// </summary>
        Installing,

        /// <summary>
        ///   No updates have been found
        /// </summary>
        NoUpdates,

        /// <summary>
        ///   A reboot is needed to finish installing updates
        /// </summary>
        RebootNeeded,

        /// <summary>
        ///   No updates have been found
        /// </summary>
        UpdatesFound,
    }

    #endregion

    /// <summary>
    ///   Interaction logic for InfoBar.xaml
    /// </summary>
    public partial class InfoBar : UserControl, INotifyPropertyChanged
    {
        #region Fields

        private UILayout layout;

        #endregion

        #region Properties
        
        public UILayout UiLayout
        {
            get { return layout; }
            set
            {
                layout = value;
                
                // Call OnPropertyChanged whenever the property is updated
                DataContext = UiLayout;
                OnPropertyChanged("UiLayout");
            }
        }

        #endregion

        /// <summary>
        ///   A Control that displays update progress and information
        /// </summary>
        public InfoBar()
        {
            InitializeComponent();
            
        }

        //private void infoBar.Layout = UILayout layout, string errorDescription, int updatesInstalled, int updatesFailed)
        //{

        //    switch (layout)
        //    {
        //        case UILayout.Canceled:

        //            #region GUI Code

        //            infoBar.btnAction.Visibility = Visibility.Visible;

        //            infoBar.lblHeading.Text = Properties.Resources.UpdatesCanceled;
        //            infoBar.btnAction.ButtonText = Properties.Resources.TryAgain;d

        //            #endregion

        //            break;
        //        case UILayout.CheckForUpdates:

        //            #region GUI Code

        //            infoBar.btnAction.Visibility = Visibility.Visible;
        //            infoBar.btnAction.ButtonText = Properties.Resources.CheckForUpdates;
        //            infoBar.lblHeading.Text = Properties.Resources.CheckForUpdatesHeading;

        //            #endregion

        //            break;
        //        case UILayout.CheckingForUpdates:

        //            #region GUI Code

        //            infoBar.imgSideBanner.Visibility = Visibility.Collapsed;
        //            infoBar.pbProgressBar.Visibility = Visibility.Visible;

        //            infoBar.lblSelectedUpdates.FontWeight = FontWeights.Normal;

        //            infoBar.lblHeading.Text = Properties.Resources.CheckingForUpdates + "...";
        //            lblRecentCheck.Text = Properties.Resources.TodayAt + " " + DateTime.Now.ToShortTimeString();

        //            #endregion

        //            #region Code

        //            App.IsInstallInProgress = true;

        //            #endregion

        //            break;
        //        case UILayout.ConnectingToService:

        //            #region GUI Code

        //            infoBar.pbProgressBar.Visibility = Visibility.Visible;
        //            infoBar.lblHeading.Text = Properties.Resources.ConnectingToService + "...";


        //            #endregion

        //            #region Code

        //            AdminClient.Connect();

        //            #endregion

        //            break;
        //        case UILayout.Downloading:

        //            #region GUI Code
        //            infoBar.pbProgressBar.Visibility = Visibility.Visible;
        //            infoBar.btnAction.Visibility = Visibility.Visible;
        //            infoBar.btnAction.IsShieldNeeded = true;

        //            infoBar.lblHeading.Text = Properties.Resources.DownloadingUpdates + "...";
        //            infoBar.btnAction.ButtonText = Properties.Resources.StopDownload;

        //            infoBar.imgSideBanner.Visibility = Visibility.Visible;

        //            #endregion

        //            break;
        //        case UILayout.DownloadCompleted:

        //            #region GUI Code

        //            infoBar.btnAction.IsShieldNeeded = true;
        //            infoBar.lblSelectedUpdates.Visibility = Visibility.Visible;
        //            infoBar.line.Visibility = Visibility.Visible;

        //            infoBar.lblHeading.Text = Properties.Resources.UpdatesReadyInstalled;
        //            infoBar.btnAction.ButtonText = Properties.Resources.InstallUpdates;
        //            infoBar.imgSideBanner.Visibility = Visibility.Visible;

        //            #endregion

        //            break;
        //        case UILayout.ErrorOccurred:

        //            #region GUI Code

        //            infoBar.btnAction.Visibility = Visibility.Visible;

        //            infoBar.lblHeading.Text = Properties.Resources.ErrorOccurred;
        //            infoBar.btnAction.ButtonText = Properties.Resources.TryAgain;
        //            infoBar.lblStatus.Text = errorDescription ?? Properties.Resources.UnknownErrorOccurred;
        //            infoBar.imgSideBanner.Visibility = Visibility.Visible;

        //            #endregion

        //            break;
        //        case UILayout.Installing:

        //            #region GUI Code

        //            infoBar.btnAction.IsShieldNeeded = true;
        //            infoBar.btnAction.Visibility = Visibility.Visible;
        //            infoBar.pbProgressBar.Visibility = Visibility.Visible;

        //            infoBar.btnAction.ButtonText = Properties.Resources.StopInstallation;
        //            infoBar.lblHeading.Text = Properties.Resources.InstallingUpdates + "...";

        //            infoBar.imgSideBanner.Visibility = Visibility.Visible;

        //            #endregion

        //            #region Code

        //            App.IsInstallInProgress = true;

        //            #endregion

        //            break;
        //        case UILayout.InstallationCompleted:

        //            #region GUI Code

        //            infoBar.btnAction.IsShieldNeeded = true;

        //            infoBar.lblHeading.Text = Properties.Resources.UpdatesInstalled;
        //            infoBar.imgSideBanner.Visibility = Visibility.Visible;

        //            #region Update Status

        //            infoBar.lblStatus.Text = Properties.Resources.Succeeded + ": " + updatesInstalled + " ";

        //            if (updatesInstalled == 1)
        //                infoBar.lblStatus.Text += Properties.Resources.Update;
        //            else
        //                infoBar.lblStatus.Text += Properties.Resources.Updates;

        //            if (updatesFailed > 0)
        //            {
        //                if (updatesInstalled == 0)
        //                    infoBar.lblStatus.Text = Properties.Resources.Failed + ": " + updatesFailed + " ";
        //                else
        //                    infoBar.lblStatus.Text += ", " + Properties.Resources.Failed + ": " + updatesFailed + " ";

        //                if (updatesFailed == 1)
        //                    infoBar.lblStatus.Text += Properties.Resources.Update;
        //                else
        //                    infoBar.lblStatus.Text += Properties.Resources.Updates;
        //            }

        //            #endregion

        //            lblUpdatesInstalled.Text = Properties.Resources.TodayAt + " " + DateTime.Now.ToShortTimeString();

        //            #endregion

        //            #region Code

        //            Settings.Default.lastInstall = DateTime.Now;

        //            #endregion

        //            break;
        //        case UILayout.NoUpdates:

        //            #region GUI Code

        //            infoBar.lblHeading.Text = Properties.Resources.ProgramsUpToDate;

        //            infoBar.imgSideBanner.Visibility = Visibility.Visible;

        //            #endregion

        //            #region Code

        //            App.IsInstallInProgress = false;

        //            #endregion

        //            break;
        //        case UILayout.RebootNeeded:

        //            #region GUI Code

        //            infoBar.btnAction.Visibility = Visibility.Visible;

        //            infoBar.btnAction.ButtonText = Properties.Resources.RestartNow;
        //            infoBar.lblHeading.Text = Properties.Resources.RebootNeeded;


        //            #endregion

        //            break;

        //        case UILayout.UpdatesFound:

        //            #region GUI Code

        //            infoBar.btnAction.IsShieldNeeded = true;
        //            infoBar.lblSelectedUpdates.Visibility = Visibility.Visible;
        //            infoBar.lblViewOptionalUpdates.Visibility = Visibility.Visible;
        //            infoBar.lblViewImportantUpdates.Visibility = Visibility.Visible;
        //            infoBar.line.Visibility = Visibility.Visible;
        //            infoBar.spnlUpdateInfo.Visibility = Visibility.Visible;
        //            infoBar.line.Y1 = 25;

        //            infoBar.lblHeading.Text = Properties.Resources.DownloadAndInstallUpdates;
        //            infoBar.btnAction.ButtonText = Properties.Resources.InstallUpdates;

        //            infoBar.imgSideBanner.Visibility = Visibility.Visible;

        //            #endregion

        //            break;
        //    }
        //}


        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        /// <summary>
        ///   When a property has changed, call the <see cref = "OnPropertyChanged" /> Event
        /// </summary>
        /// <param name = "name" />
        private void OnPropertyChanged(string name)
        {
            var handler = PropertyChanged;

            if (handler != null)
                handler(this, new PropertyChangedEventArgs(name));
        }

        private void Infobar_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            DataContext = UiLayout;
        }
    }
}