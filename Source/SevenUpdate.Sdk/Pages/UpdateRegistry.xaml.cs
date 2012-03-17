// ***********************************************************************
// <copyright file="UpdateRegistry.xaml.cs" project="SevenUpdate.Sdk" assembly="SevenUpdate.Sdk" solution="SevenUpdate" company="Seven Software">
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
    using System;
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Media;

    using Microsoft.Win32;

    using SevenSoftware.Windows;
    using SevenSoftware.Windows.Controls;
    using SevenSoftware.Windows.Dialogs.TaskDialog;

    using SevenUpdate.Sdk.ValidationRules;
    using SevenUpdate.Sdk.Windows;

    /// <summary>Interaction logic for UpdateRegistry.xaml.</summary>
    public sealed partial class UpdateRegistry
    {
        /// <summary>Initializes a new instance of the <see cref="UpdateRegistry" /> class.</summary>
        public UpdateRegistry()
        {
            this.InitializeComponent();

            this.listBox.ItemsSource = Core.UpdateInfo.RegistryItems;

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

        /// <summary>Deletes the selected <c>RegistryItem</c> from the <c>ListBox</c>.</summary>
        /// <param name="sender">The object that called the event.</param>
        /// <param name="e">The <c>System.Windows.Input.KeyEventArgs</c> instance containing the event data.</param>
        private void DeleteRegistryItem(object sender, KeyEventArgs e)
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

            Core.UpdateInfo.RegistryItems.RemoveAt(index);
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
            // ReSharper disable PossibleNullReferenceException
            return Core.UpdateInfo.RegistryItems.Count != 0 && this.tbxKeyPath.HasError;

            // ReSharper restore PossibleNullReferenceException
        }

        /// <summary>Opens a dialog and imports the selected .reg file.</summary>
        /// <param name="sender">The object that called the event.</param>
        /// <param name="e">The <c>System.Windows.RoutedEventArgs</c> instance containing the event data.</param>
        private void ImportRegistryFile(object sender, RoutedEventArgs e)
        {
            string[] files = Core.OpenFileDialog(
                    Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), null, false, "reg");
            if (files == null)
            {
                return;
            }

            var registryParser = new RegistryParser();
            IEnumerable<RegistryItem> results = registryParser.Parse(files[0]);

            foreach (var t in results)
            {
                Core.UpdateInfo.RegistryItems.Add(t);
            }
        }

        /// <summary>Loads the default values for the UI.</summary>
        /// <param name="sender">The object that called the event.</param>
        /// <param name="e">The <c>System.Windows.RoutedEventArgs</c> instance containing the event data.</param>
        private void LoadUI(object sender, RoutedEventArgs e)
        {
            // ReSharper disable PossibleNullReferenceException
            this.tbxKeyPath.GetBindingExpression(TextBox.TextProperty).UpdateSource();

            // ReSharper restore PossibleNullReferenceException
        }

        /// <summary>Navigates to the next page if no errors exist.</summary>
        /// <param name="sender">The object that called the event.</param>
        /// <param name="e">The <c>System.Windows.RoutedEventArgs</c> instance containing the event data.</param>
        private void MoveOn(object sender, RoutedEventArgs e)
        {
            if (!this.HasErrors())
            {
                MainWindow.NavService.Navigate(Core.UpdateShortcutsPage);
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

        /// <summary>Adds a new <c>RegistryItem</c>.</summary>
        /// <param name="sender">The object that called the event.</param>
        /// <param name="e">The <c>System.Windows.RoutedEventArgs</c> instance containing the event data.</param>
        private void NewRegistryItem(object sender, RoutedEventArgs e)
        {
            var registryItem = new RegistryItem
                {
                        KeyValue = Properties.Resources.NewRegistryItem, 
                        Key = @"HKLM\Software\MyApp", 
                        Action = RegistryAction.Add, 
                        ValueKind = RegistryValueKind.String
                };
            Core.UpdateInfo.RegistryItems.Add(registryItem);
        }

        /// <summary>Removes all <c>RegistryItem</c>'s from the collection.</summary>
        /// <param name="sender">The object that called the event.</param>
        /// <param name="e">The <c>System.Windows.RoutedEventArgs</c> instance containing the event data.</param>
        private void RemoveAll(object sender, RoutedEventArgs e)
        {
            Core.UpdateInfo.RegistryItems.Clear();
        }

        /// <summary>Removes a <c>RegistryItem</c> from a collection.</summary>
        /// <param name="sender">The object that called the event.</param>
        /// <param name="e">The <c>System.Windows.RoutedEventArgs</c> instance containing the event data.</param>
        private void RemoveSelected(object sender, RoutedEventArgs e)
        {
            Core.UpdateInfo.RegistryItems.RemoveAt(this.listBox.SelectedIndex);
        }

        /// <summary>Updates the UI based on whether Aero Glass is enabled.</summary>
        /// <param name="sender">The object that called the event.</param>
        /// <param name="e">The <c>CompositionChangedEventArgs</c> instance containing the event data.</param>
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

        /// <summary>Restricts the input to the characters needed for <c>RegistryValueKind</c>.</summary>
        /// <param name="sender">The object that called the event.</param>
        /// <param name="e">The <c>System.Windows.Input.KeyEventArgs</c> instance containing the event data.</param>
        private void ValidateData(object sender, KeyEventArgs e)
        {
            if (Core.UpdateInfo.RegistryItems[this.listBox.SelectedIndex].ValueKind != RegistryValueKind.Binary
                && Core.UpdateInfo.RegistryItems[this.listBox.SelectedIndex].ValueKind != RegistryValueKind.DWord
                && Core.UpdateInfo.RegistryItems[this.listBox.SelectedIndex].ValueKind != RegistryValueKind.QWord)
            {
                return;
            }

            var converter = new KeyConverter();
            string key = converter.ConvertToString(e.Key);

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

        /// <summary>Validates the registry path.</summary>
        /// <param name="sender">The object that called the event.</param>
        /// <param name="e">The <c>TextChangedEventArgs</c> instance containing the event data.</param>
        private void ValidateRegistryPath(object sender, TextChangedEventArgs e)
        {
            var textBox = sender as InfoTextBox;

            if (textBox == null)
            {
                return;
            }

            textBox.HasError = !new RegistryPathRule().Validate(textBox.Text, null).IsValid;
            textBox.ToolTip = textBox.HasError ? Properties.Resources.FilePathInvalid : null;

            if (!textBox.HasError && this.listBox.SelectedItem != null)
            {
                ((RegistryItem)this.listBox.SelectedItem).Key = textBox.Text;
            }
        }
    }
}