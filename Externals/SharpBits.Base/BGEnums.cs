// ***********************************************************************
// <copyright file="BGEnums.cs"
//            project="SharpBits.Base"
//            assembly="SharpBits.Base"
//            solution="SevenUpdate"
//            company="Xidar Solutions">
//     Copyright (c) xidar solutions. All rights reserved.
// </copyright>
// <author username="xidar">xidar</author>
// <author username="sevenalive">Robert Baker</author>
// <license href="http://sharpbits.codeplex.com/license">BSD License</license> 
// ***********************************************************************
namespace SharpBits.Base
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.InteropServices;

    /// <summary>Authentication scheme used for the background job</summary>
    internal enum BGAuthScheme
    {
        /// <summary>Basic is a scheme in which the user name and password are sent in clear-text to the server or proxy.</summary>
        Basic = 1,

        /// <summary>Digest is a challenge-response scheme that uses a server-specified data string for the challenge.</summary>
        Digest = 2,

        /// <summary>Windows NT LAN Manager (NTLM) is a challenge-response scheme that uses the credentials of the user for authentication in a Windows network environment.</summary>
        Ntlm = 3,

        /// <summary>Simple and Protected Negotiation protocol (SNEGO) is a challenge-response scheme that negotiates with the server or proxy to determine which scheme to use for authentication. Examples are the Kerberos protocol, Secure Socket Layer (SSL), and NTLM.</summary>
        Negotiate = 4,

        /// <summary>Passport is a centralized authentication service provided by Microsoft that offers a single logon for member sites.</summary>
        Passport = 5,
    }

    /// <summary>The location from which to download the code.</summary>
    internal enum BGAuthTarget
    {
        /// <summary>Use credentials for server requests.</summary>
        Server = 1,

        /// <summary>Use credentials for proxy requests.</summary>
        Proxy = 2,
    }

    /// <summary>The BG_JOB_STATE enumeration type defines constant values for the different states of a job</summary>
    internal enum BGJobState
    {
        /// <summary>
        /// Specifies that the job is in the queue and waiting to run. 
        /// If a user logs off while their job is transferring, the job 
        /// transitions to the queued state
        /// </summary>
        Queued = 0,

        /// <summary>
        /// Specifies that BITS is trying to connect to the server. If the 
        /// connection succeeds, the state of the job becomes 
        /// BG_JOB_STATE_TRANSFERRING; otherwise, the state becomes 
        /// BG_JOB_STATE_TRANSIENT_ERROR
        /// </summary>
        Connecting = 1,

        /// <summary>Specifies that BITS is transferring data for the job</summary>
        Transferring = 2,

        /// <summary>Specifies that the job is suspended (paused)</summary>
        Suspended = 3,

        /// <summary>
        /// Specifies that a non-recoverable error occurred (the service is 
        /// unable to transfer the file). When the error can be corrected, 
        /// such as an access-denied error, call the IBackgroundCopyJob::Resume 
        /// method after the error is fixed. However, if the error cannot be 
        /// corrected, call the IBackgroundCopyJob::Cancel method to cancel 
        /// the job, or call the IBackgroundCopyJob::Complete method to accept 
        /// the portion of a download job that transferred successfully.
        /// </summary>
        Error = 4,

        /// <summary>
        /// Specifies that a recoverable error occurred. The service tries to 
        /// recover from the transient error until the retry time value that 
        /// you specify using the IBackgroundCopyJob::SetNoProgressTimeout method 
        /// expires. If the retry time expires, the job state changes to 
        /// BG_JOB_STATE_ERROR
        /// </summary>
        TransientError = 5,

        /// <summary>Specifies that your job was successfully processed</summary>
        Transferred = 6,

        /// <summary>Specifies that you called the IBackgroundCopyJob::Complete method to acknowledge that your job completed successfully</summary>
        Acknowledged = 7,

        /// <summary>Specifies that you called the IBackgroundCopyJob::Cancel method to cancel the job (remove the job from the transfer queue)</summary>
        Canceled = 8,

        /// <summary>This is custom state not provided by BITS</summary>
        Unknown = 1001, // This is not provided by BITS but is Custom
    }

    /// <summary>The BG_JOB_TYPE enumeration type defines constant values that you use to specify the type of transfer job, such as download</summary>
    internal enum BGJobType
    {
        /// <summary>Specifies that the job downloads files to the client</summary>
        Download = 0,

        /// <summary>Specifies that the job uploads a file to the server</summary>
        Upload = 1,

        /// <summary>Specifies that the job uploads a file to the server and receives a reply file from the server application.</summary>
        UploadReply = 2,

        /// <summary>This is not provided by BITS but is Custom</summary>
        Unknown,
    }

    /// <summary>Used for the SetNotifyFlags method.</summary>
    [Flags]
    internal enum BGJobNotificationTypes
    {
        /// <summary>All of the files in the job have been transferred.</summary>
        BGNotifyJobTransferred = 0x0001,

        /// <summary>An error has occurred.</summary>
        BGNotifyJobError = 0x0002,

        /// <summary>Event notification is disabled. BITS ignores the other flags.</summary>
        BGNotifyDisable = 0x0004,

        /// <summary>The job has been modified. For example, a property value changed, the state of the job changed, or progress is made transferring the files. This flag is ignored if command line notification is specified.</summary>
        BGNotifyJobModification = 0x0008,
    }

    /// <summary>The BG_JOB_PROXY_USAGE enumeration type defines constant values that you use to specify which proxy to use for file transfers</summary>
    internal enum BGJobProxyUsage
    {
        /// <summary>Use the proxy and proxy bypass list settings defined by each user to transfer files</summary>
        PreConfig = 0,

        /// <summary>Do not use a proxy to transfer files</summary>
        NoProxy = 1,

        /// <summary>Use the application's proxy and proxy bypass list to transfer files</summary>
        Override = 2,

        /// <summary>Automatically detect proxy settings. BITS detects proxy settings for each file in the job</summary>
        AutoDetect = 3,
    }

    /// <summary>The BG_JOB_PRIORITY enumeration type defines the constant values that you use to specify the priority level of the job</summary>
    internal enum BGJobPriority
    {
        /// <summary>Transfers the job in the foreground</summary>
        Foreground = 0,

        /// <summary>Transfers the job in the background. This is the highest background priority level.</summary>
        High = 1,

        /// <summary>Transfers the job in the background. This is the default priority level for a job</summary>
        Normal = 2,

        /// <summary>Transfers the job in the background. This is the lowest background priority level</summary>
        Low = 3,
    }

    /// <summary>The BG_ERROR_CONTEXT enumeration type defines the constant values that specify the context in which the error occurred</summary>
    internal enum BGErrorContext
    {
        /// <summary>An error has not occurred</summary>
        None = 0,

        /// <summary>The error context is unknown</summary>
        Unknown = 1,

        /// <summary>The transfer queue manager generated the error</summary>
        GeneralQueueManager = 2,

        /// <summary>The error was generated while the queue manager was notifying the client of an event</summary>
        QueueManagerNotification = 3,

        /// <summary>The error was related to the specified local file. For example, permission was denied or the volume was unavailable</summary>
        LocalFile = 4,

        /// <summary>The error was related to the specified remote file. For example, the Url is not accessible</summary>
        RemoteFile = 5,

        /// <summary>The transport layer generated the error. These errors are general transport failures; errors not specific to the remote file</summary>
        GeneralTransport = 6,
    }

    /// <summary>The ACL's of the file to set when downloaded</summary>
    [Flags]
    internal enum BGFileAclFlags
    {
        /// <summary>Set current owner</summary>
        BGCopyFileOwner = 0x0001,

        /// <summary>Set current group</summary>
        BGCopyFileGroup = 0x0002,

        /// <summary>Delete all ACL lists</summary>
        BGCopyDestinationFileAcl = 0x0004,

        /// <summary>Give special permissions</summary>
        BGCopySourceFileAcl = 0x0008,

        /// <summary>Inherit all lists</summary>
        BGCopyFileAll = 0x0015,
    }

    /// <summary>The BG_FILE_INFO structure provides the local and remote names of the file to transfer</summary>
    [StructLayout(LayoutKind.Sequential, Pack = 4, Size = 0)]
    internal struct BGFileInfo
    {
        /// <summary>Remote Name for the File</summary>
        [MarshalAs(UnmanagedType.LPWStr)]
        public string RemoteName;

        /// <summary>Local Name for the file</summary>
        [MarshalAs(UnmanagedType.LPWStr)]
        public string LocalName;
    }

    /// <summary>The BG_JOB_PROGRESS structure provides job-related progress information, such as the number of bytes and files transferred</summary>
    [StructLayoutAttribute(LayoutKind.Sequential, Pack = 8, Size = 0)]
    internal struct BGJobProgress
    {
        /// <summary>Total number of bytes to transfer for the job.</summary>
        public ulong BytesTotal;

        /// <summary>Number of bytes transferred</summary>
        public ulong BytesTransferred;

        /// <summary>Total number of files to transfer for this job</summary>
        public uint FilesTotal;

        /// <summary>Number of files transferred.</summary>
        public uint FilesTransferred;
    }

    /// <summary>The BG_JOB_REPLY_PROGRESS structure provides progress information related to the reply portion of an upload-reply job.</summary>
    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    internal struct BGJobReplyProgress
    {
        /// <summary>Size of the file in bytes. The value is BG_SIZE_UNKNOWN if the reply has not begun.</summary>
        public ulong BytesTotal;

        /// <summary>Number of bytes transferred.</summary>
        public ulong BytesTransferred;
    }

    /// <summary>The BG_JOB_TIMES structure provides job-related timestamps</summary>
    [StructLayoutAttribute(LayoutKind.Sequential, Pack = 4, Size = 0)]
    internal struct BGJobTimes
    {
        /// <summary>Time the job was created</summary>
        public FileTime CreationTime;

        /// <summary>Time the job was last modified or bytes were transferred</summary>
        public FileTime ModificationTime;

        /// <summary>Time the job entered the BG_JOB_STATE_TRANSFERRED state</summary>
        public FileTime TransferCompletionTime;
    }

    /// <summary>This structure is a 64-bit value representing the number of 100-nanosecond intervals since January 1, 1601.</summary>
    [StructLayoutAttribute(LayoutKind.Sequential, Pack = 4, Size = 0)]
    internal struct FileTime
    {
        /// <summary>Specifies the low 32 bits of the file time.</summary>
        public uint DWLowDateTime;

        /// <summary>Specifies the high 32 bits of the file time.</summary>
        public uint DWHighDateTime;
    }

    /// <summary>The BG_FILE_PROGRESS structure provides file-related progress information, such as the number of bytes transferred</summary>
    [StructLayoutAttribute(LayoutKind.Sequential, Pack = 8, Size = 0)]
    internal struct BGFileProgress
    {
        /// <summary>Size of the file in bytes</summary>
        public ulong BytesTotal;

        /// <summary>Number of bytes transferred.</summary>
        public ulong BytesTransferred;

        /// <summary>For downloads, the value is <see langword = "true" /> if the file is available to the user; otherwise, the value is <see langword = "false" /></summary>
        public int Completed;
    }

    /// <summary>The BG_FILE_RANGE structure identifies a range of bytes to download from a file.</summary>
    [StructLayout(LayoutKind.Sequential, Pack = 8, Size = 0)]
    internal struct BGFileRange
    {
        /// <summary>The length to the end of the file</summary>
        public const ulong BGLengthToEof = unchecked((ulong)-1);

        /// <summary>Zero-based offset to the beginning of the range of bytes to download from a file.</summary>
        public ulong InitialOffset;

        /// <summary>Number of bytes in the range. To indicate that the range extends to the end of the file, specify BG_LENGTH_TO_EOF</summary>
        public ulong Length;
    }

    /// <summary>The BG_AUTH_CREDENTIALS_UNION union identifies the credentials to use for the authentication scheme specified in the BG_AUTH_CREDENTIALS structure.</summary>
    [StructLayout(LayoutKind.Sequential, Size = 8)]
    internal struct BGAuthCredentialsUnion
    {
        /// <summary>Identifies the user name and password of the user to authenticate. For details, see the BG_BASIC_CREDENTIALS structure.</summary>
        public BGBasicCredentials Basic;
    }

    /// <summary>The BG_BASIC_CREDENTIALS structure identifies the user name and password to authenticate.</summary>
    [SuppressMessage("Microsoft.Portability", "CA1900:ValueTypeFieldsShouldBePortable", MessageId = "Password", Justification = "Interop"), StructLayout(LayoutKind.Sequential, Size = 8)]
    internal struct BGBasicCredentials
    {
        /// <summary>Null-terminated string that contains the user name to authenticate. The user name is limited to 300 characters, not including the <see langword = "null" /> terminator. The format of the user name depends on the authentication scheme requested. For example, for Basic, NTLM, and Negotiate authentication, the user name is of the form "domain\user name" or "user name". For Passport authentication, the user name is an e-mail address. If <see langword = "null" />, default credentials for this session context are used.</summary>
        [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields", Justification = "Interop"), MarshalAs(UnmanagedType.LPWStr)]
        internal string UserName;

        /// <summary>Null-terminated string that contains the password in clear-text. The password is limited to 300 characters, not including the <see langword = "null" /> terminator. The password can be blank. Set to <see langword = "null" /> if <see cref = "UserName" /> is <see langword = "null" />. BITS encrypts the password before persisting the job if a network disconnect occurs or the user logs off.</summary>
        [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields", Justification = "Interop"), MarshalAs(UnmanagedType.LPWStr)]
        internal string Password;
    }

    /// <summary>The BG_AUTH_CREDENTIALS structure identifies the target (proxy or server), authentication scheme, and the user's credentials to use for user authentication requests. The structure is passed to the IBackgroundCopyJob2::SetCredentials method.</summary>
    [SuppressMessage("Microsoft.Portability", "CA1900:ValueTypeFieldsShouldBePortable", MessageId = "UserName", Justification = "Interop"),
     SuppressMessage("Microsoft.Portability", "CA1900:ValueTypeFieldsShouldBePortable", MessageId = "Password", Justification = "Interop"), StructLayout(LayoutKind.Sequential, Size = 16)]
    internal struct BGAuthCredentials
    {
        /// <summary>Identifies whether to use the credentials for a proxy or server authentication request. For a list of values, see the BG_AUTH_TARGET enumeration.</summary>
        [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields", Justification = "Interop"),]
        public BGAuthTarget Target;

        /// <summary>Identifies the scheme to use for authentication (for example, Basic or NTLM). For a list of values, see the BG_AUTH_SCHEME enumeration.</summary>
        [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields", Justification = "Interop")]
        public BGAuthScheme Scheme;

        /// <summary>Identifies the credentials to use for the specified authentication scheme. For details, see the BG_AUTH_CREDENTIALS_UNION union.</summary>
        public BGAuthCredentialsUnion Credentials;
    }
}