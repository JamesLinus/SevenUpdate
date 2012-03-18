// ***********************************************************************
// <copyright file="TaskDialogBar.cs" project="SevenSoftware.Windows" assembly="SevenSoftware.Windows" solution="SevenUpdate" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// <license href="http://code.msdn.microsoft.com/WindowsAPICodePack/Project/License.aspx">Microsoft Software License</license>
// ***********************************************************************

namespace SevenSoftware.Windows.Dialogs.TaskDialog
{
    /// <summary>Defines a common class for all task dialog bar controls, such as the progress and marquee bars.</summary>
    public class TaskDialogBar : TaskDialogControl
    {
        /// <summary>The state of the progressbar.</summary>
        private TaskDialogProgressBarState state;

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
        protected TaskDialogBar(string name) : base(name)
        {
        }

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

        /// <summary>Resets the state of the control to normal.</summary>
        protected internal virtual void Reset()
        {
            this.state = TaskDialogProgressBarState.Normal;
        }
    }
}