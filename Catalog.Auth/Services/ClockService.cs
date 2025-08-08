namespace Catalog.Auth.Services;

using NodaTime;
using NodaTime.TimeZones;

internal sealed class ClockService : IClockService
{
    private readonly IClock _clock;

    public DateTimeZone? TimeZone { get; }

    public ClockService()
        : this(SystemClock.Instance)
    {
    }

    public ClockService(IClock clock)
    {
        _clock = clock;

        // NOTE: Get the current users timezone here instead of hard coding it...
        TimeZone = DateTimeZoneProviders.Tzdb.GetZoneOrNull("Europe/London");
    }

    public Instant Now
        => _clock.GetCurrentInstant();

    public LocalDateTime LocalNow
        => Now.InZone(TimeZone!).LocalDateTime;

    public Instant? ToInstant(LocalDateTime? local)
        => local?.InZone(TimeZone!, Resolvers.LenientResolver).ToInstant();

    public LocalDateTime? ToLocal(Instant? instant)
        => instant?.InZone(TimeZone!).LocalDateTime;
}