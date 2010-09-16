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
    ///   Defines the text box controls in the Common File Dialog.
    /// </summary>
    public abstract class CommonFileDialogTextBox : CommonFileDialogControl
    {
        /// <summary>
        ///   Holds an instance of the customized (/native) dialog and should
        ///   be null until after the Attach() call is made.
        /// </summary>
        private IFileDialogCustomize customizedDialog;

        /// <summary>
        ///   Creates a new instance of this class.
        /// </summary>
        public CommonFileDialogTextBox() : base(String.Empty)
        {
        }

        /// <summary>
        ///   Creates a new instance of this class with the specified text.
        /// </summary>
        /// <param name = "text">The text to display for this control.</param>
        public CommonFileDialogTextBox(string text) : base(text)
        {
        }

        /// <summary>
        ///   Creates a new instance of this class with the specified name and text.
        /// </summary>
        /// <param name = "name">The name of this control.</param>
        /// <param name = "text">The text to display for this control.</param>
        public CommonFileDialogTextBox(string name, string text) : base(name, text)
        {
        }

        internal bool Closed { set; get; }

        /// <summary>
        ///   Gets or sets a value for the text string contained in the CommonFileDialogTextBox.
        /// </summary>
        public override string Text
        {
            get
            {
                if (!Closed)
                    SyncValue();

                return base.Text;
            }

            set
            {
                if (customizedDialog != null)
                    customizedDialog.SetEditBoxText(Id, value);

                base.Text = value;
            }
        }

        /// <summary>
        ///   Attach the TextBox control to the dialog object
        /// </summary>
        /// <param name = "dialog">Target dialog</param>
        internal override void Attach(IFileDialogCustomize dialog)
        {
            Debug.Assert(dialog != null, "CommonFileDialogTextBox.Attach: dialog parameter can not be null");

            // Add a text entry control
            dialog.AddEditBox(Id, Text);

            // Set to local instance in order to gate access to same.
            customizedDialog = dialog;

            // Sync unmanaged properties with managed properties
            SyncUnmanagedProperties();

            Closed = false;
        }

        internal void SyncValue()
        {
            // Make sure that the local native dialog instance is NOT 
            // null. If it's null, just return the "textValue" var,
            // otherwise, use the native call to get the text value, 
            // setting the textValue member variable then return it.

            if (customizedDialog == null)
                return;
            string textValue;
            customizedDialog.GetEditBoxText(Id, out textValue);

            base.Text = textValue;
        }
    }
}