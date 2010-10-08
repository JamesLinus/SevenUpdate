// ***********************************************************************
// <copyright file="StringToBooleanConverter.cs"
//            project="System.Windows"
//            assembly="System.Windows"
//            solution="SevenUpdate"
//            company="Seven Software">
//     Copyright (c) Seven Software. All rights reserved.
// </copyright>
// <author username="sevenalive">Robert Baker</author>
// ***********************************************************************
namespace System.Windows.Converters
{
    using System.Globalization;
    using System.Windows.Data;

    /// <summary>
    /// Converts the string to a Boolean
    /// </summary>
    [ValueConversion(typeof(string), typeof(bool))]
    public class StringToBooleanConverter : IValueConverter
    {
        #region Implemented Interfaces

        #region IValueConverter

        /// <summary>
        /// Converts a object into another object
        /// </summary>
        /// <param name="value">
        /// The value produced by the binding source.
        /// </param>
        /// <param name="targetType">
        /// The type of the binding target property.
        /// </param>
        /// <param name="parameter">
        /// The converter parameter to use.
        /// </param>
        /// <param name="culture">
        /// The culture to use in the converter.
        /// </param>
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
        /// <param name="value">
        /// The value that is produced by the binding target.
        /// </param>
        /// <param name="targetType">
        /// The type to convert to.
        /// </param>
        /// <param name="parameter">
        /// The converter parameter to use.
        /// </param>
        /// <param name="culture">
        /// The culture to use in the converter.
        /// </param>
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
}