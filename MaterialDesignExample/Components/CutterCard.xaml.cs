using SealWatch.Code.CutterLayer;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace SealWatch.Wpf.Custom;

/// <summary>
/// Custom WPF component to show single Cutter including
/// it's properties and analytic properties
/// </summary>
public partial class CutterCard : UserControl
{
    public CutterCard()
    {
        InitializeComponent();
    }

    public static readonly DependencyProperty CutterProperty = DependencyProperty.Register("AnalysedCutter", typeof(AnalysedCutterDto), typeof(CutterCard));

    public AnalysedCutterDto Cutter
    {
        get => (AnalysedCutterDto)GetValue(CutterProperty);
        set => SetValue(CutterProperty, value);
    }

    public string DurabilityText => Cutter.Durability + " %";
}

/// <summary>
/// Adding time a suffix to something
/// e.g.: 2:03 => 2:03 Hours
/// </summary>
internal class AddHoursConverter : IValueConverter
{
    public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var input = value.ToString();
        if (input is null && string.IsNullOrEmpty(input)) return value;
        else
        {
            if (System.Convert.ToInt32(value) is 1)
            {
                return value.ToString() + " Stunde";
            }
            else
            {
                return value.ToString() + " Stunden";
            }
        }
    }

    public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var input = value.ToString();
        if (input is null && string.IsNullOrEmpty(input)) return value;
        else
        {
            return input.Split(' ')[0];
        }
    }
}

internal class AddYearsConverter : IValueConverter
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

internal class AddDaysConverter : IValueConverter
{
    public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var input = value.ToString();
        if (input is null && string.IsNullOrEmpty(input)) return value;
        else
        {
            if (System.Convert.ToInt32(value) is 1)
            {
                return value.ToString() + " Stunde";
            }
            else
            {
                return value.ToString() + " Stunden";
            }
        }
    }

    public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var input = value.ToString();
        if (input is null && string.IsNullOrEmpty(input)) return value;
        else
        {
            return input.Split(' ')[0];
        }
    }
}
