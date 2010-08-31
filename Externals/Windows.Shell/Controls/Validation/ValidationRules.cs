#region

using System;
using System.Globalization;
using System.IO;
using System.Net;
using System.Windows.Controls;
using Microsoft.Windows.Properties;

#endregion

namespace Microsoft.Windows.Controls
{
    public class RequiredInputRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            var input = value as string;

            return String.IsNullOrWhiteSpace(input) ? new ValidationResult(false, Resources.InputRequired) : new ValidationResult(true, null);
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
                {
                    var uri = new Uri(value.ToString());
                    var irequest = WebRequest.Create(uri);
                    var iresponse = irequest.GetResponse();
                    return iresponse == null ? new ValidationResult(true, null) : new ValidationResult(false, Resources.UrilInvalid);
                }
                return new ValidationResult(true, null);
            }
            catch
            {
                return new ValidationResult(false, Resources.UrilInvalid);
            }
        }
    }

    public class UrlRequiredInputRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            try
            {
                var url = value as string;
                if (!String.IsNullOrEmpty(url))
                {
                    var uri = new Uri(value.ToString());
                    var irequest = WebRequest.Create(uri);
                    var iresponse = irequest.GetResponse();
                    return iresponse == null ? new ValidationResult(true, null) : new ValidationResult(false, Resources.UrilInvalid);
                }
                return new ValidationResult(false, Resources.UrilInvalid);
            }
            catch
            {
                return new ValidationResult(false, Resources.UrilInvalid);
            }
        }
    }

    public class FileNameInputRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            var input = value as string;

            if (string.IsNullOrEmpty(input) || input.IndexOfAny(Path.GetInvalidPathChars()) >= 0 || input.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
                return new ValidationResult(false, Resources.FilePathInvalid);
            return new ValidationResult(true, null);
        }
    }

    public class DirectoryInputRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            var input = value as string;

            if (string.IsNullOrEmpty(input) || input.IndexOfAny(Path.GetInvalidPathChars()) >= 0)
                return new ValidationResult(false, Resources.FilePathInvalid);
            return new ValidationResult(true, null);
        }
    }
}