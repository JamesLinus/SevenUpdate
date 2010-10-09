// ***********************************************************************
// <copyright file="MainWindow.xaml.cs"
//            project="SevenUpdate"
//            assembly="SevenUpdate"
//            solution="SevenUpdate"
//            company="Seven Software">
//     Copyright (c) Seven Software. All rights reserved.
// </copyright>
// <author username="sevenalive">Robert Baker</author>
// ***********************************************************************
namespace SevenUpdate.Windows
{
    using System;
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Markup;

    using SevenUpdate.Properties;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    [ContentProperty]
    [TemplatePart(Name = @"PART_NavWinCP", Type = typeof(ContentPresenter))]
    public sealed partial class MainWindow
    {
        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the <see cref = "MainWindow" /> class.
        /// </summary>
        public MainWindow()
        {
            this.InitializeComponent();
            App.TaskBar = this.taskBar;
            App.NavService = this.NavigationService;
        }

        #endregion

        #region Methods

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
            AdminClient.Disconnect();
            Environment.Exit(0);
        }

        #endregion
    }
}