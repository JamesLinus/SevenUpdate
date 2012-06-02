// <copyright file="AppDirectoryRule.cs" project="SevenUpdate.Sdk">Robert Baker</copyright>
// <license href="http://www.gnu.org/licenses/gpl-3.0.txt" name="GNU General Public License 3" />

namespace SevenUpdate.Sdk.ValidationRules
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Windows.Controls;

    using SevenUpdate.Sdk.Properties;

    /// <summary>Validates a value and determines if the value is a application location.</summary>
    internal sealed class AppDirectoryRule : ValidationRule
    {
        /// <summary>When overridden in a derived class, performs validation checks on a value.</summary>
        /// <param name="value">The value from the binding target to check.</param>
        /// <param name="cultureInfo">The culture to use in this rule.</param>
        /// <returns>A <c>T:System.Windows.Controls.ValidationResult</c> object.</returns>
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            var input = value as string;
            if (input == null)
            {
                return new ValidationResult(false, Resources.FilePathInvalid);
            }

            input = Core.AppInfo.Directory == null
                        ? Utilities.ConvertPath(input, true, Core.AppInfo.Platform)
                        : Utilities.ExpandInstallLocation(
                            input, Core.AppInfo.Directory, Core.AppInfo.Platform, Core.AppInfo.ValueName);

            if (string.IsNullOrEmpty(input) || input.IndexOfAny(Path.GetInvalidPathChars()) >= 0)
            {
                return new ValidationResult(false, Resources.FilePathInvalid);
            }

            if (File.Exists(input) || Directory.Exists(Path.GetDirectoryName(input)))
            {
                return new ValidationResult(true, null);
            }

            if (Uri.IsWellFormedUriString(input, UriKind.Absolute))
            {
                return new ValidationResult(true, null);
            }

            return new ValidationResult(false, Resources.FilePathInvalid);
        }
    }
}