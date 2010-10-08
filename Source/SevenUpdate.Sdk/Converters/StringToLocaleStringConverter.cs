// ***********************************************************************
// <copyright file="StringToLocaleStringConverter.cs"
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
    using System.Collections.ObjectModel;
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
            return localeStrings != null ? localeStrings.Where(t => t.Lang == Utilities.Locale).Select(t => t.Value).FirstOrDefault() : null;
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

                    foreach (var t in localeStrings.Where(t => t.Lang == Utilities.Locale))
                    {
                        t.Value = valueString;
                        found = true;
                    }

                    if (!found)
                    {
                        var ls = new LocaleString { Lang = Utilities.Locale, Value = valueString };
                        localeStrings.Add(ls);
                    }
                }
                else
                {
                    for (var x = 0; x < localeStrings.Count; x++)
                    {
                        if (localeStrings[x].Lang == Utilities.Locale)
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
                    var ls = new LocaleString { Lang = Utilities.Locale, Value = valueString };
                    localeStrings.Add(ls);
                }
            }

            return localeStrings;
        }

        #endregion

        #endregion
    }
}