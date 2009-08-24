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
using System.Windows.Forms;
using SevenUpdate.SDK.Properties;

namespace SevenUpdate.SDK
{
    public partial class UpdateMenu : UserControl
    {
        #region Global Vars

        /// <summary>
        /// Index of the current update
        /// </summary>
        internal int index = -1;

        #endregion

        public UpdateMenu()
        {
            this.Font = System.Drawing.SystemFonts.MessageBoxFont;

            InitializeComponent();
        }

        #region Methods

        /// <summary>
        /// Updates the UI with the updates created.
        /// </summary>
        internal void UpdateUI()
        {
            lbUpdates.Items.Clear();

            if (SDK.SUSDK.application.Updates != null)
            {
                if (SUSDK.application.Updates.Count > 0)
                {
                    lbUpdates.Visible = true;

                    label1.Visible = true;

                    clEditUpdate.Enabled = true;

                    for (int x = 0; x < SDK.SUSDK.application.Updates.Count; x++)
                    {
                        if (SUSDK.application.Updates[x].Title[0].Value != null)
                            lbUpdates.Items.Add(SUSDK.application.Updates[x].Title[0].Value);
                        else
                        {
                            lbUpdates.Items.Add(Program.RM.GetString("Update"));
                        }
                    }

                    lbUpdates.SelectedIndex = 0;
                }
                else
                {
                    removeUpdateToolStripMenuItem.Enabled = false;

                    clEditUpdate.Enabled = false;
                }
            }
            else
            {
                removeUpdateToolStripMenuItem.Enabled = false;

                lbUpdates.Visible = false;

                label1.Visible = false;

                clEditUpdate.Enabled = false;
            }
        }

        #endregion

        #region UI Events

        void lbUpdates_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            index = lbUpdates.SelectedIndex;

            if (index >= 0)
                removeUpdateToolStripMenuItem.Enabled = true;
            else
                removeUpdateToolStripMenuItem.Enabled = false;
        }

        void removeUpdateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SUSDK.RemoveUpdate(lbUpdates.SelectedIndex);

            UpdateUI();

            if (index < 0)
            {
                removeUpdateToolStripMenuItem.Enabled = false;
                clEditUpdate.Enabled = false;
            }
        }
        #endregion
    }
}
