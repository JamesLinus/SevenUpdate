#region GNU Public License v3

// Copyright 2007-2010 Robert Baker, aka Seven ALive.
// This file is part of Seven Update.
//  
//     Seven Update is free software: you can redistribute it and/or modify
//     it under the terms of the GNU General Public License as published by
//     the Free Software Foundation, either version 3 of the License, or
//     (at your option) any later version.
// 
//     Seven Update is distributed in the hope that it will be useful,
//     but WITHOUT ANY WARRANTY; without even the implied warranty of
//     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//     GNU General Public License for more details.
//  
//    You should have received a copy of the GNU General Public License
//    along with Seven Update.  If not, see <http://www.gnu.org/licenses/>.

#endregion

#region

using System;
using System.Drawing;
using System.Windows.Forms;

#endregion

namespace SevenUpdate.Sdk.Pages
{
    public sealed partial class UpdateInfo : UserControl
    {
        public UpdateInfo()
        {
            Font = SystemFonts.MessageBoxFont;

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

        internal static string DownloadDirectory { get; set; }
    }
}