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
    public sealed partial class SUAInfo : UserControl
    {
        public SUAInfo()
        {
            Font = SystemFonts.MessageBoxFont;

            InitializeComponent();

            lblHeading.Font = new Font(SystemFonts.MessageBoxFont.Name, 11.25F);

            cbLoc.SelectedIndex = 0;
        }

        #region UI Events

        #region Buttons

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Dispose();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (txtDescription.Text.Length < 1 || txtAppDir.Text.Length < 1 || txtAppName.Text.Length < 1 || txtPublisher.Text.Length < 1 || txtSUILocation.Text.Length < 1 ||
                (cbLoc.SelectedIndex > 0 && txtValueName.Text.Length < 1))
                MessageBox.Show(Program.RM.GetString("FillRequiredInformation"));
            else
            {
                if (!txtSUILocation.Text.EndsWith(".sui", StringComparison.OrdinalIgnoreCase))
                    MessageBox.Show(Program.RM.GetString("UrlSUI"));
                else
                {
                    dlgSaveFile.FileName = txtAppName.Text;

                    if (dlgSaveFile.ShowDialog() == DialogResult.OK)
                    {
                        var sua = new SUA();

                        if (cbLoc.SelectedIndex == 0)
                            sua.Directory = txtAppDir.Text;
                        else
                        {
                            if (txtAppDir.Text.Substring(0, 1) == "\\")
                                sua.Directory = cbLoc.SelectedItem + txtAppDir.Text;
                            else
                                sua.Directory = cbLoc.SelectedItem + "\\" + txtAppDir.Text + "|" + txtValueName.Text;
                        }

                        if (txtAppDir.Text.Substring(txtAppDir.Text.Length - 1, 1) == "\\")
                            txtAppDir.Text = txtAppDir.Text.Substring(0, txtAppDir.Text.Length - 1);
                        var ls = new LocaleString {Lang = "en", Value = txtAppName.Text};
                        var appName = new ObservableCollection<LocaleString> {ls};
                        sua.Name = appName;

                        var publisher = new ObservableCollection<LocaleString>();
                        ls = new LocaleString {Lang = "en", Value = txtPublisher.Text};
                        publisher.Add(ls);
                        sua.Publisher = publisher;

                        var description = new ObservableCollection<LocaleString>();
                        ls = new LocaleString {Lang = "en", Value = txtDescription.Text};
                        description.Add(ls);
                        sua.Description = description;

                        sua.Is64Bit = chk64Bit.Checked;

                        sua.Source = txtSUILocation.Text;

                        Base.Base.Serialize(sua, dlgSaveFile.FileName);

                        Dispose();
                    }
                }
            }
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

        private void SUAInfo_Load(object sender, EventArgs e)
        {
        }

        #region Labels

        private void lblBrowse_Click(object sender, EventArgs e)
        {
            if (dlgFolder.ShowDialog() == DialogResult.OK)
                txtAppDir.Text = Base.Base.ConvertPath(dlgFolder.SelectedPath, false, Main.Is64Bit);
        }

        private void lblValidate_Click(object sender, EventArgs e)
        {
            txtAppDir.Text = Base.Base.ConvertPath(txtAppDir.Text, false, Main.Is64Bit);
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

        #endregion
    }
}