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
            if (input == null)
                return new ValidationResult(false, Resources.FilePathInvalid);

            const string pattern = @"^HKLM\\|^HKEY_CLASSES_ROOT\\|^HKEY_CURRENT_USER\\|^HKEY_LOCAL_MACHINE\\|^HKEY_USERS\\|^HKU\\|^HKCR\\";

            return Regex.IsMatch(input, pattern, RegexOptions.IgnoreCase) ? new ValidationResult(true, null) : new ValidationResult(false, Resources.FilePathInvalid);
        }
    }
}