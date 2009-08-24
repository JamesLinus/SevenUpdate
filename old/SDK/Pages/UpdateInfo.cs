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
using System.Windows.Forms;


namespace SevenUpdate.SDK
{
    public partial class UpdateInfo : UserControl
    {
        internal static string DownloadDirectory { get; set; }
        
        public UpdateInfo()
        {
            this.Font = System.Drawing.SystemFonts.MessageBoxFont;

            InitializeComponent();

            cbUpdateType.SelectedIndex = 0;
        }
        
        #region Methods

        /// <summary>
        /// Clears the UI
        /// </summary>
        internal void ClearUI()
        {
            txtLicenseURL.Text = null;

            txtInfoURL.Text = null;

            txtUpdateInfo.Text = null;

            txtUpdateTitle.Text = null;

            cbUpdateType.SelectedIndex = 0;

            dtReleaseDate.Text = DateTime.Now.ToShortDateString();

            txtInfoURL.Text = null;
        }

        #endregion
    }
}
