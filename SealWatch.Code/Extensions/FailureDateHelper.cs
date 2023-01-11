namespace SealWatch.Code.Extensions;

internal static class FailureDateHelper
{
    public static DateTime GetFailureDate(int workDays, double millingPerDay, int lifeSpan, DateTime millingStart)
    {
        var currentDate = millingStart;
        var hoursPerWeek = workDays * millingPerDay;
        var weeksLeft = lifeSpan / hoursPerWeek;
        return currentDate.AddDays(weeksLeft * 7);
    }
}