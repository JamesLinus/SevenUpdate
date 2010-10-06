// ***********************************************************************
// Assembly         : SharpBits.Base
// Author           : xidar solutions
// Created          : 09-17-2010
// Last Modified By : sevenalive (Robert Baker)
// Last Modified On : 10-05-2010
// Description      : 
// Copyright        : (c) xidar solutions. All rights reserved.
// ***********************************************************************

namespace SharpBits.Base
{
    using System;
    using System.Runtime.InteropServices;
    using System.Security.Permissions;

    #region IBackgroundCopyManager

    /// <summary>
    /// Use the IBackgroundCopyManager interface to create transfer jobs,
    ///   retrieve an enumerator object that contains the jobs in the queue, and to retrieve individual jobs from the queue.
    /// </summary>
    [InterfaceTypeAttribute(ComInterfaceType.InterfaceIsIUnknown)]
    [GuidAttribute("5CE34C0D-0DC9-4C1F-897C-DAA1B78CEE7C")]
    [ComImportAttribute]
    internal interface IBackgroundCopyManager
    {
        /// <summary>
        /// Creates a new transfer job
        /// </summary>
        /// <param name="displayName">
        /// <see langword="null"/>-terminated string that contains a display name for the job. Typically, the display name is used to identify the job in a user interface. Note that more than one job may have the same display name. Must not be <see langword="null"/>. The name is limited to 256 characters, not including the <see langword="null"/> terminator.
        /// </param>
        /// <param name="type">
        /// Type of transfer job, such as BGJob_TYPE_DOWNLOAD. For a list of transfer types, see the BGJob_TYPE enumeration.
        /// </param>
        /// <param name="jobId">
        /// Uniquely identifies your job in the queue. Use this identifier when you call the <see cref="IBackgroundCopyManager"/>::<see cref="GetJob"/> method to get a job from the queue.
        /// </param>
        /// <param name="job">
        /// An <see cref="IBackgroundCopyJob"/> interface pointer that you use to modify the job's properties and specify the files to be transferred. To activate the job in the queue, call the <see cref="IBackgroundCopyJob"/>::Resume method. Release <paramref name="job"/> when done.
        /// </param>
        void CreateJob(
            [MarshalAs(UnmanagedType.LPWStr)] string displayName, BGJobType type, out Guid jobId, [MarshalAs(UnmanagedType.Interface)] out IBackgroundCopyJob job);

        /// <summary>
        /// Retrieves a given job from the queue
        /// </summary>
        /// <param name="jobID">
        /// Identifies the job to retrieve from the transfer queue. The <see cref="CreateJob"/> method returns the job identifier.
        /// </param>
        /// <param name="job">
        /// An <see cref="IBackgroundCopyJob"/> interface pointer to the job specified by JobID. When done, release job.
        /// </param>
        void GetJob(ref Guid jobID, [MarshalAs(UnmanagedType.Interface)] out IBackgroundCopyJob job);

        /// <summary>
        /// Retrieves an enumerator object that you use to enumerate jobs in the queue
        /// </summary>
        /// <param name="flags">
        /// Specifies whose jobs to include in the enumeration. If <paramref name="flags"/> is set to 0, the user receives all jobs that they own in the transfer queue. The following table lists the enumeration options.
        /// </param>
        /// <param name="enum">
        /// An <see cref="IEnumBackgroundCopyJobs"/> interface pointer that you use to enumerate the jobs in the transfer queue. The contents of the enumerator depend on the value of <paramref name="flags"/>. Release p@enumJobs when done.
        /// </param>
        void EnumJobs(uint flags, [MarshalAs(UnmanagedType.Interface)] out IEnumBackgroundCopyJobs @enum);

        /// <summary>
        /// Retrieves a description for the given error code
        /// </summary>
        /// <param name="result">
        /// Error code from a previous call to a BITS method.
        /// </param>
        /// <param name="languageId">
        /// Identifies the language identifier to use to generate the description. To create the language identifier, use the MAKELANGID macro.
        /// </param>
        /// <param name="errorDescription">
        /// <see langword="null"/>-terminated string that contains a description of the error. Call the CoTaskMemFree function to free <paramref name="errorDescription"/> when done.
        /// </param>
        void GetErrorDescription([MarshalAs(UnmanagedType.Error)] int result, uint languageId, [MarshalAs(UnmanagedType.LPWStr)] out string errorDescription);
    }

    #endregion

    #region IBackgroundCopyCallback

    /// <summary>
    /// Implement the IBackgroundCopyCallback interface to receive notification that a job is complete, has been modified, or
    ///   is in error. Clients use this interface instead of polling for the status of the job.
    /// </summary>
    [ComImport]
    [Guid("97EA99C7-0186-4AD4-8DF9-C5B4E0ED6B22")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IBackgroundCopyCallback
    {
        /// <summary>
        /// Called when all of the files in the job have successfully transferred.
        /// </summary>
        /// <param name="job">
        /// Contains job-related information, such as the time the job completed, the number of bytes transferred, and the number of files transferred. Do not release job; BITS releases the interface when the method returns.
        /// </param>
        void JobTransferred([MarshalAs(UnmanagedType.Interface)] IBackgroundCopyJob job);

        /// <summary>
        /// Called when an error occurs.
        /// </summary>
        /// <param name="job">
        /// Contains job-related information, such as the number of bytes and files transferred before the error occurred. It also contains the methods to resume and cancel the job. Do not release job; BITS releases the interface when the JobError method returns.
        /// </param>
        /// <param name="error">
        /// Contains error information, such as the file being processed at the time the fatal error occurred and a description of the error. Do not release error; BITS releases the interface when the JobError method returns.
        /// </param>
        void JobError([MarshalAs(UnmanagedType.Interface)] IBackgroundCopyJob job, [MarshalAs(UnmanagedType.Interface)] IBackgroundCopyError error);

        /// <summary>
        /// Called when a job is modified.
        /// </summary>
        /// <param name="job">
        /// Contains the methods for accessing property, progress, and state information of the job. Do not release job; BITS releases the interface when the JobModification method returns.
        /// </param>
        /// <param name="reserved">
        /// Reserved for future use.
        /// </param>
        void JobModification([MarshalAs(UnmanagedType.Interface)] IBackgroundCopyJob job, uint reserved);
    }

    #endregion

    #region IBackgroundCopyJob

    /// <summary>
    /// Use the IBackgroundCopyJob interface to add files to the job, set the priority level of the job, determine the state
    ///   of the job, and to start and stop the job.
    /// </summary>
    [InterfaceTypeAttribute(ComInterfaceType.InterfaceIsIUnknown)]
    [GuidAttribute("37668D37-507E-4160-9316-26306D150B12")]
    [ComImportAttribute]
    internal interface IBackgroundCopyJob
    {
        /// <summary>
        /// Adds multiple files to the job
        /// </summary>
        /// <param name="fileCount">
        /// Number of elements in paFileSet.
        /// </param>
        /// <param name="fileSet">
        /// Array of <see cref="BGFileInfo"/> structures that identify the local and remote file names of the files to transfer.
        /// </param>
        void AddFileSet(uint fileCount, [MarshalAs(UnmanagedType.LPArray)] BGFileInfo[] fileSet);

        /// <summary>
        /// Adds a single file to the job
        /// </summary>
        /// <param name="remoteUrl">
        /// <see langword="null"/>-terminated string that contains the name of the file on the client. For information on specifying the local name, see the LocalName member and Remarks section of the <see cref="BGFileInfo"/> structure.
        /// </param>
        /// <param name="localName">
        /// <see langword="null"/>-terminated string that contains the name of the file on the server. For information on specifying the remote name, see the RemoteName member and Remarks section of the <see cref="BGFileInfo"/> structure.
        /// </param>
        void AddFile([MarshalAs(UnmanagedType.LPWStr)] string remoteUrl, [MarshalAs(UnmanagedType.LPWStr)] string localName);

        /// <summary>
        /// Returns an interface pointer to an enumerator
        ///   object that you use to enumerate the files in the job
        /// </summary>
        /// <param name="enum">
        /// <see cref="IEnumBackgroundCopyFiles"/> interface pointer that you use to enumerate the files in the job. Release p@enumFiles when done.
        /// </param>
        void EnumFiles([MarshalAs(UnmanagedType.Interface)] out IEnumBackgroundCopyFiles @enum);

        /// <summary>
        /// Pauses the job
        /// </summary>
        void Suspend();

        /// <summary>
        /// Restarts a suspended job
        /// </summary>
        void Resume();

        /// <summary>
        /// Cancels the job and removes temporary files from the client
        /// </summary>
        void Cancel();

        /// <summary>
        /// Ends the job and saves the transferred files on the client
        /// </summary>
        void Complete();

        /// <summary>
        /// Retrieves the identifier of the job in the queue
        /// </summary>
        /// <param name="val">
        /// GUID that identifies the job within the BITS queue.
        /// </param>
        void GetId(out Guid val);

        /// <summary>
        /// Retrieves the type of transfer being performed,
        ///   such as a file download
        /// </summary>
        /// <param name="val">
        /// Type of transfer being performed. For a list of transfer types, see the BGJob_TYPE enumeration type.
        /// </param>
        void GetType(out BGJobType val);

        /// <summary>
        /// Retrieves job-related progress information,
        ///   such as the number of bytes and files transferred
        ///   to the client
        /// </summary>
        /// <param name="val">
        /// Contains data that you can use to calculate the percentage of the job that is complete. For more information, see <see cref="BGJobProgress"/>.
        /// </param>
        void GetProgress(out BGJobProgress val);

        /// <summary>
        /// Retrieves timestamps for activities related
        ///   to the job, such as the time the job was created
        /// </summary>
        /// <param name="val">
        /// Contains job-related time stamps. For available time stamps, see the <see cref="BGJobTimes"/> structure.
        /// </param>
        void GetTimes(out BGJobTimes val);

        /// <summary>
        /// Retrieves the state of the job
        /// </summary>
        /// <param name="val">
        /// Current state of the job. For example, the state reflects whether the job is in error, transferring data, or suspended. For a list of job states, see the <see cref="BGJobState"/> enumeration type. 
        /// </param>
        void GetState(out BGJobState val);

        /// <summary>
        /// Retrieves an interface pointer to
        ///   the error object after an error occurs
        /// </summary>
        /// <param name="error">
        /// Error interface that provides the error code, a description of the error, and the context in which the error occurred. This parameter also identifies the file being transferred at the time the error occurred. Release error when done.
        /// </param>
        void GetError([MarshalAs(UnmanagedType.Interface)] out IBackgroundCopyError error);

        /// <summary>
        /// Retrieves the job owner's identity
        /// </summary>
        /// <param name="val">
        /// <see langword="null"/>-terminated string that contains the string version of the SID that identifies the job's owner. Call the CoTaskMemFree function to free ppOwner when done. 
        /// </param>
        void GetOwner([MarshalAs(UnmanagedType.LPWStr)] out string val);

        /// <summary>
        /// Specifies a display name that identifies the job in
        ///   a user interface
        /// </summary>
        /// <param name="val">
        /// <see langword="null"/>-terminated string that identifies the job. Must not be <see langword="null"/>. The length of the string is limited to 256 characters, not including the <see langword="null"/> terminator.
        /// </param>
        void SetDisplayName([MarshalAs(UnmanagedType.LPWStr)] string val);

        /// <summary>
        /// Retrieves the display name that identifies the job
        /// </summary>
        /// <param name="val">
        /// <see langword="null"/>-terminated string that contains the display name that identifies the job. More than one job can have the same display name. Call the CoTaskMemFree function to free displayName when done.
        /// </param>
        void GetDisplayName([MarshalAs(UnmanagedType.LPWStr)] out string val);

        /// <summary>
        /// Specifies a description of the job
        /// </summary>
        /// <param name="val">
        /// <see langword="null"/>-terminated string that provides additional information about the job. The length of the string is limited to 1,024 characters, not including the <see langword="null"/> terminator.
        /// </param>
        void SetDescription([MarshalAs(UnmanagedType.LPWStr)] string val);

        /// <summary>
        /// Retrieves the description of the job
        /// </summary>
        /// <param name="val">
        /// <see langword="null"/>-terminated string that contains a short description of the job. Call the CoTaskMemFree function to free ppDescription when done.
        /// </param>
        void GetDescription([MarshalAs(UnmanagedType.LPWStr)] out string val);

        /// <summary>
        /// Specifies the priority of the job relative to
        ///   other jobs in the transfer queue
        /// </summary>
        /// <param name="val">
        /// Specifies the priority level of your job relative to other jobs in the transfer queue. The default is <see cref="BGJobPriority"/>.Normal. For a list of priority levels, see the <see cref="BGJobPriority"/> enumeration.
        /// </param>
        void SetPriority(BGJobPriority val);

        /// <summary>
        /// Retrieves the priority level you have set for the job.
        /// </summary>
        /// <param name="val">
        /// Priority of the job relative to other jobs in the transfer queue.
        /// </param>
        void GetPriority(out BGJobPriority val);

        /// <summary>
        /// Specifies the type of event notification to receive
        /// </summary>
        /// <param name="val">
        /// Set one or more of the following flags to identify the events that you want to receive.
        /// </param>
        void SetNotifyFlags(BGJobNotificationTypes val);

        /// <summary>
        /// Retrieves the event notification (callback) flags
        ///   you have set for your application.
        /// </summary>
        /// <param name="val">
        /// Identifies the events that your application receives. The following table lists the event notification flag values.
        /// </param>
        void GetNotifyFlags(out BGJobNotificationTypes val);

        /// <summary>
        /// Specifies a pointer to your implementation of the
        /// <see cref="IBackgroundCopyCallback"/> interface (callbacks). The
        /// interface receives notification based on the event
        /// notification flags you set
        /// </summary>
        /// <param name="val">An <see cref="IBackgroundCopyCallback"/> interface pointer. To remove the current callback interface pointer, set this parameter to <see langword="null"/>.</param>
        void SetNotifyInterface([MarshalAs(UnmanagedType.IUnknown)] object val);

        /// <summary>
        /// Retrieves a pointer to your implementation
        /// of the <see cref="IBackgroundCopyCallback"/> interface (callbacks).
        /// </summary>
        /// <param name="val">Interface pointer to your implementation of the <see cref="IBackgroundCopyCallback"/> interface. When done, release notifyInterface.</param>
        void GetNotifyInterface([MarshalAs(UnmanagedType.IUnknown)] out object val);

        /// <summary>
        /// Specifies the minimum length of time that BITS waits after
        /// encountering a transient error condition before trying to
        /// transfer the file
        /// </summary>
        /// <param name="seconds">Minimum length of time, in seconds, that BITS waits after encountering a transient error before trying to transfer the file. The default retry delay is 600 seconds (10 minutes). The minimum retry delay that you can specify is 60 seconds. If you specify a value less than 60 seconds, BITS changes the value to 60 seconds. If the value exceeds the no-progress-timeout value retrieved from the <see cref="GetNoProgressTimeout"/> method, BITS will not retry the transfer and moves the job to the BGJobStateError state.</param>
        void SetMinimumRetryDelay(uint seconds);

        /// <summary>
        /// Retrieves the minimum length of time that BITS waits after
        /// encountering a transient error condition before trying to
        /// transfer the file
        /// </summary>
        /// <param name="seconds">Length of time, in seconds, that the service waits after encountering a transient error before trying to transfer the file.</param>
        void GetMinimumRetryDelay(out uint seconds);

        /// <summary>
        /// Specifies the length of time that BITS continues to try to
        /// transfer the file after encountering a transient error
        /// condition
        /// </summary>
        /// <param name="seconds">Length of time, in seconds, that BITS tries to transfer the file after the first transient error occurs. The default retry period is 1,209,600 seconds (14 days). Set the retry period to 0 to prevent retries and to force the job into the BGJobStateError state for all errors. If the retry period value exceeds the JobInactivityTimeout Group Policy value (90-day default), BITS cancels the job after the policy value is exceeded.</param>
        void SetNoProgressTimeout(uint seconds);

        /// <summary>
        /// Retrieves the length of time that BITS continues to try to
        /// transfer the file after encountering a transient error condition
        /// </summary>
        /// <param name="seconds">Length of time, in seconds, that the service tries to transfer the file after a transient error occurs.</param>
        void GetNoProgressTimeout(out uint seconds);

        /// <summary>
        /// Retrieves the number of times the job was interrupted by
        /// network failure or server unavailability
        /// </summary>
        /// <param name="errors">Number of errors that occurred while BITS tried to transfer the job. The count increases when the job moves from the BGJobStateTransferring state to the BGJobStateTransientError or BGJobStateError state.</param>
        void GetErrorCount(out ulong errors);

        /// <summary>
        /// Specifies which proxy to use to transfer the files
        /// </summary>
        /// <param name="proxyUsage">Specifies whether to use the user's proxy settings, not to use a proxy, or to use application-specified proxy settings. The default is to use the user's proxy settings, BGJobProxyUsagePreConfig. For a list of proxy options, see the <see cref="BGJobProxyUsage"/> enumeration.</param>
        /// <param name="proxyList"><see langword="null"/>-terminated string that contains the proxies to use to transfer files. The list is space-delimited. For details on specifying a proxy, see Remarks. This parameter must be <see langword="null"/> if the value of <see cref="ProxyUsage"/> is BGJobProxyUsagePreConfig, BGJobProxyUsageNoProxy, or BGJobProxyUsageNoAutoDetect. The length of the proxy list is limited to 4,000 characters, not including the <see langword="null"/> terminator.</param>
        /// <param name="proxyBypassList"><see langword="null"/>-terminated string that contains an optional list of host names, IP addresses, or both, that can bypass the proxy. The list is space-delimited. For details on specifying a bypass proxy, see Remarks. This parameter must be <see langword="null"/> if the value of <see cref="ProxyUsage"/> is BGJobProxyUsagePreConfig, BGJobProxyUsageNoProxy, or BGJobProxyUsageNoAutoDetect. The length of the proxy bypass list is limited to 4,000 characters, not including the <see langword="null"/> terminator.</param>
        void SetProxySettings(BGJobProxyUsage proxyUsage, [MarshalAs(UnmanagedType.LPWStr)] string proxyList, [MarshalAs(UnmanagedType.LPWStr)] string proxyBypassList);

        /// <summary>
        /// Retrieves the proxy settings the job uses to transfer the files
        /// </summary>
        /// <param name="proxyUsage">Specifies the proxy settings the job uses to transfer the files. For a list of proxy options, see the <see cref="BGJobProxyUsage"/> enumeration.</param>
        /// <param name="proxyList"><see langword="null"/>-terminated string that contains one or more proxies to use to transfer files. The list is space-delimited. For details on the format of the string, see the Listing Proxy Servers section of Enabling Internet Functionality. Call the CoTaskMemFree function to free <paramref name="proxyList"/> when done.</param>
        /// <param name="proxyBypassList"><see langword="null"/>-terminated string that contains an optional list of host names or IP addresses, or both, that were not routed through the proxy. The list is space-delimited. For details on the format of the string, see the Listing the Proxy Bypass section of Enabling Internet Functionality. Call the CoTaskMemFree function to free <paramref name="proxyBypassList"/> when done.</param>
        void GetProxySettings(
            out BGJobProxyUsage proxyUsage, [MarshalAs(UnmanagedType.LPWStr)] out string proxyList, [MarshalAs(UnmanagedType.LPWStr)] out string proxyBypassList);

        /// <summary>
        /// Changes the ownership of the job to the current user
        /// </summary>
        void TakeOwnership();
    }

    #endregion

    #region IBackgroundCopyJob2

    /// <summary>
    /// Use the IBackgroundCopyJob2 interface to retrieve reply data from an upload-reply job, determine the progress of the
    ///   reply data transfer to the client, request command line execution, and provide credentials for proxy and remote
    ///   server authentication requests.
    /// </summary>
    [ComImport]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("54B50739-686F-45EB-9DFF-D6A9A0FAA9AF")]
    internal interface IBackgroundCopyJob2
    {
        /// <summary>
        /// Adds multiple files to the job
        /// </summary>
        /// <param name="fileCount">
        /// Number of elements in paFileSet.
        /// </param>
        /// <param name="fileSet">
        /// Array of <see cref="BGFileInfo"/> structures that identify the local and remote file names of the files to transfer.
        /// </param>
        void AddFileSet(uint fileCount, [MarshalAs(UnmanagedType.LPArray)] BGFileInfo[] fileSet);

        /// <summary>
        /// Adds a single file to the job
        /// </summary>
        /// <param name="remoteUrl">
        /// <see langword="null"/>-terminated string that contains the name of the file on the client. For information on specifying the local name, see the LocalName member and Remarks section of the <see cref="BGFileInfo"/> structure.
        /// </param>
        /// <param name="localName">
        /// <see langword="null"/>-terminated string that contains the name of the file on the server. For information on specifying the remote name, see the RemoteName member and Remarks section of the <see cref="BGFileInfo"/> structure.
        /// </param>
        void AddFile([MarshalAs(UnmanagedType.LPWStr)] string remoteUrl, [MarshalAs(UnmanagedType.LPWStr)] string localName);

        /// <summary>
        /// Returns an interface pointer to an enumerator
        ///   object that you use to enumerate the files in the job
        /// </summary>
        /// <param name="enum">
        /// I<see cref="IEnumBackgroundCopyFiles"/>interface pointer that you use to enumerate the files in the job. Release enumFiles when done.
        /// </param>
        void EnumFiles([MarshalAs(UnmanagedType.Interface)] out IEnumBackgroundCopyFiles @enum);

        /// <summary>
        /// Pauses the job
        /// </summary>
        void Suspend();

        /// <summary>
        /// Restarts a suspended job
        /// </summary>
        void Resume();

        /// <summary>
        /// Cancels the job and removes temporary files from the client
        /// </summary>
        void Cancel();

        /// <summary>
        /// Ends the job and saves the transferred files on the client
        /// </summary>
        void Complete();

        /// <summary>
        /// Retrieves the identifier of the job in the queue
        /// </summary>
        /// <param name="val">
        /// GUID that identifies the job within the BITS queue.
        /// </param>
        void GetId(out Guid val);

        /// <summary>
        /// Retrieves the type of transfer being performed,
        /// such as a file download
        /// </summary>
        /// <param name="val">Type of transfer being performed. For a list of transfer types, see the BGJob_TYPE enumeration type.</param>
        void GetType(out BGJobType val);

        /// <summary>
        /// Retrieves job-related progress information,
        /// such as the number of bytes and files transferred
        /// to the client
        /// </summary>
        /// <param name="val">Contains data that you can use to calculate the percentage of the job that is complete. For more information, see <see cref="BGJobProgress"/>.</param>
        void GetProgress(out BGJobProgress val);

        /// <summary>
        /// Retrieves timestamps for activities related
        ///   to the job, such as the time the job was created
        /// </summary>
        /// <param name="val">
        /// Contains job-related time stamps. For available time stamps, see the BGJob_TIMES structure.
        /// </param>
        void GetTimes(out BGJobTimes val);

        /// <summary>
        /// Retrieves the state of the job
        /// </summary>
        /// <param name="val">
        /// Current state of the job. For example, the state reflects whether the job is in error, transferring data, or suspended. For a list of job states, see the <see cref="BGJobState"/> enumeration type. 
        /// </param>
        void GetState(out BGJobState val);

        /// <summary>
        /// Retrieves an interface pointer to
        ///   the error object after an error occurs
        /// </summary>
        /// <param name="error">
        /// Error interface that provides the error code, a description of the error, and the context in which the error occurred. This parameter also identifies the file being transferred at the time the error occurred. Release error when done.
        /// </param>
        void GetError([MarshalAs(UnmanagedType.Interface)] out IBackgroundCopyError error);

        /// <summary>
        /// Retrieves the job owner's identity
        /// </summary>
        /// <param name="val">
        /// <see langword="null"/>-terminated string that contains the string version of the SID that identifies the job's owner. Call the CoTaskMemFree function to free ppOwner when done.
        /// </param>
        void GetOwner([MarshalAs(UnmanagedType.LPWStr)] out string val);

        /// <summary>
        /// Specifies a display name that identifies the job in
        ///   a user interface
        /// </summary>
        /// <param name="val">
        /// <see langword="null"/>-terminated string that identifies the job. Must not be <see langword="null"/>. The length of the string is limited to 256 characters, not including the <see langword="null"/> terminator.
        /// </param>
        void SetDisplayName([MarshalAs(UnmanagedType.LPWStr)] string val);

        /// <summary>
        /// Retrieves the display name that identifies the job
        /// </summary>
        /// <param name="val">
        /// <see langword="null"/>-terminated string that contains the display name that identifies the job. More than one job can have the same display name. Call the CoTaskMemFree function to free displayName when done.
        /// </param>
        void GetDisplayName([MarshalAs(UnmanagedType.LPWStr)] out string val);

        /// <summary>
        /// Specifies a description of the job
        /// </summary>
        /// <param name="val">
        /// <see langword="null"/>-terminated string that provides additional information about the job. The length of the string is limited to 1,024 characters, not including the <see langword="null"/> terminator.
        /// </param>
        void SetDescription([MarshalAs(UnmanagedType.LPWStr)] string val);

        /// <summary>
        /// Retrieves the description of the job
        /// </summary>
        /// <param name="val">
        /// <see langword="null"/>-terminated string that contains a short description of the job. Call the CoTaskMemFree function to free ppDescription when done. 
        /// </param>
        void GetDescription([MarshalAs(UnmanagedType.LPWStr)] out string val);

        /// <summary>
        /// Specifies the priority of the job relative to 
        ///   other jobs in the transfer queue
        /// </summary>
        /// <param name="val">
        /// Specifies the priority level of your job relative to other jobs in the transfer queue. The default is BGJobPriorityNormal. For a list of priority levels, see the <see cref="BGJobPriority"/> enumeration. 
        /// </param>
        void SetPriority(BGJobPriority val);

        /// <summary>
        /// Retrieves the priority level you have set for the job.
        /// </summary>
        /// <param name="val">
        /// Priority of the job relative to other jobs in the transfer queue. 
        /// </param>
        void GetPriority(out BGJobPriority val);

        /// <summary>
        /// Specifies the type of event notification to receive
        /// </summary>
        /// <param name="val">
        /// Set one or more of the following flags to identify the events that you want to receive. 
        /// </param>
        void SetNotifyFlags([MarshalAs(UnmanagedType.U4)] BGJobNotificationTypes val);

        /// <summary>
        /// Retrieves the event notification (callback) flags 
        ///   you have set for your application.
        /// </summary>
        /// <param name="val">
        /// Identifies the events that your application receives. The following table lists the event notification flag values. 
        /// </param>
        void GetNotifyFlags(out uint val);

        /// <summary>
        /// Specifies a pointer to your implementation of the 
        ///   <see cref="IBackgroundCopyCallback"/> interface (callbacks). The 
        ///   interface receives notification based on the event 
        ///   notification flags you set
        /// </summary>
        /// <param name="val">
        /// An <see cref="IBackgroundCopyCallback"/> interface pointer. To remove the current callback interface pointer, set this parameter to <see langword="null"/>.
        /// </param>
        void SetNotifyInterface([MarshalAs(UnmanagedType.IUnknown)] object val);

        /// <summary>
        /// Retrieves a pointer to your implementation 
        ///   of the <see cref="IBackgroundCopyCallback"/> interface (callbacks).
        /// </summary>
        /// <param name="val">
        /// Interface pointer to your implementation of the <see cref="IBackgroundCopyCallback"/> interface. When done, release ppNotifyInterface.
        /// </param>
        void GetNotifyInterface([MarshalAs(UnmanagedType.IUnknown)] out object val);

        /// <summary>
        /// Specifies the minimum length of time that BITS waits after 
        ///   encountering a transient error condition before trying to 
        ///   transfer the file
        /// </summary>
        /// <param name="seconds">
        /// Minimum length of time, in seconds, that BITS waits after encountering a transient error before trying to transfer the file. The default retry delay is 600 seconds (10 minutes). The minimum retry delay that you can specify is 60 seconds. If you specify a value less than 60 seconds, BITS changes the value to 60 seconds. If the value exceeds the no-progress-timeout value retrieved from the <see cref="GetNoProgressTimeout"/> method, BITS will not retry the transfer and moves the job to the BGJobStateError state. 
        /// </param>
        void SetMinimumRetryDelay(uint seconds);

        /// <summary>
        /// Retrieves the minimum length of time that BITS waits after 
        ///   encountering a transient error condition before trying to 
        ///   transfer the file
        /// </summary>
        /// <param name="seconds">
        /// Length of time, in seconds, that the service waits after encountering a transient error before trying to transfer the file. 
        /// </param>
        void GetMinimumRetryDelay(out uint seconds);

        /// <summary>
        /// Specifies the length of time that BITS continues to try to 
        ///   transfer the file after encountering a transient error 
        ///   condition
        /// </summary>
        /// <param name="seconds">
        /// Length of time, in seconds, that BITS tries to transfer the file after the first transient error occurs. The default retry period is 1,209,600 seconds (14 days). Set the retry period to 0 to prevent retries and to force the job into the BGJobStateError state for all errors. If the retry period value exceeds the JobInactivityTimeout Group Policy value (90-day default), BITS cancels the job after the policy value is exceeded.
        /// </param>
        void SetNoProgressTimeout(uint seconds);

        /// <summary>
        /// Retrieves the length of time that BITS continues to try to 
        ///   transfer the file after encountering a transient error condition
        /// </summary>
        /// <param name="seconds">
        /// Length of time, in seconds, that the service tries to transfer the file after a transient error occurs. 
        /// </param>
        void GetNoProgressTimeout(out uint seconds);

        /// <summary>
        /// Retrieves the number of times the job was interrupted by 
        ///   network failure or server unavailability
        /// </summary>
        /// <param name="errors">
        /// Number of errors that occurred while BITS tried to transfer the job. The count increases when the job moves from the BGJobStateTransferring state to the BGJobStateTransientError or BGJobStateError state.
        /// </param>
        void GetErrorCount(out ulong errors);

        /// <summary>
        /// Specifies which proxy to use to transfer the files
        /// </summary>
        /// <param name="proxyUsage">
        /// Specifies whether to use the user's proxy settings, not to use a proxy, or to use application-specified proxy settings. The default is to use the user's proxy settings, BGJobProxyUsagePreConfig. For a list of proxy options, see the <see cref="BGJobProxyUsage"/> enumeration.
        /// </param>
        /// <param name="proxyList">
        /// <see langword="null"/>-terminated string that contains the proxies to use to transfer files. The list is space-delimited. For details on specifying a proxy, see Remarks. This parameter must be <see langword="null"/> if the value of <see cref="ProxyUsage"/> is BGJobProxyUsagePreConfig, BGJobProxyUsageNoProxy, or BGJobProxyUsageNoAutoDetect. The length of the proxy list is limited to 4,000 characters, not including the <see langword="null"/> terminator. 
        /// </param>
        /// <param name="proxyBypassList">
        /// <see langword="null"/>-terminated string that contains an optional list of host names, IP addresses, or both, that can bypass the proxy. The list is space-delimited. For details on specifying a bypass proxy, see Remarks. This parameter must be <see langword="null"/> if the value of <see cref="ProxyUsage"/> is BGJobProxyUsagePreConfig, BGJobProxyUsageNoProxy, or BGJobProxyUsageNoAutoDetect. The length of the proxy bypass list is limited to 4,000 characters, not including the <see langword="null"/> terminator. 
        /// </param>
        void SetProxySettings(BGJobProxyUsage proxyUsage, [MarshalAs(UnmanagedType.LPWStr)] string proxyList, [MarshalAs(UnmanagedType.LPWStr)] string proxyBypassList);

        /// <summary>
        /// Retrieves the proxy settings the job uses to transfer the files
        /// </summary>
        /// <param name="proxyUsage">
        /// Specifies the proxy settings the job uses to transfer the files. For a list of proxy options, see the <see cref="BGJobProxyUsage"/> enumeration. 
        /// </param>
        /// <param name="proxyList">
        /// <see langword="null"/>-terminated string that contains one or more proxies to use to transfer files. The list is space-delimited. For details on the format of the string, see the Listing Proxy Servers section of Enabling Internet Functionality. Call the CoTaskMemFree function to free <paramref name="proxyList"/> when done.
        /// </param>
        /// <param name="proxyBypassList">
        /// <see langword="null"/>-terminated string that contains an optional list of host names or IP addresses, or both, that were not routed through the proxy. The list is space-delimited. For details on the format of the string, see the Listing the Proxy Bypass section of Enabling Internet Functionality. Call the CoTaskMemFree function to free <paramref name="proxyBypassList"/> when done.
        /// </param>
        void GetProxySettings(
            out BGJobProxyUsage proxyUsage, [MarshalAs(UnmanagedType.LPWStr)] out string proxyList, [MarshalAs(UnmanagedType.LPWStr)] out string proxyBypassList);

        /// <summary>
        /// Changes the ownership of the job to the current user
        /// </summary>
        void TakeOwnership();

        /// <summary>
        /// Use the SetNotifyCmdLine method to specify a program to execute if the job enters the BGJobStateError or BGJobStateTransferred state. BITS executes the program in the context of the user.
        /// </summary>
        /// <param name="program">
        /// <see langword="null"/>-terminated string that contains the program to execute. The program parameter is limited to MAX_PATH characters, not including the <see langword="null"/> terminator. You should specify a full path to the program; the method will not use the search path to locate the program. To remove command line notification, set program and parameters to <see langword="null"/>. The method fails if program is <see langword="null"/> and parameters is non-<see langword="null"/>. 
        /// </param>
        /// <param name="parameters">
        /// <see langword="null"/>-terminated string that contains the parameters of the program in program. The first parameter must be the program in program (use quotes if the path uses long file names). The parameters parameter is limited to 4,000 characters, not including the <see langword="null"/> terminator. This parameter can be <see langword="null"/>.
        /// </param>
        void SetNotifyCmdLine([MarshalAs(UnmanagedType.LPWStr)] string program, [MarshalAs(UnmanagedType.LPWStr)] string parameters);

        /// <summary>
        /// Use the GetNotifyCmdLine method to retrieve the program to execute when the job enters the error or transferred state.
        /// </summary>
        /// <param name="program">
        /// <see langword="null"/>-terminated string that contains the program to execute when the job enters the error or transferred state. Call the CoTaskMemFree function to free program when done. 
        /// </param>
        /// <param name="parameters">
        /// <see langword="null"/>-terminated string that contains the arguments of the program in program. Call the CoTaskMemFree function to free parameters when done. 
        /// </param>
        void GetNotifyCmdLine([MarshalAs(UnmanagedType.LPWStr)] out string program, [MarshalAs(UnmanagedType.LPWStr)] out string parameters);

        /// <summary>
        /// Use the GetReplyProgress method to retrieve progress information related to the transfer of the reply data from an upload-reply job.
        /// </summary>
        /// <param name="progress">
        /// Contains information that you use to calculate the percentage of the reply file transfer that is complete. For more information, see <see cref="BGJobReplyProgress"/>.
        /// </param>
        void GetReplyProgress([Out] out BGJobReplyProgress progress);

        /// <summary>
        /// Use the GetReplyData method to retrieve an in-memory copy of the reply data from the server application. Only call this method if the job's type is BGJobTypeUploadReply and its state is BGJobStateTransferred.
        /// </summary>
        /// <param name="buffer">
        /// Buffer to contain the reply data. The method sets buffer to <see langword="null"/> if the server application did not return a reply. Call the CoTaskMemFree function to free buffer when done.
        /// </param>
        /// <param name="length">
        /// Size, in bytes, of the reply data in buffer.
        /// </param>
        void GetReplyData(IntPtr buffer, out ulong length);

        /// <summary>
        /// Use the SetReplyFileName method to specify the name of the file to contain the reply data from the server application. Only call this method if the job's type is BGJobTypeUploadReply.
        /// </summary>
        /// <param name="replyFileName">
        /// <see langword="null"/>-terminated string that contains the full path to the reply file. BITS generates the file name if ReplyFileNamePathSpec is <see langword="null"/> or an empty string. You cannot use wild cards in the path or file name, and directories in the path must exist. The path is limited to MAX_PATH, not including the <see langword="null"/> terminator. The user must have permissions to write to the directory. BITS does not support NTFS streams. Instead of using network drives, which are session specific, use UNC paths (for example, \\server\share\path\file). Do not include the \\? prefix in the path. 
        /// </param>
        void SetReplyFileName([MarshalAs(UnmanagedType.LPWStr)] string replyFileName);

        /// <summary>
        /// Use the GetReplyFileName method to retrieve the name of the file that contains the reply data from the server application. Only call this method if the job type is BGJobTypeUploadReply.
        /// </summary>
        /// <param name="replyFileName">
        /// <see langword="null"/>-terminated string that contains the full path to the reply file. Call the CoTaskMemFree function to free <paramref name="replyFileName"/> when done. 
        /// </param>
        void GetReplyFileName([MarshalAs(UnmanagedType.LPWStr)] out string replyFileName);

        /// <summary>
        /// Use the SetCredentials method to specify the credentials to use for a proxy or remote server user authentication request.
        /// </summary>
        /// <param name="credentials">
        /// Identifies the target (proxy or server), authentication scheme, and the user's credentials to use for user authentication. For details, see the <see cref="BGAuthCredentials"/> structure. If the job currently contains credentials with the same target and scheme pair, the existing credentials are replaced with the new credentials. The credentials persist for the life of the job. To remove the credentials from the job, call the IBackgroundCopyJob2::<see cref="RemoveCredentials"/> method. 
        /// </param>
        void SetCredentials([In] ref BGAuthCredentials credentials);

        /// <summary>
        /// Use the RemoveCredentials method to remove credentials from use. The credentials must match an existing target and scheme pair that you specified using the IBackgroundCopyJob2::<see cref="SetCredentials"/> method. There is no method to retrieve the credentials you have set.
        /// </summary>
        /// <param name="target">
        /// Identifies whether to use the credentials for proxy or server authentication.
        /// </param>
        /// <param name="scheme">
        /// Identifies the authentication scheme to use (basic or one of several challenge-response schemes). For details, see the <see cref="BGAuthScheme"/> enumeration. 
        /// </param>
        void RemoveCredentials(BGAuthTarget target, BGAuthScheme scheme);
    }

    #endregion

    #region IBackgroundCopyJob3

    /// <summary>Use the IBackgroundCopyJob3 interface to download ranges of a file and change the prefix of a remote file name.</summary>
    [ComImport]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("443C8934-90FF-48ED-BCDE-26F5C7450042")]
    internal interface IBackgroundCopyJob3
    {
        /// <summary>
        /// Adds multiple files to the job
        /// </summary>
        /// <param name="fileCount">
        /// Number of elements in paFileSet. 
        /// </param>
        /// <param name="fileSet">
        /// Array of <see cref="BGFileInfo"/> structures that identify the local and remote file names of the files to transfer.
        /// </param>
        void AddFileSet(uint fileCount, [MarshalAs(UnmanagedType.LPArray)] BGFileInfo[] fileSet);

        /// <summary>
        /// Adds a single file to the job
        /// </summary>
        /// <param name="remoteUrl">
        /// <see langword="null"/>-terminated string that contains the name of the file on the client. For information on specifying the local name, see the LocalName member and Remarks section of the <see cref="BGFileInfo"/> structure. 
        /// </param>
        /// <param name="localName">
        /// <see langword="null"/>-terminated string that contains the name of the file on the server. For information on specifying the remote name, see the RemoteName member and Remarks section of the <see cref="BGFileInfo"/> structure. 
        /// </param>
        void AddFile([MarshalAs(UnmanagedType.LPWStr)] string remoteUrl, [MarshalAs(UnmanagedType.LPWStr)] string localName);

        /// <summary>
        /// Returns an interface pointer to an enumerator
        ///   object that you use to enumerate the files in the job
        /// </summary>
        /// <param name="enum">
        /// <see cref="IEnumBackgroundCopyFiles"/> interface pointer that you use to enumerate the files in the job. Release enumFiles when done. 
        /// </param>
        void EnumFiles([MarshalAs(UnmanagedType.Interface)] out IEnumBackgroundCopyFiles @enum);

        /// <summary>
        /// Pauses the job
        /// </summary>
        void Suspend();

        /// <summary>
        /// Restarts a suspended job
        /// </summary>
        void Resume();

        /// <summary>
        /// Cancels the job and removes temporary files from the client
        /// </summary>
        void Cancel();

        /// <summary>
        /// Ends the job and saves the transferred files on the client
        /// </summary>
        void Complete();

        /// <summary>
        /// Retrieves the identifier of the job in the queue
        /// </summary>
        /// <param name="val">
        /// GUID that identifies the job within the BITS queue.
        /// </param>
        void GetId(out Guid val);

        /// <summary>
        /// Retrieves the type of transfer being performed, 
        ///   such as a file download
        /// </summary>
        /// <param name="val">
        /// Type of transfer being performed. For a list of transfer types, see the BGJob_TYPE enumeration type. 
        /// </param>
        void GetType(out BGJobType val);

        /// <summary>
        /// Retrieves job-related progress information, 
        ///   such as the number of bytes and files transferred 
        ///   to the client
        /// </summary>
        /// <param name="val">
        /// Contains data that you can use to calculate the percentage of the job that is complete. For more information, see <see cref="BGJobProgress"/>. 
        /// </param>
        void GetProgress(out BGJobProgress val);

        /// <summary>
        /// Retrieves timestamps for activities related
        ///   to the job, such as the time the job was created
        /// </summary>
        /// <param name="val">
        /// Contains job-related time stamps. For available time stamps, see the BGJob_TIMES structure.
        /// </param>
        void GetTimes(out BGJobTimes val);

        /// <summary>
        /// Retrieves the state of the job
        /// </summary>
        /// <param name="val">
        /// Current state of the job. For example, the state reflects whether the job is in error, transferring data, or suspended. For a list of job states, see the <see cref="BGJobState"/> enumeration type. 
        /// </param>
        void GetState(out BGJobState val);

        /// <summary>
        /// Retrieves an interface pointer to 
        ///   the error object after an error occurs
        /// </summary>
        /// <param name="error">
        /// Error interface that provides the error code, a description of the error, and the context in which the error occurred. This parameter also identifies the file being transferred at the time the error occurred. Release error when done. 
        /// </param>
        void GetError([MarshalAs(UnmanagedType.Interface)] out IBackgroundCopyError error);

        /// <summary>
        /// Retrieves the job owner's identity
        /// </summary>
        /// <param name="val">
        /// <see langword="null"/>-terminated string that contains the string version of the SID that identifies the job's owner. Call the CoTaskMemFree function to free ppOwner when done. 
        /// </param>
        void GetOwner([MarshalAs(UnmanagedType.LPWStr)] out string val);

        /// <summary>
        /// Specifies a display name that identifies the job in 
        ///   a user interface
        /// </summary>
        /// <param name="val">
        /// <see langword="null"/>-terminated string that identifies the job. Must not be <see langword="null"/>. The length of the string is limited to 256 characters, not including the <see langword="null"/> terminator. 
        /// </param>
        void SetDisplayName([MarshalAs(UnmanagedType.LPWStr)] string val);

        /// <summary>
        /// Retrieves the display name that identifies the job
        /// </summary>
        /// <param name="val">
        /// <see langword="null"/>-terminated string that contains the display name that identifies the job. More than one job can have the same display name. Call the CoTaskMemFree function to free displayName when done.
        /// </param>
        void GetDisplayName([MarshalAs(UnmanagedType.LPWStr)] out string val);

        /// <summary>
        /// Specifies a description of the job
        /// </summary>
        /// <param name="val">
        /// <see langword="null"/>-terminated string that provides additional information about the job. The length of the string is limited to 1,024 characters, not including the <see langword="null"/> terminator.
        /// </param>
        void SetDescription([MarshalAs(UnmanagedType.LPWStr)] string val);

        /// <summary>
        /// Retrieves the description of the job
        /// </summary>
        /// <param name="val">
        /// <see langword="null"/>-terminated string that contains a short description of the job. Call the CoTaskMemFree function to free ppDescription when done. 
        /// </param>
        void GetDescription([MarshalAs(UnmanagedType.LPWStr)] out string val);

        /// <summary>
        /// Specifies the priority of the job relative to 
        ///   other jobs in the transfer queue
        /// </summary>
        /// <param name="val">
        /// Specifies the priority level of your job relative to other jobs in the transfer queue. The default is BGJobPriorityNormal. For a list of priority levels, see the <see cref="BGJobPriority"/> enumeration. 
        /// </param>
        void SetPriority(BGJobPriority val);

        /// <summary>
        /// Retrieves the priority level you have set for the job.
        /// </summary>
        /// <param name="val">
        /// Priority of the job relative to other jobs in the transfer queue. 
        /// </param>
        void GetPriority(out BGJobPriority val);

        /// <summary>
        /// Specifies the type of event notification to receive
        /// </summary>
        /// <param name="val">
        /// Set one or more of the following flags to identify the events that you want to receive. 
        /// </param>
        void SetNotifyFlags([MarshalAs(UnmanagedType.U4)] BGJobNotificationTypes val);

        /// <summary>
        /// Retrieves the event notification (callback) flags 
        ///   you have set for your application.
        /// </summary>
        /// <param name="val">
        /// Identifies the events that your application receives. The following table lists the event notification flag values. 
        /// </param>
        void GetNotifyFlags(out uint val);

        /// <summary>
        /// Specifies a pointer to your implementation of the 
        ///   <see cref="IBackgroundCopyCallback"/> interface (callbacks). The 
        ///   interface receives notification based on the event 
        ///   notification flags you set
        /// </summary>
        /// <param name="val">
        /// An <see cref="IBackgroundCopyCallback"/> interface pointer. To remove the current callback interface pointer, set this parameter to <see langword="null"/>.
        /// </param>
        void SetNotifyInterface([MarshalAs(UnmanagedType.IUnknown)] object val);

        /// <summary>
        /// Retrieves a pointer to your implementation 
        ///   of the <see cref="IBackgroundCopyCallback"/> interface (callbacks).
        /// </summary>
        /// <param name="val">
        /// Interface pointer to your implementation of the <see cref="IBackgroundCopyCallback"/> interface. When done, release ppNotifyInterface.
        /// </param>
        void GetNotifyInterface([MarshalAs(UnmanagedType.IUnknown)] out object val);

        /// <summary>
        /// Specifies the minimum length of time that BITS waits after 
        ///   encountering a transient error condition before trying to 
        ///   transfer the file
        /// </summary>
        /// <param name="seconds">
        /// Minimum length of time, in seconds, that BITS waits after encountering a transient error before trying to transfer the file. The default retry delay is 600 seconds (10 minutes). The minimum retry delay that you can specify is 60 seconds. If you specify a value less than 60 seconds, BITS changes the value to 60 seconds. If the value exceeds the no-progress-timeout value retrieved from the <see cref="GetNoProgressTimeout"/> method, BITS will not retry the transfer and moves the job to the BGJobStateError state. 
        /// </param>
        void SetMinimumRetryDelay(uint seconds);

        /// <summary>
        /// Retrieves the minimum length of time that BITS waits after 
        ///   encountering a transient error condition before trying to 
        ///   transfer the file
        /// </summary>
        /// <param name="seconds">
        /// Length of time, in seconds, that the service waits after encountering a transient error before trying to transfer the file. 
        /// </param>
        void GetMinimumRetryDelay(out uint seconds);

        /// <summary>
        /// Specifies the length of time that BITS continues to try to 
        ///   transfer the file after encountering a transient error 
        ///   condition
        /// </summary>
        /// <param name="seconds">
        /// Length of time, in seconds, that BITS tries to transfer the file after the first transient error occurs. The default retry period is 1,209,600 seconds (14 days). Set the retry period to 0 to prevent retries and to force the job into the BGJobStateError state for all errors. If the retry period value exceeds the JobInactivityTimeout Group Policy value (90-day default), BITS cancels the job after the policy value is exceeded.
        /// </param>
        void SetNoProgressTimeout(uint seconds);

        /// <summary>
        /// Retrieves the length of time that BITS continues to try to 
        ///   transfer the file after encountering a transient error condition
        /// </summary>
        /// <param name="seconds">
        /// Length of time, in seconds, that the service tries to transfer the file after a transient error occurs. 
        /// </param>
        void GetNoProgressTimeout(out uint seconds);

        /// <summary>
        /// Retrieves the number of times the job was interrupted by 
        ///   network failure or server unavailability
        /// </summary>
        /// <param name="errors">
        /// Number of errors that occurred while BITS tried to transfer the job. The count increases when the job moves from the BGJobStateTransferring state to the BGJobStateTransientError or BGJobStateError state.
        /// </param>
        void GetErrorCount(out ulong errors);

        /// <summary>
        /// Specifies which proxy to use to transfer the files
        /// </summary>
        /// <param name="proxyUsage">
        /// Specifies whether to use the user's proxy settings, not to use a proxy, or to use application-specified proxy settings. The default is to use the user's proxy settings, BGJobProxyUsagePreConfig. For a list of proxy options, see the <see cref="BGJobProxyUsage"/> enumeration.
        /// </param>
        /// <param name="proxyList">
        /// <see langword="null"/>-terminated string that contains the proxies to use to transfer files. The list is space-delimited. For details on specifying a proxy, see Remarks. This parameter must be <see langword="null"/> if the value of <see cref="ProxyUsage"/> is BGJobProxyUsagePreConfig, BGJobProxyUsageNoProxy, or BGJobProxyUsageNoAutoDetect. The length of the proxy list is limited to 4,000 characters, not including the <see langword="null"/> terminator. 
        /// </param>
        /// <param name="proxyBypassList">
        /// <see langword="null"/>-terminated string that contains an optional list of host names, IP addresses, or both, that can bypass the proxy. The list is space-delimited. For details on specifying a bypass proxy, see Remarks. This parameter must be <see langword="null"/> if the value of <see cref="ProxyUsage"/> is BGJobProxyUsagePreConfig, BGJobProxyUsageNoProxy, or BGJobProxyUsageNoAutoDetect. The length of the proxy bypass list is limited to 4,000 characters, not including the <see langword="null"/> terminator. 
        /// </param>
        void SetProxySettings(BGJobProxyUsage proxyUsage, [MarshalAs(UnmanagedType.LPWStr)] string proxyList, [MarshalAs(UnmanagedType.LPWStr)] string proxyBypassList);

        /// <summary>
        /// Retrieves the proxy settings the job uses to transfer the files
        /// </summary>
        /// <param name="proxyUsage">
        /// Specifies the proxy settings the job uses to transfer the files. For a list of proxy options, see the <see cref="BGJobProxyUsage"/> enumeration. 
        /// </param>
        /// <param name="proxyList">
        /// <see langword="null"/>-terminated string that contains one or more proxies to use to transfer files. The list is space-delimited. For details on the format of the string, see the Listing Proxy Servers section of Enabling Internet Functionality. Call the CoTaskMemFree function to free <paramref name="proxyList"/> when done.
        /// </param>
        /// <param name="proxyBypassList">
        /// <see langword="null"/>-terminated string that contains an optional list of host names or IP addresses, or both, that were not routed through the proxy. The list is space-delimited. For details on the format of the string, see the Listing the Proxy Bypass section of Enabling Internet Functionality. Call the CoTaskMemFree function to free <paramref name="proxyBypassList"/> when done.
        /// </param>
        void GetProxySettings(
            out BGJobProxyUsage proxyUsage, [MarshalAs(UnmanagedType.LPWStr)] out string proxyList, [MarshalAs(UnmanagedType.LPWStr)] out string proxyBypassList);

        /// <summary>
        /// Changes the ownership of the job to the current user
        /// </summary>
        void TakeOwnership();

        /// <summary>
        /// Use the SetNotifyCmdLine method to specify a program to execute if the job enters the BGJobStateError or BGJobStateTransferred state. BITS executes the program in the context of the user.
        /// </summary>
        /// <param name="program">
        /// <see langword="null"/>-terminated string that contains the program to execute. The program parameter is limited to MAX_PATH characters, not including the <see langword="null"/> terminator. You should specify a full path to the program; the method will not use the search path to locate the program. To remove command line notification, set program and parameters to <see langword="null"/>. The method fails if program is <see langword="null"/> and parameters is non-<see langword="null"/>. 
        /// </param>
        /// <param name="parameters">
        /// <see langword="null"/>-terminated string that contains the parameters of the program in program. The first parameter must be the program in program (use quotes if the path uses long file names). The parameters parameter is limited to 4,000 characters, not including the <see langword="null"/> terminator. This parameter can be <see langword="null"/>.
        /// </param>
        void SetNotifyCmdLine([MarshalAs(UnmanagedType.LPWStr)] string program, [MarshalAs(UnmanagedType.LPWStr)] string parameters);

        /// <summary>
        /// Use the GetNotifyCmdLine method to retrieve the program to execute when the job enters the error or transferred state.
        /// </summary>
        /// <param name="program">
        /// <see langword="null"/>-terminated string that contains the program to execute when the job enters the error or transferred state. Call the CoTaskMemFree function to free program when done. 
        /// </param>
        /// <param name="parameters">
        /// <see langword="null"/>-terminated string that contains the arguments of the program in program. Call the CoTaskMemFree function to free parameters when done. 
        /// </param>
        void GetNotifyCmdLine([MarshalAs(UnmanagedType.LPWStr)] out string program, [MarshalAs(UnmanagedType.LPWStr)] out string parameters);

        /// <summary>
        /// Use the GetReplyProgress method to retrieve progress information related to the transfer of the reply data from an upload-reply job.
        /// </summary>
        /// <param name="progress">
        /// Contains information that you use to calculate the percentage of the reply file transfer that is complete. For more information, see <see cref="BGJobReplyProgress"/>.
        /// </param>
        void GetReplyProgress([Out] out BGJobReplyProgress progress);

        /// <summary>
        /// Use the GetReplyData method to retrieve an in-memory copy of the reply data from the server application. Only call this method if the job's type is BGJobTypeUploadReply and its state is BGJobStateTransferred.
        /// </summary>
        /// <param name="buffer">
        /// Buffer to contain the reply data. The method sets buffer to <see langword="null"/> if the server application did not return a reply. Call the CoTaskMemFree function to free buffer when done.
        /// </param>
        /// <param name="length">
        /// Size, in bytes, of the reply data in buffer.
        /// </param>
        void GetReplyData(IntPtr buffer, out ulong length);

        /// <summary>
        /// Use the SetReplyFileName method to specify the name of the file to contain the reply data from the server application. Only call this method if the job's type is BGJobTypeUploadReply.
        /// </summary>
        /// <param name="replyFileName">
        /// <see langword="null"/>-terminated string that contains the full path to the reply file. BITS generates the file name if ReplyFileNamePathSpec is <see langword="null"/> or an empty string. You cannot use wild cards in the path or file name, and directories in the path must exist. The path is limited to MAX_PATH, not including the <see langword="null"/> terminator. The user must have permissions to write to the directory. BITS does not support NTFS streams. Instead of using network drives, which are session specific, use UNC paths (for example, \\server\share\path\file). Do not include the \\? prefix in the path. 
        /// </param>
        void SetReplyFileName([MarshalAs(UnmanagedType.LPWStr)] string replyFileName);

        /// <summary>
        /// Use the GetReplyFileName method to retrieve the name of the file that contains the reply data from the server application. Only call this method if the job type is BGJobTypeUploadReply.
        /// </summary>
        /// <param name="replyFileName">
        /// <see langword="null"/>-terminated string that contains the full path to the reply file. Call the CoTaskMemFree function to free <paramref name="replyFileName"/> when done. 
        /// </param>
        void GetReplyFileName([MarshalAs(UnmanagedType.LPWStr)] out string replyFileName);

        /// <summary>
        /// Use the SetCredentials method to specify the credentials to use for a proxy or remote server user authentication request.
        /// </summary>
        /// <param name="credentials">
        /// Identifies the target (proxy or server), authentication scheme, and the user's credentials to use for user authentication. For details, see the <see cref="BGAuthCredentials"/> structure. If the job currently contains credentials with the same target and scheme pair, the existing credentials are replaced with the new credentials. The credentials persist for the life of the job. To remove the credentials from the job, call the IBackgroundCopyJob2::<see cref="RemoveCredentials"/> method. 
        /// </param>
        void SetCredentials([In] ref BGAuthCredentials credentials);

        /// <summary>
        /// Use the RemoveCredentials method to remove credentials from use. The credentials must match an existing target and scheme pair that you specified using the IBackgroundCopyJob2::<see cref="SetCredentials"/> method. There is no method to retrieve the credentials you have set.
        /// </summary>
        /// <param name="target">
        /// Identifies whether to use the credentials for proxy or server authentication.
        /// </param>
        /// <param name="scheme">
        /// Identifies the authentication scheme to use (basic or one of several challenge-response schemes). For details, see the <see cref="BGAuthScheme"/> enumeration. 
        /// </param>
        void RemoveCredentials(BGAuthTarget target, BGAuthScheme scheme);

        /// <summary>
        /// Replaces the remote prefix.
        /// </summary>
        /// <param name="oldPrefix">The old prefix.</param>
        /// <param name="newPrefix">The new prefix.</param>
        void ReplaceRemotePrefix([MarshalAs(UnmanagedType.LPWStr)] string oldPrefix, [MarshalAs(UnmanagedType.LPWStr)] string newPrefix);

        /// <summary>
        /// Adds the file with ranges.
        /// </summary>
        /// <param name="remoteUrl">The remote URL.</param>
        /// <param name="localName">Name of the local.</param>
        /// <param name="rangeCount">The range count.</param>
        /// <param name="ranges">The ranges.</param>
        void AddFileWithRanges(
            [MarshalAs(UnmanagedType.LPWStr)] string remoteUrl, 
            [MarshalAs(UnmanagedType.LPWStr)] string localName, 
            uint rangeCount, 
            [MarshalAs(UnmanagedType.LPArray)] BGFileRange[] ranges);

        /// <summary>
        /// Sets the file acl flags.
        /// </summary>
        /// <param name="flags">The flags.</param>
        void SetFileAclFlags(BGFileAclFlags flags);

        /// <summary>
        /// Gets the file acl flags.
        /// </summary>
        /// <param name="flags">The flags.</param>
        void GetFileAclFlags([Out] out BGFileAclFlags flags);
    }

    #endregion

    #region IBackgroundCopyJob4

    /// <summary>Use this interface to enable peer caching, restrict download time, and inspect user token characteristics.</summary>
    [ComImport]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("BC2C92DF-4972-4FA7-B8A0-444E127BA670")]
    internal interface IBackgroundCopyJob4
    {
        #region inherited

        /// <summary>
        /// Adds multiple files to the job
        /// </summary>
        /// <param name="fileCount">Number of elements in paFileSet.</param>
        /// <param name="fileSet">Array of <see cref="BGFileInfo"/> structures that identify the local and remote file names of the files to transfer.</param>
        void AddFileSet(uint fileCount, [MarshalAs(UnmanagedType.LPArray)] BGFileInfo[] fileSet);

        /// <summary>
        /// Adds a single file to the job
        /// </summary>
        /// <param name="remoteUrl"><see langword="null"/>-terminated string that contains the name of the file on the client. For information on specifying the local name, see the LocalName member and Remarks section of the <see cref="BGFileInfo"/> structure.</param>
        /// <param name="localName"><see langword="null"/>-terminated string that contains the name of the file on the server. For information on specifying the remote name, see the RemoteName member and Remarks section of the <see cref="BGFileInfo"/> structure.</param>
        void AddFile([MarshalAs(UnmanagedType.LPWStr)] string remoteUrl, [MarshalAs(UnmanagedType.LPWStr)] string localName);

        /// <summary>
        /// Returns an interface pointer to an enumerator
        /// object that you use to enumerate the files in the job
        /// </summary>
        /// <param name="enum"><see cref="IEnumBackgroundCopyFiles"/> interface pointer that you use to enumerate the files in the job. Release enumFiles when done.</param>
        void EnumFiles([MarshalAs(UnmanagedType.Interface)] out IEnumBackgroundCopyFiles @enum);

        /// <summary>
        /// Pauses the job
        /// </summary>
        void Suspend();

        /// <summary>
        /// Restarts a suspended job
        /// </summary>
        void Resume();

        /// <summary>
        /// Cancels the job and removes temporary files from the client
        /// </summary>
        void Cancel();

        /// <summary>
        /// Ends the job and saves the transferred files on the client
        /// </summary>
        void Complete();

        /// <summary>
        /// Retrieves the identifier of the job in the queue
        /// </summary>
        /// <param name="val">GUID that identifies the job within the BITS queue.</param>
        void GetId(out Guid val);

        /// <summary>
        /// Retrieves the type of transfer being performed,
        /// such as a file download
        /// </summary>
        /// <param name="val">Type of transfer being performed. For a list of transfer types, see the BGJob_TYPE enumeration type.</param>
        void GetType(out BGJobType val);

        /// <summary>
        /// Retrieves job-related progress information,
        /// such as the number of bytes and files transferred
        /// to the client
        /// </summary>
        /// <param name="val">Contains data that you can use to calculate the percentage of the job that is complete. For more information, see <see cref="BGJobProgress"/>.</param>
        void GetProgress(out BGJobProgress val);

        /// <summary>
        /// Retrieves timestamps for activities related
        /// to the job, such as the time the job was created
        /// </summary>
        /// <param name="val">Contains job-related time stamps. For available time stamps, see the BGJob_TIMES structure.</param>
        void GetTimes(out BGJobTimes val);

        /// <summary>
        /// Retrieves the state of the job
        /// </summary>
        /// <param name="val">Current state of the job. For example, the state reflects whether the job is in error, transferring data, or suspended. For a list of job states, see the <see cref="BGJobState"/> enumeration type.</param>
        void GetState(out BGJobState val);

        /// <summary>
        /// Retrieves an interface pointer to
        /// the error object after an error occurs
        /// </summary>
        /// <param name="error">Error interface that provides the error code, a description of the error, and the context in which the error occurred. This parameter also identifies the file being transferred at the time the error occurred. Release error when done.</param>
        void GetError([MarshalAs(UnmanagedType.Interface)] out IBackgroundCopyError error);

        /// <summary>
        /// Retrieves the job owner's identity
        /// </summary>
        /// <param name="val"><see langword="null"/>-terminated string that contains the string version of the SID that identifies the job's owner. Call the CoTaskMemFree function to free ppOwner when done.</param>
        void GetOwner([MarshalAs(UnmanagedType.LPWStr)] out string val);

        /// <summary>
        /// Specifies a display name that identifies the job in 
        ///   a user interface
        /// </summary>
        /// <param name="val">
        /// <see langword="null"/>-terminated string that identifies the job. Must not be <see langword="null"/>. The length of the string is limited to 256 characters, not including the <see langword="null"/> terminator. 
        /// </param>
        void SetDisplayName([MarshalAs(UnmanagedType.LPWStr)] string val);

        /// <summary>
        /// Retrieves the display name that identifies the job
        /// </summary>
        /// <param name="val">
        /// <see langword="null"/>-terminated string that contains the display name that identifies the job. More than one job can have the same display name. Call the CoTaskMemFree function to free displayName when done.
        /// </param>
        void GetDisplayName([MarshalAs(UnmanagedType.LPWStr)] out string val);

        /// <summary>
        /// Specifies a description of the job
        /// </summary>
        /// <param name="val">
        /// <see langword="null"/>-terminated string that provides additional information about the job. The length of the string is limited to 1,024 characters, not including the <see langword="null"/> terminator.
        /// </param>
        void SetDescription([MarshalAs(UnmanagedType.LPWStr)] string val);

        /// <summary>
        /// Retrieves the description of the job
        /// </summary>
        /// <param name="val">
        /// <see langword="null"/>-terminated string that contains a short description of the job. Call the CoTaskMemFree function to free ppDescription when done. 
        /// </param>
        void GetDescription([MarshalAs(UnmanagedType.LPWStr)] out string val);

        /// <summary>
        /// Specifies the priority of the job relative to 
        ///   other jobs in the transfer queue
        /// </summary>
        /// <param name="val">
        /// Specifies the priority level of your job relative to other jobs in the transfer queue. The default is BGJobPriorityNormal. For a list of priority levels, see the <see cref="BGJobPriority"/> enumeration. 
        /// </param>
        void SetPriority(BGJobPriority val);

        /// <summary>
        /// Retrieves the priority level you have set for the job.
        /// </summary>
        /// <param name="val">
        /// Priority of the job relative to other jobs in the transfer queue. 
        /// </param>
        void GetPriority(out BGJobPriority val);

        /// <summary>
        /// Specifies the type of event notification to receive
        /// </summary>
        /// <param name="val">
        /// Set one or more of the following flags to identify the events that you want to receive. 
        /// </param>
        void SetNotifyFlags([MarshalAs(UnmanagedType.U4)] BGJobNotificationTypes val);

        /// <summary>
        /// Retrieves the event notification (callback) flags 
        ///   you have set for your application.
        /// </summary>
        /// <param name="val">
        /// Identifies the events that your application receives. The following table lists the event notification flag values. 
        /// </param>
        void GetNotifyFlags(out uint val);

        /// <summary>
        /// Specifies a pointer to your implementation of the 
        ///   <see cref="IBackgroundCopyCallback"/> interface (callbacks). The 
        ///   interface receives notification based on the event 
        ///   notification flags you set
        /// </summary>
        /// <param name="val">
        /// An <see cref="IBackgroundCopyCallback"/> interface pointer. To remove the current callback interface pointer, set this parameter to <see langword="null"/>.
        /// </param>
        void SetNotifyInterface([MarshalAs(UnmanagedType.IUnknown)] object val);

        /// <summary>
        /// Retrieves a pointer to your implementation 
        ///   of the <see cref="IBackgroundCopyCallback"/> interface (callbacks).
        /// </summary>
        /// <param name="val">
        /// Interface pointer to your implementation of the <see cref="IBackgroundCopyCallback"/> interface. When done, release ppNotifyInterface.
        /// </param>
        void GetNotifyInterface([MarshalAs(UnmanagedType.IUnknown)] out object val);

        /// <summary>
        /// Specifies the minimum length of time that BITS waits after 
        ///   encountering a transient error condition before trying to 
        ///   transfer the file
        /// </summary>
        /// <param name="seconds">
        /// Minimum length of time, in seconds, that BITS waits after encountering a transient error before trying to transfer the file. The default retry delay is 600 seconds (10 minutes). The minimum retry delay that you can specify is 60 seconds. If you specify a value less than 60 seconds, BITS changes the value to 60 seconds. If the value exceeds the no-progress-timeout value retrieved from the <see cref="GetNoProgressTimeout"/> method, BITS will not retry the transfer and moves the job to the BGJobStateError state. 
        /// </param>
        void SetMinimumRetryDelay(uint seconds);

        /// <summary>
        /// Retrieves the minimum length of time that BITS waits after 
        ///   encountering a transient error condition before trying to 
        ///   transfer the file
        /// </summary>
        /// <param name="seconds">
        /// Length of time, in seconds, that the service waits after encountering a transient error before trying to transfer the file. 
        /// </param>
        void GetMinimumRetryDelay(out uint seconds);

        /// <summary>
        /// Specifies the length of time that BITS continues to try to 
        ///   transfer the file after encountering a transient error 
        ///   condition
        /// </summary>
        /// <param name="seconds">
        /// Length of time, in seconds, that BITS tries to transfer the file after the first transient error occurs. The default retry period is 1,209,600 seconds (14 days). Set the retry period to 0 to prevent retries and to force the job into the BGJobStateError state for all errors. If the retry period value exceeds the JobInactivityTimeout Group Policy value (90-day default), BITS cancels the job after the policy value is exceeded.
        /// </param>
        void SetNoProgressTimeout(uint seconds);

        /// <summary>
        /// Retrieves the length of time that BITS continues to try to 
        ///   transfer the file after encountering a transient error condition
        /// </summary>
        /// <param name="seconds">
        /// Length of time, in seconds, that the service tries to transfer the file after a transient error occurs. 
        /// </param>
        void GetNoProgressTimeout(out uint seconds);

        /// <summary>
        /// Retrieves the number of times the job was interrupted by 
        ///   network failure or server unavailability
        /// </summary>
        /// <param name="errors">
        /// Number of errors that occurred while BITS tried to transfer the job. The count increases when the job moves from the BGJobStateTransferring state to the BGJobStateTransientError or BGJobStateError state.
        /// </param>
        void GetErrorCount(out ulong errors);

        /// <summary>
        /// Specifies which proxy to use to transfer the files
        /// </summary>
        /// <param name="proxyUsage">
        /// Specifies whether to use the user's proxy settings, not to use a proxy, or to use application-specified proxy settings. The default is to use the user's proxy settings, BGJobProxyUsagePreConfig. For a list of proxy options, see the <see cref="BGJobProxyUsage"/> enumeration.
        /// </param>
        /// <param name="proxyList">
        /// <see langword="null"/>-terminated string that contains the proxies to use to transfer files. The list is space-delimited. For details on specifying a proxy, see Remarks. This parameter must be <see langword="null"/> if the value of <see cref="ProxyUsage"/> is BGJobProxyUsagePreConfig, BGJobProxyUsageNoProxy, or BGJobProxyUsageNoAutoDetect. The length of the proxy list is limited to 4,000 characters, not including the <see langword="null"/> terminator. 
        /// </param>
        /// <param name="proxyBypassList">
        /// <see langword="null"/>-terminated string that contains an optional list of host names, IP addresses, or both, that can bypass the proxy. The list is space-delimited. For details on specifying a bypass proxy, see Remarks. This parameter must be <see langword="null"/> if the value of <see cref="ProxyUsage"/> is BGJobProxyUsagePreConfig, BGJobProxyUsageNoProxy, or BGJobProxyUsageNoAutoDetect. The length of the proxy bypass list is limited to 4,000 characters, not including the <see langword="null"/> terminator. 
        /// </param>
        void SetProxySettings(BGJobProxyUsage proxyUsage, [MarshalAs(UnmanagedType.LPWStr)] string proxyList, [MarshalAs(UnmanagedType.LPWStr)] string proxyBypassList);

        /// <summary>
        /// Retrieves the proxy settings the job uses to transfer the files
        /// </summary>
        /// <param name="proxyUsage">
        /// Specifies the proxy settings the job uses to transfer the files. For a list of proxy options, see the <see cref="BGJobProxyUsage"/> enumeration. 
        /// </param>
        /// <param name="proxyList">
        /// <see langword="null"/>-terminated string that contains one or more proxies to use to transfer files. The list is space-delimited. For details on the format of the string, see the Listing Proxy Servers section of Enabling Internet Functionality. Call the CoTaskMemFree function to free <paramref name="proxyList"/> when done.
        /// </param>
        /// <param name="proxyBypassList">
        /// <see langword="null"/>-terminated string that contains an optional list of host names or IP addresses, or both, that were not routed through the proxy. The list is space-delimited. For details on the format of the string, see the Listing the Proxy Bypass section of Enabling Internet Functionality. Call the CoTaskMemFree function to free <paramref name="proxyBypassList"/> when done.
        /// </param>
        void GetProxySettings(
            out BGJobProxyUsage proxyUsage, [MarshalAs(UnmanagedType.LPWStr)] out string proxyList, [MarshalAs(UnmanagedType.LPWStr)] out string proxyBypassList);

        /// <summary>
        /// Changes the ownership of the job to the current user
        /// </summary>
        void TakeOwnership();

        /// <summary>
        /// Use the SetNotifyCmdLine method to specify a program to execute if the job enters the BGJobStateError or BGJobStateTransferred state. BITS executes the program in the context of the user.
        /// </summary>
        /// <param name="program">
        /// <see langword="null"/>-terminated string that contains the program to execute. The program parameter is limited to MAX_PATH characters, not including the <see langword="null"/> terminator. You should specify a full path to the program; the method will not use the search path to locate the program. To remove command line notification, set program and parameters to <see langword="null"/>. The method fails if program is <see langword="null"/> and parameters is non-<see langword="null"/>. 
        /// </param>
        /// <param name="parameters">
        /// <see langword="null"/>-terminated string that contains the parameters of the program in program. The first parameter must be the program in program (use quotes if the path uses long file names). The parameters parameter is limited to 4,000 characters, not including the <see langword="null"/> terminator. This parameter can be <see langword="null"/>.
        /// </param>
        void SetNotifyCmdLine([MarshalAs(UnmanagedType.LPWStr)] string program, [MarshalAs(UnmanagedType.LPWStr)] string parameters);

        /// <summary>
        /// Use the GetNotifyCmdLine method to retrieve the program to execute when the job enters the error or transferred state.
        /// </summary>
        /// <param name="program">
        /// <see langword="null"/>-terminated string that contains the program to execute when the job enters the error or transferred state. Call the CoTaskMemFree function to free program when done. 
        /// </param>
        /// <param name="parameters">
        /// <see langword="null"/>-terminated string that contains the arguments of the program in program. Call the CoTaskMemFree function to free parameters when done. 
        /// </param>
        void GetNotifyCmdLine([MarshalAs(UnmanagedType.LPWStr)] out string program, [MarshalAs(UnmanagedType.LPWStr)] out string parameters);

        /// <summary>
        /// Use the GetReplyProgress method to retrieve progress information related to the transfer of the reply data from an upload-reply job.
        /// </summary>
        /// <param name="progress">
        /// Contains information that you use to calculate the percentage of the reply file transfer that is complete. For more information, see <see cref="BGJobReplyProgress"/>.
        /// </param>
        void GetReplyProgress([Out] out BGJobReplyProgress progress);

        /// <summary>
        /// Use the GetReplyData method to retrieve an in-memory copy of the reply data from the server application. Only call this method if the job's type is BGJobTypeUploadReply and its state is BGJobStateTransferred.
        /// </summary>
        /// <param name="buffer">
        /// Buffer to contain the reply data. The method sets buffer to <see langword="null"/> if the server application did not return a reply. Call the CoTaskMemFree function to free buffer when done.
        /// </param>
        /// <param name="length">
        /// Size, in bytes, of the reply data in buffer.
        /// </param>
        void GetReplyData(IntPtr buffer, out ulong length);

        /// <summary>
        /// Use the SetReplyFileName method to specify the name of the file to contain the reply data from the server application. Only call this method if the job's type is BGJobTypeUploadReply.
        /// </summary>
        /// <param name="replyFileName">
        /// <see langword="null"/>-terminated string that contains the full path to the reply file. BITS generates the file name if ReplyFileNamePathSpec is <see langword="null"/> or an empty string. You cannot use wild cards in the path or file name, and directories in the path must exist. The path is limited to MAX_PATH, not including the <see langword="null"/> terminator. The user must have permissions to write to the directory. BITS does not support NTFS streams. Instead of using network drives, which are session specific, use UNC paths (for example, \\server\share\path\file). Do not include the \\? prefix in the path. 
        /// </param>
        void SetReplyFileName([MarshalAs(UnmanagedType.LPWStr)] string replyFileName);

        /// <summary>
        /// Use the GetReplyFileName method to retrieve the name of the file that contains the reply data from the server application. Only call this method if the job type is BGJobTypeUploadReply.
        /// </summary>
        /// <param name="replyFileName">
        /// <see langword="null"/>-terminated string that contains the full path to the reply file. Call the CoTaskMemFree function to free <paramref name="replyFileName"/> when done. 
        /// </param>
        void GetReplyFileName([MarshalAs(UnmanagedType.LPWStr)] out string replyFileName);

        /// <summary>
        /// Use the SetCredentials method to specify the credentials to use for a proxy or remote server user authentication request.
        /// </summary>
        /// <param name="credentials">
        /// Identifies the target (proxy or server), authentication scheme, and the user's credentials to use for user authentication. For details, see the <see cref="BGAuthCredentials"/> structure. If the job currently contains credentials with the same target and scheme pair, the existing credentials are replaced with the new credentials. The credentials persist for the life of the job. To remove the credentials from the job, call the IBackgroundCopyJob2::<see cref="RemoveCredentials"/> method. 
        /// </param>
        void SetCredentials([In] ref BGAuthCredentials credentials);

        /// <summary>
        /// Use the RemoveCredentials method to remove credentials from use. The credentials must match an existing target and scheme pair that you specified using the IBackgroundCopyJob2::<see cref="SetCredentials"/> method. There is no method to retrieve the credentials you have set.
        /// </summary>
        /// <param name="target">
        /// Identifies whether to use the credentials for proxy or server authentication.
        /// </param>
        /// <param name="scheme">
        /// Identifies the authentication scheme to use (basic or one of several challenge-response schemes). For details, see the <see cref="BGAuthScheme"/> enumeration. 
        /// </param>
        void RemoveCredentials(BGAuthTarget target, BGAuthScheme scheme);

        /// <summary>
        /// Replaces the remote prefix.
        /// </summary>
        /// <param name="oldPrefix">The old prefix.</param>
        /// <param name="newPrefix">The new prefix.</param>
        void ReplaceRemotePrefix([MarshalAs(UnmanagedType.LPWStr)] string oldPrefix, [MarshalAs(UnmanagedType.LPWStr)] string newPrefix);

        /// <summary>
        /// Adds the file with ranges.
        /// </summary>
        /// <param name="remoteUrl">The remote URL.</param>
        /// <param name="localName">Name of the local.</param>
        /// <param name="rangeCount">The range count.</param>
        /// <param name="ranges">The ranges.</param>
        void AddFileWithRanges(
            [MarshalAs(UnmanagedType.LPWStr)] string remoteUrl, 
            [MarshalAs(UnmanagedType.LPWStr)] string localName, 
            uint rangeCount, 
            [MarshalAs(UnmanagedType.LPArray)] BGFileRange[] ranges);

        /// <summary>
        /// Sets the file acl flags.
        /// </summary>
        /// <param name="flags">The flags.</param>
        void SetFileAclFlags(BGFileAclFlags flags);

        /// <summary>
        /// Gets the file acl flags.
        /// </summary>
        /// <param name="flags">The flags.</param>
        void GetFileAclFlags([Out] out BGFileAclFlags flags);

        #endregion

        /// <summary>
        /// Sets the peer caching flags.
        /// </summary>
        /// <param name="flags">The flags.</param>
        void SetPeerCachingFlags(PeerCachingFlagss flags);

        /// <summary>
        /// Gets the peer caching flags.
        /// </summary>
        /// <param name="flags">The flags.</param>
        void GetPeerCachingFlags([Out] out PeerCachingFlagss flags);

        /// <summary>
        /// Gets the owner integrity level.
        /// </summary>
        /// <param name="level">The p level.</param>
        void GetOwnerIntegrityLevel([Out] out ulong level);

        /// <summary>
        /// Gets the state of the owner elevation.
        /// </summary>
        /// <param name="elevated">if set to <see langword="true"/> [p elevated].</param>
        void GetOwnerElevationState([Out] out bool elevated);

        /// <summary>
        /// Sets the maximum download time.
        /// </summary>
        /// <param name="timeout">The timeout.</param>
        void SetMaximumDownloadTime(ulong timeout);

        /// <summary>
        /// Gets the maximum download time.
        /// </summary>
        /// <param name="timeout">The number of milliseconds to pass before the download times out</param>
        void GetMaximumDownloadTime([Out] out ulong timeout);
    }

    #endregion

    #region IBackgroundCopyError

    /// <summary>
    /// Use the information in the IBackgroundCopyError interface to determine the cause of the error and if the transfer
    /// process can proceed.
    /// </summary>
    [GuidAttribute("19C613A0-FCB8-4F28-81AE-897C3D078F81")]
    [InterfaceTypeAttribute(ComInterfaceType.InterfaceIsIUnknown)]
    [ComImportAttribute]
    internal interface IBackgroundCopyError
    {
        /// <summary>
        /// Retrieves the error code and identify the context
        /// in which the error occurred
        /// </summary>
        /// <param name="context">Context in which the error occurred. For a list of context values, see the <see cref="BGErrorContext"/> enumeration.</param>
        /// <param name="code">Error code of the error that occurred.</param>
        void GetError(out BGErrorContext context, [MarshalAs(UnmanagedType.Error)] out int code);

        /// <summary>
        /// Retrieves an interface pointer to the file object
        /// associated with the error
        /// </summary>
        /// <param name="val">An <see cref="IBackgroundCopyFile"/> interface pointer whose methods you use to determine the local and remote file names associated with the error. The file parameter is set to <see langword="null"/> if the error is not associated with the local or remote file. When done, release file.</param>
        void GetFile([MarshalAs(UnmanagedType.Interface)] out IBackgroundCopyFile val);

        /// <summary>
        /// Retrieves the error text associated with the error
        /// </summary>
        /// <param name="languageId">Identifies the locale to use to generate the description. To create the language identifier, use the MAKELANGID macro.</param>
        /// <param name="errorDescription"><see langword="null"/>-terminated string that contains the error text associated with the error. Call the CoTaskMemFree function to free <paramref name="errorDescription"/> when done.</param>
        void GetErrorDescription(uint languageId, [MarshalAs(UnmanagedType.LPWStr)] out string errorDescription);

        /// <summary>
        /// Retrieves a description of the context in which the error occurred
        /// </summary>
        /// <param name="languageId">Identifies the locale to use to generate the description. To create the language identifier, use the MAKELANGID macro.</param>
        /// <param name="contextDescription"><see langword="null"/>-terminated string that contains the description of the context in which the error occurred. Call the CoTaskMemFree function to free ppContextDescription when done.</param>
        void GetErrorContextDescription(uint languageId, [MarshalAs(UnmanagedType.LPWStr)] out string contextDescription);

        /// <summary>
        /// Retrieves the protocol used to transfer the file
        /// </summary>
        /// <param name="protocol"><see langword="null"/>-terminated string that contains the protocol used to transfer the file. The string contains HTTP for the HTTP protocol and file for the SMB protocol. The ppProtocol parameter is set to <see langword="null"/> if the error is not related to the transfer protocol. Call the CoTaskMemFree function to free ppProtocol when done.</param>
        void GetProtocol([MarshalAs(UnmanagedType.LPWStr)] out string protocol);
    }

    #endregion

    #region IEnumBackgroundCopyJobs

    /// <summary>
    /// Use the IEnumBackgroundCopyJobs interface to enumerate the list of jobs in the transfer queue. To get an <see cref="IEnumBackgroundCopyJobs"/> interface pointer, call the <see
    ///   cref="IBackgroundCopyManager"/>:: EnumJobs method.
    /// </summary>
    [InterfaceTypeAttribute(ComInterfaceType.InterfaceIsIUnknown)]
    [GuidAttribute("1AF4F612-3B71-466F-8F58-7B6F73AC57AD")]
    [ComImportAttribute]
    internal interface IEnumBackgroundCopyJobs
    {
        /// <summary>
        /// Retrieves a specified number of items in the enumeration sequence
        /// </summary>
        /// <param name="celt">Number of elements requested.</param>
        /// <param name="copyJob">Array of <see cref="IBackgroundCopyJob"/> objects. You must release each object in <paramref name="copyJob"/> when done.</param>
        /// <param name="celtFetched">Number of elements returned in <paramref name="copyJob"/>. You can set fetched to <see langword="null"/> if <paramref name="copyJob"/> is one. Otherwise, initialize the value of fetched to 0 before calling this method.</param>
        void Next(uint celt, [MarshalAs(UnmanagedType.Interface)] out IBackgroundCopyJob copyJob, out uint celtFetched);

        /// <summary>
        /// Skips a specified number of items in the enumeration sequence
        /// </summary>
        /// <param name="celt">
        /// Number of elements to skip. 
        /// </param>
        void Skip(uint celt);

        /// <summary>
        /// Resets the enumeration sequence to the beginning.
        /// </summary>
        void Reset();

        /// <summary>
        /// Creates another enumerator that contains the same 
        ///   enumeration state as the current one
        /// </summary>
        /// <param name="enum">
        /// Receives the interface pointer to the enumeration object. If the method is unsuccessful, the value of this output variable is undefined. You must release enumJobs when done.
        /// </param>
        void Clone([MarshalAs(UnmanagedType.Interface)] out IEnumBackgroundCopyJobs @enum);

        /// <summary>
        /// Returns the number of items in the enumeration
        /// </summary>
        /// <param name="count">
        /// Number of jobs in the enumeration.
        /// </param>
        void GetCount(out uint count);
    }

    #endregion

    #region IEnumBackgroundCopyFiles

    /// <summary>
    /// Use the IEnumBackgroundCopyFiles interface to enumerate the files
    /// that a job contains. To get an IEnumBackgroundCopyFiles interface pointer, call the <see cref="IBackgroundCopyJob"/>::EnumFiles method.
    /// </summary>
    [InterfaceTypeAttribute(ComInterfaceType.InterfaceIsIUnknown)]
    [GuidAttribute("CA51E165-C365-424C-8D41-24AAA4FF3C40")]
    [ComImportAttribute]
    internal interface IEnumBackgroundCopyFiles
    {
        /// <summary>
        /// Retrieves a specified number of items in the enumeration sequence
        /// </summary>
        /// <param name="celt">Number of elements requested.</param>
        /// <param name="copyFile">Array of <see cref="IBackgroundCopyFile"/> objects. You must release each object in <paramref name="copyFile"/> when done.</param>
        /// <param name="fetched">Number of elements returned in <paramref name="copyFile"/>. You can set fetched to <see langword="null"/> if <paramref name="celt"/> is one. Otherwise, initialize the value of fetched to 0 before calling this method.</param>
        void Next(uint celt, [MarshalAs(UnmanagedType.Interface)] out IBackgroundCopyFile copyFile, out uint fetched);

        /// <summary>
        /// Skips a specified number of items in the enumeration sequence
        /// </summary>
        /// <param name="celt">
        /// Number of elements to skip.
        /// </param>
        void Skip(uint celt);

        /// <summary>
        /// Resets the enumeration sequence to the beginning
        /// </summary>
        void Reset();

        /// <summary>
        /// Creates another enumerator that contains the same 
        ///   enumeration state as the current enumerator
        /// </summary>
        /// <param name="enum">
        /// Receives the interface pointer to the enumeration object. If the method is unsuccessful, the value of this output variable is undefined. You must release enumFiles when done.
        /// </param>
        void Clone([MarshalAs(UnmanagedType.Interface)] out IEnumBackgroundCopyFiles @enum);

        /// <summary>
        /// Retrieves the number of items in the enumeration
        /// </summary>
        /// <param name="count">
        /// Number of files in the enumeration.
        /// </param>
        void GetCount(out uint count);
    }

    #endregion

    #region IBackgroundCopyFile

    /// <summary>
    /// The IBackgroundCopyFile interface contains information about a file
    ///   that is part of a job. For example, you can use the interfaces methods to retrieve the local and remote names of the
    ///   file and transfer progress information.
    /// </summary>
    [GuidAttribute("01B7BD23-FB88-4A77-8490-5891D3E4653A")]
    [InterfaceTypeAttribute(ComInterfaceType.InterfaceIsIUnknown)]
    [ComImportAttribute]
    internal interface IBackgroundCopyFile
    {
        /// <summary>
        /// Retrieves the remote name of the file
        /// </summary>
        /// <param name="val">
        /// <see langword="null"/>-terminated string that contains the remote name of the file to transfer. The name is fully qualified. Call the CoTaskMemFree function to free ppName when done. 
        /// </param>
        void GetRemoteName([MarshalAs(UnmanagedType.LPWStr)] out string val);

        /// <summary>
        /// Retrieves the local name of the file
        /// </summary>
        /// <param name="val">
        /// <see langword="null"/>-terminated string that contains the name of the file on the client.
        /// </param>
        void GetLocalName([MarshalAs(UnmanagedType.LPWStr)] out string val);

        /// <summary>
        /// Retrieves the progress of the file transfer
        /// </summary>
        /// <param name="val">
        /// Structure whose members indicate the progress of the file transfer. For details on the type of progress information available, see the BG_FILEProgress structure.
        /// </param>
        void GetProgress(out BGFileProgress val);
    }

    #endregion

    #region IBackgroundCopyFile2

    /// <summary>
    /// The IBackgroundCopyFile2 interface contains information about a file
    ///   that is part of a job. The IBackgroundCopyFile2 interface is used to specify a new remote name for the file and
    ///   retrieve the list of ranges to download.
    /// </summary>
    [GuidAttribute("83E81B93-0873-474D-8A8C-F2018B1A939C")]
    [InterfaceTypeAttribute(ComInterfaceType.InterfaceIsIUnknown)]
    [ComImportAttribute]
    internal interface IBackgroundCopyFile2
    {
        /// <summary>
        /// Retrieves the remote name of the file
        /// </summary>
        /// <param name="val">
        /// <see langword="null"/>-terminated string that contains the remote name of the file to transfer. The name is fully qualified. Call the CoTaskMemFree function to free ppName when done. 
        /// </param>
        void GetRemoteName([MarshalAs(UnmanagedType.LPWStr)] out string val);

        /// <summary>
        /// Retrieves the local name of the file
        /// </summary>
        /// <param name="val">
        /// <see langword="null"/>-terminated string that contains the name of the file on the client.
        /// </param>
        void GetLocalName([MarshalAs(UnmanagedType.LPWStr)] out string val);

        /// <summary>
        /// Retrieves the progress of the file transfer
        /// </summary>
        /// <param name="val">
        /// Structure whose members indicate the progress of the file transfer. For details on the type of progress information available, see the BG_FILEProgress structure.
        /// </param>
        void GetProgress(out BGFileProgress val);

        /// <summary>
        /// Retrieves the ranges that could be downloaded from the remote file
        /// </summary>
        /// <param name="rangeCount">
        /// Number of elements in Ranges
        /// </param>
        /// <param name="ranges">
        /// Array of BG_FILE_RANGE structures that specify the ranges to download
        /// </param>
        void GetFileRanges(out uint rangeCount, out IntPtr /*BG_FILE_RANGE[]*/ ranges);

        /// <summary>
        /// Changes the remote name to a new Url in a download job
        /// </summary>
        /// <param name="remoteName">
        /// <see langword="null"/>-terminated string that contains the name of the file on the server
        /// </param>
        void SetRemoteName([MarshalAs(UnmanagedType.LPWStr)] string remoteName);
    }

    #endregion

    /// <summary>Entry point to the BITS infrastructure.</summary>
    [Guid("4991D34B-80A1-4291-83B6-3328366B9097")]
    [ClassInterfaceAttribute(ClassInterfaceType.None)]
    [ComImportAttribute]
    [SecurityPermission(SecurityAction.Demand, UnmanagedCode = true)]
    internal class BackgroundCopyManager
    {
    }
}