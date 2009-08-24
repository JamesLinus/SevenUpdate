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
using System.Drawing;
using System.Windows.Forms;
using System.Collections.ObjectModel;

namespace SevenUpdate.SDK
{
    public partial class AppInfo : UserControl
    {

        /// <summary>
        /// The application directory for the SUI
        /// </summary>
        internal static string ApplicationDirectory { get; set; }

        /// <summary>
        /// Application information
        /// </summary>
        public AppInfo()
        {
            this.Font = System.Drawing.SystemFonts.MessageBoxFont;
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
            txtAppName.Text = null;
            txtPublisher.Text = null;
            txtPublisherURL.Text = null;
            txtValueName.Text = null;
        }

        /// <summary>
        /// Loads the information
        /// </summary>
        internal void LoadInfo()
        {
            if (SUSDK.application.Directory != null)
            {
                if (SUSDK.application.Directory.StartsWith("HKEY"))
                {
                    char[] split = { '|' };
                    int slash = SUSDK.application.Directory.IndexOf(@"\", StringComparison.OrdinalIgnoreCase);
                    txtAppDir.Text = SUSDK.application.Directory.Substring(slash + 1);
                    txtAppDir.Text = txtAppDir.Text.Split(split)[0];
                    txtValueName.Text = txtAppDir.Text.Split(split)[1];
                    cbLoc.SelectedItem = SUSDK.application.Directory.Substring(0, slash);
                    txtValueName.Visible = true;
                    lblValue.Visible = true;
                    lblValidate.Visible = false;
                    lblBrowse.Visible = false;
                }
                else
                {
                    txtAppDir.Text = SUSDK.application.Directory;
                    cbLoc.SelectedIndex = 0;
                    txtValueName.Visible = false;
                    lblValue.Visible = false;
                    lblValidate.Visible = true;
                    lblBrowse.Visible = true;
                }
                chk64Bit.Checked = SUSDK.application.Is64Bit;
                txtAppName.Text = SUSDK.application.Name[0].Value;
                txtHelpURL.Text = SUSDK.application.HelpUrl;
                txtPublisher.Text = SUSDK.application.Publisher[0].Value;
                txtPublisherURL.Text = SUSDK.application.PublisherUrl;
            }
        }

        /// <summary>
        /// Saves the application information
        /// </summary>
        internal void SaveInfo()
        {
            ApplicationDirectory = txtAppDir.Text;
            if (cbLoc.SelectedIndex == 0)
                SUSDK.application.Directory = txtAppDir.Text;
            else
            {
                if (txtAppDir.Text.Substring(0, 1) == "\\")
                    SUSDK.application.Directory = cbLoc.SelectedItem + txtAppDir.Text;
                else
                    SUSDK.application.Directory = cbLoc.SelectedItem + "\\" + txtAppDir.Text + "|" + txtValueName.Text;

            }
            if (txtAppDir.Text.Substring(txtAppDir.Text.Length - 1, 1) == "\\")
                txtAppDir.Text = txtAppDir.Text.Substring(0, txtAppDir.Text.Length - 1);
            SUSDK.application.Is64Bit = chk64Bit.Checked;
            LocaleString ls = new LocaleString();
            ls.lang = "en";
            ls.Value = txtAppName.Text;
            Collection<LocaleString> appName = new Collection<LocaleString>();
            appName.Add(ls);
            SUSDK.application.Name = appName;
            SUSDK.application.HelpUrl = txtHelpURL.Text;
            Collection<LocaleString> publisher = new Collection<LocaleString>();
            ls.Value = txtPublisher.Text;
            publisher.Add(ls);
            SUSDK.application.Publisher = publisher;
            SUSDK.application.PublisherUrl = txtPublisherURL.Text;

        }

        #endregion

        #region UI Events

        #region Labels

        void lblBrowse_Click(object sender, EventArgs e)
        {
            if (dlgFolder.ShowDialog() == DialogResult.OK)
            {
                txtAppDir.Text = Shared.ConvertPath(dlgFolder.SelectedPath, false, chk64Bit.Checked);
            }
        }

        void lblValidate_Click(object sender, EventArgs e)
        {
            txtAppDir.Text = Shared.ConvertPath(txtAppDir.Text, false,chk64Bit.Checked);
        }

        void Label_MouseEnter(object sender, System.EventArgs e)
         {
             Label label = ((Label)sender);
             label.ForeColor = Color.FromArgb(51, 153, 255);
             label.Font = new Font(label.Font, FontStyle.Underline);
         }

        void Label_MouseLeave(object sender, System.EventArgs e)
         {
             Label label = ((Label)sender);
             label.ForeColor = Color.FromArgb(0, 102, 204);
             label.Font = new Font(label.Font, FontStyle.Regular);
         }

        #endregion

        void cbLoc_SelectedIndexChanged(object sender, EventArgs e)
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

        void chk64Bit_CheckedChanged(object sender, EventArgs e)
         {
             Main.Is64Bit = chk64Bit.Checked;
         }

        #endregion
    }
}