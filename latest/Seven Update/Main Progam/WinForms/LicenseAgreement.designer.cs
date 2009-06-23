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
    partial class LicenseAgreement
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LicenseAgreement));
            this.lblHeading = new System.Windows.Forms.Label();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.rbAccept = new System.Windows.Forms.RadioButton();
            this.rbDecline = new System.Windows.Forms.RadioButton();
            this.panel1 = new System.Windows.Forms.Panel();
            this.txtSLA = new System.Windows.Forms.RichTextBox();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblHeading
            // 
            resources.ApplyResources(this.lblHeading, "lblHeading");
            this.lblHeading.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(51)))), ((int)(((byte)(153)))));
            this.lblHeading.Name = "lblHeading";
            // 
            // btnOK
            // 
            resources.ApplyResources(this.btnOK, "btnOK");
            this.btnOK.Name = "btnOK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            resources.ApplyResources(this.btnCancel, "btnCancel");
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // rbAccept
            // 
            resources.ApplyResources(this.rbAccept, "rbAccept");
            this.rbAccept.Name = "rbAccept";
            this.rbAccept.UseVisualStyleBackColor = true;
            this.rbAccept.CheckedChanged += new System.EventHandler(this.rbAccept_CheckedChanged);
            // 
            // rbDecline
            // 
            resources.ApplyResources(this.rbDecline, "rbDecline");
            this.rbDecline.Name = "rbDecline";
            this.rbDecline.UseVisualStyleBackColor = true;
            this.rbDecline.CheckedChanged += new System.EventHandler(this.rbDecline_CheckedChanged);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.txtSLA);
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Name = "panel1";
            this.panel1.Paint += new System.Windows.Forms.PaintEventHandler(this.panel1_Paint);
            // 
            // txtSLA
            // 
            resources.ApplyResources(this.txtSLA, "txtSLA");
            this.txtSLA.BackColor = System.Drawing.Color.White;
            this.txtSLA.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtSLA.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtSLA.Name = "txtSLA";
            this.txtSLA.ReadOnly = true;
            // 
            // LicenseAgreement
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.White;
            this.CancelButton = this.btnCancel;
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.rbDecline);
            this.Controls.Add(this.rbAccept);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.lblHeading);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "LicenseAgreement";
            this.Load += new System.EventHandler(this.SLA_Load);
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        System.Windows.Forms.Label lblHeading;
        System.Windows.Forms.Button btnOK;
        System.Windows.Forms.Button btnCancel;
        System.Windows.Forms.RadioButton rbAccept;
        System.Windows.Forms.RadioButton rbDecline;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.RichTextBox txtSLA;
    }
}