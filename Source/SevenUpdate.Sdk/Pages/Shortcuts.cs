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
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using SevenUpdate.Base;
using SevenUpdate.Sdk.WinForms;
using Shortcut = SevenUpdate.Base.Shortcut;

#endregion

namespace SevenUpdate.Sdk.Pages
{
    public sealed partial class Shortcuts : UserControl
    {
        #region Global Vars

        /// <summary>
        /// the Index of the current shortcut selection
        /// </summary>
        private int index;

        /// <summary>
        /// Gets the UpdateShortcuts of the update
        /// </summary>
        internal ObservableCollection<Shortcut> UpdateShortcuts { get; private set; }

        #endregion

        public Shortcuts()
        {
            Font = SystemFonts.MessageBoxFont;

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
        /// <param name="clearShortcuts">If True the UpdateShortcuts and list will be cleared out</param>
        internal void ClearUI(bool clearShortcuts)
        {
            if (clearShortcuts)
                lbShortcuts.Items.Clear();

            txtArgs.Text = null;

            txtDescription.Text = null;

            txtIcon.Text = null;

            txtLoc.Text = null;

            txtTarget.Text = null;

            cbShortcutAction.SelectedIndex = 0;
        }

        /// <summary>
        /// Deletes the selected shortcut from the update
        /// </summary>
        private void DeleteShortcut()
        {
            var x = lbShortcuts.SelectedIndex;
            if (x < 0)
                return;
            if (UpdateShortcuts.Count > 0)
            {
                UpdateShortcuts.RemoveAt(x);
                lbShortcuts.Items.RemoveAt(x);
            }

            if (lbShortcuts.Items.Count > 0)
                lbShortcuts.SelectedIndex = lbShortcuts.Items.Count - 1;
            else
                ClearUI(false);

            tmiAddShortcut.Enabled = false;
        }

        /// <summary>
        /// Loads the shortcut in the UI
        /// </summary>
        /// <param name="x">Index of the Shortcut</param>
        private void LoadShortcut(int x)
        {
            if (x < 0)
                return;
            ClearUI(false);

            txtArgs.Text = UpdateShortcuts[x].Arguments;

            txtDescription.Text = UpdateShortcuts[x].Description[0].Value;

            txtIcon.Text = UpdateShortcuts[x].Icon;

            txtLoc.Text = UpdateShortcuts[x].Location;

            txtTarget.Text = UpdateShortcuts[x].Target;

            switch (UpdateShortcuts[x].Action)
            {
                case ShortcutAction.Add:
                    cbShortcutAction.SelectedIndex = 0;
                    break;
                case ShortcutAction.Update:
                    cbShortcutAction.SelectedIndex = 1;
                    break;
                case ShortcutAction.Delete:
                    cbShortcutAction.SelectedIndex = 2;
                    break;
            }
        }

        /// <summary>
        /// Loads the shortcutItems into the UI
        /// </summary>
        /// <param name="shortcutItems">The collection of UpdateShortcuts to load</param>
        internal void LoadShortcuts(ObservableCollection<Shortcut> shortcutItems)
        {
            if (shortcutItems == null)
                return;

            UpdateShortcuts = new ObservableCollection<Shortcut>();

            for (var x = 0; x < shortcutItems.Count; x++)
                UpdateShortcuts.Add(shortcutItems[x]);

            lbShortcuts.Items.Clear();

            for (var x = 0; x < UpdateShortcuts.Count; x++)
                lbShortcuts.Items.Add(Program.RM.GetString("Shortcut") + " " + (x + 1));
            if (lbShortcuts.Items.Count <= 0)
                return;
            lbShortcuts.SelectedIndex = 0;

            index = lbShortcuts.SelectedIndex;

            LoadShortcut(index);
        }

        /// <summary>
        /// Saves a shortcut to the collection
        /// </summary>
        private void SaveShortcut()
        {
            if (UpdateShortcuts == null)
                UpdateShortcuts = new ObservableCollection<Shortcut>();

            var shortcut = new Shortcut {Arguments = txtArgs.Text};

            var desc = new ObservableCollection<LocaleString>();
            var ls = new LocaleString {Lang = "en", Value = txtDescription.Text};
            desc.Add(ls);
            shortcut.Description = desc;

            shortcut.Icon = txtIcon.Text;

            shortcut.Location = txtLoc.Text;

            shortcut.Target = txtTarget.Text;

            switch (cbShortcutAction.SelectedIndex)
            {
                case 0:
                    shortcut.Action = ShortcutAction.Add;
                    break;
                case 1:
                    shortcut.Action = ShortcutAction.Update;
                    break;
                case 2:
                    shortcut.Action = ShortcutAction.Delete;
                    break;
            }


            if (index > -1 && UpdateShortcuts.Count > 0)
            {
                UpdateShortcuts.RemoveAt(index);

                UpdateShortcuts.Insert(index, shortcut);
                lbShortcuts.Items[index] = Program.RM.GetString("Shortcut") + " " + (lbShortcuts.SelectedIndex + 1);
                lbShortcuts.SelectedIndex = index;
            }
            else
            {
                UpdateShortcuts.Add(shortcut);
                lbShortcuts.Items.Add(Program.RM.GetString("Shortcut") + " " + (lbShortcuts.Items.Count + 1));
                lbShortcuts.SelectedIndex = lbShortcuts.Items.Count - 1;
            }
            tmiAddShortcut.Enabled = true;
            tmiDeleteShortcut.Enabled = true;
        }

        #endregion

        #region UI Events

        #region Buttons

        private void Save_Click(object sender, EventArgs e)
        {
            if (txtLoc.Text.Length > 0 && (txtTarget.Text.Length > 0 || cbShortcutAction.SelectedIndex == 2))
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

        private void AddShortcut_Click(object sender, EventArgs e)
        {
            lbShortcuts.SelectedIndex = -1;
            AddShortcut();
        }

        private void DeleteShortcut_Click(object sender, EventArgs e)
        {
            DeleteShortcut();
        }

        #endregion

        #region Labels

        private void BrowseLoc_Click(object sender, EventArgs e)
        {
            dlgOpenFile.FileName = null;

            dlgOpenFile.FileName = Path.GetFileName(txtLoc.Text);

            dlgOpenFile.AddExtension = true;

            dlgOpenFile.DereferenceLinks = false;

            dlgOpenFile.Filter = @"Shortcut files|*.lnk";

            if (dlgOpenFile.ShowDialog() != DialogResult.OK)
                return;
            txtLoc.Text = Base.Base.ConvertPath(dlgOpenFile.FileName, false, Main.Is64Bit);

            txtLoc.Text = Base.Base.Replace(txtLoc.Text, AppInfo.ApplicationDirectory, "[AppDir]");
        }

        private void BrowseIcon_Click(object sender, EventArgs e)
        {
            dlgOpenFile.FileName = null;
            dlgOpenFile.Filter = null;
            dlgOpenFile.DereferenceLinks = true;
            var dir = Base.Base.ConvertPath(AppInfo.ApplicationDirectory, true, Main.Is64Bit) + @"\";

            if (!Directory.Exists(dir))
                dir = null;

            dlgOpenFile.FileName = dir + Path.GetFileName(txtIcon.Text);


            if (dlgOpenFile.ShowDialog() != DialogResult.OK)
                return;
            txtIcon.Text = Base.Base.ConvertPath(dlgOpenFile.FileName, false, Main.Is64Bit);

            txtIcon.Text = Base.Base.Replace(txtIcon.Text, AppInfo.ApplicationDirectory, "[AppDir]");
        }

        private void BrowseTarget_Click(object sender, EventArgs e)
        {
            dlgOpenFile.FileName = null;
            dlgOpenFile.Filter = null;
            dlgOpenFile.DereferenceLinks = true;

            var dir = Base.Base.ConvertPath(AppInfo.ApplicationDirectory, true, Main.Is64Bit) + @"\";

            if (!Directory.Exists(dir))
                dir = null;


            dlgOpenFile.FileName = dir + Path.GetFileName(txtTarget.Text);


            dlgOpenFile.DereferenceLinks = true;

            if (dlgOpenFile.ShowDialog() != DialogResult.OK)
                return;
            txtTarget.Text = Base.Base.ConvertPath(dlgOpenFile.FileName, false, Main.Is64Bit);

            txtTarget.Text = Base.Base.Replace(txtTarget.Text, AppInfo.ApplicationDirectory, "[AppDir]");

            txtIcon.Text = txtTarget.Text + @",0";
        }

        private void ValidateIcon_Click(object sender, EventArgs e)
        {
            txtIcon.Text = Base.Base.ConvertPath(txtIcon.Text, false, Main.Is64Bit);

            txtIcon.Text = Base.Base.Replace(txtIcon.Text, AppInfo.ApplicationDirectory, "[AppDir]");
        }

        private void ValidateShortcutLoc_Click(object sender, EventArgs e)
        {
            txtLoc.Text = Base.Base.ConvertPath(txtLoc.Text, false, Main.Is64Bit);

            txtLoc.Text = Base.Base.Replace(txtLoc.Text, AppInfo.ApplicationDirectory, "[AppDir]");

            if (!txtLoc.Text.ToLower().EndsWith(".lnk"))
                MessageBox.Show(Program.RM.GetString("ShortcutNotValid"));
        }

        private void ValidateTarget_Click(object sender, EventArgs e)
        {
            txtTarget.Text = Base.Base.ConvertPath(txtTarget.Text, false, Main.Is64Bit);

            txtTarget.Text = Base.Base.Replace(txtTarget.Text, AppInfo.ApplicationDirectory, "[AppDir]");
        }

        private void Shortcuts_SelectedIndexChanged(object sender, EventArgs e)
        {
            index = lbShortcuts.SelectedIndex;

            if (index >= 0)
                LoadShortcut(index);
        }

        private void Label_MouseEnter(object sender, EventArgs e)
        {
            var label = ((Label) sender);

            label.ForeColor = Color.FromArgb(51, 153, 255);

            label.Font = new Font(label.Font, FontStyle.Underline);
        }

        private void Label_MouseLeave(object sender, EventArgs e)
        {
            var label = ((Label) sender);

            label.ForeColor = Color.FromArgb(0, 102, 204);

            label.Font = new Font(label.Font, FontStyle.Regular);
        }

        #endregion

        #region lbShortcuts

        private void Shortcuts_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right)
                return;
            var x = lbShortcuts.IndexFromPoint(e.X, e.Y);
            if (x > -1)
                lbShortcuts.SelectedIndex = x;

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
            cmsMenu.Show(MousePosition);
        }

        private void Shortcuts_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Delete)
                DeleteShortcut();
        }

        #endregion

        private void ShortcutAction_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbShortcutAction.SelectedIndex == 2)
            {
                txtTarget.Enabled = false;
                txtArgs.Enabled = false;
                txtIcon.Enabled = false;
                txtDescription.Enabled = false;

                txtTarget.Text = null;
                txtArgs.Text = null;
                txtIcon.Text = null;
                txtDescription.Text = null;
            }
            else
            {
                txtTarget.Enabled = true;
                txtArgs.Enabled = true;
                txtIcon.Enabled = true;
                txtDescription.Enabled = true;
            }
        }

        #endregion
    }
}