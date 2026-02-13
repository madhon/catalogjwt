namespace Catalog.API.Infrastructure.Persistence.EntityConfigurations;

using Vogen;
using Catalog.API.Domain.Entities;

[EfCoreConverter<BrandId>]
[EfCoreConverter<ProductId>]
public sealed partial class VogenEfCoreConverters;