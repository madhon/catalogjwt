namespace Catalog.API.Infrastructure.Authentication.Settings;

using Microsoft.Extensions.Options;

[OptionsValidator]
public partial class AuthenticationSettingsValidator : IValidateOptions<AuthenticationSettings>;