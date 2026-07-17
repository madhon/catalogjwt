namespace Catalog.Auth.Refresh;

internal static class RefreshEndpoint
{
    public static IEndpointRouteBuilder MapRefreshEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("auth/refresh", HandleRefresh)
            .AllowAnonymous()
            .WithName(nameof(HandleRefresh))
            .WithDescription("Refresh authentication token")
            .WithTags("Auth")
            .Accepts<RefreshRequest>("application/json")
            .Produces<RefreshResponse>(200, "application/json")
            .Produces<UnauthorizedHttpResult>()
            .Produces<ProblemHttpResult>()
            .ProducesValidationProblem();

        return app;
    }

    private static async Task<Results<Ok<RefreshResponse>, ValidationProblem, ProblemHttpResult, UnauthorizedHttpResult>>
        HandleRefresh(
            RefreshRequest request,
            IAuthenticationService authenticationService,
            UserManager<ApplicationUser> userManager,
            AuthDbContext  dbContext,
            ApiMetrics metrics,
            CancellationToken ct)
    {
        var existing = await dbContext.RefreshTokens
            .FirstOrDefaultAsync(t => t.Token == request.RefreshToken, ct);
        if (existing is null)
        {
            return TypedResults.Unauthorized();
        }

        if (!existing.IsActive)
        {
            if (existing.Revoked is not null)
            {
                await RevokeAllActiveTokensAsync(dbContext, existing.UserId);
            }
            return TypedResults.Unauthorized();
        }

        var user = await userManager.FindByIdAsync(existing.UserId);
        if (user is null)
        {
            return TypedResults.Unauthorized();
        }

        var result = await authenticationService.Refresh(user.Id);

        var response = new RefreshResponse
        {
            AccessToken = result.Token,
            ExpiresIn = result.ExpiresIn,
            RefreshToken = result.RefreshToken,
            RefreshExpiresAt = result.RefreshExpiresAt,
        };

        existing.Revoked = DateTime.UtcNow;
        existing.ReplacedByToken = response.RefreshToken;
        await dbContext.SaveChangesAsync(ct);

        return TypedResults.Ok(response);
    }

    private static async Task RevokeAllActiveTokensAsync(AuthDbContext db, string userId)
    {
        var activeTokens = await db.RefreshTokens
            .Where(t => t.UserId == userId && t.Revoked == null)
            .ToListAsync();

        foreach (var token in activeTokens)
        {
            token.Revoked = DateTime.UtcNow;
        }

        await db.SaveChangesAsync();
    }
}