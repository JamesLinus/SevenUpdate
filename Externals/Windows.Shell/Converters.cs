using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Microsoft.Windows.Common
{
    /// <summary>
    ///   Converts the string to a bool
    /// </summary>
    [ValueConversion(typeof(string), typeof(bool))]
    public sealed class StringToBoolConverter : IValueConverter
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
    [ValueConversion(typeof(string), typeof(Visibility))]
    public sealed class StringToVisibilityConverter : IValueConverter
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
    [ValueConversion(typeof(int), typeof(Visibility))]
    public sealed class IntToVisibilityConverter : IValueConverter
    {
        #region IValueConverter Members

        /// <summary>
        ///   Converts a object into another object
        /// </summary>
        /// <returns>the converted object</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var count = value is int ? (int)value : 0;

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
    [ValueConversion(typeof(int), typeof(bool))]
    public sealed class IntToBoolConverter : IValueConverter
    {
        #region IValueConverter Members

        /// <summary>
        ///   Converts a object into another object
        /// </summary>
        /// <returns>the converted object</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var count = value is int ? (int)value : 0;

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
    [ValueConversion(typeof(Enum), typeof(bool))]
    public sealed class EnumToBool : IValueConverter
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
    [ValueConversion(typeof(Enum), typeof(bool))]
    public sealed class InverseEnumToBool : IValueConverter
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
    [ValueConversion(typeof(bool), typeof(bool))]
    public sealed class InverseBoolConverter : IValueConverter
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
