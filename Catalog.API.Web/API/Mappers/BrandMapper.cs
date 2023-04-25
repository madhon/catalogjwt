namespace Catalog.API.Web.API.Mappers
{
    using Catalog.API.Web.API.Endpoints.Requests;

    [Mapper]
    public partial class BrandMapper
    {

        public partial Brand MapAddBrandRequestToBrand(AddBrandRequest request);

        public partial AddBrandRequest MapBrandAddBrandRequest(Brand request);

    }
}
