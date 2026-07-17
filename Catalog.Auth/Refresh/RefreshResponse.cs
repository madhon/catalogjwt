namespace Catalog.Auth.Refresh;

internal sealed record RefreshResponse
{
    [JsonPropertyName("access_token")]
    public string AccessToken { get; init; } = null!;

    [JsonPropertyName("expires_in")]
    public int ExpiresIn { get; init; }

    [JsonPropertyName("refresh_token")]
    public string RefreshToken { get; init; } = null!;

    [JsonPropertyName("refresh_expires_at")]
    public DateTime RefreshExpiresAt { get; init; }

    [JsonPropertyName("token_type")]
    public string TokenType { get; init; } = "Bearer";        
}