using SealMonitoring.Code.Infrastructure;
using SealWatch.Data.Model;

namespace SealWatch.Code.ProjectLayer;

public class ProjectDetailDto : BaseListDto
{
    public string Location { get; set; }

    public int Blades { get; set; }

    public int SlitDepth_m { get; set; }

    public DateTime StartDate { get; set; }

    public List<Cutter> Cutters { get; set; }


    public DateTime CreateDate { get; set; }

    public DateTime? DeleteDate { get; set; }

    public string CreateUser { get; set; }

    public string DeleteUser { get; set; }

    public DateTime? ChangeDate { get; set; }

    public string ChangeUser { get; set; }


    public bool IsDeleted { get; set; }

    public bool IsDone { get; set; }
}