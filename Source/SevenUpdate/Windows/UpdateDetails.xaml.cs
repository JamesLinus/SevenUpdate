#region GNU Public License Version 3

// Copyright 2007-2010 Robert Baker, Seven Software.
// This file is part of Seven Update.
//   
//      Seven Update is free software: you can redistribute it and/or modify
//      it under the terms of the GNU General Public License as published by
//      the Free Software Foundation, either version 3 of the License, or
//      (at your option) any later version.
//  
//      Seven Update is distributed in the hope that it will be useful,
//      but WITHOUT ANY WARRANTY; without even the implied warranty of
//      MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//      GNU General Public License for more details.
//   
//      You should have received a copy of the GNU General Public License
//      along with Seven Update.  If not, see <http://www.gnu.org/licenses/>.

#endregion

#region

using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using SevenUpdate.Base;

#endregion

namespace SevenUpdate.Windows
{
    /// <summary>
    ///   Interaction logic for Update_Details.xaml
    /// </summary>
    public sealed partial class UpdateDetails : Window
    {
        /// <summary>
        ///   Constructor for the Update Details window
        /// </summary>
        public UpdateDetails()
        {
            InitializeComponent();
        }

        /// <summary>
        ///   Shows the window and displays the update information
        /// </summary>
        /// <param name = "updateInfo">The update information to display</param>
        /// <returns><c>true</c></returns>
        internal bool? ShowDialog(Suh updateInfo)
        {
            tbUpdateName.Text = Base.Base.GetLocaleString(updateInfo.Name);
            tbUpdateType.Text = updateInfo.Importance.ToString();
            tbUpdateDescription.Text = Base.Base.GetLocaleString(updateInfo.Description);

            if (updateInfo.Status == UpdateStatus.Hidden)
            {
                tbStatusLabel.Text = App.RM.GetString("DownloadSize") + ":";
                tbStatus.Text = Base.Base.ConvertFileSize(updateInfo.Size);
            }
            else
                tbStatus.Text = updateInfo.Status + ", " + App.RM.GetString("InstalledOn") + " " + updateInfo.InstallDate;

            if (String.IsNullOrEmpty(updateInfo.InfoUrl))
            {
                tbMoreInfoUrl.Visibility = Visibility.Collapsed;
                textBlock3.Visibility = Visibility.Collapsed;
            }
            else
                tbMoreInfoUrl.Text = updateInfo.InfoUrl;
            if (String.IsNullOrEmpty(updateInfo.HelpUrl))
            {
                tbHelpUrl.Visibility = Visibility.Collapsed;
                textBlock4.Visibility = Visibility.Collapsed;
            }
            else
                tbHelpUrl.Text = updateInfo.HelpUrl;

            return ShowDialog();
        }

        /// <summary>
        ///   Launches the More Information Url
        /// </summary>
        private void MoreInfoUrl_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (tbMoreInfoUrl.Text != null)
                Process.Start(tbMoreInfoUrl.Text);
        }

        /// <summary>
        ///   Launches the Help Url
        /// </summary>
        private void HelpUrl_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (tbHelpUrl.Text != null)
                Process.Start(tbHelpUrl.Text);
        }
    }
}