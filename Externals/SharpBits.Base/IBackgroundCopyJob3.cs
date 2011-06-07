// ***********************************************************************
// <copyright file="IBackgroundCopyJob3.cs" project="SharpBits.Base" assembly="SharpBits.Base" solution="SevenUpdate" company="Xidar Solutions">
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
    ///   Use the IBackgroundCopyJob3 interface to download ranges of a file and change the prefix of a remote file
    ///   name.
    /// </summary>
    [ComImport]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("443C8934-90FF-48ED-BCDE-26F5C7450042")]
    internal interface IBackgroundCopyJob3
    {
        /// <summary>
        ///   Adds multiple files to the job.
        /// </summary>
        /// <param name="fileCount">
        ///   Number of elements in paFileSet. .
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
        ///    cref="BGFileInfo" /> structure. .
        /// </param>
        /// <param name="localName">
        ///   A string that contains the name of the file on the server. For information on specifying the remote name, see the RemoteName member and Remarks section of the <see
        ///    cref="BGFileInfo" /> structure. .
        /// </param>
        void AddFile(
            [MarshalAs(UnmanagedType.LPWStr)] string remoteUrl, [MarshalAs(UnmanagedType.LPWStr)] string localName);

        /// <summary>
        ///   Returns an interface pointer to an enumerator object that you use to enumerate the files in the job.
        /// </summary>
        /// <param name="enum">
        ///   <see cref="IEnumBackgroundCopyFiles" /> interface pointer that you use to enumerate the files in the job. Release enumFiles when done. .
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
        ///   Type of transfer being performed. For a list of transfer types, see the BGJob_TYPE enumeration type. .
        /// </param>
        void GetType(out BGJobType val);

        /// <summary>
        ///   Retrieves job-related progress information, such as the number of bytes and files transferred to the client.
        /// </summary>
        /// <param name="val">
        ///   Contains data that you can use to calculate the percentage of the job that is complete. For more information, see <see
        ///    cref="BGJobProgress" />. .
        /// </param>
        void GetProgress(out BGJobProgress val);

        /// <summary>
        ///   Retrieves timestamps for activities related to the job, such as the time the job was created.
        /// </summary>
        /// <param name="val">
        ///   Contains job-related time stamps. For available time stamps, see the BGJob_TIMES structure.
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
        ///   Error interface that provides the error code, a description of the error, and the context in which the error occurred. This parameter also identifies the file being transferred at the time the error occurred. Release error when done. .
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
        ///   A string that identifies the job. Must not be <c>null</c>. The length of the string is limited to 256 characters, not including the <c>null</c> terminator. .
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
        ///   A string that contains a short description of the job. Call the CoTaskMemFree function to free ppDescription when done. .
        /// </param>
        void GetDescription([MarshalAs(UnmanagedType.LPWStr)] out string val);

        /// <summary>
        ///   Specifies the priority of the job relative to other jobs in the transfer queue.
        /// </summary>
        /// <param name="val">
        ///   Specifies the priority level of your job relative to other jobs in the transfer queue. The default is BGJobPriorityNormal. For a list of priority levels, see the <see
        ///    cref="BGJobPriority" /> enumeration. .
        /// </param>
        void SetPriority(BGJobPriority val);

        /// <summary>
        ///   Retrieves the priority level you have set for the job.
        /// </summary>
        /// <param name="val">
        ///   Priority of the job relative to other jobs in the transfer queue. .
        /// </param>
        void GetPriority(out BGJobPriority val);

        /// <summary>
        ///   Specifies the type of event notification to receive.
        /// </summary>
        /// <param name="val">
        ///   Set one or more of the following flags to identify the events that you want to receive. .
        /// </param>
        void SetNotifyFlags([MarshalAs(UnmanagedType.U4)] BGJobNotificationTypes val);

        /// <summary>
        ///   Retrieves the event notification (callback) flags you have set for your application.
        /// </summary>
        /// <param name="val">
        ///   Identifies the events that your application receives. The following table lists the event notification flag values. .
        /// </param>
        void GetNotifyFlags(out uint val);

        /// <summary>
        ///   Specifies a pointer to your implementation of the <see cref="IBackgroundCopyCallback" /> interface (callbacks). The interface receives notification based on the event notification flags you set.
        /// </summary>
        /// <param name="val">
        ///   An <see cref="IBackgroundCopyCallback" /> interface pointer. To remove the current callback interface pointer, set this parameter to <c>null</c>.
        /// </param>
        void SetNotifyInterface([MarshalAs(UnmanagedType.IUnknown)] object val);

        /// <summary>
        ///   Retrieves a pointer to your implementation of the <see cref="IBackgroundCopyCallback" /> interface (callbacks).
        /// </summary>
        /// <param name="val">
        ///   Interface pointer to your implementation of the <see cref="IBackgroundCopyCallback" /> interface. When done, release ppNotifyInterface.
        /// </param>
        void GetNotifyInterface([MarshalAs(UnmanagedType.IUnknown)] out object val);

        /// <summary>
        ///   Specifies the minimum length of time that BITS waits after encountering a transient error condition before trying to transfer the file.
        /// </summary>
        /// <param name="seconds">
        ///   Minimum length of time, in seconds, that BITS waits after encountering a transient error before trying to transfer the file. The default retry delay is 600 seconds (10 minutes). The minimum retry delay that you can specify is 60 seconds. If you specify a value less than 60 seconds, BITS changes the value to 60 seconds. If the value exceeds the no-progress-timeout value retrieved from the <see
        ///    cref="GetNoProgressTimeout" /> method, BITS will not retry the transfer and moves the job to the BGJobStateError state. .
        /// </param>
        void SetMinimumRetryDelay(uint seconds);

        /// <summary>
        ///   Retrieves the minimum length of time that BITS waits after encountering a transient error condition before trying to transfer the file.
        /// </summary>
        /// <param name="seconds">
        ///   Length of time, in seconds, that the service waits after encountering a transient error before trying to transfer the file. .
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
        ///   Length of time, in seconds, that the service tries to transfer the file after a transient error occurs. .
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
        ///    cref="ProxyUsage" /> is BGJobProxyUsagePreConfig, BGJobProxyUsageNoProxy, or BGJobProxyUsageNoAutoDetect. The length of the proxy list is limited to 4,000 characters, not including the <c>null</c> terminator. .
        /// </param>
        /// <param name="proxyBypassList">
        ///   A string that contains an optional list of host names, IP addresses, or both, that can bypass the proxy. The list is space-delimited. For details on specifying a bypass proxy, see Remarks. This parameter must be <c>null</c> if the value of <see
        ///    cref="ProxyUsage" /> is BGJobProxyUsagePreConfig, BGJobProxyUsageNoProxy, or BGJobProxyUsageNoAutoDetect. The length of the proxy bypass list is limited to 4,000 characters, not including the <c>null</c> terminator. .
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
        ///    cref="BGJobProxyUsage" /> enumeration. .
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

        /// <summary>
        ///   Use the SetNotifyCmdLine method to specify a program to execute if the job enters the BGJobStateError or BGJobStateTransferred state. BITS executes the program in the context of the user.
        /// </summary>
        /// <param name="program">
        ///   A string that contains the program to execute. The program parameter is limited to MAX_PATH characters, not including the <c>null</c> terminator. You should specify a full path to the program; the method will not use the search path to locate the program. To remove command line notification, set program and parameters to <c>null</c>. The method fails if program is <c>null</c> and parameters is non-<c>null</c>. .
        /// </param>
        /// <param name="parameters">
        ///   A string that contains the parameters of the program in program. The first parameter must be the program in program (use quotes if the path uses long file names). The parameters parameter is limited to 4,000 characters, not including the <c>null</c> terminator. This parameter can be <c>null</c>.
        /// </param>
        void SetNotifyCmdLine(
            [MarshalAs(UnmanagedType.LPWStr)] string program, [MarshalAs(UnmanagedType.LPWStr)] string parameters);

        /// <summary>
        ///   Use the GetNotifyCmdLine method to retrieve the program to execute when the job enters the error or transferred state.
        /// </summary>
        /// <param name="program">
        ///   A string that contains the program to execute when the job enters the error or transferred state. Call the CoTaskMemFree function to free program when done. .
        /// </param>
        /// <param name="parameters">
        ///   A string that contains the arguments of the program in program. Call the CoTaskMemFree function to free parameters when done. .
        /// </param>
        void GetNotifyCmdLine(
            [MarshalAs(UnmanagedType.LPWStr)] out string program,
            [MarshalAs(UnmanagedType.LPWStr)] out string parameters);

        /// <summary>
        ///   Use the GetReplyProgress method to retrieve progress information related to the transfer of the reply data from an upload-reply job.
        /// </summary>
        /// <param name="progress">
        ///   Contains information that you use to calculate the percentage of the reply file transfer that is complete. For more information, see <see
        ///    cref="BGJobReplyProgress" />.
        /// </param>
        void GetReplyProgress([Out] out BGJobReplyProgress progress);

        /// <summary>
        ///   Use the GetReplyData method to retrieve an in-memory copy of the reply data from the server application. Only call this method if the job's type is BGJobTypeUploadReply and its state is BGJobStateTransferred.
        /// </summary>
        /// <param name="buffer">
        ///   Buffer to contain the reply data. The method sets buffer to <c>null</c> if the server application did not return a reply. Call the CoTaskMemFree function to free buffer when done.
        /// </param>
        /// <param name="length">
        ///   Size, in bytes, of the reply data in buffer.
        /// </param>
        void GetReplyData(IntPtr buffer, out ulong length);

        /// <summary>
        ///   Use the SetReplyFileName method to specify the name of the file to contain the reply data from the server application. Only call this method if the job's type is BGJobTypeUploadReply.
        /// </summary>
        /// <param name="replyFileName">
        ///   A string that contains the full path to the reply file. BITS generates the file name if ReplyFileNamePathSpec is <c>null</c> or an empty string. You cannot use wild cards in the path or file name, and directories in the path must exist. The path is limited to MAX_PATH, not including the <c>null</c> terminator. The user must have permissions to write to the directory. BITS does not support NTFS streams. Instead of using network drives, which are session specific, use UNC paths (for example, \server\share\path\file). Do not include the \? prefix in the path. .
        /// </param>
        void SetReplyFileName([MarshalAs(UnmanagedType.LPWStr)] string replyFileName);

        /// <summary>
        ///   Use the GetReplyFileName method to retrieve the name of the file that contains the reply data from the server application. Only call this method if the job type is BGJobTypeUploadReply.
        /// </summary>
        /// <param name="replyFileName">
        ///   A string that contains the full path to the reply file. Call the CoTaskMemFree function to free <paramref
        ///    name = "replyFileName" /> when done. .
        /// </param>
        void GetReplyFileName([MarshalAs(UnmanagedType.LPWStr)] out string replyFileName);

        /// <summary>
        ///   Use the SetCredentials method to specify the credentials to use for a proxy or remote server user authentication request.
        /// </summary>
        /// <param name="credentials">
        ///   Identifies the target (proxy or server), authentication scheme, and the user's credentials to use for user authentication. For details, see the <see
        ///    cref="BGAuthCredentials" /> structure. If the job currently contains credentials with the same target and scheme pair, the existing credentials are replaced with the new credentials. The credentials persist for the life of the job. To remove the credentials from the job, call the IBackgroundCopyJob2::<see
        ///   cref="RemoveCredentials" /> method. .
        /// </param>
        void SetCredentials([In] ref BGAuthCredentials credentials);

        /// <summary>
        ///   Use the RemoveCredentials method to remove credentials from use. The credentials must match an existing target and scheme pair that you specified using the IBackgroundCopyJob2::<see
        ///   cref="SetCredentials" /> method. There is no method to retrieve the credentials you have set.
        /// </summary>
        /// <param name="target">
        ///   Identifies whether to use the credentials for proxy or server authentication.
        /// </param>
        /// <param name="scheme">
        ///   Identifies the authentication scheme to use (basic or one of several challenge-response schemes). For details, see the <see
        ///    cref="BGAuthScheme" /> enumeration. .
        /// </param>
        void RemoveCredentials(BGAuthTarget target, BGAuthScheme scheme);

        /// <summary>
        ///   Replaces the remote prefix.
        /// </summary>
        /// <param name="oldPrefix">
        ///   The old prefix.
        /// </param>
        /// <param name="newPrefix">
        ///   The new prefix.
        /// </param>
        void ReplaceRemotePrefix(
            [MarshalAs(UnmanagedType.LPWStr)] string oldPrefix, [MarshalAs(UnmanagedType.LPWStr)] string newPrefix);

        /// <summary>
        ///   Adds the file with ranges.
        /// </summary>
        /// <param name="remoteUrl">
        ///   The remote URL.
        /// </param>
        /// <param name="localName">
        ///   Name of the local.
        /// </param>
        /// <param name="rangeCount">
        ///   The range count.
        /// </param>
        /// <param name="ranges">
        ///   The ranges.
        /// </param>
        void AddFileWithRanges(
            [MarshalAs(UnmanagedType.LPWStr)] string remoteUrl,
            [MarshalAs(UnmanagedType.LPWStr)] string localName,
            uint rangeCount,
            [MarshalAs(UnmanagedType.LPArray)] BGFileRange[] ranges);

        /// <summary>
        ///   Sets the file acl flags.
        /// </summary>
        /// <param name="flags">
        ///   The flags.
        /// </param>
        void SetFileAclFlags(BGFileAclFlags flags);

        /// <summary>
        ///   Gets the file acl flags.
        /// </summary>
        /// <param name="flags">
        ///   The flags.
        /// </param>
        void GetFileAclFlags([Out] out BGFileAclFlags flags);
    }
}