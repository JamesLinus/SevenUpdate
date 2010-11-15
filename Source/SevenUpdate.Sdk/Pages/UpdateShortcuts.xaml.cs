// ***********************************************************************
// <copyright file="UpdateShortcuts.xaml.cs"
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
    using System.Windows.Input;
    using System.Windows.Media;

    using SevenUpdate.Sdk.Windows;

    /// <summary>Interaction logic for UpdateShortcuts.xaml</summary>
    public sealed partial class UpdateShortcuts
    {
        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref = "UpdateShortcuts" /> class.</summary>
        public UpdateShortcuts()
        {
            this.InitializeComponent();

            this.listBox.ItemsSource = Core.UpdateInfo.Shortcuts;

            this.MouseLeftButtonDown += Core.EnableDragOnGlass;
            AeroGlass.CompositionChanged += this.UpdateUI;
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
            var installDirectory = Utilities.IsRegistryKey(Core.AppInfo.Directory) ? Utilities.GetRegistryValue(Core.AppInfo.Directory, Core.AppInfo.ValueName, Core.AppInfo.Is64Bit) : Core.AppInfo.Directory;

            installDirectory = Utilities.ConvertPath(installDirectory, true, Core.AppInfo.Is64Bit);

            var installUrl = fileLocation.Replace(installDirectory, @"%INSTALLDIR%\", true);
            installUrl = installUrl.Replace(@"\\", @"\");

            source.Text = Utilities.ConvertPath(installUrl, false, Core.AppInfo.Is64Bit);
        }

        /// <summary>Deletes the selected UpdateShortcut from the collection</summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.KeyEventArgs"/> instance containing the event data.</param>
        private void DeleteShortcut(object sender, KeyEventArgs e)
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

            Core.UpdateInfo.Shortcuts.RemoveAt(index);
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
            if (Core.UpdateInfo.Shortcuts.Count == 0)
            {
                return false;
            }

            return this.tbxName.HasError || this.tbxSaveLocation.HasError || this.tbxTarget.HasError;
        }

        /// <summary>Opens a dialog to browse for the shortcut to import</summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void ImportShortcut(object sender, RoutedEventArgs e)
        {
            var file = Core.OpenFileDialog(Environment.GetFolderPath(Environment.SpecialFolder.CommonStartMenu), null, false, "lnk", true);

            if (file == null)
            {
                return;
            }

            var importedShortcut = Shortcut.GetShortcutData(file[0]);

            var path = Utilities.ConvertPath(Path.GetDirectoryName(importedShortcut.Location), false, Core.AppInfo.Is64Bit);
            path = path.Replace(Core.AppInfo.Directory, "%INSTALLDIR%");

            var icon = Utilities.ConvertPath(importedShortcut.Icon, false, Core.AppInfo.Is64Bit);
            icon = icon.Replace(Core.AppInfo.Directory, "%INSTALLDIR%");
            var shortcut = new Shortcut { Arguments = importedShortcut.Arguments, Icon = icon, Location = path, Action = ShortcutAction.Update, Target = Utilities.ConvertPath(importedShortcut.Target, false, Core.AppInfo.Is64Bit), };

            shortcut.Name.Add(new LocaleString(Path.GetFileNameWithoutExtension(file[0]), Utilities.Locale));

            Core.UpdateInfo.Shortcuts.Add(shortcut);
            this.listBox.SelectedIndex = Core.UpdateInfo.Shortcuts.Count - 1;
        }

        /// <summary>Load the <see cref="LocaleString"/>'s into the UI</summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Controls.SelectionChangedEventArgs"/> instance containing the event data.</param>
        private void LoadLocaleStrings(object sender, SelectionChangedEventArgs e)
        {
            if (this.tbxDescription == null || this.cbxLocale.SelectedIndex < 0)
            {
                return;
            }

            Utilities.Locale = ((ComboBoxItem)this.cbxLocale.SelectedItem).Tag.ToString();

            var found = false;
            var shortcutDescriptions = Core.UpdateInfo.Shortcuts[this.listBox.SelectedIndex].Description ?? new ObservableCollection<LocaleString>();

            // Load Values
            foreach (var t in shortcutDescriptions.Where(t => t.Lang == Utilities.Locale))
            {
                this.tbxDescription.Text = t.Value;
                found = true;
            }

            if (!found)
            {
                this.tbxDescription.Text = null;
            }

            found = false;
            var shortcutNames = Core.UpdateInfo.Shortcuts[this.listBox.SelectedIndex].Name ?? new ObservableCollection<LocaleString>();

            // Load Values
            foreach (var t in shortcutNames.Where(t => t.Lang == Utilities.Locale))
            {
                this.tbxName.Text = t.Value;
                found = true;
            }

            if (!found)
            {
                this.tbxName.Text = null;
            }
        }

        /// <summary>Opens a dialog to browse for the shortcut icon</summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        private void LocateIcon(object sender, MouseButtonEventArgs e)
        {
            var installDirectory = Utilities.IsRegistryKey(Core.AppInfo.Directory) ? Utilities.GetRegistryValue(Core.AppInfo.Directory, Core.AppInfo.ValueName, Core.AppInfo.Is64Bit) : Core.AppInfo.Directory;

            installDirectory = Utilities.ConvertPath(installDirectory, true, Core.AppInfo.Is64Bit);

            var shortcut = Core.OpenFileDialog(installDirectory);

            if (shortcut == null)
            {
                return;
            }

            var fileUrl = shortcut[0].Replace(installDirectory, @"%INSTALLDIR%\", true);
            fileUrl = fileUrl.Replace(@"\\", @"\");
            Core.UpdateInfo.Shortcuts[this.listBox.SelectedIndex].Icon = fileUrl;
        }

        /// <summary>Opens a dialog to browse for the shortcut location</summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        private void LocateShortcutLocation(object sender, MouseButtonEventArgs e)
        {
            var installDirectory = Utilities.IsRegistryKey(Core.AppInfo.Directory) ? Utilities.GetRegistryValue(Core.AppInfo.Directory, Core.AppInfo.ValueName, Core.AppInfo.Is64Bit) : Core.AppInfo.Directory;

            installDirectory = Utilities.ConvertPath(installDirectory, true, Core.AppInfo.Is64Bit);
            var shortcut = Core.OpenFileDialog(installDirectory, null, false, "lnk", true);

            if (shortcut == null)
            {
                return;
            }

            var saveLoc = Path.GetDirectoryName(shortcut[0]);

            var fileUrl = saveLoc.Replace(installDirectory, @"%INSTALLDIR%\", true);
            fileUrl = fileUrl.Replace(@"\\", @"\");
            Core.UpdateInfo.Shortcuts[this.listBox.SelectedIndex].Location = fileUrl;
        }

        /// <summary>Opens a dialog to browse for the shortcut target</summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        private void LocateShortcutTarget(object sender, MouseButtonEventArgs e)
        {
            var installDirectory = Utilities.IsRegistryKey(Core.AppInfo.Directory) ? Utilities.GetRegistryValue(Core.AppInfo.Directory, Core.AppInfo.ValueName, Core.AppInfo.Is64Bit) : Core.AppInfo.Directory;

            installDirectory = Utilities.ConvertPath(installDirectory, true, Core.AppInfo.Is64Bit);
            var files = Core.OpenFileDialog(installDirectory);
            var fileUrl = files[0].Replace(installDirectory, @"%INSTALLDIR%\", true);
            fileUrl = fileUrl.Replace(@"\\", @"\");
            Core.UpdateInfo.Shortcuts[this.listBox.SelectedIndex].Target = fileUrl;
        }

        /// <summary>Navigates to the next page if no errors exist</summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void MoveOn(object sender, RoutedEventArgs e)
        {
            if (!this.HasErrors())
            {
                MainWindow.NavService.Navigate(new Uri(@"/SevenUpdate.Sdk;component/Pages/UpdateReview.xaml", UriKind.Relative));
            }
            else
            {
                Core.ShowMessage(Properties.Resources.CorrectErrors, TaskDialogStandardIcon.Error);
            }
        }

        /// <summary>Removes all Shortcuts from the collection</summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void RemoveAllShortcuts(object sender, RoutedEventArgs e)
        {
            Core.UpdateInfo.Shortcuts.RemoveAt(this.listBox.SelectedIndex);
        }

        /// <summary>Removes the selected Shortcuts from the collection</summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void RemoveShortcut(object sender, RoutedEventArgs e)
        {
            Core.UpdateInfo.Shortcuts.Clear();
        }

        /// <summary>Sets the selected shortcut</summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Controls.SelectionChangedEventArgs"/> instance containing the event data.</param>
        private void SetSelectedShortcut(object sender, SelectionChangedEventArgs e)
        {
            Core.SelectedShortcut = this.listBox.SelectedIndex;
        }

        /// <summary>Updates the UI based on whether Aero Glass is enabled</summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="CompositionChangedEventArgs"/> instance containing the event data.</param>
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

        #endregion

        /// <summary>Fires the OnPropertyChanged Event with the collection changes</summary>
        /// <param name="sender">The sender</param>
        /// <param name="e">The event data</param>
        private void ChangeName(object sender, RoutedEventArgs e)
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

            Core.UpdateLocaleStrings(textBox.Text, Core.UpdateInfo.Shortcuts[Core.SelectedShortcut].Name);
        }

        /// <summary>Fires the OnPropertyChanged Event with the collection changes</summary>
        /// <param name="sender">The sender</param>
        /// <param name="e">The event data</param>
        private void ChangeDescription(object sender, RoutedEventArgs e)
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

            Core.UpdateLocaleStrings(textBox.Text, Core.UpdateInfo.Shortcuts[Core.SelectedShortcut].Description);
        }
    }
}