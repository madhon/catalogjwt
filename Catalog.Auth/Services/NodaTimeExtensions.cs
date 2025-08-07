namespace Catalog.Auth.Services;

using NodaTime;

internal static class NodaTimeExtensions
{
    public static string? ToDateTimeString(this LocalDateTime? local)
    {
        return local?.ToDateTimeString();
    }

    public static string? ToDateTimeString(this LocalDateTime local)
    {
        var culture = System.Threading.Thread.CurrentThread.CurrentCulture;
        return local.ToString($"{culture.DateTimeFormat.ShortDatePattern} {culture.DateTimeFormat.ShortTimePattern}", culture);
    }

    public static string? ToShortDateString(this LocalDate? local)
    {
        return local?.ToShortDateString();
    }

    public static string ToShortDateString(this LocalDate local)
    {
        var culture = System.Threading.Thread.CurrentThread.CurrentCulture;
        return local.ToString(culture.DateTimeFormat.ShortDatePattern, culture);
    }
}