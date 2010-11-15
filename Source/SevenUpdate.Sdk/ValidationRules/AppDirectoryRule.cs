// ***********************************************************************
// <copyright file="AppDirectoryRule.cs"
//            project="SevenUpdate.Sdk"
//            assembly="SevenUpdate.Sdk"
//            solution="SevenUpdate"
//            company="Seven Software">
//     Copyright (c) Seven Software. All rights reserved.
// </copyright>
// <author username="sevenalive">Robert Baker</author>
// <license href="http://www.gnu.org/licenses/gpl-3.0.txt" name="GNU General Public License 3">
//  This file is part of Seven Update.
//    Seven Update is free software: you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation, either version 3 of the License, or
//    (at your option) any later version.
//    Seven Update is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.
//    You should have received a copy of the GNU General Public License
//    along with Seven Update.  If not, see http://www.gnu.org/licenses/.
// </license>
// ***********************************************************************
namespace SevenUpdate.Sdk.ValidationRules
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.IO;
    using System.Text.RegularExpressions;
    using System.Windows.Controls;

    using SevenUpdate.Sdk.Properties;

    /// <summary>Validates a value and determines if the value is a application location</summary>
    [SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "ValidationRule")]
    internal sealed class AppDirectoryRule : ValidationRule
    {
        #region Constants and Fields

        /// <summary>A regex to detect a registry root key</summary>
        private const string RegistryPattern = @"^HKLM\\|^HKEY_CLASSES_ROOT\\|^HKEY_CURRENT_USER\\|^HKEY_LOCAL_MACHINE\\|^HKEY_USERS\\|^HKU\\|^HKCR\\";

        #endregion

        #region Properties

        /// <summary>Gets or sets a value indicating whether this instance is registry path.</summary>
        /// <value><see langword = "true" /> if this instance is registry path; otherwise, <see langword = "false" />.</value>
        internal bool IsRegistryPath { private get; set; }

        #endregion

        #region Public Methods

        /// <summary>When overridden in a derived class, performs validation checks on a value.</summary>
        /// <param name="value">The value from the binding target to check.</param>
        /// <param name="cultureInfo">The culture to use in this rule.</param>
        /// <returns>A <see cref="T:System.Windows.Controls.ValidationResult"/> object.</returns>
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            var input = value as string;
            if (input == null)
            {
                return new ValidationResult(false, Resources.FilePathInvalid);
            }

            if (this.IsRegistryPath)
            {
                if (Regex.IsMatch(input, RegistryPattern, RegexOptions.IgnoreCase))
                {
                    return Utilities.CheckRegistryKey(input, Core.AppInfo.Is64Bit) ? new ValidationResult(true, null) : new ValidationResult(false, Resources.PathDoesNotExist);
                }
            }

            input = Core.AppInfo.Directory == null
                        ? Utilities.ConvertPath(input, true, Core.AppInfo.Is64Bit)
                        : Utilities.ConvertPath(input, Core.AppInfo.Directory, Core.AppInfo.Is64Bit, Core.AppInfo.ValueName);
            if (File.Exists(input) || Directory.Exists(input))
            {
                return new ValidationResult(true, null);
            }

            if (Uri.IsWellFormedUriString(input, UriKind.Absolute))
            {
                return new ValidationResult(true, null);
            }

            if (string.IsNullOrEmpty(input) || input.IndexOfAny(Path.GetInvalidPathChars()) >= 0)
            {
                return new ValidationResult(false, Resources.FilePathInvalid);
            }

            return new ValidationResult(true, null);
        }

        #endregion
    }
}