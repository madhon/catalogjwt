namespace Catalog.AppHost;

using Aspire.Hosting;
using Aspire.Hosting.ApplicationModel;
using Aspire.Hosting.Lifecycle;

internal static class Extensions
{
    public static IDistributedApplicationBuilder AddForwardedHeaders(this IDistributedApplicationBuilder builder)
    {
        builder.Services.TryAddLifecycleHook<AddForwardHeadersHook>();
        return builder;
    }

    // ReSharper disable once ClassNeverInstantiated.Local
    private sealed class AddForwardHeadersHook : IDistributedApplicationLifecycleHook
    {
        public Task BeforeStartAsync(DistributedApplicationModel appModel, CancellationToken cancellationToken = default)
        {
            foreach (var p in appModel.GetProjectResources())
            {
                p.Annotations.Add(new EnvironmentCallbackAnnotation(context =>
                {
                    context.EnvironmentVariables["ASPNETCORE_FORWARDEDHEADERS_ENABLED"] = "true";
                }));
            }

            return Task.CompletedTask;
        }
    }
}