// <copyright file="RegistryPathRule.cs" project="SevenUpdate.Sdk">Robert Baker</copyright>
// <license href="http://www.gnu.org/licenses/gpl-3.0.txt" name="GNU General Public License 3" />

namespace SevenUpdate.Sdk.ValidationRules
{
    using System.Globalization;
    using System.Text.RegularExpressions;
    using System.Windows.Controls;

    using SevenUpdate.Sdk.Properties;

    /// <summary>Validates a value and determines if the value is a registry path.</summary>
    public class RegistryPathRule : ValidationRule
    {
        /// <summary>A regex to detect a registry root key.</summary>
        private const string RegistryPattern =
            @"^HKLM\\|^HKEY_CLASSES_ROOT\\|^HKEY_CURRENT_USER\\|^HKEY_LOCAL_MACHINE\\|^HKEY_USERS\\|^HKU\\|^HKCR\\";

        /// <summary>When overridden in a derived class, performs validation checks on a value.</summary>
        /// <param name="value">The value from the binding target to check.</param>
        /// <param name="cultureInfo">The culture to use in this rule.</param>
        /// <returns>A <c>T:System.Windows.Controls.ValidationResult</c> object.</returns>
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            var input = value as string;
            if (string.IsNullOrWhiteSpace(input))
            {
                return new ValidationResult(false, Resources.FilePathInvalid);
            }

            return Regex.IsMatch(input, RegistryPattern, RegexOptions.IgnoreCase)
                       ? new ValidationResult(true, null) : new ValidationResult(false, Resources.RegistryKeyInvalid);
        }
    }
}