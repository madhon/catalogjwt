namespace Catalog.API.Application.Features.Products;

public sealed class AddProductChannelOptions
{
    public const string SectionName = "AddProductChannel";

    public int MaxBatchSize { get; set; } = 20;
}
