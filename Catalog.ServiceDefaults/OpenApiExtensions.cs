namespace Catalog.ServiceDefaults;

using Asp.Versioning;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi;
using Scalar.AspNetCore;

public static class OpenApiExtensions
{
    public static IApplicationBuilder UseDefaultOpenApi(this WebApplication app)
    {
        ArgumentNullException.ThrowIfNull(app);

        // var configuration = app.Configuration;
        // var openApiSection = configuration.GetSection("OpenApi");
        //
        // if (!openApiSection.Exists())
        // {
        //     return app;
        // }

        app.MapOpenApi().WithDocumentPerVersion();
        app.MapScalarApiReference(opts => opts.DefaultFonts = false);
        app.MapGet("/", () => Results.Redirect("~/scalar/v1")).ExcludeFromDescription();

        return app;
    }

    public static TBuilder  AddDefaultOpenApi<TBuilder>(this TBuilder  builder) where TBuilder : IHostApplicationBuilder
    {
        ArgumentNullException.ThrowIfNull(builder);

        var openApi = builder.Configuration.GetSection("OpenApi");
        if (!openApi.Exists())
        {
            return builder;
        }

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddApiVersioning(options =>
            {
                options.ApiVersionReader = new UrlSegmentApiVersionReader();
            })
            .AddApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VV";
                options.SubstituteApiVersionInUrl = true;
                options.DefaultApiVersion = ApiVersion.Default;

            }).AddOpenApi(options =>
            {
                options.Document.AddScalarTransformers();
            });

        /*builder.Services.AddOpenApi(options =>
        {
            var apiDoc = openApi.GetRequiredSection("Document");

            var version = apiDoc.GetRequiredValue("Version");
            options.AddScalarTransformers();

            options.AddDocumentTransformer((document, _, _) =>
            {
                document.Info = new OpenApiInfo
                {
                    Title = apiDoc.GetRequiredValue("Title"),
                    Version = version,
                    Description = apiDoc.GetRequiredValue("Description"),
                    Contact = new OpenApiContact
                    {
                        Name = $"{apiDoc.GetRequiredValue("Description")} Team",
                    },
                };

                document.Servers = [];

                return Task.CompletedTask;
            });
        });*/

        return builder;
    }
}
