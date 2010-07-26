#region GNU Public License Version 3

// Copyright 2007-2010 Robert Baker, Seven Software.
// This file is part of Seven Update.
//   
//      Seven Update is free software: you can redistribute it and/or modify
//      it under the terms of the GNU General Public License as published by
//      the Free Software Foundation, either version 3 of the License, or
//      (at your option) any later version.
//  
//      Seven Update is distributed in the hope that it will be useful,
//      but WITHOUT ANY WARRANTY; without even the implied warranty of
//      MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//      GNU General Public License for more details.
//   
//      You should have received a copy of the GNU General Public License
//      along with Seven Update.  If not, see <http://www.gnu.org/licenses/>.

#endregion

#region

using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;

#endregion

namespace SevenUpdate.Windows
{
    /// <summary>
    ///   Interaction logic for License_Agreement.xaml
    /// </summary>
    public sealed partial class LicenseAgreement : Window
    {
        #region Global Vars

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

        /// <summary>
        ///   Data containing the update's license agreement
        /// </summary>
        private struct Eula
        {
            /// <summary>
            ///   The index of the application of the update
            /// </summary>
            internal int AppIndex { get; set; }

            /// <summary>
            ///   The Url for the license agreement
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
        }

        #endregion

        /// <summary>
        ///   Constructor for the License Agreement page
        /// </summary>
        public LicenseAgreement()
        {
            InitializeComponent();
        }

        #region Methods

        /// <summary>
        ///   Downloads the licenseInformation
        /// </summary>
        private void DownloadLicenseInformation()
        {
            var worker = new BackgroundWorker();

            worker.DoWork += WorkerDoWork;

            Cursor = Cursors.Wait;

            worker.RunWorkerCompleted += WorkerRunWorkerCompleted;

            worker.RunWorkerAsync();
        }

        /// <summary>
        ///   Gets the license agreements from the selected updates
        /// </summary>
        private void GetLicenseAgreements()
        {
            licenseInformation = new Collection<Eula>();

            if (App.Applications == null)
                return;
            for (var x = 0; x < App.Applications.Count; x++)
            {
                for (var y = 0; y < App.Applications[x].Updates.Count; y++)
                {
                    if (App.Applications[x].Updates[y].LicenseUrl == null)
                        continue;
                    if (App.Applications[x].Updates[y].LicenseUrl.Length <= 0)
                        continue;
                    var sla = new Eula {LicenseUrl = App.Applications[x].Updates[y].LicenseUrl, Title = Base.Base.GetLocaleString(App.Applications[x].Updates[y].Name), AppIndex = x, UpdateIndex = y};

                    licenseInformation.Add(sla);
                }
            }
        }

        /// <summary>
        ///   Loads the licenseInformation and shows the form
        /// </summary>
        /// <returns></returns>
        internal bool? LoadLicenses()
        {
            GetLicenseAgreements();

            if (licenseInformation.Count < 1 || licenseInformation == null)
                return true;
            if (licenseInformation.Count > 1)
                btnAction.ButtonText = App.RM.GetString("Next");

            return ShowDialog();
        }

        /// <summary>
        ///   Downloads the license agreements of the updates
        /// </summary>
        /// <param name = "sender"></param>
        /// <param name = "e"></param>
        private void WorkerDoWork(object sender, DoWorkEventArgs e)
        {
            licenseText = new string[licenseInformation.Count];

            var wc = new WebClient();

            for (var x = 0; x < licenseInformation.Count; x++)
            {
                try
                {
                    licenseText[x] = wc.DownloadString(licenseInformation[x].LicenseUrl);
                }
                catch (Exception f)
                {
                    Base.Base.ReportError(f.Message, Base.Base.UserStore);
                    licenseText[x] = "Error Downloading License Agreement";
                }
            }
            wc.Dispose();
        }

        #endregion

        #region UI Events

        /// <summary>
        ///   Updates the UI when the downloading the license agreements has completed
        /// </summary>
        private void WorkerRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            rtbSLA.Cursor = Cursors.IBeam;
            var mcFlowDoc = new FlowDocument();
            var para = new Paragraph();
            var r = new Run(licenseText[0]);
            para.Inlines.Add(r);
            mcFlowDoc.Blocks.Add(para);
            rtbSLA.Document = mcFlowDoc;
            btnAction.IsShieldNeeded = licenseInformation.Count == 1;
            tbHeading.Text = App.RM.GetString("AcceptLicenseTerms") + " " + licenseInformation[0].Title;
            rbAccept.IsEnabled = true;
            rbDecline.IsEnabled = true;
            rtbSLA.Cursor = Cursors.IBeam;
            Cursor = Cursors.Arrow;
        }

        #region Buttons

        /// <summary>
        ///   Closes the window, declining all EULA's
        /// </summary>
        /// <param name = "sender"></param>
        /// <param name = "e"></param>
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        /// <summary>
        ///   Downloads the EULA's when the window is loaded
        /// </summary>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DownloadLicenseInformation();
        }

        /// <summary>
        ///   Sets the IsEnabled property of btnAction depending if Accept is selected
        /// </summary>
        private void Accept_Checked(object sender, RoutedEventArgs e)
        {
            btnAction.IsEnabled = true;
        }

        /// <summary>
        ///   Sets the IsEnabled property of btnAction depending if Decline selected
        /// </summary>
        private void Decline_Checked(object sender, RoutedEventArgs e)
        {
            if (App.Applications.Count != 1)
            {
                btnAction.IsEnabled = true;;
            }
            else
            {
                btnAction.IsEnabled = false;
            }
        }

        /// <summary>
        ///   Displays the next license agreement or returns
        /// </summary>
        private void Action_Click(object sender, RoutedEventArgs e)
        {
            if (rbDecline.IsChecked == true)
            {
                App.Applications[licenseInformation[index].AppIndex].Updates.RemoveAt(licenseInformation[index].UpdateIndex);
                if (App.Applications[licenseInformation[index].AppIndex].Updates.Count == 0)
                    App.Applications.RemoveAt(licenseInformation[index].AppIndex);
            }
            index++;

            if (btnAction.ButtonText == App.RM.GetString("Next"))
            {
                tbHeading.Text = App.RM.GetString("AcceptLicenseTerms") + " " + licenseInformation[index].Title;
                var mcFlowDoc = new FlowDocument();
                var para = new Paragraph();
                var r = new Run(licenseText[index]);
                para.Inlines.Add(r);
                mcFlowDoc.Blocks.Add(para);
                rtbSLA.Document = mcFlowDoc;
                rbAccept.IsChecked = false;
                rbDecline.IsChecked = false;
            }
            if (btnAction.ButtonText == App.RM.GetString("Finish"))
            {
                DialogResult = App.Applications.Count > 0;
                Close();
            }
            if (index != licenseInformation.Count - 1)
                return;
            btnAction.ButtonText = App.RM.GetString("Finish");
            if(App.Applications.Count > 0)
                btnAction.IsShieldNeeded = true;
        }

        #endregion

        #endregion
    }
}