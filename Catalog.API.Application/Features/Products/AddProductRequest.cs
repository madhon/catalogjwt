namespace Catalog.API.Application.Features.Products;

public record AddProductRequest(BrandId BrandId, string Name, string Description, decimal Price, Uri? PictureUri);