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
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Windows.Controls;
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
        #region Fields

        private string locale;

        #endregion

        #region Properties

        private bool IsInfoValid
        {
            get
            {
                return (imgUpdateInfo.Visibility != Visibility.Visible && imgUpdateDetails.Visibility != Visibility.Visible && imgReleaseDate.Visibility != Visibility.Visible &&
                        imgLicense.Visibility != Visibility.Visible && imgDownloadLoc.Visibility != Visibility.Visible);
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        ///   The constructor for the UpdateInfo page
        /// </summary>
        public UpdateInfo()
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

        #region Button - Click

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (IsInfoValid)
            {
                SaveInfo();
                MainWindow.NavService.Navigate(new Uri(@"Pages\UpdateFiles.xaml", UriKind.Relative));
            }
            else
                App.ShowInputErrorMessage();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.NavService.Navigate(new Uri(@"Pages\Main.xaml", UriKind.Relative));
        }

        #endregion

        #region TextBox - Text Changed

        private void Textbox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var source = e.Source as InfoTextBox;
            if (source == null)
                return;

            if (source.Name == "tbxUpdateName" || source.Name == "tbxUpdateDetails")
            {
                imgUpdateName.Visibility = tbxUpdateName.Text.Length > 5 ? Visibility.Collapsed : Visibility.Visible;
                imgUpdateDetails.Visibility = tbxUpdateDetails.Text.Length > 5 ? Visibility.Collapsed : Visibility.Visible;
            }
            else
            {
                if (Base.CheckUrl(SevenUpdate.Base.ConvertPath(source.Text, true, Base.Sua.Is64Bit)))
                {
                    switch (source.Name)
                    {
                        case "tbxLicenseUrl":
                            imgLicense.Visibility = Visibility.Collapsed;
                            break;

                        case "tbxDownloadUrl":
                            if (source.Text.Length > 2)
                                imgDownloadLoc.Visibility = Visibility.Collapsed;
                            break;

                        case "tbxInfoUrl":
                            imgUpdateInfo.Visibility = Visibility.Collapsed;
                            break;
                    }
                }
                else
                {
                    switch (source.Name)
                    {
                        case "tbxLicenseUrl":
                            imgLicense.Visibility = Visibility.Visible;
                            break;

                        case "tbxDownloadUrl":
                            imgDownloadLoc.Visibility = Visibility.Visible;
                            break;

                        case "tbxInfoUrl":
                            imgUpdateInfo.Visibility = Visibility.Visible;
                            break;
                    }
                }
            }
        }

        #endregion

        #region ComboBox Selection Changed

        private void Language_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (tbxUpdateName == null)
                return;

            locale = ((ComboBoxItem) cbxLanguage.SelectedItem).Tag.ToString();
            tbxUpdateName.Text = null;
            tbxUpdateDetails.Text = null;

            if (Base.Update.Description == null)
                Base.Update.Description = new ObservableCollection<LocaleString>();
            else
            {
                // Load Values
                foreach (LocaleString t in Base.Update.Description.Where(t => t.Lang == locale))
                    tbxUpdateDetails.Text = t.Value;
            }

            if (Base.Update.Name == null)
                Base.Update.Name = new ObservableCollection<LocaleString>();
            else
            {
                // Load Values
                foreach (LocaleString t in Base.Update.Name.Where(t => t.Lang == locale))
                    tbxUpdateName.Text = t.Value;
            }
        }

        private void dpReleaseDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            imgReleaseDate.Visibility = dpReleaseDate.SelectedDate.HasValue ? Visibility.Hidden : Visibility.Visible;
        }

        #endregion

        #region TextBox - Lost Keyboard Focus

        private void UpdateTitle_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (Base.Update.Name == null)
                Base.Update.Name = new ObservableCollection<LocaleString>();

            bool found = false;
            foreach (LocaleString t in Base.Update.Name.Where(t => t.Lang == locale))
            {
                t.Value = tbxUpdateName.Text;
                found = true;
            }

            if (found)
                return;

            var ls = new LocaleString {Lang = locale, Value = tbxUpdateName.Text};
            Base.Update.Name.Add(ls);
        }

        private void UpdateDetails_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (Base.Update.Description == null)
                Base.Update.Description = new ObservableCollection<LocaleString>();

            bool found = false;
            foreach (LocaleString t in Base.Update.Description.Where(t => t.Lang == locale))
            {
                t.Value = tbxUpdateDetails.Text;
                found = true;
            }

            if (found)
                return;

            var ls = new LocaleString {Lang = locale, Value = tbxUpdateDetails.Text};
            Base.Update.Description.Add(ls);
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
            tbxLicenseUrl.Text = Base.Update.LicenseUrl;
            tbxInfoUrl.Text = Base.Update.InfoUrl;
            if (Base.Update.ReleaseDate != null)
                dpReleaseDate.SelectedDate = DateTime.Parse(Base.Update.ReleaseDate);
            cbxUpdateImportance.SelectedIndex = (int) Base.Update.Importance;

            // Load Values
            foreach (LocaleString t in Base.Update.Description.Where(t => t.Lang == "en"))
                tbxUpdateDetails.Text = t.Value;

            foreach (LocaleString t in Base.Update.Name.Where(t => t.Lang == "en"))
                tbxUpdateName.Text = t.Value;
        }

        private void SaveInfo()
        {
            Base.Update.LicenseUrl = tbxLicenseUrl.Text;
            Base.Update.InfoUrl = tbxInfoUrl.Text;
            Base.Update.Importance = (Importance) cbxUpdateImportance.SelectedIndex;
            Base.Update.ReleaseDate = dpReleaseDate.SelectedDate.Value.ToShortDateString();
        }

        #endregion

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            LoadInfo();
        }
    }
}