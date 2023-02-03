using SealMonitoring.Code.Infrastructure;

namespace SealWatch.Code.CutterLayer
{
    public class CutterEditDto : BaseListDto
    {
        public int ProjectId { get; set; }

        public string SerialNumber { get; set; }

        public DateTime MillingStart { get; set; }

        public int WorkDays { get; set; }

        public double MillingPerDay_h { get; set; }

        public double MillingDuration_y { get; set; }

        public DateTime MillingStop { get; set; }

        public bool SealOrdered { get; set; }

        public int LifeSpan_h { get; set; }
    }
}
