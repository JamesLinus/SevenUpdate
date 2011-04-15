// ***********************************************************************
// <copyright file="BackgroundCopyManager.cs"
//            project="SharpBits.Base"
//            assembly="SharpBits.Base"
//            solution="SevenUpdate"
//            company="Xidar Solutions">
//     Copyright (c) xidar solutions. All rights reserved.
// </copyright>
// <author username="xidar">xidar</author>
// <author username="sevenalive">Robert Baker</author>
// <license href="http://sharpbits.codeplex.com/license">BSD License</license> 
// ***********************************************************************
namespace SharpBits.Base
{
    using System.Runtime.InteropServices;
    using System.Security.Permissions;

    /// <summary>Entry point to the BITS infrastructure.</summary>
    [Guid("4991D34B-80A1-4291-83B6-3328366B9097"), ClassInterfaceAttribute(ClassInterfaceType.None), ComImportAttribute, SecurityPermission(SecurityAction.Demand, UnmanagedCode = true)]
    internal class BackgroundCopyManager
    {
    }
}