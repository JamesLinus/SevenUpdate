// ***********************************************************************
// <copyright file="NativeMethods.cs" project="SharpBits.Base" assembly="SharpBits.Base" solution="SevenUpdate" company="Xidar Solutions">
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
    using System.Text;

    /// <summary>Specifies an authentication level, which indicates the amount of authentication provided to help protect the integrity of the data. Each level includes the protection provided by the previous levels.</summary>
    [Flags]
    internal enum RpcAuthenticationLevels
    {
        /// <summary>Tells DCOM to choose the authentication level using its normal security blanket negotiation algorithm.</summary>
        Default = 0,

        /// <summary>Performs no authentication.</summary>
        None = 1,

        /// <summary>Authenticates the credentials of the client only when the client establishes a relationship with the server.</summary>
        Connect = 2,

        /// <summary>Authenticates only at the beginning of each remote procedure call when the server receives the request.</summary>
        Call = 3,

        /// <summary>Authenticates that all data received is from the expected client.</summary>
        Pkt = 4,

        /// <summary>Authenticates and verifies that none of the data transferred between client and server has been modified.</summary>
        PktIntegrity = 5,

        /// <summary>Authenticates all previous levels and encrypts the argument value of each remote procedure call.</summary>
        PktPrivacy = 6
    }

    /// <summary>Specifies an impersonation level, which indicates the amount of authority given to the server when it is impersonating the client.</summary>
    internal enum RpcImpersonationLevels
    {
        /// <summary>DCOM can choose the impersonation level using its normal security blanket negotiation algorithm.</summary>
        Default = 0,

        /// <summary>The client is anonymous to the server. The server process can impersonate the client, but the impersonation token will not contain any information and cannot be used.</summary>
        Anonymous = 1,

        /// <summary>The server can obtain the client's identity. The server can impersonate the client for ACL checking, but it cannot access system objects as the client.</summary>
        Identify = 2,

        /// <summary>The server process can impersonate the client's security context while acting on behalf of the client. This level of impersonation can be used to access local resources such as files. When impersonating at this level, the impersonation token can only be passed across one machine boundary. The Schannel authentication service only supports this level of impersonation.</summary>
        Impersonate = 3,

        /// <summary>The server process can impersonate the client's security context while acting on behalf of the client. The server process can also make outgoing calls to other servers while acting on behalf of the client, using cloaking. The server may use the client's security context on other machines to access local and remote resources as the client. When impersonating at this level, the impersonation token can be passed across any number of computer boundaries.</summary>
        Delegate = 4
    }

    /// <summary>Specifies various capabilities in CoInitializeSecurity and IClientSecurity::SetBlanket (or its helper function CoSetProxyBlanket).</summary>
    internal enum EoAuthenticationCapabilities
    {
        /// <summary>Indicates that no capability flags are set.</summary>
        None = 0x00,

        /// <summary>Causes DCOM to send Schannel server principal names in fullsic format to clients as part of the default security negotiation. The name is extracted from the server certificate.</summary>
        MakeFullSic = 0x100,

        /// <summary>Tells DCOM to use the valid capabilities from the call to CoInitializeSecurity. If CoInitializeSecurity was not called, EOAC_NONE will be used for the capabilities flag. This flag can be set only by clients in a call to IClientSecurity::SetBlanket or CoSetProxyBlanket.</summary>
        Default = 0x800,

        /// <summary>Authenticates distributed reference count calls to prevent malicious users from releasing objects that are still being used. If this flag is set, which can be done only in a call to CoInitializeSecurity by the client, the authentication level (in AuthLevel) cannot be set to none.The server always authenticates Release calls. Setting this flag prevents an authenticated client from releasing the objects of another authenticated client. It is recommended that clients always set this flag, although performance is affected because of the overhead associated with the extra security.</summary>
        SecureRefs = 0x02,

        /// <summary>The application ID.</summary>
        [SuppressMessage("Microsoft.Naming", "CA1706:ShortAcronymsShouldBeUppercase", MessageId = "Member", Justification = "Interop")]
        AppID = 0x08,

        /// <summary>Require full.</summary>
        [SuppressMessage("Microsoft.Naming", "CA1705:LongAcronymsShouldBePascalCased", MessageId = "Member", Justification = "Interop")]
        RequireFullSic = 0x200,

        /// <summary>Causes any activation where a server process would be launched under the caller's identity (activate-as-activator) to fail with E_ACCESSDENIED. This value, which can be specified only in a call to CoInitializeSecurity by the client, allows an application that runs under a privileged account (such as LocalSystem) to help prevent its identity from being used to launch untrusted components.</summary>
        [SuppressMessage("Microsoft.Naming", "CA1705:LongAcronymsShouldBePascalCased", MessageId = "Member", Justification = "Interop")]
        DisableAaa = 0x1000
    }

    /// <summary>The version of BITS.</summary>
    internal enum BitsVersion
    {
        /// <summary>Undefined bits version.</summary>
        BitsUndefined,

        /// <summary>BITS version 1.0.</summary>
        Bits1,

        /// <summary>BITS version 1.2.</summary>
        Bits1Dot2,

        /// <summary>BITS version 1.5.</summary>
        Bits1Dot5,

        /// <summary>Bits Version 2.0.</summary>
        Bits2,

        /// <summary>Bits version 2.5.</summary>
        Bits2Dot5,

        /// <summary>Bits version 3.0.</summary>
        Bits3,
    }

    /// <summary>Win32 native methods.</summary>
    internal static class NativeMethods
    {
        /// <summary>Converts a string-format security identifier (SID) into a valid, functional SID. You can use this function to retrieve a SID that the ConvertSidToStringSid function converted to string format.</summary>
        /// <param name="sid">A pointer to a <see langword="null" />-terminated string containing the string-format SID to convert. The SID string can use either the standard S-R-I-S-S� format for SID strings, or the SID string constant format, such as "BA" for built-in administrators.</param>
        /// <param name="sidPointer">A pointer to a variable that receives a pointer to the converted SID. To free the returned buffer, call the LocalFree function.</param>
        /// <returns><see langword="true" /> if function succecced.</returns>
        [DllImport(@"advapi32.dll", CharSet = CharSet.Unicode)]
        [return: MarshalAsAttribute(UnmanagedType.Bool)]
        internal static extern bool ConvertStringSidToSidW(string sid, ref IntPtr sidPointer);

        /// <summary>Retrieves the name of the account for this SID and the name of the first domain on which this SID is found.</summary>
        /// <param name="systemName">A pointer to a <see langword="null" />-terminated character string that specifies the target computer. This string can be the name of a remote computer. If this parameter is <see langword="null" />, the account name translation begins on the local system. If the name cannot be resolved on the local system, this function will try to resolve the name using domain controllers trusted by the local system. Generally, specify a value only when the account is in an untrusted domain and the name of a computer in that domain is known.</param>
        /// <param name="sid">A pointer to the SID to look up.</param>
        /// <param name="name">A pointer to a buffer that receives a <see langword="null" />-terminated string that contains the account name that corresponds to the sid parameter.</param>
        /// <param name="nameSize">On input, specifies the size, of the name buffer. If the function fails because the buffer is too small or if name is zero, name receives the required buffer size, including the terminating <see langword="null" /> character.</param>
        /// <param name="referencedDomainName">A pointer to a buffer that receives a <see langword="null" />-terminated string that contains the name of the domain where the account name was found.</param>
        /// <param name="domainNameSize">On input, specifies the size, of the <paramref name="referencedDomainName" /> buffer. If the function fails because the buffer is too small or if <paramref name="referencedDomainName" /> is zero, <paramref name="referencedDomainName" /> receives the required buffer size, including the terminating <see langword="null" /> character.</param>
        /// <param name="use">A pointer to a variable that receives a SidNameUse value that indicates the type of the account.</param>
        /// <returns><see langword="true" /> if function succecced.</returns>
        [DllImport(@"advapi32.dll", CharSet = CharSet.Unicode)]
        [return: MarshalAsAttribute(UnmanagedType.Bool)]
        internal static extern bool LookupAccountSidW(
            string systemName,
            IntPtr sid,
            StringBuilder name,
            ref long nameSize,
            StringBuilder referencedDomainName,
            ref long domainNameSize,
            ref int use);
    }
}