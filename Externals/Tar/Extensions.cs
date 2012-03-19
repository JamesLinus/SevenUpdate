// <copyright file="Extensions.cs" project="Tar">Dino Chiesa</copyright>
// <license href="http://www.gnu.org/licenses/gpl-3.0.txt" name="GNU General Public License 3" />

namespace Tar
{
    using System.IO;

    /// <summary>This class is intended for internal use only, by the Tar library.</summary>
    internal static class Extensions
    {
        /// <param name="t">
        /// </param><returns></returns>
        public static string TrimNull(this string t)
        {
            return t.Trim(new[] { (char)0x20, (char)0x00 });
        }

        /// <param name="t">
        /// </param><returns></returns>
        public static string TrimSlash(this string t)
        {
            return t.TrimEnd(new[] { Path.DirectorySeparatorChar }).TrimStart(new[] { Path.DirectorySeparatorChar });
        }

        /*
        public static string TrimVolume(this string t)
        {
            if (t.Length > 3 && t[1] == ':' && t[2] == Path.DirectorySeparatorChar)
            {
                return t.Substring(3);
            }

            if (t.Length > 2 && t[0] == Path.DirectorySeparatorChar && t[1] == Path.DirectorySeparatorChar)
            {
                return t.Substring(2);
            }

            return t;
        }
*/
    }
}