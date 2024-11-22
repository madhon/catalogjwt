/*namespace Catalog.Auth.Infrastructure;

using Asp.Versioning.ApiExplorer;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

public class ConfigureSwaggerOptions : IConfigureOptions<SwaggerGenOptions>
{
    readonly IApiVersionDescriptionProvider provider;

    public ConfigureSwaggerOptions(IApiVersionDescriptionProvider provider) =>
        this.provider = provider;

    public void Configure(SwaggerGenOptions options)
    {
        foreach (var description in provider.ApiVersionDescriptions)
        {
            options.SwaggerDoc(
                description.GroupName,
                new OpenApiInfo()
                {
                    Title = $"Catalog Auth API {description.ApiVersion}",
                    Description = "Catalog Authentication API",
                    Version = description.ApiVersion.ToString(),
                    Contact = new OpenApiContact()
                    {
                        Name = "tester",
                        Email = "tester@tester.com"
                    }
                });
        }
    }
}*/