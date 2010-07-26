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
using Microsoft.Windows.Dwm;
using SevenUpdate.Sdk.Windows;

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
            if (!AeroGlass.IsEnabled)
                return;
            line.Visibility = Visibility.Collapsed;
            rectangle.Visibility = Visibility.Collapsed;
            MouseLeftButtonDown += App.Rectangle_MouseLeftButtonDown;
        }

        private void Textbox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var source = e.Source as InfoTextBox;
            string path = Base.Base.ConvertPath(source.Text, true, true);
            try
            {
                if (source.Text.Length > 0)
                    new Uri(path);
                switch (source.Name)
                {
                    case "tbxDownloadUrl":
                        imgDownloadUrl.Visibility = Visibility.Collapsed;
                        break;

                    case "tbxInstallUri":
                        if (path.Length > 2 && Path.GetFileName(path).ContainsAny(Path.GetInvalidPathChars()) == false)
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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.NavService.Navigate(new Uri(@"Pages\UpdateRegistry.xaml", UriKind.Relative));
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.NavService.Navigate(new Uri(@"Pages\Main.xaml", UriKind.Relative));
        }

        private void Hash_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var cfd = new CommonOpenFileDialog {Multiselect = false};
            if (cfd.ShowDialog() == CommonFileDialogResult.OK)
                tbHash.Text = Base.Base.GetHash(cfd.FileName);
        }

        private void AddFile_Click(object sender, RoutedEventArgs e)
        {
            var cfd = new CommonOpenFileDialog {Multiselect = false};
            if (cfd.ShowDialog() == CommonFileDialogResult.OK)
                AddFile(cfd.FileName);
        }

        private void AddFile(string fileName)
        {
            SPHelp.Visibility = Visibility.Collapsed;
            SPInput.Visibility = Visibility.Visible;
            tbxInstallUri.Text = Base.Base.ConvertPath(fileName, false, true);
            tbHash.Text = Base.Base.GetHash(fileName);
            cbxUpdateType.SelectedIndex = 0;
            listBox.Items.Add(Path.GetFileName(fileName));
        }

        private void AddFiles(string[] files)
        {
            for (int x = 0; x < files.Length; x++)
                AddFile(files[x]);
        }

        private void AddFolder_Click(object sender, RoutedEventArgs e)
        {
            var cfd = new CommonOpenFileDialog {Multiselect = false, IsFolderPicker = true};
            if (cfd.ShowDialog() == CommonFileDialogResult.OK)
                AddFiles(Directory.GetFiles(cfd.FileName));
        }
    }
}