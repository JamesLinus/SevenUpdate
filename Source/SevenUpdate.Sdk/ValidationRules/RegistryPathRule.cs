// ***********************************************************************
// <copyright file="RegistryPathRule.cs"
//            project="SevenUpdate.Sdk"
//            assembly="SevenUpdate.Sdk"
//            solution="SevenUpdate"
//            company="Seven Software">
//     Copyright (c) Seven Software. All rights reserved.
// </copyright>
// <author username="sevenalive">Robert Baker</author>
// ***********************************************************************
namespace SevenUpdate.Sdk.ValidationRules
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Text.RegularExpressions;
    using System.Windows.Controls;

    using SevenUpdate.Sdk.Properties;

    /// <summary>
    /// Validates a value and determines if the value is a registry path
    /// </summary>
    [SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "ValidationRule")]
    public class RegistryPathRule : ValidationRule
    {
        #region Constants and Fields

        /// <summary>
        ///   A regex to detect a registry root key
        /// </summary>
        private const string RegistryPattern = @"^HKLM\\|^HKEY_CLASSES_ROOT\\|^HKEY_CURRENT_USER\\|^HKEY_LOCAL_MACHINE\\|^HKEY_USERS\\|^HKU\\|^HKCR\\";

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
            if (String.IsNullOrWhiteSpace(input) || input == null)
            {
                return new ValidationResult(false, Resources.FilePathInvalid);
            }

            return Regex.IsMatch(input, RegistryPattern, RegexOptions.IgnoreCase)
                       ? new ValidationResult(true, null)
                       : new ValidationResult(false, Resources.FilePathInvalid);
        }

        #endregion
    }
}