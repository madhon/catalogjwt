namespace Catalog.API.Web;

using Catalog.API.Application;
using Catalog.API.Web.API;
using Catalog.API.Web.Swagger;
using Catalog.API.Web.Telemetry;

public class Startup
{
    protected IConfiguration Configuration { get; }
    protected IWebHostEnvironment Environment { get; }

    public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        => (Configuration, Environment) = (configuration, environment);

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddMyApi();
        services.AddMySwagger(Configuration);
        services.AddMyTelemetry(Configuration, Environment);
        services.AddMyInfrastructureDependencies(Configuration, Environment);
        services.AddApplicationServices();
    }

    public void Configure(IApplicationBuilder app)
    {
        app.UseMyRequestLogging();
        app.UseExceptionHandler();
        app.UseStatusCodePages();
        app.UseRouting();
        app.UseMyInfrastructure(Configuration, Environment);
        app.UseMyApi(Configuration, Environment);
        app.UseMySwagger(Configuration);
    }
}