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
    /// Represents the base class for all types of folders (filesystem and non filesystem)
    /// </summary>
    [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", 
        Justification = "This will complicate the class hierarchy and naming convention used in the Shell area")]
    public abstract class ShellFolder : ShellContainer
    {
    }
}