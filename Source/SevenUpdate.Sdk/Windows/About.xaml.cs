// ***********************************************************************
// <copyright file="About.xaml.cs"
//            project="SevenUpdate.Sdk"
//            assembly="SevenUpdate.Sdk"
//            solution="SevenUpdate"
//            company="Seven Software">
//     Copyright (c) Seven Software. All rights reserved.
// </copyright>
// <author username="sevenalive">Robert Baker</author>
// <license href="http://www.gnu.org/licenses/gpl-3.0.txt" name="GNU General Public License 3">
//  This file is part of Seven Update.
//
//    Seven Update is free software: you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation, either version 3 of the License, or
//    (at your option) any later version.
//
//    Seven Update is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.
//
//    You should have received a copy of the GNU General Public License
//    along with Seven Update.  If not, see http://www.gnu.org/licenses/.
// </license>
// ***********************************************************************
namespace SevenUpdate.Sdk.Windows
{
    using System.Reflection;
    using System.Windows;
    using System.Windows.Navigation;

    /// <summary>Interaction logic for About.xaml</summary>
    public sealed partial class About
    {
        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref = "About" /> class.</summary>
        public About()
        {
            this.InitializeComponent();

            var version = Assembly.GetExecutingAssembly().GetName().Version;

            this.tbVersion.Text = version.ToString();
        }

        #endregion

        #region Methods

        /// <summary>Closes the About window</summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void CloseWindow(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>Opens a web browser and navigates to the specified url</summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Navigation.RequestNavigateEventArgs"/> instance containing the event data.</param>
        private void NavigateToUrl(object sender, RequestNavigateEventArgs e)
        {
            Utilities.StartProcess(e.Uri.PathAndQuery);
            e.Handled = true;
        }

        #endregion
    }
}