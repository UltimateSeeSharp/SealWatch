﻿using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace SealWatch.Wpf.Converter;

[ValueConversion(typeof(bool), typeof(Visibility))]
public class BoolToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if ((bool)value)
            return Visibility.Visible;
        else
            return Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        string strValue = value as string;
        if (Enum.TryParse<Visibility>(strValue, out Visibility result))
        {
            return result;
        }
        return DependencyProperty.UnsetValue;
    }
}
