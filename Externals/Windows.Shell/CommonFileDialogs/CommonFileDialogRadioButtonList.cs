//Copyright (c) Microsoft Corporation.  All rights reserved.
//Modified by Robert Baker, Seven Software 2010.

#region

using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Markup;

#endregion

namespace Microsoft.Windows.Dialogs.Controls
{
    /// <summary>
    ///   Represents a radio button list for the Common File Dialog.
    /// </summary>
    [ContentProperty("Items")]
    public abstract class CommonFileDialogRadioButtonList : CommonFileDialogControl, ICommonFileDialogIndexedControls
    {
        private int selectedIndex = -1;

        /// <summary>
        ///   Creates a new instance of this class.
        /// </summary>
        public CommonFileDialogRadioButtonList()
        {
            Initialize();
        }

        /// <summary>
        ///   Creates a new instance of this class with the specified name.
        /// </summary>
        /// <param name = "name">The name of this control.</param>
        public CommonFileDialogRadioButtonList(string name) : base(name, String.Empty)
        {
            Initialize();
        }

        /// <summary>
        ///   Gets the collection of CommonFileDialogRadioButtonListItem objects
        /// </summary>
        public Collection<CommonFileDialogRadioButtonListItem> Items { get; private set; }

        #region ICommonFileDialogIndexedControls Members

        /// <summary>
        ///   Gets or sets the current index of the selected item.
        /// </summary>
        public int SelectedIndex
        {
            get { return selectedIndex; }
            set
            {
                // Don't update this property if it hasn't changed
                if (selectedIndex == value)
                    return;

                // If the native dialog has not been created yet
                if (HostingDialog == null)
                {
                    selectedIndex = value;
                    return;
                }

                // Check for valid index
                if (value < 0 || value >= Items.Count)
                    throw new IndexOutOfRangeException("Index was outside the bounds of the CommonFileDialogRadioButtonList.");
                selectedIndex = value;
                ApplyPropertyChange("SelectedIndex");
            }
        }

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

        /// <summary>
        ///   Occurs when the user changes the SelectedIndex.
        /// </summary>
        /// <remarks>
        ///   Because this method is defined in an interface, we can either
        ///   have it as public, or make it private and explicitly implement (like below).
        ///   Making it public doesn't really help as its only internal (but can't have this 
        ///   internal because of the interface)
        /// </remarks>
        void ICommonFileDialogIndexedControls.RaiseSelectedIndexChangedEvent()
        {
            // Make sure that this control is enabled and has a specified delegate
            if (Enabled)
                SelectedIndexChanged(this, EventArgs.Empty);
        }

        #endregion

        /// <summary>
        ///   Initializes the item collection for this class.
        /// </summary>
        private void Initialize()
        {
            Items = new Collection<CommonFileDialogRadioButtonListItem>();
        }

        /// <summary>
        ///   Attach the RadioButtonList control to the dialog object
        /// </summary>
        /// <param name = "dialog">The target dialog</param>
        internal override void Attach(IFileDialogCustomize dialog)
        {
            Debug.Assert(dialog != null, "CommonFileDialogRadioButtonList.Attach: dialog parameter can not be null");

            // Add the radio button list control
            dialog.AddRadioButtonList(Id);

            // Add the radio button list items
            for (var index = 0; index < Items.Count; index++)
                dialog.AddControlItem(Id, index, Items[index].Text);

            // Set the currently selected item
            if (selectedIndex >= 0 && selectedIndex < Items.Count)
                dialog.SetSelectedControlItem(Id, selectedIndex);
            else if (selectedIndex != -1)
                throw new IndexOutOfRangeException("Index was outside the bounds of the CommonFileDialogRadioButtonList.");


            // Sync unmanaged properties with managed properties
            SyncUnmanagedProperties();
        }
    }

    /// <summary>
    ///   Represents a list item for the CommonFileDialogRadioButtonList object.
    /// </summary>
    public abstract class CommonFileDialogRadioButtonListItem
    {
        /// <summary>
        ///   Creates a new instance of this class.
        /// </summary>
        public CommonFileDialogRadioButtonListItem()
        {
            Text = String.Empty;
        }

        /// <summary>
        ///   Creates a new instance of this class with the specified text.
        /// </summary>
        /// <param name = "text">The string that you want to display for this list item.</param>
        public CommonFileDialogRadioButtonListItem(string text)
        {
            Text = text;
        }

        /// <summary>
        ///   Gets or sets the string that will be displayed for this list item.
        /// </summary>
        public string Text { get; set; }
    }
}