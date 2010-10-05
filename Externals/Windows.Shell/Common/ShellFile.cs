//***********************************************************************
// Assembly         : Windows.Shell
// Author           : sevenalive
// Created          : 09-17-2010
// Last Modified By : sevenalive
// Last Modified On : 10-05-2010
// Description      : 
// Copyright        : (c) Seven Software. All rights reserved.
//***********************************************************************

namespace Microsoft.Windows.Shell
{
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.IO;

    /// <summary>
    /// A file in the Shell Namespace
    /// </summary>
    public sealed class ShellFile : ShellObjectNode
    {
        #region Constructors and Destructors

        /// <summary>
        /// </summary>
        /// <param name="path">
        /// </param>
        /// <exception cref="FileNotFoundException">
        /// </exception>
        [SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.String.Format(System.String,System.Object)")]
        internal ShellFile(string path)
        {
            // Get the absolute path
            var absPath = ShellHelper.GetAbsolutePath(path);

            // Make sure this is valid
            if (!File.Exists(absPath))
            {
                throw new FileNotFoundException(string.Format(CultureInfo.CurrentCulture, "The given path does not exist ({0})", path));
            }

            this.ParsingName = absPath;
        }

        /// <summary>
        /// </summary>
        /// <param name="shellItem">
        /// </param>
        internal ShellFile(IShellItem2 shellItem)
        {
            this.nativeShellItem = shellItem;
        }

        #endregion

        #region Properties

        /// <summary>
        ///   The path for this file
        /// </summary>
        public string Path
        {
            get
            {
                return this.ParsingName;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructs a new ShellFile object given a file path
        /// </summary>
        /// <param name="path">
        /// The file or folder path
        /// </param>
        /// <returns>
        /// ShellFile object created using given file path.
        /// </returns>
        [SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.String.Format(System.String,System.Object)", 
            Justification = "We are not currently handling globalization or localization")]
        public static ShellFile FromFilePath(string path)
        {
            return new ShellFile(path);
        }

        #endregion
    }
}