namespace Catalog.API.Web.API.Mappers;

using Catalog.API.Application.Features.Products;

[Mapper]
internal sealed partial class ProductMapper
{
    [MapperIgnoreTarget(nameof(Product.Id))]
    [MapperIgnoreTarget(nameof(Product.Brand))]
    public partial Product MapAddProductRequestToProduct(AddProductRequest request);
}