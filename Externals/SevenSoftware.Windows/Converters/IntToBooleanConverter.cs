// <copyright file="IntToBooleanConverter.cs" project="SevenSoftware.Windows">Robert Baker</copyright>
// <license href="http://www.gnu.org/licenses/gpl-3.0.txt" name="GNU General Public License 3" />

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace SevenSoftware.Windows.Converters
{
    /// <summary>Converts the Int to Bool.</summary>
    [ValueConversion(typeof(int), typeof(bool))]
    public class IntToBooleanConverter : IValueConverter
    {
        /// <summary>Converts a object into another object.</summary>
        /// <param name="value">The value produced by the binding source.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>The converted object.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            ulong count = System.Convert.ToUInt64(value, CultureInfo.CurrentCulture);

            if (parameter != null)
            {
                // If count is less then 1 and should return visible
                return count < 1 && System.Convert.ToBoolean(parameter, CultureInfo.CurrentCulture);
            }

            return !(count < 1);
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
    }
}