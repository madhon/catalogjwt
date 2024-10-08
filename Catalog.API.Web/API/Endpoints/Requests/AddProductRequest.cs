namespace Catalog.API.Web.API.Endpoints.Requests;

public sealed record AddProductRequest(BrandId BrandId, string Name, string Description, decimal Price, string PictureUri);