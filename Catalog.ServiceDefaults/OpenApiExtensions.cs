namespace Catalog.ServiceDefaults;

using Asp.Versioning;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Scalar.AspNetCore;

public static class OpenApiExtensions
{
    public static IApplicationBuilder UseDefaultOpenApi(this WebApplication app)
    {
        var configuration = app.Configuration;
        var openApiSection = configuration.GetSection("OpenApi");

        if (openApiSection is null)
        {
            return app;
        }

        app.MapOpenApi();
        app.MapScalarApiReference(opts => opts.DefaultFonts = false);

        // Add a redirect from the root of the app to the swagger endpoint
        app.MapGet("/", () => Results.Redirect("~/scalar/v1")).ExcludeFromDescription();

        return app;
    }

    public static WebApplicationBuilder AddDefaultOpenApi(this WebApplicationBuilder builder)
    {
        var services = builder.Services;
        var configuration = builder.Configuration;

        var openApi = configuration.GetSection("OpenApi");
        if (openApi is null)
        {
            return builder;
        }
        services.AddEndpointsApiExplorer();
        services.AddApiVersioning()
            .AddApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VV";
                options.SubstituteApiVersionInUrl = true;
                options.DefaultApiVersion = new ApiVersion(1, 0);
            });

        services.AddOpenApi(options =>
        {
            var apidoc = openApi.GetRequiredSection("Document");

            var version = apidoc.GetRequiredValue("Version") ?? "v1";

            options.AddDocumentTransformer((document, _, _) =>
            {
                document.Info = new OpenApiInfo
                {
                    Title = apidoc.GetRequiredValue("Title"),
                    Version = version,
                    Description = apidoc.GetRequiredValue("Description"),
                    Contact = new OpenApiContact
                    {
                        Name = $"{apidoc.GetRequiredValue("Description")} Team",
                    },
                };

                document.Servers = [];

                return Task.CompletedTask;
            });
        });
        return builder;
    }
}