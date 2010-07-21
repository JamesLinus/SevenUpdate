//Copyright (c) Microsoft Corporation.  All rights reserved.

#region

using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;

#endregion

namespace Microsoft.Windows.Shell
{
    /// <summary>
    ///   A file in the Shell Namespace
    /// </summary>
    public class ShellFile : ShellObjectNode, IDisposable
    {
        #region Internal Constructor

        [SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.String.Format(System.String,System.Object)")]
        internal ShellFile(string path)
        {
            // Get the absolute path
            string absPath = ShellHelper.GetAbsolutePath(path);

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
        public virtual string Path { get { return ParsingName; } }

        #endregion
    }
}