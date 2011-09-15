//-----------------------------------------------------------------------
// <copyright file="TimeConverter.cs" project="SevenUpdate.Installer" assembly="SevenUpdate.Installer" solution="SevenUpdate.Installer" company="Dino Chiesa">
//     Copyright (c) Dino Chiesa. All rights reserved.
// </copyright>
// <author username="Cheeso">Dino Chiesa</author>
// <summary></summary>
//-----------------------------------------------------------------------

namespace Tar
{
    using System;

    /// <summary>This class is intended for internal use only, by the Tar library.</summary>
    internal static class TimeConverter
    {
        #region Constants and Fields

        private static DateTime unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        #endregion

        #region Public Methods

        /// <param name="dateTime">
        /// </param><returns></returns>
        public static int DateTime2TimeT(DateTime dateTime)
        {
            var delta = dateTime - unixEpoch;
            return (int)delta.TotalSeconds;
        }

        /*
        public static long DateTime2Win32Ticks(DateTime dateTime)
        {
            var delta = dateTime - win32Epoch;
            return (long)(delta.TotalSeconds * 10000000L);
        }
*/

        /// <param name="time">
        /// </param><returns></returns>
        public static DateTime TimeT2DateTime(int time)
        {
            return unixEpoch.AddSeconds(time);
        }

        #endregion

        /*
        private static DateTime win32Epoch = new DateTime(1601, 1, 1, 0, 0, 0, DateTimeKind.Utc);
*/

        /*
        public static DateTime Win32Ticks2DateTime(long ticks)
        {
            return win32Epoch.AddSeconds(ticks / 10000000);
        }
*/
    }
}
