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
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.Windows.Dialogs;
using Microsoft.Windows.Dwm;
using SevenUpdate.Sdk.Windows;

#endregion

namespace SevenUpdate.Sdk.Pages
{
    /// <summary>
    ///   Interaction logic for UpdateInfo.xaml
    /// </summary>
    public sealed partial class UpdateInfo
    {
        #region Properties

        #endregion

        #region Constructors

        /// <summary>
        ///   The constructor for the UpdateInfo page
        /// </summary>
        public UpdateInfo()
        {
            InitializeComponent();
            DataContext = Core.UpdateInfo;

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
                tbTitle.Foreground = new SolidColorBrush(Color.FromRgb(0, 51, 153));
                line.Visibility = Visibility.Visible;
                rectangle.Visibility = Visibility.Visible;
            }
        }

        #endregion

        private bool HasErrors()
        {
            return tbxUpdateName.GetBindingExpression(TextBox.TextProperty).HasError || tbxUpdateDetails.GetBindingExpression(TextBox.TextProperty).HasError ||
                   tbxSourceLocation.GetBindingExpression(TextBox.TextProperty).HasError || imgReleaseDate.Visibility == Visibility.Visible;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            LoadInfo();
        }

        #region UI Events

        #region Button - Click

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (!HasErrors())
                MainWindow.NavService.Navigate(new Uri(@"/SevenUpdate.Sdk;component/Pages/UpdateFiles.xaml", UriKind.Relative));
            else
                Core.ShowMessage(Properties.Resources.CorrectErrors, TaskDialogStandardIcon.Error);
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.NavService.Navigate(new Uri(@"/SevenUpdate.Sdk;component/Pages/Main.xaml", UriKind.Relative));
        }

        #endregion

        #region ComboBox Selection Changed

        private void Locale_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (tbxUpdateName == null || cbxLocale.SelectedIndex < 0)
                return;

            Base.Locale = ((ComboBoxItem) cbxLocale.SelectedItem).Tag.ToString();

            var found = false;
            // Load Values
            foreach (var t in Core.UpdateInfo.Name.Where(t => t.Lang == Base.Locale))
            {
                tbxUpdateName.Text = t.Value;
                found = true;
            }
            if (!found)
                tbxUpdateName.Text = null;


            found = false;
            // Load Values
            foreach (var t in Core.UpdateInfo.Description.Where(t => t.Lang == Base.Locale))
            {
                tbxUpdateDetails.Text = t.Value;
                found = true;
            }
            if (!found)
                tbxUpdateDetails.Text = null;
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
                tbTitle.Foreground = new SolidColorBrush(Color.FromRgb(0, 51, 153));
                line.Visibility = Visibility.Collapsed;
                rectangle.Visibility = Visibility.Collapsed;
            }
        }

        #endregion

        #endregion

        #region Methods

        private void LoadInfo()
        {
            // ReSharper disable PossibleNullReferenceException
            tbxUpdateName.GetBindingExpression(TextBox.TextProperty).UpdateSource();

            tbxUpdateDetails.GetBindingExpression(TextBox.TextProperty).UpdateSource();

            tbxSourceLocation.GetBindingExpression(TextBox.TextProperty).UpdateSource();
            // ReSharper restore PossibleNullReferenceException

            // Load Values
            foreach (var t in Core.UpdateInfo.Description.Where(t => t.Lang == Base.Locale))
                tbxUpdateDetails.Text = t.Value;

            foreach (var t in Core.UpdateInfo.Name.Where(t => t.Lang == Base.Locale))
                tbxUpdateName.Text = t.Value;
        }

        #endregion
    }
}