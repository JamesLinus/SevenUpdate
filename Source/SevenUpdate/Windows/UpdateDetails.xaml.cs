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

using System.Diagnostics;
using System.Windows.Input;

#endregion

namespace SevenUpdate.Windows
{
    /// <summary>
    ///   Interaction logic for Update_Details.xaml
    /// </summary>
    public sealed partial class UpdateDetails
    {
        private string helpUrl, infoUrl;

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
        internal void ShowDialog(Suh updateInfo)
        {
            DataContext = updateInfo;
            helpUrl = updateInfo.HelpUrl;
            infoUrl = updateInfo.InfoUrl;
            if (updateInfo.Status == UpdateStatus.Hidden)
            {
                tbStatusLabel.Text = Properties.Resources.DownloadSize + ":";
                tbStatus.Text = Base.ConvertFileSize(updateInfo.UpdateSize);
            }
            else
                tbStatus.Text = updateInfo.Status == UpdateStatus.Failed
                                    ? Properties.Resources.Failed
                                    : Properties.Resources.Successful + ", " + Properties.Resources.InstalledOn + " " + updateInfo.InstallDate;


            ShowDialog();
            return;
        }

        /// <summary>
        ///   Launches the More Information Url
        /// </summary>
        private void MoreInfoUrl_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Process.Start(infoUrl);
        }

        /// <summary>
        ///   Launches the Help Url
        /// </summary>
        private void HelpUrl_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Process.Start(helpUrl);
        }
    }
}