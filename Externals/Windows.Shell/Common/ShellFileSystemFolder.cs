#region GNU Public License Version 3

// Copyright 2007-2010 Robert Baker, Seven Software.
// This file is part of Seven Update.
//   
//      Seven Update is free software: you can redistribute it and/or modify
//      it under the terms of the GNU General Public License as published by
//      the Free Software Foundation, either version 3 of the License, or
//      (at your option) any later version.
//  
//      Seven Update is distributed in the hope that it will be useful,
//      but WITHOUT ANY WARRANTY; without even the implied warranty of
//      MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//      GNU General Public License for more details.
//   
//      You should have received a copy of the GNU General Public License
//      along with Seven Update.  If not, see <http://www.gnu.org/licenses/>.

#endregion

#region

using System.Diagnostics.CodeAnalysis;
using System.IO;

#endregion

namespace Microsoft.Windows.Shell
{
    /// <summary>
    ///   A folder in the Shell Namespace
    /// </summary>
    [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "This will complicate the class hierarchy and naming convention used in the Shell area")]
    public class ShellFileSystemFolder : ShellFolder
    {
        #region Internal Constructor

        internal ShellFileSystemFolder()
        {
        }

        internal ShellFileSystemFolder(IShellItem2 shellItem)
        {
            nativeShellItem = shellItem;
        }

        #endregion

        #region Public Methods

        /// <summary>
        ///   Constructs a new ShellFileSystemFolder object given a folder path
        /// </summary>
        /// <param name = "path">The folder path</param>
        /// <remarks>
        ///   ShellFileSystemFolder created from the given folder path.
        /// </remarks>
        [SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.String.Format(System.String,System.Object)",
            Justification = "We are not currently handling globalization or localization")]
        public static ShellFileSystemFolder FromFolderPath(string path)
        {
            // Get the absolute path
            var absPath = ShellHelper.GetAbsolutePath(path);

            // Make sure this is valid
            if (!Directory.Exists(absPath))
                throw new DirectoryNotFoundException(string.Format("The given path does not exist ({0})", path));

            var folder = new ShellFileSystemFolder {ParsingName = absPath};
            return folder;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///   The path for this Folder
        /// </summary>
        public virtual string Path { get { return ParsingName; } }

        #endregion
    }
}