//***********************************************************************
// Assembly         : Windows.Shell
// Author           : sevenalive
// Created          : 09-17-2010
// Last Modified By : sevenalive
// Last Modified On : 10-05-2010
// Description      : 
// Copyright        : (c) Seven Software. All rights reserved.
//***********************************************************************

namespace Microsoft.Windows.Dialogs
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Text;

    using Microsoft.Windows.Shell;

    /// <summary>
    /// Stores the file extensions used when filtering files in File Open and File Save dialogs.
    /// </summary>
    public class CommonFileDialogFilter
    {
        // We'll keep a parsed list of separate 
        // extensions and rebuild as needed.
        #region Constants and Fields

        /// <summary>
        /// </summary>
        private string rawDisplayName;

        /// <summary>
        /// </summary>
        private bool showExtensions = true;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///   Creates a new instance of this class.
        /// </summary>
        public CommonFileDialogFilter()
        {
            this.Extensions = new Collection<string>();
        }

        /// <summary>
        /// Creates a new instance of this class with the specified display name and 
        ///   file extension list.
        /// </summary>
        /// <param name="rawDisplayName">
        /// The name of this filter.
        /// </param>
        /// <param name="extensionList">
        /// The list of extensions in 
        ///   this filter. See remarks.
        /// </param>
        /// <remarks>
        /// The <paramref name="extensionList"/> can use a semicolon(";") 
        ///   or comma (",") to separate extensions. Extensions can be prefaced 
        ///   with a period (".") or with the file wild card specifier "*.".
        /// </remarks>
        /// <permission cref="System.ArgumentNullException">
        /// The <paramref name="extensionList"/> cannot be null or a 
        ///   zero-length string. 
        /// </permission>
        public CommonFileDialogFilter(string rawDisplayName, string extensionList)
            : this()
        {
            if (String.IsNullOrEmpty(extensionList))
            {
                throw new ArgumentNullException("extensionList", "extensionList must be non-null.");
            }

            this.rawDisplayName = rawDisplayName;

            // Parse string and create extension strings.
            // Format: "bat,cmd", or "bat;cmd", or "*.bat;*.cmd"
            // Can support leading "." or "*." - these will be stripped.
            var rawExtensions = extensionList.Split(',', ';');
            foreach (var extension in rawExtensions)
            {
                this.Extensions.Add(NormalizeExtension(extension));
            }
        }

        #endregion

        #region Properties

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
            get
            {
                return this.showExtensions
                           ? String.Format(CultureInfo.CurrentCulture, "{0} ({1})", this.rawDisplayName, GetDisplayExtensionList(this.Extensions))
                           : this.rawDisplayName;
            }

            set
            {
                if (String.IsNullOrEmpty(value))
                {
                    throw new ArgumentNullException("value", "DisplayName must be non-null.");
                }

                this.rawDisplayName = value;
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
        public bool ShowExtensions
        {
            get
            {
                return this.showExtensions;
            }

            set
            {
                this.showExtensions = value;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Returns a string representation for this filter that includes
        ///   the display name and the list of extensions.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/>.
        /// </returns>
        [SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.String.Format(System.String,System.Object,System.Object)", 
            Justification = "We are not currently handling globalization or localization")]
        public override string ToString()
        {
            return String.Format(CultureInfo.CurrentCulture, "{0} ({1})", this.rawDisplayName, GetDisplayExtensionList(this.Extensions));
        }

        #endregion

        #region Methods

        /// <summary>
        /// Internal helper that generates a single filter 
        ///   specification for this filter, used by the COM API.
        /// </summary>
        /// <returns>
        /// Filter specification for this filter
        /// </returns>
        internal ShellNativeMethods.OmdlgFilterspec GetFilterSpec()
        {
            var filterList = new StringBuilder();
            foreach (var extension in this.Extensions)
            {
                if (filterList.Length > 0)
                {
                    filterList.Append(";");
                }

                filterList.Append("*.");
                filterList.Append(extension);
            }

            return new ShellNativeMethods.OmdlgFilterspec(this.DisplayName, filterList.ToString());
        }

        /// <summary>
        /// </summary>
        /// <param name="extensions">
        /// </param>
        /// <returns>
        /// </returns>
        private static string GetDisplayExtensionList(IEnumerable<string> extensions)
        {
            var extensionList = new StringBuilder();
            foreach (var extension in extensions)
            {
                if (extensionList.Length > 0)
                {
                    extensionList.Append(", ");
                }

                extensionList.Append("*.");
                extensionList.Append(extension);
            }

            return extensionList.ToString();
        }

        /// <summary>
        /// </summary>
        /// <param name="rawExtension">
        /// </param>
        /// <returns>
        /// </returns>
        private static string NormalizeExtension(string rawExtension)
        {
            rawExtension = rawExtension.Trim();
            rawExtension = rawExtension.Replace("*.", null);
            rawExtension = rawExtension.Replace(".", null);
            return rawExtension;
        }

        #endregion
    }
}