namespace Catalog.API.Web.Logging;

using Catalog.API.Web.Logging.Helper;
using Catalog.API.Web.Logging.Settings;
using Serilog.Exceptions;
using Serilog.Settings.Configuration;

public static class LoggingStartup
{
    public static IHostBuilder AddMySerilogLogging(this IHostBuilder builder)
    {
        builder.UseSerilog((context, loggerConfiguration) =>
        {
            var serilogOptions = new SerilogOptions();
            context.Configuration.GetSection("Serilog").Bind(serilogOptions);

            var options = new ConfigurationReaderOptions { SectionName = "Serilog" };
            loggerConfiguration.ReadFrom.Configuration(context.Configuration, options);

            loggerConfiguration
                .Enrich.WithProperty("Application", context.HostingEnvironment.ApplicationName)
                .Enrich.FromLogContext()
                .Enrich.WithExceptionDetails();

            loggerConfiguration.MinimumLevel.Override("Microsoft", LogEventLevel.Information);
            loggerConfiguration.MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning);

            if (context.HostingEnvironment.IsDevelopment())
            {
                loggerConfiguration.MinimumLevel.Override("ZiggyCreatures.Caching.Fusion", LogEventLevel.Debug);
            }
            else
            {
                loggerConfiguration.MinimumLevel.Override("ZiggyCreatures.Caching.Fusion", LogEventLevel.Warning);
            }

            if (serilogOptions.UseConsole)
            {
                loggerConfiguration.WriteTo.Async(writeTo =>
                {
                    writeTo.Console(outputTemplate: serilogOptions.LogTemplate);
                });
            }

            if (!string.IsNullOrEmpty(serilogOptions.SeqUrl))
            {
                loggerConfiguration.WriteTo.Seq(serilogOptions.SeqUrl);
            }
        });

        return builder;
    }

    public static IApplicationBuilder UseMyRequestLogging(this IApplicationBuilder appBuilder)
    {
        appBuilder.UseSerilogRequestLogging(opts =>
        {
            opts.GetLevel = LogHelper.ExcludeHealthChecks;
        });

        return appBuilder;
    }
}