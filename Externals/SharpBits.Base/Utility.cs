// ***********************************************************************
// <copyright file="Utility.cs"
//            project="SharpBits.Base"
//            assembly="SharpBits.Base"
//            solution="SevenUpdate"
//            company="Xidar Solutions">
//     Copyright (c) xidar solutions. All rights reserved.
// </copyright>
// <author username="xidar">xidar/author>
// ***********************************************************************
namespace SharpBits.Base
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Text;

    /// <summary>
    /// Various utility methods.
    /// </summary>
    internal static class Utilities
    {
        #region Properties

        /// <summary>
        ///   Gets the bits version.
        /// </summary>
        /// <value>The bits version.</value>
        internal static BitsVersion BitsVersion
        {
            get
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
                    int block2 = Marshal.ReadInt16(subBlock, 2);
                    var spv = string.Format(@"\StringFileInfo\{0:X4}{1:X4}\ProductVersion", block1, block2);

                    IntPtr versionInfoPtr;
                    if (!NativeMethods.VerQueryValue(buffer, spv, out versionInfoPtr, out len))
                    {
                        return BitsVersion.BitsUndefined;
                    }

                    var versionInfo = Marshal.PtrToStringAuto(versionInfoPtr);

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
                catch (Exception)
                {
                    return BitsVersion.BitsUndefined;
                }
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Converts a <see cref="DateTime"/> to <see cref="FileTime"/>
        /// </summary>
        /// <param name="dateTime">
        /// The date time.
        /// </param>
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
        /// <param name="fileTime">
        /// The file time.
        /// </param>
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
        /// <param name="sid">
        /// The SID as a string
        /// </param>
        /// <returns>
        /// The name from the SID
        /// </returns>
        internal static string GetName(string sid)
        {
            long userNameSize = 255;
            long domainNameSize = 255;
            var pointerSid = new IntPtr(0);
            var use = 0;
            var userName = new StringBuilder(255);
            var domainName = new StringBuilder(255);
            if (NativeMethods.ConvertStringSidToSidW(sid, ref pointerSid))
            {
                if (NativeMethods.LookupAccountSidW(string.Empty, pointerSid, userName, ref userNameSize, domainName, ref domainNameSize, ref use))
                {
                    return string.Concat(domainName.ToString(), "\\", userName.ToString());
                }
            }

            return string.Empty;
        }

        #endregion
    }
}