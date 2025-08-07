namespace Microsoft.Extensions.Configuration;

public static class ConfigurationExtensions
{
    public static string GetRequiredValue(this IConfiguration configuration, string name) =>
#pragma warning disable CA1062
        configuration[name] ?? throw new InvalidOperationException($"Configuration missing value for: {(configuration is IConfigurationSection s ? s.Path + ":" + name : name)}");
#pragma warning restore CA1062
}