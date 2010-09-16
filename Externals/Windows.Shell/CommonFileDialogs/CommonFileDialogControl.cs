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

using System.Diagnostics.CodeAnalysis;

#endregion

namespace Microsoft.Windows.Dialogs.Controls
{
    /// <summary>
    ///   Defines an abstract class that supports shared functionality for the 
    ///   common file dialog controls.
    /// </summary>
    public abstract class CommonFileDialogControl : DialogControl
    {
        private bool enabled = true;

        /// <summary>
        ///   Holds the text that is displayed for this control.
        /// </summary>
        private string textValue;

        private bool visible = true;

        /// <summary>
        ///   Creates a new instance of this class.
        /// </summary>
        protected CommonFileDialogControl()
        {
        }

        /// <summary>
        ///   Creates a new instance of this class with the text.
        /// </summary>
        /// <param name = "text">The text of the common file dialog control.</param>
        protected CommonFileDialogControl(string text)
        {
            textValue = text;
        }

        /// <summary>
        ///   Creates a new instance of this class with the specified name and text.
        /// </summary>
        /// <param name = "name">The name of the common file dialog control.</param>
        /// <param name = "text">The text of the common file dialog control.</param>
        protected CommonFileDialogControl(string name, string text) : base(name)
        {
            textValue = text;
        }

        /// <summary>
        ///   Gets or sets the text string that is displayed on the control.
        /// </summary>
        [SuppressMessage("Microsoft.Globalization", "CA1307:SpecifyStringComparison", MessageId = "System.String.Compare(System.String,System.String)",
            Justification = "We are not currently handling globalization or localization")]
        public virtual string Text
        {
            get { return textValue; }
            set
            {
                // Don't update this property if it hasn't changed
                if (string.Compare(value, textValue) == 0)
                    return;

                textValue = value;
                ApplyPropertyChange("Text");
            }
        }

        /// <summary>
        ///   Gets or sets a value that determines if this control is enabled.
        /// </summary>
        public bool Enabled
        {
            get { return enabled; }
            set
            {
                // Don't update this property if it hasn't changed
                if (value == enabled)
                    return;

                enabled = value;
                ApplyPropertyChange("Enabled");
            }
        }

        /// <summary>
        ///   Gets or sets a boolean value that indicates whether  
        ///   this control is visible.
        /// </summary>
        public bool Visible
        {
            get { return visible; }
            set
            {
                // Don't update this property if it hasn't changed
                if (value == visible)
                    return;

                visible = value;
                ApplyPropertyChange("Visible");
            }
        }

        /// <summary>
        ///   Has this control been added to the dialog
        /// </summary>
        internal bool IsAdded { get; set; }

        /// <summary>
        ///   Attach the custom control itself to the specified dialog
        /// </summary>
        /// <param name = "dialog">the target dialog</param>
        internal abstract void Attach(IFileDialogCustomize dialog);

        internal virtual void SyncUnmanagedProperties()
        {
            ApplyPropertyChange("Enabled");
            ApplyPropertyChange("Visible");
        }
    }
}