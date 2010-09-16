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

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Microsoft.Windows.Shell;

#endregion

namespace Microsoft.Windows.Dialogs
{
    /// <summary>
    ///   Stores the file extensions used when filtering files in File Open and File Save dialogs.
    /// </summary>
    public class CommonFileDialogFilter
    {
        // We'll keep a parsed list of separate 
        // extensions and rebuild as needed.

        private string rawDisplayName;
        private bool showExtensions = true;

        /// <summary>
        ///   Creates a new instance of this class.
        /// </summary>
        public CommonFileDialogFilter()
        {
            Extensions = new Collection<string>();
        }

        /// <summary>
        ///   Creates a new instance of this class with the specified display name and 
        ///   file extension list.
        /// </summary>
        /// <param name = "rawDisplayName">The name of this filter.</param>
        /// <param name = "extensionList">The list of extensions in 
        ///   this filter. See remarks.</param>
        /// <remarks>
        ///   The <paramref name = "extensionList" /> can use a semicolon(";") 
        ///   or comma (",") to separate extensions. Extensions can be prefaced 
        ///   with a period (".") or with the file wild card specifier "*.".
        /// </remarks>
        /// <permission cref = "System.ArgumentNullException">
        ///   The <paramref name = "extensionList" /> cannot be null or a 
        ///   zero-length string. 
        /// </permission>
        public CommonFileDialogFilter(string rawDisplayName, string extensionList) : this()
        {
            if (String.IsNullOrEmpty(extensionList))
                throw new ArgumentNullException("extensionList", "extensionList must be non-null.");

            this.rawDisplayName = rawDisplayName;

            // Parse string and create extension strings.
            // Format: "bat,cmd", or "bat;cmd", or "*.bat;*.cmd"
            // Can support leading "." or "*." - these will be stripped.
            var rawExtensions = extensionList.Split(',', ';');
            foreach (var extension in rawExtensions)
                Extensions.Add(NormalizeExtension(extension));
        }

        /// <summary>
        ///   Gets or sets the display name for this filter.
        /// </summary>
        /// <permission cref = "System.ArgumentNullException">
        ///   The value for this property cannot be set to null or a 
        ///   zero-length string. 
        /// </permission>
        [SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.String.Format(System.String,System.Object,System.Object)",
            Justification = "We are not currently handling globalization or localization")]
        public string DisplayName
        {
            get { return showExtensions ? String.Format("{0} ({1})", rawDisplayName, GetDisplayExtensionList(Extensions)) : rawDisplayName; }

            set
            {
                if (String.IsNullOrEmpty(value))
                    throw new ArgumentNullException("value", "DisplayName must be non-null.");
                rawDisplayName = value;
            }
        }

        /// <summary>
        ///   Gets a collection of the individual extensions 
        ///   described by this filter.
        /// </summary>
        public Collection<string> Extensions { get; private set; }

        /// <summary>
        ///   Gets or sets a value that controls whether the extensions are displayed.
        /// </summary>
        public bool ShowExtensions { get { return showExtensions; } set { showExtensions = value; } }

        private static string NormalizeExtension(string rawExtension)
        {
            rawExtension = rawExtension.Trim();
            rawExtension = rawExtension.Replace("*.", null);
            rawExtension = rawExtension.Replace(".", null);
            return rawExtension;
        }

        private static string GetDisplayExtensionList(IEnumerable<string> extensions)
        {
            var extensionList = new StringBuilder();
            foreach (var extension in extensions)
            {
                if (extensionList.Length > 0)
                    extensionList.Append(", ");
                extensionList.Append("*.");
                extensionList.Append(extension);
            }

            return extensionList.ToString();
        }

        /// <summary>
        ///   Internal helper that generates a single filter 
        ///   specification for this filter, used by the COM API.
        /// </summary>
        /// <returns>Filter specification for this filter</returns>
        internal ShellNativeMethods.COMDLG_FILTERSPEC GetFilterSpec()
        {
            var filterList = new StringBuilder();
            foreach (var extension in Extensions)
            {
                if (filterList.Length > 0)
                    filterList.Append(";");

                filterList.Append("*.");
                filterList.Append(extension);
            }
            return new ShellNativeMethods.COMDLG_FILTERSPEC(DisplayName, filterList.ToString());
        }

        /// <summary>
        ///   Returns a string representation for this filter that includes
        ///   the display name and the list of extensions.
        /// </summary>
        /// <returns>A <see cref = "System.String" />.</returns>
        [SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.String.Format(System.String,System.Object,System.Object)",
            Justification = "We are not currently handling globalization or localization")]
        public override string ToString()
        {
            return String.Format("{0} ({1})", rawDisplayName, GetDisplayExtensionList(Extensions));
        }
    }
}