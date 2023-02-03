using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace SealWatch.Wpf.Converter;

/// <summary>
/// Colorizes order text. If ordered it's green.
/// If it's not yet ordered its orange.
/// </summary>
[ValueConversion(typeof(Boolean), typeof(Brush))]
public class OrderedNotOrderedConverterForeground : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not null && (bool)value) 
            return Brushes.Green;

        return Brushes.Orange;
    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
        return DependencyProperty.UnsetValue;
    }
}