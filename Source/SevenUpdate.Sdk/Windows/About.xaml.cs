// ***********************************************************************
// Assembly         : SevenUpdate.Sdk
// Author           : Robert Baker (sevenalive)
// Last Modified By : Robert Baker (sevenalive)
// Last Modified On : 10-06-2010
// Copyright        : (c) Seven Software. All rights reserved.
// ***********************************************************************
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