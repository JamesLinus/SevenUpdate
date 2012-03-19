// <copyright file="RequiredInputRule.cs" project="SevenSoftware.Windows">Robert Baker</copyright>
// <license href="http://www.gnu.org/licenses/gpl-3.0.txt" name="GNU General Public License 3" />

namespace SevenSoftware.Windows.ValidationRules
{
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Windows.Controls;

    using SevenSoftware.Windows.Properties;

    /// <summary>The required input rule.</summary>
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1606:ElementDocumentationMustHaveSummaryText", 
        Justification = "Validation Rule")]
    public class RequiredInputRule : ValidationRule
    {
        /// <summary>When overridden in a derived class, performs validation checks on a value.</summary>
        /// <param name="value">The value from the binding target to check.</param>
        /// <param name="cultureInfo">The culture to use in this rule.</param>
        /// <returns>A <c>T:System.Windows.Controls.ValidationResult</c> object.</returns>
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            var input = value as string;

            return string.IsNullOrWhiteSpace(input)
                       ? new ValidationResult(false, Resources.InputRequired) : new ValidationResult(true, null);
        }
    }
}