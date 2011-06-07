//-----------------------------------------------------------------------
// <copyright file="Extensions.cs" project="SevenUpdate.Installer" assembly="SevenUpdate.Installer" solution="SevenUpdate.Installer" company="Dino Chiesa">
//     Copyright (c) Dino Chiesa. All rights reserved.
// </copyright>
// <author username="Cheeso">Dino Chiesa</author>
// <summary></summary>
//-----------------------------------------------------------------------

namespace Tar
{
    using System.IO;

    /// <summary>This class is intended for internal use only, by the Tar library.</summary>
    internal static class Extensions
    {
        #region Public Methods

        /// <param name="t">
        /// </param>
        /// <returns>
        /// </returns>
        public static string TrimNull(this string t)
        {
            return t.Trim(new[] { (char)0x20, (char)0x00 });
        }

        /// <param name="t">
        /// </param>
        /// <returns>
        /// </returns>
        public static string TrimSlash(this string t)
        {
            return t.TrimEnd(new[] { Path.DirectorySeparatorChar }).TrimStart(new[] { Path.DirectorySeparatorChar });
        }

        #endregion

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