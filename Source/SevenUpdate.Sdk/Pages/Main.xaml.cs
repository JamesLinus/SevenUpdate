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
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using SevenUpdate.Sdk.Windows;

#endregion

namespace SevenUpdate.Sdk.Pages
{
    /// <summary>
    ///   Interaction logic for Main.xaml
    /// </summary>
    public sealed partial class Main : Page
    {
        #region Constructors

        /// <summary>
        ///   The constructor for the Main page
        /// </summary>
        public Main()
        {
            InitializeComponent();
        }

        #endregion

        #region UI Events

        #region TextBox - Text Changed Events

        #endregion

        #region TextBox - Lost Keyboard Focus

        #endregion

        #region RadioButton - Checked

        #endregion

        #region Button - Click

        #endregion

        #region Commandlink - Click

        private void NewProject_Click(object sender, RoutedEventArgs e)
        {
            Base.AppInfo = new Sua();
            Base.UpdateInfo = new Update();
            MainWindow.NavService.Navigate(new Uri(@"Pages\AppInfo.xaml", UriKind.Relative));
        }

        #endregion

        #region TextBlock - Mouse Down

        private void Help_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Process.Start(@"http://sevenupdate.com/support");
        }

        #endregion

        #region MenuItem - Click

        #endregion

        #region ComboBox - Selection Changed

        #endregion

        #region Aero

        #endregion

        #endregion
    }
}