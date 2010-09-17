//Copyright (c) Microsoft Corporation.  All rights reserved.
//Modified by Robert Baker, Seven Software 2010.
namespace Microsoft.Windows.Shell
{
    /// <summary>
    ///   Represents a non filesystem item (e.g. virtual items inside Control Panel)
    /// </summary>
    public class ShellNonFileSystemItem : ShellObjectNode
    {
        internal ShellNonFileSystemItem(IShellItem2 shellItem)
        {
            nativeShellItem = shellItem;
        }
    }
}