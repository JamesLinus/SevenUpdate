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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Windows.Controls;
using Microsoft.Windows.Dialogs;


#endregion

namespace SevenUpdate.Sdk.Pages
{
    /// <summary>
    ///   Interaction logic for AppInfo.xaml
    /// </summary>
    public sealed partial class AppInfo : Page
    {
        /// <summary>
        ///   The constructor for the AppInfo page
        /// </summary>
        public AppInfo()
        {
            InitializeComponent();
        }

        private void Browse_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var cfd = new CommonOpenFileDialog { IsFolderPicker = true, Multiselect = false };
            if (cfd.ShowDialog() == CommonFileDialogResult.OK)
                tbxAppLocation.Text = Base.Base.ConvertPath(cfd.FileName, false, Convert.ToBoolean(cxbIs64Bit.IsChecked));
        }

        private void FileSystem_Checked(object sender, RoutedEventArgs e)
        {
            if (lblValue == null)
                return;
            lblRegistry.Visibility = Visibility.Collapsed;
            lblValue.Visibility = Visibility.Collapsed;
            tbxValueName.Visibility = Visibility.Collapsed;
            tbBrowse.Visibility = Visibility.Visible;
        }

        private void Registry_Checked(object sender, RoutedEventArgs e)
        {
            if (lblValue == null)
                return;
            lblRegistry.Visibility = Visibility.Visible;
            lblValue.Visibility = Visibility.Visible;
            tbxValueName.Visibility = Visibility.Visible;
            tbBrowse.Visibility = Visibility.Collapsed;
            
        }

        private void AppLocation_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (((bool)rbtnRegistry.IsChecked))
            {
                if (tbxAppLocation.Text.StartsWith(@"HKLM\") || tbxAppLocation.Text.StartsWith(@"HKCR\") || tbxAppLocation.Text.StartsWith(@"HKCU\") || tbxAppLocation.Text.StartsWith(@"HKU\") ||
                    tbxAppLocation.Text.StartsWith(@"HKEY_CLASSES_ROOT\") || tbxAppLocation.Text.StartsWith(@"HKEY_CURRENT_USER\") || tbxAppLocation.Text.StartsWith(@"HKEY_LOCAL_MACHINE\") ||
                    tbxAppLocation.Text.StartsWith(@"HKEY_USERS\"))
                {
                    imgAppPath.Visibility = Visibility.Collapsed;
                }
                else
                {
                    imgAppPath.Visibility = Visibility.Visible;
                }
            }
            else
            {
                imgAppPath.Visibility = !App.IsValidFilePath(tbxAppLocation.Text, (bool) cxbIs64Bit.IsChecked) ? Visibility.Visible : Visibility.Collapsed;
            }

        }

        private void Textbox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var source = e.Source as InfoTextBox;

            try
            {
                if (source.Text.Length > 0)
                    new Uri(source.Text);
                switch (source.Tag.ToString())
                {
                    case "publisherurl":
                        imgPublisherUrl.Visibility = Visibility.Collapsed;
                        break;

                    case "helpurl":
                        imgHelpUrl.Visibility = Visibility.Collapsed;
                        break;
                }
            }
            catch
            {
                imgPublisherUrl.Visibility = Visibility.Collapsed;
                imgHelpUrl.Visibility = Visibility.Collapsed;

                switch (source.Tag.ToString())
                {
                    case "publisherurl":
                        imgPublisherUrl.Visibility = Visibility.Visible;
                        break;

                    case "helpurl":
                        imgHelpUrl.Visibility = Visibility.Visible;
                        break;
                }
            }
        }
    }
}