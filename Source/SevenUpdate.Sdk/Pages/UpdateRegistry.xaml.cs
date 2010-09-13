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
using System.Windows.Media;
using Microsoft.Win32;
using Microsoft.Windows.Dialogs;
using Microsoft.Windows.Dwm;
using SevenUpdate.Sdk.Windows;

#endregion

namespace SevenUpdate.Sdk.Pages
{
    /// <summary>
    ///   Interaction logic for UpdateRegistry.xaml
    /// </summary>
    public sealed partial class UpdateRegistry
    {
        #region Contructors

        /// <summary>
        ///   The constructor for the UpdateRegistry page
        /// </summary>
        public UpdateRegistry()
        {
            InitializeComponent();

            listBox.ItemsSource = Core.UpdateInfo.RegistryItems;

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

        #region Methods

        private bool HasErrors()
        {
            return Core.UpdateInfo.RegistryItems.Count != 0 && tbxKeyPath.GetBindingExpression(TextBox.TextProperty).HasError;
        }

        #endregion

        #region UI Events

        #region TextBox - Key Down

        private void ValueData_KeyDown(object sender, KeyEventArgs e)
        {
            if (Core.UpdateInfo.RegistryItems[listBox.SelectedIndex].ValueKind != RegistryValueKind.Binary && Core.UpdateInfo.RegistryItems[listBox.SelectedIndex].ValueKind != RegistryValueKind.DWord &&
                Core.UpdateInfo.RegistryItems[listBox.SelectedIndex].ValueKind != RegistryValueKind.QWord)
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
            MainWindow.NavService.Navigate(new Uri(@"Pages\UpdateShortcuts.xaml", UriKind.Relative));
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            if (!HasErrors())
                MainWindow.NavService.Navigate(new Uri(@"Pages\Main.xaml", UriKind.Relative));
            else
                Core.ShowMessage(Properties.Resources.CorrectErrors, TaskDialogStandardIcon.Error);
        }

        #endregion

        #region TextBlock - Mouse Down

        #endregion

        #region Content Menu

        #region MenuItem - Click

        private void AddRegistryItem_Click(object sender, RoutedEventArgs e)
        {
            var registryItem = new RegistryItem {KeyValue = Properties.Resources.NewRegistryItem, Key = @"HKLM\Software\MyApp", Action = RegistryAction.Add, ValueKind = RegistryValueKind.String};
            Core.UpdateInfo.RegistryItems.Add(registryItem);
        }

        private void ImportRegistryFile_Click(object sender, RoutedEventArgs e)
        {
            var cfd = new CommonOpenFileDialog {Multiselect = false, DefaultExtension = "reg"};
            cfd.Filters.Add(new CommonFileDialogFilter("Registry file", "*.reg"));
            if (cfd.ShowDialog(Application.Current.MainWindow) != CommonFileDialogResult.OK)
                return;

            var registryParser = new RegistryParser();
            var results = registryParser.Parse(cfd.FileName);

            foreach (var t in results)
                Core.UpdateInfo.RegistryItems.Add(t);
        }

        private void miRemove_Click(object sender, RoutedEventArgs e)
        {
            Core.UpdateInfo.RegistryItems.RemoveAt(listBox.SelectedIndex);
        }

        private void miRemoveAll_Click(object sender, RoutedEventArgs e)
        {
            Core.UpdateInfo.RegistryItems.Clear();
        }

        #endregion

        #endregion

        #region ComboBox - Selection Changed

        #endregion

        #region Aero

        private void AeroGlass_DwmCompositionChanged(object sender, AeroGlass.DwmCompositionChangedEventArgs e)
        {
            if (e.IsGlassEnabled)
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

        private void listBox_KeyDown(object sender, KeyEventArgs e)
        {
            var index = listBox.SelectedIndex;
            if (index < 0)
                return;
            if (e.Key != Key.Delete)
                return;

            Core.UpdateInfo.RegistryItems.RemoveAt(index);
            listBox.SelectedIndex = (index - 1);

            if (listBox.SelectedIndex < 0 && listBox.Items.Count > 0)
                listBox.SelectedIndex = 0;
        }

        #endregion

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            tbxKeyPath.GetBindingExpression(TextBox.TextProperty).UpdateSource();
        }
    }
}