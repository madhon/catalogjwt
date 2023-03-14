namespace Catalog.API
{
    using System.Reflection;

    public static class ApplicationHeaderExtensions
    {

        public static IApplicationBuilder UseApplicationHeaders(this IApplicationBuilder app,
            IWebHostEnvironment hostingEnvironment,
            IConfiguration configuration)
        {
            var appVersion = configuration["APP_VERSION"] ??
                             Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyFileVersionAttribute>().Version;

            return app.Use(async (context, func) =>
            {
                context.Response.OnStarting(state =>
                {
                    var httpContext = (HttpContext)state;

                    httpContext.Response.Headers.Add("X-Environment", new[] { hostingEnvironment.EnvironmentName });
                    httpContext.Response.Headers.Add("X-Version", new[] { appVersion });
                    return Task.FromResult(0);
                }, context);

                await func.Invoke();
            });
        }

    }
}
