using SealWatch.Data.Model;
using SealWatch.Wpf.Service;
using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace SealWatch.Wpf.Custom;

public partial class CutterCard : UserControl
{
    private SealWatchService _sealWatchService = new();

    public CutterCard()
    {
        InitializeComponent();
    }

    public static readonly DependencyProperty CutterProperty = DependencyProperty.Register("Cutter", typeof(Cutter), typeof(CutterCard));

    public Cutter Cutter
    {
        get 
        { 
            var cutter = (Cutter)GetValue(CutterProperty);
            cutter.MillingStop = _sealWatchService.GetFailureDates(cutter.WorkDays, cutter.MillingPerDay_h, cutter.LifeSpan_h, cutter.MillingStart).First();
            return cutter;
        }
        set { SetValue(CutterProperty, value); }
    }

    public int CutterDurability
    {
        get
        {
            var totalDays1Percent = (Cutter.MillingStop - Cutter.MillingStart).TotalDays / 100;
            var daysPassed = (DateTime.Now - Cutter.MillingStart).TotalDays;
            var pace = daysPassed / totalDays1Percent;
           
            if (Cutter.MillingStart.Year == 2018)
            {
                Console.WriteLine();
            }

            return (int)Math.Round(pace);     
        }
    }

    public string CutterDurabilityText
    {
        get => CutterDurability.ToString() + " %";
    }
}

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
