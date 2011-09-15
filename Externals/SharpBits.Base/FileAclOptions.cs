// ***********************************************************************
// <copyright file="FileAclOptions.cs" project="SharpBits.Base" assembly="SharpBits.Base" solution="SevenUpdate" company="Xidar Solutions">
//     Copyright (c) xidar solutions. All rights reserved.
// </copyright>
// <author username="xidar">xidar</author>
// <author username="sevenalive">Robert Baker</author>
// <license href="http://sharpbits.codeplex.com/license">BSD License</license> 

namespace SharpBits.Base
{
    using System;

    /// <summary>Identifies the owner and ACL information to maintain when transferring a file using SMB.</summary>
    [Flags]
    public enum FileAclOptions
    {
        /// <summary>
        ///   If set, the file's owner information is maintained. Otherwise, the job's owner becomes the owner of the
        ///   file.
        /// </summary>
        CopyFileOwner = 1, 

        /// <summary>
        ///   If set, the file's group information is maintained. Otherwise, BITS uses the job owner's primary group to
        ///   assign the group information to the file.
        /// </summary>
        CopyFileGroup = 2, 

        /// <summary>
        ///   If set, BITS copies the explicit ACEs from the source file and inheritable ACEs from the destination
        ///   parent folder. Otherwise, BITS copies the inheritable ACEs from the destination parent folder. If the
        ///   parent folder does not contain inheritable ACEs, BITS uses the default DACL from the account.
        /// </summary>
        CopyDestinationFileAcl = 4, 

        /// <summary>
        ///   If set, BITS copies the explicit ACEs from the source file and inheritable ACEs from the destination
        ///   parent folder. Otherwise, BITS copies the inheritable ACEs from the destination parent folder.
        /// </summary>
        CopySourceFileAcl = 8, 

        /// <summary>If set, BITS copies the owner and ACL information. This is the same as setting all the flags individually.</summary>
        CopyFileAll = 15, 
    }
}
