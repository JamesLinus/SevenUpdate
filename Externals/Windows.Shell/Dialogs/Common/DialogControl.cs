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

namespace Microsoft.Windows.Dialogs
{
    /// <summary>
    ///   Abstract base class for all dialog controls
    /// </summary>
    public abstract class DialogControl
    {
        private static int nextId = DialogsDefaults.MinimumDialogControlId;
        private IDialogControlHost hostingDialog;
        private string name;

        /// <summary>
        ///   Creates a new instance of a dialog control
        /// </summary>
        protected DialogControl()
        {
            Id = nextId;

            // Support wrapping of control IDs in case you create a lot of custom controls
            if (nextId == Int32.MaxValue)
                nextId = DialogsDefaults.MinimumDialogControlId;
            else
                nextId++;
        }

        /// <summary>
        ///   Creates a new instance of a dialog control with the specified name.
        /// </summary>
        /// <param name = "name">The name for this dialog.</param>
        protected DialogControl(string name) : this()
        {
            Name = name;
        }

        /// <summary>
        ///   The native dialog that is hosting this control. This property is null is
        ///   there is not associated dialog
        /// </summary>
        public IDialogControlHost HostingDialog { get { return hostingDialog; } set { hostingDialog = value; } }

        /// <summary>
        ///   Gets or sets the name for this control.
        /// </summary>
        /// <value>A <see cref = "System.String" /> value.</value>
        /// <remarks>
        ///   The name of the control should not be modified once set
        /// </remarks>
        /// <exception cref = "System.ArgumentException">The name cannot be null or a zero-length string.</exception>
        /// <exception cref = "System.InvalidOperationException">The name has already been set.</exception>
        public string Name
        {
            get { return name; }
            set
            {
                // Names for controls need to be quite stable, 
                // as we are going to maintain a mapping between 
                // the names and the underlying Win32/COM control IDs.
                if (String.IsNullOrEmpty(value))
                    throw new ArgumentException("Dialog control name cannot be empty or null.");

                if (!String.IsNullOrEmpty(name))
                    throw new InvalidOperationException("Dialog controls cannot be renamed.");

                // Note that we don't notify the hosting dialog of 
                // the change, as the initial set of name is (must be)
                // always legal, and renames are always illegal.
                name = value;
            }
        }

        /// <summary>
        ///   Gets the identifier for this control.
        /// </summary>
        /// <value>An <see cref = "System.Int32" /> value.</value>
        public int Id { get; private set; }

        ///<summary>
        ///  Calls the hosting dialog, if it exists, to check whether the 
        ///  property can be set in the dialog's current state. 
        ///  The host should throw an exception if the change is not supported.
        ///  Note that if the dialog isn't set yet, 
        ///  there are no restrictions on setting the property.
        ///</summary>
        ///<param name = "propName">The name of the property that is changing</param>
        protected void CheckPropertyChangeAllowed(string propName)
        {
            Debug.Assert(!String.IsNullOrEmpty(propName), "Property to change was not specified");

            if (hostingDialog != null)
                hostingDialog.IsControlPropertyChangeAllowed(propName, this);
        }

        ///<summary>
        ///  Calls the hosting dialog, if it exists, to
        ///  to indicate that a property has changed, and that 
        ///  the dialog should do whatever is necessary 
        ///  to propagate the change to the native control.
        ///  Note that if the dialog isn't set yet, 
        ///  there are no restrictions on setting the property.
        ///</summary>
        ///<param name = "propName">The name of the property that is changing.</param>
        protected void ApplyPropertyChange(string propName)
        {
            Debug.Assert(!String.IsNullOrEmpty(propName), "Property changed was not specified");

            if (hostingDialog != null)
                hostingDialog.ApplyControlPropertyChange(propName, this);
        }

        /// <summary>
        ///   Compares two objects to determine whether they are equal
        /// </summary>
        /// <param name = "obj">The object to compare against.</param>
        /// <returns>A <see cref = "System.Boolean" /> value.</returns>
        public override bool Equals(object obj)
        {
            var control = obj as DialogControl;

            if (control != null)
                return (Id == control.Id);

            return false;
        }

        /// <summary>
        ///   Serves as a hash function for a particular type.
        /// </summary>
        /// <returns>An <see cref = "System.Int32" /> hash code for this control.</returns>
        public override int GetHashCode()
        {
            return name == null ? ToString().GetHashCode() : name.GetHashCode();
        }
    }
}