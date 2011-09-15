// ***********************************************************************
// <copyright file="SuiLocationRule.cs" project="SevenUpdate.Sdk" assembly="SevenUpdate.Sdk" solution="SevenUpdate" company="Seven Software">
//     Copyright (c) Seven Software. All rights reserved.
// </copyright>
// <author username="sevenalive">Robert Baker</author>
// <license href="http://www.gnu.org/licenses/gpl-3.0.txt" name="GNU General Public License 3">
//  This file is part of Seven Update.
//    Seven Update is free software: you can redistribute it and/or modify it under the terms of the GNU General Public
//    License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any
//    later version. Seven Update is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without
//    even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details. You should have received a copy of the GNU General Public License
//    along with Seven Update.  If not, see http://www.gnu.org/licenses/.
// </license>
// ***********************************************************************

namespace SevenUpdate.Sdk.ValidationRules
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.IO;
    using System.Windows.Controls;

    using SevenUpdate.Sdk.Properties;

    /// <summary>Validates a value and determines if the value is a Sui location.</summary>
    [SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", 
        Justification = "ValidationRule")]
    public class SuiLocationRule : ValidationRule
    {
        #region Public Methods

        /// <summary>When overridden in a derived class, performs validation checks on a value.</summary>
        /// <param name = "value">The value from the binding target to check.</param>
        /// <param name = "cultureInfo">The culture to use in this rule.</param>
        /// <returns>A <c>T:System.Windows.Controls.ValidationResult</c> object.</returns>
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            var input = value as string;

            if (string.IsNullOrWhiteSpace(input))
            {
                return new ValidationResult(false, Resources.FilePathInvalid);
            }

            var result = Uri.IsWellFormedUriString(input, UriKind.RelativeOrAbsolute);

            if (!result)
            {
                if (!Utilities.IsValidPath(input))
                {
                    return new ValidationResult(false, Resources.FilePathInvalid);
                }
            }

            var fileName = Path.GetFileName(input);

            if (fileName == null || string.IsNullOrWhiteSpace(input))
            {
                return new ValidationResult(false, Resources.FilePathInvalid);
            }

            if (fileName.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0 || !input.EndsWith(@".sui", true, cultureInfo))
            {
                return new ValidationResult(false, Resources.FilePathInvalid);
            }

            return new ValidationResult(true, null);
        }

        #endregion
    }
}
