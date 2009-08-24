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
using System.Windows.Forms;
using SevenUpdate.SDK.Properties;

namespace SevenUpdate.SDK
{
    public partial class Shortcuts : UserControl
    {
        #region Global Vars
       

        /// <summary>
        /// the index of the current shortcut selection
        /// </summary>
        int index;

        /// <summary>
        /// Gets the shortcuts of the update
        /// </summary>
        internal Collection<Shortcut> UpdateShortcuts { get { return shortcuts; } }

        /// <summary>
        /// Collection of shortcuts
        /// </summary>
        Collection<Shortcut> shortcuts { get; set; }

        #endregion

        public Shortcuts()
        {
            this.Font = System.Drawing.SystemFonts.MessageBoxFont;

            InitializeComponent();
        }

        #region Methods

        /// <summary>
        /// Adds a shortcut to the update
        /// </summary>
        private void AddShortcut()
        {
            ClearUI(false);

            tmiAddShortcut.Enabled = false;
          
            txtLoc.Focus();
        }

        /// <summary>
        /// Clears the UI
        /// </summary>
        /// <param name="clearCollection">Indicates if you want to clear the shortcuts</param>
        internal void ClearUI(bool clearShortcuts)
        {
            if (clearShortcuts)
                lbShortcuts.Items.Clear();

            txtArgs.Text = null;

            txtDescription.Text = null;

            txtIcon.Text = null;

            txtLoc.Text = null;

            txtTarget.Text = null;
        }

        /// <summary>
        /// Deletes the selected shortcut from the update
        /// </summary>
        private void DeleteShortcut()
        {
            int index = lbShortcuts.SelectedIndex;
            if (index < 0)
                return;
            if (shortcuts.Count > 0)
            {
                shortcuts.RemoveAt(index);
                lbShortcuts.Items.RemoveAt(index);
            }

            if (lbShortcuts.Items.Count > 0)
                lbShortcuts.SelectedIndex = lbShortcuts.Items.Count - 1;
            else
            {
                ClearUI(false);
            }

            tmiAddShortcut.Enabled = false;
        }

        /// <summary>
        /// Loads the shortcut in the UI
        /// </summary>
        /// <param name="x">Index of the Shortcut</param>
        void LoadShortcut(int x)
        {
            if (x < 0)
                return;
            ClearUI(false);

            txtArgs.Text = shortcuts[x].Arguments;

            txtDescription.Text = shortcuts[x].Description[0].Value;

            txtIcon.Text = shortcuts[x].Icon;

            txtLoc.Text = shortcuts[x].Location;

            txtTarget.Text = shortcuts[x].Target;
        }

        /// <summary>
        /// Loads the shortcuts into the UI
        /// </summary>
        /// <param name="shortcuts">The collection of shortcuts to load</param>
        internal void LoadShortcuts(Collection<Shortcut> shortcuts)
        {
            this.shortcuts= new Collection<Shortcut>();

            for (int x = 0; x < shortcuts.Count; x++)
            {
                this.shortcuts.Add(shortcuts[x]);
            }

            lbShortcuts.Items.Clear();

            if (this.shortcuts != null)
            {
                for (int x = 0; x < this.shortcuts.Count; x++)
                    lbShortcuts.Items.Add(Program.RM.GetString("Shortcut") +" " + (x +1));
                if (lbShortcuts.Items.Count > 0)
                {

                    lbShortcuts.SelectedIndex = 0;

                    index = lbShortcuts.SelectedIndex;

                    LoadShortcut(index);
                }
            }
        }

        /// <summary>
        /// Saves a shortcut to the collection
        /// </summary>
        void SaveShortcut()
        {
            if (shortcuts == null)
                shortcuts = new Collection<Shortcut>();

            Shortcut shortcut = new Shortcut();

            shortcut.Arguments = txtArgs.Text;
            Collection<LocaleString> desc = new Collection<LocaleString>();
            LocaleString ls = new LocaleString();
            ls.lang = "en";
            ls.Value = txtDescription.Text;
            desc.Add(ls);
            shortcut.Description = desc;

            shortcut.Icon = txtIcon.Text;

            shortcut.Location = txtLoc.Text;

            shortcut.Target = txtTarget.Text;


            if (index > -1 && shortcuts.Count > 0)
            {
                shortcuts.RemoveAt(index);

                shortcuts.Insert(index, shortcut);
                lbShortcuts.Items[index] = Program.RM.GetString("Shortcut") + " " + (lbShortcuts.SelectedIndex + 1);
                lbShortcuts.SelectedIndex = index;
            }
            else
            {
                shortcuts.Add(shortcut);
                lbShortcuts.Items.Add(Program.RM.GetString("Shortcut") + " " + (lbShortcuts.Items.Count + 1));
                lbShortcuts.SelectedIndex = lbShortcuts.Items.Count - 1;

            }
            tmiAddShortcut.Enabled = true;
            tmiDeleteShortcut.Enabled = true;;

        }

        #endregion

        #region UI Events

        #region Buttons

        void btnSave_Click(object sender, EventArgs e)
        {
            if (txtLoc.Text.Length > 0 && txtTarget.Text.Length > 0)
            {
                if (!txtLoc.Text.ToLower().EndsWith(".lnk"))
                    MessageBox.Show(Program.RM.GetString("ShortcutNotValid"));
                else
                    SaveShortcut();
            }
            else
                MessageBox.Show(Program.RM.GetString("FillRequiredInformation"));
        }

        #endregion

        #region Context Menu

        private void tmiAddShortcut_Click(object sender, EventArgs e)
        {
            lbShortcuts.SelectedIndex = -1;
            AddShortcut();
        }

        private void tmiDeleteShortcut_Click(object sender, EventArgs e)
        {
            DeleteShortcut();
        }

        #endregion

        #region Labels

        void lblBrowseLoc_Click(object sender, EventArgs e)
        {
            dlgOpenFile.FileName = null;
            
            dlgOpenFile.FileName = Path.GetFileName(txtLoc.Text);
            
            dlgOpenFile.AddExtension = true;

            dlgOpenFile.DereferenceLinks = false;

            dlgOpenFile.Filter = "Shortcut files|*.lnk";

            if (dlgOpenFile.ShowDialog() == DialogResult.OK)
            {
                txtLoc.Text = Shared.ConvertPath(dlgOpenFile.FileName, false, Main.Is64Bit);

                txtLoc.Text = Shared.Replace(txtLoc.Text, AppInfo.ApplicationDirectory , "[AppDir]");
            }
        }

        void lblBrowseIcon_Click(object sender, EventArgs e)
        {
            dlgOpenFile.FileName = null;
            dlgOpenFile.Filter = null;
            dlgOpenFile.DereferenceLinks = true;
            string dir = Shared.ConvertPath(AppInfo.ApplicationDirectory, true, Main.Is64Bit) + @"\";

            if (!Directory.Exists(dir))
                dir = null;

            dlgOpenFile.FileName = dir + Path.GetFileName(txtIcon.Text);
            

            if (dlgOpenFile.ShowDialog() == DialogResult.OK)
            {
                txtIcon.Text = Shared.ConvertPath(dlgOpenFile.FileName, false, Main.Is64Bit);

                txtIcon.Text = Shared.Replace(txtIcon.Text, AppInfo.ApplicationDirectory, "[AppDir]");
            }
        }

        void lblBrowseTarget_Click(object sender, EventArgs e)
        {
            dlgOpenFile.FileName = null;
            dlgOpenFile.Filter = null;
            dlgOpenFile.DereferenceLinks = true;

            string dir = Shared.ConvertPath(AppInfo.ApplicationDirectory, true, Main.Is64Bit) + @"\";

            if (!Directory.Exists(dir))
                dir = null;


            dlgOpenFile.FileName = dir + Path.GetFileName(txtTarget.Text);
            

            dlgOpenFile.DereferenceLinks = true;

            if (dlgOpenFile.ShowDialog() == DialogResult.OK)
            {
                txtTarget.Text = Shared.ConvertPath(dlgOpenFile.FileName, false, Main.Is64Bit);

                txtTarget.Text = Shared.Replace(txtTarget.Text, AppInfo.ApplicationDirectory, "[AppDir]");

                txtIcon.Text = txtTarget.Text +",0";
            }

        }

        void lblValidateIcon_Click(object sender, EventArgs e)
        {
            txtIcon.Text = Shared.ConvertPath(txtIcon.Text, false, Main.Is64Bit);

            txtIcon.Text = Shared.Replace(txtIcon.Text, AppInfo.ApplicationDirectory, "[AppDir]");
        }

        void lblValidateShortcutLoc_Click(object sender, EventArgs e)
        {
            txtLoc.Text = Shared.ConvertPath(txtLoc.Text, false, Main.Is64Bit);

            txtLoc.Text = Shared.Replace(txtLoc.Text, AppInfo.ApplicationDirectory, "[AppDir]");

            if (!txtLoc.Text.ToLower().EndsWith(".lnk"))
                MessageBox.Show(Program.RM.GetString("ShortcutNotValid"));
        }

        void lblValidateTarget_Click(object sender, EventArgs e)
        {
            txtTarget.Text = Shared.ConvertPath(txtTarget.Text, false, Main.Is64Bit);

            txtTarget.Text = Shared.Replace(txtTarget.Text, AppInfo.ApplicationDirectory, "[AppDir]");

        }

        void lbShortcuts_SelectedIndexChanged(object sender, EventArgs e)
        {
            index = lbShortcuts.SelectedIndex;

            if (index >= 0)
                LoadShortcut(index);
        }

        void Label_MouseEnter(object sender, System.EventArgs e)
        {
            Label label = ((Label)sender);

            label.ForeColor = Color.FromArgb(51, 153, 255);

            label.Font = new Font(label.Font, FontStyle.Underline);
        }

        void Label_MouseLeave(object sender, System.EventArgs e)
        {
            Label label = ((Label)sender);

            label.ForeColor = System.Drawing.Color.FromArgb(0, 102, 204);

            label.Font = new Font(label.Font, FontStyle.Regular);
        }

        #endregion

        #region lbShortcuts

        private void lbShortcuts_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                int index = lbShortcuts.IndexFromPoint(e.X, e.Y);
                if (index > -1)
                    lbShortcuts.SelectedIndex = index;

                if (lbShortcuts.SelectedIndex < 0)
                {
                    tmiAddShortcut.Enabled = false;
                    tmiDeleteShortcut.Enabled = false;
                }
                else
                {
                    tmiAddShortcut.Enabled = true;
                    tmiDeleteShortcut.Enabled = true;
                }
                cmsMenu.Show(Control.MousePosition);
            }

        }

        private void lbShortcuts_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Delete)
                DeleteShortcut();
        }

        #endregion

        #endregion

    }
}
