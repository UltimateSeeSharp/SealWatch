using SealWatch.Wpf.Service.Interfaces;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;

namespace SealWatch.Wpf.Service;

/// <summary>
/// Provides functionality for the CalendarView
/// like initializing calendar with start date 
/// or draw multiple dates on calendars
/// </summary>
public class CalendarService : ICalendarService
{
    public void InitializeCalendars(DateTime startDate, List<Calendar> calendars)
    {
        if (calendars is null)
        {
            Log.Error("CalendarService - InitializeCalendars | Calendars were null");
            return;
        }
        if (calendars.Count is 0)
        {
            Log.Error("CalendarService - InitializeCalendars | Calendars had count 0");
            return;
        }

        for (int x = 0; x < calendars.Count; x++)
        {
            var nextDate = startDate.AddMonths(x);
            var startView = new DateTime(nextDate.Year, nextDate.Month, 1);
            var endView = new DateTime(nextDate.Year, nextDate.Month, GetLastDayOfMonth(nextDate).Day);

            calendars[x].SelectedDates.Clear();
            calendars[x].DisplayDateStart = startView;
            calendars[x].DisplayDateEnd = endView;
        }
    }

    /// <summary>
    /// Initializes the calendar from the current date.
    /// Draws selection at future maintenance dates
    /// </summary>
    /// <param name="calendars">Calendars from CalendarView. Shows next few years</param>
    /// <param name="failureDates">Next failure dates from a specific cutter</param>
    public void DrawSelected(List<Calendar> calendars, List<DateTime> failureDates)
    {
        InitializeCalendars(failureDates.First(), calendars);

        foreach (var failureDate in failureDates)
        {
            var failureMonthCalendar = calendars.FirstOrDefault(x => x.DisplayDateStart!.Value.Year == failureDate.Year && x.DisplayDateStart.Value.Month == failureDate.Month);
            if (failureMonthCalendar is not null)
            {
                failureMonthCalendar.SelectedDate = failureDate;
            }
        }
    }

    /// <summary>
    /// Gets the count of days in that year and month to create
    /// a DateTime with the last day of month
    /// </summary>
    /// <param name="date">Date to calculate last day of month</param>
    /// <returns></returns>
    public DateTime GetLastDayOfMonth(DateTime date) => new(
        date.Year, 
        date.Month, 
        DateTime.DaysInMonth(date.Year, date.Month));
}