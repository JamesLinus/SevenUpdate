// ***********************************************************************
// <copyright file="HyperlinkClickedEventArgs.cs" project="System.Windows" assembly="System.Windows" solution="SevenUpdate" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// <license href="http://code.msdn.microsoft.com/WindowsAPICodePack/Project/License.aspx">Microsoft Software License</license>
// ***********************************************************************

namespace System.Windows.Dialogs
{
    /// <summary>
    ///   Defines event data associated with a HyperlinkClicked event.
    /// </summary>
    public class HyperlinkClickedEventArgs : EventArgs
    {
        #region Constructors and Destructors

        /// <summary>
        ///   Initializes a new instance of the <see cref="HyperlinkClickedEventArgs" /> class.
        /// </summary>
        /// <param name="link">
        ///   The text of the hyperlink that was clicked.
        /// </param>
        public HyperlinkClickedEventArgs(string link)
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