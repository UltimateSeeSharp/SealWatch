using SealMonitoring.Code.Infrastructure;

namespace SealWatch.Code.CutterLayer
{
    public class CutterEditDto : BaseListDto
    {
        public CutterEditDto()
        {
            //  That default in adding window is not 01.01.0001.
            MillingStart = new(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
        }

        public int ProjectId { get; set; }

        public string SerialNumber { get; set; }

        public DateTime MillingStart { get; set; }

        public int WorkDays { get; set; }

        public double MillingPerDay_h { get; set; }

        public double MillingDuration_y { get; set; }

        public string SoilType { get; set; }

        public DateTime MillingStop { get; set; }

        public bool SealOrdered { get; set; }

        public int LifeSpan_h { get; set; }
    }
}
