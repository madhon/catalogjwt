namespace Catalog.API.Web.Swagger
{
    internal static class SwaggerStartup
    {
        public static void AddMySwagger(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSwaggerDoc(maxEndpointVersion: 1, settings: s =>
            {
                s.DocumentName = "v1.0";
                s.Title = "Catalog API";
                s.Version = "v1.0";
            }, shortSchemaNames: true);

        }

        public static void UseMySwagger(this IApplicationBuilder app, IConfiguration configuration)
        {
            app.UseOpenApi();
            app.UseSwaggerUi3();
        }
    }
}
