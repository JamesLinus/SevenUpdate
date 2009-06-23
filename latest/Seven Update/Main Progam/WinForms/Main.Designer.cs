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
namespace SevenUpdate.WinForms
{
    partial class Main
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Main));
            this.cmsMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.checkForUpdatesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.lblAppName = new System.Windows.Forms.Label();
            this.lblAutoOption = new System.Windows.Forms.Label();
            this.lblUpdatesInstalled = new System.Windows.Forms.Label();
            this.lblRecentCheck = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.infoBar = new SevenUpdate.Controls.InfoBar();
            this.doubleBufferPanel = new SevenUpdate.Controls.DoubleBufferPanel();
            this.lblAbout = new System.Windows.Forms.Label();
            this.lblRestoreHiddenUpdates = new System.Windows.Forms.Label();
            this.lblCheckForUpdates = new System.Windows.Forms.Label();
            this.lblChangeSettings = new System.Windows.Forms.Label();
            this.lblUpdateHistory = new System.Windows.Forms.Label();
            this.cmsMenu.SuspendLayout();
            this.doubleBufferPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // cmsMenu
            // 
            this.cmsMenu.AccessibleDescription = null;
            this.cmsMenu.AccessibleName = null;
            resources.ApplyResources(this.cmsMenu, "cmsMenu");
            this.cmsMenu.BackgroundImage = null;
            this.cmsMenu.Font = null;
            this.cmsMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.checkForUpdatesToolStripMenuItem,
            this.exitToolStripMenuItem});
            this.cmsMenu.Name = "cmsMenu";
            // 
            // checkForUpdatesToolStripMenuItem
            // 
            this.checkForUpdatesToolStripMenuItem.AccessibleDescription = null;
            this.checkForUpdatesToolStripMenuItem.AccessibleName = null;
            resources.ApplyResources(this.checkForUpdatesToolStripMenuItem, "checkForUpdatesToolStripMenuItem");
            this.checkForUpdatesToolStripMenuItem.BackgroundImage = null;
            this.checkForUpdatesToolStripMenuItem.Name = "checkForUpdatesToolStripMenuItem";
            this.checkForUpdatesToolStripMenuItem.ShortcutKeyDisplayString = null;
            this.checkForUpdatesToolStripMenuItem.Click += new System.EventHandler(this.checkForUpdatesToolStripMenuItem_Click);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.AccessibleDescription = null;
            this.exitToolStripMenuItem.AccessibleName = null;
            resources.ApplyResources(this.exitToolStripMenuItem, "exitToolStripMenuItem");
            this.exitToolStripMenuItem.BackgroundImage = null;
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.ShortcutKeyDisplayString = null;
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // lblAppName
            // 
            this.lblAppName.AccessibleDescription = null;
            this.lblAppName.AccessibleName = null;
            resources.ApplyResources(this.lblAppName, "lblAppName");
            this.lblAppName.Font = null;
            this.lblAppName.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(51)))), ((int)(((byte)(153)))));
            this.lblAppName.Name = "lblAppName";
            // 
            // lblAutoOption
            // 
            this.lblAutoOption.AccessibleDescription = null;
            this.lblAutoOption.AccessibleName = null;
            resources.ApplyResources(this.lblAutoOption, "lblAutoOption");
            this.lblAutoOption.Font = null;
            this.lblAutoOption.Name = "lblAutoOption";
            this.lblAutoOption.UseMnemonic = false;
            // 
            // lblUpdatesInstalled
            // 
            this.lblUpdatesInstalled.AccessibleDescription = null;
            this.lblUpdatesInstalled.AccessibleName = null;
            resources.ApplyResources(this.lblUpdatesInstalled, "lblUpdatesInstalled");
            this.lblUpdatesInstalled.Font = null;
            this.lblUpdatesInstalled.Name = "lblUpdatesInstalled";
            // 
            // lblRecentCheck
            // 
            this.lblRecentCheck.AccessibleDescription = null;
            this.lblRecentCheck.AccessibleName = null;
            resources.ApplyResources(this.lblRecentCheck, "lblRecentCheck");
            this.lblRecentCheck.Font = null;
            this.lblRecentCheck.Name = "lblRecentCheck";
            // 
            // label7
            // 
            this.label7.AccessibleDescription = null;
            this.label7.AccessibleName = null;
            resources.ApplyResources(this.label7, "label7");
            this.label7.Font = null;
            this.label7.Name = "label7";
            // 
            // label6
            // 
            this.label6.AccessibleDescription = null;
            this.label6.AccessibleName = null;
            resources.ApplyResources(this.label6, "label6");
            this.label6.Font = null;
            this.label6.Name = "label6";
            // 
            // label5
            // 
            this.label5.AccessibleDescription = null;
            this.label5.AccessibleName = null;
            resources.ApplyResources(this.label5, "label5");
            this.label5.Font = null;
            this.label5.Name = "label5";
            // 
            // infoBar
            // 
            this.infoBar.AccessibleDescription = null;
            this.infoBar.AccessibleName = null;
            resources.ApplyResources(this.infoBar, "infoBar");
            this.infoBar.BackColor = System.Drawing.Color.White;
            this.infoBar.MaximumSize = new System.Drawing.Size(609, 109);
            this.infoBar.MinimumSize = new System.Drawing.Size(609, 69);
            this.infoBar.Name = "infoBar";
            // 
            // doubleBufferPanel
            // 
            this.doubleBufferPanel.AccessibleDescription = null;
            this.doubleBufferPanel.AccessibleName = null;
            resources.ApplyResources(this.doubleBufferPanel, "doubleBufferPanel");
            this.doubleBufferPanel.BackgroundImage = global::SevenUpdate.Properties.Resources.sidebar;
            this.doubleBufferPanel.Controls.Add(this.lblAbout);
            this.doubleBufferPanel.Controls.Add(this.lblRestoreHiddenUpdates);
            this.doubleBufferPanel.Controls.Add(this.lblCheckForUpdates);
            this.doubleBufferPanel.Controls.Add(this.lblChangeSettings);
            this.doubleBufferPanel.Controls.Add(this.lblUpdateHistory);
            this.doubleBufferPanel.Font = null;
            this.doubleBufferPanel.Name = "doubleBufferPanel";
            // 
            // lblAbout
            // 
            this.lblAbout.AccessibleDescription = null;
            this.lblAbout.AccessibleName = null;
            resources.ApplyResources(this.lblAbout, "lblAbout");
            this.lblAbout.BackColor = System.Drawing.Color.Transparent;
            this.lblAbout.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lblAbout.Font = null;
            this.lblAbout.ForeColor = System.Drawing.Color.White;
            this.lblAbout.Name = "lblAbout";
            this.lblAbout.TabStop = true;
            this.lblAbout.Click += new System.EventHandler(this.lblAbout_Click);
            this.lblAbout.MouseEnter += new System.EventHandler(this.label_MouseEnter);
            this.lblAbout.MouseLeave += new System.EventHandler(this.label_MouseLeave);
            // 
            // lblRestoreHiddenUpdates
            // 
            this.lblRestoreHiddenUpdates.AccessibleDescription = null;
            this.lblRestoreHiddenUpdates.AccessibleName = null;
            resources.ApplyResources(this.lblRestoreHiddenUpdates, "lblRestoreHiddenUpdates");
            this.lblRestoreHiddenUpdates.BackColor = System.Drawing.Color.Transparent;
            this.lblRestoreHiddenUpdates.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lblRestoreHiddenUpdates.Font = null;
            this.lblRestoreHiddenUpdates.ForeColor = System.Drawing.Color.White;
            this.lblRestoreHiddenUpdates.Name = "lblRestoreHiddenUpdates";
            this.lblRestoreHiddenUpdates.TabStop = true;
            this.lblRestoreHiddenUpdates.Click += new System.EventHandler(this.lblRestoreHiddenUpdates_Click);
            this.lblRestoreHiddenUpdates.MouseEnter += new System.EventHandler(this.label_MouseEnter);
            this.lblRestoreHiddenUpdates.MouseLeave += new System.EventHandler(this.label_MouseLeave);
            // 
            // lblCheckForUpdates
            // 
            this.lblCheckForUpdates.AccessibleDescription = null;
            this.lblCheckForUpdates.AccessibleName = null;
            resources.ApplyResources(this.lblCheckForUpdates, "lblCheckForUpdates");
            this.lblCheckForUpdates.BackColor = System.Drawing.Color.Transparent;
            this.lblCheckForUpdates.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lblCheckForUpdates.Font = null;
            this.lblCheckForUpdates.ForeColor = System.Drawing.Color.White;
            this.lblCheckForUpdates.Name = "lblCheckForUpdates";
            this.lblCheckForUpdates.TabStop = true;
            this.lblCheckForUpdates.Click += new System.EventHandler(this.lblCheckForUpdates_Click);
            this.lblCheckForUpdates.MouseEnter += new System.EventHandler(this.label_MouseEnter);
            this.lblCheckForUpdates.MouseLeave += new System.EventHandler(this.label_MouseLeave);
            // 
            // lblChangeSettings
            // 
            this.lblChangeSettings.AccessibleDescription = null;
            this.lblChangeSettings.AccessibleName = null;
            resources.ApplyResources(this.lblChangeSettings, "lblChangeSettings");
            this.lblChangeSettings.BackColor = System.Drawing.Color.Transparent;
            this.lblChangeSettings.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lblChangeSettings.Font = null;
            this.lblChangeSettings.ForeColor = System.Drawing.Color.White;
            this.lblChangeSettings.Name = "lblChangeSettings";
            this.lblChangeSettings.TabStop = true;
            this.lblChangeSettings.Click += new System.EventHandler(this.lblChangeSettings_Click);
            this.lblChangeSettings.MouseEnter += new System.EventHandler(this.label_MouseEnter);
            this.lblChangeSettings.MouseLeave += new System.EventHandler(this.label_MouseLeave);
            // 
            // lblUpdateHistory
            // 
            this.lblUpdateHistory.AccessibleDescription = null;
            this.lblUpdateHistory.AccessibleName = null;
            resources.ApplyResources(this.lblUpdateHistory, "lblUpdateHistory");
            this.lblUpdateHistory.BackColor = System.Drawing.Color.Transparent;
            this.lblUpdateHistory.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lblUpdateHistory.Font = null;
            this.lblUpdateHistory.ForeColor = System.Drawing.Color.White;
            this.lblUpdateHistory.Name = "lblUpdateHistory";
            this.lblUpdateHistory.TabStop = true;
            this.lblUpdateHistory.Click += new System.EventHandler(this.lblUpdateHistory_Click);
            this.lblUpdateHistory.MouseEnter += new System.EventHandler(this.label_MouseEnter);
            this.lblUpdateHistory.MouseLeave += new System.EventHandler(this.label_MouseLeave);
            // 
            // Main
            // 
            this.AccessibleDescription = null;
            this.AccessibleName = null;
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            resources.ApplyResources(this, "$this");
            this.BackColor = System.Drawing.Color.White;
            this.BackgroundImage = null;
            this.Controls.Add(this.infoBar);
            this.Controls.Add(this.doubleBufferPanel);
            this.Controls.Add(this.lblAppName);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.lblUpdatesInstalled);
            this.Controls.Add(this.lblRecentCheck);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.lblAutoOption);
            this.Controls.Add(this.label6);
            this.DoubleBuffered = true;
            this.Font = null;
            this.Name = "Main";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Main_FormClosing);
            this.Load += new System.EventHandler(this.Main_Load);
            this.SizeChanged += new System.EventHandler(this.Main_SizeChanged);
            this.cmsMenu.ResumeLayout(false);
            this.doubleBufferPanel.ResumeLayout(false);
            this.doubleBufferPanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        System.Windows.Forms.ContextMenuStrip cmsMenu;
        System.Windows.Forms.ToolStripMenuItem checkForUpdatesToolStripMenuItem;
        System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        System.Windows.Forms.Label lblAppName;
        System.Windows.Forms.Label label7;
        System.Windows.Forms.Label label6;
        System.Windows.Forms.Label label5;
        SevenUpdate.Controls.DoubleBufferPanel doubleBufferPanel;
        System.Windows.Forms.Label lblAutoOption;
        System.Windows.Forms.Label lblUpdatesInstalled;
        System.Windows.Forms.Label lblRecentCheck;
        private System.Windows.Forms.Label lblRestoreHiddenUpdates;
        private System.Windows.Forms.Label lblUpdateHistory;
        private System.Windows.Forms.Label lblAbout;
        private System.Windows.Forms.Label lblCheckForUpdates;
        private System.Windows.Forms.Label lblChangeSettings;
        private SevenUpdate.Controls.InfoBar infoBar;
    }
}

