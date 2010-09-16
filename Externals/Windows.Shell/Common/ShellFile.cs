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
    ///   A file in the Shell Namespace
    /// </summary>
    public sealed class ShellFile : ShellObjectNode
    {
        #region Internal Constructor

        [SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.String.Format(System.String,System.Object)")]
        internal ShellFile(string path)
        {
            // Get the absolute path
            var absPath = ShellHelper.GetAbsolutePath(path);

            // Make sure this is valid
            if (!File.Exists(absPath))
                throw new FileNotFoundException(string.Format("The given path does not exist ({0})", path));

            ParsingName = absPath;
        }

        internal ShellFile(IShellItem2 shellItem)
        {
            nativeShellItem = shellItem;
        }

        #endregion

        #region Public Methods

        /// <summary>
        ///   Constructs a new ShellFile object given a file path
        /// </summary>
        /// <param name = "path">The file or folder path</param>
        /// <returns>ShellFile object created using given file path.</returns>
        [SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.String.Format(System.String,System.Object)",
            Justification = "We are not currently handling globalization or localization")]
        public static ShellFile FromFilePath(string path)
        {
            return new ShellFile(path);
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///   The path for this file
        /// </summary>
        public string Path { get { return ParsingName; } }

        #endregion
    }
}