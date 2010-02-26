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
using System.Collections.ObjectModel;
using System.Drawing;
using System.Windows.Forms;
using SevenUpdate.Base;
using SevenUpdate.Sdk.Pages;
using SevenUpdate.Sdk.Properties;
using WindowsUI;

#endregion

namespace SevenUpdate.Sdk.WinForms
{
    public sealed partial class Main : Form
    {
        #region Global Vars

        /// <summary>
        /// The current panel Index
        /// </summary>
        private int curPanel;

        /// <summary>
        /// Indicates if currently editing an update
        /// </summary>
        private bool editUpdate;

        /// <summary>
        /// Indicates if it's a new Sui file
        /// </summary>
        private bool newSui;

        /// <summary>
        /// Indicates if the app in a x64 bit Application
        /// </summary>
        internal static bool Is64Bit { get; set; }

        /// <summary>
        /// Gets or Sets the current sui files that has been opened
        /// </summary>
        private string SuiFile { get; set; }

        #endregion

        #region Page Declarations

        private AppInfo pageAppInfo;
        private Files pageFiles;

        private Registry pageRegistry;

        private Shortcuts pageShortcuts;
        private SuaInfo pageSua;
        private UpdateInfo pageUpdateInfo;
        private UpdateMenu pageUpdateMenu;

        #endregion

        public Main()
        {
            Font = SystemFonts.MessageBoxFont;

            InitializeComponent();

            LoadUI();
        }

        /// <summary>
        /// Loads the Form and loads a Sui file
        /// </summary>
        /// <param name="file">The fullpath to the Sui</param>
        public Main(string file)
        {
            Font = SystemFonts.MessageBoxFont;

            InitializeComponent();

            LoadUI();

            LoadPages();

            if (!Sdk.LoadSui(file))
                return;

            pageAppInfo.LoadInfo();

            SuiFile = file;

            curPanel = 1;

            SetPanel();
        }

        #region Methods

        /// <summary>
        /// Clears the UI of the pages
        /// </summary>
        private void ClearPages()
        {
            pageUpdateInfo.ClearUI();

            pageFiles.ClearUI(true);

            pageRegistry.ClearUI(true);

            pageShortcuts.ClearUI(true);
        }

        /// <summary>
        /// Loads the pages
        /// </summary>
        private void LoadPages()
        {
            if (pageAppInfo != null)
            {
                try
                {
                    pageAppInfo.Dispose();
                }
                catch
                {
                }
            }

            if (pageFiles != null)
            {
                try
                {
                    pageFiles.Dispose();
                }
                catch
                {
                }
            }

            if (pageRegistry != null)
            {
                try
                {
                    pageRegistry.Dispose();
                }
                catch
                {
                }
            }

            if (pageShortcuts != null)
            {
                try
                {
                    pageShortcuts.Dispose();
                }
                catch
                {
                }
            }

            if (pageUpdateInfo != null)
            {
                try
                {
                    pageUpdateInfo.Dispose();
                }
                catch
                {
                }
            }

            if (pageUpdateMenu != null)
            {
                try
                {
                    pageUpdateMenu.Dispose();
                }
                catch
                {
                }
            }

            pageAppInfo = new AppInfo {BackColor = Color.White, Dock = DockStyle.Fill, Location = new Point(0, 56), Name = "pageAppInfo"};
            Controls.Add(pageAppInfo);
            pageAppInfo.Visible = false;
            pageAppInfo.SendToBack();

            pageFiles = new Files {BackColor = Color.White, Dock = DockStyle.Fill, Location = new Point(0, 56), Name = "pageFiles"};
            Controls.Add(pageFiles);
            pageFiles.Visible = false;
            pageFiles.SendToBack();

            pageRegistry = new Registry {BackColor = Color.White, Dock = DockStyle.Fill, Location = new Point(0, 56), Name = "pageRegistry"};
            Controls.Add(pageRegistry);
            pageRegistry.Visible = false;
            pageRegistry.SendToBack();

            pageShortcuts = new Shortcuts {BackColor = Color.White, Dock = DockStyle.Fill, Location = new Point(0, 56), Name = "pageShortcuts"};
            Controls.Add(pageShortcuts);
            pageShortcuts.Visible = false;
            pageShortcuts.SendToBack();

            pageUpdateInfo = new UpdateInfo {BackColor = Color.White, Dock = DockStyle.Fill, Location = new Point(0, 56), Name = "pageUpdInfo"};
            Controls.Add(pageUpdateInfo);
            pageUpdateInfo.Visible = false;
            pageUpdateInfo.SendToBack();

            pageUpdateMenu = new UpdateMenu {BackColor = Color.White, Dock = DockStyle.Fill, Location = new Point(0, 56), Name = "pageUpdateInstallMenu"};
            Controls.Add(pageUpdateMenu);
            pageUpdateMenu.Visible = false;
            pageUpdateMenu.SendToBack();

            //Event Handlers
            pageUpdateMenu.clNewUpdate.Click -= NewUpdate_Click;

            pageUpdateMenu.clEditUpdate.Click -= ClEditUpdateClick;

            pageUpdateMenu.clSave.Click -= Save_Click;

            pageUpdateMenu.clNewUpdate.Click += NewUpdate_Click;

            pageUpdateMenu.clEditUpdate.Click += ClEditUpdateClick;

            pageUpdateMenu.clSave.Click += Save_Click;
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

            if (Environment.OSVersion.Version.Major >= 6 && Size.Height == 500)
            {
                Size = new Size(Width, 507);

                MinimumSize = new Size(700, 507);
            }

            Base.Base.SerializationErrorEventHandler += Base_SerializationErrorEventHandler;
        }

        /// <summary>
        /// Save an update of a program in a sui
        /// </summary>
        private void SaveUpdate()
        {
            var update = new Update {LicenseUrl = pageUpdateInfo.txtLicenseURL.Text};

            if (pageUpdateInfo.txtDownloadURL.Text.Substring(pageUpdateInfo.txtDownloadURL.Text.Length - 1, 1) == "\\")
                pageUpdateInfo.txtDownloadURL.Text = pageUpdateInfo.txtDownloadURL.Text.Substring(0, pageUpdateInfo.txtDownloadURL.Text.Length - 1);

            var url = new Uri(pageUpdateInfo.txtDownloadURL.Text);
            update.DownloadUrl = url.AbsoluteUri;

            update.ReleaseDate = pageUpdateInfo.dtReleaseDate.Text;
            update.InfoUrl = pageUpdateInfo.txtInfoURL.Text;

            var lsCollection = new ObservableCollection<LocaleString>();
            var ls = new LocaleString {Value = pageUpdateInfo.txtUpdateTitle.Text, Lang = "en"};
            lsCollection.Add(ls);
            update.Name = lsCollection;

            lsCollection = new ObservableCollection<LocaleString>();
            ls = new LocaleString {Value = pageUpdateInfo.txtUpdateInfo.Text, Lang = "en"};
            lsCollection.Add(ls);
            update.Description = lsCollection;

            switch (pageUpdateInfo.cbUpdateType.SelectedIndex)
            {
                case 0:
                    update.Importance = Importance.Important;
                    break;

                case 1:
                    update.Importance = Importance.Recommended;
                    break;

                case 2:
                    update.Importance = Importance.Optional;
                    break;

                case 3:
                    update.Importance = Importance.Locale;
                    break;
            }
            update.Files = pageFiles.UpdateFiles;

            update.RegistryItems = pageRegistry.UpdateRegistry;

            update.Shortcuts = pageShortcuts.UpdateShortcuts;

            if (Sdk.Application.Updates == null)
                Sdk.Application.Updates = new ObservableCollection<Update>();
            Sdk.Application.Updates.Add(update);

            if (editUpdate && pageUpdateMenu.Index > -1)
                Sdk.Application.Updates.RemoveAt(pageUpdateMenu.Index);
        }

        /// <summary>
        /// Sets the Top Panel Menu depending on if your editing
        /// </summary>
        /// <param name="editing">Indictates if your editing a Sui</param>
        private void SetMenu(bool editing)
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
        private void SetPanel()
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
                    break;

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

        private static void Base_SerializationErrorEventHandler(object sender, SerializationErrorEventArgs e)
        {
            MessageBox.Show(Program.RM.GetString("CouldNotLoad") + @" " + e.File + @": " + e.Exception);
        }

        #endregion

        #region UI Events

        #region Buttons

        private void Cancel_Click(object sender, EventArgs e)
        {
            if ((curPanel < 4 && curPanel != 2) || (newSui && Sdk.Application.Updates == null))
                curPanel = 0;
            else
                curPanel = 3;

            SetPanel();
        }

        private void Next_Click(object sender, EventArgs e)
        {
            if ((pageAppInfo.cbLoc.SelectedIndex > 0 && pageAppInfo.txtValueName.Text.Length < 1) || (pageAppInfo.txtAppDir.Text.Length < 1) ||
                ((pageUpdateInfo.txtUpdateTitle.Text.Length < 1 || pageUpdateInfo.txtDownloadURL.Text.Length < 1) && curPanel == 2))
                MessageBox.Show(Program.RM.GetString("FillRequiredInformation"));
            else
            {
                if (curPanel == 2)
                    curPanel = 20;

                if (curPanel == 1)
                {
                    if (!newSui)
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

        private void ClEditUpdateClick(object sender, EventArgs e)
        {
            editUpdate = true;

            curPanel = 2;

            SetPanel();

            SetMenu(true);

            ClearPages();

            pageUpdateInfo.txtInfoURL.Text = Sdk.Application.Updates[pageUpdateMenu.Index].InfoUrl;
            pageUpdateInfo.txtLicenseURL.Text = Sdk.Application.Updates[pageUpdateMenu.Index].LicenseUrl;
            pageUpdateInfo.txtUpdateInfo.Text = Sdk.Application.Updates[pageUpdateMenu.Index].Description[0].Value;
            pageUpdateInfo.txtUpdateTitle.Text = Sdk.Application.Updates[pageUpdateMenu.Index].Name[0].Value;
            pageUpdateInfo.dtReleaseDate.Text = Sdk.Application.Updates[pageUpdateMenu.Index].ReleaseDate;
            pageUpdateInfo.txtDownloadURL.Text = Sdk.Application.Updates[pageUpdateMenu.Index].DownloadUrl;

            switch (Sdk.Application.Updates[pageUpdateMenu.Index].Importance)
            {
                case Importance.Important:
                    pageUpdateInfo.cbUpdateType.SelectedIndex = 0;
                    break;

                case Importance.Recommended:
                    pageUpdateInfo.cbUpdateType.SelectedIndex = 1;
                    break;

                case Importance.Optional:
                    pageUpdateInfo.cbUpdateType.SelectedIndex = 2;
                    break;

                case Importance.Locale:
                    pageUpdateInfo.cbUpdateType.SelectedIndex = 3;
                    break;
            }
            pageFiles.LoadFiles(Sdk.Application.Updates[pageUpdateMenu.Index].Files);

            pageRegistry.LoadRegistryItems(Sdk.Application.Updates[pageUpdateMenu.Index].RegistryItems);

            pageShortcuts.LoadShortcuts(Sdk.Application.Updates[pageUpdateMenu.Index].Shortcuts);
        }

        private void NewSui_Click(object sender, EventArgs e)
        {
            SuiFile = null;

            LoadPages();

            newSui = true;
            Sdk.Application = new Sui {Updates = new ObservableCollection<Update>()};

            pageAppInfo.ClearUI();

            ClearPages();

            curPanel = 1;

            SetPanel();
        }

        private void NewUpdate_Click(object sender, EventArgs e)
        {
            editUpdate = false;

            curPanel = 2;

            SetPanel();

            SetMenu(true);

            ClearPages();
        }

        private void OpenSui_Click(object sender, EventArgs e)
        {
            if (dlgOpenFile.ShowDialog() != DialogResult.OK)
                return;


            SuiFile = dlgOpenFile.FileName;

            if (!Sdk.LoadSui(SuiFile))
                return;

            LoadPages();

            newSui = false;

            pageAppInfo.ClearUI();

            ClearPages();


            pageAppInfo.LoadInfo();

            curPanel = 1;

            SetPanel();
        }

        private void Save_Click(object sender, EventArgs e)
        {
            pageAppInfo.SaveInfo();

            dlgSaveFile.FileName = SuiFile;

            if (dlgSaveFile.ShowDialog() != DialogResult.OK)
                return;
            Sdk.SaveSui(dlgSaveFile.FileName);

            curPanel = 0;

            SetPanel();
        }

        private void Sua_Click(object sender, EventArgs e)
        {
            if (pageSua != null)
            {
                try
                {
                    pageSua.Dispose();
                }
                catch
                {
                }
            }

            pageSua = new SuaInfo {BackColor = Color.White, Dock = DockStyle.Fill, Location = new Point(0, 56), Name = "pageSua"};

            Controls.Add(pageSua);

            pageSua.Visible = true;

            pageSua.BringToFront();
        }

        #endregion

        #region Form

        private void Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (curPanel <= 0)
                return;
            TaskDialog.UseToolWindowOnXP = false;

            if (!Settings.Default.exitMessage)
                return;
            if (
                TaskDialog.Show(ProductName, Program.RM.GetString("ExitConfirm"), Program.RM.GetString("LoseProgress"), "", "", Program.RM.GetString("DontShowMessage"),
                                TaskDialogButtons.YesNo, TaskDialogIcons.Warning, TaskDialogIcons.None) == DialogResult.No)
                e.Cancel = true;

            if (!TaskDialog.VerificationChecked)
                return;
            Settings.Default.exitMessage = false;

            Settings.Default.Save();
        }

        #endregion

        #region Labels

        private void About_Click(object sender, EventArgs e)
        {
            var about = new About();

            about.ShowDialog();
        }

        private void About_MouseEnter(object sender, EventArgs e)
        {
            var label = ((Label) sender);

            label.ForeColor = Color.FromArgb(51, 153, 255);

            label.Font = new Font(label.Font, FontStyle.Underline);
        }

        private void About_MouseLeave(object sender, EventArgs e)
        {
            var label = ((Label) sender);

            label.ForeColor = Color.FromArgb(0, 102, 204);

            label.Font = new Font(label.Font, FontStyle.Regular);
        }

        private void StepA_Click(object sender, EventArgs e)
        {
            if (lblStepA.Cursor != Cursors.Hand)
                return;
            curPanel = 21;
            SetPanel();
        }

        private void StepB_Click(object sender, EventArgs e)
        {
            if (lblStepB.Cursor != Cursors.Hand)
                return;
            curPanel = 22;
            SetPanel();
        }

        private void StepC_Click(object sender, EventArgs e)
        {
            if (lblStepC.Cursor != Cursors.Hand)
                return;
            curPanel = 23;
            SetPanel();
        }

        private void Step2_Click(object sender, EventArgs e)
        {
            if (lblStep2.Cursor != Cursors.Hand)
                return;
            curPanel = 2;
            SetPanel();
        }

        private void Step3_Click(object sender, EventArgs e)
        {
            if (lblStep3.Cursor != Cursors.Hand)
                return;
            curPanel = 3;
            SetPanel();
        }

        #endregion

        #endregion
    }
}