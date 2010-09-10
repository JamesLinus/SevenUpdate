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
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.Windows.Dialogs;
using Microsoft.Windows.Dwm;
using SevenUpdate.Sdk.Helpers;
using SevenUpdate.Sdk.Windows;

#endregion

namespace SevenUpdate.Sdk.Pages
{
    /// <summary>
    ///   Interaction logic for AppInfo.xaml
    /// </summary>
    public sealed partial class AppInfo
    {
        #region Properties

        #endregion

        #region Constructors

        /// <summary>
        ///   The constructor for the AppInfo page
        /// </summary>
        public AppInfo()
        {
            InitializeComponent();
            DataContext = Core.AppInfo;

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

        private void LoadInfo()
        {
            // ReSharper disable PossibleNullReferenceException
            tbxPublisher.GetBindingExpression(TextBox.TextProperty).UpdateSource();
            tbxAppDescription.GetBindingExpression(TextBox.TextProperty).UpdateSource();
            tbxAppName.GetBindingExpression(TextBox.TextProperty).UpdateSource();
            tbxAppLocation.GetBindingExpression(TextBox.TextProperty).UpdateSource();
            tbxValueName.GetBindingExpression(TextBox.TextProperty).UpdateSource();
            // ReSharper restore PossibleNullReferenceException
            // Load Values
            foreach (var t in Core.AppInfo.Name.Where(t => t.Lang == Base.Locale))
                tbxAppName.Text = t.Value;

            foreach (var t in Core.AppInfo.Description.Where(t => t.Lang == Base.Locale))
                tbxAppDescription.Text = t.Value;

            foreach (var t in Core.AppInfo.Publisher.Where(t => t.Lang == Base.Locale))
                tbxPublisher.Text = t.Value;
        }

        private bool ValidateInfo()
        {
            return Core.AppInfo.Name.Count > 0 && Core.AppInfo.Publisher.Count > 0 && Core.AppInfo.Description.Count > 0;
        }

        #endregion

        #region UI Events

        #region Button - Click

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.NavService.Navigate(new Uri(@"Pages\UpdateInfo.xaml", UriKind.Relative));
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.NavService.Navigate(new Uri(@"Pages\Main.xaml", UriKind.Relative));
        }

        #endregion

        #region TextBlock - Mouse Down

        private void Browse_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var cfd = new CommonOpenFileDialog {IsFolderPicker = true, Multiselect = false};
            if (cfd.ShowDialog(Application.Current.MainWindow) == CommonFileDialogResult.OK)
                tbxAppLocation.Text = Base.ConvertPath(cfd.FileName, false, Core.AppInfo.Is64Bit);
        }

        #endregion

        #region TextBlock - Lost Focus

        private void tbxAppLocation_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (rbtnFileSystem.IsChecked.GetValueOrDefault())
                tbxAppLocation.Text = Base.ConvertPath(tbxAppLocation.Text, false, Core.AppInfo.Is64Bit);
        }

        #endregion

        #region ComboBox - Selection Changed

        private void Locale_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (tbxAppName == null || cbxLocale.SelectedIndex < 0)
                return;

            Base.Locale = ((ComboBoxItem) cbxLocale.SelectedItem).Tag.ToString();

            var found = false;
            // Load Values
            foreach (var t in Core.AppInfo.Description.Where(t => t.Lang == Base.Locale))
            {
                tbxAppDescription.Text = t.Value;
                found = true;
            }
            if (!found)
                tbxAppDescription.Text = null;

            found = false;
            // Load Values
            foreach (var t in Core.AppInfo.Name.Where(t => t.Lang == Base.Locale))
            {
                tbxAppName.Text = t.Value;
                found = true;
            }
            if (!found)
                tbxAppName.Text = null;


            found = false;
            // Load Values
            foreach (var t in Core.AppInfo.Publisher.Where(t => t.Lang == Base.Locale))
            {
                tbxPublisher.Text = t.Value;
                found = true;
            }
            if (!found)
                tbxPublisher.Text = null;
        }

        #endregion

        #region Radio Button - Checked

        private void rbtnRegistry_Checked(object sender, RoutedEventArgs e)
        {
            if (tbxAppLocation == null)
                return;
            tbxAppLocation.Text = null;
            var rule = new AppDirectoryRule {IsRegistryPath = true};
// ReSharper disable PossibleNullReferenceException
            tbxAppLocation.GetBindingExpression(TextBox.TextProperty).ParentBinding.ValidationRules.Clear();
            tbxAppLocation.GetBindingExpression(TextBox.TextProperty).ParentBinding.ValidationRules.Add(rule);
            // ReSharper restore PossibleNullReferenceException
        }

        private void rbtnFileSystem_Checked(object sender, RoutedEventArgs e)
        {
            if (tbxAppLocation == null)
                return;

            tbxAppLocation.Text = null;
            var rule = new AppDirectoryRule {IsRegistryPath = false};
// ReSharper disable PossibleNullReferenceException
            tbxAppLocation.GetBindingExpression(TextBox.TextProperty).ParentBinding.ValidationRules.Clear();
            tbxAppLocation.GetBindingExpression(TextBox.TextProperty).ParentBinding.ValidationRules.Add(rule);
            // ReSharper restore PossibleNullReferenceException
            Core.AppInfo.ValueName = null;
            tbxValueName.Text = null;
        }

        #endregion

        #region Page

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            LoadInfo();
        }

        #endregion

        #region Aero

        private void AeroGlass_DwmCompositionChanged(object sender, AeroGlass.DwmCompositionChangedEventArgs e)
        {
            if (e.IsGlassEnabled)
            {
                tbTitle.Foreground = new SolidColorBrush(Color.FromRgb(0, 102, 204));
                line.Visibility = Visibility.Collapsed;
                rectangle.Visibility = Visibility.Collapsed;
            }
            else
            {
                tbTitle.Foreground = Brushes.Black;
                line.Visibility = Visibility.Visible;
                rectangle.Visibility = Visibility.Visible;
            }
        }

        #endregion

        #endregion
    }
}