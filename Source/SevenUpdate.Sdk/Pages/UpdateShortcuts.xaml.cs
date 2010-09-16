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
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.Windows.Dialogs;
using Microsoft.Windows.Dwm;
using Microsoft.Windows.Shell;
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
            if (Core.UpdateInfo.Shortcuts == null)
                Core.UpdateInfo.Shortcuts = new ObservableCollection<Shortcut>();
            listBox.ItemsSource = Core.UpdateInfo.Shortcuts;

            if (Environment.OSVersion.Version.Major < 6)
                return;

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
                tbTitle.Foreground = new SolidColorBrush(Color.FromRgb(0, 102, 204));
                line.Visibility = Visibility.Visible;
                rectangle.Visibility = Visibility.Visible;
            }
        }

        #endregion

        private bool HasErrors()
        {
            if (Core.UpdateInfo.Shortcuts.Count == 0)
                return false;
            return tbxName.GetBindingExpression(TextBox.TextProperty).HasError || tbxSaveLocation.GetBindingExpression(TextBox.TextProperty).HasError ||
                   tbxTarget.GetBindingExpression(TextBox.TextProperty).HasError;
        }

        #region UI Events

        #region TextBox - Lost Keyboard Focus

        #endregion

        #region RadioButton - Checked

        #endregion

        #region Button - Click

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (!HasErrors())
                MainWindow.NavService.Navigate(new Uri(@"Pages\UpdateReview.xaml", UriKind.Relative));
            else
                Core.ShowMessage(Properties.Resources.CorrectErrors, TaskDialogStandardIcon.Error);
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
                Core.UpdateInfo.Shortcuts[listBox.SelectedIndex].Target = Base.ConvertPath(cfd.FileName, false, Core.AppInfo.Is64Bit);
        }

        private void BrowsePath_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var cfd = new CommonOpenFileDialog {IsFolderPicker = true, Multiselect = false};
            if (cfd.ShowDialog(Application.Current.MainWindow) == CommonFileDialogResult.OK)
                Core.UpdateInfo.Shortcuts[listBox.SelectedIndex].Location = Base.ConvertPath(cfd.FileName, false, Core.AppInfo.Is64Bit);
        }

        private void BrowseIcon_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var cfd = new CommonOpenFileDialog {IsFolderPicker = true, Multiselect = false};
            if (cfd.ShowDialog(Application.Current.MainWindow) == CommonFileDialogResult.OK)
                Core.UpdateInfo.Shortcuts[listBox.SelectedIndex].Icon = Base.ConvertPath(cfd.FileName, false, Core.AppInfo.Is64Bit);
        }

        #endregion

        #region Content Menu

        #region MenuItem - Click

        private void AddShortcut_Click(object sender, RoutedEventArgs e)
        {
            var allUserStartMenu = new StringBuilder(260);
            NativeMethods.SHGetSpecialFolderPath(IntPtr.Zero, allUserStartMenu, FileSystemLocations.CSIDL_COMMON_PROGRAMS, false);

            var cfd = new CommonSaveFileDialog
                          {
                              AlwaysAppendDefaultExtension = true,
                              DefaultExtension = "lnk",
                              DefaultDirectory = allUserStartMenu.ToString(),
                              DefaultFileName = Core.AppInfo.Name[0].Value,
                              EnsureValidNames = true,
                          };
            cfd.Filters.Add(new CommonFileDialogFilter(Properties.Resources.Shortcut, "*.lnk"));
            if (cfd.ShowDialog(Application.Current.MainWindow) != CommonFileDialogResult.OK)
                return;

            var path = Base.ConvertPath(Path.GetDirectoryName(cfd.FileName), false, Core.AppInfo.Is64Bit);
            path = path.Replace(Core.AppInfo.Directory, "%INSTALLDIR%");

            var shortcut = new Shortcut {Location = path, Action = ShortcutAction.Add, Name = new ObservableCollection<LocaleString>(),};
            var ls = new LocaleString {Lang = Base.Locale, Value = Path.GetFileNameWithoutExtension(cfd.FileName)};
            shortcut.Name.Add(ls);
            Core.UpdateInfo.Shortcuts.Add(shortcut);
        }

        private void ImportShortcut_Click(object sender, RoutedEventArgs e)
        {
            var allUserStartMenu = new StringBuilder(260);
            NativeMethods.SHGetSpecialFolderPath(IntPtr.Zero, allUserStartMenu, FileSystemLocations.CSIDL_COMMON_PROGRAMS, false);
            var cfd = new CommonOpenFileDialog {DefaultExtension = "lnk", DefaultDirectory = allUserStartMenu.ToString(), EnsureFileExists = true, NavigateToShortcut = true, Multiselect = false};

            cfd.Filters.Add(new CommonFileDialogFilter(Properties.Resources.Shortcut, "*.lnk"));
            if (cfd.ShowDialog(Application.Current.MainWindow) != CommonFileDialogResult.OK)
                return;

            var importedShortcut = ShortcutInterop.ResolveShortcut(cfd.FileName);

            var path = Base.ConvertPath(Path.GetDirectoryName(importedShortcut.Location), false, Core.AppInfo.Is64Bit);
            path = path.Replace(Core.AppInfo.Directory, "%INSTALLDIR%");

            var icon = Base.ConvertPath(importedShortcut.Icon, false, Core.AppInfo.Is64Bit);
            icon = icon.Replace(Core.AppInfo.Directory, "%INSTALLDIR%");
            var shortcut = new Shortcut
                               {
                                   Arguments = importedShortcut.Arguments,
                                   Icon = icon,
                                   Location = path,
                                   Action = ShortcutAction.Update,
                                   Target = Base.ConvertPath(importedShortcut.Target, false, Core.AppInfo.Is64Bit),
                                   Name = new ObservableCollection<LocaleString>(),
                                   Description = new ObservableCollection<LocaleString>()
                               };

            var ls = new LocaleString {Lang = Base.Locale, Value = importedShortcut.Name};
            shortcut.Name.Add(ls);
            ls = new LocaleString {Lang = Base.Locale, Value = importedShortcut.Description};
            shortcut.Description.Add(ls);

            Core.UpdateInfo.Shortcuts.Add(shortcut);
            listBox.SelectedIndex = (Core.UpdateInfo.Shortcuts.Count - 1);
        }

        private void miRemoveAll_Click(object sender, RoutedEventArgs e)
        {
            Core.UpdateInfo.Shortcuts.Clear();
        }

        private void miRemove_Click(object sender, RoutedEventArgs e)
        {
            Core.UpdateInfo.Shortcuts.RemoveAt(listBox.SelectedIndex);
        }

        #endregion

        #endregion

        #region ComboBox - Selection Changed

        private void Language_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (tbxDescription == null || cbxLocale.SelectedIndex < 0)
                return;

            Base.Locale = ((ComboBoxItem) cbxLocale.SelectedItem).Tag.ToString();

            var found = false;
            var shortcutDescriptions = Core.UpdateInfo.Shortcuts[listBox.SelectedIndex].Description ?? new ObservableCollection<LocaleString>();


            // Load Values
            foreach (var t in shortcutDescriptions.Where(t => t.Lang == Base.Locale))
            {
                tbxDescription.Text = t.Value;
                found = true;
            }
            if (!found)
                tbxDescription.Text = null;

            found = false;
            var shortcutNames = Core.UpdateInfo.Shortcuts[listBox.SelectedIndex].Name ?? new ObservableCollection<LocaleString>();

            // Load Values
            foreach (var t in shortcutNames.Where(t => t.Lang == Base.Locale))
            {
                tbxName.Text = t.Value;
                found = true;
            }
            if (!found)
                tbxName.Text = null;
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
                tbTitle.Foreground = new SolidColorBrush(Color.FromRgb(0, 102, 204));
                line.Visibility = Visibility.Collapsed;
                rectangle.Visibility = Visibility.Collapsed;
            }
        }

        #endregion

        #region ListBox

        private void listBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Core.SelectedShortcut = listBox.SelectedIndex;
        }

        private void listBox_KeyDown(object sender, KeyEventArgs e)
        {
            var index = listBox.SelectedIndex;
            if (index < 0)
                return;
            if (e.Key != Key.Delete)
                return;

            Core.UpdateInfo.Shortcuts.RemoveAt(index);
            listBox.SelectedIndex = (index - 1);

            if (listBox.SelectedIndex < 0 && listBox.Items.Count > 0)
                listBox.SelectedIndex = 0;
        }

        #endregion

        #endregion
    }
}