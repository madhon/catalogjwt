namespace Catalog.API.Infrastructure.Authentication
{
    using Catalog.API.Infrastructure.Authentication.Settings;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.IdentityModel.Tokens;

    internal static class Startup
    {
        public static void ConfigureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.RegisterMyOptions<AuthenticationSettings>();
            ConfigureLocalJwtAuthentication(services, configuration.GetMyOptions<AuthenticationSettings>());
        }

        private static void ConfigureLocalJwtAuthentication(IServiceCollection services,
            AuthenticationSettings authSettings)
        {

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(x =>
                {
                    x.MapInboundClaims = false;
                    x.SaveToken = true;
                    x.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = authSettings.Issuer,

                        ValidateAudience = true,
                        ValidAudience = authSettings.Audience,

                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(authSettings.JwtSigningKey),
                        ClockSkew = TimeSpan.Zero,

                        RequireExpirationTime = true,
                        ValidateLifetime = true
                    };
                });
        }

        public static void Configure(IApplicationBuilder app)
        {
            app.UseAuthentication();
            app.UseAuthorization();
        }

    }
}
