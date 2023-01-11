using SealMonitoring.Code.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SealWatch.Code.CutterLayer
{
    public class CutterEditDto : BaseListDto
    {
        public CutterEditDto()
        {
            SerialNumber = String.Empty;
            MillingStart = DateTime.Now;
            LifeSpan_h = 600;
        }

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
