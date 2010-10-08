// ***********************************************************************
// <copyright file="DateConverter.cs"
//            project="SevenUpdate"
//            assembly="SevenUpdate"
//            solution="SevenUpdate"
//            company="Seven Software">
//     Copyright (c) Seven Software. All rights reserved.
// </copyright>
// <author username="sevenalive">Robert Baker</author>
// ***********************************************************************
namespace SevenUpdate.Converters
{
    using System;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Data;

    using SevenUpdate.Properties;

    /// <summary>
    /// Converts the <see cref="DateTime"/> to a String
    /// </summary>
    [ValueConversion(typeof(DateTime), typeof(string))]
    internal sealed class DateConverter : IValueConverter
    {
        #region Implemented Interfaces

        #region IValueConverter

        /// <summary>
        /// Converts a value.
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
        /// A converted value. If the method returns <see langword="null"/>, the valid <see langword="null"/> value is used.
        /// </returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var dateTime = value is DateTime ? (DateTime)value : new DateTime();

            if (dateTime != DateTime.MinValue)
            {
                return dateTime.Date.Equals(DateTime.Now.Date)
                           ? String.Format(CultureInfo.CurrentCulture, Resources.TodayAt, dateTime.ToShortTimeString())
                           : String.Format(CultureInfo.CurrentCulture, Resources.TimeAt, dateTime.ToShortDateString(), dateTime.ToShortTimeString());
            }

            return Resources.Never;
        }

        /// <summary>
        /// Converts a value.
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
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }

        #endregion

        #endregion
    }
}