//Copyright (c) Microsoft Corporation.  All rights reserved.
//Modified by Robert Baker, Seven Software 2010.

#region

using System.Diagnostics.CodeAnalysis;

#endregion

namespace Microsoft.Windows.Shell
{
    /// <summary>
    ///   Represents the base class for all types of folders (filesystem and non filesystem)
    /// </summary>
    [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "This will complicate the class hierarchy and naming convention used in the Shell area")]
    public abstract class ShellFolder : ShellContainer
    {
    }
}