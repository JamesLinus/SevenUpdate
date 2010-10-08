// ***********************************************************************
// <copyright file="FileSizeConverter.cs"
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
    using System.Collections.ObjectModel;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Data;

    /// <summary>
    /// Converts a ulong or group of ulong values into a string readable file size
    /// </summary>
    [ValueConversion(typeof(UpdateFile), typeof(string))]
    internal sealed class FileSizeConverter : IValueConverter
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
            try
            {
                var files = value as Collection<UpdateFile>;

                // Gets the full size of the update then converts it into a string format
                return Utilities.ConvertFileSize(Core.GetUpdateSize(files));
            }
            catch (Exception)
            {
                var size = System.Convert.ToUInt64(value, CultureInfo.CurrentCulture);

                // Converts the ulong into a readable file size string
                return Utilities.ConvertFileSize(size);
            }
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