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
    /// A folder in the Shell Namespace
    /// </summary>
    [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", 
        Justification = "This will complicate the class hierarchy and naming convention used in the Shell area")]
    public class ShellFileSystemFolder : ShellFolder
    {
        #region Constructors and Destructors

        /// <summary>
        /// </summary>
        internal ShellFileSystemFolder()
        {
        }

        /// <summary>
        /// </summary>
        /// <param name="shellItem">
        /// </param>
        internal ShellFileSystemFolder(IShellItem2 shellItem)
        {
            this.nativeShellItem = shellItem;
        }

        #endregion

        #region Properties

        /// <summary>
        ///   The path for this Folder
        /// </summary>
        public virtual string Path
        {
            get
            {
                return this.ParsingName;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructs a new ShellFileSystemFolder object given a folder path
        /// </summary>
        /// <param name="path">
        /// The folder path
        /// </param>
        /// <remarks>
        /// ShellFileSystemFolder created from the given folder path.
        /// </remarks>
        [SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.String.Format(System.String,System.Object)", 
            Justification = "We are not currently handling globalization or localization")]
        public static ShellFileSystemFolder FromFolderPath(string path)
        {
            // Get the absolute path
            var absPath = ShellHelper.GetAbsolutePath(path);

            // Make sure this is valid
            if (!Directory.Exists(absPath))
            {
                throw new DirectoryNotFoundException(string.Format(CultureInfo.CurrentCulture, "The given path does not exist ({0})", path));
            }

            var folder = new ShellFileSystemFolder { ParsingName = absPath };
            return folder;
        }

        #endregion
    }
}