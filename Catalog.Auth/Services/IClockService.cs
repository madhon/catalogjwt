namespace Catalog.Auth.Services;

using NodaTime;

public interface IClockService
{
    DateTimeZone? TimeZone { get; }

    Instant Now { get; }

    LocalDateTime LocalNow { get; }

    Instant? ToInstant(LocalDateTime? local);

    LocalDateTime? ToLocal(Instant? instant);
}