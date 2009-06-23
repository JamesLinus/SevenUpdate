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
namespace SevenUpdate.Controls
{
    partial class InfoBar
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

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
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(InfoBar));
            this.Shield = new System.Windows.Forms.PictureBox();
            this.lblHeading = new System.Windows.Forms.Label();
            this.lblSelectedUpdates = new System.Windows.Forms.Label();
            this.btnAction = new System.Windows.Forms.Button();
            this.lblViewImportantUpdates = new System.Windows.Forms.Label();
            this.lblViewOptionalUpdates = new System.Windows.Forms.Label();
            this.lblSubStatus = new System.Windows.Forms.Label();
            this.pbProgressBar = new System.Windows.Forms.ProgressBar();
            this.lblStatus = new System.Windows.Forms.Label();
            this.verticalLine = new System.Drawing.VerticalLine();
            ((System.ComponentModel.ISupportInitialize)(this.Shield)).BeginInit();
            this.SuspendLayout();
            // 
            // Shield
            // 
            this.Shield.Image = global::SevenUpdate.Properties.Resources.yellowShield;
            resources.ApplyResources(this.Shield, "Shield");
            this.Shield.Name = "Shield";
            this.Shield.TabStop = false;
            // 
            // lblHeading
            // 
            resources.ApplyResources(this.lblHeading, "lblHeading");
            this.lblHeading.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(51)))), ((int)(((byte)(153)))));
            this.lblHeading.Name = "lblHeading";
            // 
            // lblSelectedUpdates
            // 
            resources.ApplyResources(this.lblSelectedUpdates, "lblSelectedUpdates");
            this.lblSelectedUpdates.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblSelectedUpdates.Name = "lblSelectedUpdates";
            // 
            // btnAction
            // 
            resources.ApplyResources(this.btnAction, "btnAction");
            this.btnAction.Name = "btnAction";
            this.btnAction.UseVisualStyleBackColor = true;
            // 
            // lblViewImportantUpdates
            // 
            resources.ApplyResources(this.lblViewImportantUpdates, "lblViewImportantUpdates");
            this.lblViewImportantUpdates.BackColor = System.Drawing.Color.White;
            this.lblViewImportantUpdates.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lblViewImportantUpdates.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(102)))), ((int)(((byte)(204)))));
            this.lblViewImportantUpdates.Name = "lblViewImportantUpdates";
            this.lblViewImportantUpdates.TabStop = true;
            // 
            // lblViewOptionalUpdates
            // 
            resources.ApplyResources(this.lblViewOptionalUpdates, "lblViewOptionalUpdates");
            this.lblViewOptionalUpdates.BackColor = System.Drawing.Color.White;
            this.lblViewOptionalUpdates.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lblViewOptionalUpdates.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(102)))), ((int)(((byte)(204)))));
            this.lblViewOptionalUpdates.Name = "lblViewOptionalUpdates";
            this.lblViewOptionalUpdates.TabStop = true;
            // 
            // lblSubStatus
            // 
            resources.ApplyResources(this.lblSubStatus, "lblSubStatus");
            this.lblSubStatus.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblSubStatus.Name = "lblSubStatus";
            // 
            // pbProgressBar
            // 
            resources.ApplyResources(this.pbProgressBar, "pbProgressBar");
            this.pbProgressBar.MarqueeAnimationSpeed = 50;
            this.pbProgressBar.Name = "pbProgressBar";
            this.pbProgressBar.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            // 
            // lblStatus
            // 
            resources.ApplyResources(this.lblStatus, "lblStatus");
            this.lblStatus.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblStatus.Name = "lblStatus";
            // 
            // verticalLine
            // 
            this.verticalLine.BackColor = System.Drawing.Color.Empty;
            resources.ApplyResources(this.verticalLine, "verticalLine");
            this.verticalLine.MaximumSize = new System.Drawing.Size(2, 500);
            this.verticalLine.MinimumSize = new System.Drawing.Size(2, 0);
            this.verticalLine.Name = "verticalLine";
            // 
            // InfoBar
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.White;
            this.BackgroundImage = global::SevenUpdate.Properties.Resources.yellowSide;
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.verticalLine);
            this.Controls.Add(this.lblViewImportantUpdates);
            this.Controls.Add(this.btnAction);
            this.Controls.Add(this.Shield);
            this.Controls.Add(this.lblHeading);
            this.Controls.Add(this.lblSelectedUpdates);
            this.Controls.Add(this.lblSubStatus);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.lblViewOptionalUpdates);
            this.Controls.Add(this.pbProgressBar);
            this.MaximumSize = new System.Drawing.Size(609, 109);
            this.MinimumSize = new System.Drawing.Size(609, 69);
            this.Name = "InfoBar";
            this.Load += new System.EventHandler(this.InfoBar_Load);
            ((System.ComponentModel.ISupportInitialize)(this.Shield)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox Shield;
        private System.Drawing.VerticalLine verticalLine;
        internal System.Windows.Forms.Button btnAction;
        internal System.Windows.Forms.Label lblHeading;
        internal System.Windows.Forms.Label lblSelectedUpdates;
        internal System.Windows.Forms.Label lblViewImportantUpdates;
        internal System.Windows.Forms.Label lblViewOptionalUpdates;
        private System.Windows.Forms.ProgressBar pbProgressBar;
        private System.Windows.Forms.Label lblSubStatus;
        private System.Windows.Forms.Label lblStatus;

    }
}
