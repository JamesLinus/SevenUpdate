// ***********************************************************************
// <copyright file="DialogControlCollection.cs" project="System.Windows" assembly="System.Windows" solution="SevenUpdate" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// <license href="http://code.msdn.microsoft.com/WindowsAPICodePack/Project/License.aspx">Microsoft Software License</license>
// ***********************************************************************

namespace System.Windows.Dialogs
{
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Windows.Properties;

    /// <summary>Strongly typed collection for dialog controls.</summary>
    /// <typeparam name = "T">The DialogControl</typeparam>
    public sealed class DialogControlCollection<T> : Collection<T>
        where T : DialogControl
    {
        #region Constants and Fields

        /// <summary>The host dialog.</summary>
        private IDialogControlHost hostingDialog;

        #endregion

        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref = "DialogControlCollection{T}" /> class.</summary>
        /// <param name = "host">The host.</param>
        internal DialogControlCollection(IDialogControlHost host)
        {
            this.hostingDialog = host;
        }

        #endregion

        #region Public Indexers

        /// <summary>Defines the indexer that supports accessing controls by name.</summary>
        /// <param name = "name">The name of the control.</param>
        /// <remarks><para>Control names are case sensitive.</para> <para>This indexer is useful when the dialog is created in XAML rather than constructed in code.</para></remarks>
        /// <exception cref = "System.ArgumentException">
        ///   The name cannot be null or a zero-length string.</exception>
        /// <remarks>If there is more than one control with the same name, only the <B>first control</B> will be returned.</remarks>
        /// <returns>The control.</returns>
        public T this[string name]
        {
            get
            {
                if (string.IsNullOrEmpty(name))
                {
                    throw new ArgumentException(Resources.DialogCollectionControlNameNull, "name");
                }

                return this.Items.FirstOrDefault(x => x.Name == name);
            }
        }

        #endregion

        #region Methods

        /// <summary>Searches for the control who's id matches the value passed in the <paramref name = "id" /> parameter.</summary>
        /// <param name = "id">An integer containing the identifier of the control being searched for.</param>
        /// <returns>A DialogControl who's id matches the value of the <paramref name = "id" /> parameter.</returns>
        internal DialogControl GetControlbyId(int id)
        {
            return this.Items.FirstOrDefault(x => x.Id == id);
        }

        /// <summary>Inserts an dialog control at the specified index.</summary>
        /// <param name = "index">The location to insert the control.</param>
        /// <param name = "control">The item to insert.</param>
        /// <permission cref = "System.InvalidOperationException">A control with the same name already exists in this collection -or- the control is being hosted by another dialog -or- the associated dialog is showing and cannot be modified.</permission>
        protected override void InsertItem(int index, T control)
        {
            // Check for duplicates, lack of host, and during-show adds.
            if (this.Items.Contains(control))
            {
                throw new InvalidOperationException(Resources.DialogCollectionCannotHaveDuplicateNames);
            }

            if (control.HostingDialog != null)
            {
                throw new InvalidOperationException(Resources.DialogCollectionControlAlreadyHosted);
            }

            if (!this.hostingDialog.IsCollectionChangeAllowed())
            {
                throw new InvalidOperationException(Resources.DialogCollectionModifyShowingDialog);
            }

            // Reparent, add control.
            control.HostingDialog = this.hostingDialog;
            base.InsertItem(index, control);

            // Notify that we've added a control.
            this.hostingDialog.ApplyCollectionChanged();
        }

        /// <summary>Removes the control at the specified index.</summary>
        /// <param name = "index">The location of the control to remove.</param>
        /// <permission cref = "System.InvalidOperationException"> The associated dialog is showing and cannot be modified.</permission>
        protected override void RemoveItem(int index)
        {
            // Notify that we're about to remove a control. Throw if dialog showing.
            if (!this.hostingDialog.IsCollectionChangeAllowed())
            {
                throw new InvalidOperationException(Resources.DialogCollectionModifyShowingDialog);
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
