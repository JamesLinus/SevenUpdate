// <copyright file="TimeConverter.cs" project="Tar">Dino Chiesa</copyright>
// <license href="http://www.gnu.org/licenses/gpl-3.0.txt" name="GNU General Public License 3" />

namespace Tar
{
    using System;

    /// <summary>This class is intended for internal use only, by the Tar library.</summary>
    internal static class TimeConverter
    {
        private static DateTime unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        /// <param name="dateTime">
        /// </param><returns></returns>
        public static int DateTime2TimeT(DateTime dateTime)
        {
            TimeSpan delta = dateTime - unixEpoch;
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