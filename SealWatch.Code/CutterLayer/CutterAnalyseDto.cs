using SealMonitoring.Code.Infrastructure;

namespace SealWatch.Code.CutterLayer;

public class CutterAnalyseDto : BaseListDto
{
    public string Serialnumber { get; set; }

    public string Location { get; set; }

    public DateTime MillingStart { get; set; }

    public DateTime MillingStop { get; set; }

    public int DaysLeft { get; set; }

    public double Durability { get; set; }

    public bool SealOrdered { get; set; }

    public double MillingPerDay_h { get; set; }

    public double MillingDuration_y { get; set; }

    public int LifeSpan_h { get; set; }

    public int WorkDays { get; set; }
}