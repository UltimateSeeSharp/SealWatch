using SealWatch.Wpf.ViewModels;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace SealWatch.Wpf.Views;

public partial class CalendarPlanerView : UserControl
{
    public CalendarPlanerView(CalendarPlanerViewModel calendarPlanerViewModel)
    {
        InitializeComponent();
        DataContext = calendarPlanerViewModel;
    }

    private void UserControl_Loaded(object sender, RoutedEventArgs e)
    {
        (DataContext as CalendarPlanerViewModel)!.Loaded(new List<Calendar>()
        {
            cal_11, cal_21, cal_31, cal_41, cal_51,
            cal_13, cal_23, cal_33, cal_43, cal_53,
            cal_15, cal_25, cal_35, cal_45, cal_55,
            cal_17, cal_27, cal_37, cal_47, cal_57,
        });
    }
}