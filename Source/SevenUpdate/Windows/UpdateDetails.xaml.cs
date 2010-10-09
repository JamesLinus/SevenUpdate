// ***********************************************************************
// <copyright file="UpdateDetails.xaml.cs"
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
    using System.Diagnostics;
    using System.Globalization;
    using System.Windows.Input;

    /// <summary>
    /// Interaction logic for Update_Details.xaml
    /// </summary>
    public sealed partial class UpdateDetails
    {
        #region Constants and Fields

        /// <summary>
        ///   The help and support url for the update
        /// </summary>
        private string helpUrl;

        /// <summary>
        ///   The more update information url for the update
        /// </summary>
        private string infoUrl;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the <see cref = "UpdateDetails" /> class.
        /// </summary>
        public UpdateDetails()
        {
            this.InitializeComponent();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Shows the window and displays the update information
        /// </summary>
        /// <param name="updateInfo">
        /// The update information to display
        /// </param>
        internal void ShowDialog(Suh updateInfo)
        {
            this.DataContext = updateInfo;
            this.helpUrl = updateInfo.HelpUrl;
            this.infoUrl = updateInfo.InfoUrl;
            var updateStatus = updateInfo.Status == UpdateStatus.Failed
                                   ? Properties.Resources.Failed.ToLower(CultureInfo.CurrentCulture)
                                   : Properties.Resources.Successful.ToLower(CultureInfo.CurrentCulture);
            this.tbStatus.Text = updateInfo.Status == UpdateStatus.Hidden
                                     ? String.Format(CultureInfo.CurrentCulture, Properties.Resources.DownloadSize, Utilities.ConvertFileSize(updateInfo.UpdateSize))
                                     : String.Format(CultureInfo.CurrentCulture, Properties.Resources.InstallationStatus, updateStatus, updateInfo.InstallDate);

            this.ShowDialog();
            return;
        }

        /// <summary>
        /// Launches the Help <see cref="Uri"/>
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.
        /// </param>
        private void NavigateToHelpUrl(object sender, MouseButtonEventArgs e)
        {
            Process.Start(this.helpUrl);
        }

        /// <summary>
        /// Launches the More Information <see cref="Uri"/>
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.
        /// </param>
        private void NavigateToInfoUrl(object sender, MouseButtonEventArgs e)
        {
            Process.Start(this.infoUrl);
        }

        #endregion
    }
}