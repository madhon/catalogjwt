namespace Catalog.Auth.Infrastructure;

using Catalog.Auth.Login;
using Catalog.Auth.Signup;

public static class AuthApiExtensions
{
    public static IEndpointRouteBuilder MapAuthApi(this IEndpointRouteBuilder app, ApiVersionSet apiVersionSet)
    {
        app.MapGroup("api/v1/")
            .MapLoginEndpoint()
            .MapSignUpEndpoint();
        
        return app;
    }
}