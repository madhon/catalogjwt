namespace Catalog.Gateway.Extensions;

using Serilog;
using Serilog.Events;
using Serilog.Exceptions;
using Serilog.Settings.Configuration;

internal static class SerilogExtensions
{
    public static WebApplicationBuilder AddSerilog(this WebApplicationBuilder builder, string sectionName = "Serilog")
    {
        var serilogOptions = new SerilogOptions();
        builder.Configuration.GetSection(sectionName).Bind(serilogOptions);

        builder.Host.UseSerilog((context, loggerConfiguration) =>
        {
            var options = new ConfigurationReaderOptions { SectionName = "Serilog" };
            loggerConfiguration.ReadFrom.Configuration(context.Configuration, options);

            loggerConfiguration
                .Enrich.WithProperty("Application", builder.Environment.ApplicationName)
                .Enrich.FromLogContext()
                .Enrich.WithExceptionDetails();

            loggerConfiguration.MinimumLevel.Override("Microsoft", LogEventLevel.Information);
            loggerConfiguration.MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning);

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
        
    internal sealed class SerilogOptions
    {
        public bool UseConsole { get; set; } = true;
#pragma warning disable S3459
#pragma warning disable S1144
        public string? SeqUrl { get; set; }
#pragma warning restore S1144
#pragma warning restore S3459

        public string LogTemplate { get; set; } =
            "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} {Level:u3} - {Message:lj}{NewLine}{Exception}";
    }
}