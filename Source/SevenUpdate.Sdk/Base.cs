#region

using System;

#endregion

namespace SevenUpdate.Sdk
{
    internal static class Base
    {
        /// <summary>
        ///   The application information of the project
        /// </summary>
        internal static Sua AppInfo { get; set; }

        /// <summary>
        ///   The current update being edited
        /// </summary>
        internal static Update UpdateInfo { get; set; }

        /// <summary>
        ///   Checks to see if a Url is valid
        /// </summary>
        /// <param name = "url">A url to check</param>
        /// <returns>True if url is valid, otherwise false</returns>
        internal static bool CheckUrl(string url)
        {
            try
            {
                new Uri(url);
            }
            catch
            {
                return false;
            }
            return true;
        }
    }
}