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
    /// Defines event data associated with a HyperlinkClick event.
    /// </summary>
    public class TaskDialogHyperlinkClickedEventArgs : EventArgs
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskDialogHyperlinkClickedEventArgs"/> class.
        /// </summary>
        /// <parameter name="link">
        /// The text of the hyperlink that was clicked.
        /// </parameter>
        public TaskDialogHyperlinkClickedEventArgs(string link)
        {
            this.LinkText = link;
        }

        #endregion

        #region Properties

        /// <summary>
        ///   Gets or sets the text of the hyperlink that was clicked.
        /// </summary>
        public string LinkText { get; set; }

        #endregion
    }
}