// ***********************************************************************
// <copyright file="BGJobType.cs"
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
    /// <summary>The BG_JOB_TYPE enumeration type defines constant values that you use to specify the type of transfer job, such as download</summary>
    internal enum BGJobType
    {
        /// <summary>Specifies that the job downloads files to the client</summary>
        Download = 0, 

        /// <summary>Specifies that the job uploads a file to the server</summary>
        Upload = 1, 

        /// <summary>Specifies that the job uploads a file to the server and receives a reply file from the server application.</summary>
        UploadReply = 2, 

        /// <summary>This is not provided by BITS but is Custom</summary>
        Unknown, 
    }
}