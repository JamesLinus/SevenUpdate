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
        /// <param name="link">
        /// The text of the hyperlink that was clicked.
        /// </param>
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