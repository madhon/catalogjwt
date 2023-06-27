namespace Catalog.Auth.Signup
{

	public static class SignUpEndpoint
	{
		public static IEndpointRouteBuilder MapSignUpEndpoint(this IEndpointRouteBuilder app, ApiVersionSet versionSet)
		{
			app.MapPost("api/v{version:apiVersion}/auth/signup",
				async Task<Results<Ok<SignupResponse>, ProblemHttpResult, BadRequest>>
				(SignupRequest request, IAuthenticationService authenticationService, CancellationToken ct) =>
				{

				var result = await authenticationService.CreateUser(request.Email, request.Password, request.Fullname, ct).ConfigureAwait(false);

				if (!result.IsError)
				{
					return TypedResults.Ok(new SignupResponse(true, "User created successfully"));
				}
				else
				{
					return TypedResults.Problem(title: "Error Creating User",
						detail: result.Errors[0].Description,
						statusCode: 400);
				}
				})
				.AllowAnonymous()
				.WithName("auth.signup")
				.WithDescription("Sign Up to API")
				.WithTags("Auth")
				.WithApiVersionSet(versionSet)
				.Accepts<SignupRequest>("application/json")
				.Produces<SignupResponse>(200, "application/json")
				.Produces<BadRequest>()
				.ProducesValidationProblem()
				.WithOpenApi()
				.RequireRateLimiting(RateLimiterPolicies.RlPoicy);

			return app;
		}

	}
}
