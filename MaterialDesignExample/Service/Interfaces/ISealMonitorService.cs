using System;
using System.Collections.Generic;
using System.Windows.Controls;

namespace SealWatch.Wpf.Service.Interfaces;

public interface ISealMonitorService
{
    void DrawSelected(List<Calendar> calendars, int workDays, double millingPerDay, double millingPerYear_years, int lifeSpan, DateTime millingStart);
    DateTime GetFailureDate(int workDays, double millingPerDay, int lifeSpan, DateTime millingStart);

    List<DateTime> GetFailureDates(int workDays, double millingPerDay, int lifeSpan, DateTime millingStart);

    double GetPercantage(DateTime start, DateTime stop);
    void InitializeCalendars(DateTime date, List<Calendar> calendars);
    DateTime LastDayOfMonth(DateTime dateTime);
}