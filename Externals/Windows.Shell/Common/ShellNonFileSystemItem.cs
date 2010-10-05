//***********************************************************************
// Assembly         : Windows.Shell
// Author           : sevenalive
// Created          : 09-17-2010
// Last Modified By : sevenalive
// Last Modified On : 10-05-2010
// Description      : 
// Copyright        : (c) Seven Software. All rights reserved.
//***********************************************************************

namespace Microsoft.Windows.Shell
{
    /// <summary>
    /// Represents a non filesystem item (e.g. virtual items inside Control Panel)
    /// </summary>
    public class ShellNonFileSystemItem : ShellObjectNode
    {
        #region Constructors and Destructors

        /// <summary>
        /// </summary>
        /// <param name="shellItem">
        /// </param>
        internal ShellNonFileSystemItem(IShellItem2 shellItem)
        {
            this.nativeShellItem = shellItem;
        }

        #endregion
    }
}