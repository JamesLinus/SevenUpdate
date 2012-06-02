// <copyright file="DownloadUrlRule.cs" project="SevenUpdate.Sdk">Robert Baker</copyright>
// <license href="http://www.gnu.org/licenses/gpl-3.0.txt" name="GNU General Public License 3" />

namespace SevenUpdate.Sdk.ValidationRules
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.IO;
    using System.Text.RegularExpressions;
    using System.Windows.Controls;

    using SevenSoftware.Windows.Properties;

    /// <summary>Validates if the input is a url.</summary>
    [SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", 
        Justification = "Validation Rule")]
    public class DownloadUrlRule : ValidationRule
    {
        /// <summary>Gets or sets a value indicating whether this instance is required.</summary>
        /// <value><c>True</c> if this instance is required; otherwise, <c>False</c>.</value>
        public bool IsRequired { get; set; }

        /// <summary>When overridden in a derived class, performs validation checks on a value.</summary>
        /// <param name="value">The value from the binding target to check.</param>
        /// <param name="cultureInfo">The culture to use in this rule.</param>
        /// <returns>A <c>T:System.Windows.Controls.ValidationResult</c> object.</returns>
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            var input = value as string;

            if (input == null)
            {
                return this.IsRequired
                           ? new ValidationResult(false, Resources.UrilInvalid) : new ValidationResult(true, null);
            }

            if (string.IsNullOrWhiteSpace(input))
            {
                return this.IsRequired
                           ? new ValidationResult(false, Resources.UrilInvalid) : new ValidationResult(true, null);
            }

            input = Utilities.ExpandDownloadUrl(input, Core.UpdateInfo.DownloadUrl, Core.AppInfo.Platform);

            if ((File.Exists(input) || Directory.Exists(input)) && input.Length > 3)
            {
                return new ValidationResult(true, null);
            }

            if (Uri.IsWellFormedUriString(input, UriKind.Absolute) && input.Length > 3)
            {
                return new ValidationResult(true, null);
            }

            var r = new Regex(@"^(([a-zA-Z]\:)|(\\))(\\{1}|((\\{1})[^\\]([^/:*?<>""|]*))+)$");
            return r.IsMatch(input)
                       ? new ValidationResult(true, null) : new ValidationResult(false, Resources.UrilInvalid);
        }
    }
}