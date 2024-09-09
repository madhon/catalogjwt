namespace Catalog.API.Infrastructure.Persistence;

using Microsoft.Extensions.Options;

[OptionsValidator]
internal sealed partial class  PersistenceOptionsValidator : IValidateOptions<PersistenceOptions>;