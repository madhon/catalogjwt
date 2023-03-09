namespace Catalog.API
{
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.Extensions.Configuration;
    using System.Text;

    public static class JwtAuthServicesExtensions
    {
        public static WebApplicationBuilder RegisterJwtAuthServices(this WebApplicationBuilder builder)
        {

            var jwtOpts = new JwtOptions();
            builder.Configuration.Bind(JwtOptions.Jwt, jwtOpts);
            builder.Services.AddSingleton(Options.Create(jwtOpts));

            var secret = jwtOpts.Secret;
            var key = Encoding.ASCII.GetBytes(secret);

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(x =>
            {
                x.MapInboundClaims = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = jwtOpts.Issuer,
                    ValidAudience = jwtOpts.Audience,
                    ClockSkew = TimeSpan.Zero
                };
            });
            
            return builder;
        }
    }
}
