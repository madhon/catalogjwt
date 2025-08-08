namespace Catalog.API.Web.Logging;

using System.Globalization;
using Catalog.API.Web.Logging.Helper;
using Catalog.API.Web.Logging.Settings;
using Serilog;
using Serilog.Events;
using Serilog.Exceptions;
using Serilog.Settings.Configuration;

internal static class LoggingStartup
{
    public static IHostApplicationBuilder AddMySerilogLogging(this IHostApplicationBuilder builder)
    {
        builder.Services.AddSerilog(loggerConfiguration =>
        {
            var serilogOptions = new SerilogOptions();
            builder.Configuration.GetSection("Serilog").Bind(serilogOptions);

            var options = new ConfigurationReaderOptions { SectionName = "Serilog" };
            loggerConfiguration.ReadFrom.Configuration(builder.Configuration, options);

            loggerConfiguration
                .Enrich.WithProperty("Application", builder.Environment.ApplicationName)
                .Enrich.FromLogContext()
                .Enrich.WithExceptionDetails();

            loggerConfiguration.MinimumLevel.Override("Microsoft", LogEventLevel.Information);
            loggerConfiguration.MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning);

            if (builder.Environment.IsDevelopment())
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
                    writeTo.Console(outputTemplate: serilogOptions.LogTemplate, formatProvider: CultureInfo.InvariantCulture);
                });
            }

            if (!string.IsNullOrEmpty(serilogOptions.SeqUrl))
            {
                loggerConfiguration.WriteTo.Seq(serilogOptions.SeqUrl, formatProvider: CultureInfo.InvariantCulture);
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