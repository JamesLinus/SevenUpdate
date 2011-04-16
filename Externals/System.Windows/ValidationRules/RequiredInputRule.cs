// ***********************************************************************
// <copyright file="RequiredInputRule.cs" project="System.Windows" assembly="System.Windows" solution="SevenUpdate" company="Seven Software">
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

namespace System.Windows.ValidationRules
{
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Windows.Controls;
    using System.Windows.Properties;

    /// <summary>The required input rule.</summary>
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1606:ElementDocumentationMustHaveSummaryText",
        Justification = "Validation Rule")]
    public class RequiredInputRule : ValidationRule
    {
        #region Public Methods

        /// <summary>When overridden in a derived class, performs validation checks on a value.</summary>
        /// <param name="value">The value from the binding target to check.</param>
        /// <param name="cultureInfo">The culture to use in this rule.</param>
        /// <returns>A <see cref="T:System.Windows.Controls.ValidationResult" /> object.</returns>
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            var input = value as string;

            return string.IsNullOrWhiteSpace(input) ? new ValidationResult(false, Resources.InputRequired) : new ValidationResult(true, null);
        }

        #endregion
    }
}