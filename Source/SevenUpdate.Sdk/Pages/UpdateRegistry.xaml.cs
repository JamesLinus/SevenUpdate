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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Windows.Dwm;
using SevenUpdate.Sdk.Windows;

#endregion

namespace SevenUpdate.Sdk.Pages
{
    /// <summary>
    ///   Interaction logic for UpdateRegistry.xaml
    /// </summary>
    public sealed partial class UpdateRegistry : Page
    {
        #region Properties

        private bool IsInfoValid { get { return (imgKeyPath.Visibility != Visibility.Visible); } }

        #endregion

        #region Contructors

        /// <summary>
        ///   The constructor for the UpdateRegistry page
        /// </summary>
        public UpdateRegistry()
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

        #region UI Events

        #region TextBox - Text Changed

        private void Textbox_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (tbxKeyPath.Text.Length > 0)
                    new Uri(tbxKeyPath.Text);
                if (tbxKeyPath.Text.Length > 0)
                {
                    if (tbxKeyPath.Text.StartsWith(@"HKLM\", true, null) || tbxKeyPath.Text.StartsWith(@"HKCR\", true, null) || tbxKeyPath.Text.StartsWith(@"HKCU\", true, null) ||
                        tbxKeyPath.Text.StartsWith(@"HKU\", true, null) || tbxKeyPath.Text.StartsWith(@"HKEY_CLASSES_ROOT\") || tbxKeyPath.Text.StartsWith(@"HKEY_CURRENT_USER\", true, null) ||
                        tbxKeyPath.Text.StartsWith(@"HKEY_LOCAL_MACHINE\", true, null) || tbxKeyPath.Text.StartsWith(@"HKEY_USERS\", true, null))
                        imgKeyPath.Visibility = Visibility.Collapsed;
                }
                else
                    imgKeyPath.Visibility = Visibility.Collapsed;
            }
            catch
            {
                imgKeyPath.Visibility = Visibility.Visible;
            }
        }

        #endregion

        #region TextBox - Lost Keyboard Focus

        #endregion

        #region TextBox - Key Down

        private void ValueData_KeyDown(object sender, KeyEventArgs e)
        {
            if (cbxDataType.SelectedIndex != 0 && cbxDataType.SelectedIndex != 1 && cbxDataType.SelectedIndex != 4)
                return;
            var converter = new KeyConverter();
            var key = converter.ConvertToString(e.Key);

            switch (key)
            {
                case "0":
                case "1":
                case "2":
                case "3":
                case "4":
                case "5":
                case "6":
                case "7":
                case "8":
                case "9":
                case "A":
                case "a":
                case "B":
                case "b":
                case "C":
                case "c":
                case "D":
                case "d":
                case "E":
                case "e":
                case "F":
                case "f":
                    break;
                default:
                    e.Handled = true;
                    break;
            }
        }

        #endregion

        #region RadioButton - Checked

        #endregion

        #region Button - Click

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (IsInfoValid)
                MainWindow.NavService.Navigate(new Uri(@"Pages\UpdateShortcuts.xaml", UriKind.Relative));
            else
                App.ShowInputErrorMessage();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.NavService.Navigate(new Uri(@"Pages\Main.xaml", UriKind.Relative));
        }

        #endregion

        #region TextBlock - Mouse Down

        #endregion

        #region MenuItem - Click

        private void AddRegistryItem_Click(object sender, RoutedEventArgs e)
        {
        }

        private void ImportRegistryFile_Click(object sender, RoutedEventArgs e)
        {
        }

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

        #endregion

        #region ComboBox - Selection Changed

        private void DataType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (tbxValueData != null)
                tbxValueData.Text = null;
        }

        private void listBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listBox.Items.Count > 0)
            {
                spHelp.Visibility = Visibility.Collapsed;
                spInput.Visibility = Visibility.Visible;
                miRemove.IsEnabled = listBox.SelectedIndex > -1;
                miRemoveAll.IsEnabled = true;
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

        #region Aero

        private void AeroGlass_DwmCompositionChangedEventHandler(object sender, AeroGlass.DwmCompositionChangedEventArgs e)
        {
            line.Visibility = e.IsGlassEnabled ? Visibility.Collapsed : Visibility.Visible;
            rectangle.Visibility = e.IsGlassEnabled ? Visibility.Collapsed : Visibility.Visible;
        }

        #endregion

        #endregion
    }
}