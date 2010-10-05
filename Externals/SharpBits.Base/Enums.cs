//***********************************************************************
// Assembly         : SharpBits.Base
// Author           :xidar solutions
// Created          : 09-17-2010
// Last Modified By : sevenalive
// Last Modified On : 10-05-2010
// Description      : 
// Copyright        : (c) xidar solutions. All rights reserved.
//***********************************************************************

namespace SharpBits.Base
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// </summary>
    public enum JobOwner
    {
        /// <summary>
        /// </summary>
        CurrentUser = 0, 

        /// <summary>
        /// </summary>
        AllUsers = 1, 
    }

    /// <summary>
    /// </summary>
    public enum JobPriority
    {
        /// <summary>
        /// </summary>
        ForeGround = 0, 

        /// <summary>
        /// </summary>
        High = 1, 

        /// <summary>
        /// </summary>
        Normal = 2, 

        /// <summary>
        /// </summary>
        Low = 3, 
    }

    /// <summary>
    /// </summary>
    public enum JobState
    {
        /// <summary>
        /// </summary>
        Queued = 0, 

        /// <summary>
        /// </summary>
        Connecting = 1, 

        /// <summary>
        /// </summary>
        Transferring = 2, 

        /// <summary>
        /// </summary>
        Suspended = 3, 

        /// <summary>
        /// </summary>
        Error = 4, 

        /// <summary>
        /// </summary>
        TransientError = 5, 

        /// <summary>
        /// </summary>
        Transferred = 6, 

        /// <summary>
        /// </summary>
        Acknowledged = 7, 

        /// <summary>
        /// </summary>
        Cancelled = 8, 
    }

    /// <summary>
    /// </summary>
    public enum JobType
    {
        /// <summary>
        /// </summary>
        Download, 

        /// <summary>
        /// </summary>
        Upload, 

        /// <summary>
        /// </summary>
        UploadReply, 

        /// <summary>
        /// </summary>
        Unknown, // not available in BITS API     
    }

    /// <summary>
    /// </summary>
    public enum ProxyUsage
    {
        /// <summary>
        /// </summary>
        Preconfig, 

        /// <summary>
        /// </summary>
        NoProxy, 

        /// <summary>
        /// </summary>
        Override, 

        /// <summary>
        /// </summary>
        AutoDetect, 
    }

    /// <summary>
    /// </summary>
    public enum ErrorContext
    {
        /// <summary>
        /// </summary>
        None = 0, 

        /// <summary>
        /// </summary>
        UnknownError = 1, 

        /// <summary>
        /// </summary>
        GeneralQueueManagerError = 2, 

        /// <summary>
        /// </summary>
        QueueManagerNotificationError = 3, 

        /// <summary>
        /// </summary>
        LocalFileError = 4, 

        /// <summary>
        /// </summary>
        RemoteFileError = 5, 

        /// <summary>
        /// </summary>
        GeneralTransportError = 6, 

        /// <summary>
        /// </summary>
        RemoteApplicationError = 7, 
    }

    /// <summary>
    /// The AuthenticationTarget enumeration defines the constant values that specify whether the credentials are used for proxy or server user authentication requests.
    /// </summary>
    [SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue")]
    public enum AuthenticationTarget
    {
        /// <summary>
        ///   Use credentials for server requests.
        /// </summary>
        Server = 1, 

        /// <summary>
        ///   Use credentials for proxy requests.
        /// </summary>
        Proxy = 2, 
    }

    /// <summary>
    /// The AuthenticationScheme enumeration defines the constant values that specify the authentication scheme to use when a proxy or server requests user authentication.
    /// </summary>
    [SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue")]
    public enum AuthenticationScheme
    {
        /// <summary>
        ///   Basic is a scheme in which the user name and password are sent in clear-text to the server or proxy.
        /// </summary>
        Basic = 1, 

        /// <summary>
        ///   Digest is a challenge-response scheme that uses a server-specified data string for the challenge.
        /// </summary>
        Digest, 

        /// <summary>
        ///   Windows NT LAN Manager (NTLM) is a challenge-response scheme that uses the credentials of the 
        ///   user for authentication in a Windows network environment.
        /// </summary>
        Ntlm, 

        /// <summary>
        ///   Simple and Protected Negotiation protocol (Snego) is a challenge-response scheme that negotiates 
        ///   with the server or proxy to determine which scheme to use for authentication. Examples are the Kerberos protocol and NTLM
        /// </summary>
        Negotiate, 

        /// <summary>
        ///   Passport is a centralized authentication service provided by Microsoft that offers a single logon for member sites.
        /// </summary>
        Passport
    }

    /// <summary>
    /// </summary>
    [Flags]
    public enum NotificationFlags
    {
        /// <summary>
        ///   All of the files in the job have been transferred.
        /// </summary>
        JobTransferred = 1, 

        /// <summary>
        ///   An error has occurred
        /// </summary>
        JobErrorOccured = 2, 

        /// <summary>
        ///   Event notification is disabled. BITS ignores the other flags.
        /// </summary>
        NotificationDisabled = 4, 

        /// <summary>
        ///   The job has been modified. For example, a property value changed, the state of the job changed, or progress is made transferring the files.
        /// </summary>
        JobModified = 8, 
    }

    /// <summary>
    /// Identifies the owner and ACL information to maintain when transferring a file using SMB
    /// </summary>
    [Flags]
    public enum FileAclFlags
    {
        /// <summary>
        ///   If set, the file's owner information is maintained. Otherwise, the job's owner becomes the owner of the file.
        /// </summary>
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
        CopyFileDacl = 4, 

        /// <summary>
        ///   If set, BITS copies the explicit ACEs from the source file and inheritable ACEs from the destination parent folder. 
        ///   Otherwise, BITS copies the inheritable ACEs from the destination parent folder.
        /// </summary>
        CopyFileSacl = 8, 

        /// <summary>
        ///   If set, BITS copies the owner and ACL information. This is the same as setting all the flags individually
        /// </summary>
        CopyFileAll = 15, 
    }

    /// <summary>
    /// Flags that determine if the files of the job can be cached and served to peers and if the job can download content from peers
    /// </summary>
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