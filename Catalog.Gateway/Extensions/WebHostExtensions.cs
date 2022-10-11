namespace Catalog.Gateway
{
    using Serilog;
    using Serilog.Events;

    public static class WebHostExtensions
    {
        public static void ConfigureHost(this WebApplicationBuilder builder)
        {
            var host = builder.Host;
            var configuration = builder.Configuration;

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
