using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace SevenUpdate.Sdk
{
    /// <summary>
    ///   Converts a <see cref = "LocaleString" /> to a localized string
    /// </summary>
    [ValueConversion(typeof(LocaleString), typeof(string))]
    public sealed class LocaleStringConverter : IValueConverter
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
            return localeStrings != null ? localeStrings.Where(t => t.Lang == Base.SelectedLocale).Select(t => t.Value).FirstOrDefault() : null;
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
                    localeStrings = Base.AppInfo.Name;
                    break;
                case "App.Publisher":
                    localeStrings = Base.AppInfo.Publisher;
                    break;
                case "App.Description":
                    localeStrings = Base.AppInfo.Description;
                    break;
                case "Update.Description":
                    localeStrings = Base.UpdateInfo.Description;
                    break;
                case "Update.Name":
                    localeStrings = Base.UpdateInfo.Name;
                    break;
            }




            if (localeStrings != null)
            {
                if (!String.IsNullOrEmpty(valueString))
                {
                    var found = false;

                    foreach (var t in localeStrings.Where(t => t.Lang == Base.SelectedLocale))
                    {
                        t.Value = valueString;
                        found = true;
                    }
                    if (!found)
                    {
                        var ls = new LocaleString {Lang = Base.SelectedLocale, Value = valueString};
                        localeStrings.Add(ls);
                    }
                }
                else
                {
                    for (int x = 0; x < localeStrings.Count; x++)
                    {
                        if (localeStrings[x].Lang == Base.SelectedLocale)
                            localeStrings.RemoveAt(x);
                    }
                }
            }
            else
            {
                if (!String.IsNullOrEmpty(valueString))
                {
                    localeStrings = new ObservableCollection<LocaleString>();
                    var ls = new LocaleString {Lang = Base.SelectedLocale, Value = valueString};
                    localeStrings.Add(ls);
                }
            }
            return localeStrings;
        }

        #endregion
    }

}
