// ***********************************************************************
// Assembly         : SevenUpdate.Sdk
// Author           : sevenalive
// Created          : 09-17-2010
//
// Last Modified By : sevenalive
// Last Modified On : 10-05-2010
// Description      : 
//
// Copyright        : (c) Seven Software. All rights reserved.
// ***********************************************************************
namespace SevenUpdate.Sdk.Windows
{
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

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    [ContentProperty]
    [TemplatePart(Name = "PART_NavWinCP", Type = typeof(ContentPresenter))]
    public sealed partial class MainWindow
    {
        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the MainWindow class
        /// </summary>
        public MainWindow()
        {
            this.InitializeComponent();

            NavService = this.NavigationService;

            AeroGlass.DwmCompositionChanged += this.ChangeWindowChrome;
        }

        #endregion

        #region Properties

        /// <summary>
        ///   Gets the <see cref = "NavigationService" /> for the current window
        /// </summary>
        /// <value>The nav service.</value>
        internal static NavigationService NavService { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        /// Enables Aero Glass on the Window
        /// </summary>
        /// <param name="e">
        /// The <see cref="EventArgs"/> instance containing the event data
        /// </param>
        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            AeroGlass.EnableGlass(this);
            AeroGlass.ResetAeroGlass(new CoreNativeMethods.Margins(0, 32, 0, 44), this);
            this.Background = AeroGlass.IsEnabled ? Brushes.Transparent : Brushes.White;
        }

        /// <summary>
        /// Changes the Window Background when Aero Glass is enabled or disabled.
        /// </summary>
        /// <param name="sender">
        /// The Sender
        /// </param>
        /// <param name="e">
        /// The <see cref="AeroGlass.DwmCompositionChangedEventArgs"/> instance containing the event data
        /// </param>
        private void ChangeWindowChrome(object sender, AeroGlass.DwmCompositionChangedEventArgs e)
        {
            this.Background = e.IsGlassEnabled ? Brushes.Transparent : Brushes.White;

            if (!e.IsGlassEnabled)
            {
                return;
            }

            AeroGlass.EnableGlass(this);
            AeroGlass.ResetAeroGlass(new CoreNativeMethods.Margins(0, 32, 0, 44), this);
        }

        /// <summary>
        /// Enables the ability to drag the window on glass
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.
        /// </param>
        private void EnableDragOnGlass(object sender, MouseButtonEventArgs e)
        {
            if (AeroGlass.IsEnabled && e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }

        /// <summary>
        /// Sets the Height and Width of the window from the settings
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.
        /// </param>
        private void LoadWindowSize(object sender, RoutedEventArgs e)
        {
            this.Height = Settings.Default.windowHeight;
            this.Width = Settings.Default.windowWidth;
        }

        /// <summary>
        /// When Seven Update is closing, save the Window Width and Height in the settings
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The <see cref="System.ComponentModel.CancelEventArgs"/> instance containing the event data.
        /// </param>
        private void SaveWindowSize(object sender, CancelEventArgs e)
        {
            Settings.Default.windowHeight = this.Height;
            Settings.Default.windowWidth = this.Width;
            Settings.Default.Save();
            Environment.Exit(0);
        }

        #endregion
    }
}