// ***********************************************************************
// <copyright file="BGFileAclFlags.cs"
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
    using System;

    /// <summary>The ACL's of the file to set when downloaded</summary>
    [Flags]
    internal enum BGFileAclFlags
    {
        /// <summary>Set current owner</summary>
        BGCopyFileOwner = 0x0001, 

        /// <summary>Set current group</summary>
        BGCopyFileGroup = 0x0002, 

        /// <summary>Delete all ACL lists</summary>
        BGCopyDestinationFileAcl = 0x0004, 

        /// <summary>Give special permissions</summary>
        BGCopySourceFileAcl = 0x0008, 

        /// <summary>Inherit all lists</summary>
        BGCopyFileAll = 0x0015, 
    }
}