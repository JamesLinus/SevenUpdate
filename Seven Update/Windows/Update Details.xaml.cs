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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

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

       internal bool? ShowDialog(UpdateInformation updateInfo)
        {
            tbUpdateTitle.Text = updateInfo.UpdateTitle[0].Value;
            tbUpdateType.Text = updateInfo.Importance.ToString();
            tbUpdateDescription.Text = updateInfo.Description[0].Value;

            if (updateInfo.Status == UpdateStatus.Hidden)
            {
                tbStatusLabel.Text = App.RM.GetString("DownloadSize") +":";
                tbStatus.Text = Shared.ConvertFileSize(updateInfo.Size).ToString();
            }
            else
            {
                tbStatus.Text = updateInfo.Status.ToString() + " " + App.RM.GetString("On") + " " + updateInfo.InstallDate;
            }
            if (updateInfo.InfoUrl != null)
                tbMoreInfoURL.Text = updateInfo.InfoUrl;
            if (updateInfo.HelpUrl != null)
                tbHelpURL.Text = updateInfo.HelpUrl;

           return base.ShowDialog();
        }
        void TextBlock_MouseEnter(object sender, MouseEventArgs e)
        {
            TextBlock textBlock = ((TextBlock)sender);
            textBlock.TextDecorations = TextDecorations.Underline;
        }

        void TextBlock_MouseLeave(object sender, MouseEventArgs e)
        {
            TextBlock textBlock = ((TextBlock)sender);
            textBlock.TextDecorations = null;
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
