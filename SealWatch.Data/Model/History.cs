using SealWatch.Data.Extensions;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SealWatch.Data.Model;

public class History : Entity
{

    [Required]
    [MaxLength(50)]
    public Guid ReferenceGuid { get; set; }

    [Required]
    public string ReferenceId { get; set; }

    [Required]
    public string Property { get; set; }

    public string OldValue { get; set; }
    public string NewValue { get; set; }

    [Required]
    public string ChangeUser { get; set; }

    [Required]
    [Column(TypeName = "datetime2(7)")]
    public DateTime ChangeDate { get; set; }
}