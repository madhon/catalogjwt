namespace Catalog.API.Domain.Entities;

[ValueObject<int>(conversions: Conversions.EfCoreValueConverter | Conversions.SystemTextJson)]
public partial class ProductId;