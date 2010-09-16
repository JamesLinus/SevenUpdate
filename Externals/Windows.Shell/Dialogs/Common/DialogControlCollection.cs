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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

#endregion

namespace Microsoft.Windows.Dialogs
{
    /// <summary>
    ///   Strongly typed collection for dialog controls.
    /// </summary>
    /// <typeparam name = "T">DialogControl</typeparam>
    public sealed class DialogControlCollection<T> : Collection<T> where T : DialogControl
    {
        private readonly IDialogControlHost hostingDialog;

        internal DialogControlCollection(IDialogControlHost host)
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

                return Items.FirstOrDefault(control => control.Name == name);
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
            // Notify that we're about to remove a control.
            // Throw if dialog showing.
            if (!hostingDialog.IsCollectionChangeAllowed())
                throw new InvalidOperationException("Modifying controls collection while dialog is showing is not supported.");

            DialogControl control = Items[index];

            // Unparent and remove.
            control.HostingDialog = null;
            base.RemoveItem(index);

            hostingDialog.ApplyCollectionChanged();
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
            //return ( Items.Count == 0 ? null :  
            //    GetSubControlbyId(Items as IEnumerable<T>,
            //    id) 
            //);
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
        internal DialogControl GetSubControlbyId(IEnumerable<T> ctrlColl, int id)
        {
            // if ctrlColl is null, it will throw in the foreach.
            return ctrlColl == null ? null : ctrlColl.Cast<DialogControl>().FirstOrDefault(control => control.Id == id);

            // Control id not found - likely an error, but the calling 
            // function should ultimately decide.
        }
    }
}