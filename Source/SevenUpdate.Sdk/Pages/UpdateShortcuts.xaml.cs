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
using Microsoft.Windows.Controls;
using Microsoft.Windows.Dialogs;
using Microsoft.Windows.Dwm;
using SevenUpdate.Base;
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

        #region Properties

        private bool IsInfoValid { get { return (imgShortcutIcon.Visibility != Visibility.Visible && imgShortcutPath.Visibility != Visibility.Visible && imgShortcutTarget.Visibility != Visibility.Visible); } }

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

        private void LoadInfo()
        {
            if (Base.Update.Shortcuts != null)
            {
                for (int x = 0; x < Base.Update.Shortcuts.Count; x++)
                    listBox.Items.Add(Path.GetFileNameWithoutExtension(Base.Update.Shortcuts[x].Location) + " " + x);
            }
        }

        private void LoadShortcutInfo(int index)
        {
            tbxShortcutTarget.Text = Base.Update.Shortcuts[listBox.SelectedIndex].Target;
            tbxShortcutPath.Text = Base.Update.Shortcuts[listBox.SelectedIndex].Location;
            tbxShortcutIcon.Text = Base.Update.Shortcuts[listBox.SelectedIndex].Icon;
            tbxShortcutArguments.Text = Base.Update.Shortcuts[listBox.SelectedIndex].Arguments;

            if (rbtnAddShortcut.IsChecked.GetValueOrDefault())
                Base.Update.Shortcuts[listBox.SelectedIndex].Action = ShortcutAction.Add;
            if (rbtnUpdateShortcut.IsChecked.GetValueOrDefault())
                Base.Update.Shortcuts[listBox.SelectedIndex].Action = ShortcutAction.Update;
            if (rbtnDeleteShortcut.IsChecked.GetValueOrDefault())
                Base.Update.Shortcuts[listBox.SelectedIndex].Action = ShortcutAction.Delete;
        }

        #endregion

        #region UI Events

        #region TextBox - Text Changed Events

        private void Textbox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var source = e.Source as InfoTextBox;
            if (source == null)
                return;
            if (Base.CheckUrl(source.Text))
            {
                switch (source.Name)
                {
                    case "tbxShortcutPath":
                        if (source.Text.EndsWith(".lnk", true, null))
                            imgShortcutPath.Visibility = Visibility.Collapsed;
                        break;

                    case "tbxShortcutTarget":
                        imgShortcutTarget.Visibility = Visibility.Collapsed;
                        break;

                    case "tbxShortcutIcon":
                        imgShortcutIcon.Visibility = Visibility.Collapsed;
                        break;
                }
            }
            else
            {
                switch (source.Name)
                {
                    case "tbxShortcutPath":
                        imgShortcutPath.Visibility = Visibility.Visible;
                        break;

                    case "tbxShortcutTarget":
                        imgShortcutTarget.Visibility = Visibility.Visible;
                        break;

                    case "tbxShortcutIcon":
                        imgShortcutIcon.Visibility = Visibility.Visible;
                        break;
                }
            }
        }

        #endregion

        #region TextBox - Lost Keyboard Focus

        #endregion

        #region RadioButton - Checked

        #endregion

        #region Button - Click

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (IsInfoValid)
                MainWindow.NavService.Navigate(new Uri(@"Pages\UpdateList.xaml", UriKind.Relative));
            else
                App.ShowInputErrorMessage();
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
            if (cfd.ShowDialog() == CommonFileDialogResult.OK)
                tbxShortcutTarget.Text = SevenUpdate.Base.Base.ConvertPath(cfd.FileName, false, true);
        }

        private void BrowsePath_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var cfd = new CommonOpenFileDialog {IsFolderPicker = true, Multiselect = false};
            if (cfd.ShowDialog() == CommonFileDialogResult.OK)
                tbxShortcutPath.Text = SevenUpdate.Base.Base.ConvertPath(cfd.FileName, false, true);
        }

        private void BrowseIcon_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var cfd = new CommonOpenFileDialog {IsFolderPicker = true, Multiselect = false};
            if (cfd.ShowDialog() == CommonFileDialogResult.OK)
                tbxShortcutIcon.Text = SevenUpdate.Base.Base.ConvertPath(cfd.FileName, false, true);
        }

        #endregion

        #region MenuItem - Click

        private void AddShortcut_Click(object sender, RoutedEventArgs e)
        {
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
            miRemoveAll.IsEnabled = false;
            miRemove.IsEnabled = false;
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
                miRemoveAll.IsEnabled = true;
                miRemove.IsEnabled = listBox.SelectedIndex > -1;

                if (listBox.SelectedIndex > -1 && Base.Update.Files != null)
                {
                    if (Base.Update.Shortcuts.Count > 0)
                        LoadShortcutInfo(listBox.SelectedIndex);
                }
            }
            else
            {
                spHelp.Visibility = Visibility.Visible;
                spInput.Visibility = Visibility.Collapsed;
                miRemove.IsEnabled = false;
                miRemoveAll.IsEnabled = false;
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

            if (Base.Update.Shortcuts != null)
                return;
            Base.Update.Shortcuts = new ObservableCollection<Shortcut>();

            if (Base.Update.Shortcuts.Count < 0)
                return;

            try
            {
                foreach (LocaleString t in Base.Update.Description.Where(t => t.Lang == locale))
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
            if (Base.Update.Shortcuts == null)
                Base.Update.Shortcuts = new ObservableCollection<Shortcut>();

            if (Base.Update.Shortcuts.Count < 0)
                return;

            bool found = false;
            foreach (LocaleString t in Base.Update.Shortcuts[listBox.SelectedIndex].Description.Where(t => t.Lang == locale))
            {
                t.Value = tbxShortcutDescription.Text;
                found = true;
            }

            if (found)
                return;

            var ls = new LocaleString {Lang = locale, Value = tbxShortcutDescription.Text};
            Base.Update.Description.Add(ls);
        }
    }
}