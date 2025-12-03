namespace Catalog.AppHost;

using Aspire.Hosting;
using Aspire.Hosting.ApplicationModel;
using Aspire.Hosting.Eventing;
using Aspire.Hosting.Lifecycle;

internal static class Extensions
{
    public static IDistributedApplicationBuilder AddForwardedHeaders(this IDistributedApplicationBuilder builder)
    {
        builder.Services.TryAddEventingSubscriber<AddForwardHeadersHookSubscriber>();
        return builder;
    }

    // ReSharper disable once ClassNeverInstantiated.Local
    private sealed class AddForwardHeadersHookSubscriber : IDistributedApplicationEventingSubscriber
    {
        public Task SubscribeAsync(
            IDistributedApplicationEventing eventing,
            DistributedApplicationExecutionContext executionContext,
            CancellationToken cancellationToken)
        {
            eventing.Subscribe<BeforeStartEvent>((@event, _) =>
            {
                var model = @event.Model;

                foreach (var p in model.GetProjectResources())
                {
                    p.Annotations.Add(new EnvironmentCallbackAnnotation(context =>
                    {
                        context.EnvironmentVariables["ASPNETCORE_FORWARDEDHEADERS_ENABLED"] = "true";
                    }));
                }

                return Task.CompletedTask;
            });

            return Task.CompletedTask;
        }
    }
}