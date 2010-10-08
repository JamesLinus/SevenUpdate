// ***********************************************************************
// <copyright file="RequiredInputRule.cs"
//            project="System.Windows"
//            assembly="System.Windows"
//            solution="SevenUpdate"
//            company="Seven Software">
//     Copyright (c) Seven Software. All rights reserved.
// </copyright>
// <author username="sevenalive">Robert Baker</author>
// ***********************************************************************
namespace System.Windows.ValidationRules
{
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Windows.Controls;
    using System.Windows.Properties;

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
}