using System;
using System.Globalization;
using System.Windows.Data;

namespace SealWatch.Main.Converter;

public class AddYearsConverter : IValueConverter
{
    public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var input = value.ToString();
        if (input is null && string.IsNullOrEmpty(input)) return value;
        else
        {
            if (System.Convert.ToInt32(value) is 1)
            {
                return value.ToString() + " Jahr";
            }
            else
            {
                return value.ToString() + " Jahre";
            }
        }
    }

    public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var input = value.ToString();
        if (input is null && string.IsNullOrEmpty(input))
        {
            return value;
        }
        else
        {
            return input.Split(' ')[0];
        }
    }
}
