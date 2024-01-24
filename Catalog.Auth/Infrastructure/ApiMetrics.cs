namespace Catalog.Auth.Infrastructure;

using System.Diagnostics.Metrics;

public sealed class ApiMetrics
{
    private readonly Counter<int> loginCounter;
    private readonly Counter<int> failedLogings;

    public static readonly string MeterName = "Catalog.Auth";
    public static readonly string LoginCounterMeterName = "Catalog.Auth.Logins.Count";
    public static readonly string LoginFailedCounterMeterName = "Catalog.Auth.Logins.Failed.Count";
    
    public ApiMetrics(IMeterFactory meterFactory)
    {
        var meter = meterFactory.Create(MeterName);
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