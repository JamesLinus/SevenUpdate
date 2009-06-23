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
    partial class Sidebar2
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Sidebar2));
            this.lblUpdateTitle = new System.Windows.Forms.Label();
            this.lblPublished = new System.Windows.Forms.Label();
            this.lblPublishedDate = new System.Windows.Forms.Label();
            this.lblUrlHelp = new System.Windows.Forms.Label();
            this.lblUrlInfo = new System.Windows.Forms.Label();
            this.lblUpdateDescription = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lblUpdateTitle
            // 
            this.lblUpdateTitle.AutoEllipsis = true;
            this.lblUpdateTitle.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.lblUpdateTitle, "lblUpdateTitle");
            this.lblUpdateTitle.Name = "lblUpdateTitle";
            // 
            // lblPublished
            // 
            resources.ApplyResources(this.lblPublished, "lblPublished");
            this.lblPublished.BackColor = System.Drawing.Color.Transparent;
            this.lblPublished.Name = "lblPublished";
            // 
            // lblPublishedDate
            // 
            resources.ApplyResources(this.lblPublishedDate, "lblPublishedDate");
            this.lblPublishedDate.BackColor = System.Drawing.Color.Transparent;
            this.lblPublishedDate.Name = "lblPublishedDate";
            // 
            // lblUrlHelp
            // 
            resources.ApplyResources(this.lblUrlHelp, "lblUrlHelp");
            this.lblUrlHelp.BackColor = System.Drawing.Color.Transparent;
            this.lblUrlHelp.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lblUrlHelp.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(102)))), ((int)(((byte)(204)))));
            this.lblUrlHelp.Name = "lblUrlHelp";
            this.lblUrlHelp.TabStop = true;
            this.lblUrlHelp.Click += new System.EventHandler(this.lblUrlHelp_Click);
            // 
            // lblUrlInfo
            // 
            resources.ApplyResources(this.lblUrlInfo, "lblUrlInfo");
            this.lblUrlInfo.BackColor = System.Drawing.Color.Transparent;
            this.lblUrlInfo.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lblUrlInfo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(102)))), ((int)(((byte)(204)))));
            this.lblUrlInfo.Name = "lblUrlInfo";
            this.lblUrlInfo.TabStop = true;
            this.lblUrlInfo.Click += new System.EventHandler(this.lblUrlInfo_Click);
            // 
            // lblUpdateDescription
            // 
            resources.ApplyResources(this.lblUpdateDescription, "lblUpdateDescription");
            this.lblUpdateDescription.AutoEllipsis = true;
            this.lblUpdateDescription.BackColor = System.Drawing.Color.Transparent;
            this.lblUpdateDescription.Name = "lblUpdateDescription";
            // 
            // Sidebar2
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.White;
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.lblUrlHelp);
            this.Controls.Add(this.lblUrlInfo);
            this.Controls.Add(this.lblPublishedDate);
            this.Controls.Add(this.lblPublished);
            this.Controls.Add(this.lblUpdateDescription);
            this.Controls.Add(this.lblUpdateTitle);
            this.DoubleBuffered = true;
            this.Name = "Sidebar2";
            this.Load += new System.EventHandler(this.Sidebar2_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblUpdateTitle;
        private System.Windows.Forms.Label lblPublished;
        private System.Windows.Forms.Label lblPublishedDate;
        private System.Windows.Forms.Label lblUrlHelp;
        private System.Windows.Forms.Label lblUrlInfo;
        private System.Windows.Forms.Label lblUpdateDescription;

    }
}
