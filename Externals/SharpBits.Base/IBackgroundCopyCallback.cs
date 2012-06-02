// <copyright file="IBackgroundCopyCallback.cs" project="SharpBits.Base">Xidar</copyright>
// <license href="http://sharpbits.codeplex.com/license" name="BSD License" />

namespace SharpBits.Base
{
    using System.Runtime.InteropServices;

    /// <summary>
    ///   Implement the IBackgroundCopyCallback interface to receive notification that a job is complete, has been
    ///   modified, or is in error. Clients use this interface instead of polling for the status of the job.
    /// </summary>
    [ComImport]
    [Guid("97EA99C7-0186-4AD4-8DF9-C5B4E0ED6B22")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IBackgroundCopyCallback
    {
        /// <summary>Called when all of the files in the job have successfully transferred.</summary>
        /// <param name="job">Contains job-related information, such as the time the job completed, the number of bytes transferred, and the number of files transferred. Do not release job; BITS releases the interface when the method returns.</param>
        void JobTransferred([MarshalAs(UnmanagedType.Interface)] IBackgroundCopyJob job);

        /// <summary>Called when an error occurs.</summary>
        /// <param name="job">Contains job-related information, such as the number of bytes and files transferred before the error occurred. It also contains the methods to resume and cancel the job. Do not release job; BITS releases the interface when the JobError method returns.</param>
        /// <param name="error">Contains error information, such as the file being processed at the time the fatal error occurred and a description of the error. Do not release error; BITS releases the interface when the JobError method returns.</param>
        void JobError(
            [MarshalAs(UnmanagedType.Interface)] IBackgroundCopyJob job, 
            [MarshalAs(UnmanagedType.Interface)] IBackgroundCopyError error);

        /// <summary>Called when a job is modified.</summary>
        /// <param name="job">Contains the methods for accessing property, progress, and state information of the job. Do not release job; BITS releases the interface when the JobModification method returns.</param>
        /// <param name="reserved">Reserved for future use.</param>
        void JobModification([MarshalAs(UnmanagedType.Interface)] IBackgroundCopyJob job, uint reserved);
    }
}