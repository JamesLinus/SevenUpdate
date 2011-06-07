// ***********************************************************************
// <copyright file="TaskDialogTickEventArgs.cs" project="System.Windows" assembly="System.Windows" solution="SevenUpdate" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// <license href="http://code.msdn.microsoft.com/WindowsAPICodePack/Project/License.aspx">Microsoft Software License</license>
// ***********************************************************************

namespace System.Windows.Dialogs
{
    /// <summary>
    ///   The event data for a TaskDialogTick event.
    /// </summary>
    public class TaskDialogTickEventArgs : EventArgs
    {
        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the <see cref="TaskDialogTickEventArgs" /> class.
        /// </summary>
        /// <param name="totalTicks">
        ///   The total number of ticks since the control was activated.
        /// </param>
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