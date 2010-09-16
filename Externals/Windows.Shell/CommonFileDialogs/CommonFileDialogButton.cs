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
    ///   Creates the push button controls used by the Common File Dialog.
    /// </summary>
    public abstract class CommonFileDialogButton : CommonFileDialogProminentControl
    {
        /// <summary>
        ///   Initializes a new instance of this class.
        /// </summary>
        public CommonFileDialogButton() : base(String.Empty)
        {
        }

        /// <summary>
        ///   Initializes a new instance of this class with the text only.
        /// </summary>
        /// <param name = "text">The text to display for this control.</param>
        public CommonFileDialogButton(string text) : base(text)
        {
        }

        /// <summary>
        ///   Initializes a new instance of this class with the specified name and text.
        /// </summary>
        /// <param name = "name">The name of this control.</param>
        /// <param name = "text">The text to display for this control.</param>
        public CommonFileDialogButton(string name, string text) : base(name, text)
        {
        }

        /// <summary>
        ///   Attach the PushButton control to the dialog object
        /// </summary>
        /// <param name = "dialog">Target dialog</param>
        internal override void Attach(IFileDialogCustomize dialog)
        {
            Debug.Assert(dialog != null, "CommonFileDialogButton.Attach: dialog parameter can not be null");

            // Add a push button control
            dialog.AddPushButton(Id, Text);

            // Make this control prominent if needed
            if (IsProminent)
                dialog.MakeProminent(Id);

            // Sync unmanaged properties with managed properties
            SyncUnmanagedProperties();
        }

        /// <summary>
        ///   Occurs when the user clicks the control. This event is routed from COM via the event sink.
        /// </summary>
        public event EventHandler Click = delegate { };

        internal void RaiseClickEvent()
        {
            // Make sure that this control is enabled and has a specified delegate
            if (Enabled)
                Click(this, EventArgs.Empty);
        }
    }
}