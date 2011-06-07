// ***********************************************************************
// <copyright file="DateConverter.cs" project="SevenUpdate" assembly="SevenUpdate" solution="SevenUpdate" company="Seven Software">
//     Copyright (c) Seven Software. All rights reserved.
// </copyright>
// <author username="sevenalive">Robert Baker</author>
// <license href="http://www.gnu.org/licenses/gpl-3.0.txt" name="GNU General Public License 3">
//  This file is part of Seven Update.
//    Seven Update is free software: you can redistribute it and/or modify it under the terms of the GNU General Public
//    License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any
//    later version. Seven Update is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without
//    even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details. You should have received a copy of the GNU General Public License
//    along with Seven Update.  If not, see http://www.gnu.org/licenses/.
// </license>
// <summary>
//   Converts the <see cref="DateTime"/> to a String
// .</summary> ***********************************************************************

namespace SevenUpdate.Converters
{
    using System;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Data;

    using Properties;

    /// <summary>
    ///   Converts the <c>DateTime</c> to a String.
    /// </summary>
    [ValueConversion(typeof(DateTime), typeof(string))]
    internal sealed class DateConverter : IValueConverter
    {
        #region Implemented Interfaces

        #region IValueConverter

        /// <summary>
        ///   Converts a value.
        /// </summary>
        /// <param name="value">
        ///   The value produced by the binding source.
        /// </param>
        /// <param name="targetType">
        ///   The type of the binding target property.
        /// </param>
        /// <param name="parameter">
        ///   The converter parameter to use.
        /// </param>
        /// <param name="culture">
        ///   The culture to use in the converter.
        /// </param>
        /// <returns>
        ///   A converted value. If the method returns <c>null</c>, the valid <c>null</c> value is used.
        /// </returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var dateTime = value is DateTime ? (DateTime)value : new DateTime();

            if (dateTime != DateTime.MinValue)
            {
                return dateTime.Date.Equals(DateTime.Now.Date)
                           ? string.Format(CultureInfo.CurrentCulture, Resources.TodayAt, dateTime.ToShortTimeString())
                           : string.Format(
                               CultureInfo.CurrentCulture,
                               Resources.TimeAt,
                               dateTime.ToShortDateString(),
                               dateTime.ToShortTimeString());
            }

            return Resources.Never;
        }

        /// <summary>
        ///   Converts a value.
        /// </summary>
        /// <param name="value">
        ///   The value that is produced by the binding target.
        /// </param>
        /// <param name="targetType">
        ///   The type to convert to.
        /// </param>
        /// <param name="parameter">
        ///   The converter parameter to use.
        /// </param>
        /// <param name="culture">
        ///   The culture to use in the converter.
        /// </param>
        /// <returns>
        ///   A converted value. If the method returns <c>null</c>, the valid <c>null</c> value is used.
        /// </returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }

        #endregion

        #endregion
    }
}