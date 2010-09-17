//Copyright (c) Microsoft Corporation.  All rights reserved.
//Modified by Robert Baker, Seven Software 2010.

#region

using System;

#endregion

namespace Microsoft.Windows.Dialogs
{
    /// <summary>
    ///   Defines event data associated with a HyperlinkClick event.
    /// </summary>
    public class TaskDialogHyperlinkClickedEventArgs : EventArgs
    {
        /// <summary>
        ///   Creates a new instance of this class with the specified link text.
        /// </summary>
        /// <param name = "link">The text of the hyperlink that was clicked.</param>
        public TaskDialogHyperlinkClickedEventArgs(string link)
        {
            LinkText = link;
        }

        /// <summary>
        ///   Gets or sets the text of the hyperlink that was clicked.
        /// </summary>
        public string LinkText { get; set; }
    }
}