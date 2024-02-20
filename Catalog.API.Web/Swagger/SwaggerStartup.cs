namespace Catalog.API.Web.Swagger;

using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerUI;

internal static class SwaggerStartup
{
    public static void AddMySwagger(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(d =>
        {
            d.SwaggerDoc("v1", new OpenApiInfo()
            {
                Title = "Catalog API v1",
                Version = "v1",
                Description = "Catalog API",
                Contact = new OpenApiContact
                {
                    Name = "tester",
                    Email = "tester@tester.com"
                }
            });
        });
    }

    public static void UseMySwagger(this IApplicationBuilder app, IConfiguration configuration)
    {
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
    }
}