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
using System.Drawing;
using System.Windows.Forms;
using SevenUpdate.SDK.Properties;


namespace SevenUpdate.SDK
{
    public partial class Main : Form
    {
        #region Global Vars
        

        /// <summary>
        /// The current panel index
        /// </summary>
        int curPanel;

        /// <summary>
        /// Indicates if currently editing an update
        /// </summary>
        bool editUpdate; 

        /// <summary>
        /// Indicates if the app in a x64 bit application
        /// </summary>
        internal static bool Is64Bit { get; set; }

        /// <summary>
        /// Indicates if it's a new SUI file
        /// </summary>
        bool newSUI;
        
        /// <summary>
        /// Gets or Sets the current sui files that has been opened
        /// </summary>
        string suiFile { get; set; }

        #endregion

        #region Page Declarations
        Files pageFiles;

        AppInfo pageAppInfo;

        UpdateInfo pageUpdateInfo;

        Registry pageRegistry;

        Shortcuts pageShortcuts;

        UpdateMenu pageUpdateMenu;

        SUAInfo pageSUA;
        #endregion
        
        public Main()
        {
            this.Font = SystemFonts.MessageBoxFont;

            InitializeComponent();

            LoadUI();
        }

        /// <summary>
        /// Loads the Form and loads a SUI file
        /// </summary>
        /// <param name="file">The fullpath to the SUI</param>
        public Main(string file)
        {
            this.Font = SystemFonts.MessageBoxFont;

            InitializeComponent();

            LoadUI();

            LoadPages();

            SUSDK.LoadSUI(file);

            pageAppInfo.LoadInfo();

            curPanel = 1;

            SetPanel();
        }

        #region Methods
                
        /// <summary>
        /// Clears the UI of the pages
        /// </summary>
        void ClearPages()
        {
            pageUpdateInfo.ClearUI();

            pageFiles.ClearUI(true);

            pageRegistry.ClearUI(true);

            pageShortcuts.ClearUI(true);
        }

        /// <summary>
        /// Loads the pages
        /// </summary>
        void LoadPages()
        {
            if (pageAppInfo != null)
                try
                {
                    pageAppInfo.Dispose();
                }
                catch (Exception) { }

            if (pageFiles != null)
                try
                {
                    pageFiles.Dispose();
                }
                catch (Exception) { }

            if (pageRegistry != null)
                try
                {
                    pageRegistry.Dispose();
                }
                catch (Exception) { }

            if (pageShortcuts != null)
                try
                {
                    pageShortcuts.Dispose();
                }
                catch (Exception) { }

            if (pageUpdateInfo != null)
                try
                {
                    pageUpdateInfo.Dispose();
                }
                catch (Exception) { }

            if (pageUpdateMenu != null)
                try
                {
                    pageUpdateMenu.Dispose();
                }
                catch (Exception) { }

            pageAppInfo = new AppInfo();
            pageAppInfo.BackColor = System.Drawing.Color.White;
            pageAppInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            pageAppInfo.Location = new System.Drawing.Point(0, 56);
            pageAppInfo.Name = "pageAppInfo";
            Controls.Add(pageAppInfo);
            pageAppInfo.Visible = false;
            pageAppInfo.SendToBack();

            pageFiles = new Files();
            pageFiles.BackColor = System.Drawing.Color.White;
            pageFiles.Dock = System.Windows.Forms.DockStyle.Fill;
            pageFiles.Location = new System.Drawing.Point(0, 56);
            pageFiles.Name = "pageFiles";
            Controls.Add(pageFiles);
            pageFiles.Visible = false;
            pageFiles.SendToBack();

            pageRegistry = new Registry();
            pageRegistry.BackColor = System.Drawing.Color.White;
            pageRegistry.Dock = System.Windows.Forms.DockStyle.Fill;
            pageRegistry.Location = new System.Drawing.Point(0, 56);
            pageRegistry.Name = "pageRegistry";
            Controls.Add(pageRegistry);
            pageRegistry.Visible = false;
            pageRegistry.SendToBack();

            pageShortcuts = new Shortcuts();
            pageShortcuts.BackColor = System.Drawing.Color.White;
            pageShortcuts.Dock = System.Windows.Forms.DockStyle.Fill;
            pageShortcuts.Location = new System.Drawing.Point(0, 56);
            pageShortcuts.Name = "pageShortcuts";
            Controls.Add(pageShortcuts);
            pageShortcuts.Visible = false;
            pageShortcuts.SendToBack();

            pageUpdateInfo = new UpdateInfo();
            pageUpdateInfo.BackColor = System.Drawing.Color.White;
            pageUpdateInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            pageUpdateInfo.Location = new System.Drawing.Point(0, 56);
            pageUpdateInfo.Name = "pageUpdInfo";
            Controls.Add(pageUpdateInfo);
            pageUpdateInfo.Visible = false;
            pageUpdateInfo.SendToBack();

            pageUpdateMenu = new UpdateMenu();
            pageUpdateMenu.BackColor = System.Drawing.Color.White;
            pageUpdateMenu.Dock = System.Windows.Forms.DockStyle.Fill;
            pageUpdateMenu.Location = new System.Drawing.Point(0, 56);
            pageUpdateMenu.Name = "pageUpdateInstallMenu";
            Controls.Add(pageUpdateMenu);
            pageUpdateMenu.Visible = false;
            pageUpdateMenu.SendToBack();

            //Event Handlers
            pageUpdateMenu.clNewUpdate.Click -= clNewUpdate_Click;

            pageUpdateMenu.clEditUpdate.Click -= clEditUpdate_Click;

            pageUpdateMenu.clSave.Click -= clSave_Click;

            pageUpdateMenu.clNewUpdate.Click += new EventHandler(clNewUpdate_Click);

            pageUpdateMenu.clEditUpdate.Click += new EventHandler(clEditUpdate_Click);

            pageUpdateMenu.clSave.Click += new EventHandler(clSave_Click);

        }

        /// <summary>
        /// Loads UI
        /// </summary>
        private void LoadUI()
        {
            lblHeading.Font = new Font(SystemFonts.MessageBoxFont.Name, 11.25F);

            lblStep1.Font = new Font(SystemFonts.MessageBoxFont.Name, 11.25F);

            lblStep2.Font = new Font(SystemFonts.MessageBoxFont.Name, 11.25F);

            lblStep3.Font = new Font(SystemFonts.MessageBoxFont.Name, 11.25F);

            if (Environment.OSVersion.Version.Major >= 6 && this.Size.Height == 500)
            {
                this.Size = new Size(this.Width, 507);

                this.MinimumSize = new Size(700, 507);
            }

            Shared.SerializationErrorEventHandler += new EventHandler<Shared.SerializationErrorEventArgs>(Shared_SerializationErrorEventHandler);
        }
        
        /// <summary>
        /// Save an update of a program in a sui
        /// </summary>
        void SaveUpdate()
        {
            Update update = new Update();

            update.LicenseUrl = pageUpdateInfo.txtLicenseURL.Text;

            if (pageUpdateInfo.txtDownloadURL.Text.Substring(pageUpdateInfo.txtDownloadURL.Text.Length - 1, 1) == "\\")
                pageUpdateInfo.txtDownloadURL.Text = pageUpdateInfo.txtDownloadURL.Text.Substring(0, pageUpdateInfo.txtDownloadURL.Text.Length - 1);

            Uri url = new Uri(pageUpdateInfo.txtDownloadURL.Text);
            update.DownloadDirectory = url.AbsoluteUri;

            update.ReleaseDate = pageUpdateInfo.dtReleaseDate.Text;

            Collection<LocaleString> updateTitle = new Collection<LocaleString>();
            LocaleString ls = new LocaleString();
            ls.Value = pageUpdateInfo.txtUpdateTitle.Text;
            ls.lang = "en";
            updateTitle.Add(ls);
            update.Title = updateTitle;
            update.InfoUrl = pageUpdateInfo.txtInfoURL.Text;
            Collection<LocaleString> updInfo = new Collection<LocaleString>();
            ls.Value =  pageUpdateInfo.txtUpdateInfo.Text;

            updInfo.Add(ls);
            
            update.Description = updInfo;

            switch (pageUpdateInfo.cbUpdateType.SelectedIndex)
            {
                case 0: update.Importance = Importance.Important; break;

                case 1: update.Importance = Importance.Recommended; break;

                case 2: update.Importance = Importance.Optional; break;

                case 3: update.Importance = Importance.Locale; break;
            }
            update.Files = pageFiles.UpdateFiles;

            update.RegistryItems = pageRegistry.UpdateRegistry;

            update.Shortcuts = pageShortcuts.UpdateShortcuts;

            if (SUSDK.application.Updates == null)
                SUSDK.application.Updates = new Collection<Update>();
            SUSDK.application.Updates.Add(update);

            if (editUpdate && pageUpdateMenu.index > -1)
                SUSDK.application.Updates.RemoveAt(pageUpdateMenu.index);
        }

        /// <summary>
        /// Sets the Top Panel Menu depending on if your editing
        /// </summary>
        /// <param name="editing">Indictates if your editing a SUI</param>
        void SetMenu(bool editing)
        {
            if (editing)
            {
                lblStep2.Cursor = Cursors.Hand;

                lblStep3.Cursor = Cursors.Hand;

                lblStepA.Cursor = Cursors.Hand;

                lblStepB.Cursor = Cursors.Hand;

                lblStepC.Cursor = Cursors.Hand;
            }
            else
            {
                lblStep2.Cursor = Cursors.Arrow;

                lblStep3.Cursor = Cursors.Arrow;

                lblStepA.Cursor = Cursors.Arrow;

                lblStepB.Cursor = Cursors.Arrow;

                lblStepC.Cursor = Cursors.Arrow;
            }
        }
           
        /// <summary>
        /// Sets the Top Panel UI depending on the current panel
        /// </summary>
        void SetPanel()
        {
            lblStep1.ForeColor = SystemColors.GrayText;
            lblStep2.ForeColor = SystemColors.GrayText;
            lblStep3.ForeColor = SystemColors.GrayText;
            lblStepA.ForeColor = SystemColors.GrayText;
            lblStepB.ForeColor = SystemColors.GrayText;
            lblStepC.ForeColor = SystemColors.GrayText;

            btnCancel.Visible = true;
            btnNext.Visible = true;

            switch (curPanel)
            {
                case 0: 
                    panel.Visible = false; 
                    pnlButtonSection.Visible = false; 
                    lblAbout.Visible = true;
                    pageAppInfo.Visible = false;
                    pageFiles.Visible = false;
                    pageRegistry.Visible = false;
                    pageShortcuts.Visible = false;
                    pageUpdateMenu.Visible = false;
                    pageUpdateInfo.Visible = false; 
                    break;

                case 1: 
                    lblStep1.ForeColor = Color.FromArgb(0, 51, 153); 
                    panel.Size = new Size(panel.Size.Width, 37);
                    pageAppInfo.Visible = true; 
                    lblAbout.Visible = false;
                    pageFiles.Visible = true;
                    pageRegistry.Visible = true;
                    pageShortcuts.Visible = true;
                    pageUpdateMenu.Visible = true;
                    pageUpdateInfo.Visible = true; 
                    pageAppInfo.BringToFront();
                    pnlButtonSection.Visible = true;
                    panel.Visible = true;
                    pageAppInfo.txtAppName.Focus(); break;

                case 2:
                    pageAppInfo.SaveInfo();
                    lblStep2.ForeColor = Color.FromArgb(0, 51, 153); 
                    panel.Size = new Size(panel.Size.Width, 56);
                    pageUpdateInfo.Visible = true;
                    pageUpdateInfo.BringToFront();
                    pageUpdateInfo.txtUpdateTitle.Focus();  
                    break;

                case 3:
                    lblStep3.ForeColor = Color.FromArgb(0, 51, 153);
                    panel.Size = new Size(panel.Size.Width, 37);
                    pageUpdateMenu.UpdateUI();
                    pageUpdateMenu.Visible = true;
                    pageUpdateMenu.BringToFront();
                    btnNext.Visible = false;
                    SetMenu(false); 
                    break;

                case 21:
                    UpdateInfo.DownloadDirectory = pageUpdateInfo.txtDownloadURL.Text;
                    lblStepA.ForeColor = Color.FromArgb(0, 51, 153); 
                    panel.Size = new Size(panel.Size.Width, 56);
                    pageFiles.Visible = true; 
                    pageFiles.BringToFront();
                    break;

                case 22: 
                    lblStepB.ForeColor = Color.FromArgb(0, 51, 153); 
                    panel.Size = new Size(panel.Size.Width, 56);
                    pageRegistry.Visible = true; 
                    pageRegistry.BringToFront(); 
                    break;

                case 23: 
                    lblStepC.ForeColor = Color.FromArgb(0, 51, 153); 
                    panel.Size = new Size(panel.Size.Width, 56);
                    pageShortcuts.Visible = true; 
                    pageShortcuts.BringToFront(); 
                    break;
            }
        }

        #endregion

        #region Events

        void Shared_SerializationErrorEventHandler(object sender, Shared.SerializationErrorEventArgs e)
        {
            MessageBox.Show(Program.RM.GetString("CouldNotLoad") + " " + e.File + ": " + e.ErrorMessage);
        }

        #endregion

        #region UI Events

        #region Buttons

        void btnCancel_Click(object sender, EventArgs e)
        {
            if ((curPanel < 4 && curPanel != 2) || (newSUI && SUSDK.application.Updates == null))
            {
                curPanel = 0;
            }
            else
            {
                curPanel = 3;
            }

            SetPanel();
        }

        void btnNext_Click(object sender, EventArgs e)
        {
            if ((pageAppInfo.cbLoc.SelectedIndex > 0 && pageAppInfo.txtValueName.Text.Length < 1) || (pageAppInfo.txtAppDir.Text.Length < 1 || pageAppInfo.txtAppName.Text.Length < 1) || ((pageUpdateInfo.txtUpdateTitle.Text.Length < 1 || pageUpdateInfo.txtDownloadURL.Text.Length < 1) && curPanel == 2))
            {
                MessageBox.Show(Program.RM.GetString("FillRequiredInformation"));
            }
            else
            {
                if (curPanel == 2)
                {
                    curPanel = 20;
                }

                if (curPanel == 1)
                {
                    if (!newSUI)
                        curPanel = 2;
                }

                if (curPanel == 23)
                {
                    SaveUpdate();

                    curPanel = 2;
                }

                curPanel++;

                SetPanel();


            }
        }

        #endregion

        #region Command Links

        void clEditUpdate_Click(object sender, EventArgs e)
        {
            editUpdate = true;

            curPanel = 2;

            SetPanel();

            SetMenu(true);

            ClearPages();

            pageUpdateInfo.txtInfoURL.Text = SUSDK.application.Updates[pageUpdateMenu.index].InfoUrl;
            pageUpdateInfo.txtLicenseURL.Text = SUSDK.application.Updates[pageUpdateMenu.index].LicenseUrl;
            pageUpdateInfo.txtUpdateInfo.Text = SUSDK.application.Updates[pageUpdateMenu.index].Description[0].Value;
            pageUpdateInfo.txtUpdateTitle.Text = SUSDK.application.Updates[pageUpdateMenu.index].Title[0].Value;
            pageUpdateInfo.dtReleaseDate.Text = SUSDK.application.Updates[pageUpdateMenu.index].ReleaseDate;
            pageUpdateInfo.txtDownloadURL.Text = SUSDK.application.Updates[pageUpdateMenu.index].DownloadDirectory;

            switch (SUSDK.application.Updates[pageUpdateMenu.index].Importance)
            {
                case Importance.Important: pageUpdateInfo.cbUpdateType.SelectedIndex = 0; break;

                case Importance.Recommended: pageUpdateInfo.cbUpdateType.SelectedIndex = 1; break;

                case Importance.Optional: pageUpdateInfo.cbUpdateType.SelectedIndex = 2; break;

                case Importance.Locale: pageUpdateInfo.cbUpdateType.SelectedIndex = 3; break;
            }
            pageFiles.LoadFiles(SUSDK.application.Updates[pageUpdateMenu.index].Files);

            pageRegistry.LoadRegistryItems(SUSDK.application.Updates[pageUpdateMenu.index].RegistryItems);

            pageShortcuts.LoadShortcuts(SUSDK.application.Updates[pageUpdateMenu.index].Shortcuts);

        }

        void clNewSUI_Click(object sender, EventArgs e)
        {
            suiFile = null;

            LoadPages();

            newSUI = true;

            if (SUSDK.application.Updates != null)
                SUSDK.application.Updates.Clear();
            else
                SUSDK.application.Updates = new Collection<Update>();

            pageAppInfo.ClearUI();

            ClearPages();

            curPanel = 1;

            SetPanel();
        }
       
        void clNewUpdate_Click(object sender, EventArgs e)
        {
            editUpdate = false;

            curPanel = 2;

            SetPanel();

            SetMenu(true);
            
            ClearPages();
        }
        
        void clOpenSUI_Click(object sender, EventArgs e)
        {
            LoadPages();

            if (dlgOpenFile.ShowDialog() == DialogResult.OK)
            {
                newSUI = false;

                pageAppInfo.ClearUI();

                ClearPages();

                suiFile = dlgOpenFile.FileName;

                SUSDK.LoadSUI(suiFile);

                pageAppInfo.LoadInfo();

                curPanel = 1;

                SetPanel();
            }
        }

        void clSave_Click(object sender, EventArgs e)
        {
            pageAppInfo.SaveInfo();

            if (suiFile != null)
                dlgSaveFile.FileName = suiFile;

            else
                dlgSaveFile.FileName = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) +"\\"+ SUSDK.application.Name[0].Value;
            
            if (dlgSaveFile.ShowDialog() == DialogResult.OK)
            {
                SUSDK.SaveSUI(dlgSaveFile.FileName);

                curPanel = 0;

                SetPanel();
            }
        }
        
        void clSUA_Click(object sender, EventArgs e)
        {
            if (pageSUA != null)
            {
                try
                {
                    pageSUA.Dispose();
                }
                catch (Exception) { }
            }

            pageSUA = new SUAInfo();

            pageSUA.BackColor = System.Drawing.Color.White;

            pageSUA.Dock = System.Windows.Forms.DockStyle.Fill;

            pageSUA.Location = new System.Drawing.Point(0, 56);

            pageSUA.Name = "pageSUA";

            Controls.Add(pageSUA );

            pageSUA.Visible = true;

            pageSUA.BringToFront();
        }

        #endregion

        #region Form

        void FrmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (curPanel > 0)
            {
                WindowsUI.TaskDialog.UseToolWindowOnXP = false;

                if (Settings.Default.exitMessage)
                {
                    if (WindowsUI.TaskDialog.Show(ProductName, Program.RM.GetString("ExitConfirm"), Program.RM.GetString("LoseProgress"), "", "", Program.RM.GetString("DontShowMessage"),
                        WindowsUI.TaskDialogButtons.YesNo, WindowsUI.TaskDialogIcons.Warning, WindowsUI.TaskDialogIcons.None) == DialogResult.No)
                        e.Cancel = true;

                    if (WindowsUI.TaskDialog.VerificationChecked)
                    {
                        Settings.Default.exitMessage = false;

                        Settings.Default.Save();
                    }
                }

            }
        }

        #endregion

        #region Labels

        void lblAbout_Click(object sender, EventArgs e)
        {
            About about = new About();

            about.ShowDialog();
        }
                
        void lblAbout_MouseEnter(object sender, EventArgs e)
         {
             Label label = ((Label)sender);

             label.ForeColor = Color.FromArgb(51, 153, 255);

             label.Font = new Font(label.Font, FontStyle.Underline);
         }
         
        void lblAbout_MouseLeave(object sender, EventArgs e)
        {
             Label label = ((Label)sender);

             label.ForeColor = Color.FromArgb(0, 102, 204);

             label.Font = new Font(label.Font, FontStyle.Regular);
         }

        void lblStepA_Click(object sender, EventArgs e)
        {
            if (lblStepA.Cursor == Cursors.Hand)
            {
                curPanel = 21; 
                SetPanel();
                
            }
        }

        void lblStepB_Click(object sender, EventArgs e)
        {
            if (lblStepB.Cursor == Cursors.Hand)
            {
                curPanel = 22; SetPanel();
            }
        }

        void lblStepC_Click(object sender, EventArgs e)
        {
            if (lblStepC.Cursor == Cursors.Hand)
            {
                curPanel = 23; SetPanel();
            }
        }

        void lblStep2_Click(object sender, EventArgs e)
        {
            if (lblStep2.Cursor == Cursors.Hand)
            {
                curPanel = 2;
                SetPanel();
            }
        }

        void lblStep3_Click(object sender, EventArgs e)
        {
            if (lblStep3.Cursor == Cursors.Hand)
            {
                curPanel = 3;
                SetPanel();
            }
        }

        #endregion

        #endregion
    }
}
