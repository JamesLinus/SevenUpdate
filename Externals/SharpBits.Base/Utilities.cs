// ***********************************************************************
// <copyright file="Utilities.cs" project="SharpBits.Base" assembly="SharpBits.Base" solution="SevenUpdate" company="Xidar Solutions">
//     Copyright (c) xidar solutions. All rights reserved.
// </copyright>
// <author username="xidar">xidar</author>
// <author username="sevenalive">Robert Baker</author>
// <license href="http://sharpbits.codeplex.com/license">BSD License</license> 
// ***********************************************************************

namespace SharpBits.Base
{
    using System;
    using System.Text;

    /// <summary>Various utility methods.</summary>
    internal static class Utilities
    {
        /// <summary>Converts the <c>FileTime</c> to <c>DateTime</c>.</summary>
        /// <param name="fileTime">The file time.</param>
        /// <returns>The converted <c>FileTime</c> to <c>DateTime</c>.</returns>
        internal static DateTime FileTimeToDateTime(FileTime fileTime)
        {
            if (fileTime.DWHighDateTime == 0 && fileTime.DWLowDateTime == 0)
            {
                // Checking for MinValue
                return DateTime.MinValue;
            }

            long dateTime = (((long)fileTime.DWHighDateTime) << 32) + fileTime.DWLowDateTime;
            return DateTime.FromFileTime(dateTime);
        }

        /// <summary>Gets the name from a SID.</summary>
        /// <param name="sid">The SID as a string.</param>
        /// <returns>The name from the SID.</returns>
        internal static string GetName(string sid)
        {
            long userNameSize = 255;
            long domainNameSize = 255;
            var pointerSid = new IntPtr(0);
            int use = 0;
            var userName = new StringBuilder(255);
            var domainName = new StringBuilder(255);
            if (NativeMethods.ConvertStringSidToSidW(sid, ref pointerSid))
            {
                if (NativeMethods.LookupAccountSidW(
                        string.Empty, pointerSid, userName, ref userNameSize, domainName, ref domainNameSize, ref use))
                {
                    return string.Concat(domainName.ToString(), "\\", userName.ToString());
                }
            }

            return string.Empty;
        }
    }
}