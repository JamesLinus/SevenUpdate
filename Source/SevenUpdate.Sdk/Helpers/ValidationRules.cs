// ***********************************************************************
// Assembly         : SevenUpdate.Sdk
// Author           : Robert Baker (sevenalive)
// Last Modified By : Robert Baker (sevenalive)
// Last Modified On : 10-06-2010
// Copyright        : (c) Seven Software. All rights reserved.
// ***********************************************************************
namespace SevenUpdate.Sdk.Helpers
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.IO;
    using System.Text.RegularExpressions;
    using System.Windows.Controls;

    using SevenUpdate.Sdk.Properties;

    /// <summary>
    /// Validates a value and determines if the value is a registry path
    /// </summary>
    public class RegistryPathRule : ValidationRule
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
            if (String.IsNullOrWhiteSpace(input) || input == null)
            {
                return new ValidationResult(false, Resources.FilePathInvalid);
            }

            const string pattern = @"^HKLM\\|^HKEY_CLASSES_ROOT\\|^HKEY_CURRENT_USER\\|^HKEY_LOCAL_MACHINE\\|^HKEY_USERS\\|^HKU\\|^HKCR\\";

            return Regex.IsMatch(input, pattern, RegexOptions.IgnoreCase) ? new ValidationResult(true, null) : new ValidationResult(false, Resources.FilePathInvalid);
        }

        #endregion
    }

    /// <summary>
    /// Validates a value and determines if the value is a <see cref="LocaleString"/>
    /// </summary>
    [SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "ValidationRule")]
    public class LocaleStringRule : ValidationRule
    {
        #region Properties

        /// <summary>
        ///   Gets or sets the name of the Collection of locale strings to get
        /// </summary>
        /// <value>The name of the property.</value>
        internal string PropertyName { private get; set; }

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

            if (!String.IsNullOrWhiteSpace(input))
            {
                return new ValidationResult(true, null);
            }

            switch (this.PropertyName)
            {
                case "App.Name":

                    if (Core.AppInfo.Name == null)
                    {
                        return new ValidationResult(false, Resources.InputRequired);
                    }

                    for (var x = 0; x < Core.AppInfo.Name.Count; x++)
                    {
                        if (Core.AppInfo.Name[x].Lang != Utilities.Locale)
                        {
                            continue;
                        }

                        Core.AppInfo.Name.RemoveAt(x);
                        break;
                    }

                    break;
                case "App.Publisher":

                    if (Core.AppInfo.Publisher == null)
                    {
                        return new ValidationResult(false, Resources.InputRequired);
                    }

                    for (var x = 0; x < Core.AppInfo.Publisher.Count; x++)
                    {
                        if (Core.AppInfo.Publisher[x].Lang != Utilities.Locale)
                        {
                            continue;
                        }

                        Core.AppInfo.Publisher.RemoveAt(x);
                        break;
                    }

                    break;
                case "App.Description":

                    if (Core.AppInfo.Description == null)
                    {
                        return new ValidationResult(false, Resources.InputRequired);
                    }

                    for (var x = 0; x < Core.AppInfo.Description.Count; x++)
                    {
                        if (Core.AppInfo.Description[x].Lang != Utilities.Locale)
                        {
                            continue;
                        }

                        Core.AppInfo.Description.RemoveAt(x);
                        break;
                    }

                    break;
                case "Update.Description":

                    if (Core.UpdateInfo.Description == null)
                    {
                        return new ValidationResult(false, Resources.InputRequired);
                    }

                    for (var x = 0; x < Core.UpdateInfo.Description.Count; x++)
                    {
                        if (Core.UpdateInfo.Description[x].Lang != Utilities.Locale)
                        {
                            continue;
                        }

                        Core.UpdateInfo.Description.RemoveAt(x);
                        break;
                    }

                    break;
                case "Update.Name":

                    if (Core.UpdateInfo.Name == null)
                    {
                        return new ValidationResult(false, Resources.InputRequired);
                    }

                    for (var x = 0; x < Core.UpdateInfo.Name.Count; x++)
                    {
                        if (Core.UpdateInfo.Name[x].Lang != Utilities.Locale)
                        {
                            continue;
                        }

                        Core.UpdateInfo.Name.RemoveAt(x);
                        break;
                    }

                    break;
            }

            return new ValidationResult(false, Resources.InputRequired);
        }

        #endregion
    }

    /// <summary>
    /// Validates a value and determines if the value is a Sui location
    /// </summary>
    public class SuiLocationRule : ValidationRule
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

            if (String.IsNullOrWhiteSpace(input) || input == null)
            {
                return new ValidationResult(false, Resources.FilePathInvalid);
            }

            try
            {
                new Uri(input);
            }
            catch (Exception)
            {
                return new ValidationResult(false, Resources.FilePathInvalid);
            }

            var fileName = Path.GetFileName(input);

            if (fileName == null || String.IsNullOrWhiteSpace(input))
            {
                return new ValidationResult(false, Resources.FilePathInvalid);
            }

            if (fileName.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0 || !input.EndsWith(".sui", true, cultureInfo))
            {
                return new ValidationResult(false, Resources.FilePathInvalid);
            }

            return new ValidationResult(true, null);
        }

        #endregion
    }

    /// <summary>
    /// Validates a value and determines if the value is a application location
    /// </summary>
    internal class AppDirectoryRule : ValidationRule
    {
        #region Properties

        /// <summary>
        ///   Gets or sets a value indicating whether this instance is registry path.
        /// </summary>
        /// <value>
        ///   <see langword = "true" /> if this instance is registry path; otherwise, <see langword = "false" />.
        /// </value>
        internal bool IsRegistryPath { private get; set; }

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
                return new ValidationResult(false, Resources.FilePathInvalid);
            }

            const string pattern = @"^HKLM\\|^HKEY_CLASSES_ROOT\\|^HKEY_CURRENT_USER\\|^HKEY_LOCAL_MACHINE\\|^HKEY_USERS\\|^HKU\\|^HKCR\\";
            if (this.IsRegistryPath)
            {
                return Regex.IsMatch(input, pattern, RegexOptions.IgnoreCase) ? new ValidationResult(true, null) : new ValidationResult(false, Resources.FilePathInvalid);
            }

            try
            {
                input = Core.AppInfo.Directory == null
                            ? Utilities.ConvertPath(input, true, Core.AppInfo.Is64Bit)
                            : Utilities.ConvertPath(input, Core.AppInfo.Directory, Core.AppInfo.ValueName, Core.AppInfo.Is64Bit);
                new Uri(input);
            }
            catch (Exception)
            {
                return Regex.IsMatch(input, pattern, RegexOptions.IgnoreCase) ? new ValidationResult(true, null) : new ValidationResult(false, Resources.FilePathInvalid);
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