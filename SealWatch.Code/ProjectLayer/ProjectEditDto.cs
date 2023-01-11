using SealMonitoring.Code.Infrastructure;
using SealWatch.Data.Model;

namespace SealWatch.Code.ProjectLayer;

public class ProjectEditDto : BaseListDto
{
    public string Location { get; set; }

    public int Blades { get; set; }

    public int SlitDepth_m { get; set; }

    public DateTime StartDate { get; set; } = DateTime.Now;

    public List<Cutter> Cutters { get; set; } = new();
}