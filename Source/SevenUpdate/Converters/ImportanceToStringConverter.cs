// <copyright file="ImportanceToStringConverter.cs" project="SevenUpdate">Robert Baker</copyright>
// <license href="http://www.gnu.org/licenses/gpl-3.0.txt" name="GNU General Public License 3" />

namespace SevenUpdate.Converters
{
    using System;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Data;

    using SevenUpdate.Properties;

    /// <summary>Converts Importance to a localized string.</summary>
    [ValueConversion(typeof(Importance), typeof(string))]
    internal sealed class ImportanceToStringConverter : IValueConverter
    {
        /// <summary>Converts a object into another object.</summary>
        /// <param name="value">The value produced by the binding source.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>The converted object.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch (value is Importance ? (Importance)value : Importance.Important)
            {
                case Importance.Important:
                    return Resources.Important;
                case Importance.Recommended:
                    return Core.Settings.IncludeRecommended ? Resources.Important : Resources.Recommended;
                case Importance.Optional:
                    return Resources.Optional;

                case Importance.Locale:
                    return Resources.Locale;

                default:
                    return Resources.Important;
            }
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