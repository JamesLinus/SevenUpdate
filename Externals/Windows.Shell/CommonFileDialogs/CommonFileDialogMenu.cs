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
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Markup;

#endregion

namespace Microsoft.Windows.Dialogs.Controls
{
    /// <summary>
    ///   Defines the menu controls for the Common File Dialog.
    /// </summary>
    [ContentProperty("Items")]
    public abstract class CommonFileDialogMenu : CommonFileDialogProminentControl
    {
        /// <summary>
        ///   Creates a new instance of this class.
        /// </summary>
        public CommonFileDialogMenu()
        {
            Initialize();
        }

        /// <summary>
        ///   Creates a new instance of this class with the specified text.
        /// </summary>
        /// <param name = "text">The text to display for this control.</param>
        public CommonFileDialogMenu(string text) : base(text)
        {
            Initialize();
        }

        /// <summary>
        ///   Creates a new instance of this class with the specified name and text.
        /// </summary>
        /// <param name = "name">The name of this control.</param>
        /// <param name = "text">The text to display for this control.</param>
        public CommonFileDialogMenu(string name, string text) : base(name, text)
        {
            Initialize();
        }

        /// <summary>
        ///   Gets the collection of CommonFileDialogMenuItem objects.
        /// </summary>
        public Collection<CommonFileDialogMenuItem> Items { get; private set; }

        /// <summary>
        ///   Initializes the item collection for this class.
        /// </summary>
        private void Initialize()
        {
            Items = new Collection<CommonFileDialogMenuItem>();
        }

        /// <summary>
        ///   Attach the Menu control to the dialog object.
        /// </summary>
        /// <param name = "dialog">the target dialog</param>
        internal override void Attach(IFileDialogCustomize dialog)
        {
            Debug.Assert(dialog != null, "CommonFileDialogMenu.Attach: dialog parameter can not be null");

            // Add the menu control
            dialog.AddMenu(Id, Text);

            // Add the menu items
            foreach (var item in Items)
                dialog.AddControlItem(Id, item.Id, item.Text);

            // Make prominent as needed
            if (IsProminent)
                dialog.MakeProminent(Id);

            // Sync unmanaged properties with managed properties
            SyncUnmanagedProperties();
        }
    }

    /// <summary>
    ///   Creates the CommonFileDialogMenuItem items for the Common File Dialog.
    /// </summary>
    public abstract class CommonFileDialogMenuItem : CommonFileDialogControl
    {
        /// <summary>
        ///   Creates a new instance of this class.
        /// </summary>
        public CommonFileDialogMenuItem() : base(String.Empty)
        {
        }

        /// <summary>
        ///   Creates a new instance of this class with the specified text.
        /// </summary>
        /// <param name = "text">The text to display for this control.</param>
        public CommonFileDialogMenuItem(string text) : base(text)
        {
        }

        /// <summary>
        ///   Occurs when a user clicks a menu item.
        /// </summary>
        public event EventHandler Click = delegate { };

        internal void RaiseClickEvent()
        {
            // Make sure that this control is enabled and has a specified delegate
            if (Enabled)
                Click(this, EventArgs.Empty);
        }

        /// <summary>
        ///   Attach this control to the dialog object
        /// </summary>
        /// <param name = "dialog">Target dialog</param>
        internal override void Attach(IFileDialogCustomize dialog)
        {
            // Items are added via the menu itself
        }
    }
}