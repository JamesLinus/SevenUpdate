// <copyright file="JobType.cs" project="SharpBits.Base">Xidar</copyright>
// <license href="http://sharpbits.codeplex.com/license" name="BSD License" />

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