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
    sealed partial class Main
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Main));
            this.UpdateBox = new System.Windows.Forms.GroupBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.picReloadNotifier = new System.Windows.Forms.PictureBox();
            this.lblHeading = new System.Windows.Forms.Label();
            this.panel = new System.Windows.Forms.Panel();
            this.lineSeparator1 = new System.Drawing.LineSeparator();
            this.lblStepA = new System.Windows.Forms.Label();
            this.lblStep3 = new System.Windows.Forms.Label();
            this.lblStepC = new System.Windows.Forms.Label();
            this.lblStepB = new System.Windows.Forms.Label();
            this.lblStep2 = new System.Windows.Forms.Label();
            this.lblStep1 = new System.Windows.Forms.Label();
            this.dlgOpenFile = new WindowsUI.OpenFileDialog();
            this.dlgSaveFile = new WindowsUI.SaveFileDialog();
            this.lblAbout = new System.Windows.Forms.Label();
            this.clSUA = new WindowsUI.CommandLink();
            this.clOpenSUI = new WindowsUI.CommandLink();
            this.clNewSUI = new WindowsUI.CommandLink();
            this.btnNext = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.lineSeparator2 = new System.Drawing.LineSeparator();
            this.pnlButtonSection = new System.Windows.Forms.Panel();
            this.UpdateBox.SuspendLayout();
            this.panel.SuspendLayout();
            this.pnlButtonSection.SuspendLayout();
            this.SuspendLayout();
            // 
            // UpdateBox
            // 
            resources.ApplyResources(this.UpdateBox, "UpdateBox");
            this.UpdateBox.BackColor = System.Drawing.Color.Transparent;
            this.UpdateBox.Controls.Add(this.textBox1);
            this.UpdateBox.Controls.Add(this.picReloadNotifier);
            this.UpdateBox.Controls.Add(this.lblHeading);
            this.UpdateBox.Name = "UpdateBox";
            this.UpdateBox.TabStop = false;
            // 
            // textBox1
            // 
            resources.ApplyResources(this.textBox1, "textBox1");
            this.textBox1.BackColor = System.Drawing.Color.White;
            this.textBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox1.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            this.textBox1.TabStop = false;
            // 
            // picReloadNotifier
            // 
            resources.ApplyResources(this.picReloadNotifier, "picReloadNotifier");
            this.picReloadNotifier.Name = "picReloadNotifier";
            this.picReloadNotifier.TabStop = false;
            // 
            // lblHeading
            // 
            resources.ApplyResources(this.lblHeading, "lblHeading");
            this.lblHeading.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(51)))), ((int)(((byte)(153)))));
            this.lblHeading.Name = "lblHeading";
            // 
            // panel
            // 
            this.panel.Controls.Add(this.lineSeparator1);
            this.panel.Controls.Add(this.lblStepA);
            this.panel.Controls.Add(this.lblStep3);
            this.panel.Controls.Add(this.lblStepC);
            this.panel.Controls.Add(this.lblStepB);
            this.panel.Controls.Add(this.lblStep2);
            this.panel.Controls.Add(this.lblStep1);
            resources.ApplyResources(this.panel, "panel");
            this.panel.Name = "panel";
            // 
            // lineSeparator1
            // 
            this.lineSeparator1.BackColor = System.Drawing.Color.Empty;
            resources.ApplyResources(this.lineSeparator1, "lineSeparator1");
            this.lineSeparator1.MaximumSize = new System.Drawing.Size(2000, 2);
            this.lineSeparator1.MinimumSize = new System.Drawing.Size(0, 2);
            this.lineSeparator1.Name = "lineSeparator1";
            // 
            // lblStepA
            // 
            resources.ApplyResources(this.lblStepA, "lblStepA");
            this.lblStepA.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.lblStepA.ForeColor = System.Drawing.SystemColors.GrayText;
            this.lblStepA.Name = "lblStepA";
            this.lblStepA.Click += new System.EventHandler(this.lblStepA_Click);
            // 
            // lblStep3
            // 
            resources.ApplyResources(this.lblStep3, "lblStep3");
            this.lblStep3.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.lblStep3.ForeColor = System.Drawing.SystemColors.GrayText;
            this.lblStep3.Name = "lblStep3";
            this.lblStep3.Click += new System.EventHandler(this.lblStep3_Click);
            // 
            // lblStepC
            // 
            resources.ApplyResources(this.lblStepC, "lblStepC");
            this.lblStepC.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.lblStepC.ForeColor = System.Drawing.SystemColors.GrayText;
            this.lblStepC.Name = "lblStepC";
            this.lblStepC.Click += new System.EventHandler(this.lblStepC_Click);
            // 
            // lblStepB
            // 
            resources.ApplyResources(this.lblStepB, "lblStepB");
            this.lblStepB.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.lblStepB.ForeColor = System.Drawing.SystemColors.GrayText;
            this.lblStepB.Name = "lblStepB";
            this.lblStepB.Click += new System.EventHandler(this.lblStepB_Click);
            // 
            // lblStep2
            // 
            resources.ApplyResources(this.lblStep2, "lblStep2");
            this.lblStep2.Cursor = System.Windows.Forms.Cursors.Default;
            this.lblStep2.ForeColor = System.Drawing.SystemColors.GrayText;
            this.lblStep2.Name = "lblStep2";
            this.lblStep2.Click += new System.EventHandler(this.lblStep2_Click);
            // 
            // lblStep1
            // 
            resources.ApplyResources(this.lblStep1, "lblStep1");
            this.lblStep1.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.lblStep1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(51)))), ((int)(((byte)(153)))));
            this.lblStep1.Name = "lblStep1";
            // 
            // dlgOpenFile
            // 
            this.dlgOpenFile.DefaultExt = "sui";
            resources.ApplyResources(this.dlgOpenFile, "dlgOpenFile");
            // 
            // dlgSaveFile
            // 
            this.dlgSaveFile.DefaultExt = "sui";
            this.dlgSaveFile.FileName = "MyApplication";
            resources.ApplyResources(this.dlgSaveFile, "dlgSaveFile");
            // 
            // lblAbout
            // 
            resources.ApplyResources(this.lblAbout, "lblAbout");
            this.lblAbout.BackColor = System.Drawing.Color.White;
            this.lblAbout.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lblAbout.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(102)))), ((int)(((byte)(204)))));
            this.lblAbout.Name = "lblAbout";
            this.lblAbout.TabStop = true;
            this.lblAbout.Click += new System.EventHandler(this.lblAbout_Click);
            this.lblAbout.MouseEnter += new System.EventHandler(this.lblAbout_MouseEnter);
            this.lblAbout.MouseLeave += new System.EventHandler(this.lblAbout_MouseLeave);
            // 
            // clSUA
            // 
            resources.ApplyResources(this.clSUA, "clSUA");
            this.clSUA.BackColor = System.Drawing.Color.White;
            this.clSUA.Description = "This file will allow users to add your Application to Seven Update";
            this.clSUA.Name = "clSUA";
            this.clSUA.TabStop = false;
            this.clSUA.UseVisualStyleBackColor = true;
            this.clSUA.Click += new System.EventHandler(this.clSUA_Click);
            // 
            // clOpenSUI
            // 
            resources.ApplyResources(this.clOpenSUI, "clOpenSUI");
            this.clOpenSUI.BackColor = System.Drawing.Color.White;
            this.clOpenSUI.Description = "Use this if you already made a SUI file for your Application";
            this.clOpenSUI.Name = "clOpenSUI";
            this.clOpenSUI.TabStop = false;
            this.clOpenSUI.UseVisualStyleBackColor = true;
            this.clOpenSUI.Click += new System.EventHandler(this.clOpenSUI_Click);
            // 
            // clNewSUI
            // 
            resources.ApplyResources(this.clNewSUI, "clNewSUI");
            this.clNewSUI.BackColor = System.Drawing.Color.White;
            this.clNewSUI.Description = "This will create a new SUI for use with Seven Update";
            this.clNewSUI.Name = "clNewSUI";
            this.clNewSUI.TabStop = false;
            this.clNewSUI.UseVisualStyleBackColor = true;
            this.clNewSUI.Click += new System.EventHandler(this.clNewSUI_Click);
            // 
            // btnNext
            // 
            resources.ApplyResources(this.btnNext, "btnNext");
            this.btnNext.Name = "btnNext";
            this.btnNext.UseVisualStyleBackColor = true;
            this.btnNext.Click += new System.EventHandler(this.btnNext_Click);
            // 
            // btnCancel
            // 
            resources.ApplyResources(this.btnCancel, "btnCancel");
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // lineSeparator2
            // 
            this.lineSeparator2.BackColor = System.Drawing.Color.Empty;
            resources.ApplyResources(this.lineSeparator2, "lineSeparator2");
            this.lineSeparator2.MaximumSize = new System.Drawing.Size(2000, 2);
            this.lineSeparator2.MinimumSize = new System.Drawing.Size(0, 2);
            this.lineSeparator2.Name = "lineSeparator2";
            // 
            // pnlButtonSection
            // 
            this.pnlButtonSection.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.pnlButtonSection.Controls.Add(this.lineSeparator2);
            this.pnlButtonSection.Controls.Add(this.btnCancel);
            this.pnlButtonSection.Controls.Add(this.btnNext);
            resources.ApplyResources(this.pnlButtonSection, "pnlButtonSection");
            this.pnlButtonSection.Name = "pnlButtonSection";
            // 
            // Main
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.White;
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.panel);
            this.Controls.Add(this.UpdateBox);
            this.Controls.Add(this.clSUA);
            this.Controls.Add(this.clOpenSUI);
            this.Controls.Add(this.clNewSUI);
            this.Controls.Add(this.lblAbout);
            this.Controls.Add(this.pnlButtonSection);
            this.DoubleBuffered = true;
            this.Name = "Main";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmMain_FormClosing);
            this.UpdateBox.ResumeLayout(false);
            this.UpdateBox.PerformLayout();
            this.panel.ResumeLayout(false);
            this.panel.PerformLayout();
            this.pnlButtonSection.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        System.Windows.Forms.PictureBox picReloadNotifier;
        System.Windows.Forms.Label lblHeading;
        WindowsUI.CommandLink clNewSUI;
        System.Windows.Forms.TextBox textBox1;
        WindowsUI.CommandLink clOpenSUI;
        System.Windows.Forms.Panel panel;
        System.Windows.Forms.Label lblStep3;
        System.Windows.Forms.Label lblStepB;
        System.Windows.Forms.Label lblStep2;
        System.Windows.Forms.Label lblStep1;
        System.Windows.Forms.Label lblStepA;
        WindowsUI.OpenFileDialog dlgOpenFile;
        WindowsUI.CommandLink clSUA;
        WindowsUI.SaveFileDialog dlgSaveFile;
        private System.Windows.Forms.Label lblAbout;
        private System.Windows.Forms.GroupBox UpdateBox;
        private System.Windows.Forms.Label lblStepC;
        private System.Drawing.LineSeparator lineSeparator1;
        private System.Windows.Forms.Button btnNext;
        private System.Windows.Forms.Button btnCancel;
        private System.Drawing.LineSeparator lineSeparator2;
        private System.Windows.Forms.Panel pnlButtonSection;
    }
}


