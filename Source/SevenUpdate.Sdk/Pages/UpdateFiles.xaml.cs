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
        #region Properties

        private bool IsInfoValid { get { return (imgDownloadUrl.Visibility != Visibility.Visible && imgInstallUri.Visibility != Visibility.Visible); } }

        #endregion

        #region Constructors

        /// <summary>
        ///   The constructor for the UpdateFiles page
        /// </summary>
        public UpdateFiles()
        {
            InitializeComponent();

            if (Environment.OSVersion.Version.Major < 6)
                return;

            MouseLeftButtonDown += App.Rectangle_MouseLeftButtonDown;
            AeroGlass.DwmCompositionChangedEventHandler += AeroGlass_DwmCompositionChangedEventHandler;
            line.Visibility = AeroGlass.IsEnabled ? Visibility.Collapsed : Visibility.Visible;
            rectangle.Visibility = AeroGlass.IsEnabled ? Visibility.Collapsed : Visibility.Visible;
        }

        #endregion

        #region UI Events

        #region TextBox - Text Changed

        private void Textbox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var source = e.Source as InfoTextBox;
            string path = SevenUpdate.Base.Base.ConvertPath(source.Text, true, true);
            try
            {
                if (source.Text.Length > 0)
                    new Uri(path);
                switch (source.Name)
                {
                    case "tbxDownloadUrl":
                        if (path.Length > 2)
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

        #endregion

        #region TextBox - Lost Keyboard Focus

        #endregion

        #region RadioButton - Checked

        #endregion

        #region Button - Click

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (IsInfoValid)
                MainWindow.NavService.Navigate(new Uri(@"Pages\UpdateRegistry.xaml", UriKind.Relative));
            else
                App.ShowInputErrorMessage();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.NavService.Navigate(new Uri(@"Pages\Main.xaml", UriKind.Relative));
        }

        #endregion

        #region TextBlock - Mouse Down

        private void Browse_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var cfd = new CommonOpenFileDialog {IsFolderPicker = true, Multiselect = false};
            if (cfd.ShowDialog() == CommonFileDialogResult.OK)
                tbxInstallUri.Text = SevenUpdate.Base.Base.ConvertPath(cfd.FileName, false, true);
        }

        private void Hash_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var cfd = new CommonOpenFileDialog {Multiselect = false};
            if (cfd.ShowDialog() == CommonFileDialogResult.OK)
                tbHash.Text = SevenUpdate.Base.Base.GetHash(cfd.FileName);
        }

        #endregion

        #region MenuItem - Click

        private void miRemove_Click(object sender, RoutedEventArgs e)
        {
            listBox.Items.Remove(listBox.SelectedItem);
        }

        private void miRemoveAll_Click(object sender, RoutedEventArgs e)
        {
            listBox.Items.Clear();
            miRemoveAll.IsEnabled = false;
            miRemove.IsEnabled = false;
            spInput.Visibility = Visibility.Collapsed;
        }

        private void AddFile_Click(object sender, RoutedEventArgs e)
        {
            var cfd = new CommonOpenFileDialog {Multiselect = false};
            if (cfd.ShowDialog() == CommonFileDialogResult.OK)
                AddFile(cfd.FileName);
        }

        private void AddFolder_Click(object sender, RoutedEventArgs e)
        {
            var cfd = new CommonOpenFileDialog {Multiselect = false, IsFolderPicker = true};
            if (cfd.ShowDialog() == CommonFileDialogResult.OK)
                AddFiles(Directory.GetFiles(cfd.FileName));
        }

        #endregion

        #region ComboBox - Selection Changed

        private void UpdateType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (tbxDownloadUrl == null)
                return;
            if (cbxUpdateType.SelectedIndex != 4 && cbxUpdateType.SelectedIndex != 6 && cbxUpdateType.SelectedIndex != 8)
            {
                tbxDownloadUrl.IsEnabled = true;
                tbxArgs.IsEnabled = true;
            }
            else
            {
                tbxDownloadUrl.Text = null;
                tbxDownloadUrl.IsEnabled = false;
                tbxArgs.IsEnabled = false;
                tbxArgs.Text = null;
            }
        }

        private void listBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listBox.Items.Count > 0)
            {
                spHelp.Visibility = Visibility.Collapsed;
                spInput.Visibility = Visibility.Visible;
                miRemoveAll.IsEnabled = true;
                miRemove.IsEnabled = listBox.SelectedIndex > -1;
            }
            else
            {
                spHelp.Visibility = Visibility.Visible;
                spInput.Visibility = Visibility.Collapsed;
                miRemoveAll.IsEnabled = false;
                miRemove.IsEnabled = false;
            }
        }

        #endregion

        #region Aero

        private void AeroGlass_DwmCompositionChangedEventHandler(object sender, AeroGlass.DwmCompositionChangedEventArgs e)
        {
            line.Visibility = e.IsGlassEnabled ? Visibility.Collapsed : Visibility.Visible;
            rectangle.Visibility = e.IsGlassEnabled ? Visibility.Collapsed : Visibility.Visible;
        }

        #endregion

        #endregion

        #region Methods

        private void AddFile(string fileName)
        {
            spHelp.Visibility = Visibility.Collapsed;
            spInput.Visibility = Visibility.Visible;
            tbxInstallUri.Text = SevenUpdate.Base.Base.ConvertPath(fileName, false, true);
            tbHash.Text = SevenUpdate.Base.Base.GetHash(fileName);
            cbxUpdateType.SelectedIndex = 0;
            listBox.Items.Add(Path.GetFileName(fileName));
            listBox.SelectedIndex = (listBox.Items.Count - 1);
        }

        private void AddFiles(string[] files)
        {
            for (int x = 0; x < files.Length; x++)
                AddFile(files[x]);
            listBox.SelectedIndex = (listBox.Items.Count - 1);
        }

        #endregion
    }
}