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
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.Windows.Controls;
using Microsoft.Windows.Dialogs;
using Microsoft.Windows.Dwm;
using SevenUpdate.Sdk.Helpers;
using SevenUpdate.Sdk.Windows;

#endregion

namespace SevenUpdate.Sdk.Pages
{
    /// <summary>
    ///   Interaction logic for UpdateFiles.xaml
    /// </summary>
    public sealed partial class UpdateFiles
    {
        #region Fields

        private int hashesGenerating;

        #endregion

        #region Constructors

        /// <summary>
        ///   The constructor for the UpdateFiles page
        /// </summary>
        public UpdateFiles()
        {
            InitializeComponent();

            listBox.ItemsSource = Core.UpdateInfo.Files;

            MouseLeftButtonDown += Core.Rectangle_MouseLeftButtonDown;
            AeroGlass.DwmCompositionChanged += AeroGlass_DwmCompositionChanged;
            if (AeroGlass.IsEnabled)
            {
                tbTitle.Foreground = Brushes.Black;
                line.Visibility = Visibility.Collapsed;
                rectangle.Visibility = Visibility.Collapsed;
            }
            else
            {
                tbTitle.Foreground = new SolidColorBrush(Color.FromRgb(0, 51, 153));
                line.Visibility = Visibility.Visible;
                rectangle.Visibility = Visibility.Visible;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        ///   Adds a file to the list
        /// </summary>
        /// <param name = "fullName">The fullpath to the file</param>
        private void AddFile(string fullName)
        {
            var installDirectory = Base.IsRegistryKey(Core.AppInfo.Directory) ? Base.GetRegistryPath(Core.AppInfo.Directory, Core.AppInfo.ValueName, Core.AppInfo.Is64Bit) : Core.AppInfo.Directory;

            installDirectory = Base.ConvertPath(installDirectory, true, Core.AppInfo.Is64Bit);

            var installUrl = fullName.Replace(installDirectory, @"%INSTALLDIR%\", true);
            installUrl = installUrl.Replace(@"\\", @"\");

            var downloadUrl = fullName.Replace(installDirectory, @"%DOWNLOADURL%\", true);
            downloadUrl = downloadUrl.Replace(@"\\", @"\");

            var file = new UpdateFile {Action = FileAction.Update, Destination = installUrl, Hash = Properties.Resources.CalculatingHash, Source = downloadUrl};

            Core.UpdateInfo.Files.Add(file);

            tbHashCalculating.Visibility = Visibility.Visible;

            CalculateHash(ref file);
            GetFileSize(ref file);
        }

        private void CalculateHash(ref UpdateFile file, string fileLocation = null)
        {
            var context = TaskScheduler.FromCurrentSynchronizationContext();
            var updateFile = file;
            tbHashCalculating.Visibility = Visibility.Visible;
            hashesGenerating++;
            Task.Factory.StartNew(
                () => { updateFile.Hash = Base.GetHash(Base.ConvertPath(fileLocation ?? updateFile.Destination, Core.AppInfo.Directory, Core.AppInfo.ValueName, Core.AppInfo.Is64Bit)); }).ContinueWith(
                    _ => CheckHash(), context);
        }

        private static void GetFileSize(ref UpdateFile file, string fileLocation = null)
        {
            var updateFile = file;

            Task.Factory.StartNew(
                () => { updateFile.FileSize = Base.GetFileSize(Base.ConvertPath(fileLocation ?? updateFile.Destination, Core.AppInfo.Directory, Core.AppInfo.ValueName, Core.AppInfo.Is64Bit)); });
        }

        private void AddFiles(IList<string> files)
        {
            AddFile(files[0]);
            for (var x = 1; x < files.Count; x++)
                AddFile(files[x]);
            listBox.SelectedIndex = 0;
        }

        private void CheckHash()
        {
            hashesGenerating--;

            if (hashesGenerating < 1)
                tbHashCalculating.Visibility = Visibility.Collapsed;
        }

        private bool HasErrors()
        {
            if (Core.UpdateInfo.Files.Count == 0)
                return false;
            return tbxDownloadUrl.GetBindingExpression(TextBox.TextProperty).HasError || tbxInstallLocation.GetBindingExpression(TextBox.TextProperty).HasError;
        }

        #endregion

        #region UI Events

        #region TextBox - Lost Keyboard Focus

        private void TextBox_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            var source = e.Source as InfoTextBox;
            if (source == null)
                return;
            var fileLocation = Base.ConvertPath(source.Text, true, Core.AppInfo.Is64Bit);
            var installDirectory = Base.IsRegistryKey(Core.AppInfo.Directory) ? Base.GetRegistryPath(Core.AppInfo.Directory, Core.AppInfo.ValueName, Core.AppInfo.Is64Bit) : Core.AppInfo.Directory;

            installDirectory = Base.ConvertPath(installDirectory, true, Core.AppInfo.Is64Bit);

            var installUrl = fileLocation.Replace(installDirectory, @"%INSTALLDIR%\", true);
            installUrl = installUrl.Replace(@"\\", @"\");

            source.Text = Base.ConvertPath(installUrl, false, Core.AppInfo.Is64Bit);
        }

        #endregion

        #region Button - Click

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (!HasErrors())
                MainWindow.NavService.Navigate(new Uri(@"Pages\UpdateRegistry.xaml", UriKind.Relative));
            else
                Core.ShowMessage(Properties.Resources.CorrectErrors, TaskDialogStandardIcon.Error);
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.NavService.Navigate(new Uri(@"Pages\Main.xaml", UriKind.Relative));
        }

        #endregion

        #region TextBlock - Mouse Down

        private void Browse_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var installDirectory = Base.IsRegistryKey(Core.AppInfo.Directory) ? Base.GetRegistryPath(Core.AppInfo.Directory, Core.AppInfo.ValueName, Core.AppInfo.Is64Bit) : Core.AppInfo.Directory;

            installDirectory = Base.ConvertPath(installDirectory, true, Core.AppInfo.Is64Bit);

            var files = Core.OpenFileDialog(installDirectory, false, null, Path.GetFileName(Core.UpdateInfo.Files[listBox.SelectedIndex].Destination));
            if (files == null)
                return;

            var installUrl = files[0].Replace(installDirectory, @"%INSTALLDIR%\", true);
            installUrl = installUrl.Replace(@"\\", @"\");
            Core.UpdateInfo.Files[listBox.SelectedIndex].Destination = installUrl;
        }

        private void Hash_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var directory = !Base.IsRegistryKey(Core.AppInfo.Directory)
                                ? Base.ConvertPath(Core.AppInfo.Directory, true, Core.AppInfo.Is64Bit)
                                : Base.GetRegistryPath(Core.AppInfo.Directory, Core.AppInfo.ValueName, Core.AppInfo.Is64Bit);
            var files = Core.OpenFileDialog(directory, false, null, Path.GetFileName(Core.UpdateInfo.Files[listBox.SelectedIndex].Destination));

            if (files == null)
                return;

            var selectedItem = listBox.SelectedItem as UpdateFile;
            CalculateHash(ref selectedItem, files[0]);
            GetFileSize(ref selectedItem, files[0]);
        }

        #endregion

        #region Content Menu

        #region MenuItem - Click

        private void miRemove_Click(object sender, RoutedEventArgs e)
        {
            Core.UpdateInfo.Files.RemoveAt(listBox.SelectedIndex);
        }

        private void miRemoveAll_Click(object sender, RoutedEventArgs e)
        {
            Core.UpdateInfo.Files.Clear();
        }

        private void AddFile_Click(object sender, RoutedEventArgs e)
        {
            var directory = !Base.IsRegistryKey(Core.AppInfo.Directory)
                                ? Base.ConvertPath(Core.AppInfo.Directory, true, Core.AppInfo.Is64Bit)
                                : Base.GetRegistryPath(Core.AppInfo.Directory, Core.AppInfo.ValueName, Core.AppInfo.Is64Bit);

            var files = Core.OpenFileDialog(directory, true);
            if (files == null)
                return;
            AddFiles(files);
        }

        private void AddFolder_Click(object sender, RoutedEventArgs e)
        {
            var directory = !Base.IsRegistryKey(Core.AppInfo.Directory)
                                ? Base.ConvertPath(Core.AppInfo.Directory, true, Core.AppInfo.Is64Bit)
                                : Base.GetRegistryPath(Core.AppInfo.Directory, Core.AppInfo.ValueName, Core.AppInfo.Is64Bit);

            var cfd = new CommonOpenFileDialog {Multiselect = false, IsFolderPicker = true, InitialDirectory = directory};
            if (cfd.ShowDialog(Application.Current.MainWindow) == CommonFileDialogResult.OK)
                AddFiles(Directory.GetFiles(cfd.FileName, "*.*", SearchOption.AllDirectories));
        }

        #endregion

        #endregion

        #region ComboBox - SelectedItemChanged

        private void UpdateType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (tbxDownloadUrl == null || listBox.SelectedItem == null)
                return;
            var updateFile = listBox.SelectedItem as UpdateFile;
            // ReSharper disable PossibleNullReferenceException
            tbxDownloadUrl.GetBindingExpression(TextBox.TextProperty).ParentBinding.ValidationRules.Clear();
            switch (updateFile.Action)
            {
                case FileAction.CompareOnly:
                case FileAction.Delete:
                case FileAction.UnregisterThenDelete:
                    updateFile.Source = null;
                    updateFile.Args = null;
                    tbxArgs.IsEnabled = false;
                    tbxDownloadUrl.IsEnabled = false;
                    break;

                default:
                    var rule = new AppDirectoryRule();
                    tbxDownloadUrl.GetBindingExpression(TextBox.TextProperty).ParentBinding.ValidationRules.Add(rule);
                    tbxArgs.IsEnabled = true;
                    tbxDownloadUrl.IsEnabled = true;
                    break;
            }
            tbxDownloadUrl.GetBindingExpression(TextBox.TextProperty).UpdateSource();
            // ReSharper restore PossibleNullReferenceException
        }

        #endregion

        #region Aero

        private void AeroGlass_DwmCompositionChanged(object sender, AeroGlass.DwmCompositionChangedEventArgs e)
        {
            if (e.IsGlassEnabled)
            {
                tbTitle.Foreground = Brushes.Black;
                line.Visibility = Visibility.Visible;
                rectangle.Visibility = Visibility.Visible;
            }
            else
            {
                tbTitle.Foreground = new SolidColorBrush(Color.FromRgb(0, 51, 153));
                line.Visibility = Visibility.Collapsed;
                rectangle.Visibility = Visibility.Collapsed;
            }
        }

        #endregion

        private void listBox_KeyDown(object sender, KeyEventArgs e)
        {
            var index = listBox.SelectedIndex;
            if (index < 0)
                return;
            if (e.Key != Key.Delete)
                return;
            Core.UpdateInfo.Files.RemoveAt(index);
            listBox.SelectedIndex = (index - 1);

            if (listBox.SelectedIndex < 0 && listBox.Items.Count > 0)
                listBox.SelectedIndex = 0;
        }

        #endregion
    }
}