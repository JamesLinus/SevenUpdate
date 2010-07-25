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
    ///   Interaction logic for UpdateShortcuts.xaml
    /// </summary>
    public sealed partial class UpdateShortcuts : Page
    {
        /// <summary>
        ///   The constructor for the UpdateShortcuts page
        /// </summary>
        public UpdateShortcuts()
        {
            InitializeComponent();
            if (!AeroGlass.IsEnabled)
                return;
            line.Visibility = Visibility.Collapsed;
            rectangle.Visibility = Visibility.Collapsed;
            MouseLeftButtonDown += App.Rectangle_MouseLeftButtonDown;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.NavService.Navigate(new Uri(@"Pages\UpdateList.xaml", UriKind.Relative));
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.NavService.Navigate(new Uri(@"Pages\Main.xaml", UriKind.Relative));
        }

        private void AddShortcut_Click(object sender, RoutedEventArgs e)
        {
        }

        private void ImportShortcut_Click(object sender, RoutedEventArgs e)
        {
        }

        private void BrowseTarget_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var cfd = new CommonOpenFileDialog {IsFolderPicker = true, Multiselect = false};
            if (cfd.ShowDialog() == CommonFileDialogResult.OK)
                tbxShortcutTarget.Text = Base.Base.ConvertPath(cfd.FileName, false, true);
        }

        private void BrowsePath_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var cfd = new CommonOpenFileDialog { IsFolderPicker = true, Multiselect = false };
            if (cfd.ShowDialog() == CommonFileDialogResult.OK)
               tbxShortcutPath.Text = Base.Base.ConvertPath(cfd.FileName, false, true);
        }
        private void BrowseIcon_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var cfd = new CommonOpenFileDialog { IsFolderPicker = true, Multiselect = false };
            if (cfd.ShowDialog() == CommonFileDialogResult.OK)
                tbxShortcutIcon.Text = Base.Base.ConvertPath(cfd.FileName, false, true);
        }
    }
}