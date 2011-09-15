// ***********************************************************************
// <copyright file="TaskDialogTickEventArgs.cs" project="System.Windows" assembly="System.Windows" solution="SevenUpdate" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// <license href="http://code.msdn.microsoft.com/WindowsAPICodePack/Project/License.aspx">Microsoft Software License</license>
// ***********************************************************************

namespace System.Windows.Dialogs.TaskDialog
{
    /// <summary>The event data for a TaskDialogTick event.</summary>
    public class TaskDialogTickEventArgs : EventArgs
    {
        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref="TaskDialogTickEventArgs" /> class.</summary>
        /// <param name="ticks">The total number of ticks since the control was activated.</param>
        public TaskDialogTickEventArgs(int ticks)
        {
            this.Ticks = ticks;
        }

        #endregion

        #region Public Properties

        /// <summary>Gets a value that determines the current number of ticks.</summary>
        public int Ticks { get; private set; }

        #endregion
    }
}
