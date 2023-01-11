using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace SealWatch.Wpf.Converter;

[ValueConversion(typeof(Boolean), typeof(Brush))]
public class OrderedNotOrderedConverterForeground : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is null) return Brushes.Red;
        if ((bool)value)
        {
            return Brushes.Green;
        }
        return Brushes.Orange;
    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
        if (value is null) return false;
        if ((Brush)value == Brushes.Green)
        {
            return true;
        }
        return false;
    }
}