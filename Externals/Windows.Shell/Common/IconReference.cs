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
using System.Diagnostics.CodeAnalysis;

#endregion

namespace Microsoft.Windows.Shell
{
    /// <summary>
    ///   A refence to an icon resource
    /// </summary>
    public struct IconReference
    {
        #region Private members

        private static readonly char[] commaSeparator = new[] {','};
        private string moduleName;
        private string referencePath;
        private int resourceId;

        #endregion

        /// <summary>
        ///   Overloaded constructor takes in the module name and resource id for the icon reference.
        /// </summary>
        /// <param name = "moduleName">String specifying the name of an executable file, DLL, or icon file</param>
        /// <param name = "resourceId">Zero-based index of the icon</param>
        internal IconReference(string moduleName, int resourceId)
        {
            if (string.IsNullOrEmpty(moduleName))
                throw new ArgumentNullException("moduleName", "Module name cannot be null or empty string");

            this.moduleName = moduleName;
            this.resourceId = resourceId;
            referencePath = moduleName + "," + resourceId;
        }

        /// <summary>
        ///   Overloaded constructor takes in the module name and resource id separated by a comma.
        /// </summary>
        /// <param name = "refPath">Reference path for the icon consiting of the module name and resource id.</param>
        [SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.Int32.Parse(System.String)",
            Justification = "We are not currently handling globalization or localization")]
        internal IconReference(string refPath)
        {
            if (string.IsNullOrEmpty(refPath))
                throw new ArgumentNullException("refPath", "Reference path cannot be null or empty string");

            var refParams = refPath.Split(commaSeparator);

            if (refParams.Length != 2 || string.IsNullOrEmpty(refParams[0]) || string.IsNullOrEmpty(refParams[1]))
                throw new ArgumentException("Reference path is invalid.");

            moduleName = refParams[0];
            resourceId = int.Parse(refParams[1]);

            referencePath = refPath;
        }

        /// <summary>
        ///   String specifying the name of an executable file, DLL, or icon file
        /// </summary>
        public string ModuleName
        {
            get { return moduleName; }
            set
            {
                if (string.IsNullOrEmpty(value))
                    throw new ArgumentNullException("value", "Module name cannot be null or empty string");

                moduleName = value;
            }
        }

        /// <summary>
        ///   Zero-based index of the icon
        /// </summary>
        public int ResourceId { get { return resourceId; } set { resourceId = value; } }

        /// <summary>
        ///   Reference to a specific icon within a EXE, DLL or icon file.
        /// </summary>
        [SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.Int32.Parse(System.String)",
            Justification = "We are not currently handling globalization or localization")]
        public string ReferencePath
        {
            get { return referencePath; }
            set
            {
                if (string.IsNullOrEmpty(value))
                    throw new ArgumentNullException("value", "Reference path cannot be null or empty string");

                var refParams = value.Split(commaSeparator);

                if (refParams.Length != 2 || string.IsNullOrEmpty(refParams[0]) || string.IsNullOrEmpty(refParams[1]))
                    throw new ArgumentException("Reference path is invalid.");

                ModuleName = refParams[0];
                ResourceId = int.Parse(refParams[1]);

                referencePath = value;
            }
        }
    }
}