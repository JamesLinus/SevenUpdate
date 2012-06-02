// <copyright file="BGJobType.cs" project="SharpBits.Base">Xidar</copyright>
// <license href="http://sharpbits.codeplex.com/license" name="BSD License" />

namespace SharpBits.Base
{
    /// <summary>
    ///   The BG_JOB_TYPE enumeration type defines constant values that you use to specify the type of transfer job,
    ///   such as download.
    /// </summary>
    internal enum BGJobType
    {
        /// <summary>Specifies that the job downloads files to the client.</summary>
        Download = 0, 

        /// <summary>Specifies that the job uploads a file to the server.</summary>
        Upload = 1, 

        /// <summary>Specifies that the job uploads a file to the server and receives a reply file from the server application.</summary>
        UploadReply = 2, 

        /// <summary>This is not provided by BITS but is Custom.</summary>
        Unknown, 
    }
}