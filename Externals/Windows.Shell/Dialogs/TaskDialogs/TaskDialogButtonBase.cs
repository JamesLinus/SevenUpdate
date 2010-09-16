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

#endregion

namespace Microsoft.Windows.Dialogs
{
    // ContentProperty allows us to specify the text 
    // of the button as the child text of
    // a button element in XAML, as well as explicitly 
    // set with 'Text="<text>"'
    // Note that this attribute is inherited, so it 
    // applies to command-links and radio buttons as well.
    /// <summary>
    ///   Defines the abstract base class for task dialog buttons. 
    ///   Classes that inherit from this class will inherit 
    ///   the Text property defined in this class.
    /// </summary>
    public abstract class TaskDialogButtonBase : TaskDialogControl
    {
        private bool defaultControl;
        private bool enabled = true;
        private string text;

        /// <summary>
        ///   Creates a new instance on a task dialog button.
        /// </summary>
        protected TaskDialogButtonBase()
        {
        }

        /// <summary>
        ///   Creates a new instance on a task dialog button with
        ///   the specified name and text.
        /// </summary>
        /// <param name = "name">The name for this button.</param>
        /// <param name = "text">The label for this button.</param>
        protected TaskDialogButtonBase(string name, string text) : base(name)
        {
            this.text = text;
        }

        // Note that we don't need to explicitly 
        // implement the add/remove delegate for the Click event;
        // the hosting dialog only needs the delegate 
        // information when the Click event is 
        // raised (indirectly) by NativeTaskDialog, 
        // so the latest delegate is always available.

        /// <summary>
        ///   Gets or sets the button text.
        /// </summary>
        public string Text
        {
            get { return text; }
            set
            {
                CheckPropertyChangeAllowed("Text");
                text = value;
                ApplyPropertyChange("Text");
            }
        }

        /// <summary>
        ///   Gets or sets a value that determines whether the
        ///   button is enabled. The enabled state can cannot be changed
        ///   before the dialog is shown.
        /// </summary>
        public bool Enabled
        {
            get { return enabled; }
            set
            {
                CheckPropertyChangeAllowed("Enabled");
                enabled = value;
                ApplyPropertyChange("Enabled");
            }
        }

        /// <summary>
        ///   Gets or sets a value that indicates whether
        ///   this button is the default button.
        /// </summary>
        public bool Default
        {
            get { return defaultControl; }
            set
            {
                CheckPropertyChangeAllowed("Default");
                defaultControl = value;
                ApplyPropertyChange("Default");
            }
        }

        /// <summary>
        ///   Raised when the task dialog button is clicked.
        /// </summary>
        public event EventHandler Click;

        internal void RaiseClickEvent()
        {
            // Only perform click if the button is enabled.
            if (!enabled)
                return;

            var handler = Click;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }

        /// <summary>
        ///   Returns the Text property value for this button.
        /// </summary>
        /// <returns>A <see cref = "System.String" />.</returns>
        public override string ToString()
        {
            return text ?? "";
        }
    }
}