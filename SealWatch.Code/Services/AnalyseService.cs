﻿using SealWatch.Code.CutterLayer;
using Serilog;

namespace SealWatch.Code.Services;

public class AnalyseService
{
    /// <summary>
    /// Calculates durability of a cutter on a basis of 0-100
    /// 0 if unused / 100+ if used over maintenance date
    /// </summary>
    /// <param name="start">Day at which the cutter starts milling</param>
    /// <param name="stop">Day at which the cutter stops milling - new seal is needed</param>
    /// <param name="accuracy">Accuracy of decimal places</param>
    /// <returns></returns>
    public double CalcDurability(DateTime start, DateTime stop, DateTime currentDate, int accuracy = 0)
    {
        if (start >= currentDate)
            return 0;

        var totalDays1Perc = (stop - start).TotalDays / 100;
        var passedDays = (currentDate - start).TotalDays;

        var pace = passedDays / totalDays1Perc;
        var result = Math.Round(pace, accuracy);

        return result;
    }

    /// <summary>
    /// Calculates days to next maintenance.
    /// Can be negative if maintenance day is in the past.
    /// </summary>
    /// <param name="millingStop">Day at which the cutter stops milling - new seal is needed</param>
    /// <param name="accuracy">Accuracy of decimal places</param>
    /// <returns></returns>
    public double CalcRelativeTimeInDays(DateTime millingStop, DateTime currentDate, int accuracy = 0)
    {
        var daysToStop = (millingStop - currentDate).TotalDays;
        return Math.Round(daysToStop, accuracy);
    }

    /// <summary>
    /// Calculates next maintenance date for specific cutter values.
    /// Can generate valid dates in the past.
    /// </summary>
    /// <param name="millingStart">Day at which the cutter starts milling</param>
    /// <param name="workDays">Days per week of which are workdays</param>
    /// <param name="millingPerDay">Hours per day of which are work hours</param>
    /// <param name="lifespan">Lifespan of the cutter seals in hours</param>
    /// <returns></returns>
    public DateTime CalcFailureDate(DateTime millingStart, int workDays, double millingPerDay, double lifespan)
    {
        DateTime start = millingStart;

        while (lifespan > 0 && lifespan > millingPerDay * workDays)
        {
            lifespan -= (millingPerDay * workDays);
            start = start.AddDays(7);
        }

        if (lifespan is not 0)
        {
            var left = lifespan / millingPerDay;
            start = start.AddDays(left);
        }

        return start;
    }

    /// <summary>
    /// Calculates next maintenance dates for specific cutter values.
    /// </summary>
    /// <param name="cutter">Cutter of which the all maintenance dates for its whole life are calculated</param>
    /// <returns></returns>
    public List<DateTime> GetFailureDates(AnalysedCutterDto cutter)
    {
        List<DateTime> failureDates = new();
        DateTime endDate = cutter.MillingStart.AddMonths((int)(cutter.MillingDuration_y * 12));

        do
        {
            var startDate = failureDates.Count == 0 ? cutter.MillingStart : failureDates.Last();
            var failureDate = CalcFailureDate(startDate, cutter.WorkDays, cutter.MillingPerDay_h, cutter.LifeSpan_h);
            failureDates.Add(failureDate);
        }
        while (failureDates.Last() < endDate);

        return failureDates;
    }
}