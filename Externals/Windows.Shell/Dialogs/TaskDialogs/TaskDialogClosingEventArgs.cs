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
    using System.ComponentModel;

    /// <summary>
    /// Data associated with <see cref="TaskDialog.Closing"/> event.
    /// </summary>
    public class TaskDialogClosingEventArgs : CancelEventArgs
    {
        #region Properties

        /// <summary>
        ///   Gets or sets the text of the custom button that was clicked.
        /// </summary>
        public string CustomButton { get; set; }

        /// <summary>
        ///   Gets or sets the standard button that was clicked.
        /// </summary>
        public TaskDialogResult TaskDialogResult { get; set; }

        #endregion
    }
}