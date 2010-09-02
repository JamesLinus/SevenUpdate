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
using Microsoft.Windows.Dwm;
using SevenUpdate.Sdk.Windows;

#endregion

namespace SevenUpdate.Sdk.Pages
{
    /// <summary>
    ///   Interaction logic for UpdateInfo.xaml
    /// </summary>
    public sealed partial class UpdateInfo : Page
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
            DataContext = Base.UpdateInfo;

            if (Environment.OSVersion.Version.Major < 6)
                return;

            MouseLeftButtonDown += App.Rectangle_MouseLeftButtonDown;
            AeroGlass.DwmCompositionChangedEventHandler += AeroGlass_DwmCompositionChangedEventHandler;
            line.Visibility = AeroGlass.IsEnabled ? Visibility.Collapsed : Visibility.Visible;
            rectangle.Visibility = AeroGlass.IsEnabled ? Visibility.Collapsed : Visibility.Visible;
            dpReleaseDate.SelectedDate = DateTime.Today;
        }

        #endregion

        #region UI Events

        #region Button - Click

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            SaveInfo();
            MainWindow.NavService.Navigate(new Uri(@"Pages\UpdateFiles.xaml", UriKind.Relative));
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.NavService.Navigate(new Uri(@"Pages\Main.xaml", UriKind.Relative));
        }

        #endregion

        #region ComboBox Selection Changed

        private void Locale_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (tbxUpdateName == null || cbxLocale.SelectedIndex < 0)
                return;

            Base.SelectedLocale = ((ComboBoxItem) cbxLocale.SelectedItem).Tag.ToString();

            bool found = false;
            // Load Values
            foreach (LocaleString t in Base.UpdateInfo.Name.Where(t => t.Lang == Base.SelectedLocale))
            {
                tbxUpdateName.Text = t.Value;
                found = true;
            }
            if (!found)
                tbxUpdateName.Text = null;


            found = false;
            // Load Values
            foreach (LocaleString t in Base.UpdateInfo.Description.Where(t => t.Lang == Base.SelectedLocale))
            {
                tbxUpdateDetails.Text = t.Value;
                found = true;
            }
            if (!found)
                tbxUpdateDetails.Text = null;
        }

        private void dpReleaseDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dpReleaseDate == null || imgReleaseDate == null)
                return;
            imgReleaseDate.Visibility = dpReleaseDate.SelectedDate.HasValue ? Visibility.Hidden : Visibility.Visible;
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

        #region Methods

        private void LoadInfo()
        {
            tbxUpdateName.GetBindingExpression(TextBox.TextProperty).UpdateSource();
            tbxUpdateDetails.GetBindingExpression(TextBox.TextProperty).UpdateSource();

            if (Base.UpdateInfo.ReleaseDate != null)
                dpReleaseDate.SelectedDate = DateTime.Parse(Base.UpdateInfo.ReleaseDate);

            // Load Values
            foreach (LocaleString t in Base.UpdateInfo.Description.Where(t => t.Lang == "en"))
                tbxUpdateDetails.Text = t.Value;

            foreach (LocaleString t in Base.UpdateInfo.Name.Where(t => t.Lang == "en"))
                tbxUpdateName.Text = t.Value;
        }

        private void SaveInfo()
        {
            if (dpReleaseDate.SelectedDate != null)
                Base.UpdateInfo.ReleaseDate = dpReleaseDate.SelectedDate.Value.ToShortDateString();
        }

        #endregion

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            LoadInfo();
        }
    }
}