// ***********************************************************************
// <copyright file="DialogControlCollection.cs" project="System.Windows" assembly="System.Windows" solution="SevenUpdate" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// <license href="http://code.msdn.microsoft.com/WindowsAPICodePack/Project/License.aspx">Microsoft Software License</license>
// ***********************************************************************

namespace System.Windows.Dialogs
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;

    /// <summary>Strongly typed collection for dialog controls.</summary>
    /// <typeparam name="T">The <see cref="DialogControl" />.</typeparam>
    /// <typeparameter name="T">The <see cref="DialogControl" /></typeparameter>
    public sealed class DialogControlCollection<T> : Collection<T> where T : DialogControl
    {
        #region Constants and Fields

        /// <summary>The host dialog.</summary>
        private readonly IDialogControlHost hostingDialog;

        #endregion

        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref="DialogControlCollection{T}" /> class. Initializes a new instance of the <see cref="DialogControlCollection&lt;T&gt;" /> class.</summary>
        /// <param name="host">The dialog control host.</param>
        internal DialogControlCollection(IDialogControlHost host)
        {
            this.hostingDialog = host;
        }

        #endregion

        #region Methods

        /// <summary>Recursively searches for a given control id in the collection passed via the <paramref name="subControl" /> parameter.</summary>
        /// <param name="subControl">A Collection of CommonFileDialogControls.</param>
        /// <param name="id">An int containing the identifier of the control being searched for.</param>
        /// <returns>A <see cref="DialogControl" /> who's Id matches the value of the <paramref name="id" /> parameter.</returns>
        internal static DialogControl GetSubControlById(IEnumerable<T> subControl, int id)
        {
            // if subControl is null, it will throw in the foreach.
            return subControl == null ? null : subControl.Cast<DialogControl>().FirstOrDefault(control => control.Id == id);

            // Control id not found - likely an error, but the calling 
            // function should ultimately decide.
        }

        /// <summary>Recursively searches for the control who's id matches the value passed in the <paramref name="id" /> parameter.</summary>
        /// <param name="id">An integer containing the identifier of the control being searched for.</param>
        /// <returns>A <see cref="DialogControl" /> who's id matches the value of the<paramref name="id" /> parameter.</returns>
        internal DialogControl GetControlById(int id)
        {
            return GetSubControlById(this.Items, id);
        }

        /// <summary>Inserts an dialog control at the specified index.</summary>
        /// <param name="index">The location to insert the control.</param>
        /// <param name="control">The item to insert.</param>
        /// <permission cref="System.InvalidOperationException">A control with the same name already exists in this collection -or- the control is being hosted by another dialog -or- the associated dialog is showing and cannot be modified.</permission>
        protected override void InsertItem(int index, T control)
        {
            // Check for duplicates, lack of host, 
            // and during-show adds.
            if (this.Items.Contains(control))
            {
                throw new InvalidOperationException("Dialog cannot have more than one control with the same name.");
            }

            if (control.HostingDialog != null)
            {
                throw new InvalidOperationException("Dialog control must be removed from current collections first.");
            }

            if (!this.hostingDialog.IsCollectionChangeAllowed())
            {
                throw new InvalidOperationException("Modifying controls collection while dialog is showing is not supported.");
            }

            // Reparent, add control.
            control.HostingDialog = this.hostingDialog;
            base.InsertItem(index, control);

            // Notify that we've added a control.
            this.hostingDialog.ApplyCollectionChanged();
        }

        /// <summary>Removes the control at the specified index.</summary>
        /// <param name="index">The location of the control to remove.</param>
        /// <permission cref="System.InvalidOperationException">The associated dialog is showing and cannot be modified.</permission>
        protected override void RemoveItem(int index)
        {
            // Notify that we're about to remove a control.
            // Throw if dialog showing.
            if (!this.hostingDialog.IsCollectionChangeAllowed())
            {
                throw new InvalidOperationException("Modifying controls collection while dialog is showing is not supported.");
            }

            DialogControl control = this.Items[index];

            // Unparent and remove.
            control.HostingDialog = null;
            base.RemoveItem(index);

            this.hostingDialog.ApplyCollectionChanged();
        }

        #endregion
    }
}