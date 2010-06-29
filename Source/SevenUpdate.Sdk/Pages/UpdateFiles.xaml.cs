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
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Windows.Controls;
using Microsoft.Windows.Dialogs;

#endregion

namespace SevenUpdate.Sdk.Pages
{
    /// <summary>
    ///   Interaction logic for UpdateFiles.xaml
    /// </summary>
    public sealed partial class UpdateFiles : Page
    {
        /// <summary>
        ///   The constructor for the UpdateFiles page
        /// </summary>
        public UpdateFiles()
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
                switch (source.Name)
                {
                    case "tbxDownloadUrl":
                        imgDownloadUrl.Visibility = Visibility.Collapsed;
                        break;

                    case "tbxInstallUri":
                        if (source.Text.Length > 2 && Path.GetFileName(source.Text).ContainsAny(Path.GetInvalidPathChars()) == false)
                            imgInstallUri.Visibility = Visibility.Collapsed;
                        break;
                }
            }
            catch
            {

                switch (source.Name)
                {
                    case "tbxDownloadUrl":
                        imgDownloadUrl.Visibility = Visibility.Visible;
                        break;

                    case "tbxInstallUri":
                        imgInstallUri.Visibility = Visibility.Visible;
                        break;
                }
            }
        }

        private void Browse_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var cfd = new CommonOpenFileDialog {IsFolderPicker = true, Multiselect = false};
            if (cfd.ShowDialog() == CommonFileDialogResult.OK)
                tbxInstallUri.Text = Base.Base.ConvertPath(cfd.FileName, false, true);
        }
    }
}