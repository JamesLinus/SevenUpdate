// ***********************************************************************
// <copyright file="FileSizeConverter.cs" project="SevenUpdate" assembly="SevenUpdate" solution="SevenUpdate" company="Seven Software">
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
// <summary>
// Converts a ulong or group of ulong values into a string readable file size
// .</summary>
// ***********************************************************************

namespace SevenUpdate.Converters
{
    using System;
    using System.Collections.ObjectModel;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Data;

    /// <summary>Converts a ulong or group of ulong values into a string readable file size.</summary>
    [ValueConversion(typeof(UpdateFile), typeof(string))]
    internal sealed class FileSizeConverter : IValueConverter
    {
        #region Implemented Interfaces

        #region IValueConverter

        /// <summary>Converts a object into another object.</summary>
        /// <param name="value">The value produced by the binding source.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>The converted object.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var files = value as Collection<UpdateFile>;
            if (files != null)
            {
                // Gets the full size of the update then converts it into a string format
                return Utilities.ConvertFileSize(Core.GetUpdateSize(files));
            }

            var size = System.Convert.ToUInt64(value, CultureInfo.CurrentCulture);

            // Converts the ulong into a readable file size string
            return Utilities.ConvertFileSize(size);
        }

        /// <summary>Converts a converted object back into it's original form.</summary>
        /// <param name="value">The value that is produced by the binding target.</param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>The original object.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }

        #endregion

        #endregion
    }
}