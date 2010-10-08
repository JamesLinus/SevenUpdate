// ***********************************************************************
// <copyright file="BooleanToLabelConverter.cs"
//            project="SevenUpdate.Sdk"
//            assembly="SevenUpdate.Sdk"
//            solution="SevenUpdate"
//            company="Seven Software">
//     Copyright (c) Seven Software. All rights reserved.
// </copyright>
// <author username="sevenalive">Robert Baker</author>
// ***********************************************************************
namespace SevenUpdate.Sdk.Converters
{
    using System;
    using System.Globalization;
    using System.Windows.Data;

    /// <summary>
    /// Converts a Boolean to a Label
    /// </summary>
    [ValueConversion(typeof(bool), typeof(string))]
    internal class BooleanToLabelConverter : IValueConverter
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
            return (bool)value ? @"HKLM\Software\MyCompany\MyApp" : @"%PROGRAMFILES%\Seven Software\Seven Update";
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
            throw new NotImplementedException();
        }

        #endregion

        #endregion
    }
}