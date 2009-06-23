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
using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace SevenUpdate.Controls
{
    public partial class Sidebar2 : UserControl
    {
        #region Global Variables

        /// <summary>
        /// The more information URL
        /// </summary>
        string info;

        /// <summary>
        /// The help and support URL
        /// </summary>
        string help;

        #endregion
        public Sidebar2()
        {
            this.Font = SystemFonts.MessageBoxFont;

            InitializeComponent();

            lblPublished.Font = new Font(lblPublished.Font, FontStyle.Bold);

            lblUpdateTitle.Font = new Font(lblUpdateTitle.Font, FontStyle.Bold);
        }

        #region UI Methods

        internal void DisplayUpdateInfo(string updateTitle, string publishedDate, string updateDescripton, string infoUrl, string helpUrl)
        {
            lblUpdateTitle.Text = updateTitle;

            lblPublishedDate.Text = publishedDate;

            lblUpdateDescription.Text = updateDescripton;

            help = helpUrl;

            info = infoUrl;

            ShowText();
        }

        internal void HideText()
        {
            lblPublished.Visible = false;

            lblPublishedDate.Visible = false;

            lblUpdateDescription.Visible = false;

            lblUpdateTitle.Visible = false;

            lblUrlHelp.Visible = false;

            lblUrlInfo.Visible = false;
        }

        internal void ShowText()
        {
            lblPublished.Visible = true;

            lblPublishedDate.Visible = true;

            lblUpdateDescription.Visible = true;

            lblUpdateTitle.Visible = true;

            lblUrlHelp.Visible = true;

            lblUrlInfo.Visible = true;
        }


        #endregion

        #region UI Events

        private void lblUrlInfo_Click(object sender, EventArgs e)
        {
            try
            {
                Process.Start(info);
            }
            catch (Exception) { }
        }

        private void lblUrlHelp_Click(object sender, EventArgs e)
        {
            try
            {
                Process.Start(help);
            }
            catch (Exception) { }
        }

        private void Sidebar2_Load(object sender, EventArgs e)
        {
            HideText();
        }

        #endregion
    }
}
