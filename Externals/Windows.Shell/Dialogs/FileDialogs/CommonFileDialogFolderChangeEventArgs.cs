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
    /// Creates the event data associated with <see cref="CommonFileDialog.FolderChanging"/> event.
    /// </summary>
    public class CommonFileDialogFolderChangeEventArgs : CancelEventArgs
    {
        #region Constructors and Destructors

        /// <summary>
        /// Creates a new instance of this class.
        /// </summary>
        /// <param name="folder">
        /// The name of the folder.
        /// </param>
        public CommonFileDialogFolderChangeEventArgs(string folder)
        {
            this.Folder = folder;
        }

        #endregion

        #region Properties

        /// <summary>
        ///   Gets or sets the name of the folder.
        /// </summary>
        public string Folder { get; set; }

        #endregion
    }
}