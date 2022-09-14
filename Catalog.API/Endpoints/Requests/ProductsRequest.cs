namespace Catalog.API.Endpoints.Requests
{
    public sealed class ProductsRequest
    {
        public int PageSize { get; set; } = 10;
        public int PageIndex { get; set; } = 0;
    }
}
