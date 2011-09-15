// ***********************************************************************
// <copyright file="JobType.cs" project="SharpBits.Base" assembly="SharpBits.Base" solution="SevenUpdate" company="Xidar Solutions">
//     Copyright (c) xidar solutions. All rights reserved.
// </copyright>
// <author username="xidar">xidar</author>
// <author username="sevenalive">Robert Baker</author>
// <license href="http://sharpbits.codeplex.com/license">BSD License</license> 
// ***********************************************************************

namespace SharpBits.Base
{
    /// <summary>The type of <c>BitsJob</c>.</summary>
    public enum JobType
    {
        /// <summary>Downloads a file.</summary>
        Download, 

        /// <summary>Uploads a file without progress.</summary>
        Upload, 

        /// <summary>Uploads a file and reply's with progress.</summary>
        UploadReply, 

        /// <summary>Unknown job.</summary>
        Unknown, 
    }
}
