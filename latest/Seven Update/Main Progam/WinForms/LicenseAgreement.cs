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
using System.Drawing;
using System.Net;
using System.Security.Permissions;
using System.Windows.Forms;

namespace SevenUpdate.WinForms
{
    public partial class LicenseAgreement : Form
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

        #region Constructor

        public LicenseAgreement()
        {
            this.Font = SystemFonts.MessageBoxFont;

            InitializeComponent();

            lblHeading.Font = new Font(SystemFonts.MessageBoxFont.Name, 11.25F);

            txtSLA.Cursor = Cursors.WaitCursor;

            this.Cursor = Cursors.WaitCursor;
        }

        #endregion

        /// <summary>
        /// Disables the close button
        /// </summary>
        protected override CreateParams CreateParams
        {
            [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
            get
            {
                CreateParams myCp = base.CreateParams;

                myCp.ClassStyle = myCp.ClassStyle | CP_NOCLOSE_BUTTON;

                return myCp;
            }
        }

        #region Methods
        
        /// <summary>
        /// Downloads the licenses
        /// </summary>
        void DownloadLicenses()
        {
            BackgroundWorker worker = new BackgroundWorker();

            worker.DoWork += new DoWorkEventHandler(worker_DoWork);

            this.Cursor = Cursors.WaitCursor;

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

            for (int x = 0; x < Program.Applications.Count; x++)
            {
                for (int y = 0; y < Program.Applications[x].Updates.Count; y++)
                {
                    if (Program.Applications[x].Updates[y].LicenseUrl != null)
                    {
                        if (Program.Applications[x].Updates[y].LicenseUrl.Length > 0)
                        {
                            sla = new EULA();

                            sla.LicenseUrl = Program.Applications[x].Updates[y].LicenseUrl;

                            sla.Title = Program.Applications[x].Updates[y].Title[0].Value;

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
        internal DialogResult LoadLicenses()
        {
            GetLicenseAgreements();

            if (licenses.Count < 1)
                return DialogResult.OK;
            else
            {
                if (licenses.Count > 1)
                    btnOK.Text = Program.RM.GetString("Next");

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

            txtSLA.UseWaitCursor = false;
        }

        #endregion

        #region UI Events

        void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            txtSLA.Text = eulas[0];
            lblHeading.Text = Program.RM.GetString("AcceptLicenseTerms") + " " + licenses[0].Title;
            rbAccept.Enabled = true;
            rbDecline.Enabled = true;
            txtSLA.Cursor = Cursors.IBeam;
            this.Cursor = Cursors.Arrow;
            if (licenses.Count == 1 && !Program.IsAdmin())
                    Program.AddShieldToButton(btnOK);
        }

        #region Buttons

        void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            Close();
        }

        void btnOK_Click(object sender, EventArgs e)
        {
            if (rbDecline.Checked)
            {
                Program.Applications[licenses[index].AppIndex].Updates.RemoveAt(licenses[index].UpdateIndex);
                if (Program.Applications[licenses[index].AppIndex].Updates.Count == 0)
                    Program.Applications.RemoveAt(licenses[index].AppIndex);
            }
            index++;
            
            if (btnOK.Text == Program.RM.GetString("Next"))
            {
                lblHeading.Text = Program.RM.GetString("AcceptLicenseTerms") + " " + licenses[index].Title;
                txtSLA.Text = licenses[index].LicenseUrl;
                rbAccept.Checked = false;
                rbDecline.Checked = false;
            }
            if (btnOK.Text == Program.RM.GetString("Finish"))
            {
                if (Program.Applications.Count > 0)
                    this.DialogResult = DialogResult.OK;
                else
                    this.DialogResult = DialogResult.Cancel;
                Close();
            }
            if (index == licenses.Count - 1)
            {
                btnOK.Text = Program.RM.GetString("Finish");
                if (!Program.IsAdmin() && Program.Applications.Count > 0)
                    Program.AddShieldToButton(btnOK);
            }
        }

        #endregion

        #region RadioBoxes

        void rbAccept_CheckedChanged(object sender, EventArgs e)
        {
            if (rbAccept.Checked || rbDecline.Checked)
                btnOK.Enabled = true;
            else
                btnOK.Enabled = false;
        }

        void rbDecline_CheckedChanged(object sender, EventArgs e)
        {
            if (Program.Applications.Count == 1)
                if (rbDecline.Checked)
                    btnOK.Enabled = false;
                else
                    btnOK.Enabled = true;
        }

        #endregion

        #region Form

        void SLA_Load(object sender, EventArgs e)
        {
            DownloadLicenses();
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawRectangle(Pens.LightGray, e.Graphics.VisibleClipBounds.Left, e.Graphics.VisibleClipBounds.Top, e.Graphics.VisibleClipBounds.Width - 1, e.Graphics.VisibleClipBounds.Height - 1);
            base.OnPaint(e);
        }

        #endregion

        #endregion
    }
}
