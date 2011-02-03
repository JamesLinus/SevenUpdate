// ***********************************************************************
// <copyright file="MainWindow.xaml.cs"
//            project="SevenUpdate.Sdk"
//            assembly="SevenUpdate.Sdk"
//            solution="SevenUpdate"
//            company="Seven Software">
//     Copyright (c) Seven Software. All rights reserved.
// </copyright>
// <author username="sevenalive">Robert Baker</author>
// <license href="http://www.gnu.org/licenses/gpl-3.0.txt" name="GNU General Public License 3">
//  This file is part of Seven Update.
//    Seven Update is free software: you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation, either version 3 of the License, or
//    (at your option) any later version.
//    Seven Update is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.
//    You should have received a copy of the GNU General Public License
//    along with Seven Update.  If not, see http://www.gnu.org/licenses/.
// </license>
// ***********************************************************************
namespace SevenUpdate.Sdk.Windows
{
    using System;
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Internal;
    using System.Windows.Markup;
    using System.Windows.Media;
    using System.Windows.Navigation;

    using SevenUpdate.Sdk.Properties;

    /// <summary>Interaction logic for MainWindow.xaml</summary>
    [ContentProperty, TemplatePart(Name = "PART_NavWinCP", Type = typeof(ContentPresenter))]
    public sealed partial class MainWindow
    {
        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the MainWindow class</summary>
        public MainWindow()
        {
            this.InitializeComponent();
            NavService = this.NavigationService;
            AeroGlass.CompositionChanged -= this.ChangeWindowChrome;
            AeroGlass.CompositionChanged += this.ChangeWindowChrome;
            NavService = this.NavigationService;
            App.ProcessArgs(App.Args);
            this.Navigate(Core.MainPage);
        }

        #endregion

        #region Properties

        /// <summary>Gets the <see cref = "NavigationService" /> for the current window</summary>
        /// <value>The nav service.</value>
        internal static NavigationService NavService { get; private set; }

        #endregion

        #region Methods

        /// <summary>Enables Aero Glass on the Window</summary>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data</param>
        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            AeroGlass.EnableGlass(this, new Margins(0, 32, 0, 44));
            this.Background = AeroGlass.IsEnabled ? Brushes.Transparent : Brushes.White;
        }

        /// <summary>Changes the Window Background when Aero Glass is enabled or disabled.</summary>
        /// <param name="sender">The Sender</param>
        /// <param name="e">The <see cref="CompositionChangedEventArgs"/> instance containing the event data</param>
        private void ChangeWindowChrome(object sender, CompositionChangedEventArgs e)
        {
            this.Background = e.IsGlassEnabled ? Brushes.Transparent : Brushes.White;

            if (!e.IsGlassEnabled)
            {
                return;
            }

            AeroGlass.EnableGlass(this, new Margins(0, 32, 0, 44));
        }

        /// <summary>Enables the ability to drag the window on glass</summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        private void EnableDragOnGlass(object sender, MouseButtonEventArgs e)
        {
            if (AeroGlass.IsEnabled && e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }

        /// <summary>Sets the Height and Width of the window from the settings</summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void LoadWindowSize(object sender, RoutedEventArgs e)
        {
            this.Height = Settings.Default.windowHeight;
            this.Width = Settings.Default.windowWidth;
        }

        /// <summary>When Seven Update is closing, save the Window Width and Height in the settings</summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.ComponentModel.CancelEventArgs"/> instance containing the event data.</param>
        private void SaveWindowSize(object sender, CancelEventArgs e)
        {
            Settings.Default.windowHeight = this.Height;
            Settings.Default.windowWidth = this.Width;
            Settings.Default.Save();
            Application.Current.Shutdown(0);
        }

        #endregion
    }
}