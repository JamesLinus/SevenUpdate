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
    /// Defines the menu controls for the Common File Dialog.
    /// </summary>
    [ContentProperty("Items")]
    public class CommonFileDialogMenu : CommonFileDialogProminentControl
    {
        #region Constructors and Destructors

        /// <summary>
        ///   Creates a new instance of this class.
        /// </summary>
        protected CommonFileDialogMenu()
        {
            this.Initialize();
        }

        /// <summary>
        /// Creates a new instance of this class with the specified text.
        /// </summary>
        /// <param name="text">
        /// The text to display for this control.
        /// </param>
        protected CommonFileDialogMenu(string text)
            : base(text)
        {
            this.Initialize();
        }

        /// <summary>
        /// Creates a new instance of this class with the specified name and text.
        /// </summary>
        /// <param name="name">
        /// The name of this control.
        /// </param>
        /// <param name="text">
        /// The text to display for this control.
        /// </param>
        protected CommonFileDialogMenu(string name, string text)
            : base(name, text)
        {
            this.Initialize();
        }

        #endregion

        #region Properties

        /// <summary>
        ///   Gets the collection of CommonFileDialogMenuItem objects.
        /// </summary>
        public Collection<CommonFileDialogMenuItem> Items { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        /// Attach the Menu control to the dialog object.
        /// </summary>
        /// <param name="dialog">
        /// the target dialog
        /// </param>
        internal override void Attach(IFileDialogCustomize dialog)
        {
            Debug.Assert(dialog != null, "CommonFileDialogMenu.Attach: dialog parameter can not be null");

            // Add the menu control
            dialog.AddMenu(this.Id, this.Text);

            // Add the menu items
            foreach (var item in this.Items)
            {
                dialog.AddControlItem(this.Id, item.Id, item.Text);
            }

            // Make prominent as needed
            if (this.IsProminent)
            {
                dialog.MakeProminent(this.Id);
            }

            // Sync unmanaged properties with managed properties
            this.SyncUnmanagedProperties();
        }

        /// <summary>
        /// Initializes the item collection for this class.
        /// </summary>
        private void Initialize()
        {
            this.Items = new Collection<CommonFileDialogMenuItem>();
        }

        #endregion
    }

    /// <summary>
    /// Creates the CommonFileDialogMenuItem items for the Common File Dialog.
    /// </summary>
    public class CommonFileDialogMenuItem : CommonFileDialogControl
    {
        #region Constructors and Destructors

        /// <summary>
        ///   Creates a new instance of this class.
        /// </summary>
        protected CommonFileDialogMenuItem()
            : base(String.Empty)
        {
        }

        /// <summary>
        /// Creates a new instance of this class with the specified text.
        /// </summary>
        /// <param name="text">
        /// The text to display for this control.
        /// </param>
        protected CommonFileDialogMenuItem(string text)
            : base(text)
        {
        }

        #endregion

        #region Events

        /// <summary>
        ///   Occurs when a user clicks a menu item.
        /// </summary>
        public event EventHandler Click = delegate { };

        #endregion

        #region Methods

        /// <summary>
        /// Attach this control to the dialog object
        /// </summary>
        /// <param name="dialog">
        /// Target dialog
        /// </param>
        internal override void Attach(IFileDialogCustomize dialog)
        {
            // Items are added via the menu itself
        }

        /// <summary>
        /// </summary>
        internal void RaiseClickEvent()
        {
            // Make sure that this control is enabled and has a specified delegate
            if (this.Enabled)
            {
                this.Click(this, EventArgs.Empty);
            }
        }

        #endregion
    }
}