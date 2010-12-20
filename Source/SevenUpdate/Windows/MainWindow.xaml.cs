// ***********************************************************************
// <copyright file="MainWindow.xaml.cs"
//            project="SevenUpdate"
//            assembly="SevenUpdate"
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
namespace SevenUpdate.Windows
{
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Markup;
    using System.Windows.Navigation;

    using SevenUpdate.Properties;

    /// <summary>Interaction logic for MainWindow.xaml</summary>
    [ContentProperty, TemplatePart(Name = @"PART_NavWinCP", Type = typeof(ContentPresenter))]
    public sealed partial class MainWindow
    {
        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref = "MainWindow" /> class.</summary>
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

            App.TaskBar = this.taskBar;
            NavService = this.NavigationService;
            App.ProcessArgs(App.Args);

            Core.NavigateToMainPage();
        }

        #endregion

        #region Properties

        /// <summary>Gets the <see cref = "NavigationService" /> for the current window</summary>
        /// <value>The nav service.</value>
        internal static NavigationService NavService { get; private set; }

        #endregion

        #region Methods

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
            WcfService.Disconnect();
            Application.Current.Shutdown(0);
        }

        #endregion
    }
}