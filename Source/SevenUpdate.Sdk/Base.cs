#region

using System;
using System.Net;

#endregion

namespace SevenUpdate.Sdk
{
    internal static class Base
    {
        /// <summary>
        ///   The application information of the project
        /// </summary>
        public static Sua AppInfo { get; set; }

        /// <summary>
        ///   The current update being edited
        /// </summary>
        internal static Update UpdateInfo { get; set; }

        /// <summary>
        ///   Checks to see if a Url is valid and on the internet
        /// </summary>
        /// <param name = "url">A url to check</param>
        /// <returns>True if url is valid, otherwise false</returns>
        internal static bool CheckUrl(string url)
        {
            try
            {
                new Uri(url);
                var request = WebRequest.Create(url);
                request.Timeout = 15000;
                request.GetResponse();
            }
            catch
            {
                return false;
            }
            return true;
        }
    }
}