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
using System.Windows;
using System.Windows.Data;

#endregion

namespace Microsoft.Windows.Common
{
    /// <summary>
    ///   Converts the string to a bool
    /// </summary>
    [ValueConversion(typeof (string), typeof (bool))]
    public class StringToBoolConverter : IValueConverter
    {
        #region IValueConverter Members

        /// <summary>
        ///   Converts a object into another object
        /// </summary>
        /// <returns>the converted object</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var stringValue = value as string;
            // If no value should return false
            if (parameter != null)
            {
                if (System.Convert.ToBoolean(parameter))
                    // If  no value return true, otherwise false
                    return !String.IsNullOrEmpty(stringValue);
            }

            // If  no value return true, otherwise false
            return String.IsNullOrEmpty(stringValue);
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
    ///   Converts the string to a bool
    /// </summary>
    [ValueConversion(typeof (string), typeof (Visibility))]
    public class StringToVisibilityConverter : IValueConverter
    {
        #region IValueConverter Members

        /// <summary>
        ///   Converts a object into another object
        /// </summary>
        /// <returns>the converted object</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var stringValue = value as string;
            // If no value should return false
            if (parameter != null)
            {
                if (System.Convert.ToBoolean(parameter))
                {
                    // If  no value return true, otherwise false
                    return !String.IsNullOrEmpty(stringValue) ? Visibility.Visible : Visibility.Collapsed;
                }
            }

            // If  no value return true, otherwise false
            return String.IsNullOrEmpty(stringValue) ? Visibility.Visible : Visibility.Collapsed;
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
    ///   Converts the Int to Visibility
    /// </summary>
    [ValueConversion(typeof (int), typeof (Visibility))]
    public class IntToVisibilityConverter : IValueConverter
    {
        #region IValueConverter Members

        /// <summary>
        ///   Converts a object into another object
        /// </summary>
        /// <returns>the converted object</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var count = value is int ? (int) value : 0;

            if (parameter != null)
            {
                // If count is less then 1 and should return visible
                if (count < 1 && System.Convert.ToBoolean(parameter))
                    return Visibility.Visible;
                return Visibility.Collapsed;
            }

            return count < 1 ? Visibility.Collapsed : Visibility.Visible;
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
    ///   Converts the Int to Visibility
    /// </summary>
    [ValueConversion(typeof (int), typeof (Visibility))]
    public class IndexToVisibilityConverter : IValueConverter
    {
        #region IValueConverter Members

        /// <summary>
        ///   Converts a object into another object
        /// </summary>
        /// <returns>the converted object</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var count = value is int ? (int) value : -1;

            if (parameter != null)
            {
                // If count is less then 0 and should return visible
                if (count < 0 && System.Convert.ToBoolean(parameter))
                    return Visibility.Visible;
                return Visibility.Collapsed;
            }

            return count < 0 ? Visibility.Collapsed : Visibility.Visible;
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
    ///   Converts the Int to Bool
    /// </summary>
    [ValueConversion(typeof (int), typeof (bool))]
    public class IntToBoolConverter : IValueConverter
    {
        #region IValueConverter Members

        /// <summary>
        ///   Converts a object into another object
        /// </summary>
        /// <returns>the converted object</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var count = System.Convert.ToUInt64(value);

            if (parameter != null)
            {
                // If count is less then 1 and should return visible
                return count < 1 && System.Convert.ToBoolean(parameter);
            }

            return count < 1 ? false : true;
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
    ///   Converts the Enum to a Boolean
    /// </summary>
    [ValueConversion(typeof (Enum), typeof (bool))]
    public class EnumToBool : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value.Equals(parameter);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value.Equals(false) ? DependencyProperty.UnsetValue : parameter;
        }

        #endregion
    }

    /// <summary>
    ///   Converts the Enum to a Boolean
    /// </summary>
    [ValueConversion(typeof (Enum), typeof (bool))]
    public class InverseEnumToBool : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return !value.Equals(parameter);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return new NotImplementedException();
        }

        #endregion
    }

    /// <summary>
    ///   Converts a bool value to the opposite value
    /// </summary>
    [ValueConversion(typeof (bool), typeof (bool))]
    public class InverseBoolConverter : IValueConverter
    {
        #region IValueConverter Members

        /// <summary>
        ///   Converts a object into another object
        /// </summary>
        /// <returns>the converted object</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return !System.Convert.ToBoolean(value);
        }

        /// <summary>
        ///   Converts a converted object back into it's original form
        /// </summary>
        /// <returns>The original object</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return !System.Convert.ToBoolean(value);
        }

        #endregion
    }
}