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
    sealed partial class UpdateMenu
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UpdateMenu));
            this.lbUpdates = new System.Windows.Forms.ListBox();
            this.cmMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.removeUpdateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.label1 = new System.Windows.Forms.Label();
            this.clNewUpdate = new WindowsUI.CommandLink();
            this.clEditUpdate = new WindowsUI.CommandLink();
            this.clSave = new WindowsUI.CommandLink();
            this.cmMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // lbUpdates
            // 
            resources.ApplyResources(this.lbUpdates, "lbUpdates");
            this.lbUpdates.ContextMenuStrip = this.cmMenu;
            this.lbUpdates.FormattingEnabled = true;
            this.lbUpdates.Name = "lbUpdates";
            this.lbUpdates.SelectedIndexChanged += new System.EventHandler(this.Updates_SelectedIndexChanged);
            // 
            // cmMenu
            // 
            this.cmMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.removeUpdateToolStripMenuItem});
            this.cmMenu.Name = "contextMenuStrip1";
            resources.ApplyResources(this.cmMenu, "cmMenu");
            // 
            // removeUpdateToolStripMenuItem
            // 
            this.removeUpdateToolStripMenuItem.Name = "removeUpdateToolStripMenuItem";
            resources.ApplyResources(this.removeUpdateToolStripMenuItem, "removeUpdateToolStripMenuItem");
            this.removeUpdateToolStripMenuItem.Click += new System.EventHandler(this.RemoveUpdate_Click);
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // clNewUpdate
            // 
            this.clNewUpdate.BackColor = System.Drawing.Color.White;
            this.clNewUpdate.Description = "This will create a new update";
            resources.ApplyResources(this.clNewUpdate, "clNewUpdate");
            this.clNewUpdate.Name = "clNewUpdate";
            this.clNewUpdate.TabStop = false;
            this.clNewUpdate.UseVisualStyleBackColor = true;
            // 
            // clEditUpdate
            // 
            this.clEditUpdate.BackColor = System.Drawing.Color.White;
            this.clEditUpdate.Description = "This will allow you to edit a previously created update";
            resources.ApplyResources(this.clEditUpdate, "clEditUpdate");
            this.clEditUpdate.Name = "clEditUpdate";
            this.clEditUpdate.TabStop = false;
            this.clEditUpdate.UseVisualStyleBackColor = true;
            // 
            // clSave
            // 
            this.clSave.BackColor = System.Drawing.Color.White;
            this.clSave.Description = "Save your changes";
            resources.ApplyResources(this.clSave, "clSave");
            this.clSave.Name = "clSave";
            this.clSave.TabStop = false;
            this.clSave.UseVisualStyleBackColor = true;
            // 
            // UpdateMenu
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.clSave);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lbUpdates);
            this.Controls.Add(this.clNewUpdate);
            this.Controls.Add(this.clEditUpdate);
            this.DoubleBuffered = true;
            this.Name = "UpdateMenu";
            resources.ApplyResources(this, "$this");
            this.cmMenu.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        System.Windows.Forms.Label label1;
        System.Windows.Forms.ListBox lbUpdates;
        internal WindowsUI.CommandLink clNewUpdate;
        internal WindowsUI.CommandLink clEditUpdate;
        private System.Windows.Forms.ContextMenuStrip cmMenu;
        private System.Windows.Forms.ToolStripMenuItem removeUpdateToolStripMenuItem;
        internal WindowsUI.CommandLink clSave;

    }
}


