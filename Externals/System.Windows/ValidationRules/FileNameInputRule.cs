// ***********************************************************************
// <copyright file="FileNameInputRule.cs" project="System.Windows" assembly="System.Windows" solution="SevenUpdate" company="Seven Software">
//     Copyright (c) Seven Software. All rights reserved.
// </copyright>
// <author username="sevenalive">Robert Baker</author>
// <license href="http://www.gnu.org/licenses/gpl-3.0.txt" name="GNU General Public License 3">
//  This file is part of Seven Update.
//    Seven Update is free software: you can redistribute it and/or modify it under the terms of the GNU General Public
//    License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any
//    later version. Seven Update is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without
//    even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details. You should have received a copy of the GNU General Public License
//    along with Seven Update.  If not, see http://www.gnu.org/licenses/.
// </license>
// ***********************************************************************

namespace System.Windows.ValidationRules
{
    using Controls;

    using Globalization;

    using IO;

    using Properties;

    using Text.RegularExpressions;

    /// <summary>
    ///   Validates if the input is a filename.
    /// </summary>
    public class FileNameInputRule : ValidationRule
    {
        #region Properties

        /// <summary>
        ///   Gets or sets a value indicating whether the filename is required to pass validation.
        /// </summary>
        /// <value><c>True</c> if the filename is required; otherwise, <c>False</c>.</value>
        public bool IsRequired { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        ///   When overridden in a derived class, performs validation checks on a value.
        /// </summary>
        /// <param name="value">
        ///   The value from the binding target to check.
        /// </param>
        /// <param name="cultureInfo">
        ///   The culture to use in this rule.
        /// </param>
        /// <returns>
        ///   A <c>T:System.Windows.Controls.ValidationResult</c> object.
        /// </returns>
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            var input = value as string;

            if (string.IsNullOrWhiteSpace(input))
            {
                return this.IsRequired
                           ? new ValidationResult(false, Resources.FilePathInvalid)
                           : new ValidationResult(true, null);
            }

            var r = new Regex(@"^(([a-zA-Z]\:)|(\\))(\\{1}|((\\{1})[^\\]([^/:*?<>""|]*))+)$");
            if (!r.IsMatch(input))
            {
                return new ValidationResult(false, Resources.FilePathInvalid);
            }

            var fileName = Path.GetFileName(input);

            if (string.IsNullOrEmpty(fileName))
            {
                return this.IsRequired
                           ? new ValidationResult(false, Resources.FilePathInvalid)
                           : new ValidationResult(true, null);
            }

            var directoryName = Path.GetDirectoryName(input);
            if (string.IsNullOrEmpty(directoryName))
            {
                return this.IsRequired
                           ? new ValidationResult(false, Resources.FilePathInvalid)
                           : new ValidationResult(true, null);
            }

            if (fileName.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0 ||
                directoryName.IndexOfAny(Path.GetInvalidPathChars()) >= 0)
            {
                return new ValidationResult(false, Resources.FilePathInvalid);
            }

            return new ValidationResult(true, null);
        }

        #endregion
    }
}