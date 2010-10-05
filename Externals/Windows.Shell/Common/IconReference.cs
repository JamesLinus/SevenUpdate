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
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;

    /// <summary>
    /// A refence to an icon resource
    /// </summary>
    public struct IconReference
    {
        #region Constants and Fields

        /// <summary>
        /// </summary>
        private static readonly char[] commaSeparator = new[] { ',' };

        /// <summary>
        /// </summary>
        private string moduleName;

        /// <summary>
        /// </summary>
        private string referencePath;

        /// <summary>
        /// </summary>
        private int resourceId;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Overloaded constructor takes in the module name and resource id for the icon reference.
        /// </summary>
        /// <param name="moduleName">
        /// String specifying the name of an executable file, DLL, or icon file
        /// </param>
        /// <param name="resourceId">
        /// Zero-based index of the icon
        /// </param>
        internal IconReference(string moduleName, int resourceId)
        {
            if (string.IsNullOrEmpty(moduleName))
            {
                throw new ArgumentNullException("moduleName", "Module name cannot be null or empty string");
            }

            this.moduleName = moduleName;
            this.resourceId = resourceId;
            this.referencePath = moduleName + "," + resourceId;
        }

        /// <summary>
        /// Overloaded constructor takes in the module name and resource id separated by a comma.
        /// </summary>
        /// <param name="refPath">
        /// Reference path for the icon consiting of the module name and resource id.
        /// </param>
        [SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.Int32.Parse(System.String)", 
            Justification = "We are not currently handling globalization or localization")]
        internal IconReference(string refPath)
        {
            if (string.IsNullOrEmpty(refPath))
            {
                throw new ArgumentNullException("refPath", "Reference path cannot be null or empty string");
            }

            var refParams = refPath.Split(commaSeparator);

            if (refParams.Length != 2 || string.IsNullOrEmpty(refParams[0]) || string.IsNullOrEmpty(refParams[1]))
            {
                throw new ArgumentException("Reference path is invalid.");
            }

            this.moduleName = refParams[0];
            this.resourceId = int.Parse(refParams[1], CultureInfo.CurrentCulture);

            this.referencePath = refPath;
        }

        #endregion

        #region Properties

        /// <summary>
        ///   String specifying the name of an executable file, DLL, or icon file
        /// </summary>
        public string ModuleName
        {
            get
            {
                return this.moduleName;
            }

            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new ArgumentNullException("value", "Module name cannot be null or empty string");
                }

                this.moduleName = value;
            }
        }

        /// <summary>
        ///   Reference to a specific icon within a EXE, DLL or icon file.
        /// </summary>
        [SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.Int32.Parse(System.String)", 
            Justification = "We are not currently handling globalization or localization")]
        public string ReferencePath
        {
            get
            {
                return this.referencePath;
            }

            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new ArgumentNullException("value", "Reference path cannot be null or empty string");
                }

                var refParams = value.Split(commaSeparator);

                if (refParams.Length != 2 || string.IsNullOrEmpty(refParams[0]) || string.IsNullOrEmpty(refParams[1]))
                {
                    throw new ArgumentException("Reference path is invalid.");
                }

                this.ModuleName = refParams[0];
                this.ResourceId = int.Parse(refParams[1], CultureInfo.CurrentCulture);

                this.referencePath = value;
            }
        }

        /// <summary>
        ///   Zero-based index of the icon
        /// </summary>
        public int ResourceId
        {
            get
            {
                return this.resourceId;
            }

            set
            {
                this.resourceId = value;
            }
        }

        #endregion

        #region Operators

        /// <summary>
        /// </summary>
        /// <param name="x">
        /// </param>
        /// <param name="y">
        /// </param>
        /// <returns>
        /// </returns>
        /// <exception cref="NotImplementedException">
        /// </exception>
        public static bool operator ==(IconReference x, IconReference y)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// </summary>
        /// <param name="x">
        /// </param>
        /// <param name="y">
        /// </param>
        /// <returns>
        /// </returns>
        /// <exception cref="NotImplementedException">
        /// </exception>
        public static bool operator !=(IconReference x, IconReference y)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// </summary>
        /// <param name="obj">
        /// </param>
        /// <returns>
        /// </returns>
        /// <exception cref="NotImplementedException">
        /// </exception>
        public override bool Equals(object obj)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// </summary>
        /// <returns>
        /// </returns>
        /// <exception cref="NotImplementedException">
        /// </exception>
        public override int GetHashCode()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}