#region GNU Public License Version 3

// Copyright 2007-2010 Robert Baker, Seven Software.
// This file is part of Seven Update.
//   
//      Seven Update is free software: you can redistribute it and/or modify
//      it under the terms of the GNU General Public License as published by
//      the Free Software Foundation, either version 3 of the License, or
//      (at your option) any later version.
//  
//      Seven Update is distributed in the hope that it will be useful,
//      but WITHOUT ANY WARRANTY; without even the implied warranty of
//      MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//      GNU General Public License for more details.
//   
//      You should have received a copy of the GNU General Public License
//      along with Seven Update.  If not, see <http://www.gnu.org/licenses/>.

#endregion

#region

using System;
using System.Diagnostics.CodeAnalysis;

#endregion

namespace SharpBits.Base
{
    public enum JobOwner
    {
        CurrentUser = 0,
        AllUsers = 1,
    }

    public enum JobPriority
    {
        ForeGround = 0,
        High = 1,
        Normal = 2,
        Low = 3,
    }

    public enum JobState
    {
        Queued = 0,
        Connecting = 1,
        Transferring = 2,
        Suspended = 3,
        Error = 4,
        TransientError = 5,
        Transferred = 6,
        Acknowledged = 7,
        Cancelled = 8,
    }

    public enum JobType
    {
        Download,
        Upload,
        UploadReply,
        Unknown, // not available in BITS API     
    }

    public enum ProxyUsage
    {
        Preconfig,
        NoProxy,
        Override,
        AutoDetect,
    }

    public enum ErrorContext
    {
        None = 0,
        UnknownError = 1,
        GeneralQueueManagerError = 2,
        QueueManagerNotificationError = 3,
        LocalFileError = 4,
        RemoteFileError = 5,
        GeneralTransportError = 6,
        RemoteApplicationError = 7,
    }

    /// <summary>
    ///   The AuthenticationTarget enumeration defines the constant values that specify whether the credentials are used for proxy or server user authentication requests.
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
    ///   The AuthenticationScheme enumeration defines the constant values that specify the authentication scheme to use when a proxy or server requests user authentication.
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
    ///   Identifies the owner and ACL information to maintain when transferring a file using SMB
    /// </summary>
    [Flags]
    public enum FileACLFlags
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
        CopyFileDACL = 4,
        /// <summary>
        ///   If set, BITS copies the explicit ACEs from the source file and inheritable ACEs from the destination parent folder. 
        ///   Otherwise, BITS copies the inheritable ACEs from the destination parent folder.
        /// </summary>
        CopyFileSACL = 8,
        /// <summary>
        ///   If set, BITS copies the owner and ACL information. This is the same as setting all the flags individually
        /// </summary>
        CopyFileAll = 15,
    }

    /// <summary>
    ///   Flags that determine if the files of the job can be cached and served to peers and if the job can download content from peers
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