// ***********************************************************************
// <copyright file="IBackgroundCopyManager.cs" project="SharpBits.Base" assembly="SharpBits.Base" solution="SevenUpdate" company="Xidar Solutions">
//     Copyright (c) xidar solutions. All rights reserved.
// </copyright>
// <author username="xidar">xidar</author>
// <author username="sevenalive">Robert Baker</author>
// <license href="http://sharpbits.codeplex.com/license">BSD License</license> 
// ***********************************************************************

namespace SharpBits.Base
{
    using System;
    using System.Runtime.InteropServices;

    /// <summary>
    ///   Use the IBackgroundCopyManager interface to create transfer jobs,retrieve an enumerator object that contains
    ///   the jobs in the queue, and to retrieve individual jobs from the queue.
    /// </summary>
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("5CE34C0D-0DC9-4C1F-897C-DAA1B78CEE7C")]
    [ComImport]
    internal interface IBackgroundCopyManager
    {
        /// <summary>Creates a new transfer job.</summary>
        /// <param name="displayName">A string that contains a display name for the job. Typically, the display name is used to identify the job in a user interface. Note that more than one job may have the same display name. Must not be <c>null</c>. The name is limited to 256 characters, not including the <c>null</c> terminator.</param>
        /// <param name="type">Type of transfer job, such as BGJob_TYPE_DOWNLOAD. For a list of transfer types, see the BGJob_TYPE enumeration.</param>
        /// <param name="jobId">Uniquely identifies your job in the queue. Use this identifier when you call the <c>IBackgroundCopyManager</c>::<c>GetJob</c> method to get a job from the queue.</param>
        /// <param name="job">An <c>IBackgroundCopyJob</c> interface pointer that you use to modify the job's properties
        /// and specify the files to be transferred. To activate the job in the queue, call the <see
        /// cref="IBackgroundCopyJob" />::Resume method. Release <paramref name="job" /> when done.</param>
        void CreateJob([MarshalAs(UnmanagedType.LPWStr)] string displayName, 
                       BGJobType type, 
                       out Guid jobId, 
                       [MarshalAs(UnmanagedType.Interface)] out IBackgroundCopyJob job);

        /// <summary>Retrieves a given job from the queue.</summary>
        /// <param name="jobId">Identifies the job to retrieve from the transfer queue. The <c>CreateJob</c> method returns the job identifier.</param>
        /// <param name="job">An <c>IBackgroundCopyJob</c> interface pointer to the job specified by JobID. When done, release job.</param>
        void GetJob(ref Guid jobId, [MarshalAs(UnmanagedType.Interface)] out IBackgroundCopyJob job);

        /// <summary>Retrieves an enumerator object that you use to enumerate jobs in the queue.</summary>
        /// <param name="flags">Specifies whose jobs to include in the enumeration. If <paramref name="flags" /> is set to 0, the user receives all jobs that they own in the transfer queue. The following table lists the enumeration options.</param>
        /// <param name="enum">An <c>IEnumBackgroundCopyJobs</c> interface pointer that you use to enumerate the jobs in
        /// the transfer queue. The contents of the enumerator depend on the value of <paramref name="flags" />. Release
        /// p@enumJobs when done.</param>
        void EnumJobs(uint flags, [MarshalAs(UnmanagedType.Interface)] out IEnumBackgroundCopyJobs @enum);

        /// <summary>Retrieves a description for the given error code.</summary>
        /// <param name="result">Error code from a previous call to a BITS method.</param>
        /// <param name="languageId">Identifies the language identifier to use to generate the description. To create the language identifier, use the MAKELANGID macro.</param>
        /// <param name="errorDescription">A string that contains a description of the error. Call the CoTaskMemFree
        /// function to free <paramref name="errorDescription" /> when done.</param>
        void GetErrorDescription([MarshalAs(UnmanagedType.Error)] int result, 
                                 uint languageId, 
                                 [MarshalAs(UnmanagedType.LPWStr)] out string errorDescription);
    }
}