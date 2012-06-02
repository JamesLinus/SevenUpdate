// <copyright file="DialogControl.cs" project="SevenSoftware.Windows" company="Microsoft Corporation">Microsoft Corporation</copyright>
// <license href="http://code.msdn.microsoft.com/WindowsAPICodePack/Project/License.aspx" name="Microsoft Software License" />

namespace SevenSoftware.Windows.Dialogs
{
    using System;
    using System.Diagnostics;

    using SevenSoftware.Windows.Dialogs.TaskDialog;
    using SevenSoftware.Windows.Properties;

    /// <summary>Abstract base class for all dialog controls</summary>
    public abstract class DialogControl
    {
        /// <summary>The next ID.</summary>
        private static int nextId = TaskDialogDefaults.MinimumDialogControlId;

        /// <summary>The control name.</summary>
        private string name;

        /// <summary>
        ///   Initializes a new instance of the <see cref="DialogControl" /> class. Creates a new instance of a dialog
        ///   control
        /// </summary>
        protected DialogControl()
        {
            this.Id = nextId;

            // Support wrapping of control IDs in case you create a lot of custom controls
            if (nextId == int.MaxValue)
            {
                nextId = TaskDialogDefaults.MinimumDialogControlId;
            }
            else
            {
                nextId++;
            }
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="DialogControl" /> class. Creates a new instance of a dialog
        ///   control with the specified name.
        /// </summary>
        /// <param name="name">The name for this dialog.</param>
        protected DialogControl(string name) : this()
        {
            this.Name = name;
        }

        /// <summary>
        ///   Gets or sets the native dialog that is hosting this control. This property is null is there is not
        ///   associated dialog
        /// </summary>
        public IDialogControlHost HostingDialog { get; set; }

        /// <summary>Gets the identifier for this control.</summary>
        /// <value>An <see cref="int" /> value.</value>
        public int Id { get; private set; }

        /// <summary>Gets the name for this control.</summary>
        /// <value>A <see cref="string" /> value.</value>
        public string Name
        {
            get { return this.name; }

            private set
            {
                // Names for controls need to be quite stable, as we are going to maintain a mapping between the names
                // and the underlying Win32/COM control IDs.
                if (string.IsNullOrEmpty(value))
                {
                    throw new ArgumentException(Resources.DialogControlNameCannotBeEmpty);
                }

                if (!string.IsNullOrEmpty(this.name))
                {
                    throw new InvalidOperationException(Resources.DialogControlsCannotBeRenamed);
                }

                // Note that we don't notify the hosting dialog of the change, as the initial set of name is (must be)
                // always legal, and renames are always illegal.
                this.name = value;
            }
        }

        /// <summary>Compares two objects to determine whether they are equal</summary>
        /// <param name="obj">The object to compare against.</param>
        /// <returns>A <see cref="bool" /> value.</returns>
        public override bool Equals(object obj)
        {
            var control = obj as DialogControl;

            if (control != null)
            {
                return this.Id == control.Id;
            }

            return false;
        }

        /// <summary>Serves as a hash function for a particular type.</summary>
        /// <returns>An <see cref="int" /> hash code for this control.</returns>
        public override int GetHashCode()
        {
            if (this.Name == null)
            {
                return this.ToString().GetHashCode();
            }

            return this.Name.GetHashCode();
        }

        /// <summary>
        ///   Calls the hosting dialog, if it exists, to to indicate that a property has changed, and that the dialog
        ///   should do whatever is necessary to propagate the change to the native control. Note that if the dialog
        ///   isn't set yet, there are no restrictions on setting the property.
        /// </summary>
        /// <param name="propName">The name of the property that is changing.</param>
        protected void ApplyPropertyChange(string propName)
        {
            Debug.Assert(!string.IsNullOrEmpty(propName), "Property changed was not specified");

            if (this.HostingDialog != null)
            {
                this.HostingDialog.ApplyControlPropertyChange(propName, this);
            }
        }

        /// <summary>
        ///   Calls the hosting dialog, if it exists, to check whether the property can be set in the dialog's current
        ///   state. The host should throw an exception if the change is not supported. Note that if the dialog isn't
        ///   set yet, there are no restrictions on setting the property.
        /// </summary>
        /// <param name="propName">The name of the property that is changing</param>
        protected void CheckPropertyChangeAllowed(string propName)
        {
            Debug.Assert(!string.IsNullOrEmpty(propName), "Property to change was not specified");

            if (this.HostingDialog != null)
            {
                // This will throw if the property change is not allowed.
                this.HostingDialog.IsControlPropertyChangeAllowed(propName, this);
            }
        }
    }
}