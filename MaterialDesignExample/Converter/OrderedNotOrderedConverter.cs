using System;
using System.Windows;
using System.Windows.Data;

namespace SealWatch.Wpf.Converter;

/// <summary>
/// Checks if the order status and return a string
/// which gets represented in the data grid.
/// </summary>
[ValueConversion(typeof(Boolean), typeof(String))]
public class OrderedNotOrderedConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
        if (value is null)
            return String.Empty;

        bool isOrdered;
        bool.TryParse(value.ToString(), out isOrdered);

        return isOrdered ? "Bestellt" : "Zu bestellen";
    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
        return DependencyProperty.UnsetValue;
    }
}