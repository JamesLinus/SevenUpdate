// <copyright file="BackgroundCopyManager.cs" project="SharpBits.Base">Xidar</copyright>
// <license href="http://sharpbits.codeplex.com/license" name="BSD License" />

namespace SharpBits.Base
{
    using System.Runtime.InteropServices;
    using System.Security.Permissions;

    /// <summary>Entry point to the BITS infrastructure.</summary>
    [Guid("4991D34B-80A1-4291-83B6-3328366B9097")]
    [ClassInterface(ClassInterfaceType.None)]
    [ComImport]
    [SecurityPermission(SecurityAction.Demand, UnmanagedCode = true)]
    internal class BackgroundCopyManager
    {
    }
}