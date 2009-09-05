#region GNU Public License v3

// Copyright 2007, 2008 Robert Baker, aka Seven ALive.
// This file is part of Seven Update.
// 
//     Seven Update is free software: you can redistribute it and/or modify
//     it under the terms of the GNU General Public License as published by
//     the Free Software Foundation, either version 3 of the License, or
//     (at your option) any later version.
// 
//     Seven Update is distributed in the hope that it will be useful,
//     but WITHOUT ANY WARRANTY; without even the implied warranty of
//     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//     GNU General Public License for more details.
// 
//    You should have received a copy of the GNU General Public License
//     along with Seven Update.  If not, see <http://www.gnu.org/licenses/>.

#endregion

#region

using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

#endregion

namespace SevenUpdate.Converters
{
    /// <summary>
    /// Converts a Brush to bool
    /// </summary>
    [ValueConversion(typeof(Brush), typeof(bool))]
    public class BrushToBoolConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value == Brushes.Black;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return new NotImplementedException();
        }

        #endregion
    }

    /// <summary>
    /// Converts a string to a localized string
    /// </summary>
    [ValueConversion(typeof(string), typeof(string))]
    public class StringToLocaleConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            /// Returns the localized string found in the resource dictionary
            return App.RM.GetString(value.ToString());
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return new NotImplementedException();
        }

        #endregion
    }

    /// <summary>
    /// Converts a <see cref="LocaleString"/> to a localized string
    /// </summary>
    [ValueConversion(typeof(LocaleString), typeof(string))]
    public class LocaleStringConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var localeStrings = value as Collection<LocaleString>;

            /// Loops through the collection of LocaleStrings
            if (localeStrings != null)
            {
                for (var x = 0; x < localeStrings.Count; x++)
                {
                    /// If a string is available in the locale specified in the settings, return it
                    if (localeStrings[x].Lang != Shared.Locale)
                        continue;
                    return localeStrings[x].Value;
                }

                /// Returns an english string if the specified locale is not avaliable
                return localeStrings[0].Value;
            }
            return App.RM.GetString("NotAvailable");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return new NotImplementedException();
        }

        #endregion
    }

    /// <summary>
    /// Converts a collection of UpdateFiles to a string representing the size
    /// </summary>
    [ValueConversion(typeof(UpdateFile), typeof(string))]
    public class FileSizeConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var files = value as Collection<UpdateFile>;
            /// Gets the full size of the update then converts it into a string format
            return Shared.ConvertFileSize(App.GetUpdateSize(files));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return new NotImplementedException();
        }

        #endregion
    }

    /// <summary>
    /// Converts an Enum to a bool value
    /// </summary>
    [ValueConversion(typeof(UpdateStatus), typeof(bool))]
    public class EnumToBoolConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            /// If the UpdateStatus is hidden return False, otherwise return true
            return ((UpdateStatus)value) != UpdateStatus.Hidden;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((bool)value) == false ? UpdateStatus.Hidden : UpdateStatus.Visible;
        }

        #endregion
    }

    /// <summary>
    /// Converts a Bool to a readable string
    /// </summary>
    [ValueConversion(typeof(bool), typeof(string))]
    public class BoolToStringConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            /// If Is64Bit
            if (((bool)value))
                return "x64";
            return "x86";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return new NotImplementedException();
        }

        #endregion
    }
}