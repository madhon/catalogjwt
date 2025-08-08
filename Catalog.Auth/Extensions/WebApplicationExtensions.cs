namespace Catalog.Auth;

internal static class WebApplicationExtensions
{
    public static void ConfigureApplication(this WebApplication app)
    {
        ArgumentNullException.ThrowIfNull(app);

        app.UseForwardedHeaders();

        app.UseExceptionHandler();
        app.UseStatusCodePages();

        app.UseSerilogRequestLogs();

#pragma warning disable S125
        //app.AddDefaultSecurityHeaders();
#pragma warning restore S125

        app.UseRouting();

        app.UseMyRateLimiter();

        app.UseAuthentication();
        app.UseAuthorization();

        if (app.Environment.IsDevelopment())
        {
            app.UseDefaultOpenApi();
            app.UseDeveloperExceptionPage();
        }

        app.UseHeaderPropagation();

        var versionSet = app.NewApiVersionSet()
            .HasApiVersion(new ApiVersion(1.0))
            .ReportApiVersions()
            .Build();

        app.MapDefaultEndpoints();
        app.MapAuthApi(versionSet);
    }
}