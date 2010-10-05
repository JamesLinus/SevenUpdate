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
    /// Creates the ComboBox controls in the Common File Dialog.
    /// </summary>
    [ContentProperty("Items")]
    public class CommonFileDialogComboBox : CommonFileDialogProminentControl, ICommonFileDialogIndexedControls
    {
        #region Constants and Fields

        /// <summary>
        /// </summary>
        private readonly Collection<CommonFileDialogComboBoxItem> items = new Collection<CommonFileDialogComboBoxItem>();

        /// <summary>
        /// </summary>
        private int selectedIndex = -1;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///   Creates a new instance of this class.
        /// </summary>
        protected CommonFileDialogComboBox()
        {
        }

        /// <summary>
        /// Creates a new instance of this class with the specified name.
        /// </summary>
        /// <param name="name">
        /// Text to display for this control
        /// </param>
        protected CommonFileDialogComboBox(string name)
            : base(name, String.Empty)
        {
        }

        #endregion

        #region Events

        /// <summary>
        ///   Occurs when the SelectedIndex is changed.
        /// </summary>
        /// <remarks>
        ///   By initializing the SelectedIndexChanged event with an empty
        ///   delegate, it is not necessary to check  
        ///   if the SelectedIndexChanged is not null.
        /// </remarks>
        public event EventHandler SelectedIndexChanged = delegate { };

        #endregion

        #region Properties

        /// <summary>
        ///   Gets the collection of CommonFileDialogComboBoxItem objects.
        /// </summary>
        public Collection<CommonFileDialogComboBoxItem> Items
        {
            get
            {
                return this.items;
            }
        }

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
                // Don't update property if it hasn't changed
                if (this.selectedIndex == value)
                {
                    return;
                }

                if (this.HostingDialog == null)
                {
                    this.selectedIndex = value;
                    return;
                }

                // Only update this property if it has a valid value
                if (value < 0 || value >= this.items.Count)
                {
                    throw new IndexOutOfRangeException("Index was outside the bounds of the CommonFileDialogComboBox.");
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
        /// Attach the ComboBox control to the dialog object
        /// </summary>
        /// <param name="dialog">
        /// The target dialog
        /// </param>
        internal override void Attach(IFileDialogCustomize dialog)
        {
            Debug.Assert(dialog != null, "CommonFileDialogComboBox.Attach: dialog parameter can not be null");

            // Add the combo box control
            dialog.AddComboBox(this.Id);

            // Add the combo box items
            for (var index = 0; index < this.items.Count; index++)
            {
                dialog.AddControlItem(this.Id, index, this.items[index].Text);
            }

            // Set the currently selected item
            if (this.selectedIndex >= 0 && this.selectedIndex < this.items.Count)
            {
                dialog.SetSelectedControlItem(this.Id, this.selectedIndex);
            }
            else if (this.selectedIndex != -1)
            {
                throw new IndexOutOfRangeException("Index was outside the bounds of the CommonFileDialogComboBox.");
            }

            // Make this control prominent if needed
            if (this.IsProminent)
            {
                dialog.MakeProminent(this.Id);
            }

            // Sync additional properties
            this.SyncUnmanagedProperties();
        }

        /// <summary>
        /// Raises the SelectedIndexChanged event if this control is 
        ///   enabled.
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

        #endregion
    }

    /// <summary>
    /// Creates a ComboBoxItem for the Common File Dialog.
    /// </summary>
    public class CommonFileDialogComboBoxItem
    {
        #region Constants and Fields

        /// <summary>
        /// </summary>
        private string text = String.Empty;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///   Creates a new instance of this class.
        /// </summary>
        protected CommonFileDialogComboBoxItem()
        {
        }

        /// <summary>
        /// Creates a new instance of this class with the specified text.
        /// </summary>
        /// <param name="text">
        /// The text to use for the combo box item.
        /// </param>
        protected CommonFileDialogComboBoxItem(string text)
        {
            this.text = text;
        }

        #endregion

        #region Properties

        /// <summary>
        ///   Gets or sets the string that is displayed for this item.
        /// </summary>
        public string Text
        {
            get
            {
                return this.text;
            }

            set
            {
                this.text = value;
            }
        }

        #endregion
    }
}