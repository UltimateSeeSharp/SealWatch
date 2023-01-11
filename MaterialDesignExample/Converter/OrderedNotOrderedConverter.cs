using System;
using System.Windows.Data;

namespace SealWatch.Wpf.Converter;

public class OrderedNotOrderedConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
        var data = (bool)value;
        if (data)
        {
            return "Bestellt";
        }
        else if (!data)
        {
            return "Zu bestellen";
        }
         return "Unbekannt";
    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
        var data = (string)value;
        if (data == "Bestellt")
        {
            return true;
        }
        return false;
    }
}