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
using System.IO;
using System.Net;
using System.Windows.Forms;
using SevenUpdate.Properties;
using SevenUpdate.WCF;
namespace SevenUpdate.Pages
{
    public partial class Options : UserControl
    {
        #region Global Vars

        /// <summary>
        /// Shows application description in a tooltop for the application list
        /// </summary>
        ToolTip ttDescription = new ToolTip();

        /// <summary>
        /// The list of Seven Update Applications
        /// </summary>
        Collection<SUA> officalAppList;

        /// <summary>
        /// Sorts colums of a listView
        /// </summary>
        ListViewColumnSorter lvwColumnSorter = new ListViewColumnSorter();

        /// <summary>
        /// The Seven Update list location
        /// </summary>
        static Uri suaLoc = new Uri("http://ittakestime.org/su/SUlist.sul");

        #endregion

        public Options()
        {
            this.Font = SystemFonts.MessageBoxFont;

            InitializeComponent();

            lblHeading.Font = new Font(SystemFonts.MessageBoxFont.Name, 11.25F);

            if (!Program.IsAdmin())
                Program.AddShieldToButton(btnSave);
        }

        #region Event Declarations

        internal static event EventHandler<SettingsSavedEventArgs> SettingsSavedEventHandler;

        #region EventArgs

        internal class SettingsSavedEventArgs : EventArgs
        {
            internal SettingsSavedEventArgs(UpdateOptions settings )
            {
                this.UpdateSettings = settings;
            }

            /// <summary>
            /// The current settings for Seven Update
            /// </summary>
            internal UpdateOptions UpdateSettings { get; set; }
        }

        #endregion

        #endregion

        #region Methods

        /// <summary>
        /// Downloads the Seven Update Application List
        /// </summary>
        void DownloadSUL()
        {
            lblLoading.Visible = true;

            this.Cursor = Cursors.WaitCursor;

            listView.Cursor = Cursors.WaitCursor;

            File.Delete(Shared.userStore + @"list.sul");

            WebClient wc = new WebClient();

            wc.DownloadFileCompleted += new System.ComponentModel.AsyncCompletedEventHandler(wc_DownloadFileCompleted);

            wc.DownloadFileAsync(suaLoc, Shared.userStore + @"list.sul");
        }

        /// <summary>
        /// Loads the configuration and sets the UI
        /// </summary>
        void LoadSettings()
        {
            switch (UpdateSettings.Settings.AutoOption)
            {
                case AutoUpdateOption.Install: cbAutoUpdateMethod.SelectedIndex = 0; break;

                case AutoUpdateOption.Download: cbAutoUpdateMethod.SelectedIndex = 1; break;

                case AutoUpdateOption.Notify: cbAutoUpdateMethod.SelectedIndex = 2; break;

                case AutoUpdateOption.Never: cbAutoUpdateMethod.SelectedIndex = 3; break;
            }

            chkRecommendedUpdates.Checked = UpdateSettings.Settings.Recommended;
        }

        /// <summary>
        /// Loads the list of Seven Update applications and sets the ui
        /// </summary>
        void LoadSUL()
        {
            if (!File.Exists(Shared.userStore + @"list.sul"))
                DownloadSUL();
            else
            {
                officalAppList = Shared.DeserializeCollection<SUA>(Shared.userStore + @"list.sul");

                ListViewItem item;

                listView.Items.Clear();

                int index = 0;

                for (int x = 0; x < officalAppList.Count; x++)
                {
                    item = new ListViewItem();

                    item.ToolTipText = officalAppList[x].Description[0].Value;

                    item.SubItems.Add(officalAppList[x].ApplicationName[0].Value);

                    item.SubItems.Add(officalAppList[x].Publisher[0].Value);

                    if (Directory.Exists(Shared.ConvertPath(officalAppList[x].Directory, true, officalAppList[x].Is64Bit)))
                        item.SubItems.Add("Yes");
                    else
                    {
                        item.SubItems.Add("No");

                        item.Checked = false;

                        item.ForeColor = SystemColors.GrayText;
                    }
                    if (officalAppList[x].Is64Bit)
                        item.SubItems.Add("x64");
                    else
                        item.SubItems.Add("x86");

                    listView.Items.Add(item);
                }

                Collection<SUA> userAppList = Shared.DeserializeCollection<SUA>(Shared.appStore + "SUApps.sul");

                for (int x = 0; x < userAppList.Count; x++)
                {
                    index = -1;

                    for (int y = 0; y < officalAppList.Count; y++)
                    {
                        if (officalAppList[y].Directory == userAppList[x].Directory && officalAppList[y].Source == userAppList[x].Source)
                        {
                            index = y;
                            break;
                        }
                        else
                            index = -1;
                    }
                    if (index > -1)
                        listView.Items[index].Checked = true;
                    else
                    {
                        item = new ListViewItem();

                        item.ToolTipText = userAppList[x].Description[0].Value;

                        item.SubItems.Add(userAppList[x].ApplicationName[0].Value);

                        item.SubItems.Add(userAppList[x].Publisher[0].Value);

                        if (Directory.Exists(Shared.ConvertPath(userAppList[x].Directory, true, userAppList[x].Is64Bit)))
                        {
                            item.Checked = true;

                            item.SubItems.Add("Yes");
                        }
                        else
                        {
                            item.SubItems.Add("No");

                            item.Checked = false;

                            item.ForeColor = SystemColors.GrayText;

                            item.ToolTipText = userAppList[x].Description[0].Value;
                        }
                        if (officalAppList[x].Is64Bit)
                            item.SubItems.Add("x64");
                        else
                            item.SubItems.Add("x86");

                        officalAppList.Add(userAppList[x]);

                        listView.Items.Add(item);

                    }
                }
            }
        }

        /// <summary>
        /// Saves the Settings
        /// </summary>
        void SaveSettings()
        {
            UpdateOptions options = new UpdateOptions();

            if (cbAutoUpdateMethod.SelectedIndex == 0)
                options.AutoOption = AutoUpdateOption.Install;

            if (cbAutoUpdateMethod.SelectedIndex == 1)
                options.AutoOption = AutoUpdateOption.Download;

            if (cbAutoUpdateMethod.SelectedIndex == 2)
                options.AutoOption = AutoUpdateOption.Notify;

            if (cbAutoUpdateMethod.SelectedIndex == 3)
                options.AutoOption = AutoUpdateOption.Never;

            options.Recommended = chkRecommendedUpdates.Checked;

            Collection<SUA> sul = new Collection<SUA>();
            if (listView.Items.Count > 0)
            {
                for (int x = 0; x < listView.Items.Count; x++)
                {
                    if (listView.Items[x].Checked)
                    {
                        sul.Add(officalAppList[x]);
                    }
                }

                
            }

            if (cbAutoUpdateMethod.SelectedIndex == 3)
            {
                options.AutoOption = AutoUpdateOption.Never;

                Client.SaveSettings(false, options, sul);
            }
            else
                Client.SaveSettings(true, options, sul);

            if (SettingsSavedEventHandler != null)
                SettingsSavedEventHandler(this, new SettingsSavedEventArgs(options));
        }

        /// <summary>
        /// Wraps text in a label
        /// </summary>
        /// <param name="text">The string to wrap</param>
        /// <param name="maxLength">Maximum length before wrap</param>
        /// <returns>Returns wrapped string</returns>
        static string Wrap(string text, int maxLength)
        {
            string sWrappedText = "";

            string sLine = "";

            while (text.Length > 0)
            {
                if (text.Length >= maxLength)
                {
                    sLine = text.Substring(0, maxLength);

                    if (sLine.EndsWith(" ") == false)
                        sLine = sLine.Substring(0, 1 + sLine.LastIndexOf(" "));

                    sWrappedText += sLine + Environment.NewLine;
                }
                else
                {
                    sLine = text;

                    sWrappedText += sLine;
                }

                text = text.Substring(sLine.Length);
            }
            return sWrappedText;
        }

        #endregion

        #region SUL list Methods

        void wc_DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            Settings.Default.lastListUpdate = DateTime.Now.ToShortDateString() + " " + Program.RM.GetString("At") + " " + DateTime.Now.ToShortTimeString();

            Settings.Default.Save();

            lblLastUpdated.Text = Program.RM.GetString("LastUpdated") + " " + Settings.Default.lastListUpdate;

            lblLoading.Visible = false;

            LoadSUL();

            this.Cursor = Cursors.Default;

            listView.Cursor = Cursors.Default;
        }

        #endregion

        #region UI Events

        #region Button

        void btnCancel_Click(object sender, EventArgs e)
        {
            Dispose();
        }

        void btnSave_Click(object sender, EventArgs e)
        {
            SaveSettings();
            Dispose();
        }

        #endregion

        #region Check/Combo Boxes

        void cbAutoUpdateMethod_SelectedIndexChanged(object sender, EventArgs e)
        {

            switch (cbAutoUpdateMethod.SelectedIndex)
            {
                case 0: pbShield.Image = Resources.smallGreenShield; break;
                case 1:
                case 2: pbShield.Image = null; break;
                case 3: pbShield.Image = Resources.smallRedShield; break;


            }
        }

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

        #region ListView

        void listView_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            // Determine if clicked column is already the column that is being sorted.
            if (e.Column == lvwColumnSorter.SortColumn)
            {
                // Reverse the current sort direction for this column.
                if (lvwColumnSorter.Order == SortOrder.Ascending)
                {
                    lvwColumnSorter.Order = SortOrder.Descending;
                }
                else
                {
                    lvwColumnSorter.Order = SortOrder.Ascending;
                }
            }
            else
            {
                // Set the column number that is to be sorted; default to ascending.
                lvwColumnSorter.SortColumn = e.Column;
                lvwColumnSorter.Order = SortOrder.Ascending;
            }

            // Perform the sort with these new sort options.
            this.listView.Sort();
        }

        void listView_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            chkSelectAll.Enabled = true;
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

        void listView_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (listView.Items[e.Index].ForeColor != SystemColors.WindowText)
                e.NewValue = CheckState.Unchecked;
        }
        
        void listView_MouseClick(object sender, MouseEventArgs e)
        {
            if (listView.SelectedItems != null && e.Button == MouseButtons.Right)
            {
                ttDescription.Show(Wrap(listView.SelectedItems[0].ToolTipText, 80), listView, listView.PointToClient(Control.MousePosition), 5000);
            }
        }

        #endregion

        #region Label

        void label_MouseEnter(object sender, System.EventArgs e)
        {
            Label label = ((Label)sender);
            label.Font = new Font(label.Font, FontStyle.Underline);
        }

        void label_MouseLeave(object sender, System.EventArgs e)
        {
            Label label = ((Label)sender);
            label.Font = new Font(label.Font, FontStyle.Regular);
        }      
        
        void lblRefresh_Click(object sender, EventArgs e)
        {
            DownloadSUL();
        }

        #endregion

        #region Form


        void Options_Load(object sender, EventArgs e)
        {
            LoadSettings();
            lblLastUpdated.Text = Program.RM.GetString("LastUpdated") +" " + Settings.Default.lastListUpdate;
            LoadSUL();
        }

        #endregion


        #endregion

    }
}
