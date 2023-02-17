using System;
using System.Collections.Generic;
using System.Windows.Controls;

namespace SealWatch.Wpf.Service.Interfaces;

public interface ICalendarService
{
    void DrawSelected(List<Calendar> calendars, List<DateTime> failureDates);
    void InitializeCalendars(DateTime date, List<Calendar> calendars);
    DateTime GetLastDayOfMonth(DateTime dateTime);
}