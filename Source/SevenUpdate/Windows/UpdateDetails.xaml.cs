// <copyright file="UpdateDetails.xaml.cs" project="SevenUpdate">Robert Baker</copyright>
// <license href="http://www.gnu.org/licenses/gpl-3.0.txt" name="GNU General Public License 3" />

namespace SevenUpdate.Windows
{
    using System.Globalization;
    using System.Windows.Input;

    /// <summary>Interaction logic for Update_Details.xaml.</summary>
    public sealed partial class UpdateDetails
    {
        /// <summary>The help and support url for the update.</summary>
        string helpUrl;

        /// <summary>The more update information url for the update.</summary>
        string infoUrl;

        /// <summary>Initializes a new instance of the <see cref="UpdateDetails" /> class.</summary>
        public UpdateDetails()
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
        }

        /// <summary>Shows the window and displays the update information.</summary>
        /// <param name="updateInfo">The update information to display.</param>
        internal void ShowDialog(Suh updateInfo)
        {
            this.DataContext = updateInfo;
            this.helpUrl = updateInfo.HelpUrl;
            this.infoUrl = updateInfo.InfoUrl;
            string updateStatus = updateInfo.Status == UpdateStatus.Failed
                                      ? Properties.Resources.Failed.ToLower(CultureInfo.CurrentCulture)
                                      : Properties.Resources.Successful.ToLower(CultureInfo.CurrentCulture);
            this.tbStatus.Text = updateInfo.Status == UpdateStatus.Hidden
                                     ? string.Format(
                                         CultureInfo.CurrentCulture, 
                                         Properties.Resources.DownloadSize, 
                                         Utilities.ConvertFileSize(updateInfo.UpdateSize))
                                     : string.Format(
                                         CultureInfo.CurrentCulture, 
                                         Properties.Resources.InstallationStatus, 
                                         updateStatus, 
                                         updateInfo.InstallDate);

            this.ShowDialog();
            return;
        }

        /// <summary>Launches the Help <c>Uri</c>.</summary>
        /// <param name="sender">The object that called the event.</param>
        /// <param name="e">The <c>System.Windows.Input.MouseButtonEventArgs</c> instance containing the event data.</param>
        void NavigateToHelpUrl(object sender, MouseButtonEventArgs e)
        {
            Utilities.StartProcess(this.helpUrl);
        }

        /// <summary>Launches the More Information <c>Uri</c>.</summary>
        /// <param name="sender">The object that called the event.</param>
        /// <param name="e">The <c>System.Windows.Input.MouseButtonEventArgs</c> instance containing the event data.</param>
        void NavigateToInfoUrl(object sender, MouseButtonEventArgs e)
        {
            Utilities.StartProcess(this.infoUrl);
        }
    }
}