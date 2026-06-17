namespace Catalog.API.Application.Features.Products;

using System.Threading.Channels;
using Catalog.API.Application.Abstractions;
using Catalog.API.Application.Features.ListProducts;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using ZiggyCreatures.Caching.Fusion;

public sealed class AddProductChannelProcessor : BackgroundService
{
    private readonly Channel<Product> channel;
    private readonly IServiceProvider services;
    private readonly IFusionCache fusionCache;
    private readonly int maxBatchSize;
    private readonly List<Product> batch;

    public AddProductChannelProcessor(
        Channel<Product> channel,
        IServiceProvider services,
        IFusionCache fusionCache,
        IOptions<AddProductChannelOptions> options)
    {
        ArgumentNullException.ThrowIfNull(options);

        this.channel = channel;
        this.services = services;
        this.fusionCache = fusionCache;
        maxBatchSize = Math.Max(1, options.Value.MaxBatchSize);
        batch = new List<Product>(maxBatchSize);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (await channel.Reader.WaitToReadAsync(stoppingToken).ConfigureAwait(false))
        {
            batch.Clear();

            batch.Add(await channel.Reader.ReadAsync(stoppingToken).ConfigureAwait(false));

            while (batch.Count < maxBatchSize && channel.Reader.TryRead(out var item))
            {
                batch.Add(item);
            }

            await PersistBatchAsync(stoppingToken).ConfigureAwait(false);
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        await base.StopAsync(cancellationToken).ConfigureAwait(false);
        await DrainChannelAsync(cancellationToken).ConfigureAwait(false);
    }

    private async Task DrainChannelAsync(CancellationToken cancellationToken)
    {
        batch.Clear();

        while (channel.Reader.TryRead(out var item))
        {
            batch.Add(item);
            if (batch.Count >= maxBatchSize)
            {
                await PersistBatchAsync(cancellationToken).ConfigureAwait(false);
                batch.Clear();
            }
        }

        if (batch.Count > 0)
        {
            await PersistBatchAsync(cancellationToken).ConfigureAwait(false);
        }
    }

    private async Task PersistBatchAsync(CancellationToken cancellationToken)
    {
        await using var scope = services.CreateAsyncScope();
        var catalogContext = scope.ServiceProvider.GetRequiredService<ICatalogDbContext>();

        catalogContext.Products.AddRange(batch);
        await catalogContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        await fusionCache.RemoveByTagAsync(
            ListProductsRequest.ProductsAllCacheTag,
            token: cancellationToken).ConfigureAwait(false);
    }
}
