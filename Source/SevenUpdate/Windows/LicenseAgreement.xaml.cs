// ***********************************************************************
// <copyright file="LicenseAgreement.xaml.cs" project="SevenUpdate" assembly="SevenUpdate" solution="SevenUpdate" company="Seven Software">
//     Copyright (c) Seven Software. All rights reserved.
// </copyright>
// <author username="sevenalive">Robert Baker</author>
// <license href="http://www.gnu.org/licenses/gpl-3.0.txt" name="GNU General Public License 3">
//  This file is part of Seven Update.
//    Seven Update is free software: you can redistribute it and/or modify it under the terms of the GNU General Public
//    License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any
//    later version. Seven Update is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without
//    even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details. You should have received a copy of the GNU General Public License
//    along with Seven Update.  If not, see http://www.gnu.org/licenses/.
// </license>
// <summary>
//   Interaction logic for LicenseAgreement.xaml
// .</summary> ***********************************************************************

namespace SevenUpdate.Windows
{
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Globalization;
    using System.Net;
    using System.Windows;
    using System.Windows.Documents;
    using System.Windows.Input;

    /// <summary>Interaction logic for License_Agreement.xaml.</summary>
    public sealed partial class LicenseAgreement
    {
        #region Constants and Fields

        /// <summary>Current index.</summary>
        private int index;

        /// <summary>List of updates that have EULAS.</summary>
        private Collection<Eula> licenseInformation;

        /// <summary>An array of the strings that consist of the software licenses.</summary>
        private string[] licenseText;

        #endregion

        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref = "LicenseAgreement" /> class.</summary>
        public LicenseAgreement()
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

        #endregion

        #region Methods

        /// <summary>Loads the <c>licenseInformation</c> and shows the form.</summary>
        /// <returns>Returns the dialog result.</returns>
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

        /// <summary>Closes the window, declining all software licenses.</summary>
        /// <param name = "sender">The object that called the event.</param>
        /// <param name = "e">The <c>System.Windows.RoutedEventArgs</c> instance containing the event data.</param>
        private void Cancel(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        /// <summary>Updates the UI with the licenses and displays the first license.</summary>
        /// <param name = "sender">The object that called the event.</param>
        /// <param name = "e">The <c>System.ComponentModel.RunWorkerCompletedEventArgs</c> instance containing the event data.</param>
        private void DisplayLicense(object sender, RunWorkerCompletedEventArgs e)
        {
            this.rtbSLA.Cursor = Cursors.IBeam;
            var flowDoc = new FlowDocument();
            var para = new Paragraph();
            var r = new Run(this.licenseText[0]);
            para.Inlines.Add(r);
            flowDoc.Blocks.Add(para);
            this.rtbSLA.Document = flowDoc;

            if (Core.Instance.IsAdmin)
            {
                this.btnAction.IsShieldNeeded = false;
            }
            else
            {
                this.btnAction.IsShieldNeeded = this.licenseInformation.Count == 1;
            }

            this.tbHeading.Text = string.Format(
                CultureInfo.CurrentCulture, Properties.Resources.AcceptLicenseTerms, this.licenseInformation[0].Title);
            this.rbtnAccept.IsEnabled = true;
            this.rbtnDecline.IsEnabled = true;
            this.rtbSLA.Cursor = Cursors.IBeam;
            this.Cursor = Cursors.Arrow;
        }

        /// <summary>Downloads the <c>licenseInformation</c>.</summary>
        /// <param name = "sender">The object that called the event.</param>
        /// <param name = "e">The <c>System.Windows.RoutedEventArgs</c> instance containing the event data.</param>
        private void DownloadLicenseInformation(object sender, RoutedEventArgs e)
        {
            var worker = new BackgroundWorker();

            worker.DoWork -= this.DownloadLicenses;
            worker.DoWork += this.DownloadLicenses;

            this.Cursor = Cursors.Wait;

            worker.RunWorkerCompleted -= this.DisplayLicense;
            worker.RunWorkerCompleted += this.DisplayLicense;

            worker.RunWorkerAsync();
        }

        /// <summary>Downloads the license agreements of the updates.</summary>
        /// <param name = "sender">The object that called the event.</param>
        /// <param name = "e">The <c>System.ComponentModel.DoWorkEventArgs</c> instance containing the event data.</param>
        private void DownloadLicenses(object sender, DoWorkEventArgs e)
        {
            this.licenseText = new string[this.licenseInformation.Count];

            var wc = new WebClient();

            for (var x = 0; x < this.licenseInformation.Count; x++)
            {
                try
                {
                    this.licenseText[x] = wc.DownloadString(this.licenseInformation[x].LicenseUrl);
                }
                catch (WebException ex)
                {
                    Utilities.ReportError(ex, ErrorType.DownloadError);
                    this.licenseText[x] = Properties.Resources.LicenseDownloadError;
                }
            }

            wc.Dispose();
        }

        /// <summary>Gets the license agreements from the selected updates.</summary>
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

        /// <summary>Displays the next license agreement or returns the collection of updates.</summary>
        /// <param name = "sender">The object that called the event.</param>
        /// <param name = "e">The <c>System.Windows.RoutedEventArgs</c> instance containing the event data.</param>
        private void PerformAction(object sender, RoutedEventArgs e)
        {
            if (this.rbtnDecline.IsChecked == true)
            {
                Core.Applications[this.licenseInformation[this.index].AppIndex].Updates.RemoveAt(
                    this.licenseInformation[this.index].UpdateIndex);
                if (Core.Applications[this.licenseInformation[this.index].AppIndex].Updates.Count == 0)
                {
                    Core.Applications.RemoveAt(this.licenseInformation[this.index].AppIndex);
                }
            }

            this.index++;

            if (this.btnAction.ButtonText == Properties.Resources.Next)
            {
                this.tbHeading.Text = string.Format(
                    CultureInfo.CurrentCulture, 
                    Properties.Resources.AcceptLicenseTerms, 
                    this.licenseInformation[this.index].Title);
                var flowDoc = new FlowDocument();
                var para = new Paragraph();
                var r = new Run(this.licenseText[this.index]);
                para.Inlines.Add(r);
                flowDoc.Blocks.Add(para);
                this.rtbSLA.Document = flowDoc;
                this.rbtnAccept.IsChecked = false;
                this.rbtnDecline.IsChecked = false;
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

        #endregion

        /// <summary>Data containing the <c>Update</c> license agreement.</summary>
        private struct Eula
        {
            #region Properties

            /// <summary>Gets or sets the index of the application of the update.</summary>
            internal int AppIndex { get; set; }

            /// <summary>Gets or sets the <c>Uri</c> for the license agreement.</summary>
            internal string LicenseUrl { get; set; }

            /// <summary>Gets or sets the update title.</summary>
            internal string Title { get; set; }

            /// <summary>Gets or sets the index of the update.</summary>
            internal int UpdateIndex { get; set; }

            #endregion
        }
    }
}
