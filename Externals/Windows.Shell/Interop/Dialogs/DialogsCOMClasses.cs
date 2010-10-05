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
    using System.Runtime.InteropServices;

    using Microsoft.Windows.Shell;

    // Dummy base interface for CommonFileDialog coclasses.
    /// <summary>
    /// </summary>
    internal interface INativeCommonFileDialog
    {
    }

    // Coclass interfaces - designed to "look like" the object 
    // in the API, so that the 'new' operator can be used in a 
    // straightforward way. Behind the scenes, the C# compiler
    // morphs all 'new CoClass()' calls to 'new CoClassWrapper()'.

    /// <summary>
    /// </summary>
    [ComImport]
    [Guid(ShellIidGuid.IFileOpenDialog)]
    [CoClass(typeof(FileOpenDialogRcw))]
    internal interface INativeFileOpenDialog : IFileOpenDialog
    {
    }

    /// <summary>
    /// </summary>
    [ComImport]
    [Guid(ShellIidGuid.IFileSaveDialog)]
    [CoClass(typeof(FileSaveDialogRcw))]
    internal interface INativeFileSaveDialog : IFileSaveDialog
    {
    }

    // .NET classes representing runtime callable wrappers.
    /// <summary>
    /// </summary>
    [ComImport]
    [ClassInterface(ClassInterfaceType.None)]
    [TypeLibType(TypeLibTypeFlags.FCanCreate)]
    [Guid(ShellClsidGuid.FileOpenDialog)]
    internal class FileOpenDialogRcw
    {
    }

    /// <summary>
    /// </summary>
    [ComImport]
    [ClassInterface(ClassInterfaceType.None)]
    [TypeLibType(TypeLibTypeFlags.FCanCreate)]
    [Guid(ShellClsidGuid.FileSaveDialog)]
    internal class FileSaveDialogRcw
    {
    }
}