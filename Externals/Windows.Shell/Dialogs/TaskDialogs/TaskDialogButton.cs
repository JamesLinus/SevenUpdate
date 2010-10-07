// ***********************************************************************
// Assembly         : System.Windows
// Author           : Microsoft Corporation
// Last Modified By : Robert Baker (sevenalive)
// Last Modified On : 10-06-2010
// Copyright        : (c) Microsoft Corporation. All rights reserved.
// ***********************************************************************
namespace System.Windows.Dialogs.TaskDialogs
{
    /// <summary>
    /// Implements a button that can be hosted in a task dialog.
    /// </summary>
    public class TaskDialogButton : TaskDialogButtonBase
    {
        #region Constants and Fields

        /// <summary>
        ///   Indicates whether to show the UAC icon
        /// </summary>
        private bool showElevationIcon;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the <see cref = "TaskDialogButton" /> class.
        /// </summary>
        public TaskDialogButton()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskDialogButton"/> class.
        /// </summary>
        /// <parameter name="name">
        /// The name of the button.
        /// </parameter>
        /// <parameter name="text">
        /// The button label.
        /// </parameter>
        public TaskDialogButton(string name, string text)
            : base(name, text)
        {
        }

        #endregion

        #region Properties

        /// <summary>
        ///   Gets or sets a value indicating whether the elevation icon is displayed.
        /// </summary>
        /// <value>
        ///   <see langword = "true" /> if [show elevation icon]; otherwise, <see langword = "false" />.
        /// </value>
        public bool ShowElevationIcon
        {
            get
            {
                return this.showElevationIcon;
            }

            set
            {
                this.CheckPropertyChangeAllowed("ShowElevationIcon");
                this.showElevationIcon = value;
                this.ApplyPropertyChange("ShowElevationIcon");
            }
        }

        #endregion
    }
}