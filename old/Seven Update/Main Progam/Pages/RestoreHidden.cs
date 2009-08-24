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
using System.Drawing;
using System.Windows.Forms;
using SevenUpdate.Properties;
using SevenUpdate.WCF;
namespace SevenUpdate.Pages
{
    public partial class RestoreHidden : UserControl
    {
        #region Global Vars

        /// <summary>
        /// Collection of hidden updates
        /// </summary>
        Collection<UpdateInformation> hid;

        #endregion

        public RestoreHidden()
        {
            this.Font = SystemFonts.MessageBoxFont;

            InitializeComponent();

            lblHeading.Font = new Font(SystemFonts.MessageBoxFont.Name, 11.25F);

            if (!Program.IsAdmin())
            {
                Program.AddShieldToButton(btnRestore);
            }
        }

        #region Event Declarations

        public static event EventHandler<EventArgs> RestoredHiddenUpdateEventHandler;

        #endregion

        #region Methods

        /// <summary>
        /// Checks if the group already exists and returns the index
        /// </summary>
        /// <param name="header">The groupName</param>
        /// <returns>Returns the index of the group, -1 if does not exist</returns>
        int CheckGroups(string header)
        {
            for (int x = 0; x < listView.Groups.Count; x++)
            {
                if (listView.Groups[x].Header == header)
                    return x;
            }
            return -1;
        }

        /// <summary>
        /// Gets the hidden updates and loads them in the listView
        /// </summary>
        void GetHiddenUpdates()
        {
            listView.Items.Clear();

            ListViewItem item = null;

            ListViewGroup group = null;

            listView.Items.Clear();

            listView.Groups.Clear();

            hid = Program.GetHiddenUpdates();

            chkSelectAll.Enabled = false;

            btnRestore.Enabled = false;

            foreach (UpdateInformation hidden in hid)
            {
                string[] info = new string[] { null, hidden.UpdateTitle[0].Value, Program.RM.GetString("Hidden"), hidden.ReleaseDate };

                int index = CheckGroups(hidden.ApplicationName[0].Value);

                if (index < 0)
                {
                    group = new ListViewGroup(hidden.ApplicationName[0].Value);

                    listView.Groups.Add(group);

                    item = new ListViewItem(info, group);

                }
                else
                {
                    item = new ListViewItem(info, listView.Groups[index]);
                }

                listView.Groups.Add(group);

                item = new ListViewItem(info, group);

                listView.Items.Add(item);
            }

            if (hid.Count > 0)
            {
                chkSelectAll.Enabled = true;
            }
        }

        #endregion

        #region UI Events

        #region Buttons

        void btnCancel_Click(object sender, EventArgs e)
        {
            Dispose();
        }

        void btnRestore_Click(object sender, EventArgs e)
        {
            Collection<UpdateInformation> hidden = new Collection<UpdateInformation>();
            UpdateInformation hide;
            for (int x = 0; x < listView.Items.Count; x++)
            {
                if (!listView.Items[x].Checked)
                {
                    hide = new UpdateInformation();
                    hide.ApplicationName = hid[x].ApplicationName;
                    hide.HelpUrl = hid[x].HelpUrl;
                    hide.InfoUrl = hid[x].InfoUrl;
                    hide.Publisher = hid[x].Publisher;
                    hide.PublisherUrl = hid[x].PublisherUrl;
                    hide.ReleaseDate = hid[x].ReleaseDate;
                    hide.Status = hid[x].Status;
                    hide.Importance = hid[x].Importance;
                    hide.Description = hid[x].Description;
                    hide.UpdateTitle = hid[x].UpdateTitle;
                    hidden.Add(hide);
                }
            }
            if (Client.HideUpdates(hidden))
            {
                if (RestoredHiddenUpdateEventHandler != null)
                    RestoredHiddenUpdateEventHandler(this, new EventArgs());
                Dispose();
            }
        }

        #endregion

        void chkSelectAll_CheckedChanged(object sender, EventArgs e)
        {
            if (chkSelectAll.CheckState == CheckState.Checked)
            {
                for (int x = 0; x < listView.Items.Count; x++)
                {
                    if (listView.Items[x] != null)
                        listView.Items[x].Checked = true;
                }
            }
            if (chkSelectAll.CheckState == CheckState.Unchecked)
            {
                for (int x = 0; x < listView.Items.Count; x++)
                {
                    if (listView.Items[x] != null)
                        listView.Items[x].Checked = false;
                }
            }
        }

        void cmsMenu_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (listView.SelectedItems.Count == 0)
                e.Cancel = true;
        }

        #region ListView

        void listView_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (listView.Items[e.Index].ForeColor != SystemColors.WindowText)
                e.NewValue = CheckState.Unchecked;
        }

        void listView_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            chkSelectAll.Enabled = true;
            if (listView.CheckedItems.Count == 0)
                btnRestore.Enabled = false;
            else
                btnRestore.Enabled = true;

            if (listView.CheckedItems.Count > 0)
                lblSelected.Text = Program.RM.GetString("TotalSelected") + " " + listView.CheckedItems.Count + " " + Program.RM.GetString("Updates");
            else
                if (listView.CheckedItems.Count == 1)
                    lblSelected.Text = Program.RM.GetString("TotalSelected") + " 0 " + Program.RM.GetString("Update");
                else
                    lblSelected.Text = Program.RM.GetString("TotalSelected") + " 0 " + Program.RM.GetString("Updates");

            if (listView.Items.Count == listView.CheckedItems.Count)
                chkSelectAll.CheckState = CheckState.Checked;
            else
                if (listView.CheckedItems.Count == 0)
                    chkSelectAll.CheckState = CheckState.Unchecked;
                else
                    chkSelectAll.CheckState = CheckState.Indeterminate;

            if (listView.Items.Count == 0)
            {
                chkSelectAll.Enabled = false;
                chkSelectAll.CheckState = CheckState.Unchecked;
            }
        }

        void listView_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView.SelectedItems.Count > 0)
            {
                if (listView.SelectedItems[0].Group != null)
                {
                    int updateIndex = listView.Groups.IndexOf(listView.SelectedItems[0].Group);

                    sidebar.DisplayUpdateInfo(hid[updateIndex].UpdateTitle[0].Value, hid[updateIndex].ReleaseDate,
                       hid[updateIndex].Description[0].Value, hid[updateIndex].InfoUrl, hid[updateIndex].HelpUrl);
                }
            }
            else
                sidebar.HideText();
        }

        #endregion

        void RestoreHidden_Load(object sender, EventArgs e)
        {
            GetHiddenUpdates();
        }

        #endregion

    }
}