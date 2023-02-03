using SealWatch.Code.CutterLayer;
using Serilog;

namespace SealWatch.Code.Services;

public class AnalyseService
{
    public double CalcDurability(DateTime start, DateTime stop)
    {
        var totalDays1Perc = (stop - start).TotalDays / 100;
        var passedDays = (DateTime.Now - start).TotalDays;

        var pace = passedDays / totalDays1Perc;
        var result = Math.Round(pace);

        if (result < 0 || result > 101)
            Log.Error($"Calculation durability failed | Smaller 0 or Bigger 101");

        return result;
    }

    public int CalcDaysLeft(DateTime millingStop)
    {
        var daysToStop = (millingStop - DateTime.Now).TotalDays;

        if (daysToStop < 0)
            Log.Error($"Calculation days left failed | Smaller 0");

        return (int)Math.Ceiling(daysToStop);
    }

    public DateTime CalcFailureDate(DateTime millingStart, int workDays, double millingPerDay, double lifespan)
    {
        var hoursPerWeek = workDays * millingPerDay;
        var weeksLeft = lifespan / hoursPerWeek;
        var millingStop = millingStart.AddDays(weeksLeft * 7);

        if (millingStop < millingStart)
            Log.Error($"Calculation failure date failed | milling stop before milling start - start:{millingStart}, workDays:{workDays}, millingPerDay:{millingPerDay}, lifespan:{lifespan}, stop:{millingStop}");

        return millingStop;
    }

    public List<DateTime> GetFailureDates(AnalysedCutterDto cutter)
    {
        List<DateTime> failureDates = new();
        DateTime endDate = cutter.MillingStart.AddYears(Convert.ToInt32(cutter.MillingDuration_y));

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