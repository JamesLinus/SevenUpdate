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
using System.Drawing;
using System.Windows.Forms;
using SevenUpdate.Properties;
using System.IO;
using SevenUpdate.WCF;

namespace SevenUpdate.Controls
{
    public partial class InfoBar : UserControl
    {
        #region Enums

        /// <summary>
        /// The layout for the Info Panel
        /// </summary>
        internal enum UILayout
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
            /// An Error Occurred when downloading/installing updates
            /// </summary>
            ErrorOccurred,

            /// <summary>
            /// Checking for updates
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
            /// Updates have been found
            /// </summary>
            UpdatesFound,
        }

        #endregion

        public InfoBar()
        {
            this.Font = SystemFonts.MessageBoxFont;
            InitializeComponent();
            lblHeading.Font = new Font(SystemFonts.MessageBoxFont.Name, 11.25F);
            lblSelectedUpdates.Font = new Font(lblSelectedUpdates.Font, FontStyle.Bold);
            SevenUpdate.Pages.AppUpdate.UpdateSelectionChangedEventHandler+=new EventHandler<SevenUpdate.Pages.AppUpdate.UpdateSelectionChangedEventArgs>(AppUpdate_UpdateSelectionChangedEventHandler);
        }

        #region Methods
        
        /// <summary>
        /// Sets the label depending on the current UI state
        /// </summary>
        /// <param name="blue">Turns the label blue and enables underlining</param>
        void SetLabel(bool blue)
        {
            lblViewImportantUpdates.MouseEnter -= label_MouseEnter;

            lblViewImportantUpdates.MouseLeave -= label_MouseLeave;

            if (blue)
            {
                lblViewImportantUpdates.Cursor = Cursors.Hand;

                lblViewImportantUpdates.ForeColor = Color.FromArgb(0, 102, 204);

                lblViewImportantUpdates.MouseEnter += new EventHandler(label_MouseEnter);

                lblViewImportantUpdates.MouseLeave += new EventHandler(label_MouseLeave);
            }
            else
            {
                lblViewImportantUpdates.Cursor = Cursors.Arrow;

                lblViewImportantUpdates.ForeColor = Color.FromArgb(0, 0, 0);
            }
        }

        /// <summary>
        /// Sets the Main Page UI
        /// </summary>
        /// <param name="layout">Type of layout to set</param>
        internal void SetUI(UILayout layout)
        {
            SetUI(layout, null);
        }

        /// <summary>
        /// Sets the Main Page UI
        /// </summary>
        /// <param name="layout">Type of layout to set</param>
        /// <param name="errorDescription">The error message to display</param>
        internal void SetUI(UILayout layout, string errorDescription)
        {
            switch (layout)
            {
                case UILayout.Canceled:
                    #region Code

                    Height = 109;

                    lblStatus.Visible = false;

                    btnAction.Visible = true;

                    btnAction.Text = Program.RM.GetString("TryAgain");

                    Program.RemoveShieldFromButton(btnAction);

                    lblSubStatus.Visible = true;

                    pbProgressBar.Visible = false;

                    Shield.Image = Resources.redShield;

                    BackgroundImage = Resources.redSide;

                    lblViewImportantUpdates.Visible = false;

                    lblViewOptionalUpdates.Visible = false;

                    verticalLine.Visible = false;

                    lblSelectedUpdates.Visible = false;

                    lblHeading.Text = Program.RM.GetString("UpdatesCanceled");

                    Program.trayIcon.Text = Program.RM.GetString("UpdatesCanceled");

                    lblSubStatus.Text = Program.RM.GetString("CancelInstallation");

                    #endregion
                    break;
                case UILayout.NoUpdates:
                case UILayout.CheckingForUpdates:
                    #region Code
                    if (layout == UILayout.CheckingForUpdates)
                    {
                        Program.AddShieldToButton(btnAction);
                        Height = 109;

                        lblSelectedUpdates.Visible = false;

                        lblStatus.Visible = false;

                        lblHeading.Text = Program.RM.GetString("CheckingForUpdates")+ "...";

                        Program.trayIcon.Text = Program.RM.GetString("CheckingForUpdates") +"...";

                        lblSubStatus.Text = null;

                        this.Shield.Image = Resources.SUIcon;

                        this.BackgroundImage = null;

                        pbProgressBar.Visible = true;
                    }
                    else
                    {
                        Height = 69;

                        lblStatus.Visible = true;

                        lblSelectedUpdates.Visible = false;

                        lblSubStatus.Visible = false;

                        pbProgressBar.Visible = false;

                        lblHeading.Text = Program.RM.GetString("ProgramsUpToDate");

                        Program.trayIcon.Text = Program.RM.GetString("NoNewUpdates");

                        this.BackgroundImage = Resources.greenSide;

                        this.Shield.Image = Resources.greenShield;

                        pbProgressBar.Visible = false;

                        Program.trayIcon.Text = Program.RM.GetString("NoNewUpdates");

                        lblStatus.Text = Program.RM.GetString("NoNewUpdates");
                    }
                    lblViewImportantUpdates.Visible = false;

                    lblViewOptionalUpdates.Visible = false;

                    verticalLine.Visible = false;

                    btnAction.Visible = false;

                    #endregion
                    break;
                
                case UILayout.DownloadCompleted:
                    #region

                    Height = 109;

                    lblSubStatus.Visible = false;

                    lblViewImportantUpdates.Visible = true;

                    lblSelectedUpdates.Visible = true;

                    verticalLine.Visible = true;

                    lblStatus.Visible = false;

                    verticalLine.Visible = true;

                    pbProgressBar.Visible = false;

                    pbProgressBar.Visible = false;

                    Program.RemoveShieldFromButton(btnAction);

                    btnAction.Visible = true;

                    btnAction.Text = Program.RM.GetString("InstallUpdates");

                    lblHeading.Text = Program.RM.GetString("UpdatesReadyInstalled");

                    Shield.Image = Resources.yellowShield;

                    BackgroundImage = Resources.yellowSide;

                    #endregion
                    break;                
                
                case UILayout.Downloading:
                    #region Code
                    Program.RemoveShieldFromButton(btnAction);
                    Height = 109;

                    Shield.Image = Resources.yellowShield;

                    BackgroundImage = Resources.yellowSide;

                    lblSelectedUpdates.Visible = false;

                    lblStatus.Visible = false;

                    lblViewImportantUpdates.Visible = false;

                    lblViewOptionalUpdates.Visible = false;

                    verticalLine.Visible = false;

                    lblSubStatus.Visible = true;

                    pbProgressBar.Visible = true;

                    btnAction.Visible = true;

                    btnAction.Text = Program.RM.GetString("CancelUpdates");

                    lblSubStatus.Text = Program.RM.GetString("PreparingDownload");

                    lblHeading.Text = Program.RM.GetString("DownloadingUpdates") + "...";

                    #endregion
                    break;

                case UILayout.ErrorOccurred:
                    #region Code

                    Height = 109; 

                    lblStatus.Visible = false;

                    btnAction.Text = Program.RM.GetString("TryAgain");

                    Program.RemoveShieldFromButton(btnAction);

                    btnAction.Visible = true;

                    lblSubStatus.Visible = true;

                    pbProgressBar.Visible = false;

                    BackgroundImage = Resources.redSide;

                    Shield.Image = Resources.redShield;

                    lblViewImportantUpdates.Visible = false;

                    lblViewOptionalUpdates.Visible = false;

                    verticalLine.Visible = false;

                    lblSelectedUpdates.Visible = false;

                    lblHeading.Text = Program.RM.GetString("ErrorOccurred");

                    Program.trayIcon.Text = Program.RM.GetString("ErrorOccurred");

                    if (errorDescription != null)
                        lblSubStatus.Text = errorDescription;
                    else
                        lblSubStatus.Text = Program.RM.GetString("UnknownErrorOccurred");
                    #endregion
                    break;

                case UILayout.InstallationCompleted:
                    #region Code
                    Height = 69;
                    Program.AddShieldToButton(btnAction);
                    lblStatus.Visible = true;

                    lblSelectedUpdates.Visible = false;

                    lblSubStatus.Visible = false;

                    lblViewImportantUpdates.Visible = false;

                    lblViewOptionalUpdates.Visible = false;

                    verticalLine.Visible = false;

                    btnAction.Visible = false;

                    pbProgressBar.Visible = false;

                    if (Program.Applications.Count == 1)
                        lblStatus.Text = Program.RM.GetString("Succeeded") + ": " + Program.Applications.Count + " " + Program.RM.GetString("Update");
                    else
                        lblStatus.Text = Program.RM.GetString("Succeeded") + ": " + Program.Applications.Count + " " + Program.RM.GetString("Updates");

                    lblHeading.Text = Program.RM.GetString("ProgramsUpToDate");

                    Shield.Image = Resources.greenShield;

                    BackgroundImage = Resources.greenSide;

                    #endregion
                    break;

                case UILayout.Installing:
                    #region Code

                    Height = 109;

                    lblStatus.Visible = false;

                    btnAction.Visible = true;

                    pbProgressBar.Visible = true;

                    lblSubStatus.Visible = true;

                    verticalLine.Visible = false;

                    lblSelectedUpdates.Visible = false;

                    lblViewImportantUpdates.Visible = false;

                    lblViewOptionalUpdates.Visible = false;

                    Shield.Image = Resources.SUIcon;

                    btnAction.Text = Program.RM.GetString("CancelUpdates");

                    lblSubStatus.Text = Program.RM.GetString("PreparingInstall");

                    lblHeading.Text = Program.RM.GetString("InstallingUpdates") + "...";

                    #endregion
                    break;

                case UILayout.UpdatesFound:
                    #region Code

                    Height = 109;

                    lblStatus.Visible = false;

                    pbProgressBar.Visible = false;

                    lblHeading.Text = Program.RM.GetString("DownloadAndInstallUpdates");

                    Program.trayIcon.Text = Program.RM.GetString("DownloadAndInstallUpdates");

                    this.BackgroundImage = Resources.yellowSide;

                    this.Shield.Image = Resources.yellowShield;

                    btnAction.Text = Program.RM.GetString("InstallUpdates");

                    lblSubStatus.Visible = false;

                    lblViewImportantUpdates.Visible = true;

                    lblViewOptionalUpdates.Visible = true;

                    lblSelectedUpdates.Visible = true;

                    verticalLine.Visible = true;

                    btnAction.Visible = false;

                    #endregion
                    break;

                case UILayout.RebootNeeded:
                    #region Code

                    Height = 109;

                    lblStatus.Visible = false;

                    btnAction.Text = Program.RM.GetString("TryAgain");

                    btnAction.Visible = true;

                    lblSubStatus.Visible = true;

                    Shield.Image = Resources.yellowShield;

                    BackgroundImage = Resources.yellowSide;

                    lblSelectedUpdates.Visible = false;

                    lblViewImportantUpdates.Visible = false;

                    lblViewOptionalUpdates.Visible = false;

                    verticalLine.Visible = false;

                    btnAction.Text = Program.RM.GetString("RestartNow");

                    pbProgressBar.Visible = false;

                    lblHeading.Text = Program.RM.GetString("RebootNeeded");

                    Program.trayIcon.Text = Program.RM.GetString("RebootNeeded");

                    lblSubStatus.Text = Program.RM.GetString("SaveAndReboot");

                    Program.RemoveShieldFromButton(btnAction);

                    #endregion
                    break;
            }

        }

        #endregion

        #region UI Events

        void InfoBar_Load(object sender, EventArgs e)
        {
            if (!Program.IsAdmin())
            {
                Program.AddShieldToButton(btnAction);
            }

            Search.SearchDoneEventHandler += new EventHandler<Search.SearchDoneEventArgs>(Search_SearchDoneEventHandler);

            Client.DownloadDoneEventHandler += new EventHandler<Client.DownloadDoneEventArgs>(Client_DownloadDoneEventHandler);

            Client.DownloadProgressChangedEventHandler += new EventHandler<Client.DownloadProgressChangedEventArgs>(Client_DownloadProgressChangedEventHandler);

            Client.ErrorOccurredEventHandler += new EventHandler<Client.ErrorOccurredEventArgs>(Client_ErrorOccurredEventHandler);

            Client.InstallProgressChangedEventHandler += new EventHandler<Client.InstallProgressChangedEventArgs>(Client_InstallProgressChangedEventHandler);
            Client.InstallDoneEventHandler += new EventHandler<Client.InstallDoneEventArgs>(Client_InstallDoneEventHandler);
        }

        void label_MouseEnter(object sender, System.EventArgs e)
        {
            Label label = ((Label)sender);
            label.Font = new Font(label.Font, FontStyle.Underline);
        }

        void label_MouseLeave(object sender, System.EventArgs e)
        {
            Label label = ((Label)sender);
            label.Font = new Font(label.Font, FontStyle.Regular);
        }

        #endregion

        #region Client Events

        void Client_InstallDoneEventHandler(object sender, Client.InstallDoneEventArgs e)
        {
            if (!e.ErrorOccurred)
            {
                // if a reboot is needed lets say it
                if (e.RebootNeeded)
                {
                    SetUI(InfoBar.UILayout.RebootNeeded);
                }
                else
                {
                    SetUI(InfoBar.UILayout.InstallationCompleted);
                }
            }
            else
            {
                SetUI(UILayout.ErrorOccurred);
            }
        }

        void Client_InstallProgressChangedEventHandler(object sender, Client.InstallProgressChangedEventArgs e)
        {
            if (e.CurrentProgress == -1)
                lblSubStatus.Text = Program.RM.GetString("PreparingInstall") + "...";
            else
            {
                lblSubStatus.Text = Program.RM.GetString("Installing") + " " + e.UpdateTitle;

                if (e.TotalUpdates > 1)
                    lblSubStatus.Text += Environment.NewLine + e.UpdatesComplete + " " + Program.RM.GetString("OutOf") + " " + e.TotalUpdates + ", " + e.CurrentProgress + "% " + Program.RM.GetString("Complete");
                else
                    lblSubStatus.Text += ", " + e.CurrentProgress + "% " + Program.RM.GetString("Complete");

            }
        }

        void Client_ErrorOccurredEventHandler(object sender, Client.ErrorOccurredEventArgs e)
        {
            if (e.ErrorDescription == "Network Connection Error")
            {
                Program.RemoveShieldFromButton(btnAction);
                SetUI(UILayout.ErrorOccurred, Program.RM.GetString("CheckConnection"));
            }
            else if (e.ErrorDescription == "Seven Update Server Error")
            {
                Program.RemoveShieldFromButton(btnAction);
                SetUI(UILayout.ErrorOccurred, Program.RM.GetString("CouldNotConnect"));
            }
            else
                SetUI(UILayout.ErrorOccurred, e.ErrorDescription);
        }

        void Client_DownloadProgressChangedEventHandler(object sender, Client.DownloadProgressChangedEventArgs e)
        {
            try
            {
                lblSubStatus.Text = Program.RM.GetString("DownloadingUpdates") + " (" +
                    Shared.ConvertFileSize(e.BytesTotal) + ", " + (e.BytesTransferred * 100 / e.BytesTotal).ToString("F0") + " % " + Program.RM.GetString("Complete") + ")";

                Program.trayIcon.Text = lblSubStatus.Text;

            }
            catch (NullReferenceException)
            {
            }
        }

        void Client_DownloadDoneEventHandler(object sender, Client.DownloadDoneEventArgs e)
        {
            if (e.ErrorOccurred)
            {
                SetUI(UILayout.ErrorOccurred);
            }
            else
            {
                SetUI(UILayout.DownloadCompleted);
                btnAction.Visible = false;

                if (lblSelectedUpdates.Text == Program.RM.GetString("NoUpdatesSelected"))
                    btnAction.Visible = false;
            }
        }

        internal void Search_SearchDoneEventHandler(object sender, Search.SearchDoneEventArgs e)
        {
            if (e.Applications.Count > 0)
            {
                SetUI(UILayout.UpdatesFound);
                int importantCount = 0;
                int recommendedCount = 0;
                int optionalCount = 0;
                #region GUI Updating
                lblSelectedUpdates.Text = Program.RM.GetString("NoUpdatesSelected");
                for (int x = 0; x < e.Applications.Count; x++)
                {
                    for (int y = 0; y < e.Applications[x].Updates.Count; y++)
                    {
                        switch (e.Applications[x].Updates[y].Importance)
                        {
                            case Importance.Important:
                                importantCount++;
                                break;
                            case Importance.Locale:
                            case Importance.Optional:
                                optionalCount++; break;
                            case Importance.Recommended:
                                recommendedCount++;
                                break;

                        }
                    }
                }

                if (importantCount > 0 || recommendedCount > 0)
                {
                    SetLabel(true);
                    if (UpdateSettings.Settings.Recommended)
                        importantCount += recommendedCount;
                    else
                        optionalCount += recommendedCount;



                    if (importantCount == 1)
                        lblViewImportantUpdates.Text = importantCount + " " + Program.RM.GetString("ImportantUpdateAvaliable");
                    else
                        lblViewImportantUpdates.Text = importantCount + " " + Program.RM.GetString("ImportantUpdatesAvaliable");


                    if (optionalCount > 0)
                    {

                        if (optionalCount == 1)
                            lblViewOptionalUpdates.Text = optionalCount + " " + Program.RM.GetString("OptionalUpdateAvaliable");
                        else
                            lblViewOptionalUpdates.Text = optionalCount + " " + Program.RM.GetString("OptionalUpdatesAvaliable");

                        lblViewOptionalUpdates.Visible = true;


                    }
                    else
                    {
                        lblViewOptionalUpdates.Visible = false;
                    }
                }
                else
                    if (optionalCount > 0)
                    {

                        if (optionalCount == 1)
                        {
                            lblViewOptionalUpdates.Text = optionalCount + " " + Program.RM.GetString("OptionalUpdateAvaliable");
                        }
                        else
                        {
                            lblViewOptionalUpdates.Text = optionalCount + " " + Program.RM.GetString("OptionalUpdatesAvaliable");
                        }

                        lblViewOptionalUpdates.Visible = true;


                        SetLabel(false);
                        lblViewImportantUpdates.Text = Program.RM.GetString("NoImportantUpdates");

                    }
                //End Code
                #endregion
            }
            else
            {
                SetUI(UILayout.NoUpdates);
            }
        }

        #endregion

        #region AppUpdate Events

        void AppUpdate_UpdateSelectionChangedEventHandler(object sender, SevenUpdate.Pages.AppUpdate.UpdateSelectionChangedEventArgs e)
        {
            btnAction.Visible = true;

            SetLabel(false);
            lblSelectedUpdates.Text = Program.RM.GetString("NoUpdatesSelected");

            btnAction.Visible = false;

            #region GUI Updating

            if (e.ImportantUpdates > 0)
            {
                SetLabel(true);

                btnAction.Visible = true;

                if (e.ImportantUpdates == 1)
                    lblSelectedUpdates.Text = e.ImportantUpdates + " " + Program.RM.GetString("ImportantUpdateSelected");
                else
                    lblSelectedUpdates.Text = e.ImportantUpdates + " " + Program.RM.GetString("ImportantUpdatesSelected");

                if (e.ImportantDownloadSize > 0)
                    lblSelectedUpdates.Text += ", " + Shared.ConvertFileSize(e.ImportantDownloadSize);

                lblSelectedUpdates.Text += Environment.NewLine;

            }
            
            if (e.OptionalUpdates > 0)
            {
                SetLabel(true);

                btnAction.Visible = true;

                if (e.ImportantUpdates == 0)
                    if (e.OptionalUpdates == 1)
                        lblSelectedUpdates.Text = e.OptionalUpdates + " " + Program.RM.GetString("OptionalUpdateSelected");
                    else
                        lblSelectedUpdates.Text = e.OptionalUpdates + " " + Program.RM.GetString("OptionalUpdatesSelected");
                else
                    if (e.OptionalUpdates == 1)
                        lblSelectedUpdates.Text += e.OptionalUpdates + " " + Program.RM.GetString("OptionalUpdateSelected");
                    else
                        lblSelectedUpdates.Text += e.OptionalUpdates + " " + Program.RM.GetString("OptionalUpdatesSelected");

                if (e.OptionalDownloadSize > 0)
                    lblSelectedUpdates.Text += ", " + Shared.ConvertFileSize(e.OptionalDownloadSize);

                lblSelectedUpdates.Text += Environment.NewLine;
            }
            else
            {
                lblViewOptionalUpdates.Visible = false;
            }

            if (e.ImportantDownloadSize == 0 && e.OptionalDownloadSize == 0)
            {
                lblHeading.Text = Program.RM.GetString("InstallUpdatesForPrograms");

                Program.trayIcon.Text = Program.RM.GetString("InstallUpdatesForPrograms");
            }
            else
            {
                lblHeading.Text = Program.RM.GetString("DownloadAndInstallUpdates");

                Program.trayIcon.Text = Program.RM.GetString("DownloadAndInstallUpdates");
            }
            #endregion
            
        }

        #endregion

    }
}
