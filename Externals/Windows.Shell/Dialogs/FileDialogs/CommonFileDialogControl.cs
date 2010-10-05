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
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Defines an abstract class that supports shared functionality for the 
    ///   common file dialog controls.
    /// </summary>
    public abstract class CommonFileDialogControl : DialogControl
    {
        #region Constants and Fields

        /// <summary>
        /// </summary>
        private bool enabled = true;

        /// <summary>
        ///   Holds the text that is displayed for this control.
        /// </summary>
        private string textValue;

        /// <summary>
        /// </summary>
        private bool visible = true;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///   Creates a new instance of this class.
        /// </summary>
        protected CommonFileDialogControl()
        {
        }

        /// <summary>
        /// Creates a new instance of this class with the text.
        /// </summary>
        /// <param name="text">
        /// The text of the common file dialog control.
        /// </param>
        protected CommonFileDialogControl(string text)
        {
            this.textValue = text;
        }

        /// <summary>
        /// Creates a new instance of this class with the specified name and text.
        /// </summary>
        /// <param name="name">
        /// The name of the common file dialog control.
        /// </param>
        /// <param name="text">
        /// The text of the common file dialog control.
        /// </param>
        protected CommonFileDialogControl(string name, string text)
            : base(name)
        {
            this.textValue = text;
        }

        #endregion

        #region Properties

        /// <summary>
        ///   Gets or sets a value that determines if this control is enabled.
        /// </summary>
        public bool Enabled
        {
            get
            {
                return this.enabled;
            }

            set
            {
                // Don't update this property if it hasn't changed
                if (value == this.enabled)
                {
                    return;
                }

                this.enabled = value;
                this.ApplyPropertyChange("Enabled");
            }
        }

        /// <summary>
        ///   Gets or sets the text string that is displayed on the control.
        /// </summary>
        [SuppressMessage("Microsoft.Globalization", "CA1307:SpecifyStringComparison", MessageId = "System.String.Compare(System.String,System.String)", 
            Justification = "We are not currently handling globalization or localization")]
        public virtual string Text
        {
            get
            {
                return this.textValue;
            }

            set
            {
                // Don't update this property if it hasn't changed
                if (string.Compare(value, this.textValue, StringComparison.CurrentCulture) == 0)
                {
                    return;
                }

                this.textValue = value;
                this.ApplyPropertyChange("Text");
            }
        }

        /// <summary>
        ///   Gets or sets a boolean value that indicates whether  
        ///   this control is visible.
        /// </summary>
        public bool Visible
        {
            get
            {
                return this.visible;
            }

            set
            {
                // Don't update this property if it hasn't changed
                if (value == this.visible)
                {
                    return;
                }

                this.visible = value;
                this.ApplyPropertyChange("Visible");
            }
        }

        /// <summary>
        ///   Has this control been added to the dialog
        /// </summary>
        internal bool IsAdded { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Attach the custom control itself to the specified dialog
        /// </summary>
        /// <param name="dialog">
        /// the target dialog
        /// </param>
        internal abstract void Attach(IFileDialogCustomize dialog);

        /// <summary>
        /// </summary>
        internal virtual void SyncUnmanagedProperties()
        {
            this.ApplyPropertyChange("Enabled");
            this.ApplyPropertyChange("Visible");
        }

        #endregion
    }
}