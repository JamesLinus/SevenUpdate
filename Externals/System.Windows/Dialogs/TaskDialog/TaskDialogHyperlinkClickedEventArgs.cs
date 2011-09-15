// ***********************************************************************
// <copyright file="TaskDialogHyperlinkClickedEventArgs.cs" project="System.Windows" assembly="System.Windows" solution="SevenUpdate" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// <license href="http://code.msdn.microsoft.com/WindowsAPICodePack/Project/License.aspx">Microsoft Software License</license>
// ***********************************************************************

namespace System.Windows.Dialogs.TaskDialog
{
    /// <summary>Defines event data associated with a HyperlinkClick event.</summary>
    public class TaskDialogHyperlinkClickedEventArgs : EventArgs
    {
        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the <see cref = "TaskDialogHyperlinkClickedEventArgs" /> class. Creates a
        ///   new instance of this class with the specified link text.
        /// </summary>
        /// <param name = "linkText">The text of the hyperlink that was clicked.</param>
        public TaskDialogHyperlinkClickedEventArgs(string linkText)
        {
            this.LinkText = linkText;
        }

        #endregion

        #region Public Properties

        /// <summary>Gets or sets the text of the hyperlink that was clicked.</summary>
        public string LinkText { get; set; }

        #endregion
    }
}
