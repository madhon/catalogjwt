﻿namespace Catalog.Auth.Login;

public record LoginResponse
{
    [JsonPropertyName("access_token")]
    public string AccessToken { get; init; } = null!;

    [JsonPropertyName("expires_in")]
    public int ExpiresIn { get; init; }

    [JsonPropertyName("token_type")]
    public string TokenType { get; init; } = "Bearer";
}