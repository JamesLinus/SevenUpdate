#region GNU Public License v3

// Copyright 2007, 2008 Robert Baker, aka Seven ALive.
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
//     along with Seven Update.  If not, see <http://www.gnu.org/licenses/>.

#endregion

#region

using System.Diagnostics;
using System.Windows;
using System.Windows.Input;

#endregion

namespace SevenUpdate.Windows
{
    /// <summary>
    /// Interaction logic for Update_Details.xaml
    /// </summary>
    public partial class UpdateDetails : Window
    {
        public UpdateDetails()
        {
            InitializeComponent();
        }

        internal bool? ShowDialog(SUH updateInfo)
        {
            tbUpdateName.Text = Shared.GetLocaleString(updateInfo.Name);
            tbUpdateType.Text = updateInfo.Importance.ToString();
            tbUpdateDescription.Text = Shared.GetLocaleString(updateInfo.Description);

            if (updateInfo.Status == UpdateStatus.Hidden)
            {
                tbStatusLabel.Text = App.RM.GetString("DownloadSize") + ":";
                tbStatus.Text = Shared.ConvertFileSize(updateInfo.Size);
            }
            else
                tbStatus.Text = updateInfo.Status + " " + App.RM.GetString("On") + " " + updateInfo.InstallDate;
            if (updateInfo.InfoUrl != null)
                tbMoreInfoURL.Text = updateInfo.InfoUrl;
            if (updateInfo.HelpUrl != null)
                tbHelpURL.Text = updateInfo.HelpUrl;

            return ShowDialog();
        }

        private void tbMoreInfoURL_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (tbMoreInfoURL.Text != null)
                Process.Start(tbMoreInfoURL.Text);
        }

        private void tbHelpURL_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (tbHelpURL.Text != null)
                Process.Start(tbHelpURL.Text);
        }
    }
}