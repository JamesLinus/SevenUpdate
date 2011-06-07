// ***********************************************************************
// <copyright file="IntToBooleanConverter.cs" project="System.Windows" assembly="System.Windows" solution="SevenUpdate" company="Seven Software">
//     Copyright (c) Seven Software. All rights reserved.
// </copyright>
// <author username="sevenalive">Robert Baker</author>
// <license href="http://www.gnu.org/licenses/gpl-3.0.txt" name="GNU General Public License 3">
//  This file is part of Seven Update.
//    Seven Update is free software: you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation, either version 3 of the License, or
//    (at your option) any later version.
//    Seven Update is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.
//    You should have received a copy of the GNU General Public License
//    along with Seven Update.  If not, see http://www.gnu.org/licenses/.
// </license>
// ***********************************************************************

namespace System.Windows.Converters
{
    using Data;

    using Globalization;

    /// <summary>
    ///   Converts the Int to Bool.
    /// </summary>
    [ValueConversion(typeof(int), typeof(bool))]
    public class IntToBooleanConverter : IValueConverter
    {
        #region Implemented Interfaces

        #region IValueConverter

        /// <summary>
        ///   Converts a object into another object.
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
        ///   The converted object.
        /// </returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var count = System.Convert.ToUInt64(value, CultureInfo.CurrentCulture);

            if (parameter != null)
            {
                // If count is less then 1 and should return visible
                return count < 1 && System.Convert.ToBoolean(parameter, CultureInfo.CurrentCulture);
            }

            return !(count < 1);
        }

        /// <summary>
        ///   Converts a converted object back into it's original form.
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
        ///   The original object.
        /// </returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }

        #endregion

        #endregion
    }
}