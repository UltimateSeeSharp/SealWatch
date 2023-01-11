using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace SealWatch.Wpf.Converter;

[ValueConversion(typeof(int), typeof(Brush))]
internal class DurabilityProgressColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return (int)value switch
        {
            <= 70 => Brushes.LimeGreen,
            <= 80 => Brushes.Orange,
            <= 100 => Brushes.Red,
            _ => Brushes.DeepPink
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return DependencyProperty.UnsetValue;
    }
}