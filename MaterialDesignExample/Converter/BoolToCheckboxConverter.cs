using System;
using System.Globalization;
using System.Windows.Data;

namespace SealWatch.Wpf.Converter;

public class BoolToCheckboxConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if ((bool)value) return value;
        return null!;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is null) return false;
        return true;
    }
}
