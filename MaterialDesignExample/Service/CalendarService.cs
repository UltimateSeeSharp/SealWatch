using SealWatch.Wpf.Service.Interfaces;
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
    public void InitializeCalendars(DateTime date, List<Calendar> calendars)
    {
        for (int x = 0; x < calendars.Count; x++)
        {
            var nextDate = date.AddMonths(x);
            var startView = new DateTime(nextDate.Year, nextDate.Month, 1);
            var endView = new DateTime(nextDate.Year, nextDate.Month, LastDayOfMonth(nextDate).Day);

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

    public DateTime LastDayOfMonth(DateTime dateTime)
    {
        DateTime ss = new DateTime(dateTime.Year, dateTime.Month, 1);
        return ss.AddMonths(1).AddDays(-1);
    }
}