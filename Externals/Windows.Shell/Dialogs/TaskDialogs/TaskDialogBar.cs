// ***********************************************************************
// Assembly         : Windows.Shell
// Author           : Microsoft
// Created          : 09-17-2010
// Last Modified By : sevenalive (Robert Baker)
// Last Modified On : 10-05-2010
// Description      : 
// Copyright        : (c) Microsoft Corporation. All rights reserved.
// ***********************************************************************

namespace Microsoft.Windows.Dialogs.TaskDialogs
{
    using System.Windows.Controls;

    /// <summary>
    /// Defines a common class for all task dialog bar controls, such as the progress and marquee bars.
    /// </summary>
    public class TaskDialogBar : TaskDialogControl
    {
        #region Constants and Fields

        /// <summary>
        /// The <see cref="ProgressBar"/> State
        /// </summary>
        private TaskDialogProgressBarState state;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskDialogBar"/> class.
        /// </summary>
        public TaskDialogBar()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskDialogBar"/> class.
        /// </summary>
        /// <param name="name">The name for this control.</param>
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