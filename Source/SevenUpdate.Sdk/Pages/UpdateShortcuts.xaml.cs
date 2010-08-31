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
    public sealed partial class UpdateShortcuts : Page
    {
        #region Fields

        private string locale;

        #endregion

        #region Contructors

        /// <summary>
        ///   The constructor for the UpdateShortcuts page
        /// </summary>
        public UpdateShortcuts()
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

        #region Methods

        private void SaveShortcut()
        {
        }

        private void LoadInfo()
        {
            if (Base.UpdateInfo.Shortcuts != null)
            {
                for (int x = 0; x < Base.UpdateInfo.Shortcuts.Count; x++)
                    listBox.Items.Add(Path.GetFileNameWithoutExtension(Base.UpdateInfo.Shortcuts[x].Location) + " " + x);
            }
        }

        private void LoadShortcutInfo(int index)
        {
            tbxShortcutTarget.Text = Base.UpdateInfo.Shortcuts[listBox.SelectedIndex].Target;
            tbxShortcutPath.Text = Base.UpdateInfo.Shortcuts[listBox.SelectedIndex].Location;
            tbxShortcutIcon.Text = Base.UpdateInfo.Shortcuts[listBox.SelectedIndex].Icon;
            tbxShortcutArguments.Text = Base.UpdateInfo.Shortcuts[listBox.SelectedIndex].Arguments;

            if (rbtnAddShortcut.IsChecked.GetValueOrDefault())
                Base.UpdateInfo.Shortcuts[listBox.SelectedIndex].Action = ShortcutAction.Add;
            if (rbtnUpdateShortcut.IsChecked.GetValueOrDefault())
                Base.UpdateInfo.Shortcuts[listBox.SelectedIndex].Action = ShortcutAction.Update;
            if (rbtnDeleteShortcut.IsChecked.GetValueOrDefault())
                Base.UpdateInfo.Shortcuts[listBox.SelectedIndex].Action = ShortcutAction.Delete;
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
            MainWindow.NavService.Navigate(new Uri(@"Pages\UpdateList.xaml", UriKind.Relative));

            //App.ShowInputErrorMessage();
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
                tbxShortcutTarget.Text = SevenUpdate.Base.ConvertPath(cfd.FileName, false, true);
        }

        private void BrowsePath_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var cfd = new CommonOpenFileDialog {IsFolderPicker = true, Multiselect = false};
            if (cfd.ShowDialog(Application.Current.MainWindow) == CommonFileDialogResult.OK)
                tbxShortcutPath.Text = SevenUpdate.Base.ConvertPath(cfd.FileName, false, true);
        }

        private void BrowseIcon_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var cfd = new CommonOpenFileDialog {IsFolderPicker = true, Multiselect = false};
            if (cfd.ShowDialog(Application.Current.MainWindow) == CommonFileDialogResult.OK)
                tbxShortcutIcon.Text = SevenUpdate.Base.ConvertPath(cfd.FileName, false, true);
        }

        #endregion

        #region MenuItem - Click

        private void AddShortcut_Click(object sender, RoutedEventArgs e)
        {
            //App.ShowInputErrorMessage();

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
                SaveShortcut();
        }

        private void ImportShortcut_Click(object sender, RoutedEventArgs e)
        {
        }

        private void miRemoveAll_Click(object sender, RoutedEventArgs e)
        {
            listBox.Items.Clear();
        }

        private void miRemove_Click(object sender, RoutedEventArgs e)
        {
            listBox.Items.Clear();
            spInput.Visibility = Visibility.Collapsed;
        }

        #endregion

        #region ListBox - Selection Changed

        private void listBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listBox.Items.Count > 0)
            {
                spHelp.Visibility = Visibility.Collapsed;
                spInput.Visibility = Visibility.Visible;

                if (listBox.SelectedIndex > -1 && Base.UpdateInfo.Files != null)
                {
                    if (Base.UpdateInfo.Shortcuts.Count > 0)
                        LoadShortcutInfo(listBox.SelectedIndex);
                }
            }
            else
            {
                spHelp.Visibility = Visibility.Visible;
                spInput.Visibility = Visibility.Collapsed;
            }
        }

        #endregion

        #region ComboBox - Selection Changed

        private void Language_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (tbxShortcutDescription == null)
                return;

            locale = ((ComboBoxItem) cbxLanguage.SelectedItem).Tag.ToString();

            tbxShortcutDescription.Text = null;

            if (Base.UpdateInfo.Shortcuts != null)
                return;
            Base.UpdateInfo.Shortcuts = new ObservableCollection<Shortcut>();

            if (Base.UpdateInfo.Shortcuts.Count < 0)
                return;

            try
            {
                foreach (LocaleString t in Base.UpdateInfo.Description.Where(t => t.Lang == locale))
                    tbxShortcutDescription.Text = t.Value;
            }
            catch
            {
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

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            LoadInfo();
        }

        #endregion

        private void tbxShortcutArguments_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (Base.UpdateInfo.Shortcuts == null)
                Base.UpdateInfo.Shortcuts = new ObservableCollection<Shortcut>();

            if (Base.UpdateInfo.Shortcuts.Count < 0)
                return;

            bool found = false;
            foreach (LocaleString t in Base.UpdateInfo.Shortcuts[listBox.SelectedIndex].Description.Where(t => t.Lang == locale))
            {
                t.Value = tbxShortcutDescription.Text;
                found = true;
            }

            if (found)
                return;

            var ls = new LocaleString {Lang = locale, Value = tbxShortcutDescription.Text};
            Base.UpdateInfo.Description.Add(ls);
        }

        private void listBox_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            if (listBox.Items.Count > -1)
            {
                miRemoveAll.IsEnabled = true;
                miRemove.IsEnabled = listBox.SelectedIndex > -1;
            }
            else
            {
                miRemove.IsEnabled = false;
                miRemoveAll.IsEnabled = false;
            }
        }
    }
}