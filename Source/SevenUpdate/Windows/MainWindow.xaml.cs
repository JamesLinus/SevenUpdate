// ***********************************************************************
// <copyright file="MainWindow.xaml.cs" project="SevenUpdate" assembly="SevenUpdate" solution="SevenUpdate" company="Seven Software">
//     Copyright (c) Seven Software. All rights reserved.
// </copyright>
// <author username="sevenalive">Robert Baker</author>
// <license href="http://www.gnu.org/licenses/gpl-3.0.txt" name="GNU General Public License 3">
//  This file is part of Seven Update.
//    Seven Update is free software: you can redistribute it and/or modify it under the terms of the GNU General Public
//    License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any
//    later version. Seven Update is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without
//    even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details. You should have received a copy of the GNU General Public License
//    along with Seven Update.  If not, see http://www.gnu.org/licenses/.
// </license>
// <summary>
//   Interaction logic for MainWindow.xaml
// .</summary> ***********************************************************************

namespace SevenUpdate.Windows
{
    using System;
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Markup;
    using System.Windows.Media;
    using System.Windows.Navigation;

    using SevenSoftware.Windows;
    using SevenSoftware.Windows.Internal;

    using SevenUpdate.Properties;

    /// <summary>Interaction logic for MainWindow.xaml.</summary>
    [ContentProperty]
    [TemplatePart(Name = @"PART_NavWinCP", Type = typeof(ContentPresenter))]
    public sealed partial class MainWindow
    {
        /// <summary>Initializes a new instance of the <see cref="MainWindow" /> class.</summary>
        public MainWindow()
        {
            this.InitializeComponent();
            if (App.IsDev)
            {
                this.Title += " - " + Properties.Resources.DevChannel;
            }

            if (App.IsBeta)
            {
                this.Title += " - " + Properties.Resources.BetaChannel;
            }

            AeroGlass.CompositionChanged -= this.ChangeWindowChrome;
            AeroGlass.CompositionChanged += this.ChangeWindowChrome;

            App.TaskBar = this.taskBar;
            NavService = this.NavigationService;

            Core.NavigateToMainPage();
            App.ProcessArgs(App.Args);
        }

        /// <summary>Gets the <c>NavigationService</c> for the current window.</summary>
        /// <value>The nav service.</value>
        internal static NavigationService NavService { get; private set; }

        /// <summary>Enables Aero Glass on the Window.</summary>
        /// <param name="e">The <c>EventArgs</c> instance containing the event data.</param>
        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            AeroGlass.EnableGlass(this, new Margins(0, 32, 0, 41));
            this.Background = AeroGlass.IsGlassEnabled ? Brushes.Transparent : Brushes.White;
        }

        /// <summary>Changes the Window Background when Aero Glass is enabled or disabled.</summary>
        /// <param name="sender">The object that called the event.</param>
        /// <param name="e">The <c>CompositionChangedEventArgs</c> instance containing the event data.</param>
        private void ChangeWindowChrome(object sender, CompositionChangedEventArgs e)
        {
            this.Background = e.IsGlassEnabled ? Brushes.Transparent : Brushes.White;

            if (!e.IsGlassEnabled)
            {
                return;
            }

            AeroGlass.EnableGlass(this, new Margins(0, 32, 0, 41));
        }

        /// <summary>Enables the ability to drag the window on glass.</summary>
        /// <param name="sender">The object that called the event.</param>
        /// <param name="e">The <c>System.Windows.Input.MouseButtonEventArgs</c> instance containing the event data.</param>
        private void EnableDragOnGlass(object sender, MouseButtonEventArgs e)
        {
            if (AeroGlass.IsGlassEnabled && e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }

        /// <summary>Sets the Height and Width of the window from the settings.</summary>
        /// <param name="sender">The object that called the event.</param>
        /// <param name="e">The <c>System.Windows.RoutedEventArgs</c> instance containing the event data.</param>
        private void LoadWindowSize(object sender, RoutedEventArgs e)
        {
            this.Height = Settings.Default.WindowHeight;
            this.Width = Settings.Default.WindowWidth;
        }

        /// <summary>When Seven Update is closing, save the Window Width and Height in the settings.</summary>
        /// <param name="sender">The object that called the event.</param>
        /// <param name="e">The <c>System.ComponentModel.CancelEventArgs</c> instance containing the event data.</param>
        private void SaveWindowSize(object sender, CancelEventArgs e)
        {
            Settings.Default.WindowHeight = this.Height;
            Settings.Default.WindowWidth = this.Width;
            Settings.Default.Save();
            WcfService.Disconnect();
            Application.Current.Shutdown(0);
        }
    }
}