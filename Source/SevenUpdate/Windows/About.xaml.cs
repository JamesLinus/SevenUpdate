// ***********************************************************************
// <copyright file="About.xaml.cs"
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
        ///   Displays About Information
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
        /// </param>
        /// <param name="e">
        /// </param>
        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// </summary>
        /// <param name="sender">
        /// </param>
        /// <param name="e">
        /// </param>
        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(e.Uri.AbsoluteUri);
            e.Handled = true;
        }

        #endregion
    }
}