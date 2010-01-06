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
    sealed partial class Registry
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Registry));
            this.lbRegistry = new System.Windows.Forms.ListBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtValueName = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txtKeyPath = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtValueData = new System.Windows.Forms.TextBox();
            this.rbAddUpd = new System.Windows.Forms.RadioButton();
            this.rbDelKey = new System.Windows.Forms.RadioButton();
            this.gbAction = new System.Windows.Forms.GroupBox();
            this.rbDelValue = new System.Windows.Forms.RadioButton();
            this.cbDataType = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.cbHives = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.btnSave = new System.Windows.Forms.Button();
            this.cmsMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tmiAddRegItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.tmiDeleteRegItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tmiImportRegFile = new System.Windows.Forms.ToolStripMenuItem();
            this.gbAction.SuspendLayout();
            this.cmsMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // lbRegistry
            // 
            resources.ApplyResources(this.lbRegistry, "lbRegistry");
            this.lbRegistry.FormattingEnabled = true;
            this.lbRegistry.Name = "lbRegistry";
            this.lbRegistry.SelectedIndexChanged += new System.EventHandler(this.lbRegistry_SelectedIndexChanged);
            this.lbRegistry.KeyDown += new System.Windows.Forms.KeyEventHandler(this.lbRegistry_KeyDown);
            this.lbRegistry.MouseDown += new System.Windows.Forms.MouseEventHandler(this.lbRegistry_MouseDown);
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // txtValueName
            // 
            resources.ApplyResources(this.txtValueName, "txtValueName");
            this.txtValueName.Name = "txtValueName";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // txtKeyPath
            // 
            resources.ApplyResources(this.txtKeyPath, "txtKeyPath");
            this.txtKeyPath.Name = "txtKeyPath";
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // txtValueData
            // 
            resources.ApplyResources(this.txtValueData, "txtValueData");
            this.txtValueData.Name = "txtValueData";
            // 
            // rbAddUpd
            // 
            resources.ApplyResources(this.rbAddUpd, "rbAddUpd");
            this.rbAddUpd.Checked = true;
            this.rbAddUpd.Name = "rbAddUpd";
            this.rbAddUpd.TabStop = true;
            this.rbAddUpd.UseVisualStyleBackColor = true;
            // 
            // rbDelKey
            // 
            resources.ApplyResources(this.rbDelKey, "rbDelKey");
            this.rbDelKey.Name = "rbDelKey";
            this.rbDelKey.TabStop = true;
            this.rbDelKey.UseVisualStyleBackColor = true;
            // 
            // gbAction
            // 
            this.gbAction.Controls.Add(this.rbDelValue);
            this.gbAction.Controls.Add(this.rbAddUpd);
            this.gbAction.Controls.Add(this.rbDelKey);
            resources.ApplyResources(this.gbAction, "gbAction");
            this.gbAction.Name = "gbAction";
            this.gbAction.TabStop = false;
            // 
            // rbDelValue
            // 
            resources.ApplyResources(this.rbDelValue, "rbDelValue");
            this.rbDelValue.Name = "rbDelValue";
            this.rbDelValue.TabStop = true;
            this.rbDelValue.UseVisualStyleBackColor = true;
            // 
            // cbDataType
            // 
            this.cbDataType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbDataType.FormattingEnabled = true;
            this.cbDataType.Items.AddRange(new object[] {
                                                            resources.GetString("cbDataType.Items"),
                                                            resources.GetString("cbDataType.Items1"),
                                                            resources.GetString("cbDataType.Items2"),
                                                            resources.GetString("cbDataType.Items3"),
                                                            resources.GetString("cbDataType.Items4"),
                                                            resources.GetString("cbDataType.Items5")});
            resources.ApplyResources(this.cbDataType, "cbDataType");
            this.cbDataType.Name = "cbDataType";
            this.cbDataType.Sorted = true;
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // cbHives
            // 
            this.cbHives.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbHives.FormattingEnabled = true;
            this.cbHives.Items.AddRange(new object[] {
                                                         resources.GetString("cbHives.Items"),
                                                         resources.GetString("cbHives.Items1"),
                                                         resources.GetString("cbHives.Items2"),
                                                         resources.GetString("cbHives.Items3"),
                                                         resources.GetString("cbHives.Items4")});
            resources.ApplyResources(this.cbHives, "cbHives");
            this.cbHives.Name = "cbHives";
            this.cbHives.Sorted = true;
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.Name = "label5";
            // 
            // btnSave
            // 
            resources.ApplyResources(this.btnSave, "btnSave");
            this.btnSave.Name = "btnSave";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // cmsMenu
            // 
            this.cmsMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
                                                                                     this.tmiAddRegItem,
                                                                                     this.tmiImportRegFile,
                                                                                     this.toolStripSeparator1,
                                                                                     this.tmiDeleteRegItem});
            this.cmsMenu.Name = "cmsMenu";
            resources.ApplyResources(this.cmsMenu, "cmsMenu");
            // 
            // tmiAddRegItem
            // 
            resources.ApplyResources(this.tmiAddRegItem, "tmiAddRegItem");
            this.tmiAddRegItem.Name = "tmiAddRegItem";
            this.tmiAddRegItem.Click += new System.EventHandler(this.tmiAddRegItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            resources.ApplyResources(this.toolStripSeparator1, "toolStripSeparator1");
            // 
            // tmiDeleteRegItem
            // 
            this.tmiDeleteRegItem.Name = "tmiDeleteRegItem";
            resources.ApplyResources(this.tmiDeleteRegItem, "tmiDeleteRegItem");
            this.tmiDeleteRegItem.Click += new System.EventHandler(this.tmiDeleteRegItem_Click);
            // 
            // tmiImportRegFile
            // 
            resources.ApplyResources(this.tmiImportRegFile, "tmiImportRegFile");
            this.tmiImportRegFile.Name = "tmiImportRegFile";
            // 
            // Registry
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.cbHives);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.cbDataType);
            this.Controls.Add(this.gbAction);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtValueData);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtValueName);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtKeyPath);
            this.Controls.Add(this.lbRegistry);
            this.DoubleBuffered = true;
            this.Name = "Registry";
            resources.ApplyResources(this, "$this");
            this.gbAction.ResumeLayout(false);
            this.gbAction.PerformLayout();
            this.cmsMenu.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        System.Windows.Forms.ListBox lbRegistry;
        System.Windows.Forms.Label label2;
        System.Windows.Forms.TextBox txtValueName;
        System.Windows.Forms.Label label1;
        System.Windows.Forms.TextBox txtKeyPath;
        System.Windows.Forms.Label label3;
        System.Windows.Forms.TextBox txtValueData;
        System.Windows.Forms.RadioButton rbAddUpd;
        System.Windows.Forms.RadioButton rbDelKey;
        System.Windows.Forms.GroupBox gbAction;
        System.Windows.Forms.RadioButton rbDelValue;
        System.Windows.Forms.ComboBox cbDataType;
        System.Windows.Forms.Label label4;
        System.Windows.Forms.ComboBox cbHives;
        System.Windows.Forms.Label label5;
        System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.ContextMenuStrip cmsMenu;
        private System.Windows.Forms.ToolStripMenuItem tmiAddRegItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem tmiDeleteRegItem;
        private System.Windows.Forms.ToolStripMenuItem tmiImportRegFile;
    }
}


