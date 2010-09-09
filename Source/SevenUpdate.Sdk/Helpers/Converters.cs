#region

using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

#endregion

namespace SevenUpdate.Sdk
{
    /// <summary>
    ///   Converts a <see cref = "LocaleString" /> to a localized string
    /// </summary>
    [ValueConversion(typeof (LocaleString), typeof (string))]
    internal sealed class StringToLocaleStringConverter : IValueConverter
    {
        #region IValueConverter Members

        /// <summary>
        ///   Converts a object into another object
        /// </summary>
        /// <returns>the converted object</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var localeStrings = value as Collection<LocaleString>;

            // Loops through the collection of LocaleStrings
            return localeStrings != null ? localeStrings.Where(t => t.Lang == Base.Locale).Select(t => t.Value).FirstOrDefault() : null;
        }

        /// <summary>
        ///   Converts a converted object back into it's original form
        /// </summary>
        /// <returns>The original object</returns>
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
                        var ls = new LocaleString {Lang = Base.Locale, Value = valueString};
                        localeStrings.Add(ls);
                    }
                }
                else
                {
                    for (var x = 0; x < localeStrings.Count; x++)
                    {
                        if (localeStrings[x].Lang == Base.Locale)
                            localeStrings.RemoveAt(x);
                    }
                }
            }
            else
            {
                if (!String.IsNullOrWhiteSpace(valueString))
                {
                    localeStrings = new ObservableCollection<LocaleString>();
                    var ls = new LocaleString {Lang = Base.Locale, Value = valueString};
                    localeStrings.Add(ls);
                }
            }
            return localeStrings;
        }

        #endregion
    }

    /// <summary>
    ///   Converts the string to a DateTime
    /// </summary>
    [ValueConversion(typeof (DateTime), typeof (string))]
    internal sealed class DateConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value != null ? DateTime.Parse(value.ToString()) : DateTime.Now;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value == null ? DateTime.Now.ToShortDateString() : ((DateTime) value).ToShortDateString();
        }

        #endregion
    }

    /// <summary>
    ///   Converts a Bool to a Label
    /// </summary>
    [ValueConversion(typeof (bool), typeof (string))]
    internal sealed class BoolToLabelConverter : IValueConverter
    {
        #region IValueConverter Members

        /// <summary>
        ///   Converts a object into another object
        /// </summary>
        /// <returns>the converted object</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool) value ? @"HKLM\Software\MyCompany\MyApp" : @"%PROGRAMFILES%\Seven Software\Seven Update";
        }

        /// <summary>
        ///   Converts a converted object back into it's original form
        /// </summary>
        /// <returns>The original object</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}