using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SevenUpdate.Sdk
{
    internal static class Base
    {
        /// <summary>
        /// The update information of the project
        /// </summary>
        internal static SevenUpdate.Sui Sui { get; set; }

        /// <summary>
        /// The application information of the project
        /// </summary>
        internal static SevenUpdate.Sua Sua { get; set; }

        /// <summary>
        /// The current update being edited
        /// </summary>
        internal static SevenUpdate.Update Update { get; set; }

        /// <summary>
        /// Checks to see if a Url is valid
        /// </summary>
        /// <param name="url">A url to check</param>
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
