#region

using System;
using System.Globalization;
using System.IO;
using System.Windows.Controls;
using SevenUpdate.Sdk.Properties;

#endregion

namespace SevenUpdate.Sdk.Helpers
{
    public class AppLocationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            var input = value as string;
            if (input == null)
                return new ValidationResult(false, Resources.FilePathInvalid);

            if (input.StartsWith(@"HKLM\", true, null) || input.StartsWith(@"HKCR\", true, null) || input.StartsWith(@"HKCU\", true, null) || input.StartsWith(@"HKU\", true, null) ||
                input.StartsWith(@"HKEY_CLASSES_ROOT\") || input.StartsWith(@"HKEY_CURRENT_USER\", true, null) || input.StartsWith(@"HKEY_LOCAL_MACHINE\", true, null) ||
                input.StartsWith(@"HKEY_USERS\", true, null))
                return new ValidationResult(true, null);

            if (string.IsNullOrEmpty(input) || input.IndexOfAny(Path.GetInvalidPathChars()) >= 0)
                return new ValidationResult(false, Resources.FilePathInvalid);

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
}