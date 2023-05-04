namespace Catalog.API.Web.API.Endpoints.Requests
{
    public sealed class AddBrandRequest
    {
        public string BrandName { get; set; } = null!;
        public string Description { get; set; } = null!;
    }
}
