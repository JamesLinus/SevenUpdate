// ***********************************************************************
// <copyright file="Enums.cs"
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

    /// <summary>Specifies the owner of the current <see cref="BitsJob"/></summary>
    public enum JobOwner
    {
        /// <summary>The current logged in user</summary>
        CurrentUser = 0,

        /// <summary>The administrators group or system</summary>
        AllUsers = 1,
    }

    /// <summary>The <see cref="BitsJob"/> priority</summary>
    public enum JobPriority
    {
        /// <summary>Downloads without bandwidth restriction</summary>
        ForeGround = 0,

        /// <summary>Downloads with a 80% bandwidth use</summary>
        High = 1,

        /// <summary>Downloads using bandwidth available</summary>
        Normal = 2,

        /// <summary>Download slow, giving other net use priority</summary>
        Low = 3,
    }

    /// <summary>The current status of the <see cref="BitsJob"/></summary>
    public enum JobState
    {
        /// <summary>The job is queued to be ran</summary>
        Queued = 0,

        /// <summary>Connecting to the remote server</summary>
        Connecting = 1,

        /// <summary>Transferring the files</summary>
        Transferring = 2,

        /// <summary>Transfer is paused</summary>
        Suspended = 3,

        /// <summary>An fatal error occurred</summary>
        Error = 4,

        /// <summary>A non-fatal error occurred</summary>
        TransientError = 5,

        /// <summary>The job has completed</summary>
        Transferred = 6,

        /// <summary>Ready to run the job</summary>
        Acknowledged = 7,

        /// <summary>The job was canceled</summary>
        Canceled = 8,
    }

    /// <summary>The type of <see cref="BitsJob"/></summary>
    public enum JobType
    {
        /// <summary>Downloads a file</summary>
        Download,

        /// <summary>Uploads a file without progress</summary>
        Upload,

        /// <summary>Uploads a file and reply's with progress</summary>
        UploadReply,

        /// <summary>Unknown job</summary>
        Unknown,
    }

    /// <summary>The proxy use</summary>
    public enum ProxyUsage
    {
        /// <summary>Use the current configuration</summary>
        PreConfig,

        /// <summary>Don't use a proxy</summary>
        NoProxy,

        /// <summary>Override proxy settings</summary>
        Override,

        /// <summary>Auto detect proxy settings</summary>
        AutoDetect,
    }

    /// <summary>Specifies the error</summary>
    public enum ErrorContext
    {
        /// <summary>No error occurred</summary>
        None = 0,

        /// <summary>Unknown error</summary>
        UnknownError = 1,

        /// <summary>The general queue manager error</summary>
        GeneralQueueManagerError = 2,

        /// <summary>The general notification error</summary>
        QueueManagerNotificationError = 3,

        /// <summary>A local file error</summary>
        LocalFileError = 4,

        /// <summary>An error with the download file</summary>
        RemoteFileError = 5,

        /// <summary>An error while the file is transferring</summary>
        GeneralTransportError = 6,

        /// <summary>A remote program/server error</summary>
        RemoteApplicationError = 7,
    }

    /// <summary>The AuthenticationTarget enumeration defines the constant values that specify whether the credentials are used for proxy or server user authentication requests.</summary>
    public enum AuthenticationTarget
    {
        /// <summary>Use no credentials</summary>
        None = 0,

        /// <summary>Use credentials for server requests.</summary>
        Server = 1,

        /// <summary>Use credentials for proxy requests.</summary>
        Proxy = 2,
    }

    /// <summary>The AuthenticationScheme enumeration defines the constant values that specify the authentication scheme to use when a proxy or server requests user authentication.</summary>
    public enum AuthenticationScheme
    {
        /// <summary>Use no authentication scheme</summary>
        None = 0,

        /// <summary>Basic is a scheme in which the user name and password are sent in clear-text to the server or proxy.</summary>
        Basic = 1,

        /// <summary>Digest is a challenge-response scheme that uses a server-specified data string for the challenge.</summary>
        Digest,

        /// <summary>
        ///   Windows NT LAN Manager (NTLM) is a challenge-response scheme that uses the credentials of the 
        ///   user for authentication in a Windows network environment.
        /// </summary>
        NTLM,

        /// <summary>
        ///   Simple and Protected Negotiation protocol (SNEGO) is a challenge-response scheme that negotiates 
        ///   with the server or proxy to determine which scheme to use for authentication. Examples are the Kerberos protocol and NTLM
        /// </summary>
        Negotiate,

        /// <summary>Passport is a centralized authentication service provided by Microsoft that offers a single logon for member sites.</summary>
        Passport
    }

    /// <summary>Provides method of job notification</summary>
    [Flags]
    public enum NotificationFlags
    {
        /// <summary>All of the files in the job have been transferred.</summary>
        JobTransferred = 1,

        /// <summary>An error has occurred</summary>
        JobErrorOccurred = 2,

        /// <summary>Event notification is disabled. BITS ignores the other flags.</summary>
        NotificationDisabled = 4,

        /// <summary>The job has been modified. For example, a property value changed, the state of the job changed, or progress is made transferring the files.</summary>
        JobModified = 8,
    }

    /// <summary>Identifies the owner and ACL information to maintain when transferring a file using SMB</summary>
    [Flags]
    public enum FileAclFlags
    {
        /// <summary>If set, the file's owner information is maintained. Otherwise, the job's owner becomes the owner of the file.</summary>
        CopyFileOwner = 1,

        /// <summary>
        ///   If set, the file's group information is maintained. Otherwise, 
        ///   BITS uses the job owner's primary group to assign the group information to the file.
        /// </summary>
        CopyFileGroup = 2,

        /// <summary>
        ///   If set, BITS copies the explicit ACEs from the source file and inheritable ACEs from the destination parent folder. 
        ///   Otherwise, BITS copies the inheritable ACEs from the destination parent folder. If the parent folder does not 
        ///   contain inheritable ACEs, BITS uses the default DACL from the account.
        /// </summary>
        CopyDestinationFileAcl = 4,

        /// <summary>
        ///   If set, BITS copies the explicit ACEs from the source file and inheritable ACEs from the destination parent folder. 
        ///   Otherwise, BITS copies the inheritable ACEs from the destination parent folder.
        /// </summary>
        CopySourceFileAcl = 8,

        /// <summary>If set, BITS copies the owner and ACL information. This is the same as setting all the flags individually</summary>
        CopyFileAll = 15,
    }

    /// <summary>Flags that determine if the files of the job can be cached and served to peers and if the job can download content from peers</summary>
    [Flags]
    public enum PeerCachingFlags
    {
        /// <summary>
        ///   The job can download content from peers.
        ///   The job will not download from a peer unless both the client computer and the job allow BITS to download files from a peer
        /// </summary>
        ClientPeerCaching = 0x0001,

        /// <summary>
        ///   The files of the job can be cached and served to peers.
        ///   BITS will not cache the files and serve them to peers unless both the client computer and job allow BITS to cache and serve the files.
        /// </summary>
        ServerPeerCaching = 0x0002,
    }
}