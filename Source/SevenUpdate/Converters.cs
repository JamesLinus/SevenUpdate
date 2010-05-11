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
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using SevenUpdate.Base;

#endregion

namespace SevenUpdate.Converters
{
    /// <summary>
    ///   Converts Importance to a localized string
    /// </summary>
    [ValueConversion(typeof (Importance), typeof (string))]
    public sealed class ImportanceGroupConverter : IValueConverter
    {
        #region IValueConverter Members

        /// <summary>
        ///   Converts a object into another object
        /// </summary>
        /// <returns>the converted object</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var imp = ((Importance) value);

            if (imp == Importance.Recommended)
            {
                if (App.Settings.IncludeRecommended)
                    imp = Importance.Important;
            }
            if (imp == Importance.Locale)
                imp = Importance.Optional;

            return App.RM.GetString(imp.ToString());
        }

        /// <summary>
        ///   Converts a converted object back into it's original form
        /// </summary>
        /// <returns>The original object</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return new NotImplementedException();
        }

        #endregion
    }

    /// <summary>
    ///   Converts a string to a localized string
    /// </summary>
    [ValueConversion(typeof (string), typeof (string))]
    public sealed class StringToLocaleConverter : IValueConverter
    {
        #region IValueConverter Members

        /// <summary>
        ///   Converts a object into another object
        /// </summary>
        /// <returns>the converted object</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Returns the localized string found in the resource dictionary
            return value == null ? null : App.RM.GetString(value.ToString());
        }

        /// <summary>
        ///   Converts a converted object back into it's original form
        /// </summary>
        /// <returns>The original object</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return new NotImplementedException();
        }

        #endregion
    }

    /// <summary>
    ///   Converts a <see cref = "LocaleString" /> to a localized string
    /// </summary>
    [ValueConversion(typeof (LocaleString), typeof (string))]
    public sealed class LocaleStringConverter : IValueConverter
    {
        #region IValueConverter Members

        /// <summary>
        ///   Converts a object into another object
        /// </summary>
        /// <returns>the converted object</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var localeStrings = value as Collection<LocaleString>;

            // Loops through the collection of LocaleStrings
            if (localeStrings != null)
            {
                foreach (LocaleString t in localeStrings.Where(t => t.Lang == Base.Base.Locale))
                    return t.Value;

                // Returns an english string if the specified locale is not avaliable
                return localeStrings[0].Value;
            }
            return App.RM.GetString("NotAvailable");
        }

        /// <summary>
        ///   Converts a converted object back into it's original form
        /// </summary>
        /// <returns>The original object</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return new NotImplementedException();
        }

        #endregion
    }

    /// <summary>
    ///   Converts a ulong or group of ulongs into a string readable filesize
    /// </summary>
    [ValueConversion(typeof (UpdateFile), typeof (string))]
    public sealed class FileSizeConverter : IValueConverter
    {
        #region IValueConverter Members

        /// <summary>
        ///   Converts a object into another object
        /// </summary>
        /// <returns>the converted object</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                var files = value as Collection<UpdateFile>;
                // Gets the full size of the update then converts it into a string format
                return Base.Base.ConvertFileSize(App.GetUpdateSize(files));
            }
            catch (Exception)
            {
                var size = System.Convert.ToUInt64(value);
                // Converts the ulong into a readable file size string
                return Base.Base.ConvertFileSize(size);
            }

        }

        /// <summary>
        ///   Converts a converted object back into it's original form
        /// </summary>
        /// <returns>The original object</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return new NotImplementedException();
        }

        #endregion
    }

    /// <summary>
    ///   Converts an Enum to a bool value
    /// </summary>
    [ValueConversion(typeof (UpdateStatus), typeof (bool))]
    public sealed class EnumToBoolConverter : IValueConverter
    {
        #region IValueConverter Members

        /// <summary>
        ///   Converts a object into another object
        /// </summary>
        /// <returns>the converted object</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // If the UpdateStatus is hidden return False, otherwise return true
            return ((UpdateStatus) value) != UpdateStatus.Hidden;
        }

        /// <summary>
        ///   Converts a converted object back into it's original form
        /// </summary>
        /// <returns>The original object</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((bool) value) == false ? UpdateStatus.Hidden : UpdateStatus.Visible;
        }

        #endregion
    }

    /// <summary>
    ///   Converts a Bool to a readable string
    /// </summary>
    [ValueConversion(typeof (bool), typeof (string))]
    public sealed class BoolToStringConverter : IValueConverter
    {
        #region IValueConverter Members

        /// <summary>
        ///   Converts a object into another object
        /// </summary>
        /// <returns>the converted object</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // If Is64Bit
            if (((bool) value))
                return "x64";
            return "x86";
        }

        /// <summary>
        ///   Converts a converted object back into it's original form
        /// </summary>
        /// <returns>The original object</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return new NotImplementedException();
        }

        #endregion
    }
}