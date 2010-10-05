//***********************************************************************
// Assembly         : Windows.Shell
// Author           : sevenalive
// Created          : 09-17-2010
// Last Modified By : sevenalive
// Last Modified On : 10-05-2010
// Description      : 
// Copyright        : (c) Seven Software. All rights reserved.
//***********************************************************************

namespace Microsoft.Windows.Dialogs.Controls
{
    using System;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Windows.Markup;

    /// <summary>
    /// Represents a radio button list for the Common File Dialog.
    /// </summary>
    [ContentProperty("Items")]
    public class CommonFileDialogRadioButtonList : CommonFileDialogControl, ICommonFileDialogIndexedControls
    {
        #region Constants and Fields

        /// <summary>
        /// </summary>
        private int selectedIndex = -1;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///   Creates a new instance of this class.
        /// </summary>
        protected CommonFileDialogRadioButtonList()
        {
            this.Initialize();
        }

        /// <summary>
        /// Creates a new instance of this class with the specified name.
        /// </summary>
        /// <param name="name">
        /// The name of this control.
        /// </param>
        protected CommonFileDialogRadioButtonList(string name)
            : base(name, String.Empty)
        {
            this.Initialize();
        }

        #endregion

        #region Events

        /// <summary>
        ///   Occurs when the user changes the SelectedIndex.
        /// </summary>
        /// <remarks>
        ///   By initializing the SelectedIndexChanged event with an empty
        ///   delegate, we can skip the test to determine
        ///   if the SelectedIndexChanged is null.
        ///   test.
        /// </remarks>
        public event EventHandler SelectedIndexChanged = delegate { };

        #endregion

        #region Properties

        /// <summary>
        ///   Gets the collection of CommonFileDialogRadioButtonListItem objects
        /// </summary>
        public Collection<CommonFileDialogRadioButtonListItem> Items { get; private set; }

        /// <summary>
        ///   Gets or sets the current index of the selected item.
        /// </summary>
        public int SelectedIndex
        {
            get
            {
                return this.selectedIndex;
            }

            set
            {
                // Don't update this property if it hasn't changed
                if (this.selectedIndex == value)
                {
                    return;
                }

                // If the native dialog has not been created yet
                if (this.HostingDialog == null)
                {
                    this.selectedIndex = value;
                    return;
                }

                // Check for valid index
                if (value < 0 || value >= this.Items.Count)
                {
                    throw new IndexOutOfRangeException("Index was outside the bounds of the CommonFileDialogRadioButtonList.");
                }

                this.selectedIndex = value;
                this.ApplyPropertyChange("SelectedIndex");
            }
        }

        #endregion

        #region Implemented Interfaces

        #region ICommonFileDialogIndexedControls

        /// <summary>
        /// </summary>
        void ICommonFileDialogIndexedControls.RaiseSelectedIndexChangedEvent()
        {
            // Make sure that this control is enabled and has a specified delegate
            if (this.Enabled)
            {
                this.SelectedIndexChanged(this, EventArgs.Empty);
            }
        }

        #endregion

        #endregion

        #region Methods

        /// <summary>
        /// Attach the RadioButtonList control to the dialog object
        /// </summary>
        /// <param name="dialog">
        /// The target dialog
        /// </param>
        internal override void Attach(IFileDialogCustomize dialog)
        {
            Debug.Assert(dialog != null, "CommonFileDialogRadioButtonList.Attach: dialog parameter can not be null");

            // Add the radio button list control
            dialog.AddRadioButtonList(this.Id);

            // Add the radio button list items
            for (var index = 0; index < this.Items.Count; index++)
            {
                dialog.AddControlItem(this.Id, index, this.Items[index].Text);
            }

            // Set the currently selected item
            if (this.selectedIndex >= 0 && this.selectedIndex < this.Items.Count)
            {
                dialog.SetSelectedControlItem(this.Id, this.selectedIndex);
            }
            else if (this.selectedIndex != -1)
            {
                throw new IndexOutOfRangeException("Index was outside the bounds of the CommonFileDialogRadioButtonList.");
            }

            // Sync unmanaged properties with managed properties
            this.SyncUnmanagedProperties();
        }

        /// <summary>
        /// Occurs when the user changes the SelectedIndex.
        /// </summary>
        /// <remarks>
        /// Because this method is defined in an interface, we can either
        ///   have it as public, or make it private and explicitly implement (like below).
        ///   Making it public doesn't really help as its only internal (but can't have this 
        ///   internal because of the interface)
        /// </remarks>
        protected virtual void RaiseSelectedIndexChangedEvent()
        {
            ((ICommonFileDialogIndexedControls)this).RaiseSelectedIndexChangedEvent();
        }

        /// <summary>
        /// Initializes the item collection for this class.
        /// </summary>
        private void Initialize()
        {
            this.Items = new Collection<CommonFileDialogRadioButtonListItem>();
        }

        #endregion
    }

    /// <summary>
    /// Represents a list item for the CommonFileDialogRadioButtonList object.
    /// </summary>
    public class CommonFileDialogRadioButtonListItem
    {
        #region Constructors and Destructors

        /// <summary>
        ///   Creates a new instance of this class.
        /// </summary>
        protected CommonFileDialogRadioButtonListItem()
        {
            this.Text = String.Empty;
        }

        /// <summary>
        /// Creates a new instance of this class with the specified text.
        /// </summary>
        /// <param name="text">
        /// The string that you want to display for this list item.
        /// </param>
        protected CommonFileDialogRadioButtonListItem(string text)
        {
            this.Text = text;
        }

        #endregion

        #region Properties

        /// <summary>
        ///   Gets or sets the string that will be displayed for this list item.
        /// </summary>
        public string Text { get; set; }

        #endregion
    }
}