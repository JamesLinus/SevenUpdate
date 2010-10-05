// Copyright 2007-2010 Robert Baker, Seven Software.
// This file is part of Seven Update.
// Seven Update is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
// Seven Update is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.
// You should have received a copy of the GNU General Public License along with Seven Update.  If not, see <http://www.gnu.org/licenses/>.

namespace SevenUpdate.Sdk.Windows
{
    using System.Diagnostics;
    using System.Reflection;
    using System.Windows;
    using System.Windows.Navigation;

    /// <summary>
    /// Interaction logic for About.xaml
    /// </summary>
    public sealed partial class About
    {
        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the <see cref = "About" /> class.
        /// </summary>
        public About()
        {
            this.InitializeComponent();

            var version = Assembly.GetExecutingAssembly().GetName().Version;

            this.tbVersion.Text = version.ToString();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Closes the About window
        /// </summary>
        /// <param name="sender">
        /// The source of the event.
        /// </param>
        /// <param name="e">
        /// The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.
        /// </param>
        private void CloseWindow(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Opens a web browser and navigates to the specified url
        /// </summary>
        /// <param name="sender">
        /// The source of the event.
        /// </param>
        /// <param name="e">
        /// The <see cref="System.Windows.Navigation.RequestNavigateEventArgs"/> instance containing the event data.
        /// </param>
        private void GoToUrl(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(e.Uri.AbsoluteUri);
            e.Handled = true;
        }

        #endregion
    }
}