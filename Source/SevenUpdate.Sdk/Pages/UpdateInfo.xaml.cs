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
using Microsoft.Windows.Controls;

#endregion

namespace SevenUpdate.Sdk.Pages
{
    /// <summary>
    ///   Interaction logic for UpdateInfo.xaml
    /// </summary>
    public sealed partial class UpdateInfo : Page
    {
        /// <summary>
        ///   The constructor for the UpdateInfo page
        /// </summary>
        public UpdateInfo()
        {
            InitializeComponent();
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
                    case "eula":
                        imgLicense.Visibility = Visibility.Collapsed;
                        break;

                    case "download":
                        imgDownloadLoc.Visibility = Visibility.Collapsed;
                        break;

                    case "info":
                        imgUpdateInfo.Visibility = Visibility.Collapsed;
                        break;
                }
            }
            catch
            {
                imgLicense.Visibility = Visibility.Collapsed;
                imgUpdateInfo.Visibility = Visibility.Collapsed;
                imgDownloadLoc.Visibility = Visibility.Collapsed;

                switch (source.Tag.ToString())
                {
                    case "eula":
                        imgLicense.Visibility = Visibility.Visible;
                        break;

                    case "download":
                        imgDownloadLoc.Visibility = Visibility.Visible;
                        break;

                    case "info":
                        imgUpdateInfo.Visibility = Visibility.Visible;
                        break;
                }
            }
        }
    }
}