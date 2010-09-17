//Copyright (c) Microsoft Corporation.  All rights reserved.
//Modified by Robert Baker, Seven Software 2010.

#region

using System;

#endregion

namespace Microsoft.Windows.Dialogs
{
    /// <summary>
    ///   The event data for a TaskDialogTick event.
    /// </summary>
    public class TaskDialogTickEventArgs : EventArgs
    {
        /// <summary>
        ///   Initializes the data associated with the TaskDialog tick event.
        /// </summary>
        /// <param name = "totalTicks">The total number of ticks since the control was activated.</param>
        public TaskDialogTickEventArgs(int totalTicks)
        {
            Ticks = totalTicks;
        }

        /// <summary>
        ///   Gets a value that determines the current number of ticks.
        /// </summary>
        public int Ticks { get; private set; }
    }
}