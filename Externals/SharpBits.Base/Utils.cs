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
    using System.Globalization;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Text;

    /// <summary>
    /// </summary>
    [SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue")]
    [Flags]
    public enum RpcAuthnLevels
    {
        /// <summary>
        /// </summary>
        Default = 0, 

        /// <summary>
        /// </summary>
        None = 1, 

        /// <summary>
        /// </summary>
        Connect = 2, 

        /// <summary>
        /// </summary>
        Call = 3, 

        /// <summary>
        /// </summary>
        Pkt = 4, 

        /// <summary>
        /// </summary>
        PktIntegrity = 5, 

        /// <summary>
        /// </summary>
        PktPrivacy = 6
    }

    /// <summary>
    /// </summary>
    public enum RpcImpLevel
    {
        /// <summary>
        /// </summary>
        Default = 0, 

        /// <summary>
        /// </summary>
        Anonymous = 1, 

        /// <summary>
        /// </summary>
        Identify = 2, 

        /// <summary>
        /// </summary>
        Impersonate = 3, 

        /// <summary>
        /// </summary>
        Delegate = 4
    }

    /// <summary>
    /// </summary>
    [SuppressMessage("Microsoft.Naming", "CA1706:ShortAcronymsShouldBeUppercase")]
    [SuppressMessage("Microsoft.Design", "CA1027:MarkEnumsWithFlags")]
    public enum EoAuthnCap
    {
        /// <summary>
        /// </summary>
        None = 0x00, 

        /// <summary>
        /// </summary>
        MutualAuth = 0x01, 

        /// <summary>
        /// </summary>
        StaticCloaking = 0x20, 

        /// <summary>
        /// </summary>
        DynamicCloaking = 0x40, 

        /// <summary>
        /// </summary>
        AnyAuthority = 0x80, 

        /// <summary>
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1705:LongAcronymsShouldBePascalCased", MessageId = "Member")]
        MakeFullSic = 0x100, 

        /// <summary>
        /// </summary>
        Default = 0x800, 

        /// <summary>
        /// </summary>
        SecureRefs = 0x02, 

        /// <summary>
        /// </summary>
        AccessControl = 0x04, 

        /// <summary>
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1706:ShortAcronymsShouldBeUppercase", MessageId = "Member")]
        AppID = 0x08, 

        /// <summary>
        /// </summary>
        Dynamic = 0x10, 

        /// <summary>
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1705:LongAcronymsShouldBePascalCased", MessageId = "Member")]
        RequireFullSic = 0x200, 

        /// <summary>
        /// </summary>
        AutoImpersonate = 0x400, 

        /// <summary>
        /// </summary>
        NoCustomMarshal = 0x2000, 

        /// <summary>
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1705:LongAcronymsShouldBePascalCased", MessageId = "Member")]
        DisableAaa = 0x1000
    }

    /// <summary>
    /// </summary>
    public enum BitsVersion
    {
        /// <summary>
        /// </summary>
        Bits00, // undefinied
        /// <summary>
        /// </summary>
        Bits10, 

        /// <summary>
        /// </summary>
        Bits12, 

        /// <summary>
        /// </summary>
        Bits15, 

        /// <summary>
        /// </summary>
        Bits20, 

        /// <summary>
        /// </summary>
        Bits25, 

        /// <summary>
        /// </summary>
        Bits30, 
    }

    /// <summary>
    /// </summary>
    internal static class NativeMethods
    {
        /// <summary>
        /// </summary>
        /// <param name="SID">
        /// </param>
        /// <param name="sid">
        /// </param>
        /// <returns>
        /// </returns>
        [DllImport("advapi32.dll", CharSet = CharSet.Auto)]
        [return: MarshalAsAttribute(UnmanagedType.Bool)]
        internal static extern bool ConvertStringSidToSidW(string SID, ref IntPtr sid);

        /// <summary>
        /// </summary>
        /// <param name="systemName">
        /// </param>
        /// <param name="sid">
        /// </param>
        /// <param name="name">
        /// </param>
        /// <param name="cbName">
        /// </param>
        /// <param name="domainName">
        /// </param>
        /// <param name="cbDomainName">
        /// </param>
        /// <param name="psUse">
        /// </param>
        /// <returns>
        /// </returns>
        [DllImport("advapi32.dll", CharSet = CharSet.Auto)]
        [return: MarshalAsAttribute(UnmanagedType.Bool)]
        internal static extern bool LookupAccountSidW(
            string systemName, IntPtr sid, StringBuilder name, ref long cbName, StringBuilder domainName, ref long cbDomainName, ref int psUse);

        /// <summary>
        /// </summary>
        /// <param name="pVoid">
        /// </param>
        /// <param name="cAuthSvc">
        /// </param>
        /// <param name="asAuthSvc">
        /// </param>
        /// <param name="pReserved1">
        /// </param>
        /// <param name="level">
        /// </param>
        /// <param name="impers">
        /// </param>
        /// <param name="pAuthList">
        /// </param>
        /// <param name="dwCapabilities">
        /// </param>
        /// <param name="pReserved3">
        /// </param>
        /// <returns>
        /// </returns>
        [DllImport("ole32.dll", CharSet = CharSet.Auto)]
        public static extern int COInitializeSecurity(
            IntPtr pVoid, 
            int cAuthSvc, 
            IntPtr asAuthSvc, 
            IntPtr pReserved1, 
            RpcAuthnLevels level, 
            RpcImpLevel impers, 
            IntPtr pAuthList, 
            EoAuthnCap dwCapabilities, 
            IntPtr pReserved3);

        /// <summary>
        /// </summary>
        /// <param name="sFileName">
        /// </param>
        /// <param name="handle">
        /// </param>
        /// <param name="size">
        /// </param>
        /// <param name="infoBuffer">
        /// </param>
        /// <returns>
        /// </returns>
        [DllImport("version.dll", CharSet = CharSet.Auto)]
        internal static extern bool GetFileVersionInfo(string sFileName, int handle, int size, byte[] infoBuffer);

        /// <summary>
        /// </summary>
        /// <param name="sFileName">
        /// </param>
        /// <param name="handle">
        /// </param>
        /// <returns>
        /// </returns>
        [DllImport("version.dll", CharSet = CharSet.Auto)]
        internal static extern int GetFileVersionInfoSize(string sFileName, out int handle);

        /// <summary>
        /// </summary>
        /// <param name="pBlock">
        /// </param>
        /// <param name="pSubBlock">
        /// </param>
        /// <param name="pValue">
        /// </param>
        /// <param name="len">
        /// </param>
        /// <returns>
        /// </returns>
        [DllImport("version.dll", CharSet = CharSet.Auto)]
        internal static extern bool VerQueryValue(byte[] pBlock, string pSubBlock, out string pValue, out uint len);

        /// <summary>
        /// </summary>
        /// <param name="pBlock">
        /// </param>
        /// <param name="pSubBlock">
        /// </param>
        /// <param name="pValue">
        /// </param>
        /// <param name="len">
        /// </param>
        /// <returns>
        /// </returns>
        [DllImport("version.dll", CharSet = CharSet.Auto)]
        internal static extern bool VerQueryValue(byte[] pBlock, string pSubBlock, out IntPtr pValue, out uint len);
    }

    /// <summary>
    /// </summary>
    internal static class Utils
    {
        #region Constructors and Destructors

        /// <summary>
        /// </summary>
        static Utils()
        {
            BitsVersion = GetBitsVersion();
        }

        #endregion

        #region Properties

        /// <summary>
        /// </summary>
        internal static BitsVersion BitsVersion { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        /// </summary>
        /// <param name="dateTime">
        /// </param>
        /// <returns>
        /// </returns>
        internal static FILETIME DateTime2FileTime(DateTime dateTime)
        {
            long fileTime = 0;
            if (dateTime != DateTime.MinValue)
            {
                // Checking for MinValue
                fileTime = dateTime.ToFileTime();
            }

            var resultingFileTime = new FILETIME { DWLowDateTime = (uint)(fileTime & 0xFFFFFFFF), DWHighDateTime = (uint)(fileTime >> 32) };
            return resultingFileTime;
        }

        /// <summary>
        /// </summary>
        /// <param name="fileTime">
        /// </param>
        /// <returns>
        /// </returns>
        internal static DateTime FileTime2DateTime(FILETIME fileTime)
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
        /// </summary>
        /// <param name="sid">
        /// </param>
        /// <returns>
        /// </returns>
        internal static string GetName(string sid)
        {
            const int size = 255;
            long cbUserName = size;
            long cbDomainName = size;
            var ptrSID = new IntPtr(0);
            var psUse = 0;
            var userName = new StringBuilder(size);
            var domainName = new StringBuilder(size);
            if (NativeMethods.ConvertStringSidToSidW(sid, ref ptrSID))
            {
                if (NativeMethods.LookupAccountSidW(string.Empty, ptrSID, userName, ref cbUserName, domainName, ref cbDomainName, ref psUse))
                {
                    return string.Concat(domainName.ToString(), "\\", userName.ToString());
                }
            }

            return string.Empty;
        }

        /// <summary>
        /// maps version information from file version
        ///   version number returned by qmgr.dll
        ///   6.0.xxxx = BITS 1.0
        ///   6.2.xxxx = BITS 1.2
        ///   6.5.xxxx = BITS 1.5
        ///   6.6.xxxx = BITS 2.0
        ///   6.7.xxxx = BITS 2.5
        ///   7.0.xxxx = BITS 3.0
        /// </summary>
        /// <returns>
        /// </returns>
        private static BitsVersion GetBitsVersion()
        {
            try
            {
                var fileName = Path.Combine(Environment.SystemDirectory, "qmgr.dll");
                int handle;
                var size = NativeMethods.GetFileVersionInfoSize(fileName, out handle);
                if (size == 0)
                {
                    return BitsVersion.Bits00;
                }

                var buffer = new byte[size];
                if (!NativeMethods.GetFileVersionInfo(fileName, handle, size, buffer))
                {
                    return BitsVersion.Bits00;
                }

                IntPtr subBlock;
                uint len;
                if (!NativeMethods.VerQueryValue(buffer, @"\VarFileInfo\Translation", out subBlock, out len))
                {
                    return BitsVersion.Bits00;
                }

                int block1 = Marshal.ReadInt16(subBlock);
                int block2 = Marshal.ReadInt16((IntPtr)((int)subBlock + 2));
                var spv = string.Format(CultureInfo.CurrentCulture, @"\StringFileInfo\{0:X4}{1:X4}\ProductVersion", block1, block2);

                string versionInfo;
                if (!NativeMethods.VerQueryValue(buffer, spv, out versionInfo, out len))
                {
                    return BitsVersion.Bits00;
                }

                var versionNumbers = versionInfo.Split('.');

                if (versionNumbers.Length < 2)
                {
                    return BitsVersion.Bits00;
                }

                var major = int.Parse(versionNumbers[0], CultureInfo.CurrentCulture);
                var minor = int.Parse(versionNumbers[1], CultureInfo.CurrentCulture);

                switch (major)
                {
                    case 6:
                        switch (minor)
                        {
                            case 0:
                                return BitsVersion.Bits10;
                            case 2:
                                return BitsVersion.Bits12;
                            case 5:
                                return BitsVersion.Bits15;
                            case 6:
                                return BitsVersion.Bits20;
                            case 7:
                                return BitsVersion.Bits25;
                            default:
                                return BitsVersion.Bits00;
                        }

                    case 7:
                        return BitsVersion.Bits30;
                    default:
                        return BitsVersion.Bits00;
                }
            }
            catch
            {
                return BitsVersion.Bits00;
            }
        }

        #endregion
    }
}