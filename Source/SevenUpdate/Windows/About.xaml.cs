#region GNU Public License v3

// Copyright 2007-2010 Robert Baker, Seven Software.
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
//    along with Seven Update.  If not, see <http://www.gnu.org/licenses/>.

#endregion

#region

using System;
using System.Diagnostics;
using System.Reflection;
using System.Windows;
using System.Windows.Input;

#endregion

namespace SevenUpdate.Windows
{
    /// <summary>
    /// Interaction logic for About.xaml
    /// </summary>
    public sealed partial class About : Window
    {
        /// <summary>
        /// Displays About Information
        /// </summary>
        public About()
        {
            InitializeComponent();

            var assembly = Assembly.GetExecutingAssembly();
            var version = assembly.GetName().Version;

            Title = Title + " " + assembly.GetName().Name;

            tbVersion.Text = version.ToString();

            tbCopyright.Text = "© " + "2007 - " + DateTime.Now.Year + " " + App.RM.GetString("SevenSoftware");
            tbLicense.Text = assembly.GetName().Name + " " + tbLicense.Text;
        }

        /// <summary>
        /// Closes the About window
        /// </summary>
        private void CloseClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void SupportMouseDown(object sender, MouseButtonEventArgs e)
        {
            Process.Start(tbSupport.Text);
        }

        private void LicenseMouseDown(object sender, MouseButtonEventArgs e)
        {
            Process.Start("http://www.gnu.org/licenses/gpl-3.0.txt");
        }
    }
}