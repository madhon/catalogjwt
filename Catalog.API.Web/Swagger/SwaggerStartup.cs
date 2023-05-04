namespace Catalog.API.Web.Swagger
{
    internal static class SwaggerStartup
    {
        public static void AddMySwagger(this IServiceCollection services, IConfiguration configuration)
        {
            services.SwaggerDocument(o =>
            {
                o.MaxEndpointVersion = 1;
                o.ShortSchemaNames = true;
                o.DocumentSettings = s =>
                {
                    s.DocumentName = "v1.0";
                    s.Title = "Catalog API";
                    s.Version = "v1.0";

                };


            });
        }

        public static void UseMySwagger(this IApplicationBuilder app, IConfiguration configuration)
        {
            app.UseOpenApi();
            app.UseSwaggerUi3();
        }
    }
}
