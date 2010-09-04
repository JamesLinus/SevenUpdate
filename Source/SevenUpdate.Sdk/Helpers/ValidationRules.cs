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
    public class AppDirectoryRule : ValidationRule
    {
        public bool IsRegistryPath { get; set; }

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
                input = SevenUpdate.Base.ConvertPath(input, true, Base.AppInfo.Is64Bit);
                new Uri(input);
            }
            catch
            {
                return new ValidationResult(false, Resources.FilePathInvalid);
            }


            return new ValidationResult(true, null);
        }
    }

    public class RegistryPathRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            var input = value as string;
            if (String.IsNullOrWhiteSpace(input))
                return new ValidationResult(false, Resources.FilePathInvalid);

            const string pattern = @"^HKLM\\|^HKEY_CLASSES_ROOT\\|^HKEY_CURRENT_USER\\|^HKEY_LOCAL_MACHINE\\|^HKEY_USERS\\|^HKU\\|^HKCR\\";

            return Regex.IsMatch(input, pattern, RegexOptions.IgnoreCase) ? new ValidationResult(true, null) : new ValidationResult(false, Resources.FilePathInvalid);
        }
    }

    public class LocaleStringRule : ValidationRule
    {
        /// <summary>
        ///   The name of the Collection of locale strings to get
        /// </summary>
        public string PropertyName { get; set; }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            var input = value as string;

            if (!String.IsNullOrWhiteSpace(input))
                return new ValidationResult(true, null);
            switch (PropertyName)
            {
                case "App.Name":

                    #region Code

                    if (Base.AppInfo.Name == null)
                        return new ValidationResult(false, Resources.InputRequired);

                    for (int x = 0; x < Base.AppInfo.Name.Count; x++)
                    {
                        if (Base.AppInfo.Name[x].Lang != SevenUpdate.Base.Locale)
                            continue;
                        Base.AppInfo.Name.RemoveAt(x);
                        break;
                    }

                    #endregion

                    break;
                case "App.Publisher":

                    #region Code

                    if (Base.AppInfo.Publisher == null)
                        return new ValidationResult(false, Resources.InputRequired);

                    for (int x = 0; x < Base.AppInfo.Publisher.Count; x++)
                    {
                        if (Base.AppInfo.Publisher[x].Lang != SevenUpdate.Base.Locale)
                            continue;
                        Base.AppInfo.Publisher.RemoveAt(x);
                        break;
                    }

                    #endregion

                    break;
                case "App.Description":

                    #region Code

                    if (Base.AppInfo.Description == null)
                        return new ValidationResult(false, Resources.InputRequired);

                    for (int x = 0; x < Base.AppInfo.Description.Count; x++)
                    {
                        if (Base.AppInfo.Description[x].Lang != SevenUpdate.Base.Locale)
                            continue;
                        Base.AppInfo.Description.RemoveAt(x);
                        break;
                    }

                    #endregion

                    break;
                case "Update.Description":

                    #region Code

                    if (Base.UpdateInfo.Description == null)
                        return new ValidationResult(false, Resources.InputRequired);

                    for (int x = 0; x < Base.UpdateInfo.Description.Count; x++)
                    {
                        if (Base.UpdateInfo.Description[x].Lang != SevenUpdate.Base.Locale)
                            continue;
                        Base.UpdateInfo.Description.RemoveAt(x);
                        break;
                    }

                    #endregion

                    break;
                case "Update.Name":

                    #region Code

                    if (Base.UpdateInfo.Name == null)
                        return new ValidationResult(false, Resources.InputRequired);

                    for (int x = 0; x < Base.UpdateInfo.Name.Count; x++)
                    {
                        if (Base.UpdateInfo.Name[x].Lang != SevenUpdate.Base.Locale)
                            continue;
                        Base.UpdateInfo.Name.RemoveAt(x);
                        break;
                    }

                    #endregion

                    break;
            }
            return new ValidationResult(false, Resources.InputRequired);
        }
    }
}