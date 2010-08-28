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
            SevenUpdate.Base.HashGeneratedEventHandler += Base_HashGeneratedEventHandler;
            line.Visibility = AeroGlass.IsEnabled ? Visibility.Collapsed : Visibility.Visible;
            rectangle.Visibility = AeroGlass.IsEnabled ? Visibility.Collapsed : Visibility.Visible;
        }

        #endregion

        #region Methods

        /// <summary>
        ///   Loads the file information into the ui
        /// </summary>
        /// <param name = "index"></param>
        private void LoadFileInfo(int index)
        {
            tbxInstallUri.Text = Base.Update.Files[index].Destination;
            tbxDownloadUrl.Text = Base.Update.Files[index].Source;
            tbxArgs.Text = Base.Update.Files[index].Args;
            tbHash.Text = Base.Update.Files[index].Hash;
            cbxUpdateType.SelectedIndex = (int) Base.Update.Files[index].Action;
        }

        /// <summary>
        ///   Adds a file to the list
        /// </summary>
        /// <param name = "fullName">The fullpath to the file</param>
        private void AddFile(string fullName)
        {
            string filename = Path.GetFileName(fullName);

            string installUrl = SevenUpdate.Base.ConvertPath(fullName, false, true);
            spHelp.Visibility = Visibility.Collapsed;
            spInput.Visibility = Visibility.Visible;
            tbxInstallUri.Text = installUrl;

            var file = new UpdateFile {Action = FileAction.Update, Destination = installUrl, Hash = App.RM.GetString("CalculatingHash") + "...", FileSize = (ulong) new FileInfo(fullName).Length};

            if (Base.Update.Files == null)
                Base.Update.Files = new ObservableCollection<UpdateFile>();

            Base.Update.Files.Add(file);

            cbxUpdateType.SelectedIndex = 0;
            listBox.Items.Add(filename);
            listBox.SelectedIndex = (listBox.Items.Count - 1);

            piHashProgress.IsRunning = true;
            tbHash.Text = App.RM.GetString("CalculatingHash");
            tbHash.IsEnabled = false;
            listBox.IsEnabled = false;
            SevenUpdate.Base.GetHashAsync(fullName);
        }

        private void Base_HashGeneratedEventHandler(object sender, HashGeneratedEventArgs e)
        {
            piHashProgress.IsRunning = false;
            listBox.IsEnabled = true;
            Base.Update.Files[listBox.SelectedIndex].Hash = e.Hash;
            tbHash.Text = e.Hash;
            tbHash.IsEnabled = true;
        }

        private void AddFiles(string[] files)
        {
            for (int x = 0; x < files.Length; x++)
                AddFile(files[x]);
            listBox.SelectedIndex = (listBox.Items.Count - 1);
        }

        private void LoadInfo()
        {
            if (Base.Update.Files != null)
            {
                for (int x = 0; x < Base.Update.Files.Count; x++)
                    listBox.Items.Add(Path.GetFileName(Base.Update.Files[x].Destination));
            }
            listBox.SelectedIndex = 0;
        }

        #endregion

        #region UI Events

        #region TextBox - Text Changed

        private void Textbox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var source = e.Source as InfoTextBox;
            string path = SevenUpdate.Base.ConvertPath(source.Text, true, true);

            if (Base.CheckUrl(path))
            {
                switch (source.Name)
                {
                    case "tbxDownloadUrl":
                        if (cbxUpdateType.SelectedIndex != 4 && cbxUpdateType.SelectedIndex != 6 && cbxUpdateType.SelectedIndex != 8)
                        {
                            if (listBox.SelectedIndex > -1)
                                Base.Update.Files[listBox.SelectedIndex].Source = SevenUpdate.Base.ConvertPath(tbxDownloadUrl.Text, false, Base.Sua.Is64Bit);

                            if (Path.GetFileName(tbxDownloadUrl.Text) != "")
                                imgDownloadUrl.Visibility = Visibility.Collapsed;
                        }
                        break;

                    case "tbxInstallUri":
                        if (Path.GetFileName(path).ContainsAny(Path.GetInvalidPathChars()) == false)
                        {
                            if (listBox.SelectedIndex > -1)
                                Base.Update.Files[listBox.SelectedIndex].Destination = SevenUpdate.Base.ConvertPath(tbxInstallUri.Text, false, Base.Sua.Is64Bit);

                            if (Path.GetFileName(tbxInstallUri.Text) != "")
                                imgInstallUri.Visibility = Visibility.Collapsed;
                        }
                        break;
                }
            }
            else
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

        private void TextBox_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            var source = e.Source as InfoTextBox;
            if (source == null)
                return;

            source.Text = SevenUpdate.Base.ConvertPath(source.Text, false, Base.Sua.Is64Bit);
        }

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
            if (cfd.ShowDialog(Application.Current.MainWindow) == CommonFileDialogResult.OK)
                tbxInstallUri.Text = SevenUpdate.Base.ConvertPath(cfd.FileName, false, true);
        }

        private void Hash_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var cfd = new CommonOpenFileDialog {Multiselect = false};
            if (cfd.ShowDialog(Application.Current.MainWindow) != CommonFileDialogResult.OK)
                return;
            piHashProgress.IsRunning = true;
            tbHash.Text = App.RM.GetString("CalculatingHash") + "...";
            tbHash.IsEnabled = false;
            listBox.IsEnabled = false;
            SevenUpdate.Base.GetHashAsync(cfd.FileName);
            Base.Update.Files[listBox.SelectedIndex].Hash = tbHash.Text;
            Base.Update.Files[listBox.SelectedIndex].FileSize = (ulong) new FileInfo(cfd.FileName).Length;
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

        #region ComboBox - Selection Changed

        private void UpdateType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Base.Update.Files != null)
            {
                if (Base.Update.Files.Count > 0 && listBox.SelectedIndex > -1)
                    Base.Update.Files[listBox.SelectedIndex].Action = (FileAction) cbxUpdateType.SelectedIndex;
            }

            if (tbxDownloadUrl == null)
                return;
            if (cbxUpdateType.SelectedIndex != 4 && cbxUpdateType.SelectedIndex != 6 && cbxUpdateType.SelectedIndex != 8)
            {
                imgDownloadUrl.Visibility = Base.CheckUrl(tbxDownloadUrl.Text) ? Visibility.Collapsed : Visibility.Visible;

                tbxDownloadUrl.IsEnabled = true;
                tbxArgs.IsEnabled = true;
            }
            else
            {
                tbxDownloadUrl.Text = null;
                imgDownloadUrl.Visibility = Visibility.Collapsed;
                tbxDownloadUrl.IsEnabled = false;
                tbxArgs.IsEnabled = false;
                tbxArgs.Text = null;
            }
        }

        #endregion

        #region ListBox - Selection Changed

        private void listBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listBox.Items.Count > 0)
            {
                spHelp.Visibility = Visibility.Collapsed;
                spInput.Visibility = Visibility.Visible;
                miRemoveAll.IsEnabled = true;
                miRemove.IsEnabled = listBox.SelectedIndex > -1;
                if (listBox.SelectedIndex > -1 && Base.Update.Files != null)
                {
                    if (Base.Update.Files.Count > 0)
                        LoadFileInfo(listBox.SelectedIndex);
                }
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

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            LoadInfo();
        }
    }
}