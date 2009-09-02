#region GNU Public License v3

// Copyright 2007, 2008 Robert Baker, aka Seven ALive.
// This file is part of Seven Update.
// 
//     Seven Update is free software: you can redistribute it and/or modify
//     it under the terms of the GNU General Public License as published by
//     the Free Software Foundation, either version 3 of the License, or
//     (at your option) any later version.
// 
//     Seven Update is distributed in the hope that it will be useful,
//     but WITHOUT ANY WARRANTY; without even the implied warranty of
//     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//     GNU General Public License for more details.
// 
//    You should have received a copy of the GNU General Public License
//     along with Seven Update.  If not, see <http://www.gnu.org/licenses/>.

#endregion

#region

using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Navigation;
using SevenUpdate.Properties;

#endregion

namespace SevenUpdate.Windows
{
    /// <summary>
    /// Interaction logic for TestWindow.xaml
    /// </summary>
    [ContentProperty]
    [TemplatePart(Name = "PART_NavWinCP", Type = typeof (ContentPresenter))]
    public partial class MainWindow : NavigationWindow
    {
        internal static NavigationService NavService;

        public MainWindow()
        {
            InitializeComponent();

            NavService = NavigationService;
        }

        private void NavigationWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Height = Settings.Default.windowHeight;
            Width = Settings.Default.windowWidth;
        }

        private void NavigationWindow_Closing(object sender, CancelEventArgs e)
        {
            Settings.Default.windowHeight = Height;
            Settings.Default.windowWidth = Width;
            Settings.Default.Save();
            Environment.Exit(0);
        }
    }
}