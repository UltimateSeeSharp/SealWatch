using Bogus.DataSets;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace SealWatch.Wpf.Converter;

/// <summary>
/// Gets the MillingStop date to check if its already in the past.
/// When maintenance is needed it's colored red else green.
/// </summary>
[ValueConversion(typeof(DateTime), typeof(Brush))]
public class CutterAlreadyFailedForegroundConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if(value is not null && (DateTime)value >= DateTime.Now)
            return Brushes.Green;
           
        return Brushes.Red;
    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
        return DependencyProperty.UnsetValue;
    }
}