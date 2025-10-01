namespace Catalog.API.Domain.Entities;

public class Product : BaseEntity<ProductId>
{
    public BrandId BrandId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public Uri? PictureUri { get; set; }
    public Brand? Brand { get; set; } = new ();
}