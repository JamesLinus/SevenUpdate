// Copyright 2007-2010 Robert Baker, Seven Software.
// This file is part of Seven Update.
// Seven Update is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
// Seven Update is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.
// You should have received a copy of the GNU General Public License along with Seven Update.  If not, see <http://www.gnu.org/licenses/>.

namespace SevenUpdate.Sdk
{
    using System;
    using System.Collections.ObjectModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Linq;
    using System.Windows.Data;

    /// <summary>
    /// Converts a <see cref="LocaleString"/> to a localized string
    /// </summary>
    [ValueConversion(typeof(LocaleString), typeof(string))]
    internal class StringToLocaleStringConverter : IValueConverter
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
            var localeStrings = value as Collection<LocaleString>;

            // Loops through the collection of LocaleStrings
            return localeStrings != null ? localeStrings.Where(t => t.Lang == Base.Locale).Select(t => t.Value).FirstOrDefault() : null;
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
            var valueString = value as string;

            ObservableCollection<LocaleString> localeStrings = null;
            switch (parameter as string)
            {
                case "App.Name":
                    localeStrings = Core.AppInfo.Name;
                    break;
                case "App.Publisher":
                    localeStrings = Core.AppInfo.Publisher;
                    break;
                case "App.Description":
                    localeStrings = Core.AppInfo.Description;
                    break;
                case "Update.Description":
                    localeStrings = Core.UpdateInfo.Description;
                    break;
                case "Update.Name":
                    localeStrings = Core.UpdateInfo.Name;
                    break;
                case "Shortcut.Name":
                    localeStrings = Core.UpdateInfo.Shortcuts[Core.SelectedShortcut].Name;
                    break;
                case "Shortcut.Description":
                    localeStrings = Core.UpdateInfo.Shortcuts[Core.SelectedShortcut].Description;
                    break;
            }

            if (localeStrings != null)
            {
                if (!String.IsNullOrWhiteSpace(valueString))
                {
                    var found = false;

                    foreach (var t in localeStrings.Where(t => t.Lang == Base.Locale))
                    {
                        t.Value = valueString;
                        found = true;
                    }

                    if (!found)
                    {
                        var ls = new LocaleString { Lang = Base.Locale, Value = valueString };
                        localeStrings.Add(ls);
                    }
                }
                else
                {
                    for (var x = 0; x < localeStrings.Count; x++)
                    {
                        if (localeStrings[x].Lang == Base.Locale)
                        {
                            localeStrings.RemoveAt(x);
                        }
                    }
                }
            }
            else
            {
                if (!String.IsNullOrWhiteSpace(valueString))
                {
                    localeStrings = new ObservableCollection<LocaleString>();
                    var ls = new LocaleString { Lang = Base.Locale, Value = valueString };
                    localeStrings.Add(ls);
                }
            }

            return localeStrings;
        }

        #endregion

        #endregion
    }

    /// <summary>
    /// Converts the string to a <see cref="DateTime"/>
    /// </summary>
    [SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Converter")]
    [ValueConversion(typeof(DateTime), typeof(string))]
    internal class DateConverter : IValueConverter
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
            return value != null ? DateTime.Parse(value.ToString(), CultureInfo.CurrentCulture) : DateTime.Now;
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
        /// A converted value. If the method returns <see langword="null"/>, the valid <see langword="null"/> value is used.
        /// </returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value == null ? DateTime.Now.ToShortDateString() : ((DateTime)value).ToShortDateString();
        }

        #endregion

        #endregion
    }

    /// <summary>
    /// Converts a Boolean to a Label
    /// </summary>
    [ValueConversion(typeof(bool), typeof(string))]
    internal class BoolToLabelConverter : IValueConverter
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