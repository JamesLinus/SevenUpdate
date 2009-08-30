/*Copyright 2007-09 Robert Baker, aka Seven ALive.
This file is part of Seven Update.

    Seven Update is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    Seven Update is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with Seven Update.  If not, see <http://www.gnu.org/licenses/>.*/
using System;
using System.Windows.Data;
using System.Collections.ObjectModel;
using System.Windows.Media;

namespace SevenUpdate.Converters
{
    [ValueConversion(typeof(Brush), typeof(bool))]
    public class BrushToBoolConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (((Brush)value) == Brushes.Black)
                return true;
            else
                return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return new NotImplementedException();
        }

        #endregion
    }

    [ValueConversion(typeof(string), typeof(string))]
    public class StringToLocaleConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return App.RM.GetString(value.ToString());
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return new NotImplementedException();
        }

        #endregion
    }

    [ValueConversion(typeof(LocaleString), typeof(string))]
    public class LocaleStringConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            ObservableCollection<LocaleString> localeStrings = value as ObservableCollection<LocaleString>;

            for (int x = 0; x < localeStrings.Count; x++)
            {
                if (localeStrings[x].lang == Shared.Locale)
                    return localeStrings[x].Value;
            }

            return localeStrings[0].Value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return new NotImplementedException();
        }

        #endregion
    }

    [ValueConversion(typeof(UpdateFile), typeof(string))]
    public class FileSizeConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            ObservableCollection<UpdateFile> files = value as ObservableCollection<UpdateFile>;
            return Shared.ConvertFileSize(App.GetUpdateSize(files));
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return new NotImplementedException();
        }

        #endregion
    }

    [ValueConversion(typeof(UpdateStatus), typeof(bool))]
    public class EnumToBoolConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (((UpdateStatus)value) == UpdateStatus.Hidden)
                return false;
            else
                return true;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (((bool)value) == false)
                return UpdateStatus.Hidden;
            else
                return UpdateStatus.Visible;
        }

        #endregion
    }

    [ValueConversion(typeof(bool), typeof(string))]
    public class BoolToStringConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (((bool)value))
                return "x64";
            else
                return "x86";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return new NotImplementedException();
        }

        #endregion
    }
}
