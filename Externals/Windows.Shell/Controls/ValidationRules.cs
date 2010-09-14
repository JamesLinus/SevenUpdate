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
using System.Windows.Controls;
using Microsoft.Windows.Properties;

#endregion

namespace Microsoft.Windows.Controls
{
    public sealed class RequiredInputRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            var input = value as string;

            return String.IsNullOrWhiteSpace(input) ? new ValidationResult(false, Resources.InputRequired) : new ValidationResult(true, null);
        }
    }

    public sealed class UrlInputRule : ValidationRule
    {
        public bool IsRequired { get; set; }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            try
            {
                var url = value as string;

                if (String.IsNullOrWhiteSpace(url))
                    return IsRequired ? new ValidationResult(false, Resources.FilePathInvalid) : new ValidationResult(true, null);

                new Uri(value.ToString());
                return new ValidationResult(true, null);
            }
            catch
            {
                return new ValidationResult(false, Resources.UrilInvalid);
            }
        }
    }

    public sealed class FileNameInputRule : ValidationRule
    {
        public bool IsRequired { get; set; }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            var input = value as string;

            if (String.IsNullOrWhiteSpace(input))
                return IsRequired ? new ValidationResult(false, Resources.FilePathInvalid) : new ValidationResult(true, null);

            if (Path.GetFileName(input).IndexOfAny(Path.GetInvalidFileNameChars()) >= 0 || Path.GetDirectoryName(input).IndexOfAny(Path.GetInvalidPathChars()) >= 0)
                return new ValidationResult(false, Resources.FilePathInvalid);


            return new ValidationResult(true, null);
        }
    }

    public class DirectoryInputRule : ValidationRule
    {
        public bool IsRequired { get; set; }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            var input = value as string;
            if (input == null)
                return IsRequired ? new ValidationResult(false, Resources.FilePathInvalid) : new ValidationResult(true, null);

            if (string.IsNullOrWhiteSpace(input))
                return IsRequired ? new ValidationResult(false, Resources.FilePathInvalid) : new ValidationResult(true, null);

            return Path.GetDirectoryName(input).IndexOfAny(Path.GetInvalidPathChars()) >= 0 ? new ValidationResult(false, Resources.FilePathInvalid) : new ValidationResult(true, null);
        }
    }
}