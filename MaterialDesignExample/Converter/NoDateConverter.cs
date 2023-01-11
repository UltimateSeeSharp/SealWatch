using System;
using System.Globalization;
using System.Windows.Data;

namespace SealWatch.Wpf.Converter;

public class NoDateConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if ((DateTime)value == DateTime.MinValue)
            return new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
        else
            return value;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if ((DateTime)value == new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1))
            return DateTime.MinValue;
        else
            return value;
    }
}
