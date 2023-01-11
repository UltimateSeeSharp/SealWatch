using System.ComponentModel.DataAnnotations;

namespace SealWatch.Data.Extensions;

public interface IEntity
{

}

public abstract class BaseEntity<T> : IEntity
{
    [Key]
    public T Id { get; set; }

    public override string ToString() => $"{GetType().Name}, Id = {Id}";
}

public abstract class Entity : BaseEntity<int>
{

}