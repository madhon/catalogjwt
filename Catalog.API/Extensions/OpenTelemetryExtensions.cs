namespace Catalog.Api
{
    using System.Diagnostics;
    using System.Reflection;
    using OpenTelemetry.Resources;
    using OpenTelemetry.Trace;

    public static class OpenTelemetryExtensions
    {
        public static void AddOpenTelemetry(this IServiceCollection services, IWebHostEnvironment webHostEnvironment)
        {
            services.AddOpenTelemetryTracing(
                options =>
                {
                    options
                        .SetResourceBuilder(GetResourceBuilder(webHostEnvironment))
                        .AddEntityFrameworkCoreInstrumentation()
                        .AddHttpClientInstrumentation()
                        .AddAspNetCoreInstrumentation(
                            options =>
                            {
                                options.Enrich = Enrich;
                                options.RecordException = true;
                            });
                    if (webHostEnvironment.IsDevelopment())
                    {
                        options.AddConsoleExporter();
                    }
                });
        }

        private static void Enrich(Activity activity, string eventName, object obj)
        {
            if (obj is HttpRequest request)
            {
                var context = request.HttpContext;
                activity.AddTag("http.flavor", GetHttpFlavour(request.Protocol));
                activity.AddTag("http.scheme", request.Scheme);
                activity.AddTag("http.client_ip", context.Connection.RemoteIpAddress);
                activity.AddTag("http.request_content_length", request.ContentLength);
                activity.AddTag("http.request_content_type", request.ContentType);
            }
            else if (obj is HttpResponse response)
            {
                activity.AddTag("http.response_content_length", response.ContentLength);
                activity.AddTag("http.response_content_type", response.ContentType);
            }
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
