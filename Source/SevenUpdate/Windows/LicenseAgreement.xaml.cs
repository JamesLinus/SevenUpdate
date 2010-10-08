// ***********************************************************************
// <copyright file="LicenseAgreement.xaml.cs"
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
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Globalization;
    using System.Net;
    using System.Windows;
    using System.Windows.Documents;
    using System.Windows.Input;

    /// <summary>
    /// Interaction logic for License_Agreement.xaml
    /// </summary>
    public sealed partial class LicenseAgreement
    {
        #region Constants and Fields

        /// <summary>
        ///   Current index
        /// </summary>
        private int index;

        /// <summary>
        ///   List of updates that have EULAS
        /// </summary>
        private Collection<Eula> licenseInformation;

        /// <summary>
        ///   An array of the strings that consist of the software licenses
        /// </summary>
        private string[] licenseText;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///   Constructor for the License Agreement page
        /// </summary>
        public LicenseAgreement()
        {
            this.InitializeComponent();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Loads the <see cref="licenseInformation"/> and shows the form
        /// </summary>
        /// <returns>
        /// </returns>
        internal bool? LoadLicenses()
        {
            this.GetLicenseAgreements();

            if (this.licenseInformation.Count < 1 || this.licenseInformation == null)
            {
                return true;
            }

            if (this.licenseInformation.Count > 1)
            {
                this.btnAction.ButtonText = Properties.Resources.Next;
            }

            return this.ShowDialog();
        }

        /// <summary>
        /// Sets the IsEnabled property of btnAction depending if Accept is selected
        /// </summary>
        /// <param name="sender">
        /// </param>
        /// <param name="e">
        /// </param>
        private void Accept_Checked(object sender, RoutedEventArgs e)
        {
            this.btnAction.IsEnabled = true;
        }

        /// <summary>
        /// Displays the next license agreement or returns
        /// </summary>
        /// <param name="sender">
        /// </param>
        /// <param name="e">
        /// </param>
        private void Action_Click(object sender, RoutedEventArgs e)
        {
            if (this.rbDecline.IsChecked == true)
            {
                Core.Applications[this.licenseInformation[this.index].AppIndex].Updates.RemoveAt(this.licenseInformation[this.index].UpdateIndex);
                if (Core.Applications[this.licenseInformation[this.index].AppIndex].Updates.Count == 0)
                {
                    Core.Applications.RemoveAt(this.licenseInformation[this.index].AppIndex);
                }
            }

            this.index++;

            if (this.btnAction.ButtonText == Properties.Resources.Next)
            {
                this.tbHeading.Text = String.Format(CultureInfo.CurrentCulture, Properties.Resources.AcceptLicenseTerms, this.licenseInformation[this.index].Title);
                var mcFlowDoc = new FlowDocument();
                var para = new Paragraph();
                var r = new Run(this.licenseText[this.index]);
                para.Inlines.Add(r);
                mcFlowDoc.Blocks.Add(para);
                this.rtbSLA.Document = mcFlowDoc;
                this.rbAccept.IsChecked = false;
                this.rbDecline.IsChecked = false;
            }

            if (this.btnAction.ButtonText == Properties.Resources.Finish)
            {
                this.DialogResult = Core.Applications.Count > 0;
                this.Close();
            }

            if (this.index != this.licenseInformation.Count - 1)
            {
                return;
            }

            this.btnAction.ButtonText = Properties.Resources.Finish;
            if (Core.Applications.Count > 0)
            {
                this.btnAction.IsShieldNeeded = !Core.Instance.IsAdmin;
            }
        }

        /// <summary>
        /// Closes the window, declining all EULA's
        /// </summary>
        /// <param name="sender">
        /// </param>
        /// <param name="e">
        /// </param>
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        /// <summary>
        /// Sets the IsEnabled property of btnAction depending if Decline selected
        /// </summary>
        /// <param name="sender">
        /// </param>
        /// <param name="e">
        /// </param>
        private void Decline_Checked(object sender, RoutedEventArgs e)
        {
            this.btnAction.IsEnabled = Core.Applications.Count != 1;
        }

        /// <summary>
        /// Downloads the <see cref="licenseInformation"/>
        /// </summary>
        private void DownloadLicenseInformation()
        {
            var worker = new BackgroundWorker();

            worker.DoWork += this.WorkerDoWork;

            this.Cursor = Cursors.Wait;

            worker.RunWorkerCompleted += this.WorkerRunWorkerCompleted;

            worker.RunWorkerAsync();
        }

        /// <summary>
        /// Gets the license agreements from the selected updates
        /// </summary>
        private void GetLicenseAgreements()
        {
            this.licenseInformation = new Collection<Eula>();

            if (Core.Applications == null)
            {
                return;
            }

            for (var x = 0; x < Core.Applications.Count; x++)
            {
                for (var y = 0; y < Core.Applications[x].Updates.Count; y++)
                {
                    if (Core.Applications[x].Updates[y].LicenseUrl == null)
                    {
                        continue;
                    }

                    if (Core.Applications[x].Updates[y].LicenseUrl.Length <= 0)
                    {
                        continue;
                    }

                    var sla = new Eula
                        {
                            LicenseUrl = Core.Applications[x].Updates[y].LicenseUrl, 
                            Title = Utilities.GetLocaleString(Core.Applications[x].Updates[y].Name), 
                            AppIndex = x, 
                            UpdateIndex = y
                        };

                    this.licenseInformation.Add(sla);
                }
            }
        }

        /// <summary>
        /// Downloads the EULA's when the window is loaded
        /// </summary>
        /// <param name="sender">
        /// </param>
        /// <param name="e">
        /// </param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.DownloadLicenseInformation();
        }

        /// <summary>
        /// Downloads the license agreements of the updates
        /// </summary>
        /// <param name="sender">
        /// </param>
        /// <param name="e">
        /// </param>
        private void WorkerDoWork(object sender, DoWorkEventArgs e)
        {
            this.licenseText = new string[this.licenseInformation.Count];

            var wc = new WebClient();

            for (var x = 0; x < this.licenseInformation.Count; x++)
            {
                try
                {
                    this.licenseText[x] = wc.DownloadString(this.licenseInformation[x].LicenseUrl);
                }
                catch (Exception f)
                {
                    Utilities.ReportError(f.Message, Utilities.UserStore);
                    this.licenseText[x] = Properties.Resources.LicenseDownloadError;
                }
            }

            wc.Dispose();
        }

        /// <summary>
        /// Updates the UI when the downloading the license agreements has completed
        /// </summary>
        /// <param name="sender">
        /// </param>
        /// <param name="e">
        /// </param>
        private void WorkerRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.rtbSLA.Cursor = Cursors.IBeam;
            var mcFlowDoc = new FlowDocument();
            var para = new Paragraph();
            var r = new Run(this.licenseText[0]);
            para.Inlines.Add(r);
            mcFlowDoc.Blocks.Add(para);
            this.rtbSLA.Document = mcFlowDoc;

            if (Core.Instance.IsAdmin)
            {
                this.btnAction.IsShieldNeeded = false;
            }
            else
            {
                this.btnAction.IsShieldNeeded = this.licenseInformation.Count == 1;
            }

            this.tbHeading.Text = String.Format(CultureInfo.CurrentCulture, Properties.Resources.AcceptLicenseTerms, this.licenseInformation[0].Title);
            this.rbAccept.IsEnabled = true;
            this.rbDecline.IsEnabled = true;
            this.rtbSLA.Cursor = Cursors.IBeam;
            this.Cursor = Cursors.Arrow;
        }

        #endregion

        /// <summary>
        /// Data containing the <see cref="Update"/> license agreement
        /// </summary>
        private struct Eula
        {
            #region Properties

            /// <summary>
            ///   The index of the application of the update
            /// </summary>
            internal int AppIndex { get; set; }

            /// <summary>
            ///   The <see cref = "Uri" /> for the license agreement
            /// </summary>
            internal string LicenseUrl { get; set; }

            /// <summary>
            ///   The update title
            /// </summary>
            internal string Title { get; set; }

            /// <summary>
            ///   The index of the update
            /// </summary>
            internal int UpdateIndex { get; set; }

            #endregion

            #region Operators

            /// <summary>
            /// </summary>
            /// <returns>
            /// </returns>
            /// <exception cref="NotImplementedException">
            /// </exception>
            public static bool operator ==(Eula x, Eula y)
            {
                throw new NotImplementedException();
            }

            /// <summary>
            /// </summary>
            /// <returns>
            /// </returns>
            /// <exception cref="NotImplementedException">
            /// </exception>
            public static bool operator !=(Eula x, Eula y)
            {
                throw new NotImplementedException();
            }

            #endregion

            #region Public Methods

            /// <summary>
            /// </summary>
            /// <param name="obj">
            /// </param>
            /// <returns>
            /// </returns>
            /// <exception cref="NotImplementedException">
            /// </exception>
            public override bool Equals(object obj)
            {
                throw new NotImplementedException();
            }

            /// <summary>
            /// </summary>
            /// <returns>
            /// </returns>
            /// <exception cref="NotImplementedException">
            /// </exception>
            public override int GetHashCode()
            {
                throw new NotImplementedException();
            }

            #endregion
        }
    }
}