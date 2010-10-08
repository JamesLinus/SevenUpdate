// ***********************************************************************
// <copyright file="UrlInputRule.cs"
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
        public bool IsRequired { private get; set; }

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
            catch (Exception)
            {
                return new ValidationResult(false, Resources.UrilInvalid);
            }
        }

        #endregion
    }
}