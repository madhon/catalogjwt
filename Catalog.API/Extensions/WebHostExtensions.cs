namespace Catalog.API
{
    using Serilog;
    using Serilog.Events;

    public static class WebHostExtensions
    {
        public static void ConfigureHost(this WebApplicationBuilder builder)
        {
            var host = builder.Host;

            host.UseSerilog((ctx, lc) =>
            {
                lc.MinimumLevel.Override("Microsoft", LogEventLevel.Information);
                lc.MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning);
                
                lc.WriteTo.Async(o => o.Console());
                lc.Enrich.FromLogContext();
                lc.Enrich.WithCorrelationIdHeader("x-correlation-id");
            });
        }
    }
}
