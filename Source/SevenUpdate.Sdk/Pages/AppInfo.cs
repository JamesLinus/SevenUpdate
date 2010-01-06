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
using System.Collections.ObjectModel;
using System.Drawing;
using System.Windows.Forms;
using SevenUpdate.Base;
using SevenUpdate.Sdk.WinForms;

#endregion

namespace SevenUpdate.Sdk.Pages
{
    public sealed partial class AppInfo : UserControl
    {
        /// <summary>
        /// Application information
        /// </summary>
        public AppInfo()
        {
            Font = SystemFonts.MessageBoxFont;
            InitializeComponent();
            cbLoc.SelectedIndex = 0;
        }

        #region UI Methods

        /// <summary>
        /// Clears the UI
        /// </summary>
        internal void ClearUI()
        {
            cbLoc.SelectedIndex = 0;
            txtHelpURL.Text = null;
            txtAppDir.Text = null;
            txtPublisher.Text = null;
            txtPublisherURL.Text = null;
            txtValueName.Text = null;
        }

        /// <summary>
        /// Loads the information
        /// </summary>
        internal void LoadInfo()
        {
            if (SDK.Application.Directory == null)
                return;
            if (SDK.Application.Directory.StartsWith("HKEY"))
            {
                char[] split = {'|'};
                var slash = SDK.Application.Directory.IndexOf(@"\", StringComparison.OrdinalIgnoreCase);
                txtAppDir.Text = SDK.Application.Directory.Substring(slash + 1);
                txtAppDir.Text = txtAppDir.Text.Split(split)[0];
                txtValueName.Text = SDK.Application.Directory.Split(split)[1];
                cbLoc.SelectedItem = SDK.Application.Directory.Substring(0, slash + 1);
                txtValueName.Visible = true;
                lblValue.Visible = true;
                lblValidate.Visible = false;
                lblBrowse.Visible = false;
            }
            else
            {
                txtAppDir.Text = SDK.Application.Directory;
                cbLoc.SelectedIndex = 0;
                txtValueName.Visible = false;
                lblValue.Visible = false;
                lblValidate.Visible = true;
                lblBrowse.Visible = true;
            }
            chk64Bit.Checked = SDK.Application.Is64Bit;
            txtHelpURL.Text = SDK.Application.HelpUrl;
            txtPublisher.Text = SDK.Application.Publisher[0].Value;
            txtPublisherURL.Text = SDK.Application.PublisherUrl;
        }

        /// <summary>
        /// Saves the Application information
        /// </summary>
        internal void SaveInfo()
        {
            ApplicationDirectory = txtAppDir.Text;
            if (cbLoc.SelectedIndex == 0)
                SDK.Application.Directory = txtAppDir.Text;
            else
            {
                if (txtAppDir.Text.Substring(0, 1) == "\\")
                    SDK.Application.Directory.Remove(0, 1);
                SDK.Application.Directory = cbLoc.SelectedItem + txtAppDir.Text + "|" + txtValueName.Text;
            }
            if (txtAppDir.Text.Substring(txtAppDir.Text.Length - 1, 1) == "\\")
                txtAppDir.Text = txtAppDir.Text.Substring(0, txtAppDir.Text.Length - 1);
            SDK.Application.Is64Bit = chk64Bit.Checked;
            var ls = new LocaleString {Lang = "en", Value = txtPublisher.Text};
            SDK.Application.HelpUrl = txtHelpURL.Text;
            var publisher = new ObservableCollection<LocaleString> {ls};
            SDK.Application.Publisher = publisher;
            SDK.Application.PublisherUrl = txtPublisherURL.Text;
        }

        #endregion

        #region UI Events

        #region Labels

        private void lblBrowse_Click(object sender, EventArgs e)
        {
            if (dlgFolder.ShowDialog() == DialogResult.OK)
                txtAppDir.Text = Base.Base.ConvertPath(dlgFolder.SelectedPath, false, chk64Bit.Checked);
        }

        private void lblValidate_Click(object sender, EventArgs e)
        {
            txtAppDir.Text = Base.Base.ConvertPath(txtAppDir.Text, false, chk64Bit.Checked);
        }

        private void Label_MouseEnter(object sender, EventArgs e)
        {
            var label = ((Label) sender);
            label.ForeColor = Color.FromArgb(51, 153, 255);
            label.Font = new Font(label.Font, FontStyle.Underline);
        }

        private void Label_MouseLeave(object sender, EventArgs e)
        {
            var label = ((Label) sender);
            label.ForeColor = Color.FromArgb(0, 102, 204);
            label.Font = new Font(label.Font, FontStyle.Regular);
        }

        #endregion

        private void cbLoc_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbLoc.SelectedIndex > 0)
            {
                txtValueName.Visible = true;
                lblValue.Visible = true;
                lblValidate.Visible = false;
                lblBrowse.Visible = false;
            }
            else
            {
                txtValueName.Visible = false;
                lblValue.Visible = false;
                lblValidate.Visible = true;
                lblBrowse.Visible = true;
            }
        }

        private void chk64Bit_CheckedChanged(object sender, EventArgs e)
        {
            Main.Is64Bit = chk64Bit.Checked;
        }

        #endregion

        /// <summary>
        /// The Application directory for the SUI
        /// </summary>
        internal static string ApplicationDirectory { get; set; }
    }
}