namespace Catalog.API.Web.API.Mappers;

using Catalog.API.Web.API.Endpoints.Requests;

[Mapper]
public partial class ProductMapper
{
    [MapperIgnoreTarget(nameof(Product.Id))]
    [MapperIgnoreTarget(nameof(Product.Brand))]
    public partial Product MapAddProductRequestToProduct(AddProductRequest request);
}