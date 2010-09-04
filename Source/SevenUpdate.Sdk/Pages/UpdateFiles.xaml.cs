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
using System.Collections.ObjectModel;
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

        #endregion

        #region Constructors

        /// <summary>
        ///   The constructor for the UpdateFiles page
        /// </summary>
        public UpdateFiles()
        {
            InitializeComponent();

            SevenUpdate.Base.HashGeneratedEventHandler += Base_HashGeneratedEventHandler;

            if (Base.UpdateInfo.Files == null)
                Base.UpdateInfo.Files = new ObservableCollection<UpdateFile>();

            listBox.ItemsSource = Base.UpdateInfo.Files;

            if (Environment.OSVersion.Version.Major < 6)
                return;

            MouseLeftButtonDown += App.Rectangle_MouseLeftButtonDown;
            AeroGlass.DwmCompositionChangedEventHandler += AeroGlass_DwmCompositionChangedEventHandler;
            line.Visibility = AeroGlass.IsEnabled ? Visibility.Collapsed : Visibility.Visible;
            rectangle.Visibility = AeroGlass.IsEnabled ? Visibility.Collapsed : Visibility.Visible;
        }

        #endregion

        #region Methods

        private void Base_HashGeneratedEventHandler(object sender, HashGeneratedEventArgs e)
        {
            tbHashCalculating.Visibility = e.IsHashGenerating ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        ///   Adds a file to the list
        /// </summary>
        /// <param name = "fullName">The fullpath to the file</param>
        private void AddFile(string fullName)
        {
            string installUrl = SevenUpdate.Base.ConvertPath(fullName, false, true);

            var file = new UpdateFile { Action = FileAction.UpdateIfExist, Destination = installUrl, Hash = Properties.Resources.CalculatingHash + "..."};

            Base.UpdateInfo.Files.Add(file);

            listBox.SelectedIndex = (listBox.Items.Count - 1);

            tbHashCalculating.Visibility = Visibility.Visible;
            SevenUpdate.Base.GetHashAsync(ref file);
            SevenUpdate.Base.GetFileSizeAsync(ref file);
        }

        private void AddFiles(string[] files)
        {
            piFileProgress.IsRunning = true;
            for (int x = 0; x < files.Length; x++)
                AddFile(files[x]);
            piFileProgress.IsRunning = false;
            listBox.SelectedIndex = (listBox.Items.Count - 1);
        }

        #endregion

        #region UI Events

        #region TextBox - Lost Keyboard Focus

        private void TextBox_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            var source = e.Source as InfoTextBox;
            if (source == null)
                return;

            source.Text = SevenUpdate.Base.ConvertPath(source.Text, false, Base.AppInfo.Is64Bit);
        }

        #endregion

        #region Button - Click

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //if (Base.UpdateInfo.Files.Count < 1 || SevenUpdate.Base.IsHashGenerating)
            //    App.ShowInputErrorMessage();
            //else
            MainWindow.NavService.Navigate(new Uri(@"Pages\UpdateRegistry.xaml", UriKind.Relative));
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
            if (cfd.ShowDialog(Application.Current.MainWindow) == CommonFileDialogResult.OK)
                Base.UpdateInfo.Files[listBox.SelectedIndex].Destination = SevenUpdate.Base.ConvertPath(cfd.FileName, false, true);
        }

        private void Hash_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var cfd = new CommonOpenFileDialog {Multiselect = false};
            if (cfd.ShowDialog(Application.Current.MainWindow) != CommonFileDialogResult.OK)
                return;

            var selectedItem = listBox.SelectedItem as UpdateFile;
            SevenUpdate.Base.GetFileSizeAsync(ref selectedItem, cfd.FileName);
            SevenUpdate.Base.GetHashAsync(ref selectedItem, cfd.FileName);
        }

        #endregion

        #region MenuItem - Click

        private void miRemove_Click(object sender, RoutedEventArgs e)
        {
            Base.UpdateInfo.Files.RemoveAt(listBox.SelectedIndex);
        }

        private void miRemoveAll_Click(object sender, RoutedEventArgs e)
        {
            listBox.Items.Clear();
        }

        private void AddFile_Click(object sender, RoutedEventArgs e)
        {
            var cfd = new CommonOpenFileDialog {Multiselect = false};
            if (cfd.ShowDialog(Application.Current.MainWindow) == CommonFileDialogResult.OK)
                AddFile(cfd.FileName);
        }

        private void AddFolder_Click(object sender, RoutedEventArgs e)
        {
            var cfd = new CommonOpenFileDialog {Multiselect = false, IsFolderPicker = true};
            if (cfd.ShowDialog(Application.Current.MainWindow) == CommonFileDialogResult.OK)
                AddFiles(Directory.GetFiles(cfd.FileName, "*.*", SearchOption.AllDirectories));
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
    }
}