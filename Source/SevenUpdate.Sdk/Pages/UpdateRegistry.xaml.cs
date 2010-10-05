// Copyright 2007-2010 Robert Baker, Seven Software.
// This file is part of Seven Update.
// Seven Update is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
// Seven Update is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.
// You should have received a copy of the GNU General Public License along with Seven Update.  If not, see <http://www.gnu.org/licenses/>.

namespace SevenUpdate.Sdk.Pages
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Media;

    using Microsoft.Win32;
    using Microsoft.Windows.Dialogs;
    using Microsoft.Windows.Dwm;

    using SevenUpdate.Sdk.Windows;

    /// <summary>
    /// Interaction logic for UpdateRegistry.xaml
    /// </summary>
    public sealed partial class UpdateRegistry
    {
        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the <see cref = "UpdateRegistry" /> class.
        /// </summary>
        public UpdateRegistry()
        {
            this.InitializeComponent();

            this.listBox.ItemsSource = Core.UpdateInfo.RegistryItems;

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

        /// <summary>
        /// Deletes the selected <see cref="RegistryItem"/> from the <see cref="ListBox"/>
        /// </summary>
        /// <param name="sender">
        /// The source of the event.
        /// </param>
        /// <param name="e">
        /// The <see cref="System.Windows.Input.KeyEventArgs"/> instance containing the event data.
        /// </param>
        private void DeleteRegistryItem(object sender, KeyEventArgs e)
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

            Core.UpdateInfo.RegistryItems.RemoveAt(index);
            this.listBox.SelectedIndex = index - 1;

            if (this.listBox.SelectedIndex < 0 && this.listBox.Items.Count > 0)
            {
                this.listBox.SelectedIndex = 0;
            }
        }

        /// <summary>
        /// Navigates to the main page
        /// </summary>
        /// <param name="sender">
        /// The source of the event.
        /// </param>
        /// <param name="e">
        /// The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.
        /// </param>
        private void GoToMainPage(object sender, RoutedEventArgs e)
        {
            MainWindow.NavService.Navigate(new Uri(@"/SevenUpdate.Sdk;component/Pages/Main.xaml", UriKind.Relative));
        }

        /// <summary>
        /// Determines whether this instance has errors.
        /// </summary>
        /// <returns>
        /// <see langword="true"/> if this instance has errors; otherwise, <see langword="false"/>.
        /// </returns>
        private bool HasErrors()
        {
            // ReSharper disable PossibleNullReferenceException
            return Core.UpdateInfo.RegistryItems.Count != 0 && this.tbxKeyPath.GetBindingExpression(TextBox.TextProperty).HasError;

            // ReSharper restore PossibleNullReferenceException
        }

        /// <summary>
        /// Opens a <see cref="CommonOpenFileDialog"/> and imports the selected .reg file
        /// </summary>
        /// <param name="sender">
        /// The source of the event.
        /// </param>
        /// <param name="e">
        /// The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.
        /// </param>
        private void ImportRegistryFile(object sender, RoutedEventArgs e)
        {
            var files = Core.OpenFileDialog(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), false, null, null, "reg");
            if (files == null)
            {
                return;
            }

            var registryParser = new RegistryParser();
            var results = registryParser.Parse(files[0]);

            foreach (var t in results)
            {
                Core.UpdateInfo.RegistryItems.Add(t);
            }
        }

        /// <summary>
        /// Loads the default values for the UI
        /// </summary>
        /// <param name="sender">
        /// The source of the event.
        /// </param>
        /// <param name="e">
        /// The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.
        /// </param>
        private void LoadUI(object sender, RoutedEventArgs e)
        {
            // ReSharper disable PossibleNullReferenceException
            this.tbxKeyPath.GetBindingExpression(TextBox.TextProperty).UpdateSource();

            // ReSharper restore PossibleNullReferenceException
        }

        /// <summary>
        /// Navigates to the next page if no errors exist
        /// </summary>
        /// <param name="sender">
        /// The source of the event.
        /// </param>
        /// <param name="e">
        /// The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.
        /// </param>
        private void MoveOn(object sender, RoutedEventArgs e)
        {
            if (!this.HasErrors())
            {
                MainWindow.NavService.Navigate(new Uri(@"/SevenUpdate.Sdk;component/Pages/UpdateShortcuts.xaml", UriKind.Relative));
            }
            else
            {
                Core.ShowMessage(Properties.Resources.CorrectErrors, TaskDialogStandardIcon.Error);
            }
        }

        /// <summary>
        /// Adds a new <see cref="RegistryItem"/>
        /// </summary>
        /// <param name="sender">
        /// The source of the event.
        /// </param>
        /// <param name="e">
        /// The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.
        /// </param>
        private void NewRegistryItem(object sender, RoutedEventArgs e)
        {
            var registryItem = new RegistryItem
                {
                   KeyValue = Properties.Resources.NewRegistryItem, Key = @"HKLM\Software\MyApp", Action = RegistryAction.Add, ValueKind = RegistryValueKind.String 
                };
            Core.UpdateInfo.RegistryItems.Add(registryItem);
        }

        /// <summary>
        /// Removes all <see cref="RegistryItem"/>'s from the collection
        /// </summary>
        /// <param name="sender">
        /// The source of the event.
        /// </param>
        /// <param name="e">
        /// The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.
        /// </param>
        private void RemoveAll(object sender, RoutedEventArgs e)
        {
            Core.UpdateInfo.RegistryItems.Clear();
        }

        /// <summary>
        /// Removes a <see cref="RegistryItem"/> from a collection
        /// </summary>
        /// <param name="sender">
        /// The source of the event.
        /// </param>
        /// <param name="e">
        /// The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.
        /// </param>
        private void RemoveSelected(object sender, RoutedEventArgs e)
        {
            Core.UpdateInfo.RegistryItems.RemoveAt(this.listBox.SelectedIndex);
        }

        /// <summary>
        /// Updates the UI based on whether Aero Glass is enabled
        /// </summary>
        /// <param name="sender">
        /// The source of the event.
        /// </param>
        /// <param name="e">
        /// The <see cref="Microsoft.Windows.Dwm.AeroGlass.DwmCompositionChangedEventArgs"/> instance containing the event data.
        /// </param>
        private void UpdateUI(object sender, AeroGlass.DwmCompositionChangedEventArgs e)
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

        /// <summary>
        /// Restricts the input to the characters needed for <see cref="RegistryValueKind"/>
        /// </summary>
        /// <param name="sender">
        /// The source of the event.
        /// </param>
        /// <param name="e">
        /// The <see cref="System.Windows.Input.KeyEventArgs"/> instance containing the event data.
        /// </param>
        private void ValidateData(object sender, KeyEventArgs e)
        {
            if (Core.UpdateInfo.RegistryItems[this.listBox.SelectedIndex].ValueKind != RegistryValueKind.Binary &&
                Core.UpdateInfo.RegistryItems[this.listBox.SelectedIndex].ValueKind != RegistryValueKind.DWord &&
                Core.UpdateInfo.RegistryItems[this.listBox.SelectedIndex].ValueKind != RegistryValueKind.QWord)
            {
                return;
            }

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
    }
}