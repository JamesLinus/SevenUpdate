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
using System.Drawing;
using System.Windows.Forms;
using SevenUpdate.Properties;
using SevenUpdate.WCF;
namespace SevenUpdate.Pages
{
    /// <summary>
    /// Displays updates avaliable
    /// </summary>
    public partial class AppUpdate : UserControl
    {
        #region Global Vars
        /// <summary>
        /// The index of the update selected
        /// </summary>
        int updateIndex;
       
        /// <summary>
        /// The index of the application of the update selected
        /// </summary>
        int appIndex;
       
        /// <summary>
        /// The index if the selected item in the list
        /// </summary>
        int listIndex;
        #endregion

        #region Event Declarations

        internal static event EventHandler<UpdateSelectionChangedEventArgs> UpdateSelectionChangedEventHandler;

        #region EventArgs

        internal class UpdateSelectionChangedEventArgs : EventArgs
        {
            public UpdateSelectionChangedEventArgs(int importantUpdates,int optionalUpdates, ulong importantDownloadSize,ulong optionalDownloadSize)
            {
                this.ImportantUpdates = importantUpdates;

                this.OptionalUpdates = optionalUpdates;

                this.ImportantDownloadSize = importantDownloadSize;

                this.OptionalDownloadSize = optionalDownloadSize;
            }

            /// <summary>
            /// Number of Important Updates selected
            /// </summary>
            internal int ImportantUpdates { get; set; }

            /// <summary>
            /// Number of Optional Updates selected
            /// </summary>
            internal int OptionalUpdates { get; set; }

            /// <summary>
            /// The total download size in bytes of the important updates
            /// </summary>
            internal ulong ImportantDownloadSize { get; set; }

            /// <summary>
            /// The total download size in bytes of the optional updates
            /// </summary>
            internal ulong OptionalDownloadSize { get; set; }
        }

        #endregion

        #endregion

        public AppUpdate()
        {
            this.Font = SystemFonts.MessageBoxFont;

            InitializeComponent();

            lblHeading.Font = new Font(SystemFonts.MessageBoxFont.Name, 11.25F);

            if (!Program.IsAdmin())
                cmsHideUpdate.Image = Resources.shield;
            else
                cmsHideUpdate.Image = null;

        }        
        
        #region Methods

        #region Methods

        /// <summary>
        /// Adds the updates to the list
        /// </summary>
        void AddUpdates()
        {
            ListViewItem item = null;

            ListViewGroup group = null;

            listView.Items.Clear();

            listView.Groups.Clear();

            foreach (Application app in Program.Applications)
            {
                foreach (Update update in app.Updates)
                {
                    int index = CheckGroups(app.Name[0].Value);

                    if (index < 0)
                    {
                        group = new ListViewGroup(app.Name[0].Value);

                        listView.Groups.Add(group);

                        item = new ListViewItem(group);
                    }
                    else
                        item = new ListViewItem(listView.Groups[index]);

                    item.SubItems.Add(update.Title[0].Value);
                    if (update.Importance == Importance.Important)
                        item.SubItems.Add(Program.RM.GetString("Important"));
                    else if (update.Importance == Importance.Optional)
                        item.SubItems.Add(Program.RM.GetString("Optional"));
                    else if (update.Importance == Importance.Recommended)
                        item.SubItems.Add(Program.RM.GetString("Recommended"));
                    else if (update.Importance == Importance.Locale)
                        item.SubItems.Add(Program.RM.GetString("Locale"));

                    item.SubItems.Add(Shared.ConvertFileSize(Program.GetUpdateSize(update, app.Name[0].Value, app.Directory, app.Is64Bit, false)));

                    ulong size = Program.GetUpdateSize(Program.Applications[appIndex].Updates[updateIndex], Program.Applications[appIndex].Name[0].Value, Program.Applications[appIndex].Directory,
    Program.Applications[appIndex].Is64Bit, true);

                    if (size > 0)
                        item.SubItems.Add(Program.RM.GetString("ReadyToDownload"));
                    else
                        item.SubItems.Add(Program.RM.GetString("ReadyToInstall"));

                    if (app.Name[0].Value == Program.RM.GetString("SevenUpdate"))
                        item.Checked = true;

                    if (update.Importance == Importance.Locale || update.Importance == Importance.Optional)
                        item.Checked = false;

                    listView.Items.Add(item);
                }
            }

            listView.Items[0].Selected = true;
        }

        /// <summary>
        /// Checks if the group already exists and returns the index
        /// </summary>
        /// <param name="header">The groupName</param>
        /// <returns>Returns the index of the group, -1 if does not exist</returns>
        int CheckGroups(string groupName)
        {
            for (int x = 0; x < listView.Groups.Count; x++)
            {
                if (listView.Groups[x].Header == groupName)
                    return x;
            }
            return -1;
        }

        void GetListCount()
        {
            int[] count = new int[2];
            ulong[] fileSize = new ulong[2];
            int a, b;

            for (int x = 0; x < listView.CheckedItems.Count; x++)
            {
               a = listView.Groups.IndexOf(listView.CheckedItems[x].Group);

               b = listView.CheckedItems[x].Group.Items.IndexOf(listView.CheckedItems[x]);


               if (listView.CheckedItems[x].SubItems[2].Text == Program.RM.GetString("Important"))
               {
                   count[0]++;

                   fileSize[0] += Program.GetUpdateSize(Program.Applications[a].Updates[b], Program.Applications[a].Name[0].Value, Program.Applications[a].Directory, Program.Applications[a].Is64Bit, true);
               }

               if (listView.CheckedItems[x].SubItems[2].Text == Program.RM.GetString("Recommended"))
                    if (UpdateSettings.Settings.Recommended)
                    {
                        count[0]++;

                        fileSize[0] += Program.GetUpdateSize(Program.Applications[a].Updates[b], Program.Applications[a].Name[0].Value, Program.Applications[a].Directory, Program.Applications[a].Is64Bit, true);
                    }
                    else
                    {
                        count[1]++;

                        fileSize[1] += Program.GetUpdateSize(Program.Applications[a].Updates[b], Program.Applications[a].Name[0].Value, Program.Applications[a].Directory, Program.Applications[a].Is64Bit, true);
                    }

               if (listView.CheckedItems[x].SubItems[2].Text == Program.RM.GetString("Optional") || listView.CheckedItems[x].SubItems[2].Text == Program.RM.GetString("Locale"))
                {
                    count[1]++;

                    fileSize[1] += Program.GetUpdateSize(Program.Applications[a].Updates[b], Program.Applications[a].Name[0].Value, Program.Applications[a].Directory, Program.Applications[a].Is64Bit, true);
                }

            }

            if (UpdateSelectionChangedEventHandler != null)
                UpdateSelectionChangedEventHandler(this, new UpdateSelectionChangedEventArgs(count[0], count[1], fileSize[0], fileSize[1]));
        }

        internal void RemoveUnSelectedUpdates()
        {
            for (int x = 0; x < listView.Items.Count; x++)
            {
                if (!listView.Items[x].Checked || listView.Items[x].ForeColor == SystemColors.GrayText)
                {
                    Program.Applications[listView.Groups.IndexOf(listView.Items[x].Group)].Updates.RemoveAt(listView.Items[x].Group.Items.IndexOf(listView.Items[x]));
                }
            }
        }

        #endregion

        #region UI Events

        void AppUpdate_Load(object sender, EventArgs e)
        {
            AddUpdates();
        }

        #region Buttons

        void btnCancel_Click(object sender, EventArgs e)
        {
            Visible = false;
            SendToBack();
        }
        
        void btnOK_Click(object sender, EventArgs e)
        {
            GetListCount();
            Visible = false;
            SendToBack();
        }

        #endregion

        #region CheckBoxes

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

        #endregion

        #region Context Menu

        /// <summary>
        /// Hides an update
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void cmsHideUpdate_Click(object sender, EventArgs e)
        {
            UpdateInformation hnh = new UpdateInformation();

            hnh.ApplicationName = Program.Applications[appIndex].Name;

            hnh.HelpUrl = Program.Applications[appIndex].HelpUrl;

            hnh.InfoUrl = Program.Applications[appIndex].Updates[updateIndex].InfoUrl;

            hnh.Publisher = Program.Applications[appIndex].Publisher;

            hnh.PublisherUrl = Program.Applications[appIndex].PublisherUrl;

            hnh.ReleaseDate = Program.Applications[appIndex].Updates[updateIndex].ReleaseDate;

            hnh.Status = UpdateStatus.Hidden;

            hnh.Importance = Program.Applications[appIndex].Updates[updateIndex].Importance;

            hnh.Description = Program.Applications[appIndex].Updates[updateIndex].Description;

            hnh.UpdateTitle = Program.Applications[appIndex].Updates[updateIndex].Title;

            if (cmsHideUpdate.Text == Program.RM.GetString("HideUpdate"))
            {
                if (Client.HideUpdate(hnh))
                {
                    cmsHideUpdate.Text = Program.RM.GetString("ShowUpdate");

                    listView.Items[listIndex].ForeColor = SystemColors.GrayText;

                    listView.Items[listIndex].Checked = false;

                    listView.Items[listIndex].Selected = false;
                }

            }
            else
            {
                if (Client.ShowUpdate(hnh))
                {
                    chkSelectAll.Enabled = true;

                    cmsHideUpdate.Text = Program.RM.GetString("HideUpdate");

                    listView.Items[listIndex].ForeColor = SystemColors.WindowText;

                    listView.Items[listIndex].Selected = false;
                }
            }
        }

        void cmsMenu_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (listView.SelectedItems.Count == 0)
                e.Cancel = true;
            else
               listIndex = listView.SelectedItems[0].Index;

        }

        #endregion

        #region ListView

        /// <summary>
        /// Sets the checkbox state and also grabs the 'Selected Updates' Count
        /// </summary>
        void listView_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            chkSelectAll.Enabled = true;

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

            if (listView.CheckedItems.Count == 0)
            {
                int hidden = 0;

                for (int x = 0; x < listView.Items.Count; x++)
                {
                    if (listView.Items[x] != null)
                        if (listView.Items[x].ForeColor == SystemColors.GrayText)
                            hidden++;
                }

                if (hidden == listView.Items.Count)
                {
                    chkSelectAll.Enabled = false;
                }

                chkSelectAll.CheckState = CheckState.Unchecked;

            }

        }
        
        /// <summary>
        /// If any item is not checked it will uncheck the 'Select all' checkbox
        /// </summary>
        void listView_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (listView.Items[e.Index].ForeColor != SystemColors.WindowText)
                e.NewValue = CheckState.Unchecked;
        }

        /// <summary>
        /// Switches the Context menu text
        /// </summary>
        void listView_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView.SelectedItems.Count > 0)
            {
                if (listView.SelectedItems[0].ForeColor == SystemColors.GrayText)
                    cmsHideUpdate.Text = Program.RM.GetString("ShowUpdate");
                else
                    cmsHideUpdate.Text = Program.RM.GetString("HideUpdate");

                if (listView.SelectedItems[0].Group != null)
                {
                    updateIndex = listView.SelectedItems[0].Group.Items.IndexOf(listView.SelectedItems[0]);

                    appIndex = listView.Groups.IndexOf(listView.SelectedItems[0].Group);

                    sidebar.DisplayUpdateInfo(Program.Applications[appIndex].Updates[updateIndex].Title[0].Value, Program.Applications[appIndex].Updates[updateIndex].ReleaseDate,
                    Program.Applications[appIndex].Updates[updateIndex].Description[0].Value, Program.Applications[appIndex].Updates[updateIndex].InfoUrl, Program.Applications[appIndex].HelpUrl);

                    if (Program.Applications[appIndex].Name[0].Value == Program.RM.GetString("SevenUpdate") && Program.Applications[appIndex].Publisher[0].Value == Program.RM.GetString("SevenSoftware"))
                        cmsHideUpdate.Enabled = false;
                    else
                    {
                        cmsHideUpdate.Enabled = true;
                    }
                }
            }
            else
                sidebar.HideText();
        }

        #endregion

        #endregion

        #endregion
    }
}