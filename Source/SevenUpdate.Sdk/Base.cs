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
        internal static SevenUpdate.Base.Sui Sui { get; set; }

        /// <summary>
        /// The application information of the project
        /// </summary>
        internal static SevenUpdate.Base.Sua Sua { get; set; }

        /// <summary>
        /// The current update being edited
        /// </summary>
        internal static SevenUpdate.Base.Update Update { get; set; }
    }

}
