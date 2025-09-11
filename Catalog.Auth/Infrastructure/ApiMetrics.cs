namespace Catalog.Auth.Infrastructure;

using System.Diagnostics.Metrics;

[RegisterSingleton]
internal sealed class ApiMetrics
{
    private readonly Counter<int> loginCounter;
    private readonly Counter<int> failedLogings;

    private const string MeterName = "Catalog.Auth";
    private const string LoginCounterMeterName = "Catalog.Auth.Logins.Count";
    private const string LoginFailedCounterMeterName = "Catalog.Auth.Logins.Failed.Count";

    public ApiMetrics(IMeterFactory meterFactory)
    {
#pragma warning disable CA2000
        var meter = meterFactory.Create(MeterName);
#pragma warning restore CA2000
        loginCounter = meter.CreateCounter<int>(LoginCounterMeterName);
        failedLogings = meter.CreateCounter<int>(LoginFailedCounterMeterName);
    }

    public void UserLoggedIn()
    {
        loginCounter.Add(1);
    }

    public void UserFailedLogin()
    {
        failedLogings.Add(1);
    }
}