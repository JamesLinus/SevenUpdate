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
namespace SevenUpdate.Pages
{
    partial class UpdateHistory
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UpdateHistory));
            this.cmsMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.cmsHideUpdate = new System.Windows.Forms.ToolStripMenuItem();
            this.pnlButtonSection = new System.Windows.Forms.Panel();
            this.btnOK = new System.Windows.Forms.Button();
            this.lblHeading = new System.Windows.Forms.Label();
            this.listView = new System.Windows.Forms.ListView();
            this.columnName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnStatus = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnImportance = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnInstallDate = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.sidebar = new SevenUpdate.Controls.Sidebar2();
            this.cmsMenu.SuspendLayout();
            this.pnlButtonSection.SuspendLayout();
            this.SuspendLayout();
            // 
            // cmsMenu
            // 
            this.cmsMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cmsHideUpdate});
            this.cmsMenu.Name = "cmsMenu";
            resources.ApplyResources(this.cmsMenu, "cmsMenu");
            this.cmsMenu.Opening += new System.ComponentModel.CancelEventHandler(this.cmsMenu_Opening);
            // 
            // cmsHideUpdate
            // 
            this.cmsHideUpdate.Name = "cmsHideUpdate";
            resources.ApplyResources(this.cmsHideUpdate, "cmsHideUpdate");
            // 
            // pnlButtonSection
            // 
            this.pnlButtonSection.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.pnlButtonSection.Controls.Add(this.btnOK);
            resources.ApplyResources(this.pnlButtonSection, "pnlButtonSection");
            this.pnlButtonSection.Name = "pnlButtonSection";
            // 
            // btnOK
            // 
            resources.ApplyResources(this.btnOK, "btnOK");
            this.btnOK.Name = "btnOK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // lblHeading
            // 
            resources.ApplyResources(this.lblHeading, "lblHeading");
            this.lblHeading.BackColor = System.Drawing.Color.White;
            this.lblHeading.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(51)))), ((int)(((byte)(153)))));
            this.lblHeading.Name = "lblHeading";
            // 
            // listView
            // 
            resources.ApplyResources(this.listView, "listView");
            this.listView.BackColor = System.Drawing.Color.White;
            this.listView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnName,
            this.columnStatus,
            this.columnImportance,
            this.columnInstallDate});
            this.listView.ContextMenuStrip = this.cmsMenu;
            this.listView.ForeColor = System.Drawing.SystemColors.WindowText;
            this.listView.FullRowSelect = true;
            this.listView.MultiSelect = false;
            this.listView.Name = "listView";
            this.listView.UseCompatibleStateImageBehavior = false;
            this.listView.View = System.Windows.Forms.View.Details;
            this.listView.SelectedIndexChanged += new System.EventHandler(this.listView_SelectedIndexChanged);
            // 
            // columnName
            // 
            resources.ApplyResources(this.columnName, "columnName");
            // 
            // columnStatus
            // 
            resources.ApplyResources(this.columnStatus, "columnStatus");
            // 
            // columnImportance
            // 
            resources.ApplyResources(this.columnImportance, "columnImportance");
            // 
            // columnInstallDate
            // 
            resources.ApplyResources(this.columnInstallDate, "columnInstallDate");
            // 
            // sidebar
            // 
            resources.ApplyResources(this.sidebar, "sidebar");
            this.sidebar.BackColor = System.Drawing.Color.White;
            this.sidebar.Name = "sidebar";
            // 
            // UpdateHistory
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.listView);
            this.Controls.Add(this.pnlButtonSection);
            this.Controls.Add(this.lblHeading);
            this.Controls.Add(this.sidebar);
            this.MinimumSize = new System.Drawing.Size(832, 552);
            this.Name = "UpdateHistory";
            resources.ApplyResources(this, "$this");
            this.Load += new System.EventHandler(this.UpdateHistory_Load);
            this.cmsMenu.ResumeLayout(false);
            this.pnlButtonSection.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        System.Windows.Forms.ContextMenuStrip cmsMenu;
        System.Windows.Forms.ToolStripMenuItem cmsHideUpdate;
        System.Windows.Forms.Panel pnlButtonSection;
        System.Windows.Forms.Button btnOK;
        System.Windows.Forms.Label lblHeading;
        System.Windows.Forms.ColumnHeader columnName;
        System.Windows.Forms.ColumnHeader columnStatus;
        System.Windows.Forms.ColumnHeader columnImportance;
        private SevenUpdate.Controls.Sidebar2 sidebar;
        private System.Windows.Forms.ColumnHeader columnInstallDate;
        private System.Windows.Forms.ListView listView;
    }
}
