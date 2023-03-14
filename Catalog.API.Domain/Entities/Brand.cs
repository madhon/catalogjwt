namespace Catalog.API.Domain.Entities
{
    public class Brand : BaseEntity
    {
        public string BrandName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}
