// ***********************************************************************
// Assembly         : System.Windows
// Author           : Robert Baker (sevenalive)
// Last Modified By : Robert Baker (sevenalive)
// Last Modified On : 10-06-2010
// Copyright        : (c) Microsoft Corporation. All rights reserved.
// ***********************************************************************
namespace System.Windows
{
    using System.Runtime.InteropServices;

    /// <summary>
    /// The Shell link class
    /// </summary>
    [ComImport]
    [Guid("00021401-0000-0000-C000-000000000046")]
    public class ShellLink
    {
        #region Properties

        /// <summary>
        ///   Gets or sets the shortcut command line arguments
        /// </summary>
        public extern string Arguments { get; set; }

        /// <summary>
        ///   Gets or sets the shortcut description
        /// </summary>
        public extern string Description { get; set; }

        /// <summary>
        ///   Gets or sets the path to the shortcut icon
        /// </summary>
        public extern string Icon { get; set; }

        /// <summary>
        ///   Gets or sets the full path to the shortcut lnk file
        /// </summary>
        public extern string Location { get; set; }

        /// <summary>
        ///   Gets or sets the filename for the shortcut
        /// </summary>
        public extern string Name { get; set; }

        /// <summary>
        ///   Gets or sets the shortcut target
        /// </summary>
        public extern string Target { get; set; }

        #endregion
    }
}