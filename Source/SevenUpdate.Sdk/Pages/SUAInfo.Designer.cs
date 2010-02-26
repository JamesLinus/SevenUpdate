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
    sealed partial class SuaInfo
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SuaInfo));
            this.txtAppName = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.lblBrowse = new System.Windows.Forms.Label();
            this.dlgFolder = new WindowsUI.FolderBrowserDialog();
            this.txtPublisher = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.txtDescription = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.txtAppDir = new System.Windows.Forms.TextBox();
            this.lblValidate = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.txtValueName = new System.Windows.Forms.TextBox();
            this.lblValue = new System.Windows.Forms.Label();
            this.cbLoc = new System.Windows.Forms.ComboBox();
            this.lblHeading = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.txtSuiLocation = new System.Windows.Forms.TextBox();
            this.btnSave = new System.Windows.Forms.Button();
            this.dlgSaveFile = new WindowsUI.SaveFileDialog();
            this.btnCancel = new System.Windows.Forms.Button();
            this.chk64Bit = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // txtAppName
            // 
            resources.ApplyResources(this.txtAppName, "txtAppName");
            this.txtAppName.Name = "txtAppName";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
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
            // txtDescription
            // 
            resources.ApplyResources(this.txtDescription, "txtDescription");
            this.txtDescription.Name = "txtDescription";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // txtAppDir
            // 
            resources.ApplyResources(this.txtAppDir, "txtAppDir");
            this.txtAppDir.Name = "txtAppDir";
            // 
            // lblValidate
            // 
            this.lblValidate.BackColor = System.Drawing.Color.White;
            this.lblValidate.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lblValidate.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(102)))), ((int)(((byte)(204)))));
            resources.ApplyResources(this.lblValidate, "lblValidate");
            this.lblValidate.Name = "lblValidate";
            this.lblValidate.TabStop = true;
            this.lblValidate.Click += new System.EventHandler(this.Validate_Click);
            this.lblValidate.MouseEnter += new System.EventHandler(this.Label_MouseEnter);
            this.lblValidate.MouseLeave += new System.EventHandler(this.Label_MouseLeave);
            // 
            // label7
            // 
            resources.ApplyResources(this.label7, "label7");
            this.label7.Name = "label7";
            // 
            // txtValueName
            // 
            resources.ApplyResources(this.txtValueName, "txtValueName");
            this.txtValueName.Name = "txtValueName";
            // 
            // lblValue
            // 
            resources.ApplyResources(this.lblValue, "lblValue");
            this.lblValue.Name = "lblValue";
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
            // lblHeading
            // 
            this.lblHeading.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(51)))), ((int)(((byte)(153)))));
            resources.ApplyResources(this.lblHeading, "lblHeading");
            this.lblHeading.Name = "lblHeading";
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // label6
            // 
            resources.ApplyResources(this.label6, "label6");
            this.label6.Name = "label6";
            // 
            // txtSuiLocation
            // 
            resources.ApplyResources(this.txtSuiLocation, "txtSuiLocation");
            this.txtSuiLocation.Name = "txtSuiLocation";
            // 
            // btnSave
            // 
            resources.ApplyResources(this.btnSave, "btnSave");
            this.btnSave.Name = "btnSave";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.Save_Click);
            // 
            // dlgSaveFile
            // 
            this.dlgSaveFile.DefaultExt = "sui";
            this.dlgSaveFile.FileName = "MyApplication";
            resources.ApplyResources(this.dlgSaveFile, "dlgSaveFile");
            // 
            // btnCancel
            // 
            resources.ApplyResources(this.btnCancel, "btnCancel");
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.Cancel_Click);
            // 
            // chk64Bit
            // 
            resources.ApplyResources(this.chk64Bit, "chk64Bit");
            this.chk64Bit.Name = "chk64Bit";
            this.chk64Bit.UseVisualStyleBackColor = true;
            // 
            // SuaInfo
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.chk64Bit);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.txtSuiLocation);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.lblHeading);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtDescription);
            this.Controls.Add(this.cbLoc);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.lblValue);
            this.Controls.Add(this.txtValueName);
            this.Controls.Add(this.txtPublisher);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.lblBrowse);
            this.Controls.Add(this.lblValidate);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtAppName);
            this.Controls.Add(this.txtAppDir);
            this.DoubleBuffered = true;
            this.Name = "SuaInfo";
            resources.ApplyResources(this, "$this");
            this.Load += new System.EventHandler(this.SuaInfo_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        System.Windows.Forms.Label label1;
        WindowsUI.FolderBrowserDialog dlgFolder;
        System.Windows.Forms.Label lblBrowse;
        internal System.Windows.Forms.TextBox txtAppName;
        System.Windows.Forms.TextBox txtPublisher;
        System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtDescription;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        internal System.Windows.Forms.TextBox txtAppDir;
        private System.Windows.Forms.Label lblValidate;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox txtValueName;
        private System.Windows.Forms.Label lblValue;
        private System.Windows.Forms.ComboBox cbLoc;
        private System.Windows.Forms.Label lblHeading;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtSuiLocation;
        private System.Windows.Forms.Button btnSave;
        private WindowsUI.SaveFileDialog dlgSaveFile;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.CheckBox chk64Bit;
    }
}


