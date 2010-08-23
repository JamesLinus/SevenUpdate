using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SevenUpdate.Sdk
{
    class Base
    {
        /// <summary>
        /// The update information of the project
        /// </summary>
        internal SevenUpdate.Base.Sui Sui { get; set; }

        /// <summary>
        /// The application information of the project
        /// </summary>
        internal SevenUpdate.Base.Sua Sua { get; set; }
    }

}
