// ***********************************************************************
// Assembly         : System.Windows
// Author           : Microsoft Corporation
// Last Modified By : Robert Baker (sevenalive)
// Last Modified On : 10-06-2010
// Copyright        : (c) Microsoft Corporation. All rights reserved.
// ***********************************************************************
namespace System.Windows.Dialogs.TaskDialogs
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