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
namespace SevenUpdate.SDK
{
    partial class UpdateInfo
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UpdateInfo));
            this.txtUpdateTitle = new System.Windows.Forms.TextBox();
            this.lblTitle = new System.Windows.Forms.Label();
            this.dlgOpenFile = new WindowsUI.OpenFileDialog();
            this.label2 = new System.Windows.Forms.Label();
            this.txtInfoURL = new System.Windows.Forms.TextBox();
            this.lblInformation = new System.Windows.Forms.Label();
            this.txtUpdateInfo = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.dtReleaseDate = new System.Windows.Forms.DateTimePicker();
            this.cbUpdateType = new System.Windows.Forms.ComboBox();
            this.lblType = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.txtDownloadURL = new System.Windows.Forms.TextBox();
            this.txtLicenseURL = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // txtUpdateTitle
            // 
            resources.ApplyResources(this.txtUpdateTitle, "txtUpdateTitle");
            this.txtUpdateTitle.Name = "txtUpdateTitle";
            // 
            // lblTitle
            // 
            resources.ApplyResources(this.lblTitle, "lblTitle");
            this.lblTitle.Name = "lblTitle";
            // 
            // dlgOpenFile
            // 
            this.dlgOpenFile.Filter = null;
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // txtInfoURL
            // 
            resources.ApplyResources(this.txtInfoURL, "txtInfoURL");
            this.txtInfoURL.Name = "txtInfoURL";
            // 
            // lblInformation
            // 
            resources.ApplyResources(this.lblInformation, "lblInformation");
            this.lblInformation.Name = "lblInformation";
            // 
            // txtUpdateInfo
            // 
            resources.ApplyResources(this.txtUpdateInfo, "txtUpdateInfo");
            this.txtUpdateInfo.Name = "txtUpdateInfo";
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // dtReleaseDate
            // 
            resources.ApplyResources(this.dtReleaseDate, "dtReleaseDate");
            this.dtReleaseDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtReleaseDate.MaxDate = new System.DateTime(2020, 12, 31, 0, 0, 0, 0);
            this.dtReleaseDate.MinDate = new System.DateTime(2001, 1, 1, 0, 0, 0, 0);
            this.dtReleaseDate.Name = "dtReleaseDate";
            this.dtReleaseDate.Value = new System.DateTime(2009, 7, 1, 0, 0, 0, 0);
            // 
            // cbUpdateType
            // 
            this.cbUpdateType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbUpdateType.FormattingEnabled = true;
            this.cbUpdateType.Items.AddRange(new object[] {
            resources.GetString("cbUpdateType.Items"),
            resources.GetString("cbUpdateType.Items1"),
            resources.GetString("cbUpdateType.Items2"),
            resources.GetString("cbUpdateType.Items3")});
            resources.ApplyResources(this.cbUpdateType, "cbUpdateType");
            this.cbUpdateType.Name = "cbUpdateType";
            // 
            // lblType
            // 
            resources.ApplyResources(this.lblType, "lblType");
            this.lblType.Name = "lblType";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.Name = "label5";
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // txtDownloadURL
            // 
            resources.ApplyResources(this.txtDownloadURL, "txtDownloadURL");
            this.txtDownloadURL.Name = "txtDownloadURL";
            // 
            // txtLicenseURL
            // 
            resources.ApplyResources(this.txtLicenseURL, "txtLicenseURL");
            this.txtLicenseURL.Name = "txtLicenseURL";
            // 
            // UpdateInfo
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtDownloadURL);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtLicenseURL);
            this.Controls.Add(this.lblType);
            this.Controls.Add(this.cbUpdateType);
            this.Controls.Add(this.dtReleaseDate);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.lblInformation);
            this.Controls.Add(this.txtUpdateInfo);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtInfoURL);
            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.txtUpdateTitle);
            this.DoubleBuffered = true;
            this.Name = "UpdateInfo";
            resources.ApplyResources(this, "$this");
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

         WindowsUI.OpenFileDialog dlgOpenFile;
         System.Windows.Forms.Label label2;
         System.Windows.Forms.Label lblInformation;
         System.Windows.Forms.Label label4;
         System.Windows.Forms.Label lblType;
         internal System.Windows.Forms.TextBox txtUpdateTitle;
         System.Windows.Forms.Label label1;
         System.Windows.Forms.Label label5;
         internal System.Windows.Forms.TextBox txtInfoURL;
         internal System.Windows.Forms.TextBox txtUpdateInfo;
         internal System.Windows.Forms.DateTimePicker dtReleaseDate;
         internal System.Windows.Forms.ComboBox cbUpdateType;
         internal System.Windows.Forms.Label lblTitle;
         private System.Windows.Forms.Label label3;
         internal System.Windows.Forms.TextBox txtDownloadURL;
         internal System.Windows.Forms.TextBox txtLicenseURL;
    }
}
