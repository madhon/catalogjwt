namespace Catalog.API.Web.API.Endpoints.Requests;

public sealed record AddProductRequest(int BrandId, string Name, string Description, decimal Price, string PictureUri);

