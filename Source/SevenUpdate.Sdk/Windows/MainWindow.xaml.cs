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
using System.Windows.Media;
using System.Windows.Navigation;
using Microsoft.Windows.Dwm;
using Microsoft.Windows.Internal;
using SevenUpdate.Sdk.Properties;

#endregion

namespace SevenUpdate.Sdk.Windows
{
    /// <summary>
    ///   Interaction logic for MainWindow.xaml
    /// </summary>
    [ContentProperty, TemplatePart(Name = "PART_NavWinCP", Type = typeof (ContentPresenter))]
    public sealed partial class MainWindow
    {
        #region Fields

        internal static NavigationService NavService;

        #endregion

        #region Constructors

        public MainWindow()
        {
            InitializeComponent();

            NavService = NavigationService;

            AeroGlass.DwmCompositionChanged += AeroGlass_DwmCompositionChanged;
        }

        #endregion

        #region Aero

        private void AeroGlass_DwmCompositionChanged(object sender, AeroGlass.DwmCompositionChangedEventArgs e)
        {
            Background = e.IsGlassEnabled ? Brushes.Transparent : Brushes.White;

            if (!e.IsGlassEnabled)
                return;
            AeroGlass.EnableGlass(this);
            AeroGlass.ResetAeroGlass(new CoreNativeMethods.MARGINS(0, 32, 0, 44), this);
        }

        #endregion

        #region Window - Loaded

        /// <summary>
        ///   Sets the Height and Width of the window from the settings
        /// </summary>
        /// <param name = "sender" />
        /// <param name = "e" />
        private void NavigationWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Height = Settings.Default.windowHeight;
            Width = Settings.Default.windowWidth;

            if (App.Args.Length <= 0)
                return;
            switch (App.Args[0])
            {
                case "-newproject":
                    Core.NewProject();
                    break;

                case "-newupdate":
                    Core.AppIndex = Convert.ToInt32(App.Args[1]);

                    Core.NewUpdate();
                    break;

                case "-edit":
                    Core.AppIndex = Convert.ToInt32(App.Args[1]);
                    Core.UpdateIndex = Convert.ToInt32(App.Args[2]);
                    Core.EditItem();
                    break;
            }
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            AeroGlass.EnableGlass(this);
            AeroGlass.ResetAeroGlass(new CoreNativeMethods.MARGINS(0, 32, 0, 44), this);
            Background = AeroGlass.IsEnabled ? Brushes.Transparent : Brushes.White;
        }

        #endregion

        #region Window - Closing

        /// <summary>
        ///   When Seven Update is closing, save the Window Width and Height in the settings
        /// </summary>
        /// <param name = "sender" />
        /// <param name = "e" />
        private void NavigationWindow_Closing(object sender, CancelEventArgs e)
        {
            Settings.Default.windowHeight = Height;
            Settings.Default.windowWidth = Width;
            Settings.Default.Save();
            Environment.Exit(0);
        }

        #endregion

        #region Window - Mouse Left Button Down

        private void NavigationWindow_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (AeroGlass.IsEnabled && e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }

        #endregion
    }
}