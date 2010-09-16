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
using System.Diagnostics;

#endregion

namespace Microsoft.Windows.Dialogs.Controls
{
    /// <summary>
    ///   Creates the check button controls used by the Common File Dialog.
    /// </summary>
    public abstract class CommonFileDialogCheckBox : CommonFileDialogProminentControl
    {
        private bool isChecked;

        /// <summary>
        ///   Creates a new instance of this class.
        /// </summary>
        public CommonFileDialogCheckBox()
        {
        }

        /// <summary>
        ///   Creates a new instance of this class with the specified text.
        /// </summary>
        /// <param name = "text">The text to display for this control.</param>
        public CommonFileDialogCheckBox(string text) : base(text)
        {
        }

        /// <summary>
        ///   Creates a new instance of this class with the specified name and text.
        /// </summary>
        /// <param name = "name">The name of this control.</param>
        /// <param name = "text">The text to display for this control.</param>
        public CommonFileDialogCheckBox(string name, string text) : base(name, text)
        {
        }

        /// <summary>
        ///   Creates a new instance of this class with the specified text and check state.
        /// </summary>
        /// <param name = "text">The text to display for this control.</param>
        /// <param name = "isChecked">The check state of this control.</param>
        public CommonFileDialogCheckBox(string text, bool isChecked) : base(text)
        {
            this.isChecked = isChecked;
        }

        /// <summary>
        ///   Creates a new instance of this class with the specified name, text and check state.
        /// </summary>
        /// <param name = "name">The name of this control.</param>
        /// <param name = "text">The text to display for this control.</param>
        /// <param name = "isChecked">The check state of this control.</param>
        public CommonFileDialogCheckBox(string name, string text, bool isChecked) : base(name, text)
        {
            this.isChecked = isChecked;
        }

        /// <summary>
        ///   Gets or sets the state of the check box.
        /// </summary>
        public bool IsChecked
        {
            get { return isChecked; }
            set
            {
                // Check if property has changed
                if (isChecked == value)
                    return;
                isChecked = value;
                ApplyPropertyChange("IsChecked");
            }
        }

        /// <summary>
        ///   Occurs when the user changes the check state.
        /// </summary>
        public event EventHandler CheckedChanged = delegate { };

        internal void RaiseCheckedChangedEvent()
        {
            // Make sure that this control is enabled and has a specified delegate
            if (Enabled)
                CheckedChanged(this, EventArgs.Empty);
        }

        /// <summary>
        ///   Attach the CheckButton control to the dialog object.
        /// </summary>
        /// <param name = "dialog">the target dialog</param>
        internal override void Attach(IFileDialogCustomize dialog)
        {
            Debug.Assert(dialog != null, "CommonFileDialogCheckBox.Attach: dialog parameter can not be null");

            // Add a check button control
            dialog.AddCheckButton(Id, Text, isChecked);

            // Make this control prominent if needed
            if (IsProminent)
                dialog.MakeProminent(Id);

            // Make sure this property is set
            ApplyPropertyChange("IsChecked");

            // Sync unmanaged properties with managed properties
            SyncUnmanagedProperties();
        }
    }
}