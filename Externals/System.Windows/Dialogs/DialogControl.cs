// ***********************************************************************
// <copyright file="DialogControl.cs" project="System.Windows" assembly="System.Windows" solution="SevenUpdate" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// <license href="http://code.msdn.microsoft.com/WindowsAPICodePack/Project/License.aspx">Microsoft Software License</license>
// ***********************************************************************

namespace System.Windows.Dialogs
{
    using Diagnostics;

    /// <summary>
    ///   Dialog Show State.
    /// </summary>
    internal enum DialogShowState
    {
        /// <summary>
        ///   The dialog is about to be shown.
        /// </summary>
        PreShow,

        /// <summary>
        ///   Currently Showing.
        /// </summary>
        Showing,

        /// <summary>
        ///   Currently Closing.
        /// </summary>
        Closing,

        /// <summary>
        ///   Closed dialog.
        /// </summary>
        Closed
    }

    /// <summary>
    ///   Abstract base class for all dialog controls.
    /// </summary>
    public abstract class DialogControl
    {
        #region Constants and Fields

        /// <summary>
        ///   The next ID.
        /// </summary>
        private static int nextId = 9;

        /// <summary>
        ///   The hosting dialog.
        /// </summary>
        private IDialogControlHost hostingDialog;

        /// <summary>
        ///   The control name.
        /// </summary>
        private string name;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the DialogControl class.
        /// </summary>
        protected DialogControl()
        {
            this.Id = nextId;

            // Support wrapping of control IDs in case you create a lot of custom controls
            if (nextId == int.MaxValue)
            {
                nextId = 9;
            }
            else
            {
                nextId++;
            }
        }

        /// <summary>
        ///   Initializes a new instance of the <c>DialogControl</c> class.
        /// </summary>
        /// <param name="name">
        ///   The name for this dialog.
        /// </param>
        protected DialogControl(string name) : this()
        {
            this.Name = name;
        }

        #endregion

        #region Properties

        /// <summary>
        ///   Gets or sets the native dialog that is hosting this control. This property is <c>null</c> is there is not
        ///   associated dialog.
        /// </summary>
        /// <value>The hosting dialog.</value>
        public IDialogControlHost HostingDialog
        {
            get
            {
                return this.hostingDialog;
            }

            set
            {
                this.hostingDialog = value;
            }
        }

        /// <summary>
        ///   Gets the identifier for this control.
        /// </summary>
        /// <value>An <c>System.Int32</c> value.</value>
        public int Id { get; private set; }

        /// <summary>
        ///   Gets or sets the name for this control.
        /// </summary>
        /// <value>A <c>System.String</c> value.</value>
        /// <remarks>
        ///   The name of the control should not be modified once set
        /// </remarks>
        /// <exception cref="System.ArgumentException">The name cannot be <c>null</c> or a zero-length string.</exception>
        /// <exception cref="System.InvalidOperationException">The name has already been set.</exception>
        public string Name
        {
            get
            {
                return this.name;
            }

            set
            {
                // Names for controls need to be quite stable, as we are going to maintain a mapping between the names
                // and the underlying Win32/COM control IDs.
                if (string.IsNullOrEmpty(value))
                {
                    throw new ArgumentException("Dialog control name cannot be empty or null.");
                }

                if (!string.IsNullOrEmpty(this.name))
                {
                    throw new InvalidOperationException("Dialog controls cannot be renamed.");
                }

                // Note that we don't notify the hosting dialog of the change, as the initial set of name is (must be)
                // always legal, and renames are always illegal.
                this.name = value;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        ///   Compares two objects to determine whether they are equal.
        /// </summary>
        /// <param name="obj">
        ///   The object to compare against.
        /// </param>
        /// <returns>
        ///   A <c>System.Boolean</c> value.
        /// </returns>
        public override bool Equals(object obj)
        {
            var control = obj as DialogControl;

            if (control != null)
            {
                return this.Id == control.Id;
            }

            return false;
        }

        /// <summary>
        ///   Serves as a hash function for a particular type.
        /// </summary>
        /// <returns>
        ///   An <c>System.Int32</c> hash code for this control.
        /// </returns>
        public override int GetHashCode()
        {
            return this.name == null ? this.ToString().GetHashCode() : this.name.GetHashCode();
        }

        #endregion

        #region Methods

        /// <summary>
        ///   Calls the hosting dialog, if it exists, to to indicate that a property has changed, and that the dialog
        ///   should do whatever is necessary to propagate the change to the native control. Note that if the dialog
        ///   isn't set yet, there are no restrictions on setting the property.
        /// </summary>
        /// <param name="propName">
        ///   The name of the property that is changing.
        /// </param>
        protected void ApplyPropertyChange(string propName)
        {
            Debug.Assert(!string.IsNullOrEmpty(propName), "Property changed was not specified");

            if (this.hostingDialog != null)
            {
                this.hostingDialog.ApplyControlPropertyChange(propName, this);
            }
        }

        /// <summary>
        ///   Calls the hosting dialog, if it exists, to check whether the property can be set in the dialog's current
        ///   state. The host should throw an exception if the change is not supported. Note that if the dialog isn't
        ///   set yet, there are no restrictions on setting the property.
        /// </summary>
        /// <param name="propName">
        ///   The name of the property that is changing.
        /// </param>
        protected void CheckPropertyChangeAllowed(string propName)
        {
            Debug.Assert(!string.IsNullOrEmpty(propName), "Property to change was not specified");

            if (this.hostingDialog != null)
            {
                this.hostingDialog.IsControlPropertyChangeAllowed(propName, this);
            }
        }

        #endregion
    }
}