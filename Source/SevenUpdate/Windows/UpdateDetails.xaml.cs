// ***********************************************************************
// Assembly         : SevenUpdate
// Author           : sevenalive
// Created          : 09-17-2010
//
// Last Modified By : sevenalive
// Last Modified On : 10-05-2010
// Description      : 
//
// Copyright        : (c) Seven Software. All rights reserved.
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
        /// </summary>
        private string helpUrl;

        /// <summary>
        /// </summary>
        private string infoUrl;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///   Constructor for the Update Details window
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
            this.tbStatus.Text = updateInfo.Status == UpdateStatus.Hidden
                                     ? String.Format(CultureInfo.CurrentCulture, Properties.Resources.DownloadSize, Base.ConvertFileSize(updateInfo.UpdateSize))
                                     : String.Format(
                                         CultureInfo.CurrentCulture, 
                                         Properties.Resources.InstallationStatus, 
                                         updateInfo.Status == UpdateStatus.Failed
                                             ? Properties.Resources.Failed.ToLower(CultureInfo.CurrentCulture)
                                             : Properties.Resources.Successful.ToLower(CultureInfo.CurrentCulture), 
                                         updateInfo.InstallDate);

            this.ShowDialog();
            return;
        }

        /// <summary>
        /// Launches the Help <see cref="Uri"/>
        /// </summary>
        /// <param name="sender">
        /// </param>
        /// <param name="e">
        /// </param>
        private void HelpUrl_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Process.Start(this.helpUrl);
        }

        /// <summary>
        /// Launches the More Information <see cref="Uri"/>
        /// </summary>
        /// <param name="sender">
        /// </param>
        /// <param name="e">
        /// </param>
        private void MoreInfoUrl_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Process.Start(this.infoUrl);
        }

        #endregion
    }
}