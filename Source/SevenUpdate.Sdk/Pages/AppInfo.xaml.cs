// ***********************************************************************
// <copyright file="AppInfo.xaml.cs"
//            project="SevenUpdate.Sdk"
//            assembly="SevenUpdate.Sdk"
//            solution="SevenUpdate"
//            company="Seven Software">
//     Copyright (c) Seven Software. All rights reserved.
// </copyright>
// <author username="sevenalive">Robert Baker</author>
// <license href="http://www.gnu.org/licenses/gpl-3.0.txt" name="GNU General Public License 3">
//  This file is part of Seven Update.
//    Seven Update is free software: you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation, either version 3 of the License, or
//    (at your option) any later version.
//    Seven Update is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.
//    You should have received a copy of the GNU General Public License
//    along with Seven Update.  If not, see http://www.gnu.org/licenses/.
// </license>
// ***********************************************************************
namespace SevenUpdate.Sdk.Pages
{
    using System;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Dialogs;
    using System.Windows.Forms;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.ValidationRules;

    using SevenUpdate.Sdk.ValidationRules;
    using SevenUpdate.Sdk.Windows;

    using Application = System.Windows.Application;

    /// <summary>Interaction logic for AppInfo.xaml</summary>
    public sealed partial class AppInfo
    {
        #region Constants and Fields

        /// <summary>Indicates the platform of the application before editing</summary>
        private static Platform oldPlatform;

        #endregion

        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref = "AppInfo" /> class.</summary>
        public AppInfo()
        {
            this.InitializeComponent();
            this.DataContext = Core.AppInfo;

            if (Core.AppIndex > -1)
            {
                this.btnNext.Visibility = Visibility.Collapsed;
                this.btnCancel.Content = Properties.Resources.Save;
            }
            else
            {
                this.btnCancel.Content = Properties.Resources.Cancel;
                this.btnNext.Visibility = Visibility.Visible;
            }

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

        /// <summary>Opens a <see cref="OpenFileDialog"/> to browse for the application install location</summary>
        /// <param name="sender">The object that called the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        private void BrowseForAppLocation(object sender, MouseButtonEventArgs e)
        {
            using (var folderBrowserDialog = new FolderBrowserDialog())
            {
                if (folderBrowserDialog.ShowDialog(Application.Current.MainWindow.GetIWin32Window()) == DialogResult.OK)
                {
                    this.tbxAppLocation.Text = Utilities.ConvertPath(folderBrowserDialog.SelectedPath, false, Core.AppInfo.Platform);
                }
            }
        }

        /// <summary>Fires the OnPropertyChanged Event with the collection changes</summary>
        /// <param name="sender">The object that called the event.</param>
        /// <param name="e">The event data</param>
        private void ChangeDescription(object sender, RoutedEventArgs e)
        {
            var textBox = (InfoTextBox)sender;
            Core.UpdateLocaleStrings(textBox.Text, Core.AppInfo.Description);
        }

        /// <summary>Fires the OnPropertyChanged Event with the collection changes</summary>
        /// <param name="sender">The object that called the event.</param>
        /// <param name="e">The event data</param>
        private void ChangeName(object sender, RoutedEventArgs e)
        {
            var textBox = (InfoTextBox)sender;
            Core.UpdateLocaleStrings(textBox.Text, Core.AppInfo.Name);
        }

        /// <summary>Fires the OnPropertyChanged Event with the collection changes</summary>
        /// <param name="sender">The object that called the event.</param>
        /// <param name="e">The event data</param>
        private void ChangePublisher(object sender, RoutedEventArgs e)
        {
            var textBox = (InfoTextBox)sender;
            Core.UpdateLocaleStrings(textBox.Text, Core.AppInfo.Publisher);
        }

        /// <summary>Changes the UI to show the file system application location</summary>
        /// <param name="sender">The object that called the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void ChangeToFileSystemLocation(object sender, RoutedEventArgs e)
        {
            if (this.tbxAppLocation == null)
            {
                return;
            }

            this.tbxAppLocation.Note = @"%PROGRAMFILES%\My Company\My App";

            this.tbxAppLocation.HasError = !new AppDirectoryRule().Validate(this.tbxAppLocation.Text, null).IsValid;
        }

        /// <summary>Changes the UI to show the registry application location</summary>
        /// <param name="sender">The object that called the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void ChangeToRegistryLocation(object sender, RoutedEventArgs e)
        {
            if (this.tbxAppLocation == null)
            {
                return;
            }

            this.tbxAppLocation.Note = @"HKLM\Software\MyCompany\MyApp";

            this.tbxAppLocation.HasError = !new RegistryPathRule().Validate(this.tbxAppLocation.Text, null).IsValid;
        }

        /// <summary>Converts the application location path to system variables</summary>
        /// <param name="sender">The object that called the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.KeyboardFocusChangedEventArgs"/> instance containing the event data.</param>
        private void ConvertPath(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (this.rbtnFileSystem.IsChecked.GetValueOrDefault())
            {
                this.tbxAppLocation.Text = Utilities.ConvertPath(this.tbxAppLocation.Text, false, Core.AppInfo.Platform);
            }
        }

        /// <summary>Determines whether this instance has errors.</summary>
        /// <returns><see langword="true"/> if this instance has errors; otherwise, <see langword="false"/>.</returns>
        private bool HasErrors()
        {
            if (this.rbtnRegistry.IsChecked.GetValueOrDefault() && this.tbxValueName.HasError)
            {
                return true;
            }

            return this.tbxAppName.HasError || this.tbxPublisher.HasError || this.tbxAppUrl.HasError || this.tbxHelpUrl.HasError || this.tbxAppLocation.HasError || this.tbxAppDescription.HasError ||
                   this.tbxSuiUrl.HasError;
        }

        /// <summary>Loads the application info into the UI.</summary>
        /// <param name="sender">The object that called the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void LoadAppInfo(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrWhiteSpace(this.tbxAppName.Text))
            {
                this.tbxAppName.HasError = true;
                this.tbxAppName.ToolTip = Properties.Resources.InputRequired;
            }
            else
            {
                this.tbxAppName.HasError = false;
                this.tbxAppName.ToolTip = null;
            }

            if (String.IsNullOrWhiteSpace(this.tbxAppDescription.Text))
            {
                this.tbxAppDescription.HasError = true;
                this.tbxAppDescription.ToolTip = Properties.Resources.InputRequired;
            }
            else
            {
                this.tbxAppDescription.HasError = false;
                this.tbxAppDescription.ToolTip = null;
            }

            if (String.IsNullOrWhiteSpace(this.tbxPublisher.Text))
            {
                this.tbxPublisher.HasError = true;
                this.tbxPublisher.ToolTip = Properties.Resources.InputRequired;
            }
            else
            {
                this.tbxPublisher.HasError = false;
                this.tbxPublisher.ToolTip = null;
            }

            var urlRule = new UrlInputRule { IsRequired = true };

            this.tbxValueName.HasError = !new RequiredInputRule().Validate(this.tbxValueName.Text, null).IsValid;

            if (this.rbtnRegistry.IsChecked.GetValueOrDefault())
            {
                this.tbxAppLocation.HasError = !new RegistryPathRule().Validate(this.tbxAppLocation.Text, null).IsValid;
            }
            else
            {
                this.tbxAppLocation.HasError = !new AppDirectoryRule().Validate(this.tbxAppLocation.Text, null).IsValid;
            }

            this.tbxSuiUrl.HasError = !new SuiLocationRule().Validate(this.tbxSuiUrl.Text, null).IsValid;
            this.tbxAppUrl.HasError = !urlRule.Validate(this.tbxAppUrl.Text, null).IsValid;
            this.tbxHelpUrl.HasError = !urlRule.Validate(this.tbxHelpUrl.Text, null).IsValid;

            this.tbxValueName.ToolTip = this.tbxValueName.HasError ? Properties.Resources.InputRequired : null;
            this.tbxAppLocation.ToolTip = this.tbxAppLocation.HasError ? Properties.Resources.UrlSui : null;
            this.tbxSuiUrl.ToolTip = this.tbxSuiUrl.HasError ? Properties.Resources.FilePathInvalid : null;
            this.tbxAppUrl.ToolTip = this.tbxAppUrl.HasError ? Properties.Resources.UrlNotValid : null;
            this.tbxHelpUrl.ToolTip = this.tbxHelpUrl.HasError ? Properties.Resources.UrlNotValid : null;

            // Load Values
            foreach (var t in Core.AppInfo.Name.Where(t => t.Lang == Utilities.Locale))
            {
                this.tbxAppName.Text = t.Value;
            }

            foreach (var t in Core.AppInfo.Description.Where(t => t.Lang == Utilities.Locale))
            {
                this.tbxAppDescription.Text = t.Value;
            }

            foreach (var t in Core.AppInfo.Publisher.Where(t => t.Lang == Utilities.Locale))
            {
                this.tbxPublisher.Text = t.Value;
            }

            oldPlatform = Core.AppInfo.Platform;
        }

        /// <summary>Loads the <see cref="LocaleString"/>'s for the <see cref="Sua"/> into the UI</summary>
        /// <param name="sender">The object that called the event.</param>
        /// <param name="e">The <see cref="System.Windows.Controls.SelectionChangedEventArgs"/> instance containing the event data.</param>
        private void LoadLocaleStrings(object sender, SelectionChangedEventArgs e)
        {
            if (this.tbxAppName == null || this.cbxLocale.SelectedIndex < 0)
            {
                return;
            }

            Utilities.Locale = ((ComboBoxItem)this.cbxLocale.SelectedItem).Tag.ToString();

            var found = false;

            // Load Values
            foreach (var t in Core.AppInfo.Description.Where(t => t.Lang == Utilities.Locale))
            {
                this.tbxAppDescription.Text = t.Value;
                found = true;
            }

            if (!found)
            {
                this.tbxAppDescription.Text = null;
            }

            found = false;

            // Load Values
            foreach (var t in Core.AppInfo.Name.Where(t => t.Lang == Utilities.Locale))
            {
                this.tbxAppName.Text = t.Value;
                found = true;
            }

            if (!found)
            {
                this.tbxAppName.Text = null;
            }

            found = false;

            // Load Values
            foreach (var t in Core.AppInfo.Publisher.Where(t => t.Lang == Utilities.Locale))
            {
                this.tbxPublisher.Text = t.Value;
                found = true;
            }

            if (!found)
            {
                this.tbxPublisher.Text = null;
            }
        }

        /// <summary>Moves on to the next pages if no errors are present</summary>
        /// <param name="sender">The object that called the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void MoveOn(object sender, RoutedEventArgs e)
        {
            if (!this.HasErrors())
            {
                MainWindow.NavService.Navigate(Core.UpdateInfoPage);
            }
            else
            {
                Core.ShowMessage(Properties.Resources.CorrectErrors, TaskDialogStandardIcon.Error);
            }
        }

        /// <summary>Saves the project and goes back to the main page</summary>
        /// <param name="sender">The object that called the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void SaveSua(object sender, RoutedEventArgs e)
        {
            if (this.btnNext.Visibility != Visibility.Visible)
            {
                if (this.HasErrors())
                {
                    Core.ShowMessage(Properties.Resources.CorrectErrors, TaskDialogStandardIcon.Error);
                }
                else
                {
                    var appName = Utilities.GetLocaleString(Core.AppInfo.Name);
                    var oldAppName = appName;

                    if (Core.AppInfo.Platform == Platform.X64)
                    {
                        if (!appName.Contains("x64") && !appName.Contains("X64"))
                        {
                            appName += " (x64)";
                        }
                    }

                    if (oldPlatform == Platform.X64)
                    {
                        if (!oldAppName.Contains("x64") && !oldAppName.Contains("X64"))
                        {
                            oldAppName += " (x64)";
                        }
                    }

                    File.Delete(Path.Combine(App.UserStore, oldAppName + ".sua"));

                    if (Core.AppInfo.Platform != oldPlatform && String.Compare(oldAppName, appName, StringComparison.OrdinalIgnoreCase) != 0)
                    {
                        File.Copy(Path.Combine(App.UserStore, oldAppName + ".sui"), Path.Combine(App.UserStore, appName + ".sui"), true);
                        File.Delete(Path.Combine(App.UserStore, oldAppName + ".sui"));
                        File.Delete(Path.Combine(App.UserStore, oldAppName + ".sua"));
                    }

                    ObservableCollection<string> updateNames = null;

                    if (Core.AppIndex > -1)
                    {
                        updateNames = Core.Projects[Core.AppIndex].UpdateNames;
                        Core.Projects.RemoveAt(Core.AppIndex);
                    }

                    // Save the SUA file
                    Utilities.Serialize(Core.AppInfo, Path.Combine(App.UserStore, appName + ".sua"));

                    // Save project file
                    var project = new Project { ApplicationName = appName, };

                    if (updateNames != null)
                    {
                        foreach (var t in updateNames)
                        {
                            project.UpdateNames.Add(t);
                        }
                    }

                    Core.Projects.Insert(0, project);
                    Utilities.Serialize(Core.Projects, Core.ProjectsFile);
                    MainWindow.NavService.Navigate(Core.MainPage);
                }
            }
            else
            {
                MainWindow.NavService.Navigate(Core.MainPage);
            }
        }

        /// <summary>Changes the UI depending on whether Aero Glass is enabled.</summary>
        /// <param name="sender">The object that called the event.</param>
        /// <param name="e">The <see cref="CompositionChangedEventArgs"/> instance containing the event data.</param>
        private void UpdateUI(object sender, CompositionChangedEventArgs e)
        {
            if (e.IsGlassEnabled)
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

        /// <summary>Validates the textbox against the AppDirectory Validation Rule</summary>
        /// <param name="sender">The object that called the event.</param>
        /// <param name="e">The data for the event.</param>
        private void ValidateAppDirectory(object sender, TextChangedEventArgs e)
        {
            var textBox = sender as InfoTextBox;

            if (textBox == null)
            {
                return;
            }

            if (this.rbtnRegistry.IsChecked.GetValueOrDefault())
            {
                textBox.HasError = !new RegistryPathRule().Validate(textBox.Text, null).IsValid;
                textBox.ToolTip = textBox.HasError ? Properties.Resources.RegistryKeyInvalid : null;
            }
            else
            {
                textBox.HasError = !new AppDirectoryRule().Validate(textBox.Text, null).IsValid;
                textBox.ToolTip = textBox.HasError ? Properties.Resources.FilePathInvalid : null;
            }
        }

        /// <summary>Validates the textbox content.</summary>
        /// <param name="sender">The object that called the event.</param>
        /// <param name="e">The data for the event</param>
        private void ValidateRequiredInput(object sender, TextChangedEventArgs e)
        {
            var textBox = (InfoTextBox)sender;

            if (String.IsNullOrWhiteSpace(textBox.Text))
            {
                textBox.HasError = true;
                textBox.ToolTip = Properties.Resources.InputRequired;
            }
            else
            {
                textBox.HasError = false;
                textBox.ToolTip = null;
            }
        }

        /// <summary>Validates the textbox against the Sui Validation Rule</summary>
        /// <param name="sender">The object that called the event.</param>
        /// <param name="e">The data for the event.</param>
        private void ValidateSuiLocation(object sender, TextChangedEventArgs e)
        {
            var textBox = sender as InfoTextBox;

            if (textBox == null)
            {
                return;
            }

            textBox.HasError = !new SuiLocationRule().Validate(textBox.Text, null).IsValid;
            textBox.ToolTip = textBox.HasError ? Properties.Resources.UrlSui : null;
        }

        /// <summary>Validates the textbox against the AppDirectory Validation Rule</summary>
        /// <param name="sender">The object that called the event.</param>
        /// <param name="e">The data for the event.</param>
        private void ValidateUrlInput(object sender, TextChangedEventArgs e)
        {
            var textBox = sender as InfoTextBox;

            if (textBox == null)
            {
                return;
            }

            textBox.HasError = !new UrlInputRule { IsRequired = true } .Validate(textBox.Text, null).IsValid;
            textBox.ToolTip = textBox.HasError ? Properties.Resources.UrlNotValid : null;
        }

        #endregion
    }
}