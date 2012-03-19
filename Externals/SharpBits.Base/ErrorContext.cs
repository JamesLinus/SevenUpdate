// <copyright file="ErrorContext.cs" project="SharpBits.Base">Xidar</copyright>
// <license href="http://sharpbits.codeplex.com/license" name="BSD License" />

namespace SharpBits.Base
{
    /// <summary>Specifies the error.</summary>
    public enum ErrorContext
    {
        /// <summary>No error occurred.</summary>
        None = 0, 

        /// <summary>Unknown error.</summary>
        UnknownError = 1, 

        /// <summary>The general queue manager error.</summary>
        GeneralQueueManagerError = 2, 

        /// <summary>The general notification error.</summary>
        QueueManagerNotificationError = 3, 

        /// <summary>A local file error.</summary>
        LocalFileError = 4, 

        /// <summary>An error with the download file.</summary>
        RemoteFileError = 5, 

        /// <summary>An error while the file is transferring.</summary>
        GeneralTransportError = 6, 

        /// <summary>A remote program/server error.</summary>
        RemoteApplicationError = 7, 
    }
}