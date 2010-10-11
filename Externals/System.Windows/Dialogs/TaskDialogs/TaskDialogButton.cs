// ***********************************************************************
// <copyright file="TaskDialogButton.cs"
//            project="System.Windows"
//            assembly="System.Windows"
//            solution="SevenUpdate"
//            company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// <license href="http://code.msdn.microsoft.com/WindowsAPICodePack/Project/License.aspx">Microsoft Software License</license>
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
        /// Initializes a new instance of the <see cref="TaskDialogButton"/> class.
        /// </summary>
        /// <param name="name">
        /// The name of the button.
        /// </param>
        /// <param name="text">
        /// The button label.
        /// </param>
        public TaskDialogButton(string name, string text) : base(name, text)
        {
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref = "TaskDialogButton" /> class.
        /// </summary>
        protected TaskDialogButton()
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