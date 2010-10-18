// ***********************************************************************
// <copyright file="LocaleStringRule.cs"
//            project="SevenUpdate.Sdk"
//            assembly="SevenUpdate.Sdk"
//            solution="SevenUpdate"
//            company="Seven Software">
//     Copyright (c) Seven Software. All rights reserved.
// </copyright>
// <author username="sevenalive">Robert Baker</author>
// <license href="http://www.gnu.org/licenses/gpl-3.0.txt" name="GNU General Public License 3">
//  This file is part of Seven Update.
//
//    Seven Update is free software: you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation, either version 3 of the License, or
//    (at your option) any later version.
//
//    Seven Update is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.
//
//    You should have received a copy of the GNU General Public License
//    along with Seven Update.  If not, see http://www.gnu.org/licenses/.
// </license>
// ***********************************************************************
namespace SevenUpdate.Sdk.ValidationRules
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Windows.Controls;

    using SevenUpdate.Sdk.Properties;

    /// <summary>Validates a value and determines if the value is a <see cref="LocaleString"/></summary>
    [SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "ValidationRule")]
    public class LocaleStringRule : ValidationRule
    {
        #region Properties

        /// <summary>Gets or sets the name of the Collection of locale strings to get</summary>
        /// <value>The name of the property.</value>
        internal string PropertyName { private get; set; }

        #endregion

        #region Public Methods

        /// <summary>When overridden in a derived class, performs validation checks on a value.</summary>
        /// <param name="value">The value from the binding target to check.</param>
        /// <param name="cultureInfo">The culture to use in this rule.</param>
        /// <returns>A <see cref="T:System.Windows.Controls.ValidationResult"/> object.</returns>
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
}