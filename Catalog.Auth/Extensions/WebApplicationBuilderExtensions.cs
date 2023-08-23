namespace Catalog.Auth
{
	using Microsoft.AspNetCore.HttpOverrides;

	public static class WebApplicationBuilderExtensions
	{
		public static void RegisterServices(this WebApplicationBuilder builder)
		{
			var services = builder.Services;
			var configuration = builder.Configuration;

			services.Configure<ForwardedHeadersOptions>(opts =>
			{
				opts.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
				opts.KnownNetworks.Clear();
				opts.KnownProxies.Clear();
			});

			services.AddProblemDetails();

			builder.Services.AddEndpointsApiExplorer();
			builder.Services.AddApiVersioning()
				.AddApiExplorer(options =>
				{
					options.GroupNameFormat = "'v'VV";
					options.SubstituteApiVersionInUrl = true;
					options.DefaultApiVersion = new ApiVersion(1, 0);
				});

			builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();

			builder.Services.AddSwaggerGen();

			var jwtOpts = new JwtOptions();
			configuration.Bind(JwtOptions.Jwt, jwtOpts);
			services.AddSingleton(Options.Create(jwtOpts));

			var secret = jwtOpts.Secret;
			var key = Encoding.ASCII.GetBytes(secret);

			services.AddAuthorization();

			services.AddAuthentication(auth =>
				{
					auth.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
					auth.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
					auth.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
				})
				.AddJwtBearer(x =>
				{
					x.SaveToken = true;
					x.TokenValidationParameters = new TokenValidationParameters
					{
						ValidateIssuer = true,
						ValidIssuer = jwtOpts.Issuer,
						ValidateAudience = true,
						ValidAudience = jwtOpts.Audience,
						ValidateIssuerSigningKey = true,
						IssuerSigningKey = new SymmetricSecurityKey(key),
						ClockSkew = TimeSpan.FromSeconds(15)
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

			services.AddDbContext<AuthDbContext>(options =>
			{
				options.UseSqlServer(configuration["ConnectionString"], options =>
				{
					options.MigrationsAssembly(typeof(Program).GetTypeInfo().Assembly.GetName().Name);
					options.EnableRetryOnFailure(maxRetryCount: 15, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
				});
			});

			services.AddDataProtection()
				.PersistKeysToDbContext<AuthDbContext>();

			services.AddScoped<IPasswordHasher<ApplicationUser>, Argon2PasswordHasher<ApplicationUser>>();

			services.AddValidatorsFromAssemblyContaining<Program>();

			services.Configure<IdentityOptions>(options =>
			{
				options.SignIn.RequireConfirmedAccount = false;
				options.User.RequireUniqueEmail = true;
				options.Password.RequireDigit = true;
				options.Password.RequiredLength = 6;
				options.Password.RequireLowercase = true;
				options.Password.RequireNonAlphanumeric = true;
				options.Password.RequireUppercase = true;
				options.Password.RequiredUniqueChars = 1;
			});

			services.AddIdentity<ApplicationUser, IdentityRole>()
				.AddEntityFrameworkStores<AuthDbContext>()
				.AddDefaultTokenProviders();

			services.AddScoped<IJwtTokenService, JwtTokenService>();
			services.AddScoped<IAuthenticationService, AuthenticationService>();

			services.AddHealthChecks()
				.AddDbContextCheck<AuthDbContext>();

			services.AddHttpContextAccessor();
			services.AddHeaderPropagation(options => options.Headers.Add("x-correlation-id"));

			services.AddMyRateLimter();
		}
	}
}