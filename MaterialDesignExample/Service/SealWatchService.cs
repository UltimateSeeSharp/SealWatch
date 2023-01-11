using SealWatch.Wpf.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;

namespace SealWatch.Wpf.Service;

public class SealWatchService : ISealMonitorService
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

    public void DrawSelected(List<Calendar> calendars, int workDays, double millingPerDay, double millingPerYear_y, int lifeSpan, DateTime millingStart)
    {
        var failureDates = GetFailureDates(workDays, millingPerDay, lifeSpan, millingStart);
        var firstFailureDate = failureDates.First(x => x.Date >= DateTime.Now);

        InitializeCalendars(firstFailureDate, calendars);

        foreach (var failureDate in failureDates)
        {
            var failureMonthCalendar = calendars.FirstOrDefault(x => x.DisplayDateStart!.Value.Year == failureDate.Year && x.DisplayDateStart.Value.Month == failureDate.Month);
            if (failureMonthCalendar is not null)
            {
                failureMonthCalendar.SelectedDate = failureDate;
            }
        }
    }

    public DateTime GetFailureDate(int workDays, double millingPerDay, int lifeSpan, DateTime millingStart)
    {
        var currentDate = millingStart;
        var hoursPerWeek = workDays * millingPerDay;
        var weeksLeft = lifeSpan / hoursPerWeek;
        return currentDate.AddDays(weeksLeft * 7);
    }

    public List<DateTime> GetFailureDates(int workDays, double millingPerDay, int lifeSpan, DateTime millingStart)
    {
        var dateList = new List<DateTime>();
        for (int x = 0; x < 20; x++)
        {
            var startDate = millingStart;
            if (dateList.Count > 0)
            {
                startDate = dateList.Last();
            }
            dateList.Add(GetFailureDate(workDays, millingPerDay, lifeSpan, startDate));
        }
        return dateList;
    }

    public double GetPercantage(DateTime start, DateTime stop)
    {
        var days = (stop - start).TotalDays;
        var daysLeft = (stop - DateTime.Now).TotalDays;
        var result = Math.Round((1 - (daysLeft / 100) / (days / 100)) * 100);
        if (result > 100)
        {
            Console.WriteLine();
        }
        return result;
    }

    public DateTime LastDayOfMonth(DateTime dateTime)
    {
        DateTime ss = new DateTime(dateTime.Year, dateTime.Month, 1);
        return ss.AddMonths(1).AddDays(-1);
    }
}