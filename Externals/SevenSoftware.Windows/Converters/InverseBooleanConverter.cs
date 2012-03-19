// <copyright file="InverseBooleanConverter.cs" project="SevenSoftware.Windows">Robert Baker</copyright>
// <license href="http://www.gnu.org/licenses/gpl-3.0.txt" name="GNU General Public License 3" />

namespace SevenSoftware.Windows.Converters
{
    using System;
    using System.Globalization;
    using System.Windows.Data;

    /// <summary>Converts a bool value to the opposite value.</summary>
    [ValueConversion(typeof(bool), typeof(bool))]
    public class InverseBooleanConverter : IValueConverter
    {
        /// <summary>Converts a object into another object.</summary>
        /// <param name="value">The value produced by the binding source.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>The converted object.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return !System.Convert.ToBoolean(value, CultureInfo.CurrentCulture);
        }

        /// <summary>Converts a converted object back into it's original form.</summary>
        /// <param name="value">The value that is produced by the binding target.</param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>The original object.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return !System.Convert.ToBoolean(value, CultureInfo.CurrentCulture);
        }
    }
}