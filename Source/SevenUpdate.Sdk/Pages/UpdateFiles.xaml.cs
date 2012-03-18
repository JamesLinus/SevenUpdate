// ***********************************************************************
// <copyright file="UpdateFiles.xaml.cs" project="SevenUpdate.Sdk" assembly="SevenUpdate.Sdk" solution="SevenUpdate" company="Seven Software">
//     Copyright (c) Seven Software. All rights reserved.
// </copyright>
// <author username="sevenalive">Robert Baker</author>
// <license href="http://www.gnu.org/licenses/gpl-3.0.txt" name="GNU General Public License 3">
//  This file is part of Seven Update.
//    Seven Update is free software: you can redistribute it and/or modify it under the terms of the GNU General Public
//    License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any
//    later version. Seven Update is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without
//    even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details. You should have received a copy of the GNU General Public License
//    along with Seven Update.  If not, see http://www.gnu.org/licenses/.
// </license>
// ***********************************************************************

namespace SevenUpdate.Sdk.Pages
{
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Forms;
    using System.Windows.Input;
    using System.Windows.Media;

    using SevenSoftware.Windows;
    using SevenSoftware.Windows.Controls;
    using SevenSoftware.Windows.Dialogs.TaskDialog;

    using SevenUpdate.Sdk.ValidationRules;
    using SevenUpdate.Sdk.Windows;

    using Application = System.Windows.Application;
    using KeyEventArgs = System.Windows.Input.KeyEventArgs;

    /// <summary>Interaction logic for UpdateFiles.xaml.</summary>
    public sealed partial class UpdateFiles
    {
        #region Constants and Fields

        /// <summary>The number of SHA-2 hashes generating.</summary>
        private int hashesGenerating;

        #endregion

        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref="UpdateFiles" /> class.</summary>
        public UpdateFiles()
        {
            this.InitializeComponent();

            this.listBox.ItemsSource = Core.UpdateInfo.Files;

            this.MouseLeftButtonDown -= Core.EnableDragOnGlass;
            this.MouseLeftButtonDown += Core.EnableDragOnGlass;
            AeroGlass.CompositionChanged -= this.UpdateUI;
            AeroGlass.CompositionChanged += this.UpdateUI;
            if (AeroGlass.IsGlassEnabled)
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
        /// <param name="file">The <c>UpdateFile</c> to get the file size for.</param>
        /// <param name="fileLocation">The location for the file.</param>
        private static void GetFileSize(ref UpdateFile file, string fileLocation = null)
        {
            UpdateFile updateFile = file;

            Task.Factory.StartNew(
                () =>
                    {
                        updateFile.FileSize =
                            Utilities.GetFileSize(
                                Utilities.ExpandInstallLocation(
                                    fileLocation ?? updateFile.Destination, 
                                    Core.AppInfo.Directory, 
                                    Core.AppInfo.Platform, 
                                    Core.AppInfo.ValueName));
                    });
        }

        /// <summary>Adds a file to the list.</summary>
        /// <param name="fullName">The full path to the file.</param>
        /// <param name="pathToFiles">The full directory path of the files being added.</param>
        /// <param name="impersonateAppDirectory"><c>True</c> to use use %INSTALLDIR% instead of real location of the file.</param>
        private void AddFile(string fullName, string pathToFiles, bool impersonateAppDirectory)
        {
            string installDirectory = Utilities.IsRegistryKey(Core.AppInfo.Directory)
                                          ? Utilities.GetRegistryValue(
                                              Core.AppInfo.Directory, Core.AppInfo.ValueName, Core.AppInfo.Platform)
                                          : Core.AppInfo.Directory;

            installDirectory = impersonateAppDirectory
                                   ? pathToFiles : Utilities.ConvertPath(installDirectory, true, Core.AppInfo.Platform);

            string installUrl = fullName.Replace(installDirectory, @"%INSTALLDIR%\", true);
            installUrl = installUrl.Replace(@"\\", @"\");

            string downloadUrl = fullName.Replace(installDirectory, @"%DOWNLOADURL%/", true);
            if (downloadUrl == fullName)
            {
                downloadUrl = @"%DOWNLOADURL%/" + Path.GetFileName(fullName);
            }
            else
            {
                downloadUrl = downloadUrl.Replace(@"\\", @"/");
                downloadUrl = downloadUrl.Replace(@"\", @"/");
                downloadUrl = downloadUrl.Replace(@"//", @"/");
            }

            var file = new UpdateFile
                {
                    Action = FileAction.Update, 
                    Destination = installUrl, 
                    Hash = Properties.Resources.CalculatingHash, 
                    Source = downloadUrl
                };

            Core.UpdateInfo.Files.Add(file);

            this.CalculateHash(ref file, fullName);
            GetFileSize(ref file, fullName);
        }

        /// <summary>Adds multiple files to the <c>UpdateFile</c> collection.</summary>
        /// <param name="files">A list of files to add to the update.</param>
        /// <param name="pathToFiles">The full directory path of the files being added.</param>
        /// <param name="impersonateAppDirectory"><c>True</c> to use use %INSTALLDIR% instead of real location of the file; otherwise, <c>False</c>.</param>
        private void AddFiles(IList<string> files, string pathToFiles, bool impersonateAppDirectory)
        {
            this.tbHashCalculating.Visibility = Visibility.Visible;
            Task.Factory.StartNew(
                () =>
                    {
                        this.AddFile(files[0], pathToFiles, impersonateAppDirectory);
                        for (int x = 1; x < files.Count; x++)
                        {
                            this.AddFile(files[x], pathToFiles, impersonateAppDirectory);
                        }
                    }).ContinueWith(delegate { this.CheckHashGenerating(); });
            this.listBox.SelectedIndex = this.listBox.SelectedIndex + 1;
        }

        /// <summary>Browses for a folder contains files to add to the <c>UpdateFile</c> collection.</summary>
        /// <param name="sender">The object that called the event.</param>
        /// <param name="e">The <c>System.Windows.RoutedEventArgs</c> instance containing the event data.</param>
        private void BrowseFolder(object sender, RoutedEventArgs e)
        {
            bool impersonate = Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift);
            string directory = !Utilities.IsRegistryKey(Core.AppInfo.Directory)
                                   ? Utilities.ConvertPath(Core.AppInfo.Directory, true, Core.AppInfo.Platform)
                                   : Utilities.GetRegistryValue(
                                       Core.AppInfo.Directory, Core.AppInfo.ValueName, Core.AppInfo.Platform);
            using (var folderBrowserDialog = new FolderBrowserDialog())
            {
                folderBrowserDialog.SelectedPath = directory;

                if (folderBrowserDialog.ShowDialog(Application.Current.MainWindow.GetIWin32Window()) == DialogResult.OK)
                {
                    this.AddFiles(
                        Directory.GetFiles(folderBrowserDialog.SelectedPath, "*.*", SearchOption.AllDirectories), 
                        folderBrowserDialog.SelectedPath, 
                        impersonate);
                }
            }
        }

        /// <summary>Browses for a file to add to the <c>UpdateFile</c> collection.</summary>
        /// <param name="sender">The object that called the event.</param>
        /// <param name="e">The <c>System.Windows.RoutedEventArgs</c> instance containing the event data.</param>
        private void BrowseForFile(object sender, RoutedEventArgs e)
        {
            bool impersonate = Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift);
            string directory = !Utilities.IsRegistryKey(Core.AppInfo.Directory)
                                   ? Utilities.ConvertPath(Core.AppInfo.Directory, true, Core.AppInfo.Platform)
                                   : Utilities.GetRegistryValue(
                                       Core.AppInfo.Directory, Core.AppInfo.ValueName, Core.AppInfo.Platform);

            string[] files = Core.OpenFileDialog(directory, null, true);
            if (files == null)
            {
                return;
            }

            this.AddFiles(files, Path.GetDirectoryName(files[0]), impersonate);
        }

        /// <summary>Calculates the hash.</summary>
        /// <param name="file">The <c>UpdateFile</c> to update the hash for.</param>
        /// <param name="fileLocation">The alternate location of the file.</param>
        private void CalculateHash(ref UpdateFile file, string fileLocation = null)
        {
            TaskScheduler context = TaskScheduler.FromCurrentSynchronizationContext();
            UpdateFile updateFile = file;
            this.hashesGenerating++;
            Task.Factory.StartNew(
                () =>
                    {
                        updateFile.Hash =
                            Utilities.GetHash(
                                Utilities.ExpandInstallLocation(
                                    fileLocation ?? updateFile.Destination, 
                                    Core.AppInfo.Directory, 
                                    Core.AppInfo.Platform, 
                                    Core.AppInfo.ValueName));
                    }).ContinueWith(_ => this.CheckHashGenerating(), context);
        }

        /// <summary>Changes the UI based on the selected <c>UpdateFile</c>'s <c>FileAction</c>.</summary>
        /// <param name="sender">The object that called the event.</param>
        /// <param name="e">The <c>System.Windows.Controls.SelectionChangedEventArgs</c> instance containing the event data.</param>
        private void ChangeUI(object sender, SelectionChangedEventArgs e)
        {
            if (this.tbxDownloadUrl == null || this.listBox.SelectedItem == null)
            {
                return;
            }

            var updateFile = this.listBox.SelectedItem as UpdateFile;

            // ReSharper disable PossibleNullReferenceException
            switch (updateFile.Action)
            {
                case FileAction.CompareOnly:
                case FileAction.Delete:
                case FileAction.UnregisterThenDelete:
                    updateFile.Source = null;
                    updateFile.Args = null;
                    this.tbxArgs.IsEnabled = false;
                    this.tbxDownloadUrl.IsEnabled = false;
                    this.tbxDownloadUrl.HasError = false;
                    this.tbxInstallLocation.HasError =
                        !new AppDirectoryRule().Validate(this.tbxInstallLocation.Text, null).IsValid;
                    this.tbxInstallLocation.ToolTip = this.tbxInstallLocation.HasError
                                                          ? Properties.Resources.FilePathInvalid : null;
                    break;

                default:
                    this.tbxArgs.IsEnabled = true;
                    this.tbxDownloadUrl.IsEnabled = true;

                    this.tbxDownloadUrl.HasError =
                        !new DownloadUrlRule { IsRequired = true }.Validate(this.tbxDownloadUrl.Text, null).IsValid;
                    this.tbxDownloadUrl.ToolTip = this.tbxDownloadUrl.HasError ? Properties.Resources.UrlNotValid : null;

                    this.tbxInstallLocation.HasError =
                        !new AppDirectoryRule().Validate(this.tbxInstallLocation.Text, null).IsValid;
                    this.tbxInstallLocation.ToolTip = this.tbxInstallLocation.HasError
                                                          ? Properties.Resources.FilePathInvalid : null;
                    break;
            }

            // ReSharper restore PossibleNullReferenceException
        }

        /// <summary>Checks if a hash is generating.</summary>
        private void CheckHashGenerating()
        {
            this.hashesGenerating--;

            if (this.hashesGenerating < 1)
            {
                this.tbHashCalculating.Visibility = Visibility.Collapsed;
            }
        }

        /// <summary>Converts a path to system variables.</summary>
        /// <param name="sender">The object that called the event.</param>
        /// <param name="e">The <c>System.Windows.Input.KeyboardFocusChangedEventArgs</c> instance containing the event data.</param>
        private void ConvertPath(object sender, KeyboardFocusChangedEventArgs e)
        {
            var source = e.Source as InfoTextBox;
            if (source == null)
            {
                return;
            }

            string fileLocation = Utilities.ConvertPath(source.Text, true, Core.AppInfo.Platform);
            string installDirectory = Utilities.IsRegistryKey(Core.AppInfo.Directory)
                                          ? Utilities.GetRegistryValue(
                                              Core.AppInfo.Directory, Core.AppInfo.ValueName, Core.AppInfo.Platform)
                                          : Core.AppInfo.Directory;

            installDirectory = Utilities.ConvertPath(installDirectory, true, Core.AppInfo.Platform);

            string installUrl = fileLocation.Replace(installDirectory, @"%INSTALLDIR%\", true);
            installUrl = installUrl.Replace(@"\\", @"\");

            source.Text = Utilities.ConvertPath(installUrl, false, Core.AppInfo.Platform);
        }

        /// <summary>Deletes an item from the <c>System.Windows.Controls.ListBox</c> on delete key down.</summary>
        /// <param name="sender">The object that called the event.</param>
        /// <param name="e">The <c>System.Windows.Input.KeyEventArgs</c> instance containing the event data.</param>
        private void DeleteItem(object sender, KeyEventArgs e)
        {
            int index = this.listBox.SelectedIndex;
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

        /// <summary>Determines whether this instance has errors.</summary>
        /// <returns><c>True</c> if this instance has errors; otherwise, <c>False</c>.</returns>
        private bool HasErrors()
        {
            if (Core.UpdateInfo.Files.Count == 0)
            {
                return false;
            }

            // ReSharper disable PossibleNullReferenceException
            return this.tbxDownloadUrl.HasError || this.tbxInstallLocation.HasError;

            // ReSharper restore PossibleNullReferenceException
        }

        /// <summary>Navigates to the next page if no errors exist.</summary>
        /// <param name="sender">The object that called the event.</param>
        /// <param name="e">The <c>System.Windows.RoutedEventArgs</c> instance containing the event data.</param>
        private void MoveOn(object sender, RoutedEventArgs e)
        {
            if (!this.HasErrors())
            {
                MainWindow.NavService.Navigate(Core.UpdateRegistryPage);
            }
            else
            {
                Core.ShowMessage(Properties.Resources.CorrectErrors, TaskDialogStandardIcon.Error);
            }
        }

        /// <summary>Navigates to the main page.</summary>
        /// <param name="sender">The object that called the event.</param>
        /// <param name="e">The <c>System.Windows.RoutedEventArgs</c> instance containing the event data.</param>
        private void NavigateToMainPage(object sender, RoutedEventArgs e)
        {
            MainWindow.NavService.Navigate(Core.MainPage);
        }

        /// <summary>Removes all files from the <c>UpdateFile</c> collection.</summary>
        /// <param name="sender">The object that called the event.</param>
        /// <param name="e">The <c>System.Windows.RoutedEventArgs</c> instance containing the event data.</param>
        private void RemoveAllFiles(object sender, RoutedEventArgs e)
        {
            Core.UpdateInfo.Files.Clear();
        }

        /// <summary>Removes a files from the <c>UpdateFile</c> collection.</summary>
        /// <param name="sender">The object that called the event.</param>
        /// <param name="e">The <c>System.Windows.RoutedEventArgs</c> instance containing the event data.</param>
        private void RemoveFile(object sender, RoutedEventArgs e)
        {
            Core.UpdateInfo.Files.RemoveAt(this.listBox.SelectedIndex);
        }

        /// <summary>Opens a dialog to browse for the selected file in the <c>UpdateFile</c> collection.</summary>
        /// <param name="sender">The object that called the event.</param>
        /// <param name="e">The <c>System.Windows.Input.MouseButtonEventArgs</c> instance containing the event data.</param>
        private void UpdateFile(object sender, MouseButtonEventArgs e)
        {
            var selectedItem = this.listBox.SelectedItem as UpdateFile;
            if (selectedItem == null)
            {
                return;
            }

            string installDirectory = Utilities.IsRegistryKey(Core.AppInfo.Directory)
                                          ? Utilities.GetRegistryValue(
                                              Core.AppInfo.Directory, Core.AppInfo.ValueName, Core.AppInfo.Platform)
                                          : Core.AppInfo.Directory;

            installDirectory = Utilities.ConvertPath(installDirectory, true, Core.AppInfo.Platform);

            string[] files = Core.OpenFileDialog(installDirectory);
            if (files == null)
            {
                return;
            }

            string installUrl = files[0].Replace(installDirectory, @"%INSTALLDIR%\", true);
            installUrl = installUrl.Replace(@"\\", @"\");
            selectedItem.Destination = installUrl;
            this.CalculateHash(ref selectedItem, files[0]);
            GetFileSize(ref selectedItem, files[0]);
        }

        /// <summary>Updates the hash for the selected <c>UpdateFile</c>.</summary>
        /// <param name="sender">The object that called the event.</param>
        /// <param name="e">The <c>System.Windows.Input.MouseButtonEventArgs</c> instance containing the event data.</param>
        private void UpdateHash(object sender, MouseButtonEventArgs e)
        {
            var selectedItem = this.listBox.SelectedItem as UpdateFile;
            if (selectedItem == null)
            {
                return;
            }

            string fileLocation = Utilities.ExpandInstallLocation(
                selectedItem.Destination, Core.AppInfo.Directory, Core.AppInfo.Platform, Core.AppInfo.ValueName);

            string[] files = Core.OpenFileDialog(Path.GetDirectoryName(fileLocation), Path.GetFileName(fileLocation));

            if (files == null)
            {
                return;
            }

            this.CalculateHash(ref selectedItem, files[0]);
            GetFileSize(ref selectedItem, files[0]);
        }

        /// <summary>Updates the UI based on whether Aero Glass is enabled.</summary>
        /// <param name="sender">The object that called the event.</param>
        /// <param name="e">The <c>CompositionChangedEventArgs</c> instance containing the event data.</param>
        private void UpdateUI(object sender, CompositionChangedEventArgs e)
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

        /// <summary>Validates the download directory.</summary>
        /// <param name="sender">The object that called the event.</param>
        /// <param name="e">The data for the event.</param>
        private void ValidateDownloadDirectory(object sender, TextChangedEventArgs e)
        {
            var textBox = sender as InfoTextBox;

            if (textBox == null)
            {
                return;
            }

            if (!textBox.IsEnabled)
            {
                return;
            }

            textBox.HasError = !new DownloadUrlRule { IsRequired = true }.Validate(textBox.Text, null).IsValid;
            textBox.ToolTip = textBox.HasError ? Properties.Resources.UrlNotValid : null;

            if (!textBox.HasError && this.listBox.SelectedItem != null)
            {
                ((UpdateFile)this.listBox.SelectedItem).Source = textBox.Text;
            }
        }

        /// <summary>Validates the install directory.</summary>
        /// <param name="sender">The object that called the event.</param>
        /// <param name="e">The data for the event.</param>
        private void ValidateInstallDirectory(object sender, TextChangedEventArgs e)
        {
            var textBox = sender as InfoTextBox;

            if (textBox == null)
            {
                return;
            }

            if (!textBox.IsEnabled)
            {
                return;
            }

            textBox.HasError = !new AppDirectoryRule().Validate(textBox.Text, null).IsValid;
            textBox.ToolTip = textBox.HasError ? Properties.Resources.FilePathInvalid : null;

            if (!textBox.HasError && this.listBox.SelectedItem != null)
            {
                ((UpdateFile)this.listBox.SelectedItem).Destination = textBox.Text;
            }
        }

        #endregion
    }
}