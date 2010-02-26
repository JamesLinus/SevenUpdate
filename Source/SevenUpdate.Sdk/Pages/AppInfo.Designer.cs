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

namespace SevenUpdate.Sdk.Pages
{
    sealed partial class AppInfo
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AppInfo));
            this.dlgOpenFile = new WindowsUI.OpenFileDialog();
            this.label2 = new System.Windows.Forms.Label();
            this.txtHelpURL = new System.Windows.Forms.TextBox();
            this.txtAppDir = new System.Windows.Forms.TextBox();
            this.lblBrowse = new System.Windows.Forms.Label();
            this.dlgFolder = new WindowsUI.FolderBrowserDialog();
            this.lblValidate = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.txtPublisher = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.txtPublisherURL = new System.Windows.Forms.TextBox();
            this.cbLoc = new System.Windows.Forms.ComboBox();
            this.lblValue = new System.Windows.Forms.Label();
            this.txtValueName = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.chkIs64Bit = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // dlgOpenFile
            // 
            resources.ApplyResources(this.dlgOpenFile, "dlgOpenFile");
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // txtHelpURL
            // 
            resources.ApplyResources(this.txtHelpURL, "txtHelpURL");
            this.txtHelpURL.Name = "txtHelpURL";
            // 
            // txtAppDir
            // 
            resources.ApplyResources(this.txtAppDir, "txtAppDir");
            this.txtAppDir.Name = "txtAppDir";
            // 
            // lblBrowse
            // 
            resources.ApplyResources(this.lblBrowse, "lblBrowse");
            this.lblBrowse.BackColor = System.Drawing.Color.White;
            this.lblBrowse.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lblBrowse.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(102)))), ((int)(((byte)(204)))));
            this.lblBrowse.Name = "lblBrowse";
            this.lblBrowse.TabStop = true;
            this.lblBrowse.Click += new System.EventHandler(this.Browse_Click);
            this.lblBrowse.MouseEnter += new System.EventHandler(this.Label_MouseEnter);
            this.lblBrowse.MouseLeave += new System.EventHandler(this.Label_MouseLeave);
            // 
            // lblValidate
            // 
            resources.ApplyResources(this.lblValidate, "lblValidate");
            this.lblValidate.BackColor = System.Drawing.Color.White;
            this.lblValidate.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lblValidate.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(102)))), ((int)(((byte)(204)))));
            this.lblValidate.Name = "lblValidate";
            this.lblValidate.TabStop = true;
            this.lblValidate.Click += new System.EventHandler(this.Validate_Click);
            this.lblValidate.MouseEnter += new System.EventHandler(this.Label_MouseEnter);
            this.lblValidate.MouseLeave += new System.EventHandler(this.Label_MouseLeave);
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // txtPublisher
            // 
            resources.ApplyResources(this.txtPublisher, "txtPublisher");
            this.txtPublisher.Name = "txtPublisher";
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.Name = "label5";
            // 
            // txtPublisherURL
            // 
            resources.ApplyResources(this.txtPublisherURL, "txtPublisherURL");
            this.txtPublisherURL.Name = "txtPublisherURL";
            // 
            // cbLoc
            // 
            this.cbLoc.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbLoc.FormattingEnabled = true;
            this.cbLoc.Items.AddRange(new object[] {
            resources.GetString("cbLoc.Items"),
            resources.GetString("cbLoc.Items1"),
            resources.GetString("cbLoc.Items2"),
            resources.GetString("cbLoc.Items3")});
            resources.ApplyResources(this.cbLoc, "cbLoc");
            this.cbLoc.Name = "cbLoc";
            this.cbLoc.Sorted = true;
            this.cbLoc.SelectedIndexChanged += new System.EventHandler(this.Loc_SelectedIndexChanged);
            // 
            // lblValue
            // 
            resources.ApplyResources(this.lblValue, "lblValue");
            this.lblValue.Name = "lblValue";
            // 
            // txtValueName
            // 
            resources.ApplyResources(this.txtValueName, "txtValueName");
            this.txtValueName.Name = "txtValueName";
            // 
            // label7
            // 
            resources.ApplyResources(this.label7, "label7");
            this.label7.Name = "label7";
            // 
            // chkIs64Bit
            // 
            resources.ApplyResources(this.chkIs64Bit, "chkIs64Bit");
            this.chkIs64Bit.Name = "chkIs64Bit";
            this.chkIs64Bit.UseVisualStyleBackColor = true;
            this.chkIs64Bit.CheckedChanged += new System.EventHandler(this.Is64Bit_CheckedChanged);
            // 
            // AppInfo
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.chkIs64Bit);
            this.Controls.Add(this.cbLoc);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.lblValue);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.txtValueName);
            this.Controls.Add(this.txtPublisher);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.lblBrowse);
            this.Controls.Add(this.lblValidate);
            this.Controls.Add(this.txtPublisherURL);
            this.Controls.Add(this.txtAppDir);
            this.Controls.Add(this.txtHelpURL);
            this.DoubleBuffered = true;
            this.Name = "AppInfo";
            resources.ApplyResources(this, "$this");
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        WindowsUI.OpenFileDialog dlgOpenFile;
        System.Windows.Forms.Label label2;
        System.Windows.Forms.TextBox txtHelpURL;
        WindowsUI.FolderBrowserDialog dlgFolder;
        System.Windows.Forms.Label lblBrowse;
        System.Windows.Forms.Label lblValidate;
        internal System.Windows.Forms.TextBox txtAppDir;
        System.Windows.Forms.Label label4;
        System.Windows.Forms.TextBox txtPublisher;
        System.Windows.Forms.Label label5;
        System.Windows.Forms.TextBox txtPublisherURL;
        private System.Windows.Forms.Label lblValue;
        private System.Windows.Forms.Label label7;
        internal System.Windows.Forms.ComboBox cbLoc;
        internal System.Windows.Forms.TextBox txtValueName;
        private System.Windows.Forms.CheckBox chkIs64Bit;
    }
}


