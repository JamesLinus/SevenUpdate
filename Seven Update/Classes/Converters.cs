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
                if (localeStrings[x].lang == App.Locale)
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
