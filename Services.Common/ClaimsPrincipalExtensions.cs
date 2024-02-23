namespace Microsoft.Extensions.Hosting;

using System.Security.Claims;

public static class ClaimsPrincipalExtensions
{
    public static string Id(this ClaimsPrincipal user)
        => user.Claims.First(x => x.Type.Equals("sub", StringComparison.OrdinalIgnoreCase)).Value;
        
    public static string? FullName(this ClaimsPrincipal user)
        => user?.Identity?.Name;


    public static string? Azp(this ClaimsPrincipal user)
    {
        return user?.GetClaim("azp");
    }

    public static string? GetClaim(this ClaimsPrincipal user, string key) => 
        user.Claims?.FirstOrDefault(i => i.Type.Equals(key, StringComparison.OrdinalIgnoreCase))?.Value;

    public static IEnumerable<string> GetClaims(this ClaimsPrincipal user, string key) => 
        user.Claims.Where(i => i.Type.Equals(key, StringComparison.OrdinalIgnoreCase)).Select(i => i.Value);
}