using System.ComponentModel.DataAnnotations;

namespace SealWatch.Data.Extensions;

public interface IAuditable : IEntity
{
    [Required]
    DateTime CreateDate { get; set; }
    DateTime? DeleteDate { get; set; }

    [Required]
    string CreateUser { get; set; }
    string DeleteUser { get; set; }

    DateTime? ChangeDate { get; set; }
    string ChangeUser { get; set; }

    bool IsDeleted { get; set; }
    bool IsDone { get; set; }
}

public abstract class BaseAuditableEntity<T> : BaseEntity<T>, IAuditable
{
    [Required]
    public DateTime CreateDate { get; set; }
    public DateTime? DeleteDate { get; set; }

    [Required]
    public string CreateUser { get; set; }
    public string? DeleteUser { get; set; }

    public DateTime? ChangeDate { get; set; }
    public string? ChangeUser { get; set; }

    public bool IsDeleted { get; set; }
    public bool IsDone { get; set; }
}

public abstract class AuditableEntity : BaseAuditableEntity<int>
{

}
