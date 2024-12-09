namespace Catalog.Auth;

using Microsoft.AspNetCore.Http.Features;
using Serilog;
using Serilog.Configuration;
using Serilog.Core;
using Serilog.Events;
using Serilog.Exceptions;
using Serilog.Settings.Configuration;

internal static class SerilogExtensions
{
    internal static WebApplicationBuilder AddSerilog(this WebApplicationBuilder builder, string sectionName = "Serilog")
    {
        var serilogOptions = new SerilogOptions();
        builder.Configuration.GetSection(sectionName).Bind(serilogOptions);

        Serilog.Debugging.SelfLog.Enable(msg => Console.WriteLine(msg));

        builder.Host.UseSerilog((context, loggerConfiguration) =>
        {
            var options = new ConfigurationReaderOptions { SectionName = "Serilog" };
            loggerConfiguration.ReadFrom.Configuration(context.Configuration, options)
                .MinimumLevel.Override("Microsoft", LogEventLevel.Error)
                .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Error)
                .MinimumLevel.Override("Serilog", LogEventLevel.Information)
                .MinimumLevel.Override("ZiggyCreatures.Caching.Fusion.FusionCache", LogEventLevel.Error)
                .Enrich.WithProperty("Application", builder.Environment.ApplicationName)
                .Enrich.FromLogContext()
                .Enrich.WithUtcTime()
                .Enrich.WithUserInfo()
                .Enrich.WithExceptionDetails();

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

    private static LoggerConfiguration WithUtcTime(this LoggerEnrichmentConfiguration enrichmentConfiguration)
    {
        return enrichmentConfiguration.With<UtcTimestampEnricher>();
    }
    private static LoggerConfiguration WithUserInfo(this LoggerEnrichmentConfiguration enrichmentConfiguration)
    {
        return enrichmentConfiguration.With<UserInfoEnricher>();
    }

    internal static WebApplication UseSerilogRequestLogs(this WebApplication app)
    {
        app.UseSerilogRequestLogging();
        return app;
    }

    internal sealed class SerilogOptions
    {
        public bool UseConsole { get; set; } = true;
        public string? SeqUrl { get; set; } = string.Empty;

        public string LogTemplate { get; set; } =
            "[{Timestamp:HH:mm:ss} {Level:u3} {ClientIp}] - {Message:lj}{NewLine}{Exception}";
    }
}

internal sealed class UtcTimestampEnricher : ILogEventEnricher
{
    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory pf)
    {
        logEvent.AddOrUpdateProperty(pf.CreateProperty("TimeStamp", logEvent.Timestamp.UtcDateTime));
    }
}

internal sealed class UserInfoEnricher : ILogEventEnricher
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    public UserInfoEnricher() : this(new HttpContextAccessor())
    {
    }
    public UserInfoEnricher(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }
    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        var userName = _httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "";
        var headers = _httpContextAccessor.HttpContext?.Request?.Headers;
        var clientIp = headers != null && headers.ContainsKey("X-Forwarded-For")
            ? headers["X-Forwarded-For"].ToString().Split(',').First().Trim()
            : _httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?.ToString() ?? "";
        var clientAgent = headers != null && headers.ContainsKey("User-Agent")
            ? headers["User-Agent"].ToString()
            : "";
        var activity = _httpContextAccessor.HttpContext?.Features.Get<IHttpActivityFeature>()?.Activity;
        if (activity != null)
        {
            logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("ActivityId", activity.Id));
            logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("ParentId", activity.ParentId));
        }
        logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("UserName", userName));
        logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("ClientIP", clientIp));
        logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("ClientAgent", clientAgent));
    }
}