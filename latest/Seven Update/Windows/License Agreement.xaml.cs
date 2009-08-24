/*Copyright 2007-09 Robert Baker, aka Seven ALive.
This file is part of Seven Update.

    Seven Update is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    Seven Update is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with Seven Update.  If not, see <http://www.gnu.org/licenses/>.*/
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;

namespace SevenUpdate.Windows
{
    /// <summary>
    /// Interaction logic for License_Agreement.xaml
    /// </summary>
    public partial class LicenseAgreement : Window
    {
        #region Global Vars

        /// <summary>
        /// Information containing the update's EULA
        /// </summary>
        struct EULA
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

        /// <summary>
        /// An array of the text of the licenses
        /// </summary>
        string[] eulas;

        /// <summary>
        /// Current index
        /// </summary>
        int index;

        /// <summary>
        /// List of updates that have EULAS
        /// </summary>
        Collection<EULA> licenses;

        /// <summary>
        /// Disables the close button
        /// </summary>
        const int CP_NOCLOSE_BUTTON = 0x200;

        #endregion
        public LicenseAgreement()
        {
            InitializeComponent();
        }

        #region Methods

        /// <summary>
        /// Downloads the licenses
        /// </summary>
        void DownloadLicenses()
        {
            BackgroundWorker worker = new BackgroundWorker();

            worker.DoWork += new DoWorkEventHandler(worker_DoWork);

            this.Cursor = System.Windows.Input.Cursors.Wait;

            worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(worker_RunWorkerCompleted);

            worker.RunWorkerAsync();
        }

        /// <summary>
        /// Gets the license agreements from the selected updates
        /// </summary>
        void GetLicenseAgreements()
        {
            licenses = new Collection<EULA>();

            EULA sla;
            if (App.Applications == null)
                return;
            for (int x = 0; x < App.Applications.Count; x++)
            {
                for (int y = 0; y < App.Applications[x].Updates.Count; y++)
                {
                    if (App.Applications[x].Updates[y].LicenseUrl != null)
                    {
                        if (App.Applications[x].Updates[y].LicenseUrl.Length > 0)
                        {
                            sla = new EULA();

                            sla.LicenseUrl = App.Applications[x].Updates[y].LicenseUrl;

                            sla.Title = App.Applications[x].Updates[y].Title[0].Value;

                            sla.AppIndex = x;

                            sla.UpdateIndex = y;

                            licenses.Add(sla);
                        }
                    }
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
            else
            {
                if (licenses.Count > 1)
                    btnAction.Content = App.RM.GetString("Next");

                return ShowDialog();
            }
        }

        /// <summary>
        /// Downloads the text of the EULA's
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            eulas = new string[licenses.Count];

            WebClient wc = new WebClient();

            for (int x = 0; x < licenses.Count; x++)
            { 
                try
                {
                    eulas[x] = wc.DownloadString(licenses[x].LicenseUrl);
                }
                catch (Exception) { eulas[x] = "Error Downloading License Agreement"; }
            }

            rtbSLA.Cursor = Cursors.IBeam;
        }

        #endregion

        #region UI Events

        void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            FlowDocument mcFlowDoc = new FlowDocument();
            Paragraph para = new Paragraph();
            Run r = new Run(eulas[0]);
            para.Inlines.Add(r);
            mcFlowDoc.Blocks.Add(para);
            rtbSLA.Document = mcFlowDoc;

            tbHeading.Text = App.RM.GetString("AcceptLicenseTerms") + " " + licenses[0].Title;
            rbAccept.IsEnabled = true;
            rbDecline.IsEnabled = true;
            rtbSLA.Cursor = Cursors.IBeam;
            this.Cursor = Cursors.Arrow;
            if (licenses.Count == 1 && !App.IsAdmin())
                App.AddShieldToButton(btnAction);
        }

        #region Buttons

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DownloadLicenses();
        }

        private void radioAccept_Checked(object sender, RoutedEventArgs e)
        {
            if (rbAccept.IsChecked == true || rbDecline.IsChecked == true)
                btnAction.IsEnabled = true;
            else
                btnAction.IsEnabled = false;
        }

        private void rbDecline_Checked(object sender, RoutedEventArgs e)
        {
            if (App.Applications.Count == 1)
                if (rbDecline.IsChecked == true)
                    btnAction.IsEnabled = false;
                else
                    btnAction.IsEnabled = true;
        }

        private void btnAction_Click(object sender, RoutedEventArgs e)
        {
            if (rbDecline.IsChecked == true)
            {
                App.Applications[licenses[index].AppIndex].Updates.RemoveAt(licenses[index].UpdateIndex);
                if (App.Applications[licenses[index].AppIndex].Updates.Count == 0)
                    App.Applications.RemoveAt(licenses[index].AppIndex);
            }
            index++;
            
            if (((string)btnAction.Content) == App.RM.GetString("Next"))
            {
                tbHeading.Text = App.RM.GetString("AcceptLicenseTerms") + " " + licenses[index].Title;
                FlowDocument mcFlowDoc = new FlowDocument();
                Paragraph para = new Paragraph();
                Run r = new Run(licenses[index].LicenseUrl);
                para.Inlines.Add(r);
                mcFlowDoc.Blocks.Add(para);
                rtbSLA.Document = mcFlowDoc;
                rbAccept.IsChecked = false;
                rbDecline.IsChecked = false;
            }
            if (((string)btnAction.Content) == App.RM.GetString("Finish"))
            {
                if (App.Applications.Count > 0)
                    this.DialogResult = true;
                else
                    this.DialogResult = false;
                Close();
            }
            if (index == licenses.Count - 1)
            {
                btnAction.Content = App.RM.GetString("Finish");
                if (!App.IsAdmin() && App.Applications.Count > 0)
                    App.AddShieldToButton(btnAction);
            }
        }
        #endregion

        #endregion
    }
}