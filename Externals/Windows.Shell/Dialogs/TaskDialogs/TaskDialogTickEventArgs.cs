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
    using System;

    /// <summary>
    /// The event data for a TaskDialogTick event.
    /// </summary>
    public class TaskDialogTickEventArgs : EventArgs
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes the data associated with the TaskDialog tick event.
        /// </summary>
        /// <param name="totalTicks">
        /// The total number of ticks since the control was activated.
        /// </param>
        public TaskDialogTickEventArgs(int totalTicks)
        {
            this.Ticks = totalTicks;
        }

        #endregion

        #region Properties

        /// <summary>
        ///   Gets a value that determines the current number of ticks.
        /// </summary>
        public int Ticks { get; private set; }

        #endregion
    }
}