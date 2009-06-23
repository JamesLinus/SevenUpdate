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
namespace SevenUpdate.Pages
{
    public partial class UpdateHistory : UserControl
    {
        #region Global Vars

        /// <summary>
        /// The list of history items
        /// </summary>
        Collection<UpdateInformation> hist;
        #endregion

        #region Methods
        public UpdateHistory()
        {
            this.Font = SystemFonts.MessageBoxFont;
            InitializeComponent();
            lblHeading.Font = new Font(SystemFonts.MessageBoxFont.Name, 11.25F);
        }

        /// <summary>
        /// Checks if a group exists in the ListView and returns the index
        /// </summary>
        /// <param name="groupName">Name of the group to check</param>
        /// <returns>Returns the index of the group, -1 if not found</returns>
        int CheckGroups(string groupName)
        {
            for (int x = 0; x < listView.Groups.Count; x++)
            {
                if (listView.Groups[x].Header == groupName)
                    return x;
            }
            return -1;
        }
        
        /// <summary>
        /// Gets the update history and loads it to the listView
        /// </summary>
        void GetHistory()
        {
            listView.Items.Clear();
            ListViewItem item = null;
            ListViewGroup group = null;
            listView.Items.Clear();
            listView.Groups.Clear();
            hist = Program.GetHistory();
            foreach (UpdateInformation history in hist)
            {
                string status = null;
                string importance = null;
                if (history.Status == UpdateStatus.Failed)
                    status = Program.RM.GetString("Failed");
                else if (history.Status == UpdateStatus.Hidden)
                    status = Program.RM.GetString("Hidden");
                else if (history.Status == UpdateStatus.Successful)
                    status = Program.RM.GetString("Successful");

                if (history.Importance == Importance.Important)
                    importance = Program.RM.GetString("Important");
                else if (history.Importance == Importance.Optional)
                    importance = Program.RM.GetString("Optional");
                else if (history.Importance == Importance.Recommended)
                    importance = Program.RM.GetString("Recommended");
                else if (history.Importance == Importance.Locale)
                    importance = Program.RM.GetString("Locale");

                string[] info = new string[] { history.UpdateTitle[0].Value, status, importance, history.InstallDate };
                int index = CheckGroups(history.ApplicationName[0].Value);
                if (index < 0)
                {
                    group = new ListViewGroup(history.ApplicationName[0].Value);
                    listView.Groups.Add(group);
                    item = new ListViewItem(info, group);
                }
                else
                {
                    item = new ListViewItem(info, listView.Groups[index]);
                }

                listView.Items.Add(item);
            }
        }

        #endregion

        #region UI Event

        #region Buttons

        void btnOK_Click(object sender, EventArgs e)
        {
            Dispose();
        }

        #endregion

        #region Context Menu

        void cmsMenu_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (listView.SelectedItems.Count == 0)
                e.Cancel = true;
        }

        #endregion

        #region ListView

        private void listView_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView.SelectedItems.Count > 0)
            {
                if (listView.SelectedItems[0].Group != null)
                {
                    int historyIndex = listView.Groups.IndexOf(listView.SelectedItems[0].Group);
                    sidebar.DisplayUpdateInfo(hist[historyIndex].UpdateTitle[0].Value, hist[historyIndex].ReleaseDate,
                       hist[historyIndex].Description[0].Value, hist[historyIndex].InfoUrl, hist[historyIndex].HelpUrl);
                }
            }
            else
                sidebar.HideText();
        }

        #endregion

        #region Form

        void UpdateHistory_Load(object sender, EventArgs e)
        {
            GetHistory();
        }

        #endregion

        #endregion
    }
}
