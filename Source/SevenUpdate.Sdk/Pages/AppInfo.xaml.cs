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

using SevenUpdate.Sdk.Windows;

#endregion

namespace SevenUpdate.Sdk.Pages
{
    /// <summary>
    ///   Interaction logic for AppInfo.xaml
    /// </summary>
    public sealed partial class AppInfo : Page
    {
        #region Fields

        private string locale;

        #endregion

        #region Properties

        private bool IsInfoValid
        {
            get
            {
                return (imgPublisherUrl.Visibility != Visibility.Visible && imgValueName.Visibility != Visibility.Visible && imgAppDescription.Visibility != Visibility.Visible &&
                        imgAppName.Visibility != Visibility.Visible && imgHelpUrl.Visibility != Visibility.Visible && imgAppLocation.Visibility != Visibility.Visible);
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        ///   The constructor for the AppInfo page
        /// </summary>
        public AppInfo()
        {
            InitializeComponent();
            LoadInfo();
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
            cxbIs64Bit.IsChecked = Base.Sua.Is64Bit;
            tbxAppUrl.Text = Base.Sua.AppUrl;
            tbxHelpUrl.Text = Base.Sua.HelpUrl;
            char[] split = {'|'};
            if (Base.Sua.Directory != null)
            {
                tbxAppLocation.Text = Base.Sua.Directory.Split(split)[0];
                tbxValueName.Text = Base.Sua.Directory.Split(split)[1];
            }
        }

        private void SaveInfo()
        {
            Base.Sua.Is64Bit = cxbIs64Bit.IsChecked.GetValueOrDefault();
            Base.Sua.AppUrl = tbxAppUrl.Text;
            Base.Sua.HelpUrl = tbxHelpUrl.Text;
            if (rbtnFileSystem.IsChecked.GetValueOrDefault())
                Base.Sua.Directory = tbxAppLocation.Text;
            else
                Base.Sua.Directory = tbxAppLocation.Text + "|" + tbxValueName.Text;
        }

        #endregion

        #region UI Events

        #region TextBox - Text Changed

        private void AppLocation_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (rbtnRegistry.IsChecked.GetValueOrDefault())
            {
                if (tbxAppLocation.Text.StartsWith(@"HKLM\", true, null) || tbxAppLocation.Text.StartsWith(@"HKCR\", true, null) || tbxAppLocation.Text.StartsWith(@"HKCU\", true, null) ||
                    tbxAppLocation.Text.StartsWith(@"HKU\", true, null) || tbxAppLocation.Text.StartsWith(@"HKEY_CLASSES_ROOT\") || tbxAppLocation.Text.StartsWith(@"HKEY_CURRENT_USER\", true, null) ||
                    tbxAppLocation.Text.StartsWith(@"HKEY_LOCAL_MACHINE\", true, null) || tbxAppLocation.Text.StartsWith(@"HKEY_USERS\", true, null))
                    imgAppLocation.Visibility = Visibility.Collapsed;
                else
                    imgAppLocation.Visibility = Visibility.Visible;
            }
            else
                imgAppLocation.Visibility = !App.IsValidFilePath(tbxAppLocation.Text, cxbIs64Bit.IsChecked.GetValueOrDefault()) ? Visibility.Visible : Visibility.Collapsed;
        }

        private void UrlTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var source = e.Source as InfoTextBox;
            if (source == null)
                return;

            if (Base.CheckUrl(source.Text))
            {
                switch (source.Name)
                {
                    case "tbxPublisherUrl":
                        imgPublisherUrl.Visibility = Visibility.Collapsed;
                        break;

                    case "tbxHelpUrl":
                        imgHelpUrl.Visibility = Visibility.Collapsed;
                        break;
                }
            }
            else
            {
                switch (source.Name)
                {
                    case "tbxPublisherUrl":
                        imgPublisherUrl.Visibility = Visibility.Visible;
                        break;

                    case "tbxHelpUrl":
                        imgHelpUrl.Visibility = Visibility.Visible;
                        break;
                }
            }
        }

        private void StringTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (rbtnRegistry.IsChecked.GetValueOrDefault())
                imgValueName.Visibility = tbxValueName.Text.Length > 2 ? Visibility.Collapsed : Visibility.Visible;
            else
                imgValueName.Visibility = Visibility.Collapsed;

            imgPublisher.Visibility = tbxPublisher.Text.Length > 2 ? Visibility.Collapsed : Visibility.Visible;
            imgAppName.Visibility = tbxAppName.Text.Length > 2 ? Visibility.Collapsed : Visibility.Visible;
            imgAppDescription.Visibility = tbxAppDescription.Text.Length > 5 ? Visibility.Collapsed : Visibility.Visible;
        }

        #endregion

        #region TextBox - Lost Keyboard Focus

        private void tbxAppLocation_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (!rbtnFileSystem.IsChecked.GetValueOrDefault())
                return;

            tbxAppLocation.Text = SevenUpdate.Base.ConvertPath(tbxAppLocation.Text, false, Base.Sua.Is64Bit);
            if (Path.GetFileName(tbxAppLocation.Text) == "")
                imgAppLocation.Visibility = Visibility.Visible;
        }

        private void AppName_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (Base.Sua.Name == null)
                Base.Sua.Name = new ObservableCollection<LocaleString>();

            bool found = false;
            foreach (LocaleString t in Base.Sua.Name.Where(t => t.Lang == locale))
            {
                t.Value = tbxAppName.Text;
                found = true;
            }

            if (found)
                return;

            var ls = new LocaleString {Lang = locale, Value = tbxAppName.Text};
            Base.Sua.Name.Add(ls);
        }

        private void AppDescription_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (Base.Sua.Description == null)
                Base.Sua.Description = new ObservableCollection<LocaleString>();

            bool found = false;
            foreach (LocaleString t in Base.Sua.Description.Where(t => t.Lang == locale))
            {
                t.Value = tbxAppDescription.Text;
                found = true;
            }

            if (found)
                return;

            var ls = new LocaleString {Lang = locale, Value = tbxAppDescription.Text};
            Base.Sua.Description.Add(ls);
        }

        private void Publisher_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (Base.Sua.Publisher == null)
                Base.Sua.Publisher = new ObservableCollection<LocaleString>();

            bool found = false;
            foreach (LocaleString t in Base.Sua.Publisher.Where(t => t.Lang == locale))
            {
                t.Value = tbxPublisher.Text;
                found = true;
            }

            if (found)
                return;
            var ls = new LocaleString {Lang = locale, Value = tbxPublisher.Text};
            Base.Sua.Publisher.Add(ls);
        }

        #endregion

        #region RadioButton - Checked

        private void FileSystem_Checked(object sender, RoutedEventArgs e)
        {
            if (lblValue == null)
                return;
            lblRegistry.Visibility = Visibility.Collapsed;
            lblValue.Visibility = Visibility.Collapsed;
            tbxValueName.Visibility = Visibility.Collapsed;
            tbBrowse.Visibility = Visibility.Visible;
            imgValueName.Visibility = Visibility.Collapsed;
            imgAppLocation.Visibility = !App.IsValidFilePath(tbxAppLocation.Text, cxbIs64Bit.IsChecked.GetValueOrDefault()) ? Visibility.Visible : Visibility.Collapsed;
        }

        private void Registry_Checked(object sender, RoutedEventArgs e)
        {
            if (lblValue == null)
                return;
            lblRegistry.Visibility = Visibility.Visible;
            lblValue.Visibility = Visibility.Visible;
            tbxValueName.Visibility = Visibility.Visible;
            tbBrowse.Visibility = Visibility.Collapsed;

            if (tbxAppLocation.Text.StartsWith(@"HKLM\", true, null) || tbxAppLocation.Text.StartsWith(@"HKCR\", true, null) || tbxAppLocation.Text.StartsWith(@"HKCU\", true, null) ||
                tbxAppLocation.Text.StartsWith(@"HKU\", true, null) || tbxAppLocation.Text.StartsWith(@"HKEY_CLASSES_ROOT\") || tbxAppLocation.Text.StartsWith(@"HKEY_CURRENT_USER\", true, null) ||
                tbxAppLocation.Text.StartsWith(@"HKEY_LOCAL_MACHINE\", true, null) || tbxAppLocation.Text.StartsWith(@"HKEY_USERS\", true, null))
                imgAppLocation.Visibility = Visibility.Collapsed;
            else
                imgAppLocation.Visibility = Visibility.Visible;

            imgValueName.Visibility = tbxValueName.Text.Length > 2 ? Visibility.Collapsed : Visibility.Visible;
        }

        #endregion

        #region Button - Click

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (IsInfoValid)
            {
                SaveInfo();
                MainWindow.NavService.Navigate(new Uri(@"Pages\UpdateInfo.xaml", UriKind.Relative));
            }
            else
                App.ShowInputErrorMessage();
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
            if (cfd.ShowDialog() == CommonFileDialogResult.OK)
                tbxAppLocation.Text = SevenUpdate.Base.ConvertPath(cfd.FileName, false, Convert.ToBoolean(cxbIs64Bit.IsChecked));
        }

        #endregion

        #region ComboBox - Selection Changed

        private void Language_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (tbxAppName == null)
                return;

            locale = ((ComboBoxItem) cbxLanguage.SelectedItem).Tag.ToString();
            tbxAppDescription.Text = null;
            tbxPublisher.Text = null;
            tbxAppName.Text = null;

            if (Base.Sua.Description == null)
                Base.Sua.Description = new ObservableCollection<LocaleString>();
            else
            {
                // Load Values
                foreach (LocaleString t in Base.Sua.Description.Where(t => t.Lang == locale))
                    tbxAppDescription.Text = t.Value;
            }

            if (Base.Sua.Name == null)
                Base.Sua.Name = new ObservableCollection<LocaleString>();
            else
            {
                // Load Values
                foreach (LocaleString t in Base.Sua.Name.Where(t => t.Lang == locale))
                    tbxAppName.Text = t.Value;
            }

            if (Base.Sua.Publisher == null)
                Base.Sua.Publisher = new ObservableCollection<LocaleString>();
            else
            {
                // Load Values
                foreach (LocaleString t in Base.Sua.Publisher.Where(t => t.Lang == locale))
                    tbxPublisher.Text = t.Value;
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
    }
}