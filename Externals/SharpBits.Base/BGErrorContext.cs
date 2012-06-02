// <copyright file="BGErrorContext.cs" project="SharpBits.Base">Xidar</copyright>
// <license href="http://sharpbits.codeplex.com/license" name="BSD License" />

namespace SharpBits.Base
{
    /// <summary>
    ///   The BG_ERROR_CONTEXT enumeration type defines the constant values that specify the context in which the error
    ///   occurred.
    /// </summary>
    internal enum BGErrorContext
    {
        /// <summary>An error has not occurred.</summary>
        None = 0, 

        /// <summary>The error context is unknown.</summary>
        Unknown = 1, 

        /// <summary>The transfer queue manager generated the error.</summary>
        GeneralQueueManager = 2, 

        /// <summary>The error was generated while the queue manager was notifying the client of an event.</summary>
        QueueManagerNotification = 3, 

        /// <summary>
        ///   The error was related to the specified local file. For example, permission was denied or the volume was
        ///   unavailable.
        /// </summary>
        LocalFile = 4, 

        /// <summary>The error was related to the specified remote file. For example, the Url is not accessible.</summary>
        RemoteFile = 5, 

        /// <summary>
        ///   The transport layer generated the error. These errors are general transport failures; errors not specific
        ///   to the remote file.
        /// </summary>
        GeneralTransport = 6, 
    }
}