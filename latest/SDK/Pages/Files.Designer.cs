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
    partial class Files
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Files));
            this.lbFiles = new System.Windows.Forms.ListBox();
            this.cmsMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tmiAddFiles = new System.Windows.Forms.ToolStripMenuItem();
            this.tmiAddFolder = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.tmiDeleteFile = new System.Windows.Forms.ToolStripMenuItem();
            this.txtDownloadLoc = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtFileLoc = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtArgs = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.lblGetHash = new System.Windows.Forms.Label();
            this.dlgOpenFile = new WindowsUI.OpenFileDialog();
            this.label5 = new System.Windows.Forms.Label();
            this.lblBrowse = new System.Windows.Forms.Label();
            this.btnSave = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.cbFileAction = new System.Windows.Forms.ComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.txtSize = new SevenUpdate.NumericTextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.dlgOpenFolder = new WindowsUI.FolderBrowserDialog();
            this.cmsMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // lbFiles
            // 
            resources.ApplyResources(this.lbFiles, "lbFiles");
            this.lbFiles.FormattingEnabled = true;
            this.lbFiles.Name = "lbFiles";
            this.lbFiles.SelectedIndexChanged += new System.EventHandler(this.lbFiles_SelectedIndexChanged);
            this.lbFiles.KeyDown += new System.Windows.Forms.KeyEventHandler(this.lbFiles_KeyDown);
            this.lbFiles.MouseDown += new System.Windows.Forms.MouseEventHandler(this.lbFiles_MouseDown);
            // 
            // cmsMenu
            // 
            this.cmsMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tmiAddFiles,
            this.tmiAddFolder,
            this.toolStripSeparator1,
            this.tmiDeleteFile});
            this.cmsMenu.Name = "cmsMenu";
            resources.ApplyResources(this.cmsMenu, "cmsMenu");
            // 
            // tmiAddFiles
            // 
            this.tmiAddFiles.Name = "tmiAddFiles";
            resources.ApplyResources(this.tmiAddFiles, "tmiAddFiles");
            this.tmiAddFiles.Click += new System.EventHandler(this.tmiAddFiles_Click);
            // 
            // tmiAddFolder
            // 
            this.tmiAddFolder.Name = "tmiAddFolder";
            resources.ApplyResources(this.tmiAddFolder, "tmiAddFolder");
            this.tmiAddFolder.Click += new System.EventHandler(this.tmiAddFolder_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            resources.ApplyResources(this.toolStripSeparator1, "toolStripSeparator1");
            // 
            // tmiDeleteFile
            // 
            this.tmiDeleteFile.Name = "tmiDeleteFile";
            resources.ApplyResources(this.tmiDeleteFile, "tmiDeleteFile");
            this.tmiDeleteFile.Click += new System.EventHandler(this.tmiDeleteFile_Click);
            // 
            // txtDownloadLoc
            // 
            resources.ApplyResources(this.txtDownloadLoc, "txtDownloadLoc");
            this.txtDownloadLoc.Name = "txtDownloadLoc";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // txtFileLoc
            // 
            resources.ApplyResources(this.txtFileLoc, "txtFileLoc");
            this.txtFileLoc.Name = "txtFileLoc";
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // txtArgs
            // 
            resources.ApplyResources(this.txtArgs, "txtArgs");
            this.txtArgs.Name = "txtArgs";
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // lblGetHash
            // 
            this.lblGetHash.BackColor = System.Drawing.Color.White;
            this.lblGetHash.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lblGetHash.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(102)))), ((int)(((byte)(204)))));
            resources.ApplyResources(this.lblGetHash, "lblGetHash");
            this.lblGetHash.Name = "lblGetHash";
            this.lblGetHash.TabStop = true;
            this.lblGetHash.UseMnemonic = false;
            this.lblGetHash.Click += new System.EventHandler(this.lblGetHash_Click);
            this.lblGetHash.MouseEnter += new System.EventHandler(this.Label_MouseEnter);
            this.lblGetHash.MouseLeave += new System.EventHandler(this.Label_MouseLeave);
            // 
            // dlgOpenFile
            // 
            this.dlgOpenFile.Filter = null;
            this.dlgOpenFile.Multiselect = true;
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.Name = "label5";
            // 
            // lblBrowse
            // 
            resources.ApplyResources(this.lblBrowse, "lblBrowse");
            this.lblBrowse.BackColor = System.Drawing.Color.White;
            this.lblBrowse.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lblBrowse.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(102)))), ((int)(((byte)(204)))));
            this.lblBrowse.Name = "lblBrowse";
            this.lblBrowse.TabStop = true;
            this.lblBrowse.Click += new System.EventHandler(this.lblBrowse_Click);
            this.lblBrowse.MouseEnter += new System.EventHandler(this.Label_MouseEnter);
            this.lblBrowse.MouseLeave += new System.EventHandler(this.Label_MouseLeave);
            // 
            // btnSave
            // 
            resources.ApplyResources(this.btnSave, "btnSave");
            this.btnSave.Name = "btnSave";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // label6
            // 
            resources.ApplyResources(this.label6, "label6");
            this.label6.Name = "label6";
            // 
            // cbFileAction
            // 
            resources.ApplyResources(this.cbFileAction, "cbFileAction");
            this.cbFileAction.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbFileAction.FormattingEnabled = true;
            this.cbFileAction.Items.AddRange(new object[] {
            resources.GetString("cbFileAction.Items"),
            resources.GetString("cbFileAction.Items1"),
            resources.GetString("cbFileAction.Items2"),
            resources.GetString("cbFileAction.Items3"),
            resources.GetString("cbFileAction.Items4"),
            resources.GetString("cbFileAction.Items5")});
            this.cbFileAction.Name = "cbFileAction";
            this.cbFileAction.SelectedIndexChanged += new System.EventHandler(this.cbFileAction_SelectedIndexChanged);
            // 
            // label7
            // 
            resources.ApplyResources(this.label7, "label7");
            this.label7.Name = "label7";
            // 
            // label8
            // 
            resources.ApplyResources(this.label8, "label8");
            this.label8.Name = "label8";
            // 
            // txtSize
            // 
            this.txtSize.AllowSpace = false;
            resources.ApplyResources(this.txtSize, "txtSize");
            this.txtSize.Name = "txtSize";
            // 
            // label9
            // 
            resources.ApplyResources(this.label9, "label9");
            this.label9.Name = "label9";
            // 
            // dlgOpenFolder
            // 
            this.dlgOpenFolder.ShowNewFolderButton = false;
            // 
            // Files
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.txtSize);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.cbFileAction);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.lblBrowse);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.lblGetHash);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtArgs);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtFileLoc);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtDownloadLoc);
            this.Controls.Add(this.lbFiles);
            this.DoubleBuffered = true;
            this.Name = "Files";
            resources.ApplyResources(this, "$this");
            this.cmsMenu.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

         System.Windows.Forms.ListBox lbFiles;
         System.Windows.Forms.TextBox txtDownloadLoc;
         System.Windows.Forms.Label label1;
         System.Windows.Forms.Label label2;
         System.Windows.Forms.TextBox txtFileLoc;
         System.Windows.Forms.Label label3;
         System.Windows.Forms.TextBox txtArgs;
         System.Windows.Forms.Label label4;
         WindowsUI.OpenFileDialog dlgOpenFile;
         System.Windows.Forms.Label label5;
         System.Windows.Forms.Button btnSave;
         System.Windows.Forms.Label label6;
         System.Windows.Forms.Label lblGetHash;
         System.Windows.Forms.Label lblBrowse;
        internal System.Windows.Forms.ComboBox cbFileAction;
         System.Windows.Forms.Label label7;
         System.Windows.Forms.Label label8;
         NumericTextBox txtSize;
         private System.Windows.Forms.Label label9;
         private System.Windows.Forms.ContextMenuStrip cmsMenu;
         private System.Windows.Forms.ToolStripMenuItem tmiAddFiles;
         private System.Windows.Forms.ToolStripMenuItem tmiAddFolder;
         private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
         private System.Windows.Forms.ToolStripMenuItem tmiDeleteFile;
         private WindowsUI.FolderBrowserDialog dlgOpenFolder;
    }
}