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
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Text;

    /// <summary>The authentication level.</summary>
    [SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue", Justification = "Interop")]
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

    /// <summary>The Impersonation level.</summary>
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

    /// <summary>Defines authentication.</summary>
    [SuppressMessage("Microsoft.Naming", "CA1706:ShortAcronymsShouldBeUppercase", Justification = "Interop")]
    [SuppressMessage("Microsoft.Design", "CA1027:MarkEnumsWithFlags", Justification = "Interop")]
    public enum EoAuthCap
    {
        /// <summary>
        ///   No authentication
        /// </summary>
        None = 0x00, 

        /// <summary>
        ///   Mutal authentication
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

    /// <summary>The version of BITS.</summary>
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

    /// <summary>Win32 native methods.</summary>
    internal static class NativeMethods
    {
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
        [DllImport("advapi32.dll", CharSet = CharSet.Auto)]
        [return: MarshalAsAttribute(UnmanagedType.Bool)]
        internal static extern bool ConvertStringSidToSidW(string sid, ref IntPtr sidPointer);

        /// <summary>
        /// Lookups the account SID
        /// </summary>
        /// <param name="systemName">Name of the system.</param>
        /// <param name="sid">The SID pointer</param>
        /// <param name="name">The name of the account</param>
        /// <param name="refName">The domain name as a Int64</param>
        /// <param name="domainName">Name of the domain.</param>
        /// <param name="refDomainName">Name of the ref domain.</param>
        /// <param name="use">Value if the SID is used</param>
        /// <returns>
        /// <see langword="true"/> if the lookup was successful
        /// </returns>
        [DllImport("advapi32.dll", CharSet = CharSet.Auto)]
        [return: MarshalAsAttribute(UnmanagedType.Bool)]
        internal static extern bool LookupAccountSidW(
            string systemName, IntPtr sid, StringBuilder name, ref long refName, StringBuilder domainName, ref long refDomainName, ref int use);

        /// <summary>
        /// COs the initialize security.
        /// </summary>
        /// <param name="pVoid">The p void.</param>
        /// <param name="authSvc">The auth SVC.</param>
        /// <param name="asAuthSvc">As auth SVC.</param>
        /// <param name="reserved1">The reserved1.</param>
        /// <param name="level">The level.</param>
        /// <param name="impersonationLevel">The impersonation level.</param>
        /// <param name="authList">The auth list.</param>
        /// <param name="capabilities">The capabilities.</param>
        /// <param name="reserved3">The reserved3.</param>
        /// <returns></returns>
        [DllImport("ole32.dll", CharSet = CharSet.Auto)]
        public static extern int COInitializeSecurity(
            IntPtr pVoid, 
            int authSvc, 
            IntPtr asAuthSvc, 
            IntPtr reserved1, 
            RpcAuthLevels level, 
            RpcImpLevel impersonationLevel, 
            IntPtr authList, 
            EoAuthCap capabilities, 
            IntPtr reserved3);

        /// <summary>
        /// Gets the file version info.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="handle">The handle.</param>
        /// <param name="size">The file size</param>
        /// <param name="infoBuffer">The info buffer.</param>
        /// <returns>
        /// <see langword="true"/> if the version was retrieved
        /// </returns>
        [DllImport("version.dll", CharSet = CharSet.Auto)]
        internal static extern bool GetFileVersionInfo(string fileName, int handle, int size, byte[] infoBuffer);

        /// <summary>
        /// Gets the size of the file version info.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="handle">The handle.</param>
        /// <returns>an integer</returns>
        [DllImport("version.dll", CharSet = CharSet.Auto)]
        internal static extern int GetFileVersionInfoSize(string fileName, out int handle);

        /// <summary>
        /// Verify the query value.
        /// </summary>
        /// <param name="block">The block.</param>
        /// <param name="subBlock">The sub block.</param>
        /// <param name="value">The value.</param>
        /// <param name="length">The length.</param>
        /// <returns>
        /// <see langword="true"/> if the value exists
        /// </returns>
        [DllImport("version.dll", CharSet = CharSet.Auto)]
        internal static extern bool VerQueryValue(byte[] block, string subBlock, out string value, out uint length);

        /// <summary>
        /// Verify the query value.
        /// </summary>
        /// <param name="bock">The byte value</param>
        /// <param name="subBlock">The sub block.</param>
        /// <param name="value">The pointer value.</param>
        /// <param name="length">The length.</param>
        /// <returns>
        /// <see langword="true"/> if the value exists
        /// </returns>
        [DllImport("version.dll", CharSet = CharSet.Auto)]
        internal static extern bool VerQueryValue(byte[] bock, string subBlock, out IntPtr value, out uint length);
    }

    /// <summary>Various utility methods.</summary>
    [SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Utility class")]
    internal static class Utils
    {
        #region Constructors and Destructors

        /// <summary>
        ///   Initializes static members of the <see cref = "Utils" /> class.
        /// </summary>
        static Utils()
        {
            BitsVersion = GetBitsVersion();
        }

        #endregion

        #region Properties

        /// <summary>
        ///   Gets the bits version.
        /// </summary>
        /// <value>The bits version.</value>
        internal static BitsVersion BitsVersion { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        /// Converts a <see cref="DateTime"/> to <see cref="FileTime"/>
        /// </summary>
        /// <param name="dateTime">The date time.</param>
        /// <returns>
        /// The converted <see cref="DateTime"/> as a <see cref="FileTime"/>
        /// </returns>
        internal static FileTime DateTimeToFileTime(DateTime dateTime)
        {
            long fileTime = 0;
            if (dateTime != DateTime.MinValue)
            {
                // Checking for MinValue
                fileTime = dateTime.ToFileTime();
            }

            var resultingFileTime = new FileTime { DWLowDateTime = (uint)(fileTime & 0xFFFFFFFF), DWHighDateTime = (uint)(fileTime >> 32) };
            return resultingFileTime;
        }

        /// <summary>
        /// Converts the <see cref="FileTime"/> to <see cref="DateTime"/>
        /// </summary>
        /// <param name="fileTime">The file time.</param>
        /// <returns>
        /// The converted <see cref="FileTime"/> to <see cref="DateTime"/>
        /// </returns>
        internal static DateTime FileTimeToDateTime(FileTime fileTime)
        {
            if (fileTime.DWHighDateTime == 0 && fileTime.DWLowDateTime == 0)
            {
                // Checking for MinValue
                return DateTime.MinValue;
            }

            var dateTime = (((long)fileTime.DWHighDateTime) << 32) + fileTime.DWLowDateTime;
            return DateTime.FromFileTime(dateTime);
        }

        /// <summary>
        /// Gets the name from a SID
        /// </summary>
        /// <param name="sid">The SID as a string</param>
        /// <returns>The name from the SID</returns>
        internal static string GetName(string sid)
        {
            const int Size = 255;
            long userNameSize = Size;
            long domainNameSize = Size;
            var ptrSid = new IntPtr(0);
            var use = 0;
            var userName = new StringBuilder(Size);
            var domainName = new StringBuilder(Size);
            if (NativeMethods.ConvertStringSidToSidW(sid, ref ptrSid))
            {
                if (NativeMethods.LookupAccountSidW(string.Empty, ptrSid, userName, ref userNameSize, domainName, ref domainNameSize, ref use))
                {
                    return string.Concat(domainName.ToString(), "\\", userName.ToString());
                }
            }

            return string.Empty;
        }

        /// <summary>
        /// Gets the BITS Version installed on the system
        /// </summary>
        /// <returns>The Bits Version</returns>
        private static BitsVersion GetBitsVersion()
        {
            try
            {
                var fileName = Path.Combine(Environment.SystemDirectory, @"qmgr.dll");
                int handle;
                var size = NativeMethods.GetFileVersionInfoSize(fileName, out handle);
                if (size == 0)
                {
                    return BitsVersion.BitsUndefined;
                }

                var buffer = new byte[size];
                if (!NativeMethods.GetFileVersionInfo(fileName, handle, size, buffer))
                {
                    return BitsVersion.BitsUndefined;
                }

                IntPtr subBlock;
                uint len;
                if (!NativeMethods.VerQueryValue(buffer, @"\VarFileInfo\Translation", out subBlock, out len))
                {
                    return BitsVersion.BitsUndefined;
                }

                int block1 = Marshal.ReadInt16(subBlock);
                int block2 = Marshal.ReadInt16((IntPtr)((int)subBlock + 2));
                var spv = string.Format(CultureInfo.CurrentCulture, @"\StringFileInfo\{0:X4}{1:X4}\ProductVersion", block1, block2);

                string versionInfo;
                if (!NativeMethods.VerQueryValue(buffer, spv, out versionInfo, out len))
                {
                    return BitsVersion.BitsUndefined;
                }

                var versionNumbers = versionInfo.Split('.');

                if (versionNumbers.Length < 2)
                {
                    return BitsVersion.BitsUndefined;
                }

                var major = int.Parse(versionNumbers[0], CultureInfo.CurrentCulture);
                var minor = int.Parse(versionNumbers[1], CultureInfo.CurrentCulture);

                switch (major)
                {
                    case 6:
                        switch (minor)
                        {
                            case 0:
                                return BitsVersion.Bits1;
                            case 2:
                                return BitsVersion.Bits1Dot2;
                            case 5:
                                return BitsVersion.Bits1Dot5;
                            case 6:
                                return BitsVersion.Bits2;
                            case 7:
                                return BitsVersion.Bits2Dot5;
                            default:
                                return BitsVersion.BitsUndefined;
                        }

                    case 7:
                        return BitsVersion.Bits3;
                    default:
                        return BitsVersion.BitsUndefined;
                }
            }
            catch
            {
                return BitsVersion.BitsUndefined;
            }
        }

        #endregion
    }
}