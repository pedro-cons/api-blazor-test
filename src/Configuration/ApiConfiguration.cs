using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.OpenApi.Models;
using System;
using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Domain.Helpers;
using Repository.Context;
using Microsoft.EntityFrameworkCore;
using Domain.Repository;
using Repository.Repository;
using Application.Interfaces;
using Application.Services;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Configuration.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Configuration;

/// <summary>
/// Class responsible for configuring API services and applications
/// </summary>
public static class ApiConfiguration
{
	#region Main

	/// <summary>
	/// Method responsible for configuring services
	/// </summary>
	/// <param name="builder">WebApplicationBuilder parameter</param>
	public static void Configure(WebApplicationBuilder builder)
	{
		ConfigureContexts(builder.Configuration);
		ConfigureAuthentication(builder.Services, builder.Configuration);
		ConfigureServices(builder.Services);
	}

	/// <summary>
	/// Method responsible for configuring the application
	/// </summary>
	/// <param name="app">WebApplication parameter</param>
	public static void Configure(WebApplication app)
	{
		ConfigureApp(app);
	}

	#endregion

	#region Configuration

	/// <summary>
	/// Method responsible for configuring contexts
	/// </summary>
	/// <param name="configuration">IConfiguration parameter</param>
	private static void ConfigureContexts(IConfiguration configuration)
	{
		Domain.Helpers.Configuration.AddSettings(configuration);
	}

	/// <summary>
	/// Method responsible for configuring the application
	/// </summary>
	/// <param name="app">WebApplication parameter</param>
	private static void ConfigureApp(WebApplication app)
	{
		app.UseHttpsRedirection();

		app.UseSwagger();
		app.UseSwaggerUI(c =>
		{
			c.SwaggerEndpoint("/swagger/v1/swagger.json", "API - v1.0");
			c.RoutePrefix = string.Empty;
		});

		app.UseDeveloperExceptionPage();

		app.UseRouting();

		app.UseCors("CorsPolicy");

		app.UseAuthentication();
		app.UseAuthorization();
		app.MapControllers();

		app.Run();
	}

	/// <summary>
	/// Method responsible for configuring services
	/// </summary>
	/// <param name="services">IServiceCollection parameter</param>
	private static void ConfigureServices(IServiceCollection services)
	{
		services.AddControllers(config =>
		{
			var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
			config.Filters.Add(new AuthorizeFilter(policy));
		});

		services.AddEndpointsApiExplorer();
		services.AddSwaggerGen();

		services.AddCors(options =>
		{
			options.AddPolicy("CorsPolicy", builder => builder
							.AllowAnyOrigin()
							.AllowAnyMethod()
							.AllowAnyHeader()
					);
		});

		services.AddSwaggerGen(c =>
		{
			c.SwaggerDoc("v1",
							new OpenApiInfo
						{
							Title = "API - Ads",
							Version = "v1",
							Description = "Ads REST API. This API provides methods that allow authentication between applications, using its own authentication mechanism following an OAuth-based flow, as well as relevant methods for querying ads data.",
						});
			c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
			{
				In = ParameterLocation.Header,
				Description = @"Access token for API authentication, generated using the ""/api/authentication"" method. Example (send in the request header): Bearer {token}.",
				Name = "Authorization",
				Type = SecuritySchemeType.ApiKey
			});
			c.AddSecurityRequirement(new OpenApiSecurityRequirement
				{
								 {
										 new OpenApiSecurityScheme
										 {
												 Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
										 },
										 Array.Empty<string>()
								 }
				});
			c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "Api.xml"));
		});

		services.AddAutoMapper(typeof(MapperConfiguration));
		services.AddDbContext<AppDbContext>(options => options.UseSqlServer(Domain.Helpers.Configuration.Connection));

		services.AddScoped<IAdsRepository, AdsRepository>();
		services.AddScoped<IAdsService, AdsService>();
	}

	/// <summary>
	/// Configuring authentication
	/// </summary>
	/// <param name="services"></param>
	/// <param name="configuration"></param>
	private static void ConfigureAuthentication(IServiceCollection services, IConfiguration configuration)
	{
		var tokens = configuration.GetSection("Token").GetChildren();
		services.Configure<Tokens>(options =>
		{
			foreach (var item in tokens)
			{
				options.Sistemas.Add(item.Key, Guid.Parse(item.Value));
			}
		});

		var secretKey = configuration.GetSection("JWTKey");
		SymmetricSecurityKey _signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secretKey.Value));

		var jwtAppSettingOptions = configuration.GetSection(nameof(JwtIssuerOptions));

		var tokenValidationParameters = new TokenValidationParameters
		{
			ValidateIssuer = true,
			ValidIssuer = jwtAppSettingOptions[nameof(JwtIssuerOptions.Issuer)],

			ValidateAudience = true,
			ValidAudience = jwtAppSettingOptions[nameof(JwtIssuerOptions.Audience)],

			ValidateIssuerSigningKey = true,
			IssuerSigningKey = _signingKey,

			RequireExpirationTime = true,
			ValidateLifetime = true,

			ClockSkew = TimeSpan.Zero,
		};

		services.AddAuthentication(options =>
		{
			options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
			options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
		}).AddJwtBearer(options =>
		{
			options.TokenValidationParameters = tokenValidationParameters;
		});

		//If more policy is needed
		services.AddAuthorization(options =>
		{
			foreach (var pcv in Policy.Compilar(configuration))
			{
				options.AddPolicy(pcv.Policy, policy => policy.RequireClaim(pcv.ClaimType, pcv.ClaimValue));
			}
		});

		services.AddAuthorization(options =>
		{
			options.AddPolicy(Policy.Search, policy => policy.RequireClaim(Operation.List, "Permitido"));
		});

		services.Configure<JwtIssuerOptions>(options =>
		{
			options.Issuer = jwtAppSettingOptions[nameof(JwtIssuerOptions.Issuer)];
			options.Audience = jwtAppSettingOptions[nameof(JwtIssuerOptions.Audience)];
			options.SigningCredentials = new SigningCredentials(_signingKey, SecurityAlgorithms.HmacSha256);
		});
	}

	#endregion
}

