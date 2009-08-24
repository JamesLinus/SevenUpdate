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
using System.Drawing;
namespace SevenUpdate.Pages
{
    partial class Options
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
            this.lblHeading = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.chkRecommendedUpdates = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.pbShield = new System.Windows.Forms.PictureBox();
            this.label4 = new System.Windows.Forms.Label();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.pnlButtonSection = new System.Windows.Forms.Panel();
            this.lblLastUpdated = new System.Windows.Forms.Label();
            this.cbAutoUpdateMethod = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.lblLoading = new System.Windows.Forms.Label();
            this.listView = new System.Windows.Forms.ListView();
            this.columnUpdate = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnTitle = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnPublisher = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnInstalled = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnType = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.lblRefresh = new System.Windows.Forms.Label();
            this.chkSelectAll = new System.Windows.Forms.CheckBox();
            this.lineSeparator1 = new System.Drawing.LineSeparator();
            this.lineSeparator2 = new System.Drawing.LineSeparator();
            ((System.ComponentModel.ISupportInitialize)(this.pbShield)).BeginInit();
            this.pnlButtonSection.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblHeading
            // 
            this.lblHeading.AutoSize = true;
            this.lblHeading.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(51)))), ((int)(((byte)(153)))));
            this.lblHeading.Location = new System.Drawing.Point(15, 14);
            this.lblHeading.Name = "lblHeading";
            this.lblHeading.Size = new System.Drawing.Size(229, 13);
            this.lblHeading.TabIndex = 13;
            this.lblHeading.Text = "Choose how Seven Update can install updates";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(16, 42);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(591, 13);
            this.label1.TabIndex = 14;
            this.label1.Text = "When your computer is online, Seven Update can automatically check for updates an" +
                "d download them using these settings.";
            // 
            // chkRecommendedUpdates
            // 
            this.chkRecommendedUpdates.AutoSize = true;
            this.chkRecommendedUpdates.Checked = true;
            this.chkRecommendedUpdates.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkRecommendedUpdates.Location = new System.Drawing.Point(56, 146);
            this.chkRecommendedUpdates.Name = "chkRecommendedUpdates";
            this.chkRecommendedUpdates.Size = new System.Drawing.Size(374, 17);
            this.chkRecommendedUpdates.TabIndex = 20;
            this.chkRecommendedUpdates.Text = "Give me recommended updates the same way i receive important updates";
            this.chkRecommendedUpdates.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(51)))), ((int)(((byte)(153)))));
            this.label3.Location = new System.Drawing.Point(15, 180);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(87, 19);
            this.label3.TabIndex = 23;
            this.label3.Text = "Update service";
            this.label3.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // pbShield
            // 
            this.pbShield.Image = global::SevenUpdate.Properties.Resources.smallGreenShield;
            this.pbShield.Location = new System.Drawing.Point(18, 100);
            this.pbShield.Name = "pbShield";
            this.pbShield.Size = new System.Drawing.Size(32, 32);
            this.pbShield.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pbShield.TabIndex = 26;
            this.pbShield.TabStop = false;
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(15, 208);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(761, 22);
            this.label4.TabIndex = 32;
            this.label4.Text = "Select the programs that will be updated with Seven Update. Right click an item t" +
                "o view a description of the program.";
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.Location = new System.Drawing.Point(737, 10);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 0;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnSave
            // 
            this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSave.Location = new System.Drawing.Point(657, 10);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 1;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // pnlButtonSection
            // 
            this.pnlButtonSection.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.pnlButtonSection.Controls.Add(this.btnSave);
            this.pnlButtonSection.Controls.Add(this.btnCancel);
            this.pnlButtonSection.Controls.Add(this.lblLastUpdated);
            this.pnlButtonSection.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlButtonSection.Location = new System.Drawing.Point(0, 508);
            this.pnlButtonSection.Name = "pnlButtonSection";
            this.pnlButtonSection.Size = new System.Drawing.Size(832, 44);
            this.pnlButtonSection.TabIndex = 30;
            // 
            // lblLastUpdated
            // 
            this.lblLastUpdated.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblLastUpdated.AutoSize = true;
            this.lblLastUpdated.Location = new System.Drawing.Point(15, 15);
            this.lblLastUpdated.Name = "lblLastUpdated";
            this.lblLastUpdated.Size = new System.Drawing.Size(72, 13);
            this.lblLastUpdated.TabIndex = 52;
            this.lblLastUpdated.Text = "Last updated:";
            // 
            // cbAutoUpdateMethod
            // 
            this.cbAutoUpdateMethod.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbAutoUpdateMethod.FormattingEnabled = true;
            this.cbAutoUpdateMethod.Items.AddRange(new object[] {
            "Install updates automatically",
            "Download updates but let me choose whether to install them (recommended)",
            "Check for updates but let me choose whether to download and install them",
            "Never check for updates (not recommended)"});
            this.cbAutoUpdateMethod.Location = new System.Drawing.Point(56, 106);
            this.cbAutoUpdateMethod.Name = "cbAutoUpdateMethod";
            this.cbAutoUpdateMethod.Size = new System.Drawing.Size(450, 21);
            this.cbAutoUpdateMethod.TabIndex = 39;
            this.cbAutoUpdateMethod.SelectedIndexChanged += new System.EventHandler(this.cbAutoUpdateMethod_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(51)))), ((int)(((byte)(153)))));
            this.label2.Location = new System.Drawing.Point(15, 64);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(109, 19);
            this.label2.TabIndex = 40;
            this.label2.Text = "Important Updates";
            this.label2.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // lblLoading
            // 
            this.lblLoading.AutoSize = true;
            this.lblLoading.Location = new System.Drawing.Point(254, 357);
            this.lblLoading.Name = "lblLoading";
            this.lblLoading.Size = new System.Drawing.Size(193, 13);
            this.lblLoading.TabIndex = 55;
            this.lblLoading.Text = "Downloading program list, please wait...";
            this.lblLoading.Visible = false;
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
            this.columnTitle,
            this.columnPublisher,
            this.columnInstalled,
            this.columnType});
            this.listView.ForeColor = System.Drawing.SystemColors.WindowText;
            this.listView.FullRowSelect = true;
            this.listView.Location = new System.Drawing.Point(0, 237);
            this.listView.MultiSelect = false;
            this.listView.Name = "listView";
            this.listView.Size = new System.Drawing.Size(832, 271);
            this.listView.TabIndex = 54;
            this.listView.UseCompatibleStateImageBehavior = false;
            this.listView.View = System.Windows.Forms.View.Details;
            this.listView.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.listView_ColumnClick);
            this.listView.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.listView_ItemCheck);
            this.listView.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler(this.listView_ItemChecked);
            this.listView.MouseClick += new System.Windows.Forms.MouseEventHandler(this.listView_MouseClick);
            // 
            // columnUpdate
            // 
            this.columnUpdate.Width = 24;
            // 
            // columnTitle
            // 
            this.columnTitle.Text = "Program Name";
            this.columnTitle.Width = 368;
            // 
            // columnPublisher
            // 
            this.columnPublisher.Text = "Publisher";
            this.columnPublisher.Width = 262;
            // 
            // columnInstalled
            // 
            this.columnInstalled.Text = "Installed";
            this.columnInstalled.Width = 78;
            // 
            // columnType
            // 
            this.columnType.Text = "Architecture";
            this.columnType.Width = 81;
            // 
            // lblRefresh
            // 
            this.lblRefresh.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblRefresh.BackColor = System.Drawing.Color.Transparent;
            this.lblRefresh.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lblRefresh.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(102)))), ((int)(((byte)(204)))));
            this.lblRefresh.Location = new System.Drawing.Point(764, 217);
            this.lblRefresh.Name = "lblRefresh";
            this.lblRefresh.Size = new System.Drawing.Size(65, 17);
            this.lblRefresh.TabIndex = 53;
            this.lblRefresh.TabStop = true;
            this.lblRefresh.Text = "Refresh";
            this.lblRefresh.TextAlign = System.Drawing.ContentAlignment.BottomRight;
            this.lblRefresh.Click += new System.EventHandler(this.lblRefresh_Click);
            this.lblRefresh.MouseEnter += new System.EventHandler(this.label_MouseEnter);
            this.lblRefresh.MouseLeave += new System.EventHandler(this.label_MouseLeave);
            // 
            // chkSelectAll
            // 
            this.chkSelectAll.AutoSize = true;
            this.chkSelectAll.BackColor = System.Drawing.Color.Transparent;
            this.chkSelectAll.Location = new System.Drawing.Point(6, 243);
            this.chkSelectAll.Name = "chkSelectAll";
            this.chkSelectAll.Size = new System.Drawing.Size(15, 14);
            this.chkSelectAll.TabIndex = 56;
            this.chkSelectAll.UseVisualStyleBackColor = false;
            this.chkSelectAll.CheckedChanged += new System.EventHandler(this.chkSelectAll_CheckedChanged);
            // 
            // lineSeparator1
            // 
            this.lineSeparator1.BackColor = System.Drawing.Color.LightGray;
            this.lineSeparator1.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lineSeparator1.Location = new System.Drawing.Point(120, 76);
            this.lineSeparator1.MaximumSize = new System.Drawing.Size(2000, 2);
            this.lineSeparator1.MinimumSize = new System.Drawing.Size(0, 2);
            this.lineSeparator1.Name = "lineSeparator1";
            this.lineSeparator1.Size = new System.Drawing.Size(575, 2);
            this.lineSeparator1.TabIndex = 41;
            // 
            // lineSeparator2
            // 
            this.lineSeparator2.BackColor = System.Drawing.Color.LightGray;
            this.lineSeparator2.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lineSeparator2.Location = new System.Drawing.Point(105, 192);
            this.lineSeparator2.MaximumSize = new System.Drawing.Size(2000, 2);
            this.lineSeparator2.MinimumSize = new System.Drawing.Size(0, 2);
            this.lineSeparator2.Name = "lineSeparator2";
            this.lineSeparator2.Size = new System.Drawing.Size(588, 2);
            this.lineSeparator2.TabIndex = 36;
            // 
            // Options
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.AutoScroll = true;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.chkSelectAll);
            this.Controls.Add(this.lblLoading);
            this.Controls.Add(this.listView);
            this.Controls.Add(this.lblRefresh);
            this.Controls.Add(this.lineSeparator1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.cbAutoUpdateMethod);
            this.Controls.Add(this.lineSeparator2);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.pbShield);
            this.Controls.Add(this.lblHeading);
            this.Controls.Add(this.pnlButtonSection);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.chkRecommendedUpdates);
            this.MinimumSize = new System.Drawing.Size(832, 552);
            this.Name = "Options";
            this.Size = new System.Drawing.Size(832, 552);
            this.Load += new System.EventHandler(this.Options_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pbShield)).EndInit();
            this.pnlButtonSection.ResumeLayout(false);
            this.pnlButtonSection.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        System.Windows.Forms.Label lblHeading;
        System.Windows.Forms.Label label1;
        System.Windows.Forms.CheckBox chkRecommendedUpdates;
        System.Windows.Forms.Label label3;
        System.Windows.Forms.PictureBox pbShield;
        System.Windows.Forms.Label label4;
        LineSeparator lineSeparator2;
        System.Windows.Forms.Button btnCancel;
        System.Windows.Forms.Panel pnlButtonSection;
        private System.Windows.Forms.ComboBox cbAutoUpdateMethod;
        private LineSeparator lineSeparator1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lblLoading;
        private System.Windows.Forms.ListView listView;
        private System.Windows.Forms.ColumnHeader columnUpdate;
        private System.Windows.Forms.ColumnHeader columnTitle;
        private System.Windows.Forms.ColumnHeader columnPublisher;
        private System.Windows.Forms.ColumnHeader columnInstalled;
        private System.Windows.Forms.Label lblRefresh;
        private System.Windows.Forms.Label lblLastUpdated;
        private System.Windows.Forms.CheckBox chkSelectAll;
        private System.Windows.Forms.ColumnHeader columnType;
        private System.Windows.Forms.Button btnSave;
    }
}
