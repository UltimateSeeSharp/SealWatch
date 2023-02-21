using SealWatch.Code.CutterLayer;

namespace SealWatch.Code.Services.Interface;

public interface IAnalyseService
{
    double CalcDurability(DateTime millingStart, DateTime millingStop, DateTime now, int accuracy = 0);
    DateTime CalcFailureDate(DateTime millingStart, int workDays, double millingPerDay, double lifespan);
    double CalcRelativeTimeInDays(DateTime millingStop, DateTime now, int accuracy = 0);
    List<DateTime> GetFailureDates(AnalysedCutterDto cutter);
}