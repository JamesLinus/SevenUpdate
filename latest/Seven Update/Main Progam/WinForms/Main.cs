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
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using SevenUpdate.Properties;
using SevenUpdate.Pages;
using SevenUpdate.Controls;
using SevenUpdate.WCF;
namespace SevenUpdate.WinForms
{
    public partial class Main : Form
    {
        #region global vars

        /// <summary>
        /// Indicates if an Auto Search was performed
        /// </summary>
        internal static bool AutoCheck { get; set; }

        /// <summary>
        /// Specifies if Seven Update is allowed to check for updates
        /// </summary>
        static bool CanCheckForUpdates { get; set; }

        /// <summary>
        /// If true, Seven Update is currently installing updates.
        /// </summary>
        static bool InstallInProgress { get; set; }

        /// <summary>
        /// Displays Application Updates and allows the user to pick which updates to install.
        /// </summary>
        AppUpdate PageAppUpdate;

        /// <summary>
        /// Displays configuration options for Seven Update
        /// </summary>
        Options PageOptions;

        /// <summary>
        /// Displays hidden updates and allows the user to unhide them
        /// </summary>
        RestoreHidden PageRestoreHidden;

        /// <summary>
        /// Displays a list of updates installed by Seven Update
        /// </summary>
        UpdateHistory PageUpdateHistory;

        /// <summary>
        /// Indicates if updates have been found
        /// </summary>
        static bool UpdatesFound { get; set; }

        #endregion

        #region Enums

        /// <summary>
        /// Pages avaliable for use in Seven Update
        /// </summary>
        enum Pages
        {
            /// <summary>
            /// Displays Application Updates and allows the user to pick which updates to install.
            /// </summary>
            ApplicationUpdates,

            /// <summary>
            /// Displays configuration options for Seven Update
            /// </summary>
            Options,

            /// <summary>
            /// Displays hidden updates and allows the user to unhide them
            /// </summary>
            RestoreHiddenUpdates,

            /// <summary>
            /// Displays a list of updates installed by Seven Update
            /// </summary>
            UpdateHistory,
        }

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

        #region Constructors

        public Main()
        {
            this.Font = SystemFonts.MessageBoxFont;

            InitializeComponent();

            LoadSettings();

            if (AutoCheck)
            {
                CheckForUpdates();
            }
        }

        #endregion

        #region UI Events

        #region Buttons

        void btnAction_Click(object sender, EventArgs e)
        {
            if (infoBar.btnAction.Text == Program.RM.GetString("InstallUpdates"))
            {
                DownloadInstallUpdates();
            }
            else if (infoBar.btnAction.Text == Program.RM.GetString("CancelUpdates"))
            {
                //Cancel installation of updates
                Client.AbortInstall();
                SetUI(UILayout.Canceled);
                infoBar.SetUI(InfoBar.UILayout.Canceled);
                return;
            }
            else if (infoBar.btnAction.Text == Program.RM.GetString("TryAgain"))
            { CheckForUpdates(); }
            else if (infoBar.btnAction.Text == Program.RM.GetString("RestartNow"))
            {
                Process.Start(Environment.GetFolderPath(Environment.SpecialFolder.System) + "\\shutdown.exe", "-r -t 00");
                System.Windows.Forms.Application.Exit();
            }
        }

        #endregion

        #region Form

        void Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            Settings.Default.windowWidth = this.Width;

            Settings.Default.windowHeight = this.Height;

            Settings.Default.Save();

            if (InstallInProgress || UpdatesFound)
            {
                e.Cancel = true;

                Hide();

                this.ShowInTaskbar = false;

                Program.trayIcon.Visible = true;
            }
        }

        void Main_SizeChanged(object sender, EventArgs e)
        {
            if (InstallInProgress)
            {
                if (WindowState == FormWindowState.Minimized)
                {
                    ShowInTaskbar = false;
                    Program.trayIcon.Visible = true;
                }
                if (WindowState == FormWindowState.Normal)
                {
                    ShowInTaskbar = true;
                    Program.trayIcon.Visible = false;
                }
            }
        }

        void Main_Load(object sender, EventArgs e)
        {
            if (!Settings.Default.lastUpdateCheck.Contains(DateTime.Now.ToShortDateString()))
                CheckForUpdates();
        }

        void Options_SettingsSavedEventHandler(object sender, Options.SettingsSavedEventArgs e)
        {
            switch (e.UpdateSettings.AutoOption)
            {
                case AutoUpdateOption.Download: lblAutoOption.Text = Program.RM.GetString("DownloadUpdatesOnly"); break;
                case AutoUpdateOption.Never: lblAutoOption.Text = Program.RM.GetString("NeverCheckUpdates"); break;
                case AutoUpdateOption.Notify: lblAutoOption.Text = Program.RM.GetString("CheckUpdatesOnly"); break;
                case AutoUpdateOption.Install: lblAutoOption.Text = Program.RM.GetString("InstallUpdatesAutomatically"); break;

            }
        }

        void RestoreHidden_RestoredHiddenUpdateEventHandler(object sender, EventArgs e)
        {
            CheckForUpdates();
        }

        #endregion

        #region InfoBar

        void infoBar_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawRectangle(Pens.LightGray, e.Graphics.VisibleClipBounds.Left, e.Graphics.VisibleClipBounds.Top, e.Graphics.VisibleClipBounds.Width - 1, e.Graphics.VisibleClipBounds.Height - 1);
            base.OnPaint(e);
        }

        #endregion

        #region Labels

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

        void lblAbout_Click(object sender, EventArgs e)
        {
            About about = new About();
            about.ShowDialog();
        }

        void lblChangeSettings_Click(object sender, EventArgs e)
        {
            LoadPage(Pages.Options);
        }

        void lblCheckForUpdates_Click(object sender, EventArgs e)
        {
            CheckForUpdates();
        }

        void lblRestoreHiddenUpdates_Click(object sender, EventArgs e)
        {
            LoadPage(Pages.RestoreHiddenUpdates);
        }

        void lblUpdateHistory_Click(object sender, EventArgs e)
        {
            LoadPage(Pages.UpdateHistory);
        }

        void lblViewImportantUpdates_Click(object sender, EventArgs e)
        {
            if (infoBar.lblViewOptionalUpdates.Cursor == Cursors.Hand)
            {
                LoadPage(Pages.ApplicationUpdates);
            }
        }

        void lblViewOptionalUpdates_Click(object sender, EventArgs e)
        {
            LoadPage(Pages.ApplicationUpdates);
        }

        #endregion

        #region Notification Icon

        void checkForUpdatesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!UpdatesFound && !InstallInProgress)
                CheckForUpdates();
        }

        void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (InstallInProgress)
            {
                DialogResult dr;
                dr = MessageBox.Show(Program.RM.GetString("WarnOnExit") + Environment.NewLine + Program.RM.GetString("ExitConfirm"), Program.RM.GetString("ExitConfirm"), MessageBoxButtons.YesNoCancel);
                if (dr == DialogResult.Yes)
                    System.Windows.Forms.Application.Exit();
            }
            else
                System.Windows.Forms.Application.Exit();
        }

        void trayIcon_BalloonTipClicked(object sender, EventArgs e)
        {
            Settings.Default.infoPopUp = false;
            Settings.Default.Save();
            this.Show();
            this.ShowInTaskbar = true;
            Program.trayIcon.Visible = false;
            if (Program.trayIcon.Text.Contains(Program.RM.GetString("DownloadAndInstallUpdates")))
            {
                PageAppUpdate.Visible = true;
                PageAppUpdate.BringToFront();
            }
        }

        void trayIcon_DoubleClick(object sender, EventArgs e)
        {
            this.Show();
            this.ShowInTaskbar = true;
            Program.trayIcon.Visible = false;
        }

        #endregion

        #endregion

        #region Update Event Methods

        /// <summary>
        /// Changes the UI when searching is complete and starts download when applicable.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Search_SearchDone(object sender, SevenUpdate.Search.SearchDoneEventArgs e)
        {
            if (e.Applications.Count < 1)
            {
                SetUI(UILayout.NoUpdates);
            }
            else
            {
                Program.Applications = e.Applications;
                SetUI(UILayout.UpdatesFound);

            }
        }
        
        void Client_ErrorOccurredEventHandler(object sender, Client.ErrorOccurredEventArgs e)
        {
            try
            {
                Process[] process = Process.GetProcessesByName("Seven Update Admin");
                for (int x = 0; x < process.Length; x++)
                {
                    process[x].Kill();
                }
            }
            catch (Exception) { }
            Client.AbortInstall();
            SetUI(UILayout.ErrorOccurred);
            TextWriter tw = new StreamWriter(Shared.userStore + "error.log");
            tw.WriteLine(DateTime.Now.ToString() + ": " + e.ErrorDescription);
            tw.Close();

        }

        void Client_InstallDoneEventHandler(object sender, Client.InstallDoneEventArgs e)
        {
            InstallInProgress = false;
            UpdatesFound = false;
            if (!e.ErrorOccurred)
            {
                Settings.Default.lastInstall = DateTime.Now.ToShortDateString() + " " + Program.RM.GetString("At") + " " + DateTime.Now.ToShortTimeString();
                lblUpdatesInstalled.Text = Program.RM.GetString("TodayAt") + " " + DateTime.Now.ToShortTimeString();
                // if a reboot is needed lets say it
                if (Shared.RebootNeeded)
                {
                    CanCheckForUpdates = false;
                }
                else
                {
                    CanCheckForUpdates = true;
                }

            }
            else
                SetUI(UILayout.ErrorOccurred);
        }

        void Client_DownloadDoneEventHandler(object sender, Client.DownloadDoneEventArgs e)
        {
            SetUI(UILayout.DownloadCompleted);
        }
        #endregion

        #region Update Methods

        /// <summary>
        /// Checks for updates
        /// </summary>
        void CheckForUpdates()
        {
            if (Process.GetProcessesByName("Seven Update Admin").Length < 1)
            {
                if (CanCheckForUpdates)
                {
                    if (Shared.RebootNeeded == false)
                    {
                        try
                        {
                            PageAppUpdate.Dispose();
                            PageAppUpdate = null;
                        }
                        catch (Exception) { }
                        infoBar.SetUI(InfoBar.UILayout.CheckingForUpdates);
                        SetUI(UILayout.CheckingForUpdates);
                        Settings.Default.lastUpdateCheck = DateTime.Now.ToShortDateString() + " " + Program.RM.GetString("At") + " " + DateTime.Now.ToShortTimeString();
                        Search.SearchForUpdatesAync(UpdateSettings.Applications);
                    }
                    else
                    {
                        SetUI(UILayout.RebootNeeded);
                        infoBar.SetUI(InfoBar.UILayout.RebootNeeded);
                    }
                }
                else
                    MessageBox.Show(Program.RM.GetString("RebootNeededFirst"), Program.RM.GetString("SevenUpdate"), MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
                MessageBox.Show(Program.RM.GetString("AlreadyUpdating"), Program.RM.GetString("SevenUpdate"), MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// Downloads updates
        /// </summary>
        /// <param name="auto">Specifies if Seven Update is doing automatic uodates</param>
        /// <param name="resume">Specifies if resuming downloads</param>
        internal void DownloadInstallUpdates()
        {
            if (Program.Applications.Count > 0)
            {
                LicenseAgreement sla = new LicenseAgreement();
                if (sla.LoadLicenses() == DialogResult.Cancel)
                {
                    infoBar.SetUI(InfoBar.UILayout.Canceled);
                    SetUI(UILayout.Canceled);
                    return;
                }
                
                if (Client.Install())
                {
                    if (PageAppUpdate != null)
                    {
                        PageAppUpdate.RemoveUnSelectedUpdates();
                        PageAppUpdate.Dispose();
                        PageAppUpdate = null;
                    }
                    Program.RemoveShieldFromButton(infoBar.btnAction);
                    if (infoBar.lblHeading.Text == Program.RM.GetString("DownloadAndInstallUpdates"))
                    {
                        infoBar.SetUI(InfoBar.UILayout.Downloading);
                        SetUI(UILayout.Downloading);
                    }
                    else
                    {
                        infoBar.SetUI(InfoBar.UILayout.Installing);
                        SetUI(UILayout.Installing);
                    }
                }
                else
                {
                    infoBar.SetUI(InfoBar.UILayout.Canceled);
                    SetUI(UILayout.Canceled);
                }
            }
            else
            {
                infoBar.SetUI(InfoBar.UILayout.Canceled);
                SetUI(UILayout.Canceled);
            }
        }

        #endregion

        #region UI Methods

        /// <summary>
        /// Load a page in Seven Update to display
        /// </summary>
        /// <param name="page">The page to display</param>
        void LoadPage(Pages page)
        {
            switch (page)
            {
                case Pages.Options:
                    if (PageOptions != null)
                    {
                        try
                        {
                            PageOptions.Dispose();
                        }
                        catch (Exception) { }
                    }
                    PageOptions = new Options();
                    PageOptions.AutoScroll = true;
                    PageOptions.BackColor = System.Drawing.Color.White;
                    PageOptions.Dock = System.Windows.Forms.DockStyle.Fill;
                    PageOptions.Location = new System.Drawing.Point(0, 0);
                    PageOptions.Name = "options";
                    PageOptions.TabIndex = 4;
                    Controls.Add(PageOptions);
                    PageOptions.Visible = true;
                    PageOptions.BringToFront();

                    break;
                case Pages.UpdateHistory:
                    if (PageUpdateHistory != null)
                    {
                        try
                        {
                            PageUpdateHistory.Dispose();
                        }
                        catch (Exception) { }
                    }
                    PageUpdateHistory = new UpdateHistory();
                    PageUpdateHistory.BackColor = System.Drawing.Color.White;
                    PageUpdateHistory.Dock = System.Windows.Forms.DockStyle.Fill;
                    PageUpdateHistory.Location = new System.Drawing.Point(0, 0);
                    PageUpdateHistory.Name = "updateHistory";
                    PageUpdateHistory.TabIndex = 3;
                    Controls.Add(PageUpdateHistory);
                    PageUpdateHistory.Visible = true;
                    PageUpdateHistory.BringToFront();
                    break;

                case Pages.ApplicationUpdates:
                    if (PageAppUpdate == null)
                    {
                        PageAppUpdate = new AppUpdate();
                        PageAppUpdate.BackColor = System.Drawing.Color.White;
                        PageAppUpdate.Dock = System.Windows.Forms.DockStyle.Fill;
                        PageAppUpdate.Location = new System.Drawing.Point(0, 0);
                        PageAppUpdate.Name = "appUpdate";
                        PageAppUpdate.TabIndex = 1;
                        Controls.Add(PageAppUpdate);

                    }
                    PageAppUpdate.Visible = true;
                    PageAppUpdate.BringToFront();
                    break;
                case Pages.RestoreHiddenUpdates:
                    if (PageRestoreHidden != null)
                    {
                        try
                        {
                            PageRestoreHidden.Dispose();
                        }
                        catch (Exception) { }
                    }
                    PageRestoreHidden = new RestoreHidden();
                    PageRestoreHidden.BackColor = System.Drawing.Color.White;
                    PageRestoreHidden.Dock = System.Windows.Forms.DockStyle.Fill;
                    PageRestoreHidden.Location = new System.Drawing.Point(0, 0);
                    PageRestoreHidden.Name = "restoreHidden";
                    Controls.Add(PageRestoreHidden);
                    PageRestoreHidden.Visible = true;
                    PageRestoreHidden.BringToFront();
                    break;


            }
            //events

        }

        /// <summary>
        /// Loads the application settings and initializes event handlers for the pages
        /// </summary>
        void LoadSettings()
        {
            this.Width = Settings.Default.windowWidth;

            this.Height = Settings.Default.windowHeight;

            switch (UpdateSettings.Settings.AutoOption)
            {
                case AutoUpdateOption.Download: lblAutoOption.Text = Program.RM.GetString("DownloadUpdatesOnly"); break;
                case AutoUpdateOption.Never: lblAutoOption.Text = Program.RM.GetString("NeverCheckUpdates"); break;
                case AutoUpdateOption.Notify: lblAutoOption.Text = Program.RM.GetString("CheckUpdatesOnly"); break;
                case AutoUpdateOption.Install: lblAutoOption.Text = Program.RM.GetString("InstallUpdatesAutomatically"); break;

            }

            if (Settings.Default.lastUpdateCheck.Contains(DateTime.Now.ToShortDateString()))
                lblRecentCheck.Text = Program.RM.GetString("TodayAt") + " " + Settings.Default.lastUpdateCheck.Replace(DateTime.Now.ToShortDateString() + " " + Program.RM.GetString("At") + " ", null);
            else
                lblRecentCheck.Text = Settings.Default.lastUpdateCheck;
            
            if (Settings.Default.lastInstall.Contains(DateTime.Now.ToShortDateString()))
                lblUpdatesInstalled.Text = Program.RM.GetString("TodayAt") + " " + Settings.Default.lastInstall.Replace(DateTime.Now.ToShortDateString() + " " + Program.RM.GetString("At") + " ", null);
            else
                lblUpdatesInstalled.Text = Settings.Default.lastInstall;
            
            if (Shared.RebootNeeded)
            {
                SetUI(UILayout.RebootNeeded);

                infoBar.SetUI(InfoBar.UILayout.RebootNeeded);
            }
            else
            {

                infoBar.SetUI(InfoBar.UILayout.NoUpdates);

                SetUI(UILayout.NoUpdates);
            }

            lblAppName.Font = new Font(SystemFonts.MessageBoxFont.Name, 11.25F);

            #region Event Handler Declarations

            infoBar.btnAction.Click += new EventHandler(btnAction_Click);

            infoBar.lblViewImportantUpdates.Click += new EventHandler(lblViewImportantUpdates_Click);

            infoBar.lblViewOptionalUpdates.Click += new EventHandler(lblViewOptionalUpdates_Click);

            infoBar.Paint += new PaintEventHandler(infoBar_Paint);

            Program.trayIcon.BalloonTipClicked += new EventHandler(trayIcon_BalloonTipClicked);

            Program.trayIcon.DoubleClick += new EventHandler(trayIcon_DoubleClick);

            Search.SearchDoneEventHandler += new EventHandler<Search.SearchDoneEventArgs>(Search_SearchDone);

            Client.DownloadDoneEventHandler += new EventHandler<Client.DownloadDoneEventArgs>(Client_DownloadDoneEventHandler);

            Client.InstallDoneEventHandler += new EventHandler<Client.InstallDoneEventArgs>(Client_InstallDoneEventHandler);

            Client.ErrorOccurredEventHandler += new EventHandler<Client.ErrorOccurredEventArgs>(Client_ErrorOccurredEventHandler);

            Options.SettingsSavedEventHandler +=new EventHandler<Options.SettingsSavedEventArgs>(Options_SettingsSavedEventHandler);

            RestoreHidden.RestoredHiddenUpdateEventHandler += new EventHandler<EventArgs>(RestoreHidden_RestoredHiddenUpdateEventHandler);

            Program.trayIcon.Icon = this.Icon;

            Program.trayIcon.ContextMenuStrip = cmsMenu;
            #endregion
        }

        /// <summary>
        /// Sets the Main Page UI
        /// </summary>
        /// <param name="layout">Type of layout to set</param>
        void SetUI(UILayout layout)
        {
            switch (layout)
            {
                case UILayout.UpdatesFound:
                case UILayout.NoUpdates:
                    #region Code
                    InstallInProgress = false;
                    CanCheckForUpdates = true;
                    checkForUpdatesToolStripMenuItem.Enabled = true;
                    if (layout == UILayout.UpdatesFound)
                        UpdatesFound = true;
                    else
                        UpdatesFound = false;
                    #endregion
                    break;
                case UILayout.CheckingForUpdates:
                    #region Code
                    InstallInProgress = false;
                    Program.trayIcon.Text = Program.RM.GetString("DownloadAndInstallUpdates");
                    if (ShowInTaskbar == false)
                        Program.trayIcon.ShowBalloonTip(5000, Program.RM.GetString("DownloadAndInstallUpdates"), Program.RM.GetString("UpdatesAvailable"), ToolTipIcon.Info);
                    CanCheckForUpdates = false;
                    checkForUpdatesToolStripMenuItem.Enabled = false;
                    lblRecentCheck.Text = Program.RM.GetString("TodayAt") + " " + DateTime.Now.ToShortTimeString();

                    #endregion
                    break;
                case UILayout.Downloading:
                    #region Code
                    InstallInProgress = false;
                    CanCheckForUpdates = false;
                    checkForUpdatesToolStripMenuItem.Enabled = false;
                    Program.trayIcon.Text = Program.RM.GetString("DownloadingUpdatesBackground");
                    if (ShowInTaskbar == false)
                        Program.trayIcon.ShowBalloonTip(5000, Program.RM.GetString("DownloadingUpdates"), Program.RM.GetString("DownloadingUpdatesBackground"), ToolTipIcon.Info);
                    #endregion
                    break;
                case UILayout.DownloadCompleted:
                    #region Code
                    InstallInProgress = false;
                    CanCheckForUpdates = true;
                    checkForUpdatesToolStripMenuItem.Enabled = true;
                    Program.trayIcon.Text = Program.RM.GetString("UpdatesDownloaded");
                    if (ShowInTaskbar == false)
                        Program.trayIcon.ShowBalloonTip(5000, Program.RM.GetString("UpdatesDownloaded"), Program.RM.GetString("FinishedDownloading"), ToolTipIcon.Info);
                    #endregion
                    break;

                case UILayout.Installing:
                    #region Code
                    InstallInProgress = true;
                    CanCheckForUpdates = false;
                    checkForUpdatesToolStripMenuItem.Enabled = false;
                    Program.trayIcon.Text = Program.RM.GetString("PreparingInstall");
                    if (ShowInTaskbar == false)
                        Program.trayIcon.ShowBalloonTip(5000, Program.RM.GetString("InstallingUpdates"), Program.RM.GetString("InstallingUpdatesBackground"), ToolTipIcon.Info);
                    #endregion
                    break;

                case UILayout.InstallationCompleted:
                    #region Code
                    InstallInProgress = false;
                    UpdatesFound = false;
                    Settings.Default.lastInstall = DateTime.Now.ToShortDateString() + " " + Program.RM.GetString("At") + " " + DateTime.Now.ToShortTimeString();
                    lblUpdatesInstalled.Text = Program.RM.GetString("TodayAt") + " " + DateTime.Now.ToShortTimeString();
                    CanCheckForUpdates = true;
                    checkForUpdatesToolStripMenuItem.Enabled = true;
                    Settings.Default.lastInstall = DateTime.Now.ToShortDateString() + " " + Program.RM.GetString("At") + " " + DateTime.Now.ToShortTimeString();
                    Program.trayIcon.Text = Program.RM.GetString("ProgramsUpToDate");
                    if (ShowInTaskbar == false)
                        Program.trayIcon.ShowBalloonTip(5000, Program.RM.GetString("UpdatesComplete"), Program.RM.GetString("CompletedInstallingUpdates"), ToolTipIcon.Info);
                    #endregion
                    break;

                case UILayout.ErrorOccurred:
                    #region Code
                    CanCheckForUpdates = true;
                    InstallInProgress = false;
                    UpdatesFound = false;
                    checkForUpdatesToolStripMenuItem.Enabled = true;

                    if (ShowInTaskbar == false)
                        Program.trayIcon.ShowBalloonTip(5000, Program.RM.GetString("ErrorOccurred"), Program.RM.GetString("ErrorOccurred"), ToolTipIcon.Info);
                    #endregion
                    break;

                case UILayout.RebootNeeded:
                    #region Code
                    CanCheckForUpdates = false;
                    checkForUpdatesToolStripMenuItem.Enabled = false;
                    Program.trayIcon.Text = Program.RM.GetString("RebootNeeded");
                    if (ShowInTaskbar == false)
                        Program.trayIcon.ShowBalloonTip(5000, Program.RM.GetString("UpdatesComplete"), Program.RM.GetString("RebootNeeded"), ToolTipIcon.Info);
                    #endregion
                    break;

                case UILayout.Canceled:
                    #region Code
                    try
                    {
                        Process[] process = Process.GetProcessesByName("Seven Update Admin");
                        for (int x = 0; x < process.Length; x++)
                        {
                            process[x].Kill();
                        }
                    }
                    catch (Exception) { }
                    Program.trayIcon.Text = Program.RM.GetString("UpdatesCanceled");
                    CanCheckForUpdates = true;
                    UpdatesFound = false;
                    InstallInProgress = false;
                    checkForUpdatesToolStripMenuItem.Enabled = true;
                    #endregion
                    break;
            }
        }

        #endregion

    }
}