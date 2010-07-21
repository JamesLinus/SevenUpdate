// Copyright (c) Microsoft Corporation.  All rights reserved.

#region

using System.Diagnostics.CodeAnalysis;

#endregion

namespace Microsoft.Windows.Shell
{
    /// <summary>
    ///   Represents a Non FileSystem folder (e.g. My Computer, Control Panel)
    /// </summary>
    [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "This will complicate the class hierarchy and naming convention used in the Shell area")]
    public class ShellNonFileSystemFolder : ShellFolder
    {
        #region Internal Constructors

        internal ShellNonFileSystemFolder()
        {
        }

        internal ShellNonFileSystemFolder(IShellItem2 shellItem)
        {
            nativeShellItem = shellItem;
        }

        #endregion
    }
}