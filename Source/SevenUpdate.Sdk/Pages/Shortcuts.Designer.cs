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
    sealed partial class Shortcuts
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Shortcuts));
            this.lblValidateIcon = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.txtIcon = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtDescription = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtTarget = new System.Windows.Forms.TextBox();
            this.lbShortcuts = new System.Windows.Forms.ListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txtArgs = new System.Windows.Forms.TextBox();
            this.lblValidateTarget = new System.Windows.Forms.Label();
            this.lblBrowseTarget = new System.Windows.Forms.Label();
            this.lblBrowseIcon = new System.Windows.Forms.Label();
            this.lblBrowseLoc = new System.Windows.Forms.Label();
            this.lblValidateShortcutLoc = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.txtLoc = new System.Windows.Forms.TextBox();
            this.btnSave = new System.Windows.Forms.Button();
            this.dlgOpenFile = new WindowsUI.OpenFileDialog();
            this.cmsMenu = new System.Windows.Forms.ContextMenuStrip();
            this.tmiAddShortcut = new System.Windows.Forms.ToolStripMenuItem();
            this.tmiImportShortcut = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.tmiDeleteShortcut = new System.Windows.Forms.ToolStripMenuItem();
            this.label7 = new System.Windows.Forms.Label();
            this.cbShortcutAction = new System.Windows.Forms.ComboBox();
            this.cmsMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblValidateIcon
            // 
            resources.ApplyResources(this.lblValidateIcon, "lblValidateIcon");
            this.lblValidateIcon.BackColor = System.Drawing.Color.White;
            this.lblValidateIcon.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lblValidateIcon.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(102)))), ((int)(((byte)(204)))));
            this.lblValidateIcon.Name = "lblValidateIcon";
            this.lblValidateIcon.TabStop = true;
            this.lblValidateIcon.Click += new System.EventHandler(this.lblValidateIcon_Click);
            this.lblValidateIcon.MouseEnter += new System.EventHandler(this.Label_MouseEnter);
            this.lblValidateIcon.MouseLeave += new System.EventHandler(this.Label_MouseLeave);
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.Name = "label5";
            // 
            // txtIcon
            // 
            resources.ApplyResources(this.txtIcon, "txtIcon");
            this.txtIcon.Name = "txtIcon";
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
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
            // txtTarget
            // 
            resources.ApplyResources(this.txtTarget, "txtTarget");
            this.txtTarget.Name = "txtTarget";
            // 
            // lbShortcuts
            // 
            resources.ApplyResources(this.lbShortcuts, "lbShortcuts");
            this.lbShortcuts.FormattingEnabled = true;
            this.lbShortcuts.Name = "lbShortcuts";
            this.lbShortcuts.SelectedIndexChanged += new System.EventHandler(this.lbShortcuts_SelectedIndexChanged);
            this.lbShortcuts.KeyDown += new System.Windows.Forms.KeyEventHandler(this.lbShortcuts_KeyDown);
            this.lbShortcuts.MouseDown += new System.Windows.Forms.MouseEventHandler(this.lbShortcuts_MouseDown);
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // txtArgs
            // 
            resources.ApplyResources(this.txtArgs, "txtArgs");
            this.txtArgs.Name = "txtArgs";
            // 
            // lblValidateTarget
            // 
            resources.ApplyResources(this.lblValidateTarget, "lblValidateTarget");
            this.lblValidateTarget.BackColor = System.Drawing.Color.White;
            this.lblValidateTarget.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lblValidateTarget.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(102)))), ((int)(((byte)(204)))));
            this.lblValidateTarget.Name = "lblValidateTarget";
            this.lblValidateTarget.TabStop = true;
            this.lblValidateTarget.Click += new System.EventHandler(this.lblValidateTarget_Click);
            this.lblValidateTarget.MouseEnter += new System.EventHandler(this.Label_MouseEnter);
            this.lblValidateTarget.MouseLeave += new System.EventHandler(this.Label_MouseLeave);
            // 
            // lblBrowseTarget
            // 
            resources.ApplyResources(this.lblBrowseTarget, "lblBrowseTarget");
            this.lblBrowseTarget.BackColor = System.Drawing.Color.White;
            this.lblBrowseTarget.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lblBrowseTarget.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(102)))), ((int)(((byte)(204)))));
            this.lblBrowseTarget.Name = "lblBrowseTarget";
            this.lblBrowseTarget.TabStop = true;
            this.lblBrowseTarget.Click += new System.EventHandler(this.lblBrowseTarget_Click);
            this.lblBrowseTarget.MouseEnter += new System.EventHandler(this.Label_MouseEnter);
            this.lblBrowseTarget.MouseLeave += new System.EventHandler(this.Label_MouseLeave);
            // 
            // lblBrowseIcon
            // 
            resources.ApplyResources(this.lblBrowseIcon, "lblBrowseIcon");
            this.lblBrowseIcon.BackColor = System.Drawing.Color.White;
            this.lblBrowseIcon.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lblBrowseIcon.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(102)))), ((int)(((byte)(204)))));
            this.lblBrowseIcon.Name = "lblBrowseIcon";
            this.lblBrowseIcon.TabStop = true;
            this.lblBrowseIcon.Click += new System.EventHandler(this.lblBrowseIcon_Click);
            this.lblBrowseIcon.MouseEnter += new System.EventHandler(this.Label_MouseEnter);
            this.lblBrowseIcon.MouseLeave += new System.EventHandler(this.Label_MouseLeave);
            // 
            // lblBrowseLoc
            // 
            resources.ApplyResources(this.lblBrowseLoc, "lblBrowseLoc");
            this.lblBrowseLoc.BackColor = System.Drawing.Color.White;
            this.lblBrowseLoc.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lblBrowseLoc.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(102)))), ((int)(((byte)(204)))));
            this.lblBrowseLoc.Name = "lblBrowseLoc";
            this.lblBrowseLoc.TabStop = true;
            this.lblBrowseLoc.Click += new System.EventHandler(this.lblBrowseLoc_Click);
            this.lblBrowseLoc.MouseEnter += new System.EventHandler(this.Label_MouseEnter);
            this.lblBrowseLoc.MouseLeave += new System.EventHandler(this.Label_MouseLeave);
            // 
            // lblValidateShortcutLoc
            // 
            resources.ApplyResources(this.lblValidateShortcutLoc, "lblValidateShortcutLoc");
            this.lblValidateShortcutLoc.BackColor = System.Drawing.Color.White;
            this.lblValidateShortcutLoc.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lblValidateShortcutLoc.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(102)))), ((int)(((byte)(204)))));
            this.lblValidateShortcutLoc.Name = "lblValidateShortcutLoc";
            this.lblValidateShortcutLoc.TabStop = true;
            this.lblValidateShortcutLoc.Click += new System.EventHandler(this.lblValidateShortcutLoc_Click);
            this.lblValidateShortcutLoc.MouseEnter += new System.EventHandler(this.Label_MouseEnter);
            this.lblValidateShortcutLoc.MouseLeave += new System.EventHandler(this.Label_MouseLeave);
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // txtLoc
            // 
            resources.ApplyResources(this.txtLoc, "txtLoc");
            this.txtLoc.Name = "txtLoc";
            // 
            // btnSave
            // 
            resources.ApplyResources(this.btnSave, "btnSave");
            this.btnSave.Name = "btnSave";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // dlgOpenFile
            // 
            resources.ApplyResources(this.dlgOpenFile, "dlgOpenFile");
            // 
            // cmsMenu
            // 
            this.cmsMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tmiAddShortcut,
            this.tmiImportShortcut,
            this.toolStripSeparator1,
            this.tmiDeleteShortcut});
            this.cmsMenu.Name = "cmsMenu";
            resources.ApplyResources(this.cmsMenu, "cmsMenu");
            // 
            // tmiAddShortcut
            // 
            resources.ApplyResources(this.tmiAddShortcut, "tmiAddShortcut");
            this.tmiAddShortcut.Name = "tmiAddShortcut";
            this.tmiAddShortcut.Click += new System.EventHandler(this.tmiAddShortcut_Click);
            // 
            // tmiImportShortcut
            // 
            resources.ApplyResources(this.tmiImportShortcut, "tmiImportShortcut");
            this.tmiImportShortcut.Name = "tmiImportShortcut";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            resources.ApplyResources(this.toolStripSeparator1, "toolStripSeparator1");
            // 
            // tmiDeleteShortcut
            // 
            this.tmiDeleteShortcut.Name = "tmiDeleteShortcut";
            resources.ApplyResources(this.tmiDeleteShortcut, "tmiDeleteShortcut");
            this.tmiDeleteShortcut.Click += new System.EventHandler(this.tmiDeleteShortcut_Click);
            // 
            // label7
            // 
            resources.ApplyResources(this.label7, "label7");
            this.label7.Name = "label7";
            // 
            // cbShortcutAction
            // 
            resources.ApplyResources(this.cbShortcutAction, "cbShortcutAction");
            this.cbShortcutAction.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbShortcutAction.FormattingEnabled = true;
            this.cbShortcutAction.Items.AddRange(new object[] {
            resources.GetString("cbShortcutAction.Items"),
            resources.GetString("cbShortcutAction.Items1"),
            resources.GetString("cbShortcutAction.Items2")});
            this.cbShortcutAction.Name = "cbShortcutAction";
            this.cbShortcutAction.SelectedIndexChanged += new System.EventHandler(this.cbShortcutAction_SelectedIndexChanged);
            // 
            // Shortcuts
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.label7);
            this.Controls.Add(this.cbShortcutAction);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.lbShortcuts);
            this.Controls.Add(this.txtArgs);
            this.Controls.Add(this.txtDescription);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.lblBrowseLoc);
            this.Controls.Add(this.lblValidateShortcutLoc);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.txtLoc);
            this.Controls.Add(this.lblBrowseIcon);
            this.Controls.Add(this.lblBrowseTarget);
            this.Controls.Add(this.lblValidateTarget);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtTarget);
            this.Controls.Add(this.lblValidateIcon);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.txtIcon);
            this.Controls.Add(this.label3);
            this.DoubleBuffered = true;
            this.Name = "Shortcuts";
            resources.ApplyResources(this, "$this");
            this.cmsMenu.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        System.Windows.Forms.Label label5;
        System.Windows.Forms.TextBox txtIcon;
        System.Windows.Forms.Label label3;
        System.Windows.Forms.TextBox txtDescription;
        System.Windows.Forms.Label label2;
        System.Windows.Forms.TextBox txtTarget;
        System.Windows.Forms.ListBox lbShortcuts;
        System.Windows.Forms.Label label1;
        System.Windows.Forms.TextBox txtArgs;
        System.Windows.Forms.Label label4;
        System.Windows.Forms.TextBox txtLoc;
        System.Windows.Forms.Button btnSave;
        System.Windows.Forms.Label lblValidateIcon;
        System.Windows.Forms.Label lblValidateTarget;
        System.Windows.Forms.Label lblBrowseTarget;
        System.Windows.Forms.Label lblBrowseIcon;
        System.Windows.Forms.Label lblBrowseLoc;
        System.Windows.Forms.Label lblValidateShortcutLoc;
        WindowsUI.OpenFileDialog dlgOpenFile;
        private System.Windows.Forms.ContextMenuStrip cmsMenu;
        private System.Windows.Forms.ToolStripMenuItem tmiAddShortcut;
        private System.Windows.Forms.ToolStripMenuItem tmiImportShortcut;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem tmiDeleteShortcut;
        private System.Windows.Forms.Label label7;
        internal System.Windows.Forms.ComboBox cbShortcutAction;
    }
}


