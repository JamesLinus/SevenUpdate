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
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Represents a Non FileSystem folder (e.g. My Computer, Control Panel)
    /// </summary>
    [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", 
        Justification = "This will complicate the class hierarchy and naming convention used in the Shell area")]
    public class ShellNonFileSystemFolder : ShellFolder
    {
        #region Constructors and Destructors

        /// <summary>
        /// </summary>
        internal ShellNonFileSystemFolder()
        {
        }

        /// <summary>
        /// </summary>
        /// <param name="shellItem">
        /// </param>
        internal ShellNonFileSystemFolder(IShellItem2 shellItem)
        {
            this.nativeShellItem = shellItem;
        }

        #endregion
    }
}