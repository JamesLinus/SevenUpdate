using System;
using System.Globalization;
using System.Windows.Controls;

namespace Microsoft.Windows.Controls
{
    public class RequiredInputRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            var input = value as string;

            if (input != null && input.Length == 0)
                return new ValidationResult(false, "Validation error. Input required.");

            return new ValidationResult(true, null);
        }
    }

    public class UrlInputRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            try
            {
                var url = value as string;
                if (!String.IsNullOrEmpty(url))
                    new Uri(value.ToString());
                return new ValidationResult(true, null);
            }
            catch
            {
                return new ValidationResult(false, "Validation error. The Url is not valid");
            }
        }
    }

    public class ValidPathInputRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            var input = value as string;

            if (input != null && input.Length == 0)
                return new ValidationResult(false, "Validation error. The path entered is not valid.");

            return new ValidationResult(true, null);
        }
    }
}
