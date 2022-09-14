namespace Catalog.API.Endpoints.Requests
{
    public sealed class AddProductRequest
    {
        public int BrandId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string PictureUri { get; set; } = string.Empty;
    }
}
