// ***********************************************************************
// <copyright file="NativeMethods.cs"
//            project="SharpBits.Base"
//            assembly="SharpBits.Base"
//            solution="SevenUpdate"
//            company="Xidar Solutions">
//     Copyright (c) xidar solutions. All rights reserved.
// </copyright>
// <author username="xidar">xidar</author>
// ***********************************************************************
namespace SharpBits.Base
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.InteropServices;
    using System.Text;

    /// <summary>
    /// The authentication level.
    /// </summary>
    [Flags]
    public enum RpcAuthLevels
    {
        /// <summary>
        ///   The default level
        /// </summary>
        Default = 0, 

        /// <summary>
        ///   No authentication
        /// </summary>
        None = 1, 

        /// <summary>
        ///   Connect to the Rpc server
        /// </summary>
        Connect = 2, 

        /// <summary>
        ///   Calls the RCP server
        /// </summary>
        Call = 3, 

        /// <summary>
        ///   No idea what this is
        /// </summary>
        Pkt = 4, 

        /// <summary>
        ///   The integrity
        /// </summary>
        PktIntegrity = 5, 

        /// <summary>
        ///   Privacy authentication
        /// </summary>
        PktPrivacy = 6
    }

    /// <summary>
    /// The Impersonation level.
    /// </summary>
    public enum RpcImpLevel
    {
        /// <summary>
        ///   The default level
        /// </summary>
        Default = 0, 

        /// <summary>
        ///   Anonymous impersonation
        /// </summary>
        Anonymous = 1, 

        /// <summary>
        ///   The current user identity
        /// </summary>
        Identify = 2, 

        /// <summary>
        ///   Impersonate another user
        /// </summary>
        Impersonate = 3, 

        /// <summary>
        ///   A delegate impersonation
        /// </summary>
        Delegate = 4
    }

    /// <summary>
    /// Defines authentication.
    /// </summary>
    public enum EoAuthCap
    {
        /// <summary>
        ///   No authentication
        /// </summary>
        None = 0x00, 

        /// <summary>
        ///   Mutual authentication
        /// </summary>
        MutualAuth = 0x01, 

        /// <summary>
        ///   Static Cloaking authentication
        /// </summary>
        StaticCloaking = 0x20, 

        /// <summary>
        ///   Dynamic Closing authentication
        /// </summary>
        DynamicCloaking = 0x40, 

        /// <summary>
        ///   Any authority
        /// </summary>
        AnyAuthority = 0x80, 

        /// <summary>
        ///   Make full access
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1705:LongAcronymsShouldBePascalCased", MessageId = "Member", Justification = "Interop")]
        MakeFullSic = 0x100, 

        /// <summary>
        ///   The default authentication
        /// </summary>
        Default = 0x800, 

        /// <summary>
        ///   Secure references
        /// </summary>
        SecureRefs = 0x02, 

        /// <summary>
        ///   Access control
        /// </summary>
        AccessControl = 0x04, 

        /// <summary>
        ///   The application ID
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1706:ShortAcronymsShouldBeUppercase", MessageId = "Member", Justification = "Interop")]
        AppID = 0x08, 

        /// <summary>
        ///   Dynamic Authentication
        /// </summary>
        Dynamic = 0x10, 

        /// <summary>
        ///   Require full
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1705:LongAcronymsShouldBePascalCased", MessageId = "Member", Justification = "Interop")]
        RequireFullSic = 0x200, 

        /// <summary>
        ///   Auto impersonate
        /// </summary>
        AutoImpersonate = 0x400, 

        /// <summary>
        ///   No custom Marshal
        /// </summary>
        NoCustomMarshal = 0x2000, 

        /// <summary>
        ///   Disable auth
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1705:LongAcronymsShouldBePascalCased", MessageId = "Member", Justification = "Interop")]
        DisableAaa = 0x1000
    }

    /// <summary>
    /// The version of BITS.
    /// </summary>
    public enum BitsVersion
    {
        /// <summary>
        ///   undefined bits version
        /// </summary>
        BitsUndefined, 

        /// <summary>
        ///   BITS version 1.0
        /// </summary>
        Bits1, 

        /// <summary>
        ///   BITS version 1.2
        /// </summary>
        Bits1Dot2, 

        /// <summary>
        ///   BITS version 1.5
        /// </summary>
        Bits1Dot5, 

        /// <summary>
        ///   Bits Version 2.0
        /// </summary>
        Bits2, 

        /// <summary>
        ///   Bits version 2.5
        /// </summary>
        Bits2Dot5, 

        /// <summary>
        ///   Bits version 3.0
        /// </summary>
        Bits3, 
    }

    /// <summary>
    /// Win32 native methods.
    /// </summary>
    internal static class NativeMethods
    {
        /// <summary>
        /// COs the initialize security.
        /// </summary>
        /// <param name="handle">
        /// The handle.
        /// </param>
        /// <param name="authServiceLength">
        /// Length of the authentication service.
        /// </param>
        /// <param name="authService">
        /// The authentication service.
        /// </param>
        /// <param name="reserved1">
        /// The reserved1.
        /// </param>
        /// <param name="level">
        /// The level.
        /// </param>
        /// <param name="impersonationLevel">
        /// The impersonation level.
        /// </param>
        /// <param name="authList">
        /// The authentication list.
        /// </param>
        /// <param name="capabilities">
        /// The capabilities.
        /// </param>
        /// <param name="reserved3">
        /// The reserved3.
        /// </param>
        /// <returns>
        /// The result
        /// </returns>
        [DllImport(@"ole32.dll", CharSet = CharSet.Auto)]
        public static extern int COInitializeSecurity(
            IntPtr handle, 
            int authServiceLength, 
            IntPtr authService, 
            IntPtr reserved1, 
            RpcAuthLevels level, 
            RpcImpLevel impersonationLevel, 
            IntPtr authList, 
            EoAuthCap capabilities, 
            IntPtr reserved3);

        /// <summary>
        /// Converts the string SID to a the class
        /// </summary>
        /// <param name="sid">
        /// The SID as a string
        /// </param>
        /// <param name="sidPointer">
        /// The SID pointer.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if SID was converted
        /// </returns>
        [DllImport(@"advapi32.dll", CharSet = CharSet.Auto)]
        [return: MarshalAsAttribute(UnmanagedType.Bool)]
        internal static extern bool ConvertStringSidToSidW(string sid, ref IntPtr sidPointer);

        /// <summary>
        /// Lookups the account SID
        /// </summary>
        /// <param name="systemName">
        /// Name of the system.
        /// </param>
        /// <param name="sid">
        /// The SID pointer
        /// </param>
        /// <param name="name">
        /// The name of the account
        /// </param>
        /// <param name="refName">
        /// The domain name as a Int64
        /// </param>
        /// <param name="domainName">
        /// Name of the domain.
        /// </param>
        /// <param name="refDomainName">
        /// Name of the ref domain.
        /// </param>
        /// <param name="use">
        /// Value if the SID is used
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the lookup was successful
        /// </returns>
        [DllImport(@"advapi32.dll", CharSet = CharSet.Auto)]
        [return: MarshalAsAttribute(UnmanagedType.Bool)]
        internal static extern bool LookupAccountSidW(
            string systemName, IntPtr sid, StringBuilder name, ref long refName, StringBuilder domainName, ref long refDomainName, ref int use);

        /// <summary>
        /// Gets the file version info.
        /// </summary>
        /// <param name="fileName">
        /// Name of the file.
        /// </param>
        /// <param name="handle">
        /// The handle.
        /// </param>
        /// <param name="size">
        /// The file size
        /// </param>
        /// <param name="infoBuffer">
        /// The info buffer.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the version was retrieved
        /// </returns>
        [DllImport(@"version.dll", CharSet = CharSet.Auto)]
        internal static extern bool GetFileVersionInfo(string fileName, int handle, int size, byte[] infoBuffer);

        /// <summary>
        /// Gets the size of the file version info.
        /// </summary>
        /// <param name="fileName">
        /// Name of the file.
        /// </param>
        /// <param name="handle">
        /// The handle.
        /// </param>
        /// <returns>
        /// an integer
        /// </returns>
        [DllImport(@"version.dll", CharSet = CharSet.Auto)]
        internal static extern int GetFileVersionInfoSize(string fileName, out int handle);

        /// <summary>
        /// Verify the query value.
        /// </summary>
        /// <param name="block">
        /// The block.
        /// </param>
        /// <param name="subBlock">
        /// The sub block.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <param name="length">
        /// The length.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the value exists
        /// </returns>
        [DllImport(@"version.dll", CharSet = CharSet.Auto)]
        internal static extern bool VerQueryValue(byte[] block, string subBlock, out string value, out uint length);

        /// <summary>
        /// Verify the query value.
        /// </summary>
        /// <param name="bock">
        /// The byte value
        /// </param>
        /// <param name="subBlock">
        /// The sub block.
        /// </param>
        /// <param name="value">
        /// The pointer value.
        /// </param>
        /// <param name="length">
        /// The length.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the value exists
        /// </returns>
        [DllImport(@"version.dll", CharSet = CharSet.Auto)]
        internal static extern bool VerQueryValue(byte[] bock, string subBlock, out IntPtr value, out uint length);
    }
}