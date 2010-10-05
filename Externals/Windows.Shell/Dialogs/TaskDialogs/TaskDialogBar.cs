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
    /// Defines a common class for all task dialog bar controls, such as the progress and marquee bars.
    /// </summary>
    public class TaskDialogBar : TaskDialogControl
    {
        #region Constants and Fields

        /// <summary>
        /// </summary>
        private TaskDialogProgressBarState state;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///   Creates a new instance of this class.
        /// </summary>
        public TaskDialogBar()
        {
        }

        /// <summary>
        /// Creates a new instance of this class with the specified name.
        /// </summary>
        /// <param name="name">
        /// The name for this control.
        /// </param>
        protected TaskDialogBar(string name)
            : base(name)
        {
        }

        #endregion

        #region Properties

        /// <summary>
        ///   Gets or sets the state of the progress bar.
        /// </summary>
        public TaskDialogProgressBarState State
        {
            get
            {
                return this.state;
            }

            set
            {
                this.CheckPropertyChangeAllowed("State");
                this.state = value;
                this.ApplyPropertyChange("State");
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Resets the state of the control to normal.
        /// </summary>
        protected internal virtual void Reset()
        {
            this.state = TaskDialogProgressBarState.Normal;
        }

        #endregion
    }
}