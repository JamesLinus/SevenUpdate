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
        #region Properties

        #endregion

        #region Constructors

        /// <summary>
        ///   The constructor for the AppInfo page
        /// </summary>
        public AppInfo()
        {
            InitializeComponent();
            DataContext = Base.AppInfo;

            if (Environment.OSVersion.Version.Major < 6)
                return;


            MouseLeftButtonDown += App.Rectangle_MouseLeftButtonDown;
            AeroGlass.DwmCompositionChangedEventHandler += AeroGlass_DwmCompositionChangedEventHandler;
            line.Visibility = AeroGlass.IsEnabled ? Visibility.Collapsed : Visibility.Visible;
            rectangle.Visibility = AeroGlass.IsEnabled ? Visibility.Collapsed : Visibility.Visible;
        }

        private void LoadInfo()
        {
            tbxPublisher.GetBindingExpression(TextBox.TextProperty).UpdateSource();
            tbxAppDescription.GetBindingExpression(TextBox.TextProperty).UpdateSource();
            tbxAppName.GetBindingExpression(TextBox.TextProperty).UpdateSource();
            tbxAppLocation.GetBindingExpression(TextBox.TextProperty).UpdateSource();
            tbxValueName.GetBindingExpression(TextBox.TextProperty).UpdateSource();

            // Load Values
            foreach (LocaleString t in Base.AppInfo.Name.Where(t => t.Lang == "en"))
                tbxAppName.Text = t.Value;

            foreach (LocaleString t in Base.AppInfo.Description.Where(t => t.Lang == "en"))
                tbxAppDescription.Text = t.Value;

            foreach (LocaleString t in Base.AppInfo.Publisher.Where(t => t.Lang == "en"))
                tbxPublisher.Text = t.Value;
        }

        #endregion

        #region Methods

        #endregion

        #region UI Events

        #region TextBox - Lost Keyboard Focus

        //private void tbxAppLocation_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        //{
        //    if (!rbtnFileSystem.IsChecked.GetValueOrDefault())
        //        return;

        //    tbxAppLocation.Text = SevenUpdate.Base.ConvertPath(tbxAppLocation.Text, false, Base.AppInfo.Is64Bit);
        //}

        //private void AppName_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        //{
        //    if (Base.AppInfo.Name == null)
        //        Base.AppInfo.Name = new ObservableCollection<LocaleString>();

        //    bool found = false;
        //    foreach (LocaleString t in Base.AppInfo.Name.Where(t => t.Lang == Base.SelectedLocale))
        //    {
        //        t.Value = tbxAppName.Text;
        //        found = true;
        //    }
        //    if (found)
        //        return;

        //    var ls = new LocaleString { Lang = Base.SelectedLocale, Value = tbxAppName.Text };
        //    Base.AppInfo.Name.Add(ls);
        //}

        //private void AppDescription_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        //{
        //    if (Base.AppInfo.Description == null)
        //        Base.AppInfo.Description = new ObservableCollection<LocaleString>();

        //    bool found = false;
        //    foreach (LocaleString t in Base.AppInfo.Description.Where(t => t.Lang == Base.SelectedLocale))
        //    {
        //        t.Value = tbxAppDescription.Text;
        //        found = true;
        //    }

        //    if (found)
        //        return;

        //    var ls = new LocaleString { Lang = Base.SelectedLocale, Value = tbxAppDescription.Text };
        //    Base.AppInfo.Description.Add(ls);
        //}

        //private void Publisher_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        //{
        //    if (Base.AppInfo.Publisher == null)
        //        Base.AppInfo.Publisher = new ObservableCollection<LocaleString>();

        //    bool found = false;
        //    foreach (LocaleString t in Base.AppInfo.Publisher.Where(t => t.Lang == Base.SelectedLocale))
        //    {
        //        t.Value = tbxPublisher.Text;
        //        found = true;
        //    }

        //    if (found)
        //        return;
        //    var ls = new LocaleString { Lang = Base.SelectedLocale, Value = tbxPublisher.Text };
        //    Base.AppInfo.Publisher.Add(ls);
        //}

        #endregion

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
                tbxAppLocation.Text = SevenUpdate.Base.ConvertPath(cfd.FileName, false, Base.AppInfo.Is64Bit);
        }

        #endregion

        #region ComboBox - Selection Changed

        private void Language_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (tbxAppName == null || cbxLanguage.SelectedIndex < 0)
                return;

            Base.SelectedLocale = ((ComboBoxItem) cbxLanguage.SelectedItem).Tag.ToString();

            if (Base.AppInfo.Description == null)
                Base.AppInfo.Description = new ObservableCollection<LocaleString>();
            else
            {
                bool found = false;
                // Load Values
                foreach (LocaleString t in Base.AppInfo.Description.Where(t => t.Lang == Base.SelectedLocale))
                {
                    tbxAppDescription.Text = t.Value;
                    found = true;
                }
                if (!found)
                    tbxAppDescription.Text = null;
            }

            if (Base.AppInfo.Name == null)
                Base.AppInfo.Name = new ObservableCollection<LocaleString>();
            else
            {
                bool found = false;
                // Load Values
                foreach (LocaleString t in Base.AppInfo.Name.Where(t => t.Lang == Base.SelectedLocale))
                {
                    tbxAppName.Text = t.Value;
                    found = true;
                }
                if (!found)
                    tbxAppName.Text = null;
            }

            if (Base.AppInfo.Publisher == null)
                Base.AppInfo.Publisher = new ObservableCollection<LocaleString>();
            else
            {
                bool found = false;
                // Load Values
                foreach (LocaleString t in Base.AppInfo.Publisher.Where(t => t.Lang == Base.SelectedLocale))
                {
                    tbxPublisher.Text = t.Value;
                    found = true;
                }
                if (!found)
                    tbxPublisher.Text = null;
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

        private void tbxAppLocation_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            tbxAppLocation.Text = SevenUpdate.Base.ConvertPath(tbxAppLocation.Text, false, Base.AppInfo.Is64Bit);
        }

        #endregion

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            LoadInfo();
        }
    }
}