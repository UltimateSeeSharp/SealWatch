using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace SealWatch.Wpf.Converter;

/// <summary>
/// Determines the color of the CutterCard's progress bar.
/// Value equals Durability and should be between 0-100 for a working cutter.
/// Can exceed 100 if MillingStop is in the past => red color
/// </summary>
[ValueConversion(typeof(int), typeof(Brush))]
internal class DurabilityProgressColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (!(value is double number))
            return Brushes.Blue;

        return number switch
        {
            <= 70 => Brushes.LimeGreen,
            <= 80 => Brushes.Orange,
            <= 100 => Brushes.OrangeRed,
            _ => Brushes.Red
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return DependencyProperty.UnsetValue;
    }
}