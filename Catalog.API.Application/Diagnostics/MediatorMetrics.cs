namespace Catalog.API.Application.Diagnostics;

using System.Diagnostics.Metrics;

public sealed class MediatorMetrics
{
    public const string MeterName = "Catalog.API.Mediator";
    private readonly Histogram<double> handlerDurationMs;

    public MediatorMetrics(IMeterFactory meterFactory)
    {
#pragma warning disable CA2000
        var meter = meterFactory.Create(MeterName);
#pragma warning restore CA2000
        handlerDurationMs = meter.CreateHistogram<double>(
            "mediator.handler.duration",
            unit: "ms",
            description: "Mediator handler execution duration");
    }

    public void RecordHandlerDuration(string handlerName, double elapsedMs) =>
        handlerDurationMs.Record(elapsedMs, new KeyValuePair<string, object?>("handler", handlerName));

}