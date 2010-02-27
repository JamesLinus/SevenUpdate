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
using System.Windows.Forms;
using Microsoft.Win32;
using SevenUpdate.Base;

#endregion

namespace SevenUpdate.Sdk.Pages
{
    public sealed partial class Registry : UserControl
    {
        #region Global Vars 

        /// <summary>
        /// The current Index of the UpdateRegistry item
        /// </summary>
        private int index;

        /// <summary>
        /// Gets the UpdateRegistry items of the update
        /// </summary>
        internal ObservableCollection<RegistryItem> UpdateRegistry { get; private set; }

        #endregion

        public Registry()
        {
            Font = SystemFonts.MessageBoxFont;
            InitializeComponent();
        }

        #region Methods

        /// <summary>
        /// Adds a UpdateRegistry item to the update
        /// </summary>
        private void AddRegItem()
        {
            ClearUI(false);

            tmiAddRegItem.Enabled = false;

            txtKeyPath.Focus();

            cbHives.SelectedIndex = 0;

            cbDataType.SelectedIndex = 0;

            cbHives.Focus();
        }

        /// <summary>
        /// Clears the UI
        /// </summary>
        /// <param name="clearRegistryItems">Indicates if to clear the UpdateRegistry items</param>
        internal void ClearUI(bool clearRegistryItems)
        {
            if (clearRegistryItems)
                lbRegistry.Items.Clear();

            txtKeyPath.Text = null;

            txtValueData.Text = null;

            txtValueName.Text = null;

            cbDataType.SelectedIndex = 0;

            cbHives.SelectedIndex = 0;

            rbAddUpd.Checked = true;

            rbDelKey.Checked = false;

            rbDelValue.Checked = false;
        }

        /// <summary>
        /// Deletes a UpdateRegistry item
        /// </summary>
        private void DeleteRegItem()
        {
            var x = lbRegistry.SelectedIndex;
            if (x < 0)
                return;

            if (UpdateRegistry.Count > 0)
            {
                UpdateRegistry.RemoveAt(index);

                lbRegistry.Items.RemoveAt(index);
            }

            if (lbRegistry.Items.Count > 0)
                lbRegistry.SelectedIndex = lbRegistry.Items.Count - 1;
            else
                ClearUI(false);

            tmiAddRegItem.Enabled = true;
        }

        /// <summary>
        /// Loads the UpdateRegistry item
        /// </summary>
        /// <param name="x">The Index of the current UpdateRegistry item</param>
        private void LoadReg(int x)
        {
            if (x < 0)
                return;
            ClearUI(false);

            txtKeyPath.Text = UpdateRegistry[x].Key;

            txtValueData.Text = UpdateRegistry[x].Data;

            txtValueName.Text = UpdateRegistry[x].KeyValue;

            switch (UpdateRegistry[x].Action)
            {
                case RegistryAction.Add:
                    rbAddUpd.Checked = true;
                    break;

                case RegistryAction.DeleteKey:
                    rbDelKey.Checked = true;
                    break;

                case RegistryAction.DeleteValue:
                    rbDelValue.Checked = true;
                    break;
            }

            switch (UpdateRegistry[x].Hive)
            {
                case RegistryHive.ClassesRoot:
                    cbHives.SelectedItem = "CLASSES_ROOT";
                    break;

                case RegistryHive.CurrentConfig:
                    cbHives.SelectedItem = "CURRENT_CONFIG";
                    break;

                case RegistryHive.CurrentUser:
                    cbHives.SelectedItem = "CURRENT_USER";
                    break;

                case RegistryHive.LocalMachine:
                    cbHives.SelectedItem = "LOCAL_MACHINE";
                    break;

                case RegistryHive.Users:
                    cbHives.SelectedItem = "USERS";
                    break;
            }

            cbDataType.SelectedItem = UpdateRegistry[x].ValueKind;
        }

        /// <summary>
        /// Loads the UpdateRegistry items in the UI
        /// </summary>
        internal void LoadRegistryItems(ObservableCollection<RegistryItem> updateRegistryItems)
        {
            if (updateRegistryItems == null)
                return;
            UpdateRegistry = new ObservableCollection<RegistryItem>();

            for (var x = 0; x < updateRegistryItems.Count; x++)
                UpdateRegistry.Add(updateRegistryItems[x]);

            lbRegistry.Items.Clear();

            for (var x = 0; x < UpdateRegistry.Count; x++)
            {
                if (lbRegistry != null)
                    lbRegistry.Items.Add(Program.RM.GetString("RegistryItem") + " " + (x + 1));
            }

            if (lbRegistry != null)
            {
                if (lbRegistry.Items.Count <= 0)
                    return;
                lbRegistry.SelectedIndex = 0;

                index = lbRegistry.SelectedIndex;
            }

            LoadReg(index);
        }

        /// <summary>
        /// Saves an UpdateRegistry item
        /// </summary>
        private void SaveReg()
        {
            if (UpdateRegistry == null)
                UpdateRegistry = new ObservableCollection<RegistryItem>();

            var reg = new RegistryItem();

            if (rbAddUpd.Checked)
                reg.Action = RegistryAction.Add;

            if (rbDelKey.Checked)
                reg.Action = RegistryAction.DeleteKey;

            if (rbDelValue.Checked)
                reg.Action = RegistryAction.DeleteValue;

            switch (cbHives.SelectedItem.ToString())
            {
                case "CLASSES_ROOT":
                    reg.Hive = RegistryHive.ClassesRoot;
                    break;

                case "CURRENT_CONFIG":
                    reg.Hive = RegistryHive.CurrentConfig;
                    break;

                case "CURRENT_USER":
                    reg.Hive = RegistryHive.CurrentUser;
                    break;

                case "LOCAL_MACHINE":
                    reg.Hive = RegistryHive.LocalMachine;
                    break;

                case "USERS":
                    reg.Hive = RegistryHive.Users;
                    break;
            }

            reg.Key = txtKeyPath.Text;

            reg.KeyValue = txtValueName.Text;

            reg.Data = txtValueData.Text;

            reg.ValueKind = (RegistryValueKind) Enum.Parse(typeof (RegistryValueKind), cbDataType.SelectedItem.ToString());

            if (index > -1 && UpdateRegistry.Count > 0)
            {
                UpdateRegistry.RemoveAt(index);

                UpdateRegistry.Insert(index, reg);
                lbRegistry.Items[index] = Program.RM.GetString("RegistryItem") + " " + (lbRegistry.SelectedIndex + 1);
                lbRegistry.SelectedIndex = index;
            }
            else
            {
                UpdateRegistry.Add(reg);
                lbRegistry.Items.Add(Program.RM.GetString("RegistryItem") + " " + (lbRegistry.Items.Count + 1));
                lbRegistry.SelectedIndex = lbRegistry.Items.Count - 1;
            }


            tmiAddRegItem.Enabled = true;
            tmiDeleteRegItem.Enabled = true;
        }

        #endregion

        #region UI Events

        #region Buttons

        private void Save_Click(object sender, EventArgs e)
        {
            if (txtKeyPath.Text.Length > 0)
                SaveReg();
            else
                MessageBox.Show(Program.RM.GetString("FillRequiredInformation"));
        }

        #endregion

        #region Context Menu

        private void AddRegItem_Click(object sender, EventArgs e)
        {
            lbRegistry.SelectedIndex = -1;
            AddRegItem();
        }

        private void DeleteRegItem_Click(object sender, EventArgs e)
        {
            DeleteRegItem();
        }

        #endregion

        #region Labels

        private void Registry_SelectedIndexChanged(object sender, EventArgs e)
        {
            index = lbRegistry.SelectedIndex;

            if (index >= 0)
                LoadReg(index);
        }

        #endregion

        #region lbRegistry

        private void Registry_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right)
                return;
            var x = lbRegistry.IndexFromPoint(e.X, e.Y);
            if (x > -1)
                lbRegistry.SelectedIndex = index;
            if (lbRegistry.SelectedIndex < 0)
            {
                tmiAddRegItem.Enabled = false;
                tmiDeleteRegItem.Enabled = false;
            }
            else
            {
                tmiAddRegItem.Enabled = true;
                tmiDeleteRegItem.Enabled = true;
            }

            cmsMenu.Show(MousePosition);
        }

        private void Registry_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Delete)
                DeleteRegItem();
        }

        #endregion

        #endregion
    }
}