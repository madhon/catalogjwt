namespace Catalog.API.Web.API.Endpoints.Requests
{
    public sealed class AddBrandRequest
    {
        public string BrandName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}
