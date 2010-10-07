// ***********************************************************************
// Assembly         : System.Windows
// Author           : Robert Baker (sevenalive)
// Last Modified By : Robert Baker (sevenalive)
// Last Modified On : 10-06-2010
// Copyright        : (c) Seven Software. All rights reserved.
// ***********************************************************************
namespace System.Windows
{
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Windows.Data;

    /// <summary>
    /// Converts the string to a Boolean
    /// </summary>
    [ValueConversion(typeof(string), typeof(bool))]
    public class StringToBoolConverter : IValueConverter
    {
        #region Implemented Interfaces

        #region IValueConverter

        /// <summary>
        /// Converts a object into another object
        /// </summary>
        /// <parameter name="value">
        /// The value produced by the binding source.
        /// </parameter>
        /// <parameter name="targetType">
        /// The type of the binding target property.
        /// </parameter>
        /// <parameter name="parameter">
        /// The converter parameter to use.
        /// </parameter>
        /// <parameter name="culture">
        /// The culture to use in the converter.
        /// </parameter>
        /// <returns>
        /// the converted object
        /// </returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var stringValue = value as string;

            // If no value should return false
            if (parameter != null)
            {
                if (System.Convert.ToBoolean(parameter, CultureInfo.CurrentCulture))
                {
                    // If  no value return true, otherwise false
                    return !String.IsNullOrEmpty(stringValue);
                }
            }

            // If  no value return true, otherwise false
            return String.IsNullOrEmpty(stringValue);
        }

        /// <summary>
        /// Converts a converted object back into it's original form
        /// </summary>
        /// <parameter name="value">
        /// The value that is produced by the binding target.
        /// </parameter>
        /// <parameter name="targetType">
        /// The type to convert to.
        /// </parameter>
        /// <parameter name="parameter">
        /// The converter parameter to use.
        /// </parameter>
        /// <parameter name="culture">
        /// The culture to use in the converter.
        /// </parameter>
        /// <returns>
        /// The original object
        /// </returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }

        #endregion

        #endregion
    }

    /// <summary>
    /// Converts the string to a bool
    /// </summary>
    [SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "ValueConverter")]
    [ValueConversion(typeof(string), typeof(Visibility))]
    public class StringToVisibilityConverter : IValueConverter
    {
        #region Implemented Interfaces

        #region IValueConverter

        /// <summary>
        /// Converts a object into another object
        /// </summary>
        /// <parameter name="value">
        /// The value produced by the binding source.
        /// </parameter>
        /// <parameter name="targetType">
        /// The type of the binding target property.
        /// </parameter>
        /// <parameter name="parameter">
        /// The converter parameter to use.
        /// </parameter>
        /// <parameter name="culture">
        /// The culture to use in the converter.
        /// </parameter>
        /// <returns>
        /// the converted object
        /// </returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var stringValue = value as string;

            // If no value should return false
            if (parameter != null)
            {
                if (System.Convert.ToBoolean(parameter, CultureInfo.CurrentCulture))
                {
                    // If  no value return true, otherwise false
                    return !String.IsNullOrEmpty(stringValue) ? Visibility.Visible : Visibility.Collapsed;
                }
            }

            // If  no value return true, otherwise false
            return String.IsNullOrEmpty(stringValue) ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// Converts a converted object back into it's original form
        /// </summary>
        /// <parameter name="value">
        /// The value that is produced by the binding target.
        /// </parameter>
        /// <parameter name="targetType">
        /// The type to convert to.
        /// </parameter>
        /// <parameter name="parameter">
        /// The converter parameter to use.
        /// </parameter>
        /// <parameter name="culture">
        /// The culture to use in the converter.
        /// </parameter>
        /// <returns>
        /// The original object
        /// </returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }

        #endregion

        #endregion
    }

    /// <summary>
    /// Converts the Int to Visibility
    /// </summary>
    [ValueConversion(typeof(int), typeof(Visibility))]
    public class IntToVisibilityConverter : IValueConverter
    {
        #region Implemented Interfaces

        #region IValueConverter

        /// <summary>
        /// Converts a object into another object
        /// </summary>
        /// <parameter name="value">
        /// The value produced by the binding source.
        /// </parameter>
        /// <parameter name="targetType">
        /// The type of the binding target property.
        /// </parameter>
        /// <parameter name="parameter">
        /// The converter parameter to use.
        /// </parameter>
        /// <parameter name="culture">
        /// The culture to use in the converter.
        /// </parameter>
        /// <returns>
        /// the converted object
        /// </returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var count = value is int ? (int)value : 0;

            if (parameter != null)
            {
                // If count is less then 1 and should return visible
                if (count < 1 && System.Convert.ToBoolean(parameter, CultureInfo.CurrentCulture))
                {
                    return Visibility.Visible;
                }

                return Visibility.Collapsed;
            }

            return count < 1 ? Visibility.Collapsed : Visibility.Visible;
        }

        /// <summary>
        /// Converts a converted object back into it's original form
        /// </summary>
        /// <parameter name="value">
        /// The value that is produced by the binding target.
        /// </parameter>
        /// <parameter name="targetType">
        /// The type to convert to.
        /// </parameter>
        /// <parameter name="parameter">
        /// The converter parameter to use.
        /// </parameter>
        /// <parameter name="culture">
        /// The culture to use in the converter.
        /// </parameter>
        /// <returns>
        /// The original object
        /// </returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }

        #endregion

        #endregion
    }

    /// <summary>
    /// Converts the Int to Visibility
    /// </summary>
    [ValueConversion(typeof(int), typeof(Visibility))]
    public class IndexToVisibilityConverter : IValueConverter
    {
        #region Implemented Interfaces

        #region IValueConverter

        /// <summary>
        /// Converts a object into another object
        /// </summary>
        /// <parameter name="value">
        /// The value produced by the binding source.
        /// </parameter>
        /// <parameter name="targetType">
        /// The type of the binding target property.
        /// </parameter>
        /// <parameter name="parameter">
        /// The converter parameter to use.
        /// </parameter>
        /// <parameter name="culture">
        /// The culture to use in the converter.
        /// </parameter>
        /// <returns>
        /// the converted object
        /// </returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var count = value is int ? (int)value : -1;

            if (parameter != null)
            {
                // If count is less then 0 and should return visible
                if (count < 0 && System.Convert.ToBoolean(parameter, CultureInfo.CurrentCulture))
                {
                    return Visibility.Visible;
                }

                return Visibility.Collapsed;
            }

            return count < 0 ? Visibility.Collapsed : Visibility.Visible;
        }

        /// <summary>
        /// Converts a converted object back into it's original form
        /// </summary>
        /// <parameter name="value">
        /// The value that is produced by the binding target.
        /// </parameter>
        /// <parameter name="targetType">
        /// The type to convert to.
        /// </parameter>
        /// <parameter name="parameter">
        /// The converter parameter to use.
        /// </parameter>
        /// <parameter name="culture">
        /// The culture to use in the converter.
        /// </parameter>
        /// <returns>
        /// The original object
        /// </returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }

        #endregion

        #endregion
    }

    /// <summary>
    /// Converts the Int to Bool
    /// </summary>
    [ValueConversion(typeof(int), typeof(bool))]
    public class IntToBoolConverter : IValueConverter
    {
        #region Implemented Interfaces

        #region IValueConverter

        /// <summary>
        /// Converts a object into another object
        /// </summary>
        /// <parameter name="value">
        /// The value produced by the binding source.
        /// </parameter>
        /// <parameter name="targetType">
        /// The type of the binding target property.
        /// </parameter>
        /// <parameter name="parameter">
        /// The converter parameter to use.
        /// </parameter>
        /// <parameter name="culture">
        /// The culture to use in the converter.
        /// </parameter>
        /// <returns>
        /// the converted object
        /// </returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var count = System.Convert.ToUInt64(value, CultureInfo.CurrentCulture);

            if (parameter != null)
            {
                // If count is less then 1 and should return visible
                return count < 1 && System.Convert.ToBoolean(parameter, CultureInfo.CurrentCulture);
            }

            return count < 1 ? false : true;
        }

        /// <summary>
        /// Converts a converted object back into it's original form
        /// </summary>
        /// <parameter name="value">
        /// The value that is produced by the binding target.
        /// </parameter>
        /// <parameter name="targetType">
        /// The type to convert to.
        /// </parameter>
        /// <parameter name="parameter">
        /// The converter parameter to use.
        /// </parameter>
        /// <parameter name="culture">
        /// The culture to use in the converter.
        /// </parameter>
        /// <returns>
        /// The original object
        /// </returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }

        #endregion

        #endregion
    }

    /// <summary>
    /// Converts the <see cref="Enum"/> to a Boolean
    /// </summary>
    [ValueConversion(typeof(Enum), typeof(bool))]
    public class EnumToBool : IValueConverter
    {
        #region Implemented Interfaces

        #region IValueConverter

        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <parameter name="value">
        /// The value produced by the binding source.
        /// </parameter>
        /// <parameter name="targetType">
        /// The type of the binding target property.
        /// </parameter>
        /// <parameter name="parameter">
        /// The converter parameter to use.
        /// </parameter>
        /// <parameter name="culture">
        /// The culture to use in the converter.
        /// </parameter>
        /// <returns>
        /// A converted value. If the method returns <see langword="null"/>, the valid <see langword="null"/> value is used.
        /// </returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value.Equals(parameter);
        }

        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <parameter name="value">
        /// The value that is produced by the binding target.
        /// </parameter>
        /// <parameter name="targetType">
        /// The type to convert to.
        /// </parameter>
        /// <parameter name="parameter">
        /// The converter parameter to use.
        /// </parameter>
        /// <parameter name="culture">
        /// The culture to use in the converter.
        /// </parameter>
        /// <returns>
        /// A converted value. If the method returns <see langword="null"/>, the valid <see langword="null"/> value is used.
        /// </returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value.Equals(false) ? DependencyProperty.UnsetValue : parameter;
        }

        #endregion

        #endregion
    }

    /// <summary>
    /// Converts the Enum to a Boolean
    /// </summary>
    [ValueConversion(typeof(Enum), typeof(bool))]
    public class InverseEnumToBool : IValueConverter
    {
        #region Implemented Interfaces

        #region IValueConverter

        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <parameter name="value">
        /// The value produced by the binding source.
        /// </parameter>
        /// <parameter name="targetType">
        /// The type of the binding target property.
        /// </parameter>
        /// <parameter name="parameter">
        /// The converter parameter to use.
        /// </parameter>
        /// <parameter name="culture">
        /// The culture to use in the converter.
        /// </parameter>
        /// <returns>
        /// A converted value. If the method returns <see langword="null"/>, the valid <see langword="null"/> value is used.
        /// </returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return !value.Equals(parameter);
        }

        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <parameter name="value">
        /// The value that is produced by the binding target.
        /// </parameter>
        /// <parameter name="targetType">
        /// The type to convert to.
        /// </parameter>
        /// <parameter name="parameter">
        /// The converter parameter to use.
        /// </parameter>
        /// <parameter name="culture">
        /// The culture to use in the converter.
        /// </parameter>
        /// <returns>
        /// A converted value. If the method returns <see langword="null"/>, the valid <see langword="null"/> value is used.
        /// </returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }

        #endregion

        #endregion
    }

    /// <summary>
    /// Converts a bool value to the opposite value
    /// </summary>
    [ValueConversion(typeof(bool), typeof(bool))]
    public class InverseBoolConverter : IValueConverter
    {
        #region Implemented Interfaces

        #region IValueConverter

        /// <summary>
        /// Converts a object into another object
        /// </summary>
        /// <parameter name="value">
        /// The value produced by the binding source.
        /// </parameter>
        /// <parameter name="targetType">
        /// The type of the binding target property.
        /// </parameter>
        /// <parameter name="parameter">
        /// The converter parameter to use.
        /// </parameter>
        /// <parameter name="culture">
        /// The culture to use in the converter.
        /// </parameter>
        /// <returns>
        /// the converted object
        /// </returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return !System.Convert.ToBoolean(value, CultureInfo.CurrentCulture);
        }

        /// <summary>
        /// Converts a converted object back into it's original form
        /// </summary>
        /// <parameter name="value">
        /// The value that is produced by the binding target.
        /// </parameter>
        /// <parameter name="targetType">
        /// The type to convert to.
        /// </parameter>
        /// <parameter name="parameter">
        /// The converter parameter to use.
        /// </parameter>
        /// <parameter name="culture">
        /// The culture to use in the converter.
        /// </parameter>
        /// <returns>
        /// The original object
        /// </returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return !System.Convert.ToBoolean(value, CultureInfo.CurrentCulture);
        }

        #endregion

        #endregion
    }
}