namespace Catalog.Auth
{
    using System.Diagnostics;
    using OpenTelemetry;
    using OpenTelemetry.Metrics;
    using OpenTelemetry.Resources;
    using OpenTelemetry.Trace;

    public static class OpenTelemetryExtensions
    {
        public static void AddOpenTelemetry(this WebApplicationBuilder builder)
        {
            var services = builder.Services;
            var environment = builder.Environment;

            services.AddOpenTelemetry().WithTracing(cfg =>
                {
                    cfg.SetResourceBuilder(GetResourceBuilder(environment))
                        .AddHttpClientInstrumentation()
                        .AddAspNetCoreInstrumentation(nci =>
                        {
                            nci.EnrichWithHttpRequest = Enrich;
                            nci.EnrichWithHttpResponse = Enrich;
                            nci.RecordException = true;
                        });

                    if (environment.IsDevelopment())
                    {
                        cfg.AddConsoleExporter();
                        cfg.AddOtlpExporter();
                    }
                    cfg.AddSource("Catalog.Auth");
                }).WithMetrics(cfg =>
                {
                    cfg.SetResourceBuilder(GetResourceBuilder(environment))
                        .AddAspNetCoreInstrumentation()
                        .AddHttpClientInstrumentation()
                        .AddRuntimeInstrumentation();
                })
                .StartWithHost();
        }

        private static void Enrich(Activity activity, HttpRequest request)
        {
            var context = request.HttpContext;
            activity.AddTag("http.flavor", GetHttpFlavour(request.Protocol));
            activity.AddTag("http.scheme", request.Scheme);
            activity.AddTag("http.client_ip", context.Connection.RemoteIpAddress);
            activity.AddTag("http.request_content_length", request.ContentLength);
            activity.AddTag("http.request_content_type", request.ContentType);
        }

        private static void Enrich(Activity activity, HttpResponse response)
        {
            activity.AddTag("http.response_content_length", response.ContentLength);
            activity.AddTag("http.response_content_type", response.ContentType);
        }


        public static string GetHttpFlavour(string protocol)
        {
            if (HttpProtocol.IsHttp10(protocol))
            {
                return "1.0";
            }
            else if (HttpProtocol.IsHttp11(protocol))
            {
                return "1.1";
            }
            else if (HttpProtocol.IsHttp2(protocol))
            {
                return "2.0";
            }
            else if (HttpProtocol.IsHttp3(protocol))
            {
                return "3.0";
            }

            throw new InvalidOperationException($"Protocol {protocol} not recognised.");
        }

        private static ResourceBuilder GetResourceBuilder(IWebHostEnvironment webHostEnvironment)
        {
            var version = Assembly
                .GetExecutingAssembly()
                .GetCustomAttribute<AssemblyFileVersionAttribute>()!
                .Version;

            return ResourceBuilder
                .CreateEmpty()
                .AddService(webHostEnvironment.ApplicationName, serviceVersion: version)
                .AddAttributes(
                    new KeyValuePair<string, object>[]
                    {
                        new("deployment.environment", webHostEnvironment.EnvironmentName),
                        new("host.name", Environment.MachineName),
                    })
                .AddEnvironmentVariableDetector();
        }
    }
}
