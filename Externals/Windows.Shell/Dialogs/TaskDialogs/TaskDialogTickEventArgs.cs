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
    /// The event data for a TaskDialogTick event.
    /// </summary>
    public class TaskDialogTickEventArgs : EventArgs
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskDialogTickEventArgs"/> class.
        /// </summary>
        /// <parameter name="totalTicks">
        /// The total number of ticks since the control was activated.
        /// </parameter>
        public TaskDialogTickEventArgs(int totalTicks)
        {
            this.Ticks = totalTicks;
        }

        #endregion

        #region Properties

        /// <summary>
        ///   Gets  a value indicating whether the current number of ticks.
        /// </summary>
        public int Ticks { get; private set; }

        #endregion
    }
}