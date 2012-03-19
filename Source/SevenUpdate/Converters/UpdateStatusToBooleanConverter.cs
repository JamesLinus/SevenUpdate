// <copyright file="UpdateStatusToBooleanConverter.cs" project="SevenUpdate">Robert Baker</copyright>
// <license href="http://www.gnu.org/licenses/gpl-3.0.txt" name="GNU General Public License 3" />

namespace SevenUpdate.Converters
{
    using System;
    using System.Globalization;
    using System.Windows.Data;

    /// <summary>Converts the update status to a Boolean value.</summary>
    [ValueConversion(typeof(UpdateStatus), typeof(bool))]
    internal sealed class UpdateStatusToBooleanConverter : IValueConverter
    {
        /// <summary>Converts a object into another object.</summary>
        /// <param name="value">The value produced by the binding source.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>The converted object.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // If the UpdateStatus is hidden return False, otherwise return true
            return ((UpdateStatus)value) != UpdateStatus.Hidden;
        }

        /// <summary>Converts a converted object back into it's original form.</summary>
        /// <param name="value">The value that is produced by the binding target.</param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>The original object.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((bool)value) == false ? UpdateStatus.Hidden : UpdateStatus.Visible;
        }
    }
}