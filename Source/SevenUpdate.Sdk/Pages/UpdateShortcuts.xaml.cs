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
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Windows.Dialogs;
using Microsoft.Windows.Dwm;
using SevenUpdate.Sdk.Windows;

#endregion

namespace SevenUpdate.Sdk.Pages
{
    /// <summary>
    ///   Interaction logic for UpdateShortcuts.xaml
    /// </summary>
    public sealed partial class UpdateShortcuts
    {
        #region Contructors

        /// <summary>
        ///   The constructor for the UpdateShortcuts page
        /// </summary>
        public UpdateShortcuts()
        {
            InitializeComponent();
            if (Base.UpdateInfo.Shortcuts == null)
                Base.UpdateInfo.Shortcuts = new ObservableCollection<Shortcut>();
            listBox.ItemsSource = Base.UpdateInfo.Shortcuts;

            if (Environment.OSVersion.Version.Major < 6)
                return;

            MouseLeftButtonDown += App.Rectangle_MouseLeftButtonDown;
            AeroGlass.DwmCompositionChanged += AeroGlass_DwmCompositionChanged;
            line.Visibility = AeroGlass.IsEnabled ? Visibility.Collapsed : Visibility.Visible;
            rectangle.Visibility = AeroGlass.IsEnabled ? Visibility.Collapsed : Visibility.Visible;
        }

        #endregion

        #region Methods

        private static void SaveShortcut(string fileName)
        {
            var shortcut = new Shortcut {Location = fileName, Action = ShortcutAction.Add};
            Base.UpdateInfo.Shortcuts.Add(shortcut);
        }

        #endregion

        #region UI Events

        #region TextBox - Lost Keyboard Focus

        #endregion

        #region RadioButton - Checked

        #endregion

        #region Button - Click

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (Base.UpdateInfo.Shortcuts.Count != -1)
                MainWindow.NavService.Navigate(new Uri(@"Pages\UpdateReview.xaml", UriKind.Relative));
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.NavService.Navigate(new Uri(@"Pages\Main.xaml", UriKind.Relative));
        }

        #endregion

        #region TextBlock - Mouse Down

        private void BrowseTarget_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var cfd = new CommonOpenFileDialog {IsFolderPicker = true, Multiselect = false};
            if (cfd.ShowDialog(Application.Current.MainWindow) == CommonFileDialogResult.OK)
                Base.UpdateInfo.Shortcuts[listBox.SelectedIndex].Target = SevenUpdate.Base.ConvertPath(cfd.FileName, false, true);
        }

        private void BrowsePath_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var cfd = new CommonOpenFileDialog {IsFolderPicker = true, Multiselect = false};
            if (cfd.ShowDialog(Application.Current.MainWindow) == CommonFileDialogResult.OK)
                Base.UpdateInfo.Shortcuts[listBox.SelectedIndex].Location = SevenUpdate.Base.ConvertPath(cfd.FileName, false, true);
        }

        private void BrowseIcon_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var cfd = new CommonOpenFileDialog {IsFolderPicker = true, Multiselect = false};
            if (cfd.ShowDialog(Application.Current.MainWindow) == CommonFileDialogResult.OK)
                Base.UpdateInfo.Shortcuts[listBox.SelectedIndex].Icon = SevenUpdate.Base.ConvertPath(cfd.FileName, false, true);
        }

        #endregion

        #region Content Menu

        #region MenuItem - Click

        private void AddShortcut_Click(object sender, RoutedEventArgs e)
        {
            var cfd = new CommonSaveFileDialog
                          {
                              AlwaysAppendDefaultExtension = true,
                              DefaultExtension = "lnk",
                              DefaultDirectory = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory),
                              DefaultFileName = Base.AppInfo.Name[0].Value,
                              EnsureValidNames = true
                          };
            cfd.Filters.Add(new CommonFileDialogFilter(Properties.Resources.Shortcut, "*.lnk"));
            if (cfd.ShowDialog(Application.Current.MainWindow) == CommonFileDialogResult.OK)
                SaveShortcut(cfd.FileName);
        }

        private void ImportShortcut_Click(object sender, RoutedEventArgs e)
        {
        }

        private void miRemoveAll_Click(object sender, RoutedEventArgs e)
        {
            Base.UpdateInfo.Shortcuts.Clear();
        }

        private void miRemove_Click(object sender, RoutedEventArgs e)
        {
            listBox.Items.RemoveAt(listBox.SelectedIndex);
        }

        #endregion

        #endregion

        #region ComboBox - Selection Changed

        private void Language_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (tbxShortcutDescription == null || cbxLocale.SelectedIndex < 0)
                return;

            SevenUpdate.Base.Locale = ((ComboBoxItem) cbxLocale.SelectedItem).Tag.ToString();

            var found = false;
            var shortcutDescriptions = ((Shortcut) listBox.SelectedItem).Description;
            // Load Values
            foreach (var t in shortcutDescriptions.Where(t => t.Lang == SevenUpdate.Base.Locale))
            {
                tbxShortcutDescription.Text = t.Value;
                found = true;
            }
            if (!found)
                tbxShortcutDescription.Text = null;
        }

        #endregion

        #region Aero

        private void AeroGlass_DwmCompositionChanged(object sender, AeroGlass.DwmCompositionChangedEventArgs e)
        {
            line.Visibility = e.IsGlassEnabled ? Visibility.Collapsed : Visibility.Visible;
            rectangle.Visibility = e.IsGlassEnabled ? Visibility.Collapsed : Visibility.Visible;
        }

        #endregion

        #endregion

        private void ShortcutDescription_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (Base.UpdateInfo.Shortcuts == null)
                Base.UpdateInfo.Shortcuts = new ObservableCollection<Shortcut>();

            if (Base.UpdateInfo.Shortcuts.Count < 0)
                return;

            var found = false;
            foreach (var t in Base.UpdateInfo.Shortcuts[listBox.SelectedIndex].Description.Where(t => t.Lang == SevenUpdate.Base.Locale))
            {
                t.Value = tbxShortcutDescription.Text;
                found = true;
            }

            if (found)
                return;

            var ls = new LocaleString {Lang = SevenUpdate.Base.Locale, Value = tbxShortcutDescription.Text};
            Base.UpdateInfo.Shortcuts[listBox.SelectedIndex].Description.Add(ls);
        }
    }
}