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
    partial class AppUpdate
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
            this.cmsMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.cmsHideUpdate = new System.Windows.Forms.ToolStripMenuItem();
            this.lblHeading = new System.Windows.Forms.Label();
            this.pnlButtonSection = new System.Windows.Forms.Panel();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.lblSelected = new System.Windows.Forms.Label();
            this.listView = new System.Windows.Forms.ListView();
            this.columnUpdate = new System.Windows.Forms.ColumnHeader();
            this.columnName = new System.Windows.Forms.ColumnHeader();
            this.columnImportance = new System.Windows.Forms.ColumnHeader();
            this.columnSize = new System.Windows.Forms.ColumnHeader();
            this.columnStatus = new System.Windows.Forms.ColumnHeader();
            this.chkSelectAll = new System.Windows.Forms.CheckBox();
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
            this.cmsMenu.Size = new System.Drawing.Size(153, 48);
            this.cmsMenu.Opening += new System.ComponentModel.CancelEventHandler(this.cmsMenu_Opening);
            // 
            // cmsHideUpdate
            // 
            this.cmsHideUpdate.Name = "cmsHideUpdate";
            this.cmsHideUpdate.Size = new System.Drawing.Size(152, 22);
            this.cmsHideUpdate.Text = "Hide update";
            this.cmsHideUpdate.Click += new System.EventHandler(this.cmsHideUpdate_Click);
            // 
            // lblHeading
            // 
            this.lblHeading.AutoSize = true;
            this.lblHeading.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(51)))), ((int)(((byte)(153)))));
            this.lblHeading.Location = new System.Drawing.Point(18, 10);
            this.lblHeading.Name = "lblHeading";
            this.lblHeading.Size = new System.Drawing.Size(183, 13);
            this.lblHeading.TabIndex = 2;
            this.lblHeading.Text = "Select the updates you want to install";
            // 
            // pnlButtonSection
            // 
            this.pnlButtonSection.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.pnlButtonSection.Controls.Add(this.btnOK);
            this.pnlButtonSection.Controls.Add(this.btnCancel);
            this.pnlButtonSection.Controls.Add(this.lblSelected);
            this.pnlButtonSection.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlButtonSection.Location = new System.Drawing.Point(0, 508);
            this.pnlButtonSection.Name = "pnlButtonSection";
            this.pnlButtonSection.Size = new System.Drawing.Size(832, 44);
            this.pnlButtonSection.TabIndex = 31;
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.Location = new System.Drawing.Point(657, 10);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 1;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.Location = new System.Drawing.Point(738, 10);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 0;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // lblSelected
            // 
            this.lblSelected.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lblSelected.Location = new System.Drawing.Point(238, 12);
            this.lblSelected.Name = "lblSelected";
            this.lblSelected.Size = new System.Drawing.Size(413, 21);
            this.lblSelected.TabIndex = 34;
            this.lblSelected.Text = "Total selected: 0 updates";
            this.lblSelected.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // listView
            // 
            this.listView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.listView.BackColor = System.Drawing.Color.White;
            this.listView.CheckBoxes = true;
            this.listView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnUpdate,
            this.columnName,
            this.columnImportance,
            this.columnSize,
            this.columnStatus});
            this.listView.ContextMenuStrip = this.cmsMenu;
            this.listView.ForeColor = System.Drawing.SystemColors.WindowText;
            this.listView.FullRowSelect = true;
            this.listView.Location = new System.Drawing.Point(0, 41);
            this.listView.MultiSelect = false;
            this.listView.Name = "listView";
            this.listView.Size = new System.Drawing.Size(663, 467);
            this.listView.TabIndex = 32;
            this.listView.UseCompatibleStateImageBehavior = false;
            this.listView.View = System.Windows.Forms.View.Details;
            this.listView.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler(this.listView_ItemChecked);
            this.listView.SelectedIndexChanged += new System.EventHandler(this.listView_SelectedIndexChanged);
            this.listView.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.listView_ItemCheck);
            // 
            // columnUpdate
            // 
            this.columnUpdate.Text = "";
            this.columnUpdate.Width = 23;
            // 
            // columnName
            // 
            this.columnName.Text = "Name";
            this.columnName.Width = 327;
            // 
            // columnImportance
            // 
            this.columnImportance.Text = "Importance";
            this.columnImportance.Width = 101;
            // 
            // columnSize
            // 
            this.columnSize.Text = "Size";
            this.columnSize.Width = 82;
            // 
            // columnStatus
            // 
            this.columnStatus.Text = "Status";
            this.columnStatus.Width = 120;
            // 
            // chkSelectAll
            // 
            this.chkSelectAll.AutoSize = true;
            this.chkSelectAll.BackColor = System.Drawing.Color.Transparent;
            this.chkSelectAll.Location = new System.Drawing.Point(6, 47);
            this.chkSelectAll.Name = "chkSelectAll";
            this.chkSelectAll.Size = new System.Drawing.Size(15, 14);
            this.chkSelectAll.TabIndex = 33;
            this.chkSelectAll.UseVisualStyleBackColor = false;
            this.chkSelectAll.CheckedChanged += new System.EventHandler(this.chkSelectAll_CheckedChanged);
            // 
            // sidebar
            // 
            this.sidebar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.sidebar.BackColor = System.Drawing.Color.White;
            this.sidebar.BackgroundImage = global::SevenUpdate.Properties.Resources.sidebar2;
            this.sidebar.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.sidebar.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.sidebar.Location = new System.Drawing.Point(663, 41);
            this.sidebar.Name = "sidebar";
            this.sidebar.Size = new System.Drawing.Size(169, 467);
            this.sidebar.TabIndex = 34;
            // 
            // AppUpdate
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.chkSelectAll);
            this.Controls.Add(this.listView);
            this.Controls.Add(this.lblHeading);
            this.Controls.Add(this.pnlButtonSection);
            this.Controls.Add(this.sidebar);
            this.DoubleBuffered = true;
            this.MinimumSize = new System.Drawing.Size(832, 552);
            this.Name = "AppUpdate";
            this.Size = new System.Drawing.Size(832, 552);
            this.Load += new System.EventHandler(this.AppUpdate_Load);
            this.cmsMenu.ResumeLayout(false);
            this.pnlButtonSection.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        System.Windows.Forms.Label lblHeading;
        System.Windows.Forms.ContextMenuStrip cmsMenu;
        System.Windows.Forms.ToolStripMenuItem cmsHideUpdate;
        System.Windows.Forms.Panel pnlButtonSection;
        System.Windows.Forms.Button btnCancel;
        System.Windows.Forms.ColumnHeader columnUpdate;
        System.Windows.Forms.ColumnHeader columnName;
        System.Windows.Forms.ColumnHeader columnImportance;
        System.Windows.Forms.CheckBox chkSelectAll;
        System.Windows.Forms.Label lblSelected;
        System.Windows.Forms.ListView listView;
        private System.Windows.Forms.ColumnHeader columnSize;
        private SevenUpdate.Controls.Sidebar2 sidebar;
        private System.Windows.Forms.ColumnHeader columnStatus;
        private System.Windows.Forms.Button btnOK;
    }
}
