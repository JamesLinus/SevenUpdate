// ***********************************************************************
// <copyright file="About.xaml.cs" project="SevenUpdate" assembly="SevenUpdate" solution="SevenUpdate" company="Seven Software">
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
    using System.Reflection;
    using System.Windows;
    using System.Windows.Input;
    using System.Windows.Internal;
    using System.Windows.Media;
    using System.Windows.Navigation;

    /// <summary>
    ///   Interaction logic for About.xaml.
    /// </summary>
    public sealed partial class About
    {
        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the About class.
        /// </summary>
        public About()
        {
            this.InitializeComponent();

            var version = Assembly.GetExecutingAssembly().GetName().Version;

            this.tbVersion.Text = version.ToString();

            this.MouseLeftButtonDown -= Core.EnableDragOnGlass;
            this.MouseLeftButtonDown += Core.EnableDragOnGlass;
            AeroGlass.CompositionChanged -= this.ChangeWindowChrome;
            AeroGlass.CompositionChanged += this.ChangeWindowChrome;

            if (AeroGlass.IsGlassEnabled)
            {
                this.line.Visibility = Visibility.Collapsed;
                this.rectangle.Visibility = Visibility.Collapsed;
            }
            else
            {
                this.line.Visibility = Visibility.Visible;
                this.rectangle.Visibility = Visibility.Visible;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        ///   Enables Aero Glass on the Window.
        /// </summary>
        /// <param name="e">
        ///   The <see cref="EventArgs" /> instance containing the event data.
        /// </param>
        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            AeroGlass.EnableGlass(this, new Margins(0, 0, 0, 41));
            this.Background = AeroGlass.IsGlassEnabled ? Brushes.Transparent : Brushes.White;
        }

        /// <summary>
        ///   Changes the Window Background when Aero Glass is enabled or disabled.
        /// </summary>
        /// <param name="sender">
        ///   The object that called the event.
        /// </param>
        /// <param name="e">
        ///   The <see cref="CompositionChangedEventArgs" /> instance containing the event data.
        /// </param>
        private void ChangeWindowChrome(object sender, CompositionChangedEventArgs e)
        {
            if (e.IsGlassEnabled)
            {
                this.line.Visibility = Visibility.Collapsed;
                this.rectangle.Visibility = Visibility.Collapsed;
                this.Background = Brushes.Transparent;
                AeroGlass.EnableGlass(this, new Margins(0, 0, 0, 41));
            }
            else
            {
                this.line.Visibility = Visibility.Visible;
                this.rectangle.Visibility = Visibility.Visible;
                this.Background = Brushes.White;
            }
        }

        /// <summary>
        ///   Closes the About window.
        /// </summary>
        /// <param name="sender">
        ///   The object that called the event.
        /// </param>
        /// <param name="e">
        ///   The <see cref="System.Windows.RoutedEventArgs" /> instance containing the event data.
        /// </param>
        private void CloseWindow(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        ///   Enables the ability to drag the window on glass.
        /// </summary>
        /// <param name="sender">
        ///   The object that called the event.
        /// </param>
        /// <param name="e">
        ///   The <see cref="System.Windows.Input.MouseButtonEventArgs" /> instance containing the event data.
        /// </param>
        private void EnableDragOnGlass(object sender, MouseButtonEventArgs e)
        {
            if (AeroGlass.IsGlassEnabled && e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }

        /// <summary>
        ///   Opens a browser and navigates to the Uri.
        /// </summary>
        /// <param name="sender">
        ///   The object that called the event.
        /// </param>
        /// <param name="e">
        ///   The <see cref="System.Windows.Navigation.RequestNavigateEventArgs" /> instance containing the event data.
        /// </param>
        private void NavigateToUri(object sender, RequestNavigateEventArgs e)
        {
            Utilities.StartProcess(e.Uri.AbsoluteUri);
            e.Handled = true;
        }

        #endregion
    }
}