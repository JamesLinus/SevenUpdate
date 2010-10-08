// ***********************************************************************
// <copyright file="TaskDialogClosingEventArgs.cs"
//            project="System.Windows"
//            assembly="System.Windows"
//            solution="SevenUpdate"
//            company="Seven Software">
//     Copyright (c) Seven Software. All rights reserved.
// </copyright>
// <author username="sevenalive">Robert Baker</author>
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