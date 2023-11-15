namespace Catalog.API.Infrastructure.Authentication
{
    using System.Linq;
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

            services.AddAuthorization();

            services.AddAuthentication(auth =>
            {
                auth.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                auth.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                auth.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
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
                        ClockSkew = TimeSpan.FromSeconds(15),

                        RequireExpirationTime = true,
                        ValidateLifetime = true
                    };


                    x.Events = new JwtBearerEvents
                    {
                        OnAuthenticationFailed = context =>
                        {
                            if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                            {
                                context.Response.Headers.Add("Token-Expired", "true");
                            }

                            return Task.CompletedTask;
                        }
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
