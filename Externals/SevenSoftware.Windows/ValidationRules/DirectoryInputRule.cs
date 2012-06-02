// <copyright file="DirectoryInputRule.cs" project="SevenSoftware.Windows">Robert Baker</copyright>
// <license href="http://www.gnu.org/licenses/gpl-3.0.txt" name="GNU General Public License 3" />

namespace SevenSoftware.Windows.ValidationRules
{
    using System.Globalization;
    using System.IO;
    using System.Text.RegularExpressions;
    using System.Windows.Controls;

    using SevenSoftware.Windows.Properties;

    /// <summary>Validates if the input is a directory.</summary>
    public class DirectoryInputRule : ValidationRule
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

            if (string.IsNullOrWhiteSpace(input))
            {
                return this.IsRequired
                           ? new ValidationResult(false, Resources.FilePathInvalid) : new ValidationResult(true, null);
            }

            var r = new Regex(@"^(([a-zA-Z]\:)|(\\))(\\{1}|((\\{1})[^\\]([^/:*?<>""|]*))+)$");
            if (!r.IsMatch(input))
            {
                return new ValidationResult(false, Resources.FilePathInvalid);
            }

            string directoryName = Path.GetDirectoryName(input);
            if (string.IsNullOrEmpty(directoryName))
            {
                return this.IsRequired
                           ? new ValidationResult(false, Resources.FilePathInvalid) : new ValidationResult(true, null);
            }

            return directoryName.IndexOfAny(Path.GetInvalidPathChars()) >= 0
                       ? new ValidationResult(false, Resources.FilePathInvalid) : new ValidationResult(true, null);
        }
    }
}