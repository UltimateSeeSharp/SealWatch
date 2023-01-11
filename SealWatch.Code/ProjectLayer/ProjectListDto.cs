using SealMonitoring.Code.Infrastructure;
using SealWatch.Data.Model;

namespace SealWatch.Code.ProjectLayer;

public class ProjectListDto : BaseListDto
{
   
    public string Location { get; set; }

    public int Blades { get; set; }

    public int SlitDepth_m { get; set; }

    public DateTime StartDate { get; set; }

    public List<Cutter>? Cutters { get; set; }

    public bool IsDeleted { get; set; }

    public bool IsDone { get; set; }
}