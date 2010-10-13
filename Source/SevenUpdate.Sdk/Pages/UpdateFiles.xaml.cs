// ***********************************************************************
// <copyright file="UpdateFiles.xaml.cs"
//            project="SevenUpdate.Sdk"
//            assembly="SevenUpdate.Sdk"
//            solution="SevenUpdate"
//            company="Seven Software">
//     Copyright (c) Seven Software. All rights reserved.
// </copyright>
// <author username="sevenalive">Robert Baker</author>
// <license href="http://www.gnu.org/licenses/gpl-3.0.txt" name="GNU General Public License 3">
//  This file is part of Seven Update.
//
//    Seven Update is free software: you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation, either version 3 of the License, or
//    (at your option) any later version.
//
//    Seven Update is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.
//
//    You should have received a copy of the GNU General Public License
//    along with Seven Update.  If not, see http://www.gnu.org/licenses/.
// </license>
// ***********************************************************************
namespace SevenUpdate.Sdk.Pages
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Dialogs.TaskDialogs;
    using System.Windows.Dwm;
    using System.Windows.Forms;
    using System.Windows.Input;
    using System.Windows.Media;

    using SevenUpdate.Sdk.ValidationRules;
    using SevenUpdate.Sdk.Windows;

    using Application = System.Windows.Application;
    using KeyEventArgs = System.Windows.Input.KeyEventArgs;
    using TextBox = System.Windows.Controls.TextBox;

    /// <summary>Interaction logic for UpdateFiles.xaml</summary>
    public sealed partial class UpdateFiles
    {
        #region Constants and Fields

        /// <summary>The number of SHA-2 hashes generating</summary>
        private int hashesGenerating;

        #endregion

        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref = "UpdateFiles" /> class.</summary>
        public UpdateFiles()
        {
            this.InitializeComponent();

            this.listBox.ItemsSource = Core.UpdateInfo.Files;

            this.MouseLeftButtonDown += Core.EnableDragOnGlass;
            AeroGlass.DwmCompositionChanged += this.UpdateUI;
            if (AeroGlass.IsEnabled)
            {
                this.tbTitle.Foreground = Brushes.Black;
                this.line.Visibility = Visibility.Collapsed;
                this.rectangle.Visibility = Visibility.Collapsed;
            }
            else
            {
                this.tbTitle.Foreground = new SolidColorBrush(Color.FromRgb(0, 51, 153));
                this.line.Visibility = Visibility.Visible;
                this.rectangle.Visibility = Visibility.Visible;
            }
        }

        #endregion

        #region Methods

        /// <summary>Gets the size of the file.</summary>
        /// <param name="file">The <see cref="UpdateFile"/> to get the file size for</param>
        /// <param name="fileLocation">The location for the file</param>
        private static void GetFileSize(ref UpdateFile file, string fileLocation = null)
        {
            var updateFile = file;

            Task.Factory.StartNew(
                () =>
                    {
                        updateFile.FileSize =
                            Utilities.GetFileSize(
                                Utilities.ConvertPath(fileLocation ?? updateFile.Destination, Core.AppInfo.Directory, Core.AppInfo.Is64Bit, Core.AppInfo.ValueName));
                    });
        }

        /// <summary>Adds a file to the list</summary>
        /// <param name="fullName">The full path to the file</param>
        private void AddFile(string fullName)
        {
            var installDirectory = Utilities.IsRegistryKey(Core.AppInfo.Directory)
                                       ? Utilities.GetRegistryValue(Core.AppInfo.Directory, Core.AppInfo.ValueName, Core.AppInfo.Is64Bit)
                                       : Core.AppInfo.Directory;

            installDirectory = Utilities.ConvertPath(installDirectory, true, Core.AppInfo.Is64Bit);

            var installUrl = fullName.Replace(installDirectory, @"%INSTALLDIR%\", true);
            installUrl = installUrl.Replace(@"\\", @"\");

            var downloadUrl = fullName.Replace(installDirectory, @"%DOWNLOADURL%\", true);
            downloadUrl = downloadUrl.Replace(@"\\", @"\");

            var file = new UpdateFile
                {
                    Action = FileAction.Update, 
                    Destination = installUrl, 
                    Hash = Properties.Resources.CalculatingHash, 
                    Source = downloadUrl
                };

            Core.UpdateInfo.Files.Add(file);

            this.tbHashCalculating.Visibility = Visibility.Visible;

            this.CalculateHash(ref file);
            GetFileSize(ref file);
        }

        /// <summary>Adds multiple files to the <see cref="UpdateFile"/> collection</summary>
        /// <param name="files">The files.</param>
        private void AddFiles(IList<string> files)
        {
            this.AddFile(files[0]);
            for (var x = 1; x < files.Count; x++)
            {
                this.AddFile(files[x]);
            }

            this.listBox.SelectedIndex = 0;
        }

        /// <summary>Browses for a folder contains files to add to the <see cref="UpdateFile"/> collection</summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void BrowseFolder(object sender, RoutedEventArgs e)
        {
            var directory = !Utilities.IsRegistryKey(Core.AppInfo.Directory)
                                ? Utilities.ConvertPath(Core.AppInfo.Directory, true, Core.AppInfo.Is64Bit)
                                : Utilities.GetRegistryValue(Core.AppInfo.Directory, Core.AppInfo.ValueName, Core.AppInfo.Is64Bit);
            var folderBrowserDialog = new FolderBrowserDialog
                {
                    SelectedPath = directory
                };

            if (folderBrowserDialog.ShowDialog(Application.Current.MainWindow.GetIWin32Window()) == DialogResult.OK)
            {
                this.AddFiles(Directory.GetFiles(folderBrowserDialog.SelectedPath, "*.*", SearchOption.AllDirectories));
            }
        }

        /// <summary>Browses for a file to add to the <see cref="UpdateFile"/> collection</summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void BrowseForFile(object sender, RoutedEventArgs e)
        {
            var directory = !Utilities.IsRegistryKey(Core.AppInfo.Directory)
                                ? Utilities.ConvertPath(Core.AppInfo.Directory, true, Core.AppInfo.Is64Bit)
                                : Utilities.GetRegistryValue(Core.AppInfo.Directory, Core.AppInfo.ValueName, Core.AppInfo.Is64Bit);

            var files = Core.OpenFileDialog(directory, null, true);
            if (files == null)
            {
                return;
            }

            this.AddFiles(files);
        }

        /// <summary>Calculates the hash.</summary>
        /// <param name="file">The <see cref="UpdateFile"/> to update the hash for</param>
        /// <param name="fileLocation">The alternate location of the file</param>
        private void CalculateHash(ref UpdateFile file, string fileLocation = null)
        {
            var context = TaskScheduler.FromCurrentSynchronizationContext();
            var updateFile = file;
            this.tbHashCalculating.Visibility = Visibility.Visible;
            this.hashesGenerating++;
            Task.Factory.StartNew(
                () =>
                    {
                        updateFile.Hash =
                            Utilities.GetHash(
                                Utilities.ConvertPath(fileLocation ?? updateFile.Destination, Core.AppInfo.Directory, Core.AppInfo.Is64Bit, Core.AppInfo.ValueName));
                    }).
                ContinueWith(_ => this.CheckHashGenerating(), context);
        }

        /// <summary>
        /// File
        ///   Changes the UI based on the selected <see cref="UpdateFile"/>'s <see cref="FileAction"/>
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Controls.SelectionChangedEventArgs"/> instance containing the event data.</param>
        private void ChangeUI(object sender, SelectionChangedEventArgs e)
        {
            if (this.tbxDownloadUrl == null || this.listBox.SelectedItem == null)
            {
                return;
            }

            var updateFile = this.listBox.SelectedItem as UpdateFile;

            // ReSharper disable PossibleNullReferenceException
            this.tbxDownloadUrl.GetBindingExpression(TextBox.TextProperty).ParentBinding.ValidationRules.Clear();
            switch (updateFile.Action)
            {
                case FileAction.CompareOnly:
                case FileAction.Delete:
                case FileAction.UnregisterThenDelete:
                    updateFile.Source = null;
                    updateFile.Args = null;
                    this.tbxArgs.IsEnabled = false;
                    this.tbxDownloadUrl.IsEnabled = false;
                    break;

                default:
                    var rule = new AppDirectoryRule();
                    this.tbxDownloadUrl.GetBindingExpression(TextBox.TextProperty).ParentBinding.ValidationRules.Add(rule);
                    this.tbxArgs.IsEnabled = true;
                    this.tbxDownloadUrl.IsEnabled = true;
                    break;
            }

            this.tbxDownloadUrl.GetBindingExpression(TextBox.TextProperty).UpdateSource();

            // ReSharper restore PossibleNullReferenceException
        }

        /// <summary>Checks if a hash is generating</summary>
        private void CheckHashGenerating()
        {
            this.hashesGenerating--;

            if (this.hashesGenerating < 1)
            {
                this.tbHashCalculating.Visibility = Visibility.Collapsed;
            }
        }

        /// <summary>Converts a path to system variables</summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.KeyboardFocusChangedEventArgs"/> instance containing the event data.</param>
        private void ConvertPath(object sender, KeyboardFocusChangedEventArgs e)
        {
            var source = e.Source as InfoTextBox;
            if (source == null)
            {
                return;
            }

            var fileLocation = Utilities.ConvertPath(source.Text, true, Core.AppInfo.Is64Bit);
            var installDirectory = Utilities.IsRegistryKey(Core.AppInfo.Directory)
                                       ? Utilities.GetRegistryValue(Core.AppInfo.Directory, Core.AppInfo.ValueName, Core.AppInfo.Is64Bit)
                                       : Core.AppInfo.Directory;

            installDirectory = Utilities.ConvertPath(installDirectory, true, Core.AppInfo.Is64Bit);

            var installUrl = fileLocation.Replace(installDirectory, @"%INSTALLDIR%\", true);
            installUrl = installUrl.Replace(@"\\", @"\");

            source.Text = Utilities.ConvertPath(installUrl, false, Core.AppInfo.Is64Bit);
        }

        /// <summary>Deletes an item from the <see cref="System.Windows.Controls.ListBox"/> on delete key down</summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.KeyEventArgs"/> instance containing the event data.</param>
        private void DeleteItem(object sender, KeyEventArgs e)
        {
            var index = this.listBox.SelectedIndex;
            if (index < 0)
            {
                return;
            }

            if (e.Key != Key.Delete)
            {
                return;
            }

            Core.UpdateInfo.Files.RemoveAt(index);
            this.listBox.SelectedIndex = index - 1;

            if (this.listBox.SelectedIndex < 0 && this.listBox.Items.Count > 0)
            {
                this.listBox.SelectedIndex = 0;
            }
        }

        /// <summary>Navigates to the main page</summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void GoToMainPage(object sender, RoutedEventArgs e)
        {
            MainWindow.NavService.Navigate(new Uri(@"/SevenUpdate.Sdk;component/Pages/Main.xaml", UriKind.Relative));
        }

        /// <summary>Determines whether this instance has errors.</summary>
        /// <returns><see langword="true"/> if this instance has errors; otherwise, <see langword="false"/>.</returns>
        private bool HasErrors()
        {
            if (Core.UpdateInfo.Files.Count == 0)
            {
                return false;
            }

            // ReSharper disable PossibleNullReferenceException
            return this.tbxDownloadUrl.GetBindingExpression(TextBox.TextProperty).HasError || this.tbxInstallLocation.GetBindingExpression(TextBox.TextProperty).HasError;

            // ReSharper restore PossibleNullReferenceException
        }

        /// <summary>Navigates to the next page if no errors exist</summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void MoveOn(object sender, RoutedEventArgs e)
        {
            if (!this.HasErrors())
            {
                MainWindow.NavService.Navigate(new Uri(@"/SevenUpdate.Sdk;component/Pages/UpdateRegistry.xaml", UriKind.Relative));
            }
            else
            {
                Core.ShowMessage(Properties.Resources.CorrectErrors, TaskDialogStandardIcon.Error);
            }
        }

        /// <summary>Removes all files from the <see cref="UpdateFile"/> collection</summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void RemoveAllFiles(object sender, RoutedEventArgs e)
        {
            Core.UpdateInfo.Files.Clear();
        }

        /// <summary>Removes a files from the <see cref="UpdateFile"/> collection</summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void RemoveFile(object sender, RoutedEventArgs e)
        {
            Core.UpdateInfo.Files.RemoveAt(this.listBox.SelectedIndex);
        }

        /// <summary>Opens a dialog to browse for the selected file in the <see cref="UpdateFile"/> collection</summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        private void UpdateFile(object sender, MouseButtonEventArgs e)
        {
            var installDirectory = Utilities.IsRegistryKey(Core.AppInfo.Directory)
                                       ? Utilities.GetRegistryValue(Core.AppInfo.Directory, Core.AppInfo.ValueName, Core.AppInfo.Is64Bit)
                                       : Core.AppInfo.Directory;

            installDirectory = Utilities.ConvertPath(installDirectory, true, Core.AppInfo.Is64Bit);

            var files = Core.OpenFileDialog(installDirectory);
            if (files == null)
            {
                return;
            }

            var installUrl = files[0].Replace(installDirectory, @"%INSTALLDIR%\", true);
            installUrl = installUrl.Replace(@"\\", @"\");
            Core.UpdateInfo.Files[this.listBox.SelectedIndex].Destination = installUrl;
        }

        /// <summary>Updates the hash for the selected <see cref="UpdateFile"/></summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        private void UpdateHash(object sender, MouseButtonEventArgs e)
        {
            var selectedItem = this.listBox.SelectedItem as UpdateFile;
            if (selectedItem == null)
            {
                return;
            }

            var fileLocation = Utilities.ConvertPath(selectedItem.Destination, Core.AppInfo.Directory, Core.AppInfo.Is64Bit, Core.AppInfo.ValueName);

            var files = Core.OpenFileDialog(Path.GetDirectoryName(fileLocation), Path.GetFileName(fileLocation));

            if (files == null)
            {
                return;
            }

            this.CalculateHash(ref selectedItem, files[0]);
            GetFileSize(ref selectedItem, files[0]);
        }

        /// <summary>Updates the UI based on whether Aero Glass is enabled</summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="AeroGlass.DwmCompositionChangedEventArgs"/> instance containing the event data.</param>
        private void UpdateUI(object sender, AeroGlass.DwmCompositionChangedEventArgs e)
        {
            if (e.IsGlassEnabled)
            {
                this.tbTitle.Foreground = Brushes.Black;
                this.line.Visibility = Visibility.Visible;
                this.rectangle.Visibility = Visibility.Visible;
            }
            else
            {
                this.tbTitle.Foreground = new SolidColorBrush(Color.FromRgb(0, 51, 153));
                this.line.Visibility = Visibility.Collapsed;
                this.rectangle.Visibility = Visibility.Collapsed;
            }
        }

        #endregion
    }
}