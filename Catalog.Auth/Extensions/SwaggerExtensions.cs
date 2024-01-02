namespace Catalog.Auth.Extensions;

using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerUI;

internal static class SwaggerExtensions
{
    internal static IServiceCollection AddSwaggerExtension(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddApiVersioning()
            .AddApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VV";
                options.SubstituteApiVersionInUrl = true;
                options.DefaultApiVersion = new ApiVersion(1, 0);
            });

        services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();

        services.AddSwaggerGen();

        return services;
    }

    public static WebApplication UseSwaggerExtension(this WebApplication app)
    {
        app.UseSwagger(c =>
        {
            c.RouteTemplate = "docs/{documentName}/openapi.json";
            c.PreSerializeFilters.Add((swagger, httpReq) =>
            {
                swagger.Servers = new List<OpenApiServer>()
                {
                    new OpenApiServer
                        { Url = $"{httpReq.Scheme}://{httpReq.Host.Value}{httpReq.PathBase.Value}" }
                };
            });

            app.UseSwaggerUI(
                options =>
                {
                    options.RoutePrefix = "docs";
                    options.DocExpansion(DocExpansion.List);
                    options.DisplayRequestDuration();
                    options.DefaultModelExpandDepth(-1);

                    foreach (var description in app.DescribeApiVersions().Select(d => d.GroupName))
                    {
                        var url = $"/docs/{description}/openapi.json";
                        options.SwaggerEndpoint(url, description);
                    }
                });

                    
        });

        return app;
    }
}