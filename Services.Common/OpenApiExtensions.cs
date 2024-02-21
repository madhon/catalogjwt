namespace Services.Common;

using Asp.Versioning;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerUI;

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
        
        app.UseSwagger(c =>
        {
            c.RouteTemplate = "docs/{documentName}/swagger.json";
            
            c.PreSerializeFilters.Add((swagger, httpReq) =>
            {
                swagger.Servers = new List<OpenApiServer>()
                {
                    new OpenApiServer
                        { Url = $"{httpReq.Scheme}://{httpReq.Host.Value}{httpReq.PathBase.Value}" }
                };
            });
            
        });
        
        app.UseSwaggerUI(ui =>
        {
            ui.RoutePrefix = "docs";
            ui.DocExpansion(DocExpansion.List);
            ui.DisplayRequestDuration();
            ui.DefaultModelExpandDepth(-1);
            
        });

        // Add a redirect from the root of the app to the swagger endpoint
        app.MapGet("/", () => Results.Redirect("/docs")).ExcludeFromDescription();

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

        services.AddSwaggerGen(options =>
        {
            var document = openApi.GetRequiredSection("Document");

            var version = document.GetRequiredValue("Version") ?? "v1";

            options.SwaggerDoc(version, new OpenApiInfo
            {
                Title = document.GetRequiredValue("Title"),
                Version = version,
                Description = document.GetRequiredValue("Description")
            });
        });

        return builder;
    }
}