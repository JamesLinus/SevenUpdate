//***********************************************************************
// Assembly         : Windows.Shell
// Author           : sevenalive
// Created          : 09-17-2010
// Last Modified By : sevenalive
// Last Modified On : 10-05-2010
// Description      : 
// Copyright        : (c) Seven Software. All rights reserved.
//***********************************************************************

namespace Microsoft.Windows.Dialogs
{
    /// <summary>
    /// Implements a button that can be hosted in a task dialog.
    /// </summary>
    public class TaskDialogButton : TaskDialogButtonBase
    {
        #region Constants and Fields

        /// <summary>
        /// </summary>
        private bool showElevationIcon;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///   Creates a new instance of this class.
        /// </summary>
        public TaskDialogButton()
        {
        }

        /// <summary>
        /// Creates a new instance of this class with the specified property settings.
        /// </summary>
        /// <param name="name">
        /// The name of the button.
        /// </param>
        /// <param name="text">
        /// The button label.
        /// </param>
        public TaskDialogButton(string name, string text)
            : base(name, text)
        {
        }

        #endregion

        #region Properties

        /// <summary>
        ///   Gets or sets a value that controls whether the elevation icon is displayed.
        /// </summary>
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