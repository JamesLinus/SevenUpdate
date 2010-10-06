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
    using System;

    /// <summary>
    /// The event data for a TaskDialogTick event.
    /// </summary>
    public class TaskDialogTickEventArgs : EventArgs
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskDialogTickEventArgs"/> class.
        /// </summary>
        /// <param name="totalTicks">The total number of ticks since the control was activated.</param>
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