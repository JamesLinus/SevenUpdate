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
using System.Collections;
using System.Collections.ObjectModel;
using System.Linq;

#endregion

namespace Microsoft.Windows.Dialogs.Controls
{
    /// <summary>
    ///   Provides a strongly typed collection for dialog controls.
    /// </summary>
    /// <typeparam name = "T">DialogControl</typeparam>
    public sealed class CommonFileDialogControlCollection<T> : Collection<T> where T : DialogControl
    {
        private readonly IDialogControlHost hostingDialog;

        internal CommonFileDialogControlCollection(IDialogControlHost host)
        {
            hostingDialog = host;
        }

        ///<summary>
        ///  Defines the indexer that supports accessing controls by name.
        ///</summary>
        ///<remarks>
        ///  <para>Control names are case sensitive.</para>
        ///  <para>This indexer is useful when the dialog is created in XAML
        ///    rather than constructed in code.</para>
        ///</remarks>
        ///<exception cref = "System.ArgumentException">
        ///  The name cannot be null or a zero-length string.</exception>
        ///<remarks>
        ///  If there is more than one control with the same name, only the <B>first control</B> will be returned.
        ///</remarks>
        public T this[string name]
        {
            get
            {
                if (String.IsNullOrEmpty(name))
                    throw new ArgumentException("Control name must not be null or zero length.");

                foreach (var control in Items)
                {
                    // NOTE: we don't ToLower() the strings - casing effects 
                    // hash codes, so we are case-sensitive.
                    if (control.Name == name)
                        return control;
                    if (!(control is CommonFileDialogGroupBox))
                        continue;
                    foreach (var subControl in (control as CommonFileDialogGroupBox).Items.Cast<T>().Where(subControl => subControl.Name == name))
                        return subControl;
                }
                return null;
            }
        }

        /// <summary>
        ///   Inserts an dialog control at the specified index.
        /// </summary>
        /// <param name = "index">The location to insert the control.</param>
        /// <param name = "control">The item to insert.</param>
        /// <permission cref = "System.InvalidOperationException">A control with 
        ///   the same name already exists in this collection -or- 
        ///   the control is being hosted by another dialog -or- the associated dialog is 
        ///   showing and cannot be modified.</permission>
        protected override void InsertItem(int index, T control)
        {
            // Check for duplicates, lack of host, 
            // and during-show adds.
            if (Items.Contains(control))
                throw new InvalidOperationException("Dialog cannot have more than one control with the same name.");
            if (control.HostingDialog != null)
                throw new InvalidOperationException("Dialog control must be removed from current collections first.");
            if (!hostingDialog.IsCollectionChangeAllowed())
                throw new InvalidOperationException("Modifying controls collection while dialog is showing is not supported.");
            if (control is CommonFileDialogMenuItem)
                throw new InvalidOperationException("CommonFileDialogMenuItem controls can only be added to CommonFileDialogMenu controls.");

            // Reparent, add control.
            control.HostingDialog = hostingDialog;
            base.InsertItem(index, control);

            // Notify that we've added a control.
            hostingDialog.ApplyCollectionChanged();
        }

        /// <summary>
        ///   Removes the control at the specified index.
        /// </summary>
        /// <param name = "index">The location of the control to remove.</param>
        /// <permission cref = "System.InvalidOperationException">
        ///   The associated dialog is 
        ///   showing and cannot be modified.</permission>
        protected override void RemoveItem(int index)
        {
            throw new NotSupportedException("Custom controls cannot be removed from a File dialog once added.");
        }

        /// <summary>
        ///   Recursively searches for the control who's id matches the value
        ///   passed in the <paramref name = "id" /> parameter.
        /// </summary>
        /// <param name = "id">An integer containing the identifier of the 
        ///   control being searched for.</param>
        /// <returns>A DialogControl who's id matches the value of the
        ///   <paramref name = "id" /> parameter.</returns>
        internal DialogControl GetControlbyId(int id)
        {
            return GetSubControlbyId(Items, id);
        }

        /// <summary>
        ///   Recursively searches for a given control id in the 
        ///   collection passed via the <paramref name = "ctrlColl" /> parameter.
        /// </summary>
        /// <param name = "ctrlColl">A Collection&lt;CommonFileDialogControl&gt;</param>
        /// <param name = "id">An int containing the identifier of the control 
        ///   being searched for.</param>
        /// <returns>A DialogControl who's Id matches the value of the
        ///   <paramref name = "id" /> parameter.</returns>
        internal DialogControl GetSubControlbyId(IEnumerable ctrlColl, int id)
        {
            // if ctrlColl is null, it will throw in the foreach.
            if (ctrlColl == null)
                return null;

            foreach (DialogControl control in ctrlColl)
            {
                // Match?
                if (control.Id == id)
                    return control;

                // Search GroupBox child items
                if (!(control is CommonFileDialogGroupBox))
                    continue;
                var groupBox = control as CommonFileDialogGroupBox;

                // recurse and search the GroupBox
                var iSubCtrlCount = ((CommonFileDialogGroupBox) control).Items.Count;

                if (iSubCtrlCount <= 0)
                    continue;
                var foundControl = GetSubControlbyId(groupBox.Items, id);

                // make sure something was actually found
                if (foundControl != null)
                    return foundControl;
            }

            // Control id not found - likely an error, but the calling 
            // function should ultimately decide.
            return null;
        }
    }
}