using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace SealWatch.Wpf.Converter;

/// <summary>
/// Converts boolean to type visibility
/// </summary>
[ValueConversion(typeof(bool), typeof(Visibility))]
public class BoolToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not null && (bool)value)
            return Visibility.Visible;

        return Visibility.Hidden;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return DependencyProperty.UnsetValue;
    }
}
