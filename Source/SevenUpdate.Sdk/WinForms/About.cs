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

#region

using System;
using System.Diagnostics;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;

#endregion

namespace SevenUpdate.Sdk.WinForms
{

    #endregion

    public sealed partial class About : Form
    {
        /// <summary>
        /// Displays information about the program
        /// </summary>
        public About()
        {
            Font = SystemFonts.MessageBoxFont;

            InitializeComponent();

            var assembly = Assembly.GetExecutingAssembly();
            var version = assembly.GetName().Version;

            Text = Text + " " + assembly.GetName().Name;

            lblVersion.Text = version.ToString();

            lblCopyright.Text = "© " + "2007 - " + DateTime.Now.Year + " " + Program.RM.GetString("SevenSoftware");

            lblLicense.Text = assembly.GetName().Name + " " + lblLicense.Text;
        }

        #region UI Events

        #region Buttons

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        #endregion

        #region Labels

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

        private void lblLicense_Click(object sender, EventArgs e)
        {
            Process.Start("http://www.gnu.org/licenses/gpl-3.0.txt");
        }

        private void lblSupport_Click(object sender, EventArgs e)
        {
            Process.Start(lblSupport.Text);
        }

        #endregion

        private void About_Load(object sender, EventArgs e)
        {
        }

        #endregion
    }
}