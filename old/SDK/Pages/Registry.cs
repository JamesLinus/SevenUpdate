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
using SevenUpdate.SDK.Properties;

namespace SevenUpdate.SDK
{
    public partial class Registry : UserControl
    {
        #region Global Vars     
        
        /// <summary>
        /// The current index of the registry item
        /// </summary>
        int index;

        /// <summary>
        /// Gets the registry items of the update
        /// </summary>
        internal Collection<RegistryItem> UpdateRegistry { get { return registry; } }

        /// <summary>
        /// Collection of registry items
        /// </summary>
        Collection<RegistryItem> registry { get; set; }

        #endregion

        public Registry()
        {
            this.Font = System.Drawing.SystemFonts.MessageBoxFont;
            InitializeComponent();
        }

        #region Methods

        /// <summary>
        /// Adds a registry item to the update
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
        /// <param name="clearRegistryItems">Indicates if to clear the registry items</param>
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
        /// Deletes a registry item
        /// </summary>
        private void DeleteRegItem()
        {
            int index = lbRegistry.SelectedIndex;
            if (index < 0)
                return;

            if (registry.Count > 0)
            {
                registry.RemoveAt(index);

                lbRegistry.Items.RemoveAt(index);
            }

            if (lbRegistry.Items.Count > 0)
                lbRegistry.SelectedIndex = lbRegistry.Items.Count - 1;
            else
            {
                ClearUI(false);
            }

            tmiAddRegItem.Enabled = true;

        }

        /// <summary>
        /// Loads the registry item
        /// </summary>
        /// <param name="x">The index of the current registry item</param>
        void LoadReg(int x)
        {
            if (x < 0)
                return;
            ClearUI(false);

            txtKeyPath.Text = registry[x].Key;

            txtValueData.Text = registry[x].Data;

            txtValueName.Text = registry[x].KeyValue;

            switch (registry[x].Action)
            {
                case RegistryAction.Add: rbAddUpd.Checked = true; break;

                case RegistryAction.DeleteKey :rbDelKey.Checked = true; break;

                case RegistryAction.DeleteValue:rbDelValue.Checked = true; break;
            }

            switch (registry[x].Hive)
            {
                case Microsoft.Win32.RegistryHive.ClassesRoot: cbHives.SelectedItem = "CLASSES_ROOT"; break;

                case Microsoft.Win32.RegistryHive.CurrentConfig: cbHives.SelectedItem = "CURRENT_CONFIG"; break;

                case Microsoft.Win32.RegistryHive.CurrentUser: cbHives.SelectedItem = "CURRENT_USER"; break;

                case Microsoft.Win32.RegistryHive.LocalMachine: cbHives.SelectedItem = "LOCAL_MACHINE"; break;

                case Microsoft.Win32.RegistryHive.Users: cbHives.SelectedItem = "USERS"; break;

            }

            cbDataType.SelectedItem = registry[x].ValueKind;
        }

        /// <summary>
        /// Loads the registry items in the UI
        /// </summary>
        internal void LoadRegistryItems(Collection<RegistryItem> registry)
        {
            this.registry = new Collection<RegistryItem>();

            for (int x = 0; x < registry.Count; x++)
            {
                this.registry.Add(registry[x]);
            }

            lbRegistry.Items.Clear();

            if (this.registry != null)
            {
                for (int x = 0; x < this.registry.Count; x++)
                    lbRegistry.Items.Add(Program.RM.GetString("RegistryItem") + " " + ( x +1));

                if (lbRegistry.Items.Count > 0)
                {
                    lbRegistry.SelectedIndex = 0;

                    index = lbRegistry.SelectedIndex;

                    LoadReg(index);
                }
            }
        }

        /// <summary>
        /// Saves an registry item
        /// </summary>
        void SaveReg()
        {
            if (registry == null)
                registry = new Collection<RegistryItem>();

            RegistryItem reg = new RegistryItem();

            if (rbAddUpd.Checked)
                reg.Action = RegistryAction.Add;

            if (rbDelKey.Checked)
                reg.Action = RegistryAction.DeleteKey;

            if (rbDelValue.Checked)
                reg.Action = RegistryAction.DeleteValue;

            switch (cbHives.SelectedItem.ToString())
            {
                case "CLASSES_ROOT": reg.Hive = Microsoft.Win32.RegistryHive.ClassesRoot; break;

                case "CURRENT_CONFIG": reg.Hive = Microsoft.Win32.RegistryHive.CurrentConfig; break;

                case "CURRENT_USER": reg.Hive = Microsoft.Win32.RegistryHive.CurrentUser; break;

                case "LOCAL_MACHINE": reg.Hive = Microsoft.Win32.RegistryHive.LocalMachine; break;
                
                case "USERS": reg.Hive = Microsoft.Win32.RegistryHive.Users; break;
            }

            reg.Key = txtKeyPath.Text;

            reg.KeyValue = txtValueName.Text;

            reg.Data = txtValueData.Text;

           reg.ValueKind = (Microsoft.Win32.RegistryValueKind)Enum.Parse(typeof(Microsoft.Win32.RegistryValueKind), cbDataType.SelectedItem.ToString());

           if (index > -1 && registry.Count > 0)
           {
               registry.RemoveAt(index);

               registry.Insert(index, reg);
               lbRegistry.Items[index] = Program.RM.GetString("RegistryItem") + " " + (lbRegistry.SelectedIndex + 1);
               lbRegistry.SelectedIndex = index;
           }
           else
           {
               registry.Add(reg);
               lbRegistry.Items.Add(Program.RM.GetString("RegistryItem") + " " + (lbRegistry.Items.Count +1));
               lbRegistry.SelectedIndex = lbRegistry.Items.Count - 1;

           }
            

            tmiAddRegItem.Enabled = true;
            tmiDeleteRegItem.Enabled = true;
        }

        #endregion
        
        #region UI Events

        #region Buttons

        void btnSave_Click(object sender, System.EventArgs e)
        {
            if (txtKeyPath.Text.Length > 0)
                SaveReg();
            else
                MessageBox.Show(Program.RM.GetString("FillRequiredInformation"));
        }

        #endregion

        #region Context Menu

        private void tmiAddRegItem_Click(object sender, EventArgs e)
        {
            lbRegistry.SelectedIndex = -1;
            AddRegItem();
        }

        private void tmiDeleteRegItem_Click(object sender, EventArgs e)
        {
            DeleteRegItem();
        }

        #endregion

        #region Labels
        
        void lbRegistry_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            index = lbRegistry.SelectedIndex;

            if (index >= 0)
                LoadReg(index);
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

             label.ForeColor = Color.FromArgb(0, 102, 204);

             label.Font = new Font(label.Font, FontStyle.Regular);
         }

        #endregion

        #region lbRegistry

        private void lbRegistry_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                int index = lbRegistry.IndexFromPoint(e.X, e.Y);
                if (index > -1)
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

                cmsMenu.Show(Control.MousePosition);
            }

        }

        private void lbRegistry_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Delete)
                DeleteRegItem();
        }

        #endregion



        #endregion
    }
}
