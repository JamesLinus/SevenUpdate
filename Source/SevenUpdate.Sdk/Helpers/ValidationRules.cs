#region GNU Public License Version 3

// Copyright 2007-2010 Robert Baker, Seven Software.
// This file is part of Seven Update.
//   
//      Seven Update is free software: you can redistribute it and/or modify
//      it under the terms of the GNU General Public License as published by
//      the Free Software Foundation, either version 3 of the License, or
//      (at your option) any later version.
//  
//      Seven Update is distributed in the hope that it will be useful,
//      but WITHOUT ANY WARRANTY; without even the implied warranty of
//      MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//      GNU General Public License for more details.
//   
//      You should have received a copy of the GNU General Public License
//      along with Seven Update.  If not, see <http://www.gnu.org/licenses/>.

#endregion

#region

using System;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using SevenUpdate.Sdk.Properties;

#endregion

namespace SevenUpdate.Sdk.Helpers
{
    internal sealed class AppDirectoryRule : ValidationRule
    {
        internal bool IsRegistryPath { private get; set; }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            var input = value as string;
            if (input == null)
                return new ValidationResult(false, Resources.FilePathInvalid);

            if (string.IsNullOrEmpty(input) || input.IndexOfAny(Path.GetInvalidPathChars()) >= 0)
                return new ValidationResult(false, Resources.FilePathInvalid);

            const string pattern = @"^HKLM\\|^HKEY_CLASSES_ROOT\\|^HKEY_CURRENT_USER\\|^HKEY_LOCAL_MACHINE\\|^HKEY_USERS\\|^HKU\\|^HKCR\\";
            if (IsRegistryPath)
                return Regex.IsMatch(input, pattern, RegexOptions.IgnoreCase) ? new ValidationResult(true, null) : new ValidationResult(false, Resources.FilePathInvalid);


            try
            {
                input = Base.ConvertPath(input, true, Core.AppInfo.Is64Bit);
                new Uri(input);
            }
            catch
            {
                return new ValidationResult(false, Resources.FilePathInvalid);
            }


            return new ValidationResult(true, null);
        }
    }

    public sealed class RegistryPathRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            var input = value as string;
            if (input == null)
                return new ValidationResult(false, Resources.FilePathInvalid);
            if (String.IsNullOrWhiteSpace(input))
                return new ValidationResult(false, Resources.FilePathInvalid);

            const string pattern = @"^HKLM\\|^HKEY_CLASSES_ROOT\\|^HKEY_CURRENT_USER\\|^HKEY_LOCAL_MACHINE\\|^HKEY_USERS\\|^HKU\\|^HKCR\\";

            return Regex.IsMatch(input, pattern, RegexOptions.IgnoreCase) ? new ValidationResult(true, null) : new ValidationResult(false, Resources.FilePathInvalid);
        }
    }

    public sealed class LocaleStringRule : ValidationRule
    {
        /// <summary>
        ///   The name of the Collection of locale strings to get
        /// </summary>
        internal string PropertyName { private get; set; }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            var input = value as string;

            if (!String.IsNullOrWhiteSpace(input))
                return new ValidationResult(true, null);
            switch (PropertyName)
            {
                case "App.Name":

                    #region Code

                    if (Core.AppInfo.Name == null)
                        return new ValidationResult(false, Resources.InputRequired);

                    for (var x = 0; x < Core.AppInfo.Name.Count; x++)
                    {
                        if (Core.AppInfo.Name[x].Lang != Base.Locale)
                            continue;
                        Core.AppInfo.Name.RemoveAt(x);
                        break;
                    }

                    #endregion

                    break;
                case "App.Publisher":

                    #region Code

                    if (Core.AppInfo.Publisher == null)
                        return new ValidationResult(false, Resources.InputRequired);

                    for (var x = 0; x < Core.AppInfo.Publisher.Count; x++)
                    {
                        if (Core.AppInfo.Publisher[x].Lang != Base.Locale)
                            continue;
                        Core.AppInfo.Publisher.RemoveAt(x);
                        break;
                    }

                    #endregion

                    break;
                case "App.Description":

                    #region Code

                    if (Core.AppInfo.Description == null)
                        return new ValidationResult(false, Resources.InputRequired);

                    for (var x = 0; x < Core.AppInfo.Description.Count; x++)
                    {
                        if (Core.AppInfo.Description[x].Lang != Base.Locale)
                            continue;
                        Core.AppInfo.Description.RemoveAt(x);
                        break;
                    }

                    #endregion

                    break;
                case "Update.Description":

                    #region Code

                    if (Core.UpdateInfo.Description == null)
                        return new ValidationResult(false, Resources.InputRequired);

                    for (var x = 0; x < Core.UpdateInfo.Description.Count; x++)
                    {
                        if (Core.UpdateInfo.Description[x].Lang != Base.Locale)
                            continue;
                        Core.UpdateInfo.Description.RemoveAt(x);
                        break;
                    }

                    #endregion

                    break;
                case "Update.Name":

                    #region Code

                    if (Core.UpdateInfo.Name == null)
                        return new ValidationResult(false, Resources.InputRequired);

                    for (var x = 0; x < Core.UpdateInfo.Name.Count; x++)
                    {
                        if (Core.UpdateInfo.Name[x].Lang != Base.Locale)
                            continue;
                        Core.UpdateInfo.Name.RemoveAt(x);
                        break;
                    }

                    #endregion

                    break;
            }
            return new ValidationResult(false, Resources.InputRequired);
        }
    }
}