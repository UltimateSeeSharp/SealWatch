using SealWatch.Data.Extensions;
using System.ComponentModel.DataAnnotations;

namespace SealWatch.Data.Model;

public class Cutter : Entity
{
    [Required]
    public int ProjectId { get; set; }

    [Required]
    public string SerialNumber { get; set; }

    [Required]
    public DateTime MillingStart { get; set; }

    [Required]
    public int WorkDays { get; set; }

    [Required]
    public double MillingPerDay_h { get; set; }

    [Required]
    public double MillingDuration_y { get; set; }

    public DateTime MillingStop { get; set; }

    public bool SealOrdered { get; set; }

    public int LifeSpan_h { get; set; }
}

