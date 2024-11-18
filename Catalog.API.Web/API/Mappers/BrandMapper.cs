namespace Catalog.API.Web.API.Mappers;

using Catalog.API.Web.API.Endpoints.Requests;

[Mapper]
public partial class BrandMapper
{
    [MapperIgnoreTarget(nameof(Brand.Id))]
    public partial Brand MapAddBrandRequestToBrand(AddBrandRequest request);

    [MapperIgnoreSource(nameof(Brand.Id))]
    public partial AddBrandRequest MapBrandAddBrandRequest(Brand request);
}