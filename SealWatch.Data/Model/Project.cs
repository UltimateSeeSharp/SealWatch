using SealWatch.Data.Extensions;
using System.ComponentModel.DataAnnotations;

namespace SealWatch.Data.Model;

public class Project : AuditableEntity
{
    [Required]
    public string Location { get; set; }

    [Required]
    public int Blades { get; set; }

    [Required]
    public int SlitDepth_m { get; set; }

    [Required]
    public DateTime StartDate { get; set; }

    [Required]
    public List<Cutter> Cutters { get; set; } = new();
}
