namespace Catalog.API.Domain.Common;

using System.Text.Json.Serialization;

public abstract class BaseEntity
{
    public int Id { get; set; }
}

public abstract class BaseEntity<TId>
{
    public TId Id { get; set; } = default(TId)!;
}