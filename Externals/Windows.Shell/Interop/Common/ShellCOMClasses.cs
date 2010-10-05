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
    using System.Runtime.InteropServices;

    /// <summary>
    /// </summary>
    [ComImport]
    [Guid(ShellIidGuid.IShellLibrary)]
    [CoClass(typeof(ShellLibraryCOClass))]
    internal interface INativeShellLibrary : IShellLibrary
    {
    }

    /// <summary>
    /// </summary>
    [ComImport]
    [ClassInterface(ClassInterfaceType.None)]
    [TypeLibType(TypeLibTypeFlags.FCanCreate)]
    [Guid(ShellClsidGuid.ShellLibrary)]
    internal class ShellLibraryCOClass
    {
    }
}