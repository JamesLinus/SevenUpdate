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
    partial class RestoreHidden
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RestoreHidden));
            this.lblHeading = new System.Windows.Forms.Label();
            this.pnlButtonSection = new System.Windows.Forms.Panel();
            this.lblSelected = new System.Windows.Forms.Label();
            this.btnRestore = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.listView = new System.Windows.Forms.ListView();
            this.columnUpdate = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnImportance = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chkSelectAll = new System.Windows.Forms.CheckBox();
            this.sidebar = new SevenUpdate.Controls.Sidebar2();
            this.pnlButtonSection.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblHeading
            // 
            resources.ApplyResources(this.lblHeading, "lblHeading");
            this.lblHeading.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(51)))), ((int)(((byte)(153)))));
            this.lblHeading.Name = "lblHeading";
            // 
            // pnlButtonSection
            // 
            this.pnlButtonSection.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.pnlButtonSection.Controls.Add(this.lblSelected);
            this.pnlButtonSection.Controls.Add(this.btnRestore);
            this.pnlButtonSection.Controls.Add(this.btnCancel);
            resources.ApplyResources(this.pnlButtonSection, "pnlButtonSection");
            this.pnlButtonSection.Name = "pnlButtonSection";
            // 
            // lblSelected
            // 
            resources.ApplyResources(this.lblSelected, "lblSelected");
            this.lblSelected.Name = "lblSelected";
            // 
            // btnRestore
            // 
            resources.ApplyResources(this.btnRestore, "btnRestore");
            this.btnRestore.Name = "btnRestore";
            this.btnRestore.UseVisualStyleBackColor = true;
            this.btnRestore.Click += new System.EventHandler(this.btnRestore_Click);
            // 
            // btnCancel
            // 
            resources.ApplyResources(this.btnCancel, "btnCancel");
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // listView
            // 
            resources.ApplyResources(this.listView, "listView");
            this.listView.BackColor = System.Drawing.Color.White;
            this.listView.CheckBoxes = true;
            this.listView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnUpdate,
            this.columnName,
            this.columnImportance});
            this.listView.ForeColor = System.Drawing.SystemColors.WindowText;
            this.listView.FullRowSelect = true;
            this.listView.MultiSelect = false;
            this.listView.Name = "listView";
            this.listView.UseCompatibleStateImageBehavior = false;
            this.listView.View = System.Windows.Forms.View.Details;
            this.listView.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.listView_ItemCheck);
            this.listView.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler(this.listView_ItemChecked);
            this.listView.SelectedIndexChanged += new System.EventHandler(this.listView_SelectedIndexChanged);
            // 
            // columnUpdate
            // 
            resources.ApplyResources(this.columnUpdate, "columnUpdate");
            // 
            // columnName
            // 
            resources.ApplyResources(this.columnName, "columnName");
            // 
            // columnImportance
            // 
            resources.ApplyResources(this.columnImportance, "columnImportance");
            // 
            // chkSelectAll
            // 
            resources.ApplyResources(this.chkSelectAll, "chkSelectAll");
            this.chkSelectAll.BackColor = System.Drawing.Color.Transparent;
            this.chkSelectAll.Name = "chkSelectAll";
            this.chkSelectAll.UseVisualStyleBackColor = false;
            this.chkSelectAll.CheckedChanged += new System.EventHandler(this.chkSelectAll_CheckedChanged);
            // 
            // sidebar
            // 
            resources.ApplyResources(this.sidebar, "sidebar");
            this.sidebar.BackColor = System.Drawing.Color.White;
            this.sidebar.Name = "sidebar";
            // 
            // RestoreHidden
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.chkSelectAll);
            this.Controls.Add(this.listView);
            this.Controls.Add(this.lblHeading);
            this.Controls.Add(this.pnlButtonSection);
            this.Controls.Add(this.sidebar);
            this.MinimumSize = new System.Drawing.Size(832, 552);
            this.Name = "RestoreHidden";
            resources.ApplyResources(this, "$this");
            this.Load += new System.EventHandler(this.RestoreHidden_Load);
            this.pnlButtonSection.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        System.Windows.Forms.Label lblHeading;
        System.Windows.Forms.Panel pnlButtonSection;
        System.Windows.Forms.Button btnCancel;
        System.Windows.Forms.ColumnHeader columnUpdate;
        System.Windows.Forms.ColumnHeader columnName;
        System.Windows.Forms.ColumnHeader columnImportance;
        System.Windows.Forms.ListView listView;
        private SevenUpdate.Controls.Sidebar2 sidebar;
        private System.Windows.Forms.CheckBox chkSelectAll;
        private System.Windows.Forms.Button btnRestore;
        private System.Windows.Forms.Label lblSelected;
    }
}
