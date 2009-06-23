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
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System;
public partial class About : Form
{
    /// <summary>
    /// Displays information about the program
    /// </summary>
    public About()
    {
        this.Font = SystemFonts.MessageBoxFont;

        InitializeComponent();

        this.Text = this.Text + " " + Application.ProductName;

        lblBuildDate.Text = File.GetLastWriteTime(Application.ExecutablePath).ToString();

        lblVersion.Text = Application.ProductVersion;

        lblCopyright.Text = "© " + System.DateTime.Now.Year + " " + Application.CompanyName;

        lblLicense.Text = ProductName + " " + lblLicense.Text;
    }

    #region UI Events

    #region Buttons

    void btnClose_Click(object sender, System.EventArgs e)
    {
        Close();
    }

    #endregion

    #region Labels

    void Label_MouseEnter(object sender, System.EventArgs e)
    {
        Label label = ((Label)sender);

        label.ForeColor = Color.FromArgb(51, 153, 255);

        label.Font = new Font(label.Font, FontStyle.Underline);
    }

    void Label_MouseLeave(object sender, System.EventArgs e)
    {
        Label label = ((Label)sender);

        label.ForeColor = System.Drawing.Color.FromArgb(0, 102, 204);

        label.Font = new Font(label.Font, FontStyle.Regular);
    }

    void lblLicense_Click(object sender, System.EventArgs e)
    {
        if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "\\license.txt"))
            Process.Start(AppDomain.CurrentDomain.BaseDirectory + "\\license.txt");
    }

    void lblSupport_Click(object sender, System.EventArgs e)
    {
        Process.Start(lblSupport.Text);
    }

    #endregion

    #endregion
}
