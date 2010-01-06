#region GNU Public License v3

// Copyright 2007, 2008 Robert Baker, aka Seven ALive.
// This file is part of Seven Update.
//  
//     Seven Update is free software: you can redistribute it and/or modify
//     it under the terms of the GNU General Public License as published by
//     the Free Software Foundation, either version 3 of the License, or
//     (at your option) any later version.
// 
//     Seven Update is distributed in the hope that it will be useful,
//     but WITHOUT ANY WARRANTY; without even the implied warranty of
//     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//     GNU General Public License for more details.
//  
//    You should have received a copy of the GNU General Public License
//    along with Seven Update.  If not, see <http://www.gnu.org/licenses/>.

#endregion

#region

using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

#endregion

namespace SharpBits.Base
{
    [SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue"), Flags]
    public enum RpcAuthnLevel
    {
        Default = 0,
        None = 1,
        Connect = 2,
        Call = 3,
        Pkt = 4,
        PktIntegrity = 5,
        PktPrivacy = 6
    }

    public enum RpcImpLevel
    {
        Default = 0,
        Anonymous = 1,
        Identify = 2,
        Impersonate = 3,
        Delegate = 4
    }

    [SuppressMessage("Microsoft.Naming", "CA1706:ShortAcronymsShouldBeUppercase"), SuppressMessage("Microsoft.Design", "CA1027:MarkEnumsWithFlags")]
    public enum EoAuthnCap
    {
        None = 0x00,
        MutualAuth = 0x01,
        StaticCloaking = 0x20,
        DynamicCloaking = 0x40,
        AnyAuthority = 0x80,
        [SuppressMessage("Microsoft.Naming", "CA1705:LongAcronymsShouldBePascalCased", MessageId = "Member")]
        MakeFullSIC = 0x100,
        Default = 0x800,
        SecureRefs = 0x02,
        AccessControl = 0x04,
        [SuppressMessage("Microsoft.Naming", "CA1706:ShortAcronymsShouldBeUppercase", MessageId = "Member")]
        AppID = 0x08,
        Dynamic = 0x10,
        [SuppressMessage("Microsoft.Naming", "CA1705:LongAcronymsShouldBePascalCased", MessageId = "Member")]
        RequireFullSIC = 0x200,
        AutoImpersonate = 0x400,
        NoCustomMarshal = 0x2000,
        [SuppressMessage("Microsoft.Naming", "CA1705:LongAcronymsShouldBePascalCased", MessageId = "Member")]
        DisableAAA = 0x1000
    }

    public enum BitsVersion
    {
        Bits0_0, //undefinied
        Bits1_0,
        Bits1_2,
        Bits1_5,
        Bits2_0,
        Bits2_5,
        Bits3_0,
    }

    internal static class NativeMethods
    {
        [DllImport("advapi32.dll", CharSet = CharSet.Auto)]
        [return: MarshalAsAttribute(UnmanagedType.Bool)]
        internal static extern bool ConvertStringSidToSidW(string stringSID, ref IntPtr sid);

        [DllImport("advapi32.dll", CharSet = CharSet.Auto)]
        [return: MarshalAsAttribute(UnmanagedType.Bool)]
        internal static extern bool LookupAccountSidW(string systemName, IntPtr sid, StringBuilder name, ref long cbName, StringBuilder domainName, ref long cbDomainName, ref int psUse);

        [DllImport("ole32.dll", CharSet = CharSet.Auto)]
        public static extern int CoInitializeSecurity(IntPtr pVoid, int cAuthSvc, IntPtr asAuthSvc, IntPtr pReserved1, RpcAuthnLevel level, RpcImpLevel impers, IntPtr pAuthList,
                                                      EoAuthnCap dwCapabilities, IntPtr pReserved3);

        [DllImport("version.dll", CharSet = CharSet.Auto)]
        internal static extern bool GetFileVersionInfo(string sFileName, int handle, int size, byte[] infoBuffer);

        [DllImport("version.dll", CharSet = CharSet.Auto)]
        internal static extern int GetFileVersionInfoSize(string sFileName, out int handle);

        [DllImport("version.dll", CharSet = CharSet.Auto)]
        internal static extern bool VerQueryValue(byte[] pBlock, string pSubBlock, out string pValue, out uint len);

        [DllImport("version.dll", CharSet = CharSet.Auto)]
        internal static extern bool VerQueryValue(byte[] pBlock, string pSubBlock, out IntPtr pValue, out uint len);
    }

    internal static class Utils
    {
        private static BitsVersion version;

        static Utils()
        {
            version = GetBitsVersion();
        }

        internal static BitsVersion BITSVersion { get { return version; } }

        internal static string GetName(string sid)
        {
            const int size = 255;
            long cbUserName = size;
            long cbDomainName = size;
            var ptrSID = new IntPtr(0);
            int psUse = 0;
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

        internal static FILETIME DateTime2FileTime(DateTime dateTime)
        {
            long fileTime = 0;
            if (dateTime != DateTime.MinValue) //Checking for MinValue
                fileTime = dateTime.ToFileTime();
            var resultingFileTime = new FILETIME {dwLowDateTime = (uint) (fileTime & 0xFFFFFFFF), dwHighDateTime = (uint) (fileTime >> 32)};
            return resultingFileTime;
        }

        internal static DateTime FileTime2DateTime(FILETIME fileTime)
        {
            if (fileTime.dwHighDateTime == 0 && fileTime.dwLowDateTime == 0) //Checking for MinValue
                return DateTime.MinValue;

            long dateTime = (((long) fileTime.dwHighDateTime) << 32) + fileTime.dwLowDateTime;
            return DateTime.FromFileTime(dateTime);
        }

        /// <summary>
        /// maps version information from file version
        /// version number returned by qmgr.dll
        /// 6.0.xxxx = BITS 1.0
        /// 6.2.xxxx = BITS 1.2
        /// 6.5.xxxx = BITS 1.5
        /// 6.6.xxxx = BITS 2.0
        /// 6.7.xxxx = BITS 2.5
        /// 7.0.xxxx = BITS 3.0
        /// </summary>
        /// <returns></returns>
        private static BitsVersion GetBitsVersion()
        {
            try
            {
                string fileName = Path.Combine(Environment.SystemDirectory, "qmgr.dll");
                int handle;
                int size = NativeMethods.GetFileVersionInfoSize(fileName, out handle);
                if (size == 0)
                    return BitsVersion.Bits0_0;
                var buffer = new byte[size];
                if (!NativeMethods.GetFileVersionInfo(fileName, handle, size, buffer))
                {
                    return BitsVersion.Bits0_0;
                }
                IntPtr subBlock;
                uint len;
                if (!NativeMethods.VerQueryValue(buffer, @"\VarFileInfo\Translation", out subBlock, out len))
                {
                    return BitsVersion.Bits0_0;
                }

                int block1 = Marshal.ReadInt16(subBlock);
                int block2 = Marshal.ReadInt16((IntPtr) ((int) subBlock + 2));
                string spv = string.Format(@"\StringFileInfo\{0:X4}{1:X4}\ProductVersion", block1, block2);

                string versionInfo;
                if (!NativeMethods.VerQueryValue(buffer, spv, out versionInfo, out len))
                {
                    return BitsVersion.Bits0_0;
                }

                string[] versionNumbers = versionInfo.Split('.');

                if (versionNumbers.Length < 2)
                    return BitsVersion.Bits0_0;

                int major = int.Parse(versionNumbers[0]);
                int minor = int.Parse(versionNumbers[1]);

                switch (major)
                {
                    case 6:
                        switch (minor)
                        {
                            case 0:
                                return BitsVersion.Bits1_0;
                            case 2:
                                return BitsVersion.Bits1_2;
                            case 5:
                                return BitsVersion.Bits1_5;
                            case 6:
                                return BitsVersion.Bits2_0;
                            case 7:
                                return BitsVersion.Bits2_5;
                            default:
                                return BitsVersion.Bits0_0;
                        }
                    case 7:
                        return BitsVersion.Bits3_0;
                    default:
                        return BitsVersion.Bits0_0;
                }
            }
            catch
            {
                return BitsVersion.Bits0_0;
            }
        }
    }
}