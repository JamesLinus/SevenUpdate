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
using Microsoft.Windows.Dialogs;
using Microsoft.Windows.Dwm;
using SevenUpdate.Base;
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

        private bool IsInfoValid { get { return (imgPublisherUrl.Visibility != Visibility.Visible && imgHelpUrl.Visibility != Visibility.Visible && imgAppPath.Visibility != Visibility.Visible); } }

        #endregion

        /// <summary>
        ///   The constructor for the AppInfo page
        /// </summary>
        public AppInfo()
        {
            InitializeComponent();

            if (Environment.OSVersion.Version.Major < 6)
                return;

            MouseLeftButtonDown += App.Rectangle_MouseLeftButtonDown;
            AeroGlass.DwmCompositionChangedEventHandler += AeroGlass_DwmCompositionChangedEventHandler;
            line.Visibility = AeroGlass.IsEnabled ? Visibility.Collapsed : Visibility.Visible;
            rectangle.Visibility = AeroGlass.IsEnabled ? Visibility.Collapsed : Visibility.Visible;
        }

        private void AeroGlass_DwmCompositionChangedEventHandler(object sender, AeroGlass.DwmCompositionChangedEventArgs e)
        {
            line.Visibility = e.IsGlassEnabled ? Visibility.Collapsed : Visibility.Visible;
            rectangle.Visibility = e.IsGlassEnabled ? Visibility.Collapsed : Visibility.Visible;
        }

        private void Browse_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var cfd = new CommonOpenFileDialog {IsFolderPicker = true, Multiselect = false};
            if (cfd.ShowDialog() == CommonFileDialogResult.OK)
                tbxAppLocation.Text = SevenUpdate.Base.Base.ConvertPath(cfd.FileName, false, Convert.ToBoolean(cxbIs64Bit.IsChecked));
        }

        private void FileSystem_Checked(object sender, RoutedEventArgs e)
        {
            if (lblValue == null)
                return;
            lblRegistry.Visibility = Visibility.Collapsed;
            lblValue.Visibility = Visibility.Collapsed;
            tbxValueName.Visibility = Visibility.Collapsed;
            tbBrowse.Visibility = Visibility.Visible;
        }

        private void Registry_Checked(object sender, RoutedEventArgs e)
        {
            if (lblValue == null)
                return;
            lblRegistry.Visibility = Visibility.Visible;
            lblValue.Visibility = Visibility.Visible;
            tbxValueName.Visibility = Visibility.Visible;
            tbBrowse.Visibility = Visibility.Collapsed;
        }

        private void AppLocation_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (((bool) rbtnRegistry.IsChecked))
            {
                if (tbxAppLocation.Text.StartsWith(@"HKLM\", true, null) || tbxAppLocation.Text.StartsWith(@"HKCR\", true, null) || tbxAppLocation.Text.StartsWith(@"HKCU\", true, null) ||
                    tbxAppLocation.Text.StartsWith(@"HKU\", true, null) || tbxAppLocation.Text.StartsWith(@"HKEY_CLASSES_ROOT\") || tbxAppLocation.Text.StartsWith(@"HKEY_CURRENT_USER\", true, null) ||
                    tbxAppLocation.Text.StartsWith(@"HKEY_LOCAL_MACHINE\", true, null) || tbxAppLocation.Text.StartsWith(@"HKEY_USERS\", true, null))
                    imgAppPath.Visibility = Visibility.Collapsed;
                else
                    imgAppPath.Visibility = Visibility.Visible;
            }
            else
                imgAppPath.Visibility = !App.IsValidFilePath(tbxAppLocation.Text, (bool) cxbIs64Bit.IsChecked) ? Visibility.Visible : Visibility.Collapsed;
        }

        private void Textbox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var source = e.Source as InfoTextBox;
            if (source == null)
                return;

            if (source.Name == "tbxAppName")
            {
                imgAppName.Visibility = tbxAppName.Text.Length > 2 ? Visibility.Collapsed : Visibility.Visible;
            }
            else
            {
                try
                {
                    if (source.Text.Length > 0)
                        new Uri(source.Text);
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
                catch
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
        }

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

        private void LoadInfo()
        {

        }

        private void SaveInfo()
        {
            Base.Sua.Is64Bit = cxbIs64Bit.IsChecked.GetValueOrDefault();
            Base.Sua.AppUrl = tbxAppUrl.Text;
            Base.Sua.HelpUrl = tbxHelpUrl.Text;
            Base.Sua.Directory = tbxAppLocation.Text;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.NavService.Navigate(new Uri(@"Pages\Main.xaml", UriKind.Relative));
        }

        private void Language_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            locale = ((ComboBoxItem) cbxLanguage.SelectedItem).Tag.ToString();
            tbxAppDescription.Text = null;
            tbxPublisher.Text = null;
            tbxAppName.Text = null;

            if (Base.Sua.Description == null)
            {
                Base.Sua.Description = new ObservableCollection<LocaleString>();
            }
            else
            {
                // Load Values
                foreach (LocaleString t in Base.Sua.Description.Where(t => t.Lang == locale))
                {
                    tbxAppDescription.Text = t.Value;
                }
            }

            if (Base.Sua.Name == null)
            {
                Base.Sua.Name = new ObservableCollection<LocaleString>();
            }
            else
            {
                // Load Values
                foreach (LocaleString t in Base.Sua.Name.Where(t => t.Lang == locale))
                {
                    tbxAppName.Text = t.Value;
                }
            }

            if (Base.Sua.Publisher == null)
            {
                Base.Sua.Publisher= new ObservableCollection<LocaleString>();
            }
            else
            {
                // Load Values
                foreach (LocaleString t in Base.Sua.Publisher.Where(t => t.Lang == locale))
                {
                    tbxPublisher.Text = t.Value;
                }
            }
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

            var ls = new LocaleString { Lang = locale, Value = tbxAppName.Text };
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

            var ls = new LocaleString { Lang = locale, Value = tbxAppDescription.Text };
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
    }
}