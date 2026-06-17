namespace Catalog.API.Domain.Entities;

public sealed class Brand : BaseEntity<BrandId>
{
    public string BrandName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}
