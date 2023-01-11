using System;
using System.Globalization;
using System.Windows.Data;

namespace SealWatch.Wpf.Converter;

public class SerialNumberShortConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is null) return value!;
        if (value.ToString()!.Split('-').Length > 1)
        {
            return value.ToString()!.Split('-')[1];
        }
        return value!;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is null) return value!;
        return value.ToString();
    }
}