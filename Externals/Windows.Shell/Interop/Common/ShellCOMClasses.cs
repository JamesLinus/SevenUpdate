//Copyright (c) Microsoft Corporation.  All rights reserved.
//Modified by Robert Baker, Seven Software 2010.

#region

using System.Runtime.InteropServices;

#endregion

namespace Microsoft.Windows.Shell
{
    [ComImport, Guid(ShellIIDGuid.IShellLibrary), CoClass(typeof (ShellLibraryCoClass))]
    internal interface INativeShellLibrary : IShellLibrary
    {
    }

    [ComImport, ClassInterface(ClassInterfaceType.None), TypeLibType(TypeLibTypeFlags.FCanCreate), Guid(ShellCLSIDGuid.ShellLibrary)]
    internal class ShellLibraryCoClass
    {
    }
}