#region GNU Public License v3

// Copyright 2007, 2008 Robert Baker, aka Seven ALive.
// This file is part of Seven Update.
// 
//     Seven Update is free software: you can redistribute it and/or modify
//     it under the terms of the GNU General Public License as published by
//     the Free Software Foundation, either version 3 of the License, or
//     (at your option) any later version.
// 
//     Seven Update is distributed in the hope that it will be useful,
//     but WITHOUT ANY WARRANTY; without even the implied warranty of
//     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//     GNU General Public License for more details.
// 
//    You should have received a copy of the GNU General Public License
//     along with Seven Update.  If not, see <http://www.gnu.org/licenses/>.

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
    /// Interaction logic for License_Agreement.xaml
    /// </summary>
    public partial class LicenseAgreement : Window
    {
        #region Global Vars

        /// <summary>
        /// An array of the text of the licenses
        /// </summary>
        private string[] licenseSources;

        /// <summary>
        /// Current index
        /// </summary>
        private int index;

        /// <summary>
        /// List of updates that have EULAS
        /// </summary>
        private Collection<EULA> licenses;

        /// <summary>
        /// Data containing the update's license agreement
        /// </summary>
        private struct EULA
        {
            /// <summary>
            /// The index of the application of the update
            /// </summary>
            internal int AppIndex { get; set; }

            /// <summary>
            /// The URL for the license agreement
            /// </summary>
            internal string LicenseUrl { get; set; }

            /// <summary>
            /// The update title
            /// </summary>
            internal string Title { get; set; }

            /// <summary>
            /// The index of the update
            /// </summary>
            internal int UpdateIndex { get; set; }
        }

        #endregion

        /// <summary>
        /// Constructor for the License Agreement page
        /// </summary>
        public LicenseAgreement()
        {
            InitializeComponent();
        }

        #region Methods

        /// <summary>
        /// Downloads the licenses
        /// </summary>
        private void DownloadLicenses()
        {
            var worker = new BackgroundWorker();

            worker.DoWork += WorkerDoWork;

            Cursor = Cursors.Wait;

            worker.RunWorkerCompleted += WorkerRunWorkerCompleted;

            worker.RunWorkerAsync();
        }

        /// <summary>
        /// Gets the license agreements from the selected updates
        /// </summary>
        private void GetLicenseAgreements()
        {
            licenses = new Collection<EULA>();

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
                    var sla = new EULA {LicenseUrl = App.Applications[x].Updates[y].LicenseUrl, Title = Shared.GetLocaleString(App.Applications[x].Updates[y].Name), AppIndex = x, UpdateIndex = y};

                    licenses.Add(sla);
                }
            }
        }

        /// <summary>
        /// Loads the licenses and shows the form
        /// </summary>
        /// <returns></returns>
        internal bool? LoadLicenses()
        {
            GetLicenseAgreements();

            if (licenses.Count < 1 || licenses == null)
                return true;
            if (licenses.Count > 1)
                tbAction.Text = App.RM.GetString("Next");

            return ShowDialog();
        }

        /// <summary>
        /// Downloads the license agreements of the updates
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WorkerDoWork(object sender, DoWorkEventArgs e)
        {
            licenseSources = new string[licenses.Count];

            var wc = new WebClient();

            for (var x = 0; x < licenses.Count; x++)
            {
                try
                {
                    licenseSources[x] = wc.DownloadString(licenses[x].LicenseUrl);
                }
                catch (Exception f)
                {
                    Shared.ReportError(f.Message, Shared.UserStore);
                    licenseSources[x] = "Error Downloading License Agreement";
                }
            }

            rtbSLA.Cursor = Cursors.IBeam;
            wc.Dispose();
        }

        #endregion

        #region UI Events

        /// <summary>
        /// Updates the UI when the downloading the license agreements has completed
        /// </summary>
        private void WorkerRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            var mcFlowDoc = new FlowDocument();
            var para = new Paragraph();
            var r = new Run(licenseSources[0]);
            para.Inlines.Add(r);
            mcFlowDoc.Blocks.Add(para);
            rtbSLA.Document = mcFlowDoc;

            tbHeading.Text = App.RM.GetString("AcceptLicenseTerms") + " " + licenses[0].Title;
            rbAccept.IsEnabled = true;
            rbDecline.IsEnabled = true;
            rtbSLA.Cursor = Cursors.IBeam;
            Cursor = Cursors.Arrow;
            if (licenses.Count == 1 && !App.IsAdmin)
                imgAdminShield.Visibility = Visibility.Visible;
        }

        #region Buttons

        /// <summary>
        /// Closes the window, declining all EULA's
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        /// <summary>
        /// Downloads the EULA's when the window is loaded
        /// </summary>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DownloadLicenses();
        }

        /// <summary>
        /// Sets the IsEnabled property of btnAction depending if Accept is selected
        /// </summary>
        private void radioAccept_Checked(object sender, RoutedEventArgs e)
        {
            if (rbAccept.IsChecked == true || rbDecline.IsChecked == true)
                btnAction.IsEnabled = true;
            else
                btnAction.IsEnabled = false;
        }

        /// <summary>
        /// Sets the IsEnabled property of btnAction depending if Decline selected
        /// </summary>
        private void rbDecline_Checked(object sender, RoutedEventArgs e)
        {
            if (App.Applications.Count != 1)
                return;
            btnAction.IsEnabled = rbDecline.IsChecked != true;
        }

        /// <summary>
        /// Displays the next license agreement or returns
        /// </summary>
        private void btnAction_Click(object sender, RoutedEventArgs e)
        {
            if (rbDecline.IsChecked == true)
            {
                App.Applications[licenses[index].AppIndex].Updates.RemoveAt(licenses[index].UpdateIndex);
                if (App.Applications[licenses[index].AppIndex].Updates.Count == 0)
                    App.Applications.RemoveAt(licenses[index].AppIndex);
            }
            index++;

            if ((tbAction.Text) == App.RM.GetString("Next"))
            {
                tbHeading.Text = App.RM.GetString("AcceptLicenseTerms") + " " + licenses[index].Title;
                var mcFlowDoc = new FlowDocument();
                var para = new Paragraph();
                var r = new Run(licenses[index].LicenseUrl);
                para.Inlines.Add(r);
                mcFlowDoc.Blocks.Add(para);
                rtbSLA.Document = mcFlowDoc;
                rbAccept.IsChecked = false;
                rbDecline.IsChecked = false;
            }
            if ((tbAction.Text) == App.RM.GetString("Finish"))
            {
                DialogResult = App.Applications.Count > 0;
                Close();
            }
            if (index != licenses.Count - 1)
                return;
            tbAction.Text = App.RM.GetString("Finish");
            if (!App.IsAdmin && App.Applications.Count > 0)
                imgAdminShield.Visibility = Visibility.Visible;
        }

        #endregion

        #endregion
    }
}