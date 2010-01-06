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
namespace SevenUpdate.Sdk.WinForms
{
    sealed partial class About
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(About));
            this.btnClose = new System.Windows.Forms.Button();
            this.lblVersion = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.lblCopyright = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.lblSupport = new System.Windows.Forms.Label();
            this.lblLicense = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btnClose
            // 
            resources.ApplyResources(this.btnClose, "btnClose");
            this.btnClose.Name = "btnClose";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // lblVersion
            // 
            resources.ApplyResources(this.lblVersion, "lblVersion");
            this.lblVersion.Name = "lblVersion";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // lblCopyright
            // 
            resources.ApplyResources(this.lblCopyright, "lblCopyright");
            this.lblCopyright.Name = "lblCopyright";
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // lblSupport
            // 
            resources.ApplyResources(this.lblSupport, "lblSupport");
            this.lblSupport.BackColor = System.Drawing.Color.White;
            this.lblSupport.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lblSupport.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(102)))), ((int)(((byte)(204)))));
            this.lblSupport.Name = "lblSupport";
            this.lblSupport.TabStop = true;
            this.lblSupport.Click += new System.EventHandler(this.lblSupport_Click);
            this.lblSupport.MouseEnter += new System.EventHandler(this.Label_MouseEnter);
            this.lblSupport.MouseLeave += new System.EventHandler(this.Label_MouseLeave);
            // 
            // lblLicense
            // 
            resources.ApplyResources(this.lblLicense, "lblLicense");
            this.lblLicense.BackColor = System.Drawing.Color.White;
            this.lblLicense.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lblLicense.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(102)))), ((int)(((byte)(204)))));
            this.lblLicense.Name = "lblLicense";
            this.lblLicense.TabStop = true;
            this.lblLicense.Click += new System.EventHandler(this.lblLicense_Click);
            this.lblLicense.MouseEnter += new System.EventHandler(this.Label_MouseEnter);
            this.lblLicense.MouseLeave += new System.EventHandler(this.Label_MouseLeave);
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // About
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.White;
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.label2);
            this.Controls.Add(this.lblLicense);
            this.Controls.Add(this.lblSupport);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.lblCopyright);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lblVersion);
            this.Controls.Add(this.btnClose);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "About";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Load += new System.EventHandler(this.About_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        System.Windows.Forms.Button btnClose;
        System.Windows.Forms.Label lblVersion;
        System.Windows.Forms.Label label1;
        System.Windows.Forms.Label label3;
        System.Windows.Forms.Label lblCopyright;
        System.Windows.Forms.Label label4;
        System.Windows.Forms.Label lblSupport;
        System.Windows.Forms.Label lblLicense;
        private System.Windows.Forms.Label label2;
    }
}