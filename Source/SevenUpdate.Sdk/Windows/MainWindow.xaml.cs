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
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Navigation;
using Microsoft.Windows.Dwm;
using SevenUpdate.Sdk.Properties;

#endregion

namespace SevenUpdate.Sdk.Windows
{
    /// <summary>
    ///   Interaction logic for MainWindow.xaml
    /// </summary>
    [ContentProperty, TemplatePart(Name = "PART_NavWinCP", Type = typeof (ContentPresenter))]
    public sealed partial class MainWindow : NavigationWindow
    {
        internal static NavigationService NavService;

        public MainWindow()
        {
            InitializeComponent();

            NavService = NavigationService;
        }

        /// <summary>
        ///   Sets the Height and Width of the window from the settings
        /// </summary>
        /// <param name = "sender"></param>
        /// <param name = "e"></param>
        private void NavigationWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Height = Settings.Default.windowHeight;
            Width = Settings.Default.windowWidth;
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            AeroGlass.EnableGlass(this);
            AeroGlass.ResetAeroGlass(new Margins(0, 32, 0, 44), this);
        }

        /// <summary>
        ///   When Seven Update is closing, save the Window Width and Height in the settings
        /// </summary>
        /// <param name = "sender"></param>
        /// <param name = "e"></param>
        private void NavigationWindow_Closing(object sender, CancelEventArgs e)
        {
            Settings.Default.windowHeight = Height;
            Settings.Default.windowWidth = Width;
            Settings.Default.Save();
            Environment.Exit(0);
        }

        private void NavigationWindow_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (AeroGlass.IsEnabled)
                DragMove();
        }
    }
}