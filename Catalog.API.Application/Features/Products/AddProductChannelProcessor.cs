namespace Catalog.API.Application.Features.Products;

using System.Threading.Channels;
using Catalog.API.Application.Abstractions;
using Microsoft.Extensions.Hosting;

public class AddProductChannelProcessor : BackgroundService
{
    private readonly Channel<Product> channel;
    private readonly IServiceProvider services;

    public AddProductChannelProcessor(Channel<Product> channel, IServiceProvider services)
    {
        this.channel = channel;
        this.services = services;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (await channel.Reader.WaitToReadAsync(stoppingToken))
        {
            var item = await channel.Reader.ReadAsync(stoppingToken);

            await using (var scope = services.CreateAsyncScope())
            {
                var catalogContext = scope.ServiceProvider.GetRequiredService<ICatalogDbContext>();
                catalogContext.Products.Add(item);
                await catalogContext.SaveChangesAsync(stoppingToken).ConfigureAwait(false);
            }
        }
    }
}