// Copyright 2007-2010 Robert Baker, Seven Software.
// This file is part of Seven Update.
// Seven Update is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
// Seven Update is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.
// You should have received a copy of the GNU General Public License along with Seven Update.  If not, see <http://www.gnu.org/licenses/>.

namespace Microsoft.Windows.Controls
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.IO;
    using System.Windows.Controls;

    using Microsoft.Windows.Properties;

    /// <summary>
    /// </summary>
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1606:ElementDocumentationMustHaveSummaryText", Justification = "Validation Rule")]
    public class RequiredInputRule : ValidationRule
    {
        #region Public Methods

        /// <summary>
        /// When overridden in a derived class, performs validation checks on a value.
        /// </summary>
        /// <param name="value">
        /// The value from the binding target to check.
        /// </param>
        /// <param name="cultureInfo">
        /// The culture to use in this rule.
        /// </param>
        /// <returns>
        /// A <see cref="T:System.Windows.Controls.ValidationResult"/> object.
        /// </returns>
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            var input = value as string;

            return String.IsNullOrWhiteSpace(input) ? new ValidationResult(false, Resources.InputRequired) : new ValidationResult(true, null);
        }

        #endregion
    }

    /// <summary>
    /// Validates if the input is a url
    /// </summary>
    [SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Validation Rule")]
    public class UrlInputRule : ValidationRule
    {
        #region Properties

        /// <summary>
        ///   Gets or sets a value indicating whether this instance is required.
        /// </summary>
        /// <value>
        ///   <see langword = "true" /> if this instance is required; otherwise, <see langword = "false" />.
        /// </value>
        public bool IsRequired { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// When overridden in a derived class, performs validation checks on a value.
        /// </summary>
        /// <param name="value">
        /// The value from the binding target to check.
        /// </param>
        /// <param name="cultureInfo">
        /// The culture to use in this rule.
        /// </param>
        /// <returns>
        /// A <see cref="T:System.Windows.Controls.ValidationResult"/> object.
        /// </returns>
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            try
            {
                var url = value as string;

                if (String.IsNullOrWhiteSpace(url))
                {
                    return this.IsRequired ? new ValidationResult(false, Resources.FilePathInvalid) : new ValidationResult(true, null);
                }

                new Uri(value.ToString());
                return new ValidationResult(true, null);
            }
            catch
            {
                return new ValidationResult(false, Resources.UrilInvalid);
            }
        }

        #endregion
    }

    /// <summary>
    /// Validates if the input is a filename
    /// </summary>
    public class FileNameInputRule : ValidationRule
    {
        #region Properties

        /// <summary>
        ///   Gets or sets a value indicating whether the filename is required to pass validation
        /// </summary>
        /// <value>
        ///   <see langword = "true" /> if the filename is required; otherwise, <see langword = "false" />.
        /// </value>
        public bool IsRequired { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// When overridden in a derived class, performs validation checks on a value.
        /// </summary>
        /// <param name="value">
        /// The value from the binding target to check.
        /// </param>
        /// <param name="cultureInfo">
        /// The culture to use in this rule.
        /// </param>
        /// <returns>
        /// A <see cref="T:System.Windows.Controls.ValidationResult"/> object.
        /// </returns>
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            var input = value as string;

            if (String.IsNullOrWhiteSpace(input))
            {
                return this.IsRequired ? new ValidationResult(false, Resources.FilePathInvalid) : new ValidationResult(true, null);
            }

            var fileName = Path.GetFileName(input);

            if (String.IsNullOrEmpty(fileName))
            {
                return this.IsRequired ? new ValidationResult(false, Resources.FilePathInvalid) : new ValidationResult(true, null);
            }

            var directoryName = Path.GetDirectoryName(input);
            if (string.IsNullOrEmpty(directoryName))
            {
                return this.IsRequired ? new ValidationResult(false, Resources.FilePathInvalid) : new ValidationResult(true, null);
            }

            if (fileName.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0 || directoryName.IndexOfAny(Path.GetInvalidPathChars()) >= 0)
            {
                return new ValidationResult(false, Resources.FilePathInvalid);
            }

            return new ValidationResult(true, null);
        }

        #endregion
    }

    /// <summary>
    /// Validates if the input is a directory
    /// </summary>
    public class DirectoryInputRule : ValidationRule
    {
        #region Properties

        /// <summary>
        ///   Gets or sets a value indicating whether this instance is required.
        /// </summary>
        /// <value>
        ///   <see langword = "true" /> if this instance is required; otherwise, <see langword = "false" />.
        /// </value>
        public bool IsRequired { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// When overridden in a derived class, performs validation checks on a value.
        /// </summary>
        /// <param name="value">
        /// The value from the binding target to check.
        /// </param>
        /// <param name="cultureInfo">
        /// The culture to use in this rule.
        /// </param>
        /// <returns>
        /// A <see cref="T:System.Windows.Controls.ValidationResult"/> object.
        /// </returns>
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            var input = value as string;
            if (input == null)
            {
                return this.IsRequired ? new ValidationResult(false, Resources.FilePathInvalid) : new ValidationResult(true, null);
            }

            if (string.IsNullOrWhiteSpace(input))
            {
                return this.IsRequired ? new ValidationResult(false, Resources.FilePathInvalid) : new ValidationResult(true, null);
            }

            var directoryName = Path.GetDirectoryName(input);

            if (string.IsNullOrEmpty(directoryName))
            {
                return this.IsRequired ? new ValidationResult(false, Resources.FilePathInvalid) : new ValidationResult(true, null);
            }

            return directoryName.IndexOfAny(Path.GetInvalidPathChars()) >= 0 ? new ValidationResult(false, Resources.FilePathInvalid) : new ValidationResult(true, null);
        }

        #endregion
    }
}