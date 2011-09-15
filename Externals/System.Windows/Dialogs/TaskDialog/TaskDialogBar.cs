// ***********************************************************************
// <copyright file="TaskDialogBar.cs" project="System.Windows" assembly="System.Windows" solution="SevenUpdate" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// <license href="http://code.msdn.microsoft.com/WindowsAPICodePack/Project/License.aspx">Microsoft Software License</license>
// ***********************************************************************

namespace System.Windows.Dialogs.TaskDialog
{
    /// <summary>Defines a common class for all task dialog bar controls, such as the progress and marquee bars.</summary>
    public class TaskDialogBar : TaskDialogControl
    {
        #region Constants and Fields

        /// <summary>The state of the progressbar.</summary>
        private TaskDialogProgressBarState state;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the <see cref="TaskDialogBar" /> class. Creates a new instance of this
        ///   class.
        /// </summary>
        public TaskDialogBar()
        {
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="TaskDialogBar" /> class. Creates a new instance of this class
        ///   with the specified name.
        /// </summary>
        /// <param name="name">The name for this control.</param>
        protected TaskDialogBar(string name)
            : base(name)
        {
        }

        #endregion

        #region Public Properties

        /// <summary>Gets or sets the state of the progress bar.</summary>
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

        /// <summary>Resets the state of the control to normal.</summary>
        protected internal virtual void Reset()
        {
            this.state = TaskDialogProgressBarState.Normal;
        }

        #endregion
    }
}
