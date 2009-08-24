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
    public partial class SUAInfo : UserControl
    {
        public SUAInfo()
        {
            this.Font = SystemFonts.MessageBoxFont;

            InitializeComponent();

            lblHeading.Font = new Font(SystemFonts.MessageBoxFont.Name, 11.25F);

            cbLoc.SelectedIndex = 0;
        }

        #region UI Events

        #region Buttons

        void btnCancel_Click(object sender, EventArgs e)
        {
            Dispose();
        }

        void btnSave_Click(object sender, EventArgs e)
        {
            if (txtDescription.Text.Length < 1 || txtAppDir.Text.Length < 1 || txtAppName.Text.Length < 1 || txtPublisher.Text.Length
                < 1 || txtSUILocation.Text.Length < 1 || (cbLoc.SelectedIndex > 0 && txtValueName.Text.Length < 1))
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
                        SUA sua = new SUA();

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
                        LocaleString ls = new LocaleString();
                        ls.lang = "en";
                        ls.Value = txtAppName.Text;
                        Collection<LocaleString> appName = new Collection<LocaleString>();
                        appName.Add(ls);
                        sua.ApplicationName = appName;

                        Collection<LocaleString> publisher = new Collection<LocaleString>();
                        ls.Value = txtPublisher.Text;
                        publisher.Add(ls);
                        sua.Publisher = publisher;

                        Collection<LocaleString> description = new Collection<LocaleString>();
                        ls.Value = txtDescription.Text;
                        description.Add(ls);
                        sua.Description = description;

                        sua.Is64Bit = chk64Bit.Checked;

                        sua.Source = txtSUILocation.Text;

                        Shared.Serialize<SUA>(sua, dlgSaveFile.FileName);

                        Dispose();
                    }
                }
            }
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

        #region Labels

        void lblBrowse_Click(object sender, EventArgs e)
        {
            if (dlgFolder.ShowDialog() == DialogResult.OK)
            {
                txtAppDir.Text = Shared.ConvertPath(dlgFolder.SelectedPath, false, Main.Is64Bit);
            }
        }

        void lblValidate_Click(object sender, EventArgs e)
        {
            txtAppDir.Text = Shared.ConvertPath(txtAppDir.Text, false, Main.Is64Bit);
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

        private void SUAInfo_Load(object sender, EventArgs e)
        {

        }

        #endregion
    }
}