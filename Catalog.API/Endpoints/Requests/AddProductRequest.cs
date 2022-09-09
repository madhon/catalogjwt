namespace Catalog.API.Endpoints.Requests
{
    public class AddProductRequest
    {
        public int BrandId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string PictureUri { get; set; } = string.Empty;
    }
}
