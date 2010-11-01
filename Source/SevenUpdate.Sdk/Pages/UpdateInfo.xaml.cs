// ***********************************************************************
// <copyright file="UpdateInfo.xaml.cs"
//            project="SevenUpdate.Sdk"
//            assembly="SevenUpdate.Sdk"
//            solution="SevenUpdate"
//            company="Seven Software">
//     Copyright (c) Seven Software. All rights reserved.
// </copyright>
// <author username="sevenalive">Robert Baker</author>
// <license href="http://www.gnu.org/licenses/gpl-3.0.txt" name="GNU General Public License 3">
//  This file is part of Seven Update.
//
//    Seven Update is free software: you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation, either version 3 of the License, or
//    (at your option) any later version.
//
//    Seven Update is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.
//
//    You should have received a copy of the GNU General Public License
//    along with Seven Update.  If not, see http://www.gnu.org/licenses/.
// </license>
// ***********************************************************************
namespace SevenUpdate.Sdk.Pages
{
    using System;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Dialogs;
    using System.Windows.Media;

    using SevenUpdate.Sdk.Windows;

    /// <summary>Interaction logic for UpdateInfo.xaml</summary>
    public sealed partial class UpdateInfo
    {
        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref = "UpdateInfo" /> class.</summary>
        public UpdateInfo()
        {
            this.InitializeComponent();
            this.DataContext = Core.UpdateInfo;
            if (String.IsNullOrWhiteSpace(Core.UpdateInfo.ReleaseDate))
            {
                Core.UpdateInfo.ReleaseDate = DateTime.Now.ToShortDateString();
            }

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

        /// <summary>Navigates to the main page</summary>
        /// <param name = "sender">The source of the event.</param>
        /// <param name = "e">The <see cref = "System.Windows.RoutedEventArgs" /> instance containing the event data.</param>
        private void GoToMainPage(object sender, RoutedEventArgs e)
        {
            MainWindow.NavService.Navigate(new Uri(@"/SevenUpdate.Sdk;component/Pages/Main.xaml", UriKind.Relative));
        }

        /// <summary>Determines whether this instance has errors.</summary>
        /// <returns><see langword = "true" /> if this instance has errors; otherwise, <see langword = "false" />.</returns>
        private bool HasErrors()
        {
            return tbxUpdateName.HasError || tbxUpdateDetails.HasError || tbxSourceLocation.HasError || this.imgReleaseDate.Visibility == Visibility.Visible;
        }

        /// <summary>Loads the <see cref = "LocaleString" />'s for the <see cref = "Update" /> into the UI</summary>
        /// <param name = "sender">The source of the event.</param>
        /// <param name = "e">The <see cref = "System.Windows.Controls.SelectionChangedEventArgs" /> instance containing the event data.</param>
        private void LoadLocaleStrings(object sender, SelectionChangedEventArgs e)
        {
            if (this.tbxUpdateName == null || this.cbxLocale.SelectedIndex < 0)
            {
                return;
            }

            Utilities.Locale = ((ComboBoxItem)this.cbxLocale.SelectedItem).Tag.ToString();

            var found = false;

            // Load Values
            foreach (var t in Core.UpdateInfo.Name.Where(t => t.Lang == Utilities.Locale))
            {
                this.tbxUpdateName.Text = t.Value;
                found = true;
            }

            if (!found)
            {
                this.tbxUpdateName.Text = null;
            }

            found = false;

            // Load Values
            foreach (var t in Core.UpdateInfo.Description.Where(t => t.Lang == Utilities.Locale))
            {
                this.tbxUpdateDetails.Text = t.Value;
                found = true;
            }

            if (!found)
            {
                this.tbxUpdateDetails.Text = null;
            }
        }

        /// <summary>Loads the <see cref = "Update" /> information to the UI</summary>
        /// <param name = "sender">The source of the event.</param>
        /// <param name = "e">The <see cref = "System.Windows.RoutedEventArgs" /> instance containing the event data.</param>
        private void LoadUI(object sender, RoutedEventArgs e)
        {
            tbxUpdateName.HasError = String.IsNullOrWhiteSpace(tbxUpdateName.Text);
            tbxUpdateDetails.HasError = String.IsNullOrWhiteSpace(tbxUpdateDetails.Text);

            // ReSharper disable PossibleNullReferenceException
            this.tbxSourceLocation.GetBindingExpression(TextBox.TextProperty).UpdateSource();

            // ReSharper restore PossibleNullReferenceException

            // Load Values
            foreach (var t in Core.UpdateInfo.Description.Where(t => t.Lang == Utilities.Locale))
            {
                this.tbxUpdateDetails.Text = t.Value;
            }

            foreach (var t in Core.UpdateInfo.Name.Where(t => t.Lang == Utilities.Locale))
            {
                this.tbxUpdateName.Text = t.Value;
            }
        }

        /// <summary>Moves on to the next pages if no errors are present</summary>
        /// <param name = "sender">The source of the event.</param>
        /// <param name = "e">The <see cref = "System.Windows.RoutedEventArgs" /> instance containing the event data.</param>
        private void MoveOn(object sender, RoutedEventArgs e)
        {
            if (!this.HasErrors())
            {
                MainWindow.NavService.Navigate(new Uri(@"/SevenUpdate.Sdk;component/Pages/UpdateFiles.xaml", UriKind.Relative));
            }
            else
            {
                Core.ShowMessage(Properties.Resources.CorrectErrors, TaskDialogStandardIcon.Error);
            }
        }

        /// <summary>Updates the UI based on whether Aero Glass is enabled</summary>
        /// <param name = "sender">The source of the event.</param>
        /// <param name = "e">The <see cref = "CompositionChangedEventArgs" /> instance containing the event data.</param>
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
            var textBox = ((InfoTextBox)sender);
            textBox.HasError = String.IsNullOrWhiteSpace(textBox.Text);
            Core.UpdateLocaleStrings(textBox.Text, Core.UpdateInfo.Name);
        }

        /// <summary>Fires the OnPropertyChanged Event with the collection changes</summary>
        /// <param name="sender">The sender</param>
        /// <param name="e">The event data</param>
        private void ChangeDescription(object sender, RoutedEventArgs e)
        {
            var textBox = ((InfoTextBox)sender);
            textBox.HasError = String.IsNullOrWhiteSpace(textBox.Text);
            Core.UpdateLocaleStrings(textBox.Text, Core.UpdateInfo.Description);
        }
    }
}