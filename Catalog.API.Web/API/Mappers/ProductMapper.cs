namespace Catalog.API.Web.API.Mappers;

using Catalog.API.Web.API.Endpoints.Requests;

[Mapper]
public partial class ProductMapper
{
    public partial Product MapAddProductRequestToProduct(AddProductRequest request);

}