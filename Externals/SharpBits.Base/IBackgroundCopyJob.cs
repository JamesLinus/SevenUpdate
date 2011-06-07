// ***********************************************************************
// <copyright file="IBackgroundCopyJob.cs" project="SharpBits.Base" assembly="SharpBits.Base" solution="SevenUpdate" company="Xidar Solutions">
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
    ///   Use the IBackgroundCopyJob interface to add files to the job, set the priority level of the job,
    ///   determine the state of the job, and to start and stop the job.
    /// </summary>
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [GuidAttribute("37668D37-507E-4160-9316-26306D150B12")]
    [ComImportAttribute]
    internal interface IBackgroundCopyJob
    {
        /// <summary>
        ///   Adds multiple files to the job.
        /// </summary>
        /// <param name="fileCount">
        ///   Number of elements in paFileSet.
        /// </param>
        /// <param name="fileSet">
        ///   Array of <see cref="BGFileInfo" /> structures that identify the local and remote file names of the files to transfer.
        /// </param>
        void AddFileSet(uint fileCount, [MarshalAs(UnmanagedType.LPArray)] BGFileInfo[] fileSet);

        /// <summary>
        ///   Adds a single file to the job.
        /// </summary>
        /// <param name="remoteUrl">
        ///   A string that contains the name of the file on the client. For information on specifying the local name, see the LocalName member and Remarks section of the <see
        ///    cref="BGFileInfo" /> structure.
        /// </param>
        /// <param name="localName">
        ///   A string that contains the name of the file on the server. For information on specifying the remote name, see the RemoteName member and Remarks section of the <see
        ///    cref="BGFileInfo" /> structure.
        /// </param>
        void AddFile(
            [MarshalAs(UnmanagedType.LPWStr)] string remoteUrl, [MarshalAs(UnmanagedType.LPWStr)] string localName);

        /// <summary>
        ///   Returns an interface pointer to an enumerator object that you use to enumerate the files in the job.
        /// </summary>
        /// <param name="enum">
        ///   <see cref="IEnumBackgroundCopyFiles" /> interface pointer that you use to enumerate the files in the job. Release p@enumFiles when done.
        /// </param>
        void EnumFiles([MarshalAs(UnmanagedType.Interface)] out IEnumBackgroundCopyFiles @enum);

        /// <summary>
        ///   Pauses the job.
        /// </summary>
        void Suspend();

        /// <summary>
        ///   Restarts a suspended job.
        /// </summary>
        void Resume();

        /// <summary>
        ///   Cancels the job and removes temporary files from the client.
        /// </summary>
        void Cancel();

        /// <summary>
        ///   Ends the job and saves the transferred files on the client.
        /// </summary>
        void Complete();

        /// <summary>
        ///   Retrieves the identifier of the job in the queue.
        /// </summary>
        /// <param name="val">
        ///   GUID that identifies the job within the BITS queue.
        /// </param>
        void GetId(out Guid val);

        /// <summary>
        ///   Retrieves the type of transfer being performed, such as a file download.
        /// </summary>
        /// <param name="val">
        ///   Type of transfer being performed. For a list of transfer types, see the BGJob_TYPE enumeration type.
        /// </param>
        void GetType(out BGJobType val);

        /// <summary>
        ///   Retrieves job-related progress information, such as the number of bytes and files transferred to the client.
        /// </summary>
        /// <param name="val">
        ///   Contains data that you can use to calculate the percentage of the job that is complete. For more information, see <see
        ///    cref="BGJobProgress" />.
        /// </param>
        void GetProgress(out BGJobProgress val);

        /// <summary>
        ///   Retrieves timestamps for activities related to the job, such as the time the job was created.
        /// </summary>
        /// <param name="val">
        ///   Contains job-related time stamps. For available time stamps, see the <see cref="BGJobTimes" /> structure.
        /// </param>
        void GetTimes(out BGJobTimes val);

        /// <summary>
        ///   Retrieves the state of the job.
        /// </summary>
        /// <param name="val">
        ///   Current state of the job. For example, the state reflects whether the job is in error, transferring data, or suspended. For a list of job states, see the <see
        ///    cref="BGJobState" /> enumeration type. .
        /// </param>
        void GetState(out BGJobState val);

        /// <summary>
        ///   Retrieves an interface pointer to the error object after an error occurs.
        /// </summary>
        /// <param name="error">
        ///   Error interface that provides the error code, a description of the error, and the context in which the error occurred. This parameter also identifies the file being transferred at the time the error occurred. Release error when done.
        /// </param>
        void GetError([MarshalAs(UnmanagedType.Interface)] out IBackgroundCopyError error);

        /// <summary>
        ///   Retrieves the job owner's identity.
        /// </summary>
        /// <param name="val">
        ///   A string that contains the string version of the SID that identifies the job's owner. Call the CoTaskMemFree function to free ppOwner when done. .
        /// </param>
        void GetOwner([MarshalAs(UnmanagedType.LPWStr)] out string val);

        /// <summary>
        ///   Specifies a display name that identifies the job in a user interface.
        /// </summary>
        /// <param name="val">
        ///   A string that identifies the job. Must not be <c>null</c>. The length of the string is limited to 256 characters, not including the <c>null</c> terminator.
        /// </param>
        void SetDisplayName([MarshalAs(UnmanagedType.LPWStr)] string val);

        /// <summary>
        ///   Retrieves the display name that identifies the job.
        /// </summary>
        /// <param name="val">
        ///   A string that contains the display name that identifies the job. More than one job can have the same display name. Call the CoTaskMemFree function to free displayName when done.
        /// </param>
        void GetDisplayName([MarshalAs(UnmanagedType.LPWStr)] out string val);

        /// <summary>
        ///   Specifies a description of the job.
        /// </summary>
        /// <param name="val">
        ///   A string that provides additional information about the job. The length of the string is limited to 1,024 characters, not including the <c>null</c> terminator.
        /// </param>
        void SetDescription([MarshalAs(UnmanagedType.LPWStr)] string val);

        /// <summary>
        ///   Retrieves the description of the job.
        /// </summary>
        /// <param name="val">
        ///   A string that contains a short description of the job. Call the CoTaskMemFree function to free ppDescription when done.
        /// </param>
        void GetDescription([MarshalAs(UnmanagedType.LPWStr)] out string val);

        /// <summary>
        ///   Specifies the priority of the job relative to other jobs in the transfer queue.
        /// </summary>
        /// <param name="val">
        ///   Specifies the priority level of your job relative to other jobs in the transfer queue. The default is <see
        ///    cref="BGJobPriority" />.Normal. For a list of priority levels, see the <see cref="BGJobPriority" /> enumeration.
        /// </param>
        void SetPriority(BGJobPriority val);

        /// <summary>
        ///   Retrieves the priority level you have set for the job.
        /// </summary>
        /// <param name="val">
        ///   Priority of the job relative to other jobs in the transfer queue.
        /// </param>
        void GetPriority(out BGJobPriority val);

        /// <summary>
        ///   Specifies the type of event notification to receive.
        /// </summary>
        /// <param name="val">
        ///   Set one or more of the following flags to identify the events that you want to receive.
        /// </param>
        void SetNotifyFlags(BGJobNotificationTypes val);

        /// <summary>
        ///   Retrieves the event notification (callback) flags you have set for your application.
        /// </summary>
        /// <param name="val">
        ///   Identifies the events that your application receives. The following table lists the event notification flag values.
        /// </param>
        void GetNotifyFlags(out BGJobNotificationTypes val);

        /// <summary>
        ///   Specifies a pointer to your implementation of the<see cref="IBackgroundCopyCallback" /> interface (callbacks). The interface receives notification based on the event notification flags you set.
        /// </summary>
        /// <param name="val">
        ///   An <see cref="IBackgroundCopyCallback" /> interface pointer. To remove the current callback interface pointer, set this parameter to <c>null</c>.
        /// </param>
        void SetNotifyInterface([MarshalAs(UnmanagedType.IUnknown)] object val);

        /// <summary>
        ///   Retrieves a pointer to your implementation of the <see cref="IBackgroundCopyCallback" /> interface (callbacks).
        /// </summary>
        /// <param name="val">
        ///   Interface pointer to your implementation of the <see cref="IBackgroundCopyCallback" /> interface. When done, release notifyInterface.
        /// </param>
        void GetNotifyInterface([MarshalAs(UnmanagedType.IUnknown)] out object val);

        /// <summary>
        ///   Specifies the minimum length of time that BITS waits after encountering a transient error condition before trying to transfer the file.
        /// </summary>
        /// <param name="seconds">
        ///   Minimum length of time, in seconds, that BITS waits after encountering a transient error before trying to transfer the file. The default retry delay is 600 seconds (10 minutes). The minimum retry delay that you can specify is 60 seconds. If you specify a value less than 60 seconds, BITS changes the value to 60 seconds. If the value exceeds the no-progress-timeout value retrieved from the <see
        ///    cref="GetNoProgressTimeout" /> method, BITS will not retry the transfer and moves the job to the BGJobStateError state.
        /// </param>
        void SetMinimumRetryDelay(uint seconds);

        /// <summary>
        ///   Retrieves the minimum length of time that BITS waits after encountering a transient error condition before trying to transfer the file.
        /// </summary>
        /// <param name="seconds">
        ///   Length of time, in seconds, that the service waits after encountering a transient error before trying to transfer the file.
        /// </param>
        void GetMinimumRetryDelay(out uint seconds);

        /// <summary>
        ///   Specifies the length of time that BITS continues to try to transfer the file after encountering a transient error condition.
        /// </summary>
        /// <param name="seconds">
        ///   Length of time, in seconds, that BITS tries to transfer the file after the first transient error occurs. The default retry period is 1,209,600 seconds (14 days). Set the retry period to 0 to prevent retries and to force the job into the BGJobStateError state for all errors. If the retry period value exceeds the JobInactivityTimeout Group Policy value (90-day default), BITS cancels the job after the policy value is exceeded.
        /// </param>
        void SetNoProgressTimeout(uint seconds);

        /// <summary>
        ///   Retrieves the length of time that BITS continues to try to transfer the file after encountering a transient error condition.
        /// </summary>
        /// <param name="seconds">
        ///   Length of time, in seconds, that the service tries to transfer the file after a transient error occurs.
        /// </param>
        void GetNoProgressTimeout(out uint seconds);

        /// <summary>
        ///   Retrieves the number of times the job was interrupted by network failure or server unavailability.
        /// </summary>
        /// <param name="errors">
        ///   Number of errors that occurred while BITS tried to transfer the job. The count increases when the job moves from the BGJobStateTransferring state to the BGJobStateTransientError or BGJobStateError state.
        /// </param>
        void GetErrorCount(out ulong errors);

        /// <summary>
        ///   Specifies which proxy to use to transfer the files.
        /// </summary>
        /// <param name="proxyUsage">
        ///   Specifies whether to use the user's proxy settings, not to use a proxy, or to use application-specified proxy settings. The default is to use the user's proxy settings, BGJobProxyUsagePreConfig. For a list of proxy options, see the <see
        ///    cref="BGJobProxyUsage" /> enumeration.
        /// </param>
        /// <param name="proxyList">
        ///   A string that contains the proxies to use to transfer files. The list is space-delimited. For details on specifying a proxy, see Remarks. This parameter must be <c>null</c> if the value of <see
        ///    cref="ProxyUsage" /> is BGJobProxyUsagePreConfig, BGJobProxyUsageNoProxy, or BGJobProxyUsageNoAutoDetect. The length of the proxy list is limited to 4,000 characters, not including the <c>null</c> terminator.
        /// </param>
        /// <param name="proxyBypassList">
        ///   A string that contains an optional list of host names, IP addresses, or both, that can bypass the proxy. The list is space-delimited. For details on specifying a bypass proxy, see Remarks. This parameter must be <c>null</c> if the value of <see
        ///    cref="ProxyUsage" /> is BGJobProxyUsagePreConfig, BGJobProxyUsageNoProxy, or BGJobProxyUsageNoAutoDetect. The length of the proxy bypass list is limited to 4,000 characters, not including the <c>null</c> terminator.
        /// </param>
        void SetProxySettings(
            BGJobProxyUsage proxyUsage,
            [MarshalAs(UnmanagedType.LPWStr)] string proxyList,
            [MarshalAs(UnmanagedType.LPWStr)] string proxyBypassList);

        /// <summary>
        ///   Retrieves the proxy settings the job uses to transfer the files.
        /// </summary>
        /// <param name="proxyUsage">
        ///   Specifies the proxy settings the job uses to transfer the files. For a list of proxy options, see the <see
        ///    cref="BGJobProxyUsage" /> enumeration.
        /// </param>
        /// <param name="proxyList">
        ///   A string that contains one or more proxies to use to transfer files. The list is space-delimited. For details on the format of the string, see the Listing Proxy Servers section of Enabling Internet Functionality. Call the CoTaskMemFree function to free <paramref
        ///    name = "proxyList" /> when done.
        /// </param>
        /// <param name="proxyBypassList">
        ///   A string that contains an optional list of host names or IP addresses, or both, that were not routed through the proxy. The list is space-delimited. For details on the format of the string, see the Listing the Proxy Bypass section of Enabling Internet Functionality. Call the CoTaskMemFree function to free <paramref
        ///    name = "proxyBypassList" /> when done.
        /// </param>
        void GetProxySettings(
            out BGJobProxyUsage proxyUsage,
            [MarshalAs(UnmanagedType.LPWStr)] out string proxyList,
            [MarshalAs(UnmanagedType.LPWStr)] out string proxyBypassList);

        /// <summary>
        ///   Changes the ownership of the job to the current user.
        /// </summary>
        void TakeOwnership();
    }
}