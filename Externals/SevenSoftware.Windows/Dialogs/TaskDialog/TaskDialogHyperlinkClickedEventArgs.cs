// ***********************************************************************
// <copyright file="TaskDialogHyperlinkClickedEventArgs.cs" project="SevenSoftware.Windows" assembly="SevenSoftware.Windows" solution="SevenUpdate" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// <license href="http://code.msdn.microsoft.com/WindowsAPICodePack/Project/License.aspx">Microsoft Software License</license>
// ***********************************************************************

namespace SevenSoftware.Windows.Dialogs.TaskDialog
{
    using System;

    /// <summary>Defines event data associated with a HyperlinkClick event.</summary>
    public class TaskDialogHyperlinkClickedEventArgs : EventArgs
    {
        /// <summary>
        ///   Initializes a new instance of the <see cref="TaskDialogHyperlinkClickedEventArgs" /> class. Creates a new
        ///   instance of this class with the specified link text.
        /// </summary>
        /// <param name="linkText">The text of the hyperlink that was clicked.</param>
        public TaskDialogHyperlinkClickedEventArgs(string linkText)
        {
            this.LinkText = linkText;
        }

        /// <summary>Gets or sets the text of the hyperlink that was clicked.</summary>
        public string LinkText { get; set; }
    }
}